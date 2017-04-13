using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSGTools
{
    public class AvitoAdCategory
    {
        public AvitoAdCategory() { }

        public AvitoAdCategory(int p_id, string p_category, string p_comment, int p_parent, int p_available)
        {
            id = p_id;
            category = p_category;
            comment = p_comment;
            parent = p_parent;
            available = p_available;
        }

        public int id { get; set; }

        public string category { get; set; }

        public string comment { get; set; }

        public DateTime lastupdate { get; set; }

        public int parent { get; set; }

        public int available { get; set; }
    }
}