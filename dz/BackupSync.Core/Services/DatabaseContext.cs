using System;
using System.IO;
using BackupSync.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BackupSync.Core.Services
{
    /// <summary>
    /// Контекст базы данных приложения (SQLite).
    /// Отвечает за соединение с БД, маппинг объектов на таблицы и выполнение запросов.
    /// </summary>
    public class DatabaseContext : DbContext
    {
        // --- Таблицы (DbSets) ---
        // Инициализируем как null!, так как EF Core заполняет их через рефлексию.

        /// <summary>
        /// Таблица устройств (физических носителей).
        /// </summary>
        public DbSet<Device> Devices { get; set; } = null!;

        /// <summary>
        /// Таблица заданий синхронизации.
        /// </summary>
        public DbSet<SyncJob> SyncJobs { get; set; } = null!;

        /// <summary>
        /// Таблица правил исключений для заданий.
        /// </summary>
        public DbSet<JobExclusion> JobExclusions { get; set; } = null!;

        /// <summary>
        /// Полный путь к файлу базы данных (.db).
        /// </summary>
        public string DbPath { get; }

        /// <summary>
        /// Конструктор контекста. Определяет местоположение файла БД.
        /// </summary>
        public DatabaseContext()
        {
            // Используем стандартную папку LocalApplicationData (обычно C:\Users\<User>\AppData\Local).
            // Это гарантирует, что у приложения будут права на запись без прав администратора.
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);

            // Папка приложения: ...\AppData\Local\BackupSync
            var appDataFolder = Path.Join(path, "BackupSync");

            // Гарантируем, что папка существует
            Directory.CreateDirectory(appDataFolder);

            // Итоговый путь: ...\AppData\Local\BackupSync\backupsync.db
            DbPath = Path.Join(appDataFolder, "backupsync.db");
        }

        /// <summary>
        /// Настройка подключения к базе данных.
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // Используем SQLite.
            // TODO [Future]: При необходимости можно добавить логирование SQL-запросов:
            // options.LogTo(Console.WriteLine);
            options.UseSqlite($"Data Source={DbPath}");
        }

        /// <summary>
        /// Настройка схемы базы данных и связей между таблицами (Fluent API).
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- Связь: Device (1) <-> SyncJob (Many) ---
            modelBuilder.Entity<SyncJob>()
                .HasOne(j => j.Device)
                .WithMany(d => d.SyncJobs)
                .HasForeignKey(j => j.DeviceId)
                .OnDelete(DeleteBehavior.Cascade); // При удалении Устройства удаляются все его Задания

            // --- Связь: SyncJob (1) <-> JobExclusion (Many) ---
            modelBuilder.Entity<JobExclusion>()
                .HasOne(e => e.SyncJob)
                .WithMany(j => j.Exclusions)
                .HasForeignKey(e => e.SyncJobId)
                .OnDelete(DeleteBehavior.Cascade); // При удалении Задания удаляются все его Исключения
        }
    }
}
