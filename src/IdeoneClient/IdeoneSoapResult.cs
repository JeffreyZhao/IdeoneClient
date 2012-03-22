using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;

namespace IdeoneClient
{
    public class IdeoneSoapResult
    {
        private readonly Func<object[]> _dataFactory;

        public IdeoneSoapResult(Func<object[]> dataFactory, Exception error, bool cancelled)
        {
            this._dataFactory = dataFactory;
            this.Error = error;
            this.Cancelled = cancelled;
        }

        public Hashtable Data { get { return Utils.ParseXmlNodes((XmlNode[])this._dataFactory()); } }

        public Exception Error { get; private set; }

        public bool Cancelled { get; private set; }
    }
}
