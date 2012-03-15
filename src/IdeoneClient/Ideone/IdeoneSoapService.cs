using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace IdeoneClient.Ideone
{
    internal interface IIdeoneSoapService
    {
        XmlNode[] TestFunction(string user, string pass);
    }

    internal partial class IdeoneSoapService : IIdeoneSoapService
    {
        public XmlNode[] TestFunction(string user, string pass)
        {
            return (XmlNode[])this.testFunction(user, pass);
        }
    }
}
