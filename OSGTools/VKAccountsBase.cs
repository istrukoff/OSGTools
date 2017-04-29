using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSGTools
{
    public static class VKAccountsBase
    {
        // получить список аккаунтов VK
        public static List<VKData> getVKList()
        {
            List<VKData> result = new List<VKData>();

            try
            {
                AccountsBase.Connect();
                string cmdtext = "SELECT id, telephone, password, android_id, firstname, lastname, sex FROM vk;";
                MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new VKData(int.Parse(reader["id"].ToString()),
                        reader["telephone"].ToString(),
                        reader["password"].ToString(),
                        reader["android_id"].ToString(),
                        reader["firstname"].ToString(),
                        reader["lastname"].ToString(),
                        int.Parse(reader["sex"].ToString())
                    ));
                }
                reader.Close();
                AccountsBase.Close();
            }
            catch { }

            return result;
        }

        // получить данные выбранного аккаунта
        public static VKData getVK(string telephone)
        {
            VKData result;

            AccountsBase.Connect();
            string cmdtext = string.Format("SELECT id, password, android_id, firstname, lastname, sex FROM vk WHERE telephone='{0}';", telephone);
            MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            result = new VKData(int.Parse(reader["id"].ToString()),
                telephone,
                reader["password"].ToString(),
                reader["android_id"].ToString(),
                reader["firstname"].ToString(),
                reader["lastname"].ToString(),
                int.Parse(reader["sex"].ToString()));
            reader.Close();
            AccountsBase.Close();

            return result;
        }

        // добавление аккаунта VK
        public static bool insertVK(VKData vk)
        {
            bool result = true;

            AccountsBase.Connect();
            string cmdtext = string.Format("INSERT INTO vk (telephone, password, firstname, lastname, sex, android_id, regdate) VALUES ('{0}', '{1}', '{2}', '{3}', {4}, '{5}', now());",
                vk.Telephone,
                vk.Password,
                vk.FirstName,
                vk.LastName,
                vk.Sex,
                vk.Android_id);
            MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
            cmd.ExecuteNonQuery();
            AccountsBase.Close();

            return result;
        }
    }
}