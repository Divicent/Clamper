#region Usings

using System;

#endregion

namespace Clamper.Base.Exceptions
{
    public class ClamperException : Exception
    {
        public ClamperException(string message) : base(message)
        {
        }

        public ClamperException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}