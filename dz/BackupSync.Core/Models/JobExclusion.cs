using System.ComponentModel.DataAnnotations;

namespace BackupSync.Core.Models
{
    /// <summary>
    /// Сущность, представляющая правило исключения файлов или папок из процесса синхронизации.
    /// Позволяет игнорировать временные файлы, системные директории и т.д.
    /// </summary>
    public class JobExclusion
    {
        /// <summary>
        /// Уникальный идентификатор правила (Primary Key).
        /// </summary>
        [Key]
        public int JobExclusionId { get; set; }

        /// <summary>
        /// Шаблон исключения. Поддерживает простые маски.
        /// Примеры: "*.tmp", "node_modules/", ".git", "Thumbs.db".
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Pattern { get; set; } = string.Empty;

        // --- Связи (Relationships) ---

        /// <summary>
        /// Внешний ключ (Foreign Key) на задание синхронизации.
        /// </summary>
        public int SyncJobId { get; set; }

        /// <summary>
        /// Навигационное свойство: Задание, к которому относится это правило.
        /// </summary>
        public SyncJob SyncJob { get; set; } = null!;
    }
}
