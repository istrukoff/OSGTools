using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSGTools
{
    public class InstagramData
    {
        public InstagramData() { }

        public InstagramData(int p_id, string p_login, string p_password, string p_telephone, string p_android_id, string p_email, string p_name, string p_description)
        {
            id = p_id;
            login = p_login;
            password = p_password;
            telephone = p_telephone;
            android_id = p_android_id;
            email = p_email;
            name = p_name;
            description = p_description;
        }

        private int id;
        public int ID { get { return id; } set { id = value; } }

        private string login;
        public string Login { get { return login; } set { login = value; } }

        private string password;
        public string Password { get { return password; } set { password = value; } }

        private string telephone;
        public string Telephone { get { return telephone; } set { telephone = value; } }

        private string android_id;
        public string Android_id { get { return android_id; } set { android_id = value; } }

        private string email;
        public string Email { get { return email; } set { email = value; } }

        private string website;
        public string WebSite { get { return website; } set { website = value; } }

        private string name;
        public string Name { get { return name; } set { name = value; } }

        private string description;
        public string Description { get { return description; } set { description = value; } }

        private string avatarfilename;
        public string AvatarFileName { get { return avatarfilename; } set { avatarfilename = value; } }
    }
}