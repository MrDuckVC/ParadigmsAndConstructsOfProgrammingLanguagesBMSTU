using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BackupSync.Core.Models;
using BackupSync.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;

namespace BackupSync.UI.ViewModels
{
    /// <summary>
    /// ViewModel для страницы управления устройствами.
    /// Отвечает за отображение списка дисков, добавление новых и удаление старых.
    /// </summary>
    public partial class DevicesViewModel : ObservableObject
    {
        private readonly DeviceService _deviceService;
        private readonly WmiService _wmiService;

        // Очередь диспетчера для маршалинга вызовов в UI-поток
        private readonly DispatcherQueue _dispatcherQueue;

        /// <summary>
        /// Коллекция неопознанных (новых) подключенных дисков.
        /// </summary>
        public ObservableCollection<DeviceStatus> UnrecognizedDisks { get; } = new();

        /// <summary>
        /// Основная коллекция устройств (все известные + подключенные).
        /// </summary>
        public ObservableCollection<DeviceStatus> Devices { get; } = new();

        // Ручная реализация свойства для избежания потенциальных AOT-предупреждений
        private bool _hasUnrecognizedDisks;

        /// <summary>
        /// Флаг: есть ли подключенные, но не добавленные диски (для отображения плашки "Найден новый диск").
        /// </summary>
        public bool HasUnrecognizedDisks
        {
            get => _hasUnrecognizedDisks;
            set => SetProperty(ref _hasUnrecognizedDisks, value);
        }

        public DevicesViewModel(DeviceService deviceService, WmiService wmiService)
        {
            _deviceService = deviceService;
            _wmiService = wmiService;

            // Получаем диспетчер текущего (UI) потока
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

            // Подписываемся на события WMI (вставка/извлечение флешки)
            _wmiService.DisksChanged += OnDisksChanged;

            // Первичная загрузка
            LoadDevices();
        }

        /// <summary>
        /// Обработчик события изменения дисков от WMI.
        /// </summary>
        private void OnDisksChanged()
        {
            // WMI вызывает это событие из фонового потока.
            // Нам нужно обновить ObservableCollection, что можно делать ТОЛЬКО в UI-потоке.
            _dispatcherQueue.TryEnqueue(() =>
            {
                Debug.WriteLine("WMI Event Received! Reloading devices...");
                LoadDevices();
            });
        }

        /// <summary>
        /// Синхронно перезагружает список устройств из сервиса.
        /// </summary>
        public void LoadDevices()
        {
            Devices.Clear();
            UnrecognizedDisks.Clear();

            // Получаем актуальный список статусов (WMI + DB)
            var statuses = _deviceService.GetDeviceStatuses();

            // Сортируем: сначала подключенные
            foreach (var status in statuses.OrderByDescending(d => d.IsConnected))
            {
                Devices.Add(status);

                // Если диск подключен, но не опознан — добавляем в список "Новых"
                if (status.IsConnected && !status.IsRecognized)
                {
                    UnrecognizedDisks.Add(status);
                }
            }

            HasUnrecognizedDisks = UnrecognizedDisks.Any();
        }

        /// <summary>
        /// Асинхронно добавляет новое устройство в базу данных.
        /// </summary>
        public async Task AddDeviceAsync(DeviceStatus deviceToAdd, string newName)
        {
            if (deviceToAdd == null || deviceToAdd.WmiDisk == null || string.IsNullOrEmpty(newName)) return;

            try
            {
                await _deviceService.AddNewDeviceAsync(deviceToAdd.WmiDisk, newName);
                LoadDevices(); // Обновляем список, чтобы диск перешел в категорию "Сохраненные"
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error adding device: {ex.Message}");
            }
        }

        /// <summary>
        /// Команда: Удалить ("Забыть") устройство из базы.
        /// </summary>
        [RelayCommand]
        private async Task ForgetDeviceAsync(DeviceStatus deviceToForget)
        {
            if (deviceToForget == null || !deviceToForget.DeviceId.HasValue) return;

            try
            {
                await _deviceService.ForgetDeviceAsync(deviceToForget.DeviceId.Value);
                LoadDevices(); // Обновляем список
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error forgetting device: {ex.Message}");
            }
        }

        /// <summary>
        /// Асинхронно обновляет привязку "потерянного" устройства к новому физическому диску.
        /// </summary>
        public async Task ReAssociateDeviceAsync(DeviceStatus lostDevice, DeviceStatus newDisk)
        {
            if (lostDevice == null || newDisk == null || !lostDevice.DeviceId.HasValue || newDisk.WmiDisk == null) return;

            try
            {
                await _deviceService.ReAssociateDeviceAsync(lostDevice.DeviceId.Value, newDisk.WmiDisk);
                LoadDevices(); // Обновляем список
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error re-associating device: {ex.Message}");
            }
        }
    }
}
