using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UboConfigTool.Infrastructure.Models
{
    public class DTOUser
    {
        public int id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string uid { get; set; }
        public DateTime? created_at { get; set; }
        public List<DTOUserRole> user_roles { get; set; }
    }
}
