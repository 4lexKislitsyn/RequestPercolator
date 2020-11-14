using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace RequestPercolator.Model.Exceptions
{
    [Serializable]
    public sealed class ProxyErrorException : Exception
    {
        public ProxyErrorException(Exception innerException, string errorType)
            : base("Error occurred while proxying to destination host", innerException)
        {
            ErrorType = errorType;
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        private ProxyErrorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ErrorType = info.GetString(nameof(ErrorType));
        }

        public string ErrorType { get; }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(ErrorType), ErrorType);
            base.GetObjectData(info, context);
        }
    }
}
