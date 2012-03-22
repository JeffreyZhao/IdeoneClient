using System;

namespace IdeoneClient.Ideone
{
    internal interface IIdeoneSoapRawResult
    {
        object[] Result { get; }

        Exception Error { get; }

        bool Cancelled { get; }
    }
}
