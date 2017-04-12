using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSGTools
{
    public class FollowingData
    {
        public FollowingData() { }

        public FollowingData(int p_id, string p_login, string p_name, string p_description, string p_website, int p_type, int p_countofposts, int p_countoffollowers, int p_countoffollowing)
        {
            id = p_id;
            login = p_login;
            name = p_name;
            description = p_description;
            website = p_website;
            type = p_type;
            countofposts = p_countofposts;
            countoffollowers = p_countoffollowers;
            countoffollowing = p_countoffollowing;
        }

        private int id;
        public int ID { get { return id; } set { id = value; } }

        private string login;
        public string Login { get { return login; } set { login = value; } }

        private string name;
        public string Name { get { return name; } set { name = value; } }

        private string description;
        public string Description { get { return description; } set { description = value; } }

        private string website;
        public string WebSite { get { return website; } set { website = value; } }

        private int type;
        public int Type { get { return type; } set { type = value; } }

        private int countofposts;
        public int Countofposts { get { return countofposts; } set { countofposts = value; } }

        private int countoffollowers;
        public int Countoffollowers { get { return countoffollowers; } set { countoffollowers = value; } }

        private int countoffollowing;
        public int Countoffollowing { get { return countoffollowing; } set { countoffollowing = value; } }

        private DateTime dateadd;
        public DateTime Dateadd { get { return dateadd; } set { dateadd = value; } }

        private DateTime lastupdate;
        public DateTime Lastupdate { get { return lastupdate; } set { lastupdate = value; } }

        private DateTime datefollow;
        public DateTime Datefollow { get { return datefollow; } set { datefollow = value; } }
    }
}