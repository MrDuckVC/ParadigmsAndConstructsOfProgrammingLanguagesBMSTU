using System;
using BackupSync.Core.Abstractions;
using BackupSync.Core.Services;
using BackupSync.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;

namespace BackupSync.UI
{
    /// <summary>
    /// Точка входа в приложение WinUI 3.
    /// Отвечает за инициализацию DI-контейнера и создание главного окна.
    /// </summary>
    public partial class App : Application
    {
        // Ссылка на главное окно (необходима, чтобы сборщик мусора не удалил окно)
        private Window? m_window;

        /// <summary>
        /// Глобальный провайдер сервисов (DI Container).
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        /// Типизированный доступ к текущему экземпляру приложения.
        /// </summary>
        public static new App Current => (App)Application.Current;

        public App()
        {
            this.InitializeComponent();
            Services = ConfigureServices();
        }

        /// <summary>
        /// Настройка внедрения зависимостей (Dependency Injection).
        /// Здесь регистрируются все сервисы, ViewModels и окна.
        /// </summary>
        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // --- 1. Data Layer (Данные и БД) ---
            services.AddDbContext<DatabaseContext>();

            // --- 2. Infrastructure (Системные сервисы) ---
            // WmiService должен быть Singleton, чтобы держать активную подписку на события USB.
            services.AddSingleton<WmiService>();

            // Абстракция файловой системы (используем стандартную System.IO)
            services.AddTransient<IFileSystemProvider, LocalFileSystemProvider>();

            // --- 3. Domain Logic (Бизнес-логика) ---
            services.AddTransient<DeviceService>();
            services.AddTransient<JobService>();
            services.AddTransient<SyncAnalyzerService>();
            services.AddTransient<SyncPolicyService>();
            services.AddTransient<SyncExecutionService>();

            // --- 4. UI Layer (ViewModels & Windows) ---
            // DevicesViewModel делаем Singleton, чтобы список устройств не перезагружался при переключении вкладок.
            services.AddSingleton<DevicesViewModel>();

            // JobsViewModel создается заново при необходимости (Transient).
            services.AddTransient<JobsViewModel>();

            // Главное окно - Singleton (в нашем приложении только одно окно).
            services.AddSingleton<MainWindow>();

            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Вызывается при запуске приложения конечным пользователем.
        /// </summary>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            // Получаем главное окно из DI-контейнера
            m_window = Services.GetService<MainWindow>();

            if (m_window == null)
            {
                throw new InvalidOperationException("Не удалось разрешить зависимость MainWindow из DI-контейнера.");
            }

            m_window.Activate();
        }
    }
}
