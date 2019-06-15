using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.APO.Inventory
{
    public class GetGoodsApo : BasePageDataApo
    {
        public HashSet<string> goodsEpsDatas { get; set; }

        public DateTime expire_date { get; set; }

        public string code { get; set; }

        public string name { get; set; }
    }
}
