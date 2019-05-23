using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UboConfigTool.Infrastructure.Models
{
    public class DTOUBOGroup
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description  { get; set; }
        public DateTime? last_deploy_request_at  { get; set; }
        public DateTime? last_info_changed_at { get; set; }
        public List<DTOUBO> ubo_devices { get; set; }
        public List<DTORule> rules { get; set; }
        public List<DTOUSB> usbs { get; set; }
        public List<DTOUBOGroupUSB> ubo_group_usbs  { get; set; }
    }
}
