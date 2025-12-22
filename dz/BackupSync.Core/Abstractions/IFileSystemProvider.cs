using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackupSync.Core.Abstractions
{
    /// <summary>
    /// Модель, содержащая основные метаданные файла, необходимые для сравнения.
    /// Используется для минимизации операций ввода-вывода (не читаем контент файла).
    /// </summary>
    public class FileMetadata
    {
        /// <summary>
        /// Относительный путь к файлу (от корня сканирования).
        /// </summary>
        public string RelativePath { get; set; } = string.Empty;

        /// <summary>
        /// Размер файла в байтах.
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Время последнего изменения файла (в UTC).
        /// </summary>
        public DateTime LastWriteTimeUtc { get; set; }
    }

    /// <summary>
    /// Абстракция для операций с файловой системой.
    /// Позволяет абстрагироваться от конкретного источника (локальный диск, сетевая папка, VSS-снимок).
    /// </summary>
    public interface IFileSystemProvider
    {
        /// <summary>
        /// Асинхронно сканирует указанную директорию и возвращает словарь метаданных всех найденных файлов.
        /// </summary>
        /// <param name="rootPath">Корневой путь для сканирования (абсолютный).</param>
        /// <param name="exclusions">Список шаблонов имен файлов/папок для исключения из сканирования.</param>
        /// <returns>
        /// Словарь, где:
        /// Key - относительный путь к файлу,
        /// Value - объект <see cref="FileMetadata"/>.
        /// </returns>
        /// <remarks>
        /// TODO [Future]: Реализовать логику фильтрации на основе параметра exclusions.
        /// TODO [VSS]: При работе с VSS здесь будет передаваться путь к смонтированному снимку.
        /// </remarks>
        Task<Dictionary<string, FileMetadata>> ScanDirectoryAsync(string rootPath, List<string> exclusions);

        /// <summary>
        /// Асинхронно копирует один файл из источника в назначение.
        /// Если файл назначения существует, он должен быть перезаписан.
        /// </summary>
        /// <param name="sourceFullPath">Полный путь к файлу-источнику.</param>
        /// <param name="destFullPath">Полный путь к файлу-назначения.</param>
        Task CopyFileAsync(string sourceFullPath, string destFullPath);

        /// <summary>
        /// Асинхронно удаляет файл по указанному пути.
        /// Если файл не существует, метод должен завершаться успешно (идемпотентность).
        /// </summary>
        /// <param name="fullPath">Полный путь к удаляемому файлу.</param>
        Task DeleteFileAsync(string fullPath);
    }
}
