using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace BackupSync.UI.Converters
{
    /// <summary>
    /// Конвертер значения bool в Visibility с инверсией.
    /// Используется для скрытия элементов, когда условие истинно.
    /// <para>True -> Visibility.Collapsed (Скрыто)</para>
    /// <para>False -> Visibility.Visible (Видно)</para>
    /// </summary>
    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Преобразует bool в Visibility (Инвертировано).
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Безопасная проверка типа.
            // Если значение является bool и равно true -> Скрываем.
            if (value is bool boolValue && boolValue)
            {
                return Visibility.Collapsed;
            }

            // Во всех остальных случаях (false, null, другой тип) -> Показываем.
            return Visibility.Visible;
        }

        /// <summary>
        /// Обратное преобразование не поддерживается.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
