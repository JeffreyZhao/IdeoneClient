using System.Collections.Generic;
using IdeoneClient.Ideone;
using System.Xml;
using System;
using System.Linq;
using System.Security.Authentication;
using System.Collections;

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

        public Dictionary<int, string> GetLanguages()
        {
            return this.Handle(() =>
            {
                var result = this._soapService.GetLanguages(this._username, this._password);
                this.CheckError(result);

                var languages = (Hashtable)result["languages"];
                return languages.Cast<DictionaryEntry>().ToDictionary(
                    p => (int)p.Key,
                    p => (string)p.Value);
            });
        }

        public Dictionary<string, object> Test()
        {
            return this.Handle(() =>
            {
                var result = this._soapService.TestFunction(this._username, this._password);
                this.CheckError(result);

                result.Remove("error");
                return result.Cast<DictionaryEntry>().ToDictionary(
                    p => (string)p.Key,
                    p => p.Value);
            });
        }

        private T Handle<T>(Func<T> body)
        {
            try
            {
                return body();
            }
            catch (IdeoneException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new IdeoneException("Ideone API goes wrong: it doesn't meet the contract.", ex);
            }
        }

        private void CheckError(Hashtable result)
        {
            var status = (string)result["error"];
            switch (status)
            {
                case "OK":
                    return;
                case "AUTH_ERROR":
                    throw new AuthenticationFailedException("Authetication failed for " + this._username);
                default:
                    throw new IdeoneException("Unexpected status: " + status);
            }
        }
    }
}
