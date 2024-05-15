using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SimTableApplication.Core.Exceptions
{
    [Serializable]
    public class VirtualControllerException : Exception
    {       

        public VirtualControllerException(string message) : base(message)
        {
        }

        public VirtualControllerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected VirtualControllerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
