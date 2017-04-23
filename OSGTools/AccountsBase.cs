using MySql.Data.MySqlClient;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSGTools
{
    public static class AccountsBase
    {
        #region **** db variables ****
        private static string server;
        public static string Server { get { return server; } }

        private static int port { get; set; }
        public static int Port { get { return port; } }

        private static string dbname;
        public static string DBName { get { return dbname; } }

        private static string user;
        public static string User { get { return user; } }

        private static string password;
        public static string Password { get { return password; } }

        private static MySqlConnection connection = null;
        public static MySqlConnection Connection { get { return connection; } }
        #endregion

        // подключение к БД
        public static bool Connect()
        {
            bool result = true;

            server = "81.177.140.105";
            port = 3306;
            dbname = "accounts";
            user = "root";
            password = "KRUS56_ak+";

            if (String.IsNullOrEmpty(dbname))
                result = false;
            string connstring = string.Format("Server={0}; Port={1}; database={2}; UID={3}; password={4}; charset=utf8", server, port, dbname, user, password);
            connection = new MySqlConnection(connstring);
            connection.Open();
            result = true;

            return result;
        }

        // закрытие подключения к БД
        public static void Close()
        {
            connection.Close();
        }

        // **** **** **** **** avito **** **** **** **** //

        // получить список аккаунтов авито
        public static List<AvitoData> getAvitoList()
        {
            List<AvitoData> result = new List<AvitoData>();

            try
            {
                Connect();
                string cmdtext = "SELECT id, name, telephone, email, password, status, used FROM avito;";
                MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
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
                Close();
            }
            catch { }

            return result;
        }

        // получить логины свободных аккаунтов
        public static List<string> getAllFreeAvitoAccounts()
        {
            List<string> result = new List<string>();

            Connect();
            try
            {
                string cmdtext = string.Format("SELECT email FROM avito WHERE status=0 AND used=0;");
                MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    result.Add(reader["email"].ToString());
            }
            catch { }
            Close();

            return result;
        }

        // получить логины свободных аккаунтов с указанием города
        public static List<string> getAllFreeAvitoAccounts(string city)
        {
            List<string> result = new List<string>();

            Connect();
            try
            {
                string cmdtext = string.Format("SELECT email FROM avito WHERE city='{0}' AND status=0 AND used=0;", city);
                MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    result.Add(reader["email"].ToString());
            }
            catch { }
            Close();

            return result;
        }

        // установить статус занятости аккаунту
        public static void setAvitoAccountUsed(string email, int used)
        {
            Connect();
            try
            {
                string cmdtext = string.Format("UPDATE avito SET used = {1} WHERE email = '{0}';", email, used);
                MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
                cmd.ExecuteNonQuery();
            }
            catch { }
            Close();
        }

        // получить данные выбранного аккаунта
        public static AvitoData getAvito(string email)
        {
            AvitoData result;

            Connect();
            string cmdtext = string.Format("SELECT id, name, telephone, password, status, used FROM avito WHERE email='{0}';", email);
            MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
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
            Close();

            return result;
        }

        // **** **** шаблоны объявлений **** **** //
        // получить список шаблонов по заданному идентификатору цели
        public static List<AvitoTemplate> getAvitoTemplates(int target)
        {
            List<AvitoTemplate> result = new List<AvitoTemplate>();

            Connect();
            string cmdtext = string.Format("SELECT id, name, description, price, size, picturespath, city FROM avito_templates WHERE target='{0}';", target);
            MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
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
            Close();

            return result;
        }

        // получить список шаблонов по заданному идентификатору цели
        public static List<AvitoTemplate> getAvitoTemplates(int target, string city)
        {
            List<AvitoTemplate> result = new List<AvitoTemplate>();

            Connect();
            string cmdtext = string.Format("SELECT id, name, description, price, size, picturespath, city FROM avito_templates WHERE target='{0}' AND city='{1}';", target, city);
            MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
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
            Close();

            return result;
        }

        // **** **** разделы и категории **** **** //
        // добавление или обновление категорий
        public static bool upsertCategory(List<AvitoAdCategory> categories)
        {
            bool result = true;

            Connect();
            MySqlTransaction transaction = connection.BeginTransaction();

            MySqlCommand cmd_1 = new MySqlCommand("UPDATE avito_categories SET available = 0;", connection, transaction);
            cmd_1.ExecuteNonQuery();

            foreach (AvitoAdCategory cat in categories)
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
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
            Close();

            return result;
        }

        // добавление или обновление категорий
        public static bool upsertCategory(List<AvitoAdCategory> categories, bool withparentname)
        {
            bool result = true;

            Connect();
            MySqlTransaction transaction = connection.BeginTransaction();

            //MySqlCommand cmd_1 = new MySqlCommand("UPDATE avito_categories SET available = 0;", connection, transaction);
            //cmd_1.ExecuteNonQuery();

            foreach (AvitoAdCategory cat in categories)
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
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
            Close();

            return result;
        }

        // добавление или обновление разделов
        public static bool upsertSection(List<AvitoAdSection> sections)
        {
            bool result = true;

            Connect();
            MySqlTransaction transaction = connection.BeginTransaction();

            MySqlCommand cmd_1 = new MySqlCommand("UPDATE avito_sections SET available = 0;", connection, transaction);
            cmd_1.ExecuteNonQuery();

            foreach (AvitoAdSection section in sections)
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.Transaction = transaction;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "upsert_avito_section";
                cmd.Parameters.AddWithValue("p_section", section.section);
                cmd.Parameters.AddWithValue("p_comment", section.comment);
                cmd.Parameters.AddWithValue("p_available", section.available);
                cmd.ExecuteNonQuery();
            }

            transaction.Commit();
            Close();

            return result;
        }

        // поставить статус недоступности всем категориям
        public static bool setCategoriesNotAvailable()
        {
            bool result = true;

            Connect();
            string cmdtext = string.Format("UPDATE avito_categories SET available = 0;");
            MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
            cmd.ExecuteNonQuery();
            Close();

            return result;
        }

        // поставить статус недоступности всем разделам
        public static bool setSectionsNotAvailable()
        {
            bool result = true;

            Connect();
            string cmdtext = string.Format("UPDATE avito_sections SET available = 0;");
            MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
            cmd.ExecuteNonQuery();
            Close();

            return result;
        }

        // получить идентификатор категории по названию
        public static int getCategoryID(string category)
        {
            int result;

            Connect();
            try
            {
                string cmdtext = string.Format("SELECT id FROM avito_categories WHERE category='{0}';", category);
                MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                result = int.Parse(reader["id"].ToString());
            }
            catch
            {
                result = -1;
            }
            Close();

            return result;
        }

        // получить идентификатор категории по названию и названию родительской категории
        public static int getCategoryID(string category, string parentname)
        {
            int result;

            Connect();
            try
            {
                string cmdtext = string.Format("SELECT id FROM avito_categories WHERE category='{0}' AND parentname='{1}';", category, parentname);
                MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                result = int.Parse(reader["id"].ToString());
            }
            catch
            {
                result = -1;
            }
            Close();

            return result;
        }

        // **** **** объявления **** **** //
        // добавление объявления
        public static bool insertAvitoAd(AvitoAd ad)
        {
            bool result = true;

            Connect();
            string cmdtext = string.Format("INSERT INTO avito_ad (idlogin, name, description, price, size, status, categoryid) VALUES ({0}, '{1}', '{2}', '{3}', '{4}', {5}, {6});", 
                ad.idlogin,
                ad.name,
                ad.description,
                ad.price,
                ad.size,
                ad.status,
                ad.categoryid);
            MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
            cmd.ExecuteNonQuery();
            Close();

            return result;
        }

        // **** **** **** **** instagram **** **** **** **** //

        // получить все записи из таблицы instagram
        public static List<InstagramData> getInstagramList()
        {
            List<InstagramData> result = new List<InstagramData>();

            try
            {
                Connect();
                string cmdtext = "SELECT id, login, telephone, android_id, email, name, description FROM instagram";
                MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new InstagramData(int.Parse(reader["id"].ToString()),
                        reader["login"].ToString(),
                        "",
                        reader["telephone"].ToString(),
                        reader["android_id"].ToString(),
                        reader["email"].ToString(),
                        reader["name"].ToString(),
                        reader["description"].ToString()));
                }
                reader.Close();
                Close();
            }
            catch { }

            return result;
        }

        // получить запись из таблицы instagram по заданному логину
        public static InstagramData getInstagram(string login)
        {
            InstagramData result;

            Connect();
            string cmdtext = string.Format("SELECT id, login, password, telephone, android_id, email, name, description FROM instagram WHERE login='{0}';", login);
            MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            result = new InstagramData(int.Parse(reader["id"].ToString()),
                reader["login"].ToString(),
                reader["password"].ToString(),
                reader["telephone"].ToString(),
                reader["android_id"].ToString(),
                reader["email"].ToString(),
                reader["name"].ToString(),
                reader["description"].ToString());
            reader.Close();
            Close();

            return result;
        }

        // **** **** регистрация **** **** //

        // проверка существования записи по логину
        public static bool InstagramLoginExists(string login)
        {
            bool result = false;

            Connect();
            string cmdtext = string.Format("SELECT COUNT(login) AS c FROM instagram WHERE login='{0}'", login);
            MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            result = (int.Parse(reader["c"].ToString()) > 0);
            Close();

            return result;
        }

        // добавление записи
        public static bool InstagramAdd(string login, string password, string telephone, string android_id, string email, string description)
        {
            bool result = true;

            if (!InstagramLoginExists(login))
            {
                Connect();
                string cmdtext = string.Format("INSERT INTO instagram (login, password, telephone, android_id, email, name, description, regdate, status, used) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', now(), 0, 1)", login, password, telephone, android_id, email, "", description);
                MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
                cmd.ExecuteNonQuery();
                Close();
            }
            else
            {
                Connect();
                string cmdtext = string.Format("UPDATE instagram SET password='{1}', telephone='{2}', android_id='{3}', email='{4}', description='{5}', status = 0, used = 1 WHERE login = '{0}'", login, password, telephone, android_id, email, description);
                MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
                cmd.ExecuteNonQuery();
                Close();
            }

            return result;
        }

        // **** **** заполнение **** **** //

        // обновление записи аккаунта после заполнения
        public static bool updateInstagram(int id, string email, string website, string name, string description)
        {
            bool result = true;

            try
            {
                Connect();
                string cmdtext = string.Format("UPDATE instagram SET email='{1}', website='{2}', name='{3}', description='{4}', filldate=now() WHERE id={0}", id, email, website, name, description);
                MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
                cmd.ExecuteNonQuery();
                Close();
            }
            catch
            {
                result = false;
            }

            return result;
        }

        // обновление записи аккаунта после заполнения без указания email
        public static bool updateInstagram(int id, string website, string name, string description)
        {
            bool result = true;

            try
            {
                Connect();
                string cmdtext = string.Format("UPDATE instagram SET website='{1}', name='{2}', description='{3}', filldate=now() WHERE id={0}", id, website, name, description);
                MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
                cmd.ExecuteNonQuery();
                Close();
            }
            catch
            {
                result = false;
            }

            return result;
        }

        // обновление только адреса сайта
        public static bool updateInstagram(int id, string website)
        {
            bool result = true;

            try
            {
                Connect();
                string cmdtext = string.Format("UPDATE instagram SET website='{1}', filldate=now() WHERE id={0}", id, website);
                MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
                cmd.ExecuteNonQuery();
                Close();
            }
            catch
            {
                result = false;
            }

            return result;
        }

        // **** **** постинг **** **** //

        // запись данных о размещённом посте
        public static bool insertPost(int idlogin, string posttext, string parsedposttext)
        {
            bool result = true;

            try
            {
                Connect();
                string cmdtext = string.Format("INSERT INTO instagram_posts (idlogin, posttext, parsedposttext, postdate) VALUES ({0}, '{1}', '{2}', now());", idlogin, posttext, parsedposttext);
                MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
                cmd.ExecuteNonQuery();
                Close();
            }
            catch
            {
                result = false;
            }

            return result;
        }

        // **** **** подписка **** **** //

        // проверка существования записи о подписке
        public static bool isFollowingExists(string login)
        {
            bool result = false;

            Connect();
            string cmdtext = string.Format("SELECT COUNT(login) AS c FROM following WHERE login='{0}'", login);
            MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            result = (int.Parse(reader["c"].ToString()) > 0);
            Close();

            return result;
        }

        // вставка или обновление записи о подписке и связи аккаунта с подпиской
        public static bool insertFollowing(int instagram, string login, string name, string description, string website, int type, int countofposts, int countoffollowers, int countoffollowing)
        {
            bool result = true;

            if (!isFollowingExists(login))
            {
                try
                {
                    Connect();
                    string cmdtext = string.Format("INSERT INTO following (login, name, description, website, type, countofposts, countoffollowers, countoffollowing, dateadd) VALUES ('{0}', '{1}', '{2}', '{3}', {4}, {5}, {6}, {7}, now()); INSERT INTO instagram_following (login, following, datefollow) VALUES ({8}, (SELECT LAST_INSERT_ID()), now());",
                        login, name, description, website, type, countofposts, countoffollowers, countoffollowing, instagram);
                    MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
                    cmd.ExecuteNonQuery();
                    Close();
                }
                catch
                {
                    result = false;
                }
            }
            else
            {
                try
                {
                    Connect();
                    string cmdtext = string.Format("UPDATE following SET name='{1}', description='{2}', website='{3}', countofposts={4}, countoffollowers={5}, countoffollowing={6}, lastupdate=now() WHERE login='{0}'; INSERT INTO instagram_following (login, following, datefollow) VALUES ({7}, (SELECT id FROM following WHERE login='{0}'), now());",
                        login, name, description, website, countofposts, countoffollowers, countoffollowing, instagram);
                    MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
                    cmd.ExecuteNonQuery();
                    Close();
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        // вставка записи только с логиным подписки
        public static bool insertFollowing(int instagram, string login)
        {
            bool result = true;

            try
            {
                Connect();
                string cmdtext = string.Format("INSERT INTO following (login, dateadd) VALUES ('{0}', now()); INSERT INTO instagram_following (login, following, datefollow) VALUES ({1}, (SELECT id FROM following WHERE login='{0}'), now());", login, instagram);
                MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
                cmd.ExecuteNonQuery();
                Close();
            }
            catch
            {
                result = false;
            }

            return result;
        }

        // **** **** общие функции **** **** //

        // получить все свободные аккаунты по заданному интервалу освобождения
        public static List<string> getAllFreeInstaAccounts(int minutes)
        {
            List<string> result = new List<string>();

            Connect();
            try
            {
                //string cmdtext = string.Format("SELECT login FROM instagram WHERE website <> '' AND name <> '' AND status = 0 AND used = 0 AND ((hour(timediff(now(), useddate)) * 60 + minute(timediff(now(), useddate))) >= '{0}' OR useddate is null);", minutes);
                string cmdtext = string.Format("SELECT login FROM instagram WHERE name <> '' AND status = 0 AND used = 0 AND ((hour(timediff(now(), useddate)) * 60 + minute(timediff(now(), useddate))) >= '{0}' OR useddate is null);", minutes);
                MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    result.Add(reader["login"].ToString());
            }
            catch { }
            Close();

            return result;
        }

        // получить все свободные аккаунты
        public static List<string> getAllFreeInstaAccounts()
        {
            List<string> result = new List<string>();

            Connect();
            try
            {
                string cmdtext = string.Format("SELECT login FROM instagram WHERE email <> '' AND name <> '' AND status=0 AND used=0;");
                MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    result.Add(reader["login"].ToString());
            }
            catch { }
            Close();

            return result;
        }

        // получить первый свободный аккаунт по заданному интервалу освобождения
        public static string getFirstFreeInstaAccount(int minutes)
        {
            string result = "";

            Connect();
            try
            {
                string cmdtext = string.Format("SELECT login FROM instagram WHERE email <> '' AND name <> '' AND status=0 AND used=0 AND (hour(timediff(now(), useddate)) * 60 + minute(timediff(now(), useddate))) >= '{0}' order by useddate desc LIMIT 1;", minutes);
                MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                result = reader["login"].ToString();
            }
            catch
            {
                result = "";
            }
            Close();

            return result;
        }

        // получить первый свободный аккаунт
        public static string getFirstFreeInstaAccount()
        {
            string result = "";

            Connect();
            try
            {
                string cmdtext = string.Format("SELECT login FROM instagram WHERE email <> '' AND status=0 AND used=0 LIMIT 1;");
                MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                result = reader["login"].ToString();
            }
            catch
            {
                result = "";
            }
            Close();

            return result;
        }

        // поставить статус занятости аккаунту с указанием вызвавшей задачи и даты
        public static bool setUsedInstaAccount(int idlogin, string task)
        {
            bool result = true;

            try
            {
                Connect();
                string cmdtext = string.Format("UPDATE instagram SET used=1, usedtask='{1}', useddate=now() WHERE id={0};", idlogin, task);
                MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
                cmd.ExecuteNonQuery();
                Close();
            }
            catch
            {
                result = false;
            }

            return result;
        }

        // поставить статус освобождения с указанием даты
        public static bool setFreeInstaAccount(int idlogin)
        {
            bool result = true;

            try
            {
                Connect();
                string cmdtext = string.Format("UPDATE instagram SET used=0, usedtask='', useddate=now() WHERE id={0};", idlogin);
                MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
                cmd.ExecuteNonQuery();
                Close();
            }
            catch
            {
                result = false;
            }

            return result;
        }

        // поставить статус освобождения с указанием даты с нулевым временем
        public static bool setFreeInstaAccount(int idlogin, bool nulltime)
        {
            bool result = true;

            try
            {
                Connect();
                string cmdtext = string.Format("UPDATE instagram SET used=0, usedtask='', useddate=date(now()) WHERE id={0};", idlogin);
                MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
                cmd.ExecuteNonQuery();
                Close();
            }
            catch
            {
                result = false;
            }

            return result;
        }

        // сохранение результата выполнения задачи
        public static bool insertInstaAccountTask(int idlogin, string task, string taskresult)
        {
            bool result = true;

            try
            {
                Connect();
                string cmdtext = string.Format("INSERT INTO instagram_tasks (idlogin, usedtask, useddate, result) VALUES ({0}, '{1}', now(), '{2}');", idlogin, task, taskresult);
                MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
                cmd.ExecuteNonQuery();
                Close();
            }
            catch
            {
                result = false;
            }

            return result;
        }

        // поставить статус недоступности аккаунту (для случаев незавершённой регистрации или неудачной авторизации)
        public static bool setAccountStatus(int id, int status)
        {
            bool result = true;

            try
            {
                Connect();
                string cmdtext = string.Format("UPDATE instagram SET status='{1}' WHERE id={0}", id, status);
                MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
                cmd.ExecuteNonQuery();
                Close();
            }
            catch
            {
                result = false;
            }

            return result;
        }
    }
}