using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UboConfigTool.Domain.Models;

namespace UboConfigTool.Domain
{
    public class DTORule
    {
        public int id { get; set; }
        public string name  { get; set; }
        public DateTime? created_at  { get; set; }
        public DateTime? updated_at { get; set; }
        public int rule_type { get; set; }
        public string pattern { get; set; }
    }
}
