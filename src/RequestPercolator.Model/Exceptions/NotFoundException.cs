using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace RequestPercolator.Model.Exceptions
{
    [Serializable]
    public sealed class NotFoundException : Exception
    {
        public NotFoundException(string message)
            : base(message)
        {
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        private NotFoundException(SerializationInfo info, StreamingContext context)
           : base(info, context)
        {
        }
    }
}
