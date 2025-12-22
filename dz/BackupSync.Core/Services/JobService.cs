using System.Collections.Generic;
using System.Threading.Tasks;
using BackupSync.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BackupSync.Core.Services
{
    /// <summary>
    /// Сервис для управления заданиями синхронизации (CRUD).
    /// Отвечает за создание, чтение и удаление профилей бэкапа в базе данных.
    /// </summary>
    public class JobService
    {
        private readonly DatabaseContext _db;

        public JobService(DatabaseContext dbContext)
        {
            _db = dbContext;
        }

        /// <summary>
        /// Получает список всех сохраненных заданий.
        /// Загружает (Eager Loading) связанные данные об Устройстве для отображения в UI.
        /// </summary>
        public async Task<List<SyncJob>> GetJobsAsync()
        {
            return await _db.SyncJobs
                .Include(job => job.Device) // Подтягиваем имя устройства
                .ToListAsync();
        }

        /// <summary>
        /// Создает и сохраняет новое задание в БД.
        /// </summary>
        public async Task<SyncJob> CreateJobAsync(
            string jobName,
            string sourcePath,
            string destPath,
            int deviceId,
            JobType jobType,
            OrphanPolicy orphanPolicy,
            ConflictPolicy conflictPolicy)
        {
            var newJob = new SyncJob
            {
                JobName = jobName,
                SourcePath = sourcePath,
                DestPath = destPath,
                DeviceId = deviceId,
                JobType = jobType,
                OrphanPolicy = orphanPolicy,
                ConflictPolicy = conflictPolicy
                // Exclusions по умолчанию пуст
            };

            _db.SyncJobs.Add(newJob);
            await _db.SaveChangesAsync();

            // Явно подгружаем связанное устройство, чтобы возвращаемый объект
            // содержал всю информацию для немедленного отображения в UI.
            await _db.Entry(newJob).Reference(j => j.Device).LoadAsync();

            return newJob;
        }

        /// <summary>
        /// Удаляет задание по ID.
        /// Связанные исключения (Exclusions) удаляются автоматически (Cascade Delete в БД).
        /// </summary>
        public async Task DeleteJobAsync(int jobId)
        {
            var jobToDelete = await _db.SyncJobs.FindAsync(jobId);

            if (jobToDelete == null) return;

            _db.SyncJobs.Remove(jobToDelete);
            await _db.SaveChangesAsync();
        }

        // TODO [Future]: Реализовать метод UpdateJobAsync для редактирования существующих заданий.
    }
}
