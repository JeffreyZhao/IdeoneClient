using System;
using System.Collections;
using System.Xml;
using IdeoneClient.Ideone;

namespace IdeoneClient
{
    internal class IdeoneSoapResult
    {
        private readonly IIdeoneSoapRawResult _rawResult;

        public IdeoneSoapResult(IIdeoneSoapRawResult rawResult)
        {
            this._rawResult = rawResult;
        }

        public Hashtable Data { get { return Utils.ParseXmlNodes((XmlNode[])this._rawResult.Result); } }

        public Exception Error { get { return this._rawResult.Error; } }

        public bool Cancelled { get { return this._rawResult.Cancelled; } }
    }
}
