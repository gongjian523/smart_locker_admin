using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UboConfigTool.Infrastructure.Models
{
    public class DTOUBOGroupUSB
    {
        public int id {  get; set; }
        public int ubo_group_id {  get; set; }
        public int usb_id {  get; set; }
        public int permission_role {  get; set; }
        public DateTime? created_at {  get; set; }
        public DateTime? updated_at {  get; set; }
    }
}
