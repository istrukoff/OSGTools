using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace OSGTools
{
    public static class Functions
    {
        public static string osg_regex = "[^a-zA-Z0-9а-яА-Я ,.-=()&!?/]";

        // функция для преобразования строки количества в число
        public static double getCountFromString(string value)
        {
            double result = 0;

            if (value.Contains("k"))
                result = double.Parse(value.Substring(0, value.Length - 1)) * 1000;
            else
                if (value.Contains("m"))
                result = double.Parse(value.Substring(0, value.Length - 1)) * 1000000;
            else
                result = double.Parse(value.Substring(0, value.Length));

            return result;
        }

        // завершение дерева процессов
        /// <summary>
        /// Завершить дерево процессов
        /// </summary>
        /// <param name="pid">Родительский процесс</param>
        public static bool KillProcessTree(int pid)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection objcollection = searcher.Get();

            foreach (ManagementObject obj in objcollection)
            {
                KillProcessTree(Convert.ToInt32(obj["ProcessID"]));
            }

            try
            {
                Process.GetProcessById(pid).Kill();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // генерация случайного имени файла
        /// <summary>
        /// Генерация случайного имени файла
        /// </summary>
        /// <param name="minlength">Минимальная длина (по умолчанию = 8)</param>
        /// <param name="maxlength">Максимальная длина (по умолчанию = 24)</param>
        /// <param name="extension">Расширение файла (по умолчанию = jpg)</param>
        /// <returns>Строка для имени файла длиной из указанного интервала и с указанным расширением через точку</returns>
        public static string generateFileName(int minlength = 8, int maxlength = 24, string extension = null)
        {
            StringBuilder result = new StringBuilder();

            Random r_length = new Random();
            int l = r_length.Next(minlength, maxlength);

            Random r_char = new Random();

            while (result.Length < l)
            {
                char c = (char)r_char.Next(48, 122);
                if (Char.IsLetterOrDigit(c))
                    result.Append(c);
            }

            return string.Format("{0}.{1}", result.ToString(), extension ?? "jpg");
        }

        // генерация случайного пароля
        /// <summary>
        /// Генерация случайного пароля
        /// </summary>
        /// <param name="minlength">Минимальная длина (по умолчанию = 8)</param>
        /// <param name="maxlength">Максимальная длина (по умолчанию = 24)</param>
        /// <param name="usespecchar">Использование специальных символов (по умолчанию = нет)</param>
        /// <returns>Строка для имени файла длиной из указанного интервала и с указанным расширением через точку</returns>
        public static string generatePassword(int minlength = 8, int maxlength = 24, bool usespecchar = false)
        {
            StringBuilder result = new StringBuilder();

            Random r_length = new Random();
            int l = r_length.Next(minlength, maxlength);

            Random r_char = new Random();

            while (result.Length < l)
            {
                char c = (char)r_char.Next(33, 125);
                if (!usespecchar)
                {
                    if (Char.IsLetterOrDigit(c))
                        result.Append(c);
                }
                else
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }
    }
}