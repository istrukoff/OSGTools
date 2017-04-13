using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSGTools.Objects
{
    public static class AvitoAdCategory
    {
        public static int id { get; set; }
        public static string category { get; set; }
        public static string comment { get; set; }
        public static DateTime lastupdate { get; set; }
        public static int parent { get; set; }
        public static int available { get; set; }
    }
}