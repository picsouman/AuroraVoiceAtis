using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuroraVoiceAtis.ValueObjects;

namespace AuroraVoiceAtis.ViewModels
{
    public class ApproachProcedureViewModel : ViewModelBase
    {
        private string runwayDesignator;
        public string RunwayDesignator
        {
            get => runwayDesignator;
            set
            {
                runwayDesignator = value;
                OnPropertyChanged();
            }
        }

        private ApproachKind approachKind;
        public ApproachKind ApproachKind
        {
            get => approachKind;
            set
            {
                approachKind = value;
                OnPropertyChanged();
            }
        }

        private string complementaryIdentifier;
        public string ComplementaryIdentifier
        {
            get => complementaryIdentifier;
            set
            {
                complementaryIdentifier = value;
                OnPropertyChanged();
            }
        }
    }
}
