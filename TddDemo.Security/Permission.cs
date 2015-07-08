using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TddDemo.Security
{
    public class Permission
    {
        public User User { get; set; }

        public Activity Activity { get; set; }

        public Group Group { get; set; }
    }
}
