using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OSGTools.InstaFB
{
    public class Settings
    {
        public Settings() { }

        public string ussd { get; set; }

        public string telephone { get; set; }

        public string android_id { get; set; }

        public string proxyIP { get; set; }

        public string pathProxyPorts { get; set; }

        public string pathFirstName { get; set; }

        public string FirstName { get; set; }

        public string pathLastName { get; set; }

        public string LastName { get; set; }

        public int sex { get; set; }

        public int birthday { get; set; }

        public string pathAvatars { get; set; }

        public string pathEmails { get; set; }

        public string pathWebSites { get; set; }

        public string pathInstaInfo { get; set; }

        public string pathNames { get; set; }

        public string pathPictures { get; set; }

        public string pathPostText { get; set; }

        public string pathOutputInsta { get; set; }
    }

    public class InstaFBSettings
    {
        // xml-файл с настройками
        private static XmlDocument xml = new XmlDocument();

        // функция загрузки настроек из xml-файла
        [STAThread]
        public static Settings LoadSettingsFromXML(string xml_path)
        {
            Settings result = new Settings();

            try
            {
                xml.Load(xml_path);
            }
            catch
            {
                return null;
            }

            foreach (XmlElement element in xml.GetElementsByTagName("account"))
            {
                if (element.Attributes["available"] == null)
                {
                    continue;
                }

                if (element.Attributes["available"].InnerText == "0")
                {
                    continue;
                }

                if (element.Attributes["available"].InnerText == "1")
                {
                    foreach (XmlElement e in element)
                    {
                        if (e.Name == "ussd")
                            result.ussd = e.InnerText;
                        if (e.Name == "proxyIP")
                            result.proxyIP = e.InnerText;
                        if (e.Name == "pathProxyPorts")
                            result.pathProxyPorts = e.InnerText;
                        if (e.Name == "pathFirstName")
                            result.pathFirstName = e.InnerText;
                        if (e.Name == "pathLastName")
                            result.pathLastName = e.InnerText;
                        if (e.Name == "sex")
                            result.sex = int.Parse(e.InnerText);
                        if (e.Name == "birthday")
                            result.birthday = int.Parse(e.InnerText);
                        if (e.Name == "pathAvatars")
                            result.pathAvatars = e.InnerText;
                        if (e.Name == "pathEmails")
                            result.pathEmails = e.InnerText;
                        if (e.Name == "pathWebSites")
                            result.pathWebSites = e.InnerText;
                        if (e.Name == "pathInstaInfo")
                            result.pathInstaInfo = e.InnerText;
                        if (e.Name == "pathInstaNames")
                            result.pathNames = e.InnerText;
                        if (e.Name == "pathPictures")
                            result.pathPictures = e.InnerText;
                        if (e.Name == "pathPostText")
                            result.pathPostText = e.InnerText;
                        if (e.Name == "pathOutputInsta")
                            result.pathOutputInsta = e.InnerText;
                    }

                    break;
                }
            }

            return result;
        }
    }
}