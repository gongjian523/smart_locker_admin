using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UboConfigTool.Infrastructure.Models
{
    public class DTOUBO
    {
        public int id { get; set; }
        public string name  { get; set; }
        public string description   { get; set; }
        public string sid   { get; set; }
        public string bid  { get; set; }
        public DateTime? created_at  { get; set; }
        public DateTime? updated_at { get; set; }
        public int ubo_group_id { get; set; }
        public string version  { get; set; }
        public string ip { get; set; }
        public string location  { get; set; }
        public int connection_status { get; set; }
        public string virusdb_version  { get; set; }
        public DateTime? last_response_at  { get; set; }
    }
}
