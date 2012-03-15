using System.Collections.Generic;
using IdeoneClient.Ideone;

namespace IdeoneClient
{
    public class IdeoneService
    {
        private readonly string _username;
        private readonly string _password;
        private readonly IIdeoneSoapService _soapService;

        internal IdeoneService(IIdeoneSoapService soapService, string username, string password)
        {
            this._soapService = soapService;
            this._username = username;
            this._password = password;
        }

        public IdeoneService(string url, string username, string password)
            : this(new IdeoneSoapService(url), username, password)
        { }

        public IdeoneService(string username, string password)
            : this(new IdeoneSoapService(), username, password)
        { }

        public Dictionary<string, object> Test()
        {
            var dict = new Dictionary<string, object>();
            var results = this._soapService.TestFunction(this._username, this._password);
            return dict;
        }
    }
}
