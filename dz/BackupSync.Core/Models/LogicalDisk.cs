namespace BackupSync.Core.Models
{
    /// <summary>
    /// Модель данных (DTO), представляющая логический диск, обнаруженный в системе через WMI.
    /// Используется для передачи информации о физически подключенных устройствах.
    /// </summary>
    public class LogicalDisk
    {
        /// <summary>
        /// Буква диска (свойство DeviceID из WMI).
        /// Пример: "E:"
        /// </summary>
        public string Letter { get; set; } = string.Empty;

        /// <summary>
        /// Метка тома (название диска). Может быть null, если метка не задана.
        /// Пример: "MY_BACKUP"
        /// </summary>
        public string? VolumeLabel { get; set; }

        /// <summary>
        /// Серийный номер тома (Volume Serial Number).
        /// Генерируется при форматировании. Используется нами как уникальный идентификатор устройства.
        /// Пример: "E4B0-1A2C"
        /// </summary>
        public string? VolumeSerialNumber { get; set; }
    }
}
