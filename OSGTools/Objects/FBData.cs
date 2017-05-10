using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSGTools
{
    public class FBData
    {
        public FBData() { }

        public FBData(int p_id, string p_telephone, string p_password, string p_android_id, string p_firstname, string p_lastname, int p_sex)
        {
            id = p_id;
            telephone = p_telephone;
            password = p_password;
            android_id = p_android_id;
            firstname = p_firstname;
            lastname = p_lastname;
            sex = p_sex;
        }

        private int id;
        public int ID { get { return id; } set { id = value; } }

        private string telephone;
        public string Telephone { get { return telephone; } set { telephone = value; } }

        private string password;
        public string Password { get { return password; } set { password = value; } }

        private string user_id;
        public string User_id { get { return user_id; } set { user_id = value; } }

        private string firstname;
        public string FirstName { get { return firstname; } set { firstname = value; } }

        private string lastname;
        public string LastName { get { return lastname; } set { lastname = value; } }

        private int sex;
        public int Sex { get { return sex; } set { sex = value; } }

        private string birthday;
        public string BirthDay { get { return birthday; } set { birthday = value; } }

        private string android_id;
        public string Android_id { get { return android_id; } set { android_id = value; } }

        private string email;
        public string Email { get { return email; } set { email = value; } }

        private string website;
        public string WebSite { get { return website; } set { website = value; } }

        private string description;
        public string Description { get { return description; } set { description = value; } }

        private string city;
        public string City { get { return city; } set { city = value; } }

        private string avatarfilename;
        public string AvatarFileName { get { return avatarfilename; } set { avatarfilename = value; } }
    }
}