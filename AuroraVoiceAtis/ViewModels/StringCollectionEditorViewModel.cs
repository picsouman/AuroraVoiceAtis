using AuroraVoiceAtis.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AuroraVoiceAtis.ViewModels
{
    public class StringCollectionEditorViewModel : ViewModelBase, IRequestFocus
    {
        private ObservableCollection<string> items;
        public ObservableCollection<string> Items
        {
            get => items;
            set
            {
                items = value;
                OnPropertyChanged();
            }
        }

        private string selectedItem;

        public string SelectedItem
        {
            get => selectedItem;
            set
            {
                selectedItem = value;
                OnPropertyChanged();
            }
        }

        private bool isEditing;
        public bool IsEditing
        {
            get => isEditing;
            private set
            {
                if (isEditing != value)
                {
                    isEditing = value;
                    OnPropertyChanged();
                }
            }
        }

        private string lastCurrentTextViewed;
        private string currentText;
        public string CurrentText
        {
            get => currentText;
            set
            {
                if (value != null)
                {
                    lastCurrentTextViewed = value;
                }
                if (currentText != value)
                {
                    OnPropertyChanged();
                }
            }
        }

        public event EventHandler<RequestFocusEventArgs> FocusRequested;
        protected virtual void OnRequestFocus(RequestFocusEventArgs e)
        {
            FocusRequested?.Invoke(this, e);
        }

        public ICommand AddItemCommand { get; private set; }
        public ICommand RemoveItemCommand { get; private set; }
        public ICommand ValidateItemCommand { get; private set; }

        public StringCollectionEditorViewModel()
        {
            Items = new ObservableCollection<string>();

            AddItemCommand = new RelayCommand(ExecuteAddItemCommand, CanExecuteAddItemCommand);
            RemoveItemCommand = new RelayCommand(ExecuteRemoveItemCommand, CanExecuteRemoveItemCommand);
            ValidateItemCommand = new RelayCommand(ExecuteValidateItemCommand);
        }

        private bool CanExecuteRemoveItemCommand(object obj)
        {
            if (IsEditing)
            {
                return true;
            }
            return Items.Any() && SelectedItem != null;
        }

        private void ExecuteRemoveItemCommand(object obj)
        {
            if (SelectedItem != null)
            {
                Items.Remove(SelectedItem);
            }
            IsEditing = false;
        }

        private bool CanExecuteAddItemCommand(object obj)
        {
            return !IsEditing;
        }

        private void ExecuteAddItemCommand(object obj)
        {
            SelectedItem = null;
            IsEditing = true;
            OnRequestFocus(new RequestFocusEventArgs(nameof(Items)));
        }

        private void ExecuteValidateItemCommand(object obj)
        {
            if (!IsEditing)
            {
                return;
            }
            Items.Add(lastCurrentTextViewed);
            SelectedItem = lastCurrentTextViewed;
            IsEditing = false;
        }
    }
}
