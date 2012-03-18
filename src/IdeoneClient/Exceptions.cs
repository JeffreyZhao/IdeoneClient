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

    public class AuthenticationFailedException : IdeoneException
    {
        public AuthenticationFailedException(string username)
            : base("Authetication failed for " + username + ".")
        { }
    }

    public class LanguageNotFoundException : IdeoneException
    {
        public LanguageNotFoundException()
            : base("Language with specified id could not be found.")
        { }
    }

    public class PasteNotFoundException : IdeoneException
    {
        public PasteNotFoundException()
            : base("Paste with specified link could not be found.")
        { }
    }

    public class AccessDeniedException : IdeoneException
    {
        public AccessDeniedException()
            : base("Cannot access the specified resource.")
        { }
    }

    public class MonthlyLimitExceededException : IdeoneException
    {
        public MonthlyLimitExceededException()
            : base("You have reached a monthly limit.")
        { }
    }
}
