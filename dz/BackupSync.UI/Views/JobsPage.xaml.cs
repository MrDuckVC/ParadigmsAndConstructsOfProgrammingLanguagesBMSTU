using System;
using BackupSync.UI.ViewModels;
using BackupSync.UI.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace BackupSync.UI.Views
{
    /// <summary>
    /// Страница управления заданиями (Jobs).
    /// </summary>
    public sealed partial class JobsPage : Page
    {
        /// <summary>
        /// ViewModel страницы.
        /// </summary>
        public JobsViewModel ViewModel { get; }

        public JobsPage()
        {
            this.InitializeComponent();

            // Получаем ViewModel из DI-контейнера
            ViewModel = App.Current.Services.GetService<JobsViewModel>()
                        ?? throw new InvalidOperationException("JobsViewModel не зарегистрирована в DI.");

            // Устанавливаем DataContext
            this.DataContext = ViewModel;
        }

        /// <summary>
        /// Обработчик кнопки "Создать новое задание".
        /// Открывает диалоговое окно мастера создания задания.
        /// </summary>
        private async void CreateJobButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Создаем диалог, передаем список известных устройств из ViewModel
            var dialog = new CreateJobDialog(ViewModel.RecognizedDevices);

            // 2. Указываем родительское окно (обязательно для WinUI 3)
            dialog.XamlRoot = this.XamlRoot;

            // 3. Показываем диалог
            var result = await dialog.ShowAsync();

            // 4. Если пользователь нажал "Создать" (и валидация внутри диалога прошла успешно)
            if (result == ContentDialogResult.Primary)
            {
                // 5. Передаем данные из диалога во ViewModel для создания задания
                await ViewModel.CreateJobAsync(
                    dialog.JobName,
                    dialog.SourcePath,
                    dialog.DestPath,
                    dialog.SelectedDeviceId,
                    dialog.SelectedJobType,
                    dialog.SelectedOrphanPolicy,
                    dialog.SelectedConflictPolicy
                );
            }
        }
    }
}
