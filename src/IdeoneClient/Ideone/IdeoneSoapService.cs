using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;

namespace IdeoneClient.Ideone
{
    internal interface IIdeoneSoapService
    {
        Hashtable TestFunction(string user, string pass);

        Hashtable GetLanguages(string user, string pass);
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
    }
}
