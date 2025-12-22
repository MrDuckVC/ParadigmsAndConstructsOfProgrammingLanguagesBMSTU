using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BackupSync.Core.Abstractions;
using BackupSync.Core.Models;

namespace BackupSync.Core.Services
{
    /// <summary>
    /// Сервис, отвечающий за этап "Анализа".
    /// Сравнивает файловую структуру Источника и Назначения и выявляет различия (Diffs).
    /// </summary>
    public class SyncAnalyzerService
    {
        private readonly IFileSystemProvider _fsProvider;

        public SyncAnalyzerService(IFileSystemProvider fsProvider)
        {
            _fsProvider = fsProvider;
        }

        /// <summary>
        /// Выполняет сравнение файлов в двух директориях.
        /// </summary>
        /// <param name="job">Объект задания (для получения списка исключений, если нужно).</param>
        /// <param name="sourceRoot">Абсолютный путь к источнику.</param>
        /// <param name="destRoot">Абсолютный путь к назначению.</param>
        /// <returns>Список различий (<see cref="FileDiff"/>).</returns>
        public async Task<List<FileDiff>> AnalyzeAsync(SyncJob job, string sourceRoot, string destRoot)
        {
            // TODO [Future]: Извлечь job.Exclusions и передать их в ScanDirectoryAsync
            var exclusions = new List<string>();

            // Запускаем сканирование параллельно для ускорения
            var sourceTask = _fsProvider.ScanDirectoryAsync(sourceRoot, exclusions);
            var destTask = _fsProvider.ScanDirectoryAsync(destRoot, exclusions);

            await Task.WhenAll(sourceTask, destTask);

            var sourceFiles = await sourceTask;
            var destFiles = await destTask; // Это изменяемый словарь (мы будем удалять из него совпадения)

            var diffs = new List<FileDiff>();

            // 1. Проходим по файлам источника
            foreach (var sourcePair in sourceFiles)
            {
                var path = sourcePair.Key;
                var sourceMeta = sourcePair.Value;
                FileDiff diff;

                // Ищем такой же файл в назначении
                if (destFiles.TryGetValue(path, out var destMeta))
                {
                    // Файл есть и там, и там -> Сравниваем
                    diff = CompareMetadata(path, sourceMeta, destMeta);

                    // Удаляем из списка файлов назначения, чтобы пометить как обработанный
                    destFiles.Remove(path);
                }
                else
                {
                    // Файл есть только в источнике
                    diff = new FileDiff
                    {
                        RelativePath = path,
                        Status = FileStatus.OnlyInSource,
                        SourceMetadata = sourceMeta
                    };
                }
                diffs.Add(diff);
            }

            // 2. Все, что осталось в destFiles — это "сироты" (есть только в Dest)
            foreach (var destPair in destFiles)
            {
                diffs.Add(new FileDiff
                {
                    RelativePath = destPair.Key,
                    Status = FileStatus.OnlyInDest,
                    DestMetadata = destPair.Value
                });
            }

            return diffs;
        }

        /// <summary>
        /// Сравнивает метаданные двух файлов и определяет их статус.
        /// </summary>
        private FileDiff CompareMetadata(string path, FileMetadata source, FileMetadata dest)
        {
            var status = FileStatus.Identical;

            // Разница во времени в секундах
            var timeDiff = (source.LastWriteTimeUtc - dest.LastWriteTimeUtc).TotalSeconds;

            // FAT32 (флешки) имеет точность времени 2 секунды.
            // Если разница меньше 2 секунд, считаем время одинаковым.
            if (Math.Abs(timeDiff) > 2)
            {
                status = timeDiff > 0 ? FileStatus.SourceIsNewer : FileStatus.DestIsNewer;
            }
            else if (source.Size != dest.Size)
            {
                // Если время "совпадает" (в пределах погрешности), но размер разный — считаем Source новее (конфликт/изменение)
                // Можно было бы добавить статус "SizeMismatch", но для простоты пусть будет SourceIsNewer
                status = FileStatus.SourceIsNewer;
            }

            return new FileDiff
            {
                RelativePath = path,
                Status = status,
                SourceMetadata = source,
                DestMetadata = dest
            };
        }
    }
}
