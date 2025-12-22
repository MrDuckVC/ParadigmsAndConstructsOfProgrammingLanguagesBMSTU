using System.Collections.Generic;
using System.Diagnostics;
using BackupSync.Core.Models;

namespace BackupSync.Core.Services
{
    /// <summary>
    /// Сервис, отвечающий за этап "Принятия решений" (Policy Engine).
    /// Принимает список различий (Diffs) и настройки задания (Job),
    /// и на их основе формирует список действий (Actions).
    /// </summary>
    public class SyncPolicyService
    {
        /// <summary>
        /// Применяет политики синхронизации к списку различий.
        /// </summary>
        /// <param name="diffs">Список найденных различий.</param>
        /// <param name="job">Задание с настройками политик (Orphan, Conflict).</param>
        /// <returns>Список действий, которые нужно выполнить.</returns>
        public List<SyncAction> ApplyPolicies(List<FileDiff> diffs, SyncJob job)
        {
            var actions = new List<SyncAction>();

            Debug.WriteLine("--- Движок Политик: Начало ---");

            foreach (var diff in diffs)
            {
                var actionType = SyncActionType.Skip; // По умолчанию - ничего не делать

                // Логирование для отладки
                Debug.WriteLine($" -> Вход: {diff.Status} | {diff.RelativePath}");

                switch (diff.Status)
                {
                    case FileStatus.OnlyInSource:
                        // Новый файл на источнике -> Копируем в назначение
                        actionType = SyncActionType.CopyFromSourceToDest;
                        Debug.WriteLine("      -> Решение: CopyFromSourceToDest");
                        break;

                    case FileStatus.SourceIsNewer:
                        // Файл изменился на источнике -> Обновляем назначение
                        actionType = SyncActionType.CopyFromSourceToDest;
                        Debug.WriteLine("      -> Решение: CopyFromSourceToDest (Overwrite)");
                        break;

                    case FileStatus.Identical:
                        // Файлы одинаковые -> Пропускаем
                        actionType = SyncActionType.Skip;
                        Debug.WriteLine("      -> Решение: Skip (Identical)");
                        break;

                    case FileStatus.OnlyInDest:
                        // Файл есть только в назначении ("Сирота") -> Применяем политику сирот
                        actionType = ApplyOrphanPolicy(job.OrphanPolicy);
                        Debug.WriteLine($"      -> Решение (Сирота): {actionType}");
                        break;

                    case FileStatus.DestIsNewer:
                        // Файл в назначении новее ("Конфликт" для One-Way, норма для Two-Way) -> Применяем политику конфликтов
                        actionType = ApplyConflictPolicy(job.ConflictPolicy);
                        Debug.WriteLine($"      -> Решение (Конфликт): {actionType}");
                        break;

                    default:
                        Debug.WriteLine("      -> Решение: (UNKNOWN STATUS!)");
                        break;
                }

                actions.Add(new SyncAction { ActionType = actionType, Diff = diff });
            }

            Debug.WriteLine("--- Движок Политик: Конец ---");
            return actions;
        }

        /// <summary>
        /// Определяет действие для "сирот" на основе выбранной политики.
        /// </summary>
        private SyncActionType ApplyOrphanPolicy(OrphanPolicy policy)
        {
            return policy switch
            {
                OrphanPolicy.Ask => SyncActionType.AskUser,
                OrphanPolicy.KeepOnRemote => SyncActionType.Skip,
                OrphanPolicy.AutoDeleteOnRemote => SyncActionType.DeleteOnDest,
                // Для Two-Way режима: сирота на Dest = новый файл
                OrphanPolicy.AutoCopyToLocal => SyncActionType.CopyFromDestToSource,
                _ => SyncActionType.AskUser
            };
        }

        /// <summary>
        /// Определяет действие для конфликтов (Dest is Newer) на основе выбранной политики.
        /// </summary>
        private SyncActionType ApplyConflictPolicy(ConflictPolicy policy)
        {
            return policy switch
            {
                ConflictPolicy.Ask => SyncActionType.AskUser,
                ConflictPolicy.KeepLocal => SyncActionType.CopyFromSourceToDest, // Перезаписать Dest старой версией с Source
                ConflictPolicy.KeepRemote => SyncActionType.CopyFromDestToSource, // Обновить Source новой версией с Dest
                ConflictPolicy.KeepNewest => SyncActionType.CopyFromDestToSource, // Dest и так новее
                _ => SyncActionType.AskUser
            };
        }
    }
}
