﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model.Constant
{
    static class Contant
    {
#if TESTENV
        public const int ClosePageEndTimer = 1000*60*1;
#else
        public const int ClosePageEndTimer = 1000*60*3;
#endif
    }
}
