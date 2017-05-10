using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSGTools.Avito
{
    class AvitoAccountsBase
    {
        // получить список аккаунтов авито
        public static List<AvitoData> getAvitoList()
        {
            List<AvitoData> result = new List<AvitoData>();

            try
            {
                AccountsBase.Connect();
                string cmdtext = "SELECT id, name, telephone, email, password, status, used FROM avito;";
                MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new AvitoData(int.Parse(reader["id"].ToString()),
                        reader["name"].ToString(),
                        reader["telephone"].ToString(),
                        reader["email"].ToString(),
                        reader["password"].ToString(),
                        int.Parse(reader["status"].ToString()),
                        int.Parse(reader["used"].ToString())));
                }
                reader.Close();
                AccountsBase.Close();
            }
            catch { }

            return result;
        }

        // получить логины свободных аккаунтов
        public static List<string> getAllFreeAvitoAccounts()
        {
            List<string> result = new List<string>();

            AccountsBase.Connect();
            try
            {
                string cmdtext = string.Format("SELECT email FROM avito WHERE status=0 AND used=0;");
                MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    result.Add(reader["email"].ToString());
            }
            catch { }
            AccountsBase.Close();

            return result;
        }

        // получить логины свободных аккаунтов с указанием города
        public static List<string> getAllFreeAvitoAccounts(string city)
        {
            List<string> result = new List<string>();

            AccountsBase.Connect();
            try
            {
                string cmdtext = string.Format("SELECT email FROM avito WHERE city='{0}' AND status=0 AND used=0;", city);
                MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    result.Add(reader["email"].ToString());
            }
            catch { }
            AccountsBase.Close();

            return result;
        }

        // получить логины свободных аккаунтов с указанием города и количеству объявлений, размещённых за текущий день
        public static List<string> getAllFreeAvitoAccounts(string city, int adscountperday)
        {
            List<string> result = new List<string>();

            AccountsBase.Connect();
            try
            {
                string cmdtext = string.Format("SELECT email FROM avito WHERE city='{0}' AND ads_count(email)<={1} AND status=0 AND used=0;",
                    city,
                    adscountperday);
                MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    result.Add(reader["email"].ToString());
            }
            catch { }
            AccountsBase.Close();

            return result;
        }

        // установить статус занятости аккаунту
        public static void setAvitoAccountUsed(string email, int used)
        {
            AccountsBase.Connect();
            try
            {
                string cmdtext = string.Format("UPDATE avito SET used = {1} WHERE email = '{0}';", email, used);
                MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
                cmd.ExecuteNonQuery();
            }
            catch { }
            AccountsBase.Close();
        }

        // получить данные выбранного аккаунта
        public static AvitoData getAvito(string email)
        {
            AvitoData result;

            AccountsBase.Connect();
            string cmdtext = string.Format("SELECT id, name, telephone, password, status, used FROM avito WHERE email='{0}';", email);
            MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            result = new AvitoData(int.Parse(reader["id"].ToString()),
                        reader["name"].ToString(),
                        reader["telephone"].ToString(),
                        email,
                        reader["password"].ToString(),
                        int.Parse(reader["status"].ToString()),
                        int.Parse(reader["used"].ToString()));
            reader.Close();
            AccountsBase.Close();

            return result;
        }

        // **** **** шаблоны объявлений **** **** //
        // получить список шаблонов по заданному идентификатору цели
        public static List<AvitoTemplate> getAvitoTemplates(int target)
        {
            List<AvitoTemplate> result = new List<AvitoTemplate>();

            AccountsBase.Connect();
            string cmdtext = string.Format("SELECT id, name, description, price, size, picturespath, city FROM avito_templates WHERE target='{0}';", target);
            MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
                result.Add(new AvitoTemplate(int.Parse(reader["id"].ToString()),
                    target,
                    reader["name"].ToString(),
                    reader["description"].ToString(),
                    reader["price"].ToString(),
                    reader["size"].ToString(),
                    reader["picturespath"].ToString(),
                    reader["city"].ToString()));
            reader.Close();
            AccountsBase.Close();

            return result;
        }

        // получить список шаблонов по заданному идентификатору цели
        public static List<AvitoTemplate> getAvitoTemplates(int target, string city)
        {
            List<AvitoTemplate> result = new List<AvitoTemplate>();

            AccountsBase.Connect();
            string cmdtext = string.Format("SELECT id, name, description, price, size, picturespath, city FROM avito_templates WHERE target='{0}' AND city='{1}';", target, city);
            MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
                result.Add(new AvitoTemplate(int.Parse(reader["id"].ToString()),
                    target,
                    reader["name"].ToString(),
                    reader["description"].ToString(),
                    reader["price"].ToString(),
                    reader["size"].ToString(),
                    reader["picturespath"].ToString(),
                    reader["city"].ToString()));
            reader.Close();
            AccountsBase.Close();

            return result;
        }

        // **** **** разделы и категории **** **** //
        // добавление или обновление категорий
        public static bool upsertCategory(List<AvitoAdCategory> categories)
        {
            bool result = true;

            AccountsBase.Connect();
            MySqlTransaction transaction = AccountsBase.Connection.BeginTransaction();

            MySqlCommand cmd_1 = new MySqlCommand("UPDATE avito_categories SET available = 0;", AccountsBase.Connection, transaction);
            cmd_1.ExecuteNonQuery();

            foreach (AvitoAdCategory cat in categories)
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = AccountsBase.Connection;
                cmd.Transaction = transaction;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "upsert_avito_category";
                cmd.Parameters.AddWithValue("p_category", cat.category);
                cmd.Parameters.AddWithValue("p_comment", cat.comment);
                cmd.Parameters.AddWithValue("p_parent", cat.parent);
                cmd.Parameters.AddWithValue("p_available", cat.available);
                cmd.ExecuteNonQuery();
            }

            transaction.Commit();
            AccountsBase.Close();

            return result;
        }

        // добавление или обновление категорий
        public static bool upsertCategory(List<AvitoAdCategory> categories, bool withparentname)
        {
            bool result = true;

            AccountsBase.Connect();
            MySqlTransaction transaction = AccountsBase.Connection.BeginTransaction();

            //MySqlCommand cmd_1 = new MySqlCommand("UPDATE avito_categories SET available = 0;", AccountsBase.Connection, transaction);
            //cmd_1.ExecuteNonQuery();

            foreach (AvitoAdCategory cat in categories)
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = AccountsBase.Connection;
                cmd.Transaction = transaction;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "upsert_avito_category2";
                cmd.Parameters.AddWithValue("p_category", cat.category);
                cmd.Parameters.AddWithValue("p_comment", cat.comment);
                cmd.Parameters.AddWithValue("p_parentname", cat.parentname);
                cmd.Parameters.AddWithValue("p_available", cat.available);
                cmd.ExecuteNonQuery();
            }

            transaction.Commit();
            AccountsBase.Close();

            return result;
        }

        // добавление или обновление разделов
        public static bool upsertSection(List<AvitoAdSection> sections)
        {
            bool result = true;

            AccountsBase.Connect();
            MySqlTransaction transaction = AccountsBase.Connection.BeginTransaction();

            MySqlCommand cmd_1 = new MySqlCommand("UPDATE avito_sections SET available = 0;", AccountsBase.Connection, transaction);
            cmd_1.ExecuteNonQuery();

            foreach (AvitoAdSection section in sections)
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = AccountsBase.Connection;
                cmd.Transaction = transaction;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "upsert_avito_section";
                cmd.Parameters.AddWithValue("p_section", section.section);
                cmd.Parameters.AddWithValue("p_comment", section.comment);
                cmd.Parameters.AddWithValue("p_available", section.available);
                cmd.ExecuteNonQuery();
            }

            transaction.Commit();
            AccountsBase.Close();

            return result;
        }

        // поставить статус недоступности всем категориям
        public static bool setCategoriesNotAvailable()
        {
            bool result = true;

            AccountsBase.Connect();
            string cmdtext = string.Format("UPDATE avito_categories SET available = 0;");
            MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
            cmd.ExecuteNonQuery();
            AccountsBase.Close();

            return result;
        }

        // поставить статус недоступности всем разделам
        public static bool setSectionsNotAvailable()
        {
            bool result = true;

            AccountsBase.Connect();
            string cmdtext = string.Format("UPDATE avito_sections SET available = 0;");
            MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
            cmd.ExecuteNonQuery();
            AccountsBase.Close();

            return result;
        }

        // получить идентификатор категории по названию
        public static int getCategoryID(string category)
        {
            int result;

            AccountsBase.Connect();
            try
            {
                string cmdtext = string.Format("SELECT id FROM avito_categories WHERE category='{0}';", category);
                MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                result = int.Parse(reader["id"].ToString());
            }
            catch
            {
                result = -1;
            }
            AccountsBase.Close();

            return result;
        }

        // получить идентификатор категории по названию и названию родительской категории
        public static int getCategoryID(string category, string parentname)
        {
            int result;

            AccountsBase.Connect();
            try
            {
                string cmdtext = string.Format("SELECT id FROM avito_categories WHERE category='{0}' AND parentname='{1}';", category, parentname);
                MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                result = int.Parse(reader["id"].ToString());
            }
            catch
            {
                result = -1;
            }
            AccountsBase.Close();

            return result;
        }

        // **** **** объявления **** **** //
        // добавление объявления
        public static bool insertAvitoAd(AvitoAd ad)
        {
            bool result = true;

            AccountsBase.Connect();
            string cmdtext = string.Format("INSERT INTO avito_ads (idlogin, name, description, price, size, categoryid, city, publicationdate, status) VALUES ({0}, '{1}', '{2}', '{3}', '{4}', {5}, '{6}', now(), {7});",
                ad.idlogin,
                ad.name,
                ad.description,
                ad.price,
                ad.size,
                ad.categoryid,
                ad.city,
                ad.status);
            MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
            cmd.ExecuteNonQuery();
            AccountsBase.Close();

            return result;
        }
    }
}