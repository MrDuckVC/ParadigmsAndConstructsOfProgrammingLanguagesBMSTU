using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using BackupSync.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BackupSync.Core.Services
{
    /// <summary>
    /// Сервис управления устройствами.
    /// Отвечает за сопоставление физически подключенных дисков (WMI) с сохраненными в БД профилями.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class DeviceService
    {
        private readonly DatabaseContext _db;
        private readonly WmiService _wmi;

        public DeviceService(DatabaseContext dbContext, WmiService wmiService)
        {
            _db = dbContext;
            _wmi = wmiService;
        }

        /// <summary>
        /// Получает сводный статус всех устройств (подключенных и сохраненных).
        /// </summary>
        /// <returns>Список статусов для отображения в UI.</returns>
        public List<DeviceStatus> GetDeviceStatuses()
        {
            // 1. Получаем "живые" данные от WMI (Синхронно, безопасно для STA/MTA)
            var connectedDisks = _wmi.GetLogicalDisks();

            // 2. Получаем сохраненные данные из БД
            var savedDevices = _db.Devices.ToList();

            var allStatuses = new List<DeviceStatus>();

            // Создаем словарь для быстрого поиска подключенных дисков по SerialNumber
            var wmiLookup = connectedDisks.ToDictionary(d => d.VolumeSerialNumber!);

            // 3. Проходим по всем сохраненным устройствам
            foreach (var savedDevice in savedDevices)
            {
                // Пытаемся найти сохраненное устройство среди подключенных
                if (wmiLookup.TryGetValue(savedDevice.VolumeSerialNumber, out var connectedDisk))
                {
                    // А. Устройство сохранено И подключено (Connected & Recognized)
                    allStatuses.Add(new DeviceStatus
                    {
                        IsConnected = true,
                        IsRecognized = true,
                        DeviceId = savedDevice.DeviceId,
                        UserGivenName = savedDevice.UserGivenName,
                        WmiDisk = connectedDisk
                    });

                    // Удаляем из lookup, чтобы пометить как "обработанное"
                    wmiLookup.Remove(savedDevice.VolumeSerialNumber);
                }
                else
                {
                    // Б. Устройство сохранено, но НЕ подключено (Disconnected & Recognized)
                    allStatuses.Add(new DeviceStatus
                    {
                        IsConnected = false,
                        IsRecognized = true,
                        DeviceId = savedDevice.DeviceId,
                        UserGivenName = savedDevice.UserGivenName
                        // WmiDisk = null
                    });
                }
            }

            // 4. Все, что осталось в wmiLookup — это новые, неизвестные устройства
            foreach (var connectedDisk in wmiLookup.Values)
            {
                // В. Устройство подключено, но НЕ сохранено (Connected & Unrecognized)
                allStatuses.Add(new DeviceStatus
                {
                    IsConnected = true,
                    IsRecognized = false,
                    WmiDisk = connectedDisk,
                    // Используем метку тома как временное имя
                    UserGivenName = connectedDisk.VolumeLabel ?? "Новый том"
                });
            }

            return allStatuses;
        }

        /// <summary>
        /// Сохраняет новое физическое устройство в базу данных.
        /// </summary>
        public async Task<Device> AddNewDeviceAsync(LogicalDisk newDisk, string userGivenName)
        {
            var newDevice = new Device
            {
                UserGivenName = userGivenName,
                VolumeSerialNumber = newDisk.VolumeSerialNumber!,
                LastKnownLabel = newDisk.VolumeLabel,
                LastKnownLetter = newDisk.Letter
            };
            _db.Devices.Add(newDevice);
            await _db.SaveChangesAsync();
            return newDevice;
        }

        /// <summary>
        /// Перепривязывает существующий профиль устройства к новому физическому носителю (или тому же после форматирования).
        /// Обновляет SerialNumber, метку и букву.
        /// </summary>
        public async Task ReAssociateDeviceAsync(int lostDeviceId, LogicalDisk newDisk)
        {
            var deviceToUpdate = await _db.Devices.FindAsync(lostDeviceId);
            if (deviceToUpdate == null) return;

            deviceToUpdate.VolumeSerialNumber = newDisk.VolumeSerialNumber!;
            deviceToUpdate.LastKnownLabel = newDisk.VolumeLabel;
            deviceToUpdate.LastKnownLetter = newDisk.Letter;

            _db.Devices.Update(deviceToUpdate);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Удаляет устройство из базы данных.
        /// Каскадно удаляет все связанные задания (SyncJobs).
        /// </summary>
        public async Task ForgetDeviceAsync(int deviceId)
        {
            var deviceToForget = await _db.Devices
                .Include(d => d.SyncJobs)
                .FirstOrDefaultAsync(d => d.DeviceId == deviceId);

            if (deviceToForget == null) return;

            // Удаляем задания явно (хотя EF Core Cascade Delete может сделать это сам, явное удаление безопаснее для логики)
            if (deviceToForget.SyncJobs.Any())
            {
                _db.SyncJobs.RemoveRange(deviceToForget.SyncJobs);
            }

            _db.Devices.Remove(deviceToForget);
            await _db.SaveChangesAsync();
        }
    }
}
