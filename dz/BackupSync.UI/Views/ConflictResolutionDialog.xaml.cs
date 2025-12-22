using System;
using System.Collections.Generic;
using System.Linq;
using BackupSync.Core.Models;
using Microsoft.UI.Xaml.Controls;

namespace BackupSync.UI.Views.Dialogs
{
    /// <summary>
    /// Диалог для ручного разрешения конфликтов синхронизации.
    /// Позволяет пользователю выбрать действие для каждого проблемного файла.
    /// </summary>
    public sealed partial class ConflictResolutionDialog : ContentDialog
    {
        // Исходный список конфликтов (только для чтения)
        private readonly List<SyncAction> _conflicts;

        /// <summary>
        /// Список принятых решений. Изначально заполнен действиями "Skip".
        /// Обновляется по мере выбора пользователем вариантов в ComboBox.
        /// </summary>
        public List<SyncAction> ResolvedActions { get; private set; } = new();

        public ConflictResolutionDialog(List<SyncAction> conflicts)
        {
            this.InitializeComponent();
            _conflicts = conflicts;

            // 1. Привязываем данные к UI
            ConflictsListView.ItemsSource = _conflicts;

            // 2. Инициализируем список решений значениями по умолчанию
            foreach (var conflict in _conflicts)
            {
                // Создаем новый объект Action, чтобы не портить исходный,
                // пока пользователь не подтвердит выбор.
                // По умолчанию ставим Skip (безопасное действие).
                ResolvedActions.Add(new SyncAction
                {
                    ActionType = SyncActionType.Skip,
                    Diff = conflict.Diff
                });
            }
        }

        /// <summary>
        /// Обработчик изменения выбора в выпадающем списке строки.
        /// </summary>
        private void ResolutionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 1. Получаем ComboBox и связанный с ним объект SyncAction (через Tag)
            if (sender is not ComboBox comboBox) return;
            if (comboBox.Tag is not SyncAction originalAction) return;

            // 2. Находим соответствующее действие в нашем списке результатов
            var targetAction = ResolvedActions.FirstOrDefault(a => a.Diff.RelativePath == originalAction.Diff.RelativePath);

            if (targetAction == null) return;

            // 3. Получаем выбранный тип действия из Tag элемента ComboBoxItem
            // (Это надежнее, чем switch по SelectedIndex)
            if (comboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag is string actionString)
            {
                if (Enum.TryParse<SyncActionType>(actionString, out var newType))
                {
                    targetAction.ActionType = newType;
                }
            }
        }
    }
}
