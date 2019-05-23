using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using UboConfigTool.Infrastructure.Interfaces;

namespace UboConfigTool.Infrastructure.Services
{
    public class EncryptionService : IEncryptionService
    {
        public EncryptionService()
        {

        }

        public string EncrypteVText( string text )

        {
            //To do :  with encryption third party
            return text;
        }
    }
}
