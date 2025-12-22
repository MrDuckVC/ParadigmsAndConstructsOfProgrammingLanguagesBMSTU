using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using BackupSync.Core.Models;
using BackupSync.Core.Services;
using BackupSync.UI.Views.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

// Псевдонимы для разрешения конфликтов имен
using DeviceModel = BackupSync.Core.Models.Device;
using SystemPath = System.IO.Path;

namespace BackupSync.UI.ViewModels
{
    /// <summary>
    /// ViewModel для страницы управления заданиями (Jobs).
    /// Отвечает за создание, удаление и запуск синхронизации.
    /// </summary>
    public partial class JobsViewModel : ObservableObject
    {
        private readonly JobService _jobService;
        private readonly DeviceService _deviceService;
        private readonly SyncAnalyzerService _analyzerService;
        private readonly SyncPolicyService _policyService;
        private readonly SyncExecutionService _executionService;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Список всех сохраненных заданий.
        /// </summary>
        public ObservableCollection<SyncJob> Jobs { get; } = new();

        /// <summary>
        /// Список распознанных устройств (для выпадающего списка при создании задания).
        /// </summary>
        public ObservableCollection<DeviceModel> RecognizedDevices { get; } = new();

        public JobsViewModel(
            JobService jobService,
            DeviceService deviceService,
            SyncAnalyzerService analyzerService,
            SyncPolicyService policyService,
            SyncExecutionService executionService,
            IServiceProvider serviceProvider)
        {
            _jobService = jobService;
            _deviceService = deviceService;
            _analyzerService = analyzerService;
            _policyService = policyService;
            _executionService = executionService;
            _serviceProvider = serviceProvider;

            // Запускаем асинхронную загрузку данных без блокировки конструктора (Fire-and-forget)
            _ = LoadDataAsync();
        }

        /// <summary>
        /// Асинхронно загружает список заданий и доступных устройств.
        /// </summary>
        public async Task LoadDataAsync()
        {
            Jobs.Clear();

            // Асинхронно получаем задания из БД
            var jobs = await _jobService.GetJobsAsync();
            foreach (var job in jobs)
            {
                Jobs.Add(job);
            }

            RecognizedDevices.Clear();

            // Получаем статусы устройств (синхронно, так как WMI/Reconciliation)
            var allDeviceStatuses = _deviceService.GetDeviceStatuses();

            // Фильтруем: нам нужны только те, что уже есть в базе (IsRecognized) и имеют DeviceId
            var recognized = allDeviceStatuses
                .Where(s => s.IsRecognized && s.DeviceId.HasValue)
                .Select(s => new DeviceModel
                {
                    DeviceId = s.DeviceId!.Value,
                    UserGivenName = s.UserGivenName!
                });

            foreach (var device in recognized)
            {
                RecognizedDevices.Add(device);
            }
        }

        /// <summary>
        /// Создает новое задание и сохраняет его в БД.
        /// </summary>
        public async Task CreateJobAsync(
            string jobName, string sourcePath, string destPath, int deviceId,
            JobType jobType, OrphanPolicy orphanPolicy, ConflictPolicy conflictPolicy)
        {
            var newJob = await _jobService.CreateJobAsync(
                jobName, sourcePath, destPath, deviceId,
                jobType, orphanPolicy, conflictPolicy);

            Jobs.Add(newJob);
        }

        /// <summary>
        /// Удаляет выбранное задание.
        /// </summary>
        [RelayCommand]
        private async Task DeleteJobAsync(SyncJob? jobToDelete)
        {
            if (jobToDelete == null) return;

            await _jobService.DeleteJobAsync(jobToDelete.SyncJobId);
            Jobs.Remove(jobToDelete);
        }

