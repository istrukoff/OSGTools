using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSGTools
{
    public class AvitoAdSection
    {
        public AvitoAdSection() { }

        public AvitoAdSection(int p_id, string p_section, string p_comment, int p_available)
        {
            id = p_id;
            section = p_section;
            comment = p_comment;
            available = p_available;
        }

        public int id { get; set; }

        public string section { get; set; }

        public string comment { get; set; }

        public DateTime lastupdate { get; set; }

        public int available { get; set; }
    }
}