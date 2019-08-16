using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model.Constant
{
    static class Contant
    {
#if TESTENV
        public const int IdleTimeExpireLength = 3;
#else
        public const int IdleTimeExpireLength = 3;
#endif

    }
}
