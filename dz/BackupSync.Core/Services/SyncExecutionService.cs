using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BackupSync.Core.Abstractions;
using BackupSync.Core.Models;

namespace BackupSync.Core.Services
{
    /// <summary>
    /// Сервис, отвечающий за этап "Выполнения" (Execution).
    /// Принимает список действий (Actions) и физически применяет их к файловой системе.
    /// </summary>
    public class SyncExecutionService
    {
        private readonly IFileSystemProvider _fsProvider;

        public SyncExecutionService(IFileSystemProvider fsProvider)
        {
            _fsProvider = fsProvider;
        }

        /// <summary>
        /// Выполняет список действий синхронизации.
        /// Действия, требующие вмешательства пользователя (AskUser), пропускаются и возвращаются в списке.
        /// </summary>
        /// <param name="actions">Список запланированных действий.</param>
        /// <param name="sourceRoot">Корневая папка источника.</param>
        /// <param name="destRoot">Корневая папка назначения.</param>
        /// <returns>Список конфликтов, которые не были разрешены автоматически (AskUser).</returns>
        public async Task<List<SyncAction>> ExecuteAsync(List<SyncAction> actions, string sourceRoot, string destRoot)
        {
            var conflictsToResolve = new List<SyncAction>();

            foreach (var action in actions)
            {
                // Пропускаем действия, которые нельзя выполнить автоматически
                if (action.ActionType == SyncActionType.AskUser)
                {
                    conflictsToResolve.Add(action);
                    continue;
                }

                if (action.ActionType == SyncActionType.Skip)
                {
                    continue;
                }

                var path = action.Diff.RelativePath;

                try
                {
                    switch (action.ActionType)
                    {
                        case SyncActionType.CopyFromSourceToDest:
                            // Копируем Source -> Dest (создание или перезапись)
                            await _fsProvider.CopyFileAsync(
                                Path.Combine(sourceRoot, path),
                                Path.Combine(destRoot, path));
                            break;

                        case SyncActionType.DeleteOnDest:
                            // Удаляем файл на носителе
                            await _fsProvider.DeleteFileAsync(Path.Combine(destRoot, path));
                            break;

                        case SyncActionType.CopyFromDestToSource:
                            // Копируем Dest -> Source (восстановление или новая версия)
                            await _fsProvider.CopyFileAsync(
                                Path.Combine(destRoot, path),
                                Path.Combine(sourceRoot, path));
                            break;

                            // TODO: Реализовать DeleteOnSource, если мы решим это поддерживать
                    }
                }
                catch (Exception ex)
                {
                    // TODO [Log]: Здесь нужно добавить запись в лог ошибок (файл или БД), чтобы пользователь знал о сбоях.
                    System.Diagnostics.Debug.WriteLine($"ОШИБКА выполнения {action.ActionType} для {path}: {ex.Message}");
                }
            }

            return conflictsToResolve;
        }
    }
}
