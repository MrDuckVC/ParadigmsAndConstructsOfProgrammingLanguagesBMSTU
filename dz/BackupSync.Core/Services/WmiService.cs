using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using BackupSync.Core.Models;

namespace BackupSync.Core.Services
{
    /// <summary>
    /// Сервис для взаимодействия с Windows Management Instrumentation (WMI).
    /// Отвечает за обнаружение физических дисков и отслеживание событий подключения/отключения USB.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class WmiService : IDisposable
    {
        private readonly ManagementEventWatcher _usbInsertWatcher;
        private readonly ManagementEventWatcher _usbRemoveWatcher;

        /// <summary>
        /// Событие, возникающее при изменении состава дисков (подключение или извлечение).
        /// </summary>
        public event Action? DisksChanged;

        /// <summary>
        /// Конструктор сервиса. Инициализирует и запускает мониторинг событий WMI.
        /// </summary>
        public WmiService()
        {
            // Подписка на событие ПОДКЛЮЧЕНИЯ (__InstanceCreationEvent) для LogicalDisk
            var insertQuery = new WqlEventQuery(
                "SELECT * FROM __InstanceCreationEvent WITHIN 2 " +
                "WHERE TargetInstance ISA 'Win32_LogicalDisk'");
            _usbInsertWatcher = new ManagementEventWatcher(insertQuery);
            _usbInsertWatcher.EventArrived += OnDisksChanged;
            _usbInsertWatcher.Start();

            // Подписка на событие ОТКЛЮЧЕНИЯ (__InstanceDeletionEvent) для LogicalDisk
            var removeQuery = new WqlEventQuery(
                "SELECT * FROM __InstanceDeletionEvent WITHIN 2 " +
                "WHERE TargetInstance ISA 'Win32_LogicalDisk'");
            _usbRemoveWatcher = new ManagementEventWatcher(removeQuery);
            _usbRemoveWatcher.EventArrived += OnDisksChanged;
            _usbRemoveWatcher.Start();
        }

        /// <summary>
        /// Обработчик событий WMI.
        /// </summary>
        private void OnDisksChanged(object sender, EventArrivedEventArgs e)
        {
            // Используем небольшую задержку (Debounce), так как WMI может прислать
            // несколько событий подряд для одного физического подключения,
            // а системе нужно время, чтобы смонтировать букву диска.
            Task.Delay(500).ContinueWith(_ => { DisksChanged?.Invoke(); });
        }

        /// <summary>
        /// Получает список всех активных логических дисков в системе (только съемные и локальные).
        /// </summary>
        public List<LogicalDisk> GetLogicalDisks()
        {
            var disks = new List<LogicalDisk>();
            try
            {
                // DriveType=2 (Removable/USB), DriveType=3 (Local HDD/SSD)
                var query = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_LogicalDisk WHERE DriveType = 2 OR DriveType = 3");

                foreach (ManagementObject disk in query.Get().Cast<ManagementObject>())
                {
                    var diskInfo = new LogicalDisk
                    {
                        Letter = disk["DeviceID"]?.ToString() ?? string.Empty,
                        VolumeLabel = disk["VolumeName"]?.ToString(),
                        VolumeSerialNumber = disk["VolumeSerialNumber"]?.ToString()
                    };

                    // Фильтруем пустые или системные разделы без серийника
                    if (!string.IsNullOrEmpty(diskInfo.Letter) &&
                        !string.IsNullOrEmpty(diskInfo.VolumeSerialNumber))
                    {
                        disks.Add(diskInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WMI Error (Sync): {ex.Message}");
            }
            return disks;
        }

        /// <summary>
        /// Освобождение ресурсов (остановка Watcher'ов).
        /// Вызывается DI-контейнером при завершении работы приложения.
        /// </summary>
        public void Dispose()
        {
            _usbInsertWatcher?.Stop();
            _usbInsertWatcher?.Dispose();

            _usbRemoveWatcher?.Stop();
            _usbRemoveWatcher?.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
