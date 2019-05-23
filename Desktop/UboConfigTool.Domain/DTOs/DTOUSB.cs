using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UboConfigTool.Domain
{
    public class DTOUSB
    {
        public int id { get; set; }
        public string pid  { get; set; }
        public string vid  { get; set; }
        public string name  { get; set; }
        public string serial_num  { get; set; }
        public string volume_id   { get; set; }
        public string capacity  { get; set; }
        public string factory  { get; set; }
        public string disk_id   { get; set; }
        public int disk_type   { get; set; }
        public string filesystem_format   { get; set; }
        public DateTime?  created_at  { get; set; }
        public DateTime?  updated_at  { get; set; }
    }
}
