using System;
using System.Collections.ObjectModel;
using System.Linq;
using BackupSync.Core.Models;
using Microsoft.UI.Xaml.Controls;

// Псевдоним для удобства
using DeviceModel = BackupSync.Core.Models.Device;

namespace BackupSync.UI.Views.Dialogs
{
    /// <summary>
    /// Диалог создания нового задания синхронизации.
    /// Собирает данные формы и возвращает их через публичные свойства.
    /// </summary>
    public sealed partial class CreateJobDialog : ContentDialog
    {
        // --- Публичные свойства для доступа к результатам ---

        public string JobName => JobNameTextBox.Text.Trim();
        public string SourcePath => SourcePathTextBox.Text.Trim();
        public string DestPath => DestPathTextBox.Text.Trim();

        /// <summary>
        /// ID выбранного устройства. Возвращает 0, если устройство не выбрано.
        /// </summary>
        public int SelectedDeviceId => (DeviceComboBox.SelectedItem as DeviceModel)?.DeviceId ?? 0;

        /// <summary>
        /// Выбранный тип задания. Парсится из Tag выбранного ComboBoxItem.
        /// </summary>
        public JobType SelectedJobType => ParseEnumFromTag<JobType>(JobTypeComboBox);

        /// <summary>
        /// Выбранная политика "сирот".
        /// </summary>
        public OrphanPolicy SelectedOrphanPolicy => ParseEnumFromTag<OrphanPolicy>(OrphanPolicyComboBox);

        /// <summary>
        /// Выбранная политика конфликтов.
        /// </summary>
        public ConflictPolicy SelectedConflictPolicy => ParseEnumFromTag<ConflictPolicy>(ConflictPolicyComboBox);


        public CreateJobDialog(ObservableCollection<DeviceModel> recognizedDevices)
        {
            this.InitializeComponent();

            // 1. Заполняем список устройств
            DeviceComboBox.ItemsSource = recognizedDevices;

            // Если есть устройства, выбираем первое по умолчанию
            if (recognizedDevices.Any())
            {
                DeviceComboBox.SelectedIndex = 0;
            }

            // 2. Подписываемся на клик по кнопке "Создать"
            this.PrimaryButtonClick += CreateJobDialog_PrimaryButtonClick;
        }

        private void CreateJobDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            bool hasError = false;

            // --- Валидация Названия ---
            if (string.IsNullOrWhiteSpace(JobName))
            {
                JobNameTextBox.Header = "Название задания (Обязательно!)";
                hasError = true;
            }
            else
            {
                JobNameTextBox.Header = "Название задания"; // Сброс ошибки
            }

            // --- Валидация Источника ---
            if (string.IsNullOrWhiteSpace(SourcePath))
            {
                SourcePathTextBox.Header = "Папка на компьютере (Обязательно!)";
                hasError = true;
            }
            else
            {
                SourcePathTextBox.Header = "Папка на компьютере (Источник)";
            }

            // --- Валидация Устройства ---
            if (SelectedDeviceId == 0)
            {
                DeviceComboBox.Header = "Целевое устройство (Не выбрано!)";
                hasError = true;
            }
            else
            {
                DeviceComboBox.Header = "Целевое устройство (USB)";
            }

            // --- Валидация Назначения (опционально, но желательно) ---
            if (string.IsNullOrWhiteSpace(DestPath))
            {
                DestPathTextBox.Header = "Папка на устройстве (Обязательно!)";
                hasError = true;
            }
            else
            {
                DestPathTextBox.Header = "Папка на устройстве (Назначение)";
            }

            // Если есть ошибки, предотвращаем закрытие диалога
            if (hasError)
            {
                args.Cancel = true;
            }
        }

        /// <summary>
        /// Вспомогательный метод для получения Enum из свойства Tag элемента ComboBox.
        /// </summary>
        private T ParseEnumFromTag<T>(ComboBox comboBox) where T : struct, Enum
        {
            if (comboBox.SelectedItem is ComboBoxItem item && item.Tag is string tagString)
            {
                if (Enum.TryParse<T>(tagString, out var result))
                {
                    return result;
                }
            }
            // Значение по умолчанию (первый элемент Enum), если парсинг не удался
            return default;
        }
    }
}
