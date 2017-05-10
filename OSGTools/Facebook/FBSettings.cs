using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OSGTools.FB
{
    public class Settings
    {
        public Settings() { }

        public string ussd { get; set; }

        public string pathFirstName { get; set; }

        public string pathLastName { get; set; }

        public int sex { get; set; }

        public int birthday { get; set; }
    }

    public static class FBSettings
    {
        // xml-файл с настройками
        private static XmlDocument xml = new XmlDocument();
        //private static string xml_path = "fbreg.xml";

        // функция загрузки настроек из xml-файла
        [STAThread]
        public static Settings LoadSettingsFromXML(string xml_path)
        {
            Settings result = new Settings();

            try
            {
                //xml.Load(string.Format(@"{0}\{1}", Environment.CurrentDirectory, xml_path));
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
                        if (e.Name == "pathFirstName")
                            result.pathFirstName = e.InnerText;
                        if (e.Name == "pathLastName")
                            result.pathLastName = e.InnerText;
                        if (e.Name == "sex")
                            result.sex = int.Parse(e.InnerText);
                        if (e.Name == "birthday")
                            result.birthday = int.Parse(e.InnerText);
                    }

                    break;
                }
            }

            return result;
        }
    }
}