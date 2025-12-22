using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BackupSync.Core.Abstractions;

namespace BackupSync.Core.Services
{
    /// <summary>
    /// Реализация провайдера файловой системы, использующая стандартные средства .NET (System.IO).
    /// Работает с локальными дисками и смонтированными сетевыми путями.
    /// </summary>
    public class LocalFileSystemProvider : IFileSystemProvider
    {
        /// <summary>
        /// Сканирует директорию и возвращает метаданные всех файлов.
        /// </summary>
        public Task<Dictionary<string, FileMetadata>> ScanDirectoryAsync(string rootPath, List<string> exclusions)
        {
            var results = new Dictionary<string, FileMetadata>();

            if (!Directory.Exists(rootPath))
            {
                // Если исходной папки нет, возвращаем пустой результат (это не ошибка, просто нечего копировать)
                return Task.FromResult(results);
            }

            // TODO [Future]: Directory.EnumerateFiles падает при ошибке доступа к одной из подпапок (AccessDenied).
            // В будущем нужно заменить на кастомный рекурсивный обход с try-catch.
            var allFiles = Directory.EnumerateFiles(rootPath, "*", SearchOption.AllDirectories);

            foreach (var fullPath in allFiles)
            {
                // Получаем путь относительно корня (например, "SubFolder\File.txt")
                string relativePath = Path.GetRelativePath(rootPath, fullPath);

                // TODO [Logic]: Реализовать проверку исключений (exclusions) здесь
                // if (IsExcluded(relativePath, exclusions)) continue;

                var fileInfo = new FileInfo(fullPath);
                results[relativePath] = new FileMetadata
                {
                    RelativePath = relativePath,
                    Size = fileInfo.Length,
                    LastWriteTimeUtc = fileInfo.LastWriteTimeUtc
                };
            }

            return Task.FromResult(results);
        }

        /// <summary>
        /// Копирует файл безопасно (через временный файл).
        /// </summary>
        public async Task CopyFileAsync(string sourceFullPath, string destFullPath)
        {
            // 1. Создаем папку назначения, если ее нет
            string? destDirectory = Path.GetDirectoryName(destFullPath);
            if (destDirectory != null)
            {
                Directory.CreateDirectory(destDirectory);
            }

            // 2. Используем временный файл для атомарности.
            // Если копирование прервется, у нас не останется "битого" файла с правильным именем.
            string tempFile = destFullPath + ".tmp";

            // Используем FileShare.Read, чтобы не блокировать файл, если его кто-то читает
            await using (var sourceStream = new FileStream(sourceFullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            await using (var tempStream = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await sourceStream.CopyToAsync(tempStream);
            }

            // 3. Переименовываем временный файл в итоговый (с заменой старого)
            File.Move(tempFile, destFullPath, true);
        }

        /// <summary>
        /// Удаляет файл. Идемпотентная операция (не падает, если файла уже нет).
        /// </summary>
        public Task DeleteFileAsync(string fullPath)
        {
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            return Task.CompletedTask;
        }
    }
}
