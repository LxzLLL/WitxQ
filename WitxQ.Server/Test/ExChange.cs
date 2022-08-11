using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Server.Test
{
    public class ExChange
    {
        public string Name { get; set; }

        public string HttpUrl { get; set; }

        public string WSUrl { get; set; }

        public List<User> Users { get; set; }

        public decimal Fee { get; set; }
    }

    public class ExChanges
    {
        public string LoopringSignUrl { get; set; }
        
        public List<ExChange> AllExChanges { get; set; }
    }


}
