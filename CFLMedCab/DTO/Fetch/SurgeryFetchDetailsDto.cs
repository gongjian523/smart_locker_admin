using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.DTO.Fetch
{
    public class SurgeryFetchDetailsDto:SurgeryFetchDto
    {
        /// <summary>
        /// 需求数量
        /// </summary>
        public int total_num { get; set; }
    }
}
