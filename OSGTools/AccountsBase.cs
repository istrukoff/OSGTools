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
    }
}