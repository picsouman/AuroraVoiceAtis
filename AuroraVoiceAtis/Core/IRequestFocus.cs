using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraVoiceAtis.Core
{
    public class RequestFocusEventArgs : EventArgs
    {
        public string PropertyName { get; }

        public RequestFocusEventArgs(string propertyName)
        {
            PropertyName = propertyName;
        }
    }

    public interface IRequestFocus
    {
        event EventHandler<RequestFocusEventArgs> FocusRequested;
    }
}
