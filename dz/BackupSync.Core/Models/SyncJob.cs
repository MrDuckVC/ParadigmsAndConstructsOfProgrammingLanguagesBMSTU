using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BackupSync.Core.Models
{
    /// <summary>
    /// Сущность, представляющая одно задание синхронизации (профиль).
    /// Содержит информацию о том, ЧТО, ОТКУДА, КУДА и КАК копировать.
    /// </summary>
    public class SyncJob
    {
        /// <summary>
        /// Уникальный идентификатор задания (Primary Key).
        /// </summary>
        [Key]
        public int SyncJobId { get; set; }

        /// <summary>
        /// Название задания для отображения в списке.
        /// Пример: "Бэкап фото", "Синхронизация работы".
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string JobName { get; set; } = string.Empty;

        /// <summary>
        /// Абсолютный путь к исходной папке на компьютере.
        /// Пример: "D:\Projects\MyProject"
        /// </summary>
        [Required]
        [MaxLength(1024)] // Разумный лимит для путей
        public string SourcePath { get; set; } = string.Empty;

        /// <summary>
        /// Относительный путь к папке назначения на внешнем носителе (от корня диска).
        /// Не включает букву диска, так как она может меняться.
        /// Пример: "Backups\MyProject" (в итоге станет "E:\Backups\MyProject").
        /// </summary>
        [Required]
        [MaxLength(1024)]
        public string DestPath { get; set; } = string.Empty;

        // --- Связи (Relationships) ---

        /// <summary>
        /// Внешний ключ (Foreign Key) на устройство, к которому привязано задание.
        /// </summary>
        public int DeviceId { get; set; }

        /// <summary>
        /// Навигационное свойство: Устройство (флешка/диск), с которым работает это задание.
        /// </summary>
        public Device Device { get; set; } = null!;

        // --- Политики (Rules) ---

        /// <summary>
        /// Тип синхронизации (Односторонняя / Двусторонняя).
        /// </summary>
        [Required]
        public JobType JobType { get; set; }

        /// <summary>
        /// Политика обработки файлов, которые есть в Назначении, но нет в Источнике.
        /// </summary>
        [Required]
        public OrphanPolicy OrphanPolicy { get; set; }

        /// <summary>
        /// Политика разрешения конфликтов (когда файл изменен с обеих сторон).
        /// </summary>
        [Required]
        public ConflictPolicy ConflictPolicy { get; set; }

        /// <summary>
        /// Навигационное свойство: Список правил исключения файлов (фильтры).
        /// </summary>
        public List<JobExclusion> Exclusions { get; set; } = new List<JobExclusion>();
    }
}
