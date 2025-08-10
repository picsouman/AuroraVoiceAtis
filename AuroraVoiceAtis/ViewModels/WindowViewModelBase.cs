using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AuroraVoiceAtis.ViewModels
{
    public class WindowViewModelBase : ViewModelBase
    {
        private string title = string.Empty;
        public string Title
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged();
            }
        }
    }
}
