using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UboConfigTool.Infrastructure.Interfaces
{
    public interface IEncryptionService
    {
        string EncrypteVText( string text );
    }
}
