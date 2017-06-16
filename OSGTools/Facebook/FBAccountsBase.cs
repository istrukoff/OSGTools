using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSGTools.FB
{
    public static class FBAccountsBase
    {
        // добавление аккаунта Facebook
        public static bool insertFB(FBData fb)
        {
            bool result = true;

            AccountsBase.Connect();
            string cmdtext = string.Format("INSERT INTO facebook (telephone, password, user_id, firstname, lastname, sex, birthday, android_id, regdate) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', {5}, '{6}', '{7}', now());",
                fb.Telephone,
                fb.Password,
                fb.User_id,
                fb.FirstName,
                fb.LastName,
                fb.Sex,
                fb.BirthDay,
                fb.Android_id);
            MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
            cmd.ExecuteNonQuery();
            AccountsBase.Close();

            return result;
        }

        // добавление аккаунта Facebook
        public static bool insertFB(FBData fb, bool proxy)
        {
            bool result = true;

            AccountsBase.Connect();
            string cmdtext = string.Format("INSERT INTO facebook (telephone, password, user_id, firstname, lastname, sex, birthday, android_id, regdate, proxyip, proxyport) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', {5}, '{6}', '{7}', now(), '{8}', {9});",
                fb.Telephone,
                fb.Password,
                fb.User_id,
                fb.FirstName,
                fb.LastName,
                fb.Sex,
                fb.BirthDay,
                fb.Android_id,
                fb.ProxyIP,
                fb.ProxyPort);
            MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
            cmd.ExecuteNonQuery();
            AccountsBase.Close();

            return result;
        }
    }
}