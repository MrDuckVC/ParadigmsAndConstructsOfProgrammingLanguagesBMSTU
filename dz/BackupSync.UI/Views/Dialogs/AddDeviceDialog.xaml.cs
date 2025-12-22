using Microsoft.UI.Xaml.Controls;

namespace BackupSync.UI.Views.Dialogs
{
    /// <summary>
    /// Диалоговое окно для ввода имени нового устройства.
    /// </summary>
    public sealed partial class AddDeviceDialog : ContentDialog
    {
        /// <summary>
        /// Результат ввода пользователя (имя устройства).
        /// Доступно только после успешного закрытия диалога кнопкой "Сохранить".
        /// </summary>
        public string DeviceName { get; private set; } = string.Empty;

        /// <summary>
        /// Конструктор диалога.
        /// </summary>
        /// <param name="defaultName">Имя по умолчанию (обычно метка тома или "Новый том").</param>
        public AddDeviceDialog(string defaultName)
        {
            this.InitializeComponent();

            // Предзаполняем поле ввода
            NameTextBox.Text = defaultName;

            // Подписываемся на клик по кнопке "Primary" (Сохранить)
            this.PrimaryButtonClick += AddDeviceDialog_PrimaryButtonClick;
        }

        private void AddDeviceDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Очищаем от лишних пробелов
            string inputName = NameTextBox.Text.Trim();

            // Простейшая валидация: имя не должно быть пустым
            if (string.IsNullOrEmpty(inputName))
            {
                // Отменяем закрытие диалога
                args.Cancel = true;

                // Визуально сообщаем об ошибке (меняем заголовок поля)
                NameTextBox.Header = "Название (обязательно)";
                // Можно также подсветить красным, но это требует стилей. 
                // Изменения Header достаточно для MVP.
            }
            else
            {
                // Все ок, сохраняем результат
                DeviceName = inputName;
            }
        }
    }
}
