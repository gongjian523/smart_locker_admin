using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UboConfigTool.Modules.Logging.Model
{
    public class searchparms
    {
        public searchparms( )
        {
            start_time = string.Empty;
            end_time = string.Empty;
            keywords = string.Empty;
        }

        public int page
        {
            get;
            set;
        }

        public string start_time
        {
            get;
            set;
        }

        public string  end_time
        {
            get;
            set;
        }

        public string keywords
        {
            get;
            set;
        }

        public string level
        {
            get;
            set;
        }
    }
}
