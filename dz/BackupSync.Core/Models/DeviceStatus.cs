using System;

namespace BackupSync.Core.Models
{
    /// <summary>
    /// Модель состояния устройства для отображения в UI.
    /// Объединяет данные из базы данных (сохраненные настройки) и WMI (текущее физическое состояние).
    /// </summary>
    public class DeviceStatus
    {
        // --- Данные из WMI (Физическое подключение) ---

        /// <summary>
        /// Объект физического диска (если подключен). Null, если устройство отключено.
        /// </summary>
        public LogicalDisk? WmiDisk { get; set; }

        // --- Данные из БД (Сохраненный профиль) ---

        /// <summary>
        /// ID устройства в базе данных (если опознано).
        /// </summary>
        public int? DeviceId { get; set; }

        /// <summary>
        /// Имя, данное пользователем (если опознано).
        /// </summary>
        public string? UserGivenName { get; set; }

        // --- Вычисляемые статусы ---

        /// <summary>
        /// Подключено ли устройство физически прямо сейчас.
        /// </summary>
        public bool IsConnected { get; set; }

        /// <summary>
        /// Есть ли это устройство в нашей базе данных.
        /// </summary>
        public bool IsRecognized { get; set; }

        // --- Умные свойства для UI (Binding) ---

        /// <summary>
        /// Форматированное имя для отображения в списке.
        /// Логика: "Имя пользователя (Метка тома)" или просто "Метка тома".
        /// </summary>
        public string DisplayName
        {
            get
            {
                // 1. Получаем системную метку (например, "KINGSTON")
                string? systemLabel = WmiDisk?.VolumeLabel;
                bool hasSystemLabel = !string.IsNullOrWhiteSpace(systemLabel);

                // 2. Если устройство ОПОЗНАНО (есть в БД)
                if (IsRecognized)
                {
                    string userName = UserGivenName ?? "Без имени";

                    // Если есть системная метка и она отличается от имени пользователя
                    // (например: Юзер назвал "Работа", а флешка называется "USB_DISK")
                    if (hasSystemLabel && !string.Equals(userName, systemLabel, StringComparison.OrdinalIgnoreCase))
                    {
                        return $"{userName} ({systemLabel})";
                    }

                    // Если метки совпадают или системной нет -> показываем только имя пользователя
                    return userName;
                }

                // 3. Если устройство НЕ ОПОЗНАНО (нет в БД)
                // Показываем метку тома или заглушку
                return hasSystemLabel ? systemLabel! : "Съемный носитель";
            }
        }

        /// <summary>
        /// Буква диска (например, "E:"). Возвращает "-", если диск отключен.
        /// </summary>
        public string Letter => WmiDisk?.Letter ?? "-";

        /// <summary>
        /// Серийный номер тома. Возвращает "-", если диск отключен.
        /// </summary>
        public string VolumeSerialNumber => WmiDisk?.VolumeSerialNumber ?? "-";

        /// <summary>
        /// Флаг для UI: можно ли перепривязать это устройство (оно в БД, но отключено).
        /// </summary>
        public bool CanBeReAssociated => !IsConnected && IsRecognized;
    }
}
