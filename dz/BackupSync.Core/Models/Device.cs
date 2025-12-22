using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BackupSync.Core.Models
{
    /// <summary>
    /// Сущность, представляющая физический внешний носитель (USB-флешку, HDD), сохраненный в базе данных.
    /// Используется для привязки задач синхронизации к конкретному "железу".
    /// </summary>
    public class Device
    {
        /// <summary>
        /// Уникальный идентификатор устройства в БД (Primary Key).
        /// </summary>
        [Key]
        public int DeviceId { get; set; }

        /// <summary>
        /// Пользовательское имя устройства (например, "Рабочая флешка").
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string UserGivenName { get; set; } = string.Empty;

        /// <summary>
        /// Серийный номер тома (Volume Serial Number), получаемый через WMI (например, "E4B0-1A2C").
        /// Является основным уникальным идентификатором физического носителя.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string VolumeSerialNumber { get; set; } = string.Empty;

        /// <summary>
        /// Последняя известная метка тома (например, "MY_BACKUP").
        /// Используется для UI, когда устройство отключено.
        /// </summary>
        [MaxLength(100)]
        public string? LastKnownLabel { get; set; }

        /// <summary>
        /// Последняя известная буква диска (например, "E:").
        /// Примечание: Буква может меняться при переподключении.
        /// </summary>
        [MaxLength(10)]
        public string? LastKnownLetter { get; set; }

        /// <summary>
        /// Навигационное свойство: Список задач синхронизации, привязанных к этому устройству.
        /// </summary>
        public List<SyncJob> SyncJobs { get; set; } = new List<SyncJob>();
    }
}
