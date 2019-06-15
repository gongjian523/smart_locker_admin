using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.APO.Inventory
{
    public class GetGoodApo:BasePageDataApo
    {
        public HashSet<string> goodsEpsDatas { get; set; }

        public string code { get; set; }

        public string name { get; set; }
    }
}
