using System;
using BackupSync.UI.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace BackupSync.UI
{
    /// <summary>
    /// Логика главного окна (Code-behind).
    /// Отвечает за навигацию между страницами (DevicesPage, JobsPage).
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            // Заголовок окна задается в XAML или здесь.
            // События (Loaded, SelectionChanged) уже привязаны в XAML,
            // поэтому здесь их дублировать не нужно.
        }

        /// <summary>
        /// Вызывается, когда NavigationView полностью загружен.
        /// Используется для установки начальной страницы.
        /// </summary>
        private void AppNavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            // Выбираем первый пункт меню ("Устройства") программно при старте.
            // Это автоматически вызовет событие SelectionChanged и загрузит страницу.
            if (AppNavigationView.MenuItems.Count > 0)
            {
                AppNavigationView.SelectedItem = AppNavigationView.MenuItems[0];
            }
        }

        /// <summary>
        /// Вызывается при смене выбранного пункта меню.
        /// </summary>
        private void AppNavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            // 1. Проверяем, выбраны ли Настройки (нижняя кнопка)
            if (args.IsSettingsSelected)
            {
                // TODO: Создать SettingsPage и раскомментировать
                // ContentFrame.Navigate(typeof(SettingsPage));
            }
            // 2. Проверяем обычные пункты меню
            else if (args.SelectedItemContainer != null)
            {
                // Получаем Tag из XAML ("devices" или "jobs")
                var tag = args.SelectedItemContainer.Tag?.ToString();

                Type pageType = tag switch
                {
                    "devices" => typeof(DevicesPage),
                    "jobs" => typeof(JobsPage),
                    _ => typeof(DevicesPage) // Fallback
                };

                // Выполняем навигацию, только если мы еще не на этой странице
                if (ContentFrame.CurrentSourcePageType != pageType)
                {
                    ContentFrame.Navigate(pageType, null, args.RecommendedNavigationTransitionInfo);
                }
            }
        }
    }
}
