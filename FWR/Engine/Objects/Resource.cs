using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWR.Engine
{
    public class Resource
    {
        public string ResourceJsonFilePath { get; set; }
        public string NickName { get; set; }
        public bool Registered { get; set; }
        public bool Locked { get; set; }
    }
}
