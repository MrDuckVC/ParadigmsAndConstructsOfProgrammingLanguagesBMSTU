using System;
using BackupSync.Core.Models;
using BackupSync.UI.ViewModels;
using BackupSync.UI.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace BackupSync.UI.Views
{
    /// <summary>
    /// Страница управления устройствами.
    /// Отвечает за отображение списка и обработку UI-событий (открытие диалогов).
    /// </summary>
    public sealed partial class DevicesPage : Page
    {
        /// <summary>
        /// ViewModel страницы. Свойство доступно для x:Bind в XAML.
        /// </summary>
        public DevicesViewModel ViewModel { get; }

        public DevicesPage()
        {
            this.InitializeComponent();

            // Получаем экземпляр ViewModel из DI-контейнера
            ViewModel = App.Current.Services.GetService<DevicesViewModel>()
                        ?? throw new InvalidOperationException("DevicesViewModel не зарегистрирована в DI.");

            // Устанавливаем DataContext для стандартных Binding'ов
            this.DataContext = ViewModel;
        }

        /// <summary>
        /// Обработчик кнопки "Сохранить" (Добавить новое устройство).
        /// </summary>
        private async void AddDeviceButton_Click(object sender, RoutedEventArgs e)
        {
            // Извлекаем объект DeviceStatus из свойства Tag кнопки
            if ((sender as Button)?.Tag is not DeviceStatus deviceToAdd)
                return;

            // Создаем диалог
            var dialog = new AddDeviceDialog(deviceToAdd.DisplayName);

            // [ВАЖНО] XamlRoot обязателен для WinUI 3
            dialog.XamlRoot = this.XamlRoot;

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary && !string.IsNullOrEmpty(dialog.DeviceName))
            {
                // Передаем данные во ViewModel
                await ViewModel.AddDeviceAsync(deviceToAdd, dialog.DeviceName);
            }
        }

        /// <summary>
        /// Обработчик кнопки "Найти диск..." (Перепривязать потерянное устройство).
        /// </summary>
        private async void ReAssociateButton_Click(object sender, RoutedEventArgs e)
        {
            // Извлекаем "потерянное" устройство из Tag
            if ((sender as Button)?.Tag is not DeviceStatus lostDevice)
                return;

            // Создаем диалог, передавая список кандидатов (неопознанных дисков)
            var dialog = new ReAssociateDialog(lostDevice, ViewModel.UnrecognizedDisks);

            dialog.XamlRoot = this.XamlRoot;

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary && dialog.SelectedDevice != null)
            {
                // Передаем данные во ViewModel
                await ViewModel.ReAssociateDeviceAsync(lostDevice, dialog.SelectedDevice);
            }
        }
    }
}
