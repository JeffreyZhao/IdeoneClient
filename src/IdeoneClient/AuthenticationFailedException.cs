namespace IdeoneClient
{
    public class AuthenticationFailedException : IdeoneException
    {
        public AuthenticationFailedException(string message)
            : base(message)
        { }
    }
}
