using AuroraVoiceAtis.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AuroraVoiceAtis.Views.Editors
{
    /// <summary>
    /// Logique d'interaction pour StringCollectionEditorView.xaml
    /// </summary>
    public partial class StringCollectionEditorView : UserControl
    {
        public StringCollectionEditorView()
        {
            InitializeComponent();
            DataContextChanged += StringCollectionEditorView_DataContextChanged;
            ComboboxItems.KeyDown += ComboboxItems_KeyDown;
        }

        private void ComboboxItems_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (DataContext is StringCollectionEditorViewModel viewModel)
                {
                    viewModel.ValidateItemCommand.Execute(null);
                }
                e.Handled = true;
            }
        }

        private void StringCollectionEditorView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null && e.OldValue is StringCollectionEditorViewModel oldViewModel)
            {
                oldViewModel.FocusRequested -= ViewModel_FocusRequested;
            }
            if (e.NewValue != null && e.NewValue is StringCollectionEditorViewModel newViewModel)
            {
                newViewModel.FocusRequested += ViewModel_FocusRequested;
            }
        }

        private void ViewModel_FocusRequested(object sender, Core.RequestFocusEventArgs e)
        {
            if (DataContext is StringCollectionEditorViewModel viewModel)
            {
                switch (e.PropertyName)
                {
                    case nameof(viewModel.Items):
                        ComboboxItems.Focus();
                        break;
                }
            }
        }
    }
}
