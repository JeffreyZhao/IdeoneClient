using System.Collections;
using System.Net;
using System.Xml;
using System;
using System.Threading.Tasks;

namespace IdeoneClient.Ideone
{
    internal interface IIdeoneSoapService
    {
        IWebProxy Proxy { get; set; }

        Hashtable TestFunction(string user, string pass);

        Hashtable GetLanguages(string user, string pass);

        void GetLanguagesAsync(string user, string pass, Action<IdeoneSoapResult> callback);

        Hashtable CreateSubmission(string user, string pass, string sourceCode, int languageId, string input, bool run, bool isPrivate);

        void CreateSubmissionAsync(string user, string pass, string sourceCode, int languageId, string input, bool run, bool isPrivate, Action<IdeoneSoapResult> callback);

        Hashtable GetSubmissionStatus(string user, string pass, string link);

        Hashtable GetSubmissionDetail(string user, string pass, string link, bool withSource, bool withInput, bool withOutput, bool withStdErr, bool withCompileInfo);
    }

    internal partial class IdeoneSoapService : IIdeoneSoapService
    {
        public Hashtable TestFunction(string user, string pass)
        {
            return Utils.ParseXmlNodes((XmlNode[])this.testFunction(user, pass));
        }

        public Hashtable GetLanguages(string user, string pass)
        {
            return Utils.ParseXmlNodes((XmlNode[])this.getLanguages(user, pass));
        }

        public void GetLanguagesAsync(string user, string pass, Action<IdeoneSoapResult> callback)
        {
            getLanguagesCompletedEventHandler onCompleted = null;

            var state = new object();

            onCompleted = new getLanguagesCompletedEventHandler((_, args) =>
            {
                if (args.UserState != state) return;

                this.getLanguagesCompleted -= onCompleted;

                callback(new IdeoneSoapResult(args));
            });

            this.getLanguagesCompleted += onCompleted;

            this.getLanguagesAsync(user, pass, state);
        }

        public Hashtable CreateSubmission(string user, string pass, string sourceCode, int languageId, string input, bool run, bool isPrivate)
        {
            return Utils.ParseXmlNodes((XmlNode[])this.createSubmission(user, pass, sourceCode, languageId, input, run, isPrivate));
        }

        public void CreateSubmissionAsync(string user, string pass, string sourceCode, int languageId, string input, bool run, bool isPrivate, Action<IdeoneSoapResult> callback)
        {
            createSubmissionCompletedEventHandler onCompleted = null;

            var state = new object();

            onCompleted = new createSubmissionCompletedEventHandler((_, args) =>
            {
                if (args.UserState != state) return;

                this.createSubmissionCompleted -= onCompleted;

                callback(new IdeoneSoapResult(args));
            });

            this.createSubmissionCompleted += onCompleted;

            this.createSubmissionAsync(user, pass, sourceCode, languageId, input, run, isPrivate, state);
        }

        public Hashtable GetSubmissionStatus(string user, string pass, string link)
        {
            return Utils.ParseXmlNodes((XmlNode[])this.getSubmissionStatus(user, pass, link));
        }

        public Hashtable GetSubmissionDetail(string user, string pass, string link, bool withSource, bool withInput, bool withOutput, bool withStdErr, bool withCompileInfo)
        {
            return Utils.ParseXmlNodes((XmlNode[])this.getSubmissionDetails(user, pass, link, withSource, withInput, withOutput, withStdErr, withCompileInfo));
        }
    }
}