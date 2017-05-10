using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSGTools.Insta
{
    public static class InstaAccountsBase
    {
        // получить все записи из таблицы instagram
        public static List<InstagramData> getInstagramList()
        {
            List<InstagramData> result = new List<InstagramData>();

            try
            {
                AccountsBase.Connect();
                string cmdtext = "SELECT id, login, telephone, android_id, email, name, description FROM instagram";
                MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
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
                AccountsBase.Close();
            }
            catch { }

            return result;
        }

        // получить запись из таблицы instagram по заданному логину
        public static InstagramData getInstagram(string login)
        {
            InstagramData result;

            AccountsBase.Connect();
            string cmdtext = string.Format("SELECT id, login, password, telephone, android_id, email, name, description FROM instagram WHERE login='{0}';", login);
            MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
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
            AccountsBase.Close();

            return result;
        }

        // **** **** регистрация **** **** //

        // проверка существования записи по логину
        public static bool InstagramLoginExists(string login)
        {
            bool result = false;

            AccountsBase.Connect();
            string cmdtext = string.Format("SELECT COUNT(login) AS c FROM instagram WHERE login='{0}'", login);
            MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            result = (int.Parse(reader["c"].ToString()) > 0);
            AccountsBase.Close();

            return result;
        }

        // добавление записи
        public static bool InstagramAdd(string login, string password, string telephone, string android_id, string email, string description)
        {
            bool result = true;

            if (!InstagramLoginExists(login))
            {
                AccountsBase.Connect();
                string cmdtext = string.Format("INSERT INTO instagram (login, password, telephone, android_id, email, name, description, regdate, status, used) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', now(), 0, 1)", login, password, telephone, android_id, email, "", description);
                MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
                cmd.ExecuteNonQuery();
                AccountsBase.Close();
            }
            else
            {
                AccountsBase.Connect();
                string cmdtext = string.Format("UPDATE instagram SET password='{1}', telephone='{2}', android_id='{3}', email='{4}', description='{5}', regdate=now(), status = 0, used = 1 WHERE login = '{0}'", login, password, telephone, android_id, email, description);
                MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
                cmd.ExecuteNonQuery();
                AccountsBase.Close();
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
                AccountsBase.Connect();
                string cmdtext = string.Format("UPDATE instagram SET email='{1}', website='{2}', name='{3}', description='{4}', filldate=now() WHERE id={0}", id, email, website, name, description);
                MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
                cmd.ExecuteNonQuery();
                AccountsBase.Close();
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
                AccountsBase.Connect();
                string cmdtext = string.Format("UPDATE instagram SET website='{1}', name='{2}', description='{3}', filldate=now() WHERE id={0}", id, website, name, description);
                MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
                cmd.ExecuteNonQuery();
                AccountsBase.Close();
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
                AccountsBase.Connect();
                string cmdtext = string.Format("UPDATE instagram SET website='{1}', filldate=now() WHERE id={0}", id, website);
                MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
                cmd.ExecuteNonQuery();
                AccountsBase.Close();
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
                AccountsBase.Connect();
                string cmdtext = string.Format("INSERT INTO instagram_posts (idlogin, posttext, parsedposttext, postdate) VALUES ({0}, '{1}', '{2}', now());", idlogin, posttext, parsedposttext);
                MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
                cmd.ExecuteNonQuery();
                AccountsBase.Close();
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

            AccountsBase.Connect();
            string cmdtext = string.Format("SELECT COUNT(login) AS c FROM following WHERE login='{0}'", login);
            MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            result = (int.Parse(reader["c"].ToString()) > 0);
            AccountsBase.Close();

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
                    AccountsBase.Connect();
                    string cmdtext = string.Format("INSERT INTO following (login, name, description, website, type, countofposts, countoffollowers, countoffollowing, dateadd) VALUES ('{0}', '{1}', '{2}', '{3}', {4}, {5}, {6}, {7}, now()); INSERT INTO instagram_following (login, following, datefollow) VALUES ({8}, (SELECT LAST_INSERT_ID()), now());",
                        login, name, description, website, type, countofposts, countoffollowers, countoffollowing, instagram);
                    MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
                    cmd.ExecuteNonQuery();
                    AccountsBase.Close();
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
                    AccountsBase.Connect();
                    string cmdtext = string.Format("UPDATE following SET name='{1}', description='{2}', website='{3}', countofposts={4}, countoffollowers={5}, countoffollowing={6}, lastupdate=now() WHERE login='{0}'; INSERT INTO instagram_following (login, following, datefollow) VALUES ({7}, (SELECT id FROM following WHERE login='{0}'), now());",
                        login, name, description, website, countofposts, countoffollowers, countoffollowing, instagram);
                    MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
                    cmd.ExecuteNonQuery();
                    AccountsBase.Close();
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
                AccountsBase.Connect();
                string cmdtext = string.Format("INSERT INTO following (login, dateadd) VALUES ('{0}', now()); INSERT INTO instagram_following (login, following, datefollow) VALUES ({1}, (SELECT id FROM following WHERE login='{0}'), now());", login, instagram);
                MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
                cmd.ExecuteNonQuery();
                AccountsBase.Close();
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

            AccountsBase.Connect();
            try
            {
                //string cmdtext = string.Format("SELECT login FROM instagram WHERE website <> '' AND name <> '' AND status = 0 AND used = 0 AND ((hour(timediff(now(), useddate)) * 60 + minute(timediff(now(), useddate))) >= '{0}' OR useddate is null);", minutes);
                string cmdtext = string.Format("SELECT login FROM instagram WHERE name <> '' AND status = 0 AND used = 0 AND ((hour(timediff(now(), useddate)) * 60 + minute(timediff(now(), useddate))) >= '{0}' OR useddate is null);", minutes);
                MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    result.Add(reader["login"].ToString());
            }
            catch { }
            AccountsBase.Close();

            return result;
        }

        // получить все свободные аккаунты
        public static List<string> getAllFreeInstaAccounts()
        {
            List<string> result = new List<string>();

            AccountsBase.Connect();
            try
            {
                string cmdtext = string.Format("SELECT login FROM instagram WHERE email <> '' AND name <> '' AND status=0 AND used=0;");
                MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    result.Add(reader["login"].ToString());
            }
            catch { }
            AccountsBase.Close();

            return result;
        }

        // получить первый свободный аккаунт по заданному интервалу освобождения
        public static string getFirstFreeInstaAccount(int minutes)
        {
            string result = "";

            AccountsBase.Connect();
            try
            {
                string cmdtext = string.Format("SELECT login FROM instagram WHERE email <> '' AND name <> '' AND status=0 AND used=0 AND (hour(timediff(now(), useddate)) * 60 + minute(timediff(now(), useddate))) >= '{0}' order by useddate desc LIMIT 1;", minutes);
                MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                result = reader["login"].ToString();
            }
            catch
            {
                result = "";
            }
            AccountsBase.Close();

            return result;
        }

        // получить первый свободный аккаунт
        public static string getFirstFreeInstaAccount()
        {
            string result = "";

            AccountsBase.Connect();
            try
            {
                string cmdtext = string.Format("SELECT login FROM instagram WHERE email <> '' AND status=0 AND used=0 LIMIT 1;");
                MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                result = reader["login"].ToString();
            }
            catch
            {
                result = "";
            }
            AccountsBase.Close();

            return result;
        }

        // поставить статус занятости аккаунту с указанием вызвавшей задачи и даты
        public static bool setUsedInstaAccount(int idlogin, string task)
        {
            bool result = true;

            try
            {
                AccountsBase.Connect();
                string cmdtext = string.Format("UPDATE instagram SET used=1, usedtask='{1}', useddate=now() WHERE id={0};", idlogin, task);
                MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
                cmd.ExecuteNonQuery();
                AccountsBase.Close();
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
                AccountsBase.Connect();
                string cmdtext = string.Format("UPDATE instagram SET used=0, usedtask='', useddate=now() WHERE id={0};", idlogin);
                MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
                cmd.ExecuteNonQuery();
                AccountsBase.Close();
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
                AccountsBase.Connect();
                string cmdtext = string.Format("UPDATE instagram SET used=0, usedtask='', useddate=date(now()) WHERE id={0};", idlogin);
                MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
                cmd.ExecuteNonQuery();
                AccountsBase.Close();
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
                AccountsBase.Connect();
                string cmdtext = string.Format("INSERT INTO instagram_tasks (idlogin, usedtask, useddate, result) VALUES ({0}, '{1}', now(), '{2}');", idlogin, task, taskresult);
                MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
                cmd.ExecuteNonQuery();
                AccountsBase.Close();
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
                AccountsBase.Connect();
                string cmdtext = string.Format("UPDATE instagram SET status='{1}' WHERE id={0}", id, status);
                MySqlCommand cmd = new MySqlCommand(cmdtext, AccountsBase.Connection);
                cmd.ExecuteNonQuery();
                AccountsBase.Close();
            }
            catch
            {
                result = false;
            }

            return result;
        }
    }
}