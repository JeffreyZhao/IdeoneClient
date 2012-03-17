using System;

namespace IdeoneClient
{
    public class IdeoneException : Exception
    {
        public IdeoneException(string message)
            : base(message)
        { }

        public IdeoneException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
