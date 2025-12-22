using System.Collections.ObjectModel;
using BackupSync.Core.Models;
using Microsoft.UI.Xaml.Controls;

namespace BackupSync.UI.Views.Dialogs
{
    /// <summary>
    /// Диалог выбора нового физического диска для существующего (но отключенного) профиля устройства.
    /// </summary>
    public sealed partial class ReAssociateDialog : ContentDialog
    {
        /// <summary>
        /// Выбранный пользователем новый диск.
        /// Свойство будет заполнено только при успешном нажатии "Сохранить".
        /// </summary>
        public DeviceStatus? SelectedDevice { get; private set; }

        /// <summary>
        /// Конструктор диалога.
        /// </summary>
        /// <param name="lostDevice">Устройство из базы данных, которое потеряло связь (Source).</param>
        /// <param name="unrecognizedDisks">Список доступных новых дисков (Candidates).</param>
        public ReAssociateDialog(DeviceStatus lostDevice, ObservableCollection<DeviceStatus> unrecognizedDisks)
        {
            this.InitializeComponent();

            // Формируем понятное сообщение для пользователя
            InfoTextBlock.Text = $"Устройство '{lostDevice.DisplayName}' сохранено в базе, но не найдено. " +
                                 $"Выберите новый физический диск, чтобы связать этот профиль с ним.";

            // Привязываем список кандидатов
            DisksComboBox.ItemsSource = unrecognizedDisks;

            // Выбираем первый элемент по умолчанию для удобства
            if (unrecognizedDisks.Count > 0)
            {
                DisksComboBox.SelectedIndex = 0;
            }

            this.PrimaryButtonClick += OnPrimaryButtonClick;
        }

        private void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Проверяем, выбран ли диск
            if (DisksComboBox.SelectedItem is DeviceStatus selected)
            {
                SelectedDevice = selected;
            }
            else
            {
                // Если ничего не выбрано, запрещаем закрытие диалога
                args.Cancel = true;
                DisksComboBox.Header = "Выберите диск (Обязательно!)";
            }
        }
    }
}
