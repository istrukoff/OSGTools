using MySql.Data;
using MySql.Data.MySqlClient;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OSGTools
{
    public static class ADBBase
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        private static XmlDocument xml = new XmlDocument();
        private static string xml_path = "dbconnections.xml";

        private static string server { get; set; }
        private static string dbname { get; set; }
        private static string user { get; set; }
        private static string password { get; set; }

        private static MySqlConnection connection = null;
        public static MySqlConnection Connection
        {
            get { return connection; }
        }

        private static bool LoadDBSettingsFromXML()
        {
            bool result = false;

            try
            {
                xml.Load(string.Format(@"{0}\{1}", @"C:\machine\configs", xml_path));
            }
            catch
            {
                result = false;
                return result;
            }

            foreach (XmlElement element in xml.GetElementsByTagName("adbbase"))
            {
                if (element.Attributes["available"] == null)
                {
                    result = false;
                    continue;
                }

                if (element.Attributes["available"].InnerText == "0")
                {
                    result = false;
                    continue;
                }

                if (element.Attributes["available"].InnerText == "1")
                {
                    foreach (XmlElement e in element)
                    {
                        if (e.Name == "server")
                            server = e.InnerText;
                        if (e.Name == "dbname")
                            dbname = e.InnerText;
                        if (e.Name == "user")
                            user = e.InnerText;
                        if (e.Name == "password")
                            password = e.InnerText;
                    }

                    result = true;

                    break;
                }
            }

            return result;
        }

        private static bool Connect()
        {
            bool result = true;

            if (LoadDBSettingsFromXML())
            {
                if (String.IsNullOrEmpty(dbname))
                    result = false;
                string connstring = string.Format("Server={0}; database={1}; UID={2}; password={3}", server, dbname, user, password);
                connection = new MySqlConnection(connstring);
                connection.Open();
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }

        private static void Close()
        {
            connection.Close();
        }

        // получить список всех устройств из БД
        [STAThread]
        public static List<Device> getDevicesList()
        {
            List<Device> result = new List<Device>();

            try
            {
                Connect();
                string cmdtext = "SELECT d.id as id, d.dev_id as dev_id, d.adb_status as adb_status, d.used as used, d.appium_port as appium_port, d.bootstrap_port as bootstrap_port, d.pid as pid FROM devices as d";
                MySqlCommand cmd = new MySqlCommand(cmdtext, connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new Device(int.Parse(reader["id"].ToString()),
                        reader["dev_id"].ToString(),
                        reader["adb_status"].ToString(),
                        bool.Parse(reader["used"].ToString()),
                        int.Parse(reader["appium_port"].ToString()),
                        int.Parse(reader["bootstrap_port"].ToString()),
                        int.Parse(reader["pid"].ToString())));
                }
                reader.Close();
                Close();
            }
            catch (Exception e)
            {
                log.Error(e.ToString());
            }

            return result;
        }
    }
}