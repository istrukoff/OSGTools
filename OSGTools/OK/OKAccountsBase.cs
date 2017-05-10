using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSGTools.OK
{
    public static class OKAccountsBase
    {
        // добавление аккаунта OK
        public static bool insertOK(OKData ok)
        {
            bool result = true;

            AccountsBase.Connect();
            string cmdtext = string.Format("INSERT INTO ok (telephone, password, firstname, lastname, sex, birthday, android_id, regdate) VALUES ('{0}', '{1}', '{2}', '{3}', {4}, '{5}', '{6}', now());",
                ok.Telephone,
                ok.Password,
                ok.FirstName,
                ok.LastName,
                ok.Sex,
                ok.BirthDay,
                ok.Android_id);
            MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
            cmd.ExecuteNonQuery();
            AccountsBase.Close();

            return result;
        }
    }
}