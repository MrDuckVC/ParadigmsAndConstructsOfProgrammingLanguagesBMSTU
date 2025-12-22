using BackupSync.Core.Abstractions;

namespace BackupSync.Core.Models
{
    // В этом файле собраны типы, описывающие результаты сравнения файлов и принятые решения.

    /// <summary>
    /// Результат сравнения двух файлов (или отсутствия одного из них) на этапе Анализа.
    /// Определяет фактическое состояние файловой системы без привязки к тому, что с этим делать.
    /// </summary>
    public enum FileStatus
    {
        /// <summary>
        /// Файл существует только в источнике (Source).
        /// Обычно требует копирования в назначение.
        /// </summary>
        OnlyInSource,

        /// <summary>
        /// Файл существует только в назначении (Dest).
        /// Называется "Сирота" (Orphan). Может быть удален или оставлен, в зависимости от политики.
        /// </summary>
        OnlyInDest,

        /// <summary>
        /// Файлы существуют в обоих местах и считаются идентичными (совпадают размер и время изменения).
        /// </summary>
        Identical,

        /// <summary>
        /// Файлы существуют в обоих местах, но версия в источнике (Source) новее.
        /// </summary>
        SourceIsNewer,

        /// <summary>
        /// Файлы существуют в обоих местах, но версия в назначении (Dest) новее.
        /// В режиме One-Way Backup это считается конфликтом. В Two-Way Sync это нормальная ситуация.
        /// </summary>
        DestIsNewer,
    }

    /// <summary>
    /// Объект, описывающий конкретное различие между Источником и Назначением для одного пути.
    /// Содержит метаданные обеих сторон (если они есть).
    /// </summary>
    public class FileDiff
    {
        /// <summary>
        /// Относительный путь к файлу (например, "Documents\Report.docx").
        /// </summary>
        public string RelativePath { get; set; } = string.Empty;

        /// <summary>
        /// Статус сравнения (Результат анализа).
        /// </summary>
        public FileStatus Status { get; set; }

        /// <summary>
        /// Метаданные файла из Источника (Source). Null, если файл отсутствует в источнике.
        /// </summary>
        public FileMetadata? SourceMetadata { get; set; }

        /// <summary>
        /// Метаданные файла из Назначения (Dest). Null, если файл отсутствует в назначении.
        /// </summary>
        public FileMetadata? DestMetadata { get; set; }
    }

    /// <summary>
    /// Тип действия, которое необходимо выполнить с файлом (Результат применения Политик).
    /// </summary>
    public enum SyncActionType
    {
        /// <summary>
        /// Скопировать файл из Источника в Назначение (создать новый или перезаписать старый).
        /// </summary>
        CopyFromSourceToDest,

        /// <summary>
        /// Удалить файл из Назначения (Dest).
        /// Применяется для очистки "сирот" в режиме зеркалирования.
        /// </summary>
        DeleteOnDest,

        /// <summary>
        /// Скопировать файл из Назначения в Источник.
        /// Используется только в режиме Двусторонней синхронизации (Two-Way).
        /// </summary>
        CopyFromDestToSource,

        /// <summary>
        /// Удалить файл из Источника.
        /// Опасно. Используется редко, только при полной синхронизации удалений в Two-Way режиме.
        /// </summary>
        DeleteOnSource,

        /// <summary>
        /// Пропустить файл (ничего не делать).
        /// </summary>
        Skip,

        /// <summary>
        /// Автоматическое решение невозможно. Требуется вмешательство пользователя.
        /// </summary>
        AskUser
    }

    /// <summary>
    /// Конкретная инструкция к выполнению. Связывает найденное различие (Diff) с решением (ActionType).
    /// </summary>
    public class SyncAction
    {
        /// <summary>
        /// Тип действия, которое нужно выполнить.
        /// </summary>
        public SyncActionType ActionType { get; set; }

        /// <summary>
        /// Информация о файле и его статусе.
        /// </summary>
        public FileDiff Diff { get; set; } = null!;
    }
}
