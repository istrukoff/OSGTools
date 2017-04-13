using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSGTools
{
    public class AvitoData
    {
        public AvitoData() { }

        public AvitoData(int p_id, string p_name, string p_telephone, string p_email, string p_password, int p_status, int p_used)
        {
            id = p_id;
            name = p_name;
            telephone = p_telephone;
            email = p_email;
            password = p_password;
            status = p_status;
            used = p_used;
        }

        public int id { get; set; }

        public string name { get; set; }

        public string telephone { get; set; }

        public string email { get; set; }

        public string password { get; set; }

        public int status { get; set; }

        public int used { get; set; }
    }
}