        /// <summary>
        /// Запускает процесс синхронизации для выбранного задания.
        /// Состоит из 3 этапов: Анализ -> Политики -> Выполнение.
        /// </summary>
        [SupportedOSPlatform("windows")]
        [RelayCommand]
        private async Task SyncJobAsync(SyncJob? jobToSync)
        {
            if (jobToSync == null) return;

            Debug.WriteLine($"--- 🚀 НАЧАЛО СИНХРОНИЗАЦИИ: {jobToSync.JobName} ---");

            // 1. Получаем свежую копию задания из БД (чтобы иметь актуальные пути и политики)
            var job = (await _jobService.GetJobsAsync())
                .FirstOrDefault(j => j.SyncJobId == jobToSync.SyncJobId);

            if (job == null)
            {
                Debug.WriteLine($"ОШИБКА: Не удалось найти задание {jobToSync.SyncJobId} в БД.");
                return;
            }

            // 2. Проверяем подключение устройства
            var deviceStatus = _deviceService.GetDeviceStatuses()
                .FirstOrDefault(d => d.DeviceId == job.DeviceId && d.IsConnected);

            if (deviceStatus == null)
            {
                Debug.WriteLine($"ОШИБКА: Устройство '{job.Device.UserGivenName}' не подключено.");
                // TODO: Показать пользователю MessageBox
                return;
            }

            // 3. Формируем пути
            string sourceRoot = job.SourcePath;
            // Убираем ведущие слеши, чтобы Combine сработал корректно
            string cleanDestPath = job.DestPath.TrimStart('\\', '/');
            string destRoot = SystemPath.Combine(deviceStatus.Letter, cleanDestPath);

            try
            {
                // 4. ЭТАП 1: Анализ (Scan & Compare)
                var diffs = await _analyzerService.AnalyzeAsync(job, sourceRoot, destRoot);
                Debug.WriteLine($"Анализ завершен. Найдено {diffs.Count} расхождений.");

                // 5. ЭТАП 2: Применение политик (Decision Making)
                var actions = _policyService.ApplyPolicies(diffs, job);
                var actionsToExecute = actions.Where(a => a.ActionType != SyncActionType.Skip).ToList();

                // 6. ЭТАП 3: Выполнение "безопасных" действий
                Debug.WriteLine("Передача 'безопасных' команд 'Исполнителю'...");
                var unresolvedConflicts = await _executionService.ExecuteAsync(actionsToExecute, sourceRoot, destRoot);
                Debug.WriteLine("'Исполнитель' завершил 'безопасные' действия.");

                // 7. Обработка конфликтов (если остались)
                if (unresolvedConflicts.Any())
                {
                    Debug.WriteLine($"'Исполнитель' вернул {unresolvedConflicts.Count} конфликтов (AskUser). Показ диалога...");

                    var dialog = new ConflictResolutionDialog(unresolvedConflicts);

                    // Получаем MainWindow для привязки диалога (WinUI 3 требование)
                    var mainWindow = _serviceProvider.GetService<MainWindow>();
                    if (mainWindow == null) throw new NullReferenceException("Не удалось получить MainWindow");

                    dialog.XamlRoot = mainWindow.Content.XamlRoot;

                    var result = await dialog.ShowAsync();

                    if (result == ContentDialogResult.Primary)
                    {
                        var resolvedActions = dialog.ResolvedActions;
                        Debug.WriteLine($"Пользователь решил {resolvedActions.Count(a => a.ActionType != SyncActionType.Skip)} конфликтов.");

                        await _executionService.ExecuteAsync(resolvedActions, sourceRoot, destRoot);
                        Debug.WriteLine("'Исполнитель' завершил 'решенные' действия.");
                    }
                    else
                    {
                        Debug.WriteLine("Пользователь отменил разрешение конфликтов.");
                    }
                }
                else
                {
                    Debug.WriteLine("Все действия выполнены, конфликтов нет.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ОШИБКА СИНХРОНИЗАЦИИ: {ex.Message}");
                // TODO: Показать пользователю MessageBox с ошибкой
            }

            Debug.WriteLine($"--- 🏁 ЗАВЕРШЕНИЕ СИНХРОНИЗАЦИИ: {job.JobName} ---");
        }
    }
}
