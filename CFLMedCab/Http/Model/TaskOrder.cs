﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CFLMedCab.Http.Model
{
    //用于映射任务单扫描后的参数
    public class TaskOrder
    {
        /// <summary>
        /// 任务单id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 任务单名字
        /// </summary>
        public string name { get; set; }
    }
}