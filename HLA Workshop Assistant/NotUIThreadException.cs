using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLA_Workshop_Assistant
{
    [System.Serializable]
    public class NotUIThreadException : Exception
    {
        const string mainMessage = "UIDependency object must be instantiated on the UI thread.  Use CreateInstance<t>(params object[] parameters) to create the instance on the UI thread.";
        internal NotUIThreadException() : base(mainMessage) { }
        internal NotUIThreadException(string message) : base(message + "\r\n" + mainMessage) { }
        internal NotUIThreadException(string message, Exception inner) : base(message + "\r\n" + mainMessage, inner) { }
        protected NotUIThreadException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
