using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdeoneClient
{
    public class AuthenticationFailedException : IdeoneException
    {
        public AuthenticationFailedException(string message)
            : base(message)
        { }
    }
}
