using IdeoneClient.Ideone;
using Moq;
using Xunit;
using System;

namespace IdeoneClient.Tests
{
    internal class IdeoneServiceTest
    {
        protected readonly string _username;
        protected readonly string _password;
        protected readonly Mock<IIdeoneSoapService> _soapServiceMock;
        protected readonly IdeoneService _service;

        public IdeoneServiceTest()
        {
            this._username = "jeffz";
            this._password = "12345678";
            this._soapServiceMock = new Mock<IIdeoneSoapService>(MockBehavior.Strict);
            this._service = new IdeoneService(this._soapServiceMock.Object, this._username, this._password);
        }
    }
}
