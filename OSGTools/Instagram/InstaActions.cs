using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Support.UI;
using StringAnalizeLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace OSGTools.Insta
{
    public static class InstaActions
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        // переменные для заполнения
        #region fill variables
        // каталог с аватарками
        private static List<string> listOfAvatars = new List<string>();
        // выбранный файл аватарки
        private static FileInfo fileAvatar;
        // каталог с аватарками на телефоне
        private static string pathTargetAvatars = "/storage/sdcard1/instagram/avatars/";

        // файл с электронными адресами
        private static StreamReader readerEmails;
        private static List<string> listEmails = new List<string>();
        private static StreamWriter writerEmails;

        // файл с web-сайтами
        private static StreamReader readerWebSites;
        private static List<string> listWebSites = new List<string>();
        private static StreamWriter writerWebSites;

        // файл с "именами"
        private static StreamReader readerNames;
        private static List<string> listNames = new List<string>();
        private static StreamWriter writerNames;

        // строка для записи в поле информации в Instagram
        private static StreamReader readerInstaInfo;
        #endregion

        // переменные для постинга
        #region post variables
        // количество загружаемых изображений и количество размещаемых постов
        private static int pictures_count = 9;

        // каталог с изображениями
        //private static string pathPictures;
        private static List<string> listOfDirectories = new List<string>(); // список папок в каталоге
        private static List<string> listOfPictures = new List<string>(); // список картинок в папке
        // выбранная папка
        private static DirectoryInfo dirKross;
        // выбранный файл
        private static FileInfo fileKross;
        private static string fileKrossName;
        // каталог с изображениями на телефоне
        private static string pathTargetKross = "/storage/sdcard1/instagram/kross/";

        // строка для записи в поле описания поста
        //private static string pathPostText;
        private static StreamReader readerPostText;
        private static string postText;
        private static string parsed_posttext;
        #endregion

        // загрузить случайный электронный адрес из файла
        private static string getRandomEmail(string path)
        {
            Random r = new Random();
            string t = "";

            using (readerEmails = new StreamReader(path))
            {
                while (true)
                {
                    t = readerEmails.ReadLine();
                    if (t != null)
                        listEmails.Add(t);
                    else
                        break;
                }
            }

            if (listEmails.Count > 0)
                return listEmails[r.Next(0, listEmails.Count)].ToString();
            else
                return "";
        }

        // удалить выбранный электронный адрес из файла
        private static void removeEmailFromFile(string path, string email)
        {
            using (writerEmails = new StreamWriter(path, false))
            {
                foreach (string t in listEmails)
                    if (t != email)
                        writerEmails.WriteLine(t);
            }
        }

        // загрузить случайный web-сайт из файла
        private static string getRandomWebSite(string path)
        {
            Random r = new Random();
            string t = "";

            using (readerWebSites = new StreamReader(path))
            {
                while (true)
                {
                    t = readerWebSites.ReadLine();
                    if (t != null)
                        listWebSites.Add(t);
                    else
                        break;
                }
            }

            if (listWebSites.Count > 0)
                return listWebSites[r.Next(0, listWebSites.Count)].ToString();
            else
                return "";
        }

        // удалить выбранный веб-сайт из файла
        private static void removeWebSiteFromFile(string path, string website)
        {
            using (writerWebSites = new StreamWriter(path, false))
            {
                foreach (string t in listWebSites)
                    if (t != website)
                        writerWebSites.WriteLine(t);
            }
        }

        // загрузить случайное "имя" из файла
        private static string getRandomName(string path)
        {
            Random r = new Random();
            string t = "";

            using (readerNames = new StreamReader(path))
            {
                while (true)
                {
                    t = readerNames.ReadLine();
                    if (t != null)
                        listNames.Add(t);
                    else
                        break;
                }
            }

            if (listNames.Count > 0)
                return listNames[r.Next(0, listNames.Count)].ToString();
            else
                return "";
        }

        // удалить выбранное "имя" из файла
        private static void removeNameFromFile(string path, string name)
        {
            using (writerNames = new StreamWriter(path, false))
            {
                foreach (string t in listNames)
                    if (t != name)
                        writerNames.WriteLine(t);
            }
        }

        // загрузить имя и описание из файла
        private static void getInstaInfo(string path, InstagramData insta)
        {
            using (readerInstaInfo = new StreamReader(path))
            {
                insta.Name = readerInstaInfo.ReadLine() ?? "";
                insta.Description = readerInstaInfo.ReadLine() ?? "";
            }
        }

        // загрузить из файла описание поста
        private static void getPostText(string path)
        {
            using (readerPostText = new StreamReader(path))
            {
                postText = readerPostText.ReadLine() ?? "";
            }
        }

        // функция регистрации нового аккаунта
        // возвращает зарегистрированный аккаунт
        [STAThread]
        public static InstagramData Registration(AndroidDriver<IWebElement> driver, WebDriverWait wait, InstaFB.Settings settings)
        {
            InstagramData insta = new InstagramData();

            string fullname = string.Format("{0} {1}", settings.FirstName, settings.LastName);
            log.Info(string.Format("Используется имя: {0}", fullname));

            insta.Password = Functions.generatePassword();
            log.Info(string.Format("Сгенерировали пароль: {0}", insta.Password));

            // запускаем Instagram
            driver.PressKeyCode(AndroidKeyCode.Home);
            driver.StartActivity("com.instagram.android", ".activity.MainTabActivity");
            Thread.Sleep(5000);
            log.Info("Запускаем Instagram.");

            #region **** instagramreg ****
            wait.Timeout = new TimeSpan(0, 0, 30);
            Thread.Sleep(1000);

            string xml = driver.PageSource;
            string test_xml = "";
            int page = 0;

            #region **** first variant ****
            test_xml = Regex.Match(xml, @"Войти как ").Value;
            if (test_xml != "")
            {
                page = 1;
                log.Info(string.Format("{0} Найдена надпись 'Войти как'.", page));
                try
                {
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'right_button')]")));
                    driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'right_button')]").Click();
                    log.Info("Кликнули кнопку: 'Зарегистрироваться'.");

                }
                catch
                {
                    log.Info("Второй раз кликнули кнопку: 'Зарегистрироваться'.");
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'right_button')]")));
                    driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'right_button')]").Click();
                }
                // регистрация с электронным адресом или номером телефона
                try
                {
                    log.Info("Кликнули кнопку: 'Регистрация с электронным адресом или номером телефона'.");
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'sign_up_with_email_or_phone')]")));
                    driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'sign_up_with_email_or_phone')]").Click();
                }
                catch
                {
                    try
                    {
                        wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'button_positive')]")));
                        driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'button_positive')]").Click();
                    }
                    catch
                    {
                        driver.PressKeyCode(AndroidKeyCode.Back);
                        log.Info("Второй раз кликнули кнопку: 'Регистрация с электронным адресом или номером телефона'.");
                        wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'sign_up_with_email_or_phone')]")));
                        driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'sign_up_with_email_or_phone')]").Click();
                    }
                }
            }
            #endregion

            #region **** second variant ****
            xml = driver.PageSource;
            test_xml = Regex.Match(xml, @"Забыли").Value;
            if (test_xml != "")
            {
                page = 2;
                log.Info(string.Format("{0} Найдена надпись 'Забыли'.", page));
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Еще нет аккаунта? Зарегистрируйтесь.')]")));
                driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Еще нет аккаунта? Зарегистрируйтесь.')]").Click();
                log.Info("Кликнули кнопку: 'Еще нет аккаунта? Зарегистрируйтесь.'.");
            }
            #endregion

            #region **** 3-rd variant ****
            xml = driver.PageSource;
            test_xml = Regex.Match(xml, @"Зарегистрируйтесь, чтобы смотреть").Value;
            if (test_xml != "")
            {
                page = 3;
                log.Info(string.Format("{0} Найдена надпись 'зарегистрируйтесь, чтобы смотреть'.", page));
                // регистрация с электронным адресом или номером телефона
                log.Info("Кликнули кнопку: 'Регистрация с электронным адресом или номером телефона'.");
                try
                {
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'sign_up_with_email_or_phone')]")));
                    driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'sign_up_with_email_or_phone')]").Click();
                }
                catch
                {
                    try
                    {
                        wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'button_positive')]")));
                        driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'button_positive')]").Click();
                    }
                    catch
                    {
                        driver.PressKeyCode(AndroidKeyCode.Back);
                        log.Info("Второй раз кликнули кнопку: 'Регистрация с электронным адресом или номером телефона'.");
                        wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'sign_up_with_email_or_phone')]")));
                        driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'sign_up_with_email_or_phone')]").Click();
                    }
                }
            }
            #endregion

            // вводим полученный номер телефона
            #region **** phonenumber input ****
            log.Info(string.Format("Вводим полученный номер телефона: {0}.", settings.telephone));
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.EditText[contains(@resource-id, 'phone_field')]")));
            driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'phone_field')]").SendKeys(settings.telephone);
            int i = 0;
            bool repeat_next = false;
            do
            {
                log.Info("Жмем далее.");
                // далее
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'next_button')]")));
                driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'next_button')]").Click();
                Thread.Sleep(2000);
                i++;
                if (i > 5) { break; }
                try
                {
                    repeat_next = driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Подождите несколько минут, прежде чем пытаться снова.')]").Displayed;
                }
                catch
                {
                    repeat_next = false;
                }
            }
            while (repeat_next);
            #endregion

            #region **** message with code ****
            log.Info("Получаем код подтверждения из сообщений. Ждём 20 + 60 секунд.");
            wait.Timeout = new TimeSpan(0, 1, 0);
            Thread.Sleep(20000);
            // получаем код подтверждения из сообщений
            driver.StartActivity("com.android.mms", ".ui.ConversationList");
            try
            {
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView [contains(@resource-id, 'subject')]")));
            }
            catch (WebDriverException e)
            {
                log.Error("Не пришло сообщение с кодом подтверждения.");
                log.Error(e.ToString());
                return null;
            }
            string t_code = driver.FindElementByXPath("//android.widget.TextView [contains(@resource-id, 'subject')]").Text;
            string code = Regex.Match(t_code, @"\d\d\d \d\d\d").Value;
            code = code.Replace(" ", "");
            log.Info(string.Format("Получили код подтверждения из сообщений: {0}.", code));
            log.Info("Удаляем все сообщения после получения.");
            // удаляем все сообщения после получения
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.ImageButton[contains(@content-desc, 'Ещё')]")));
            driver.FindElementByXPath("//android.widget.ImageButton[contains(@content-desc, 'Ещё')]").Click();
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Удалить все цепочки')]")));
            driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Удалить все цепочки')]").Click();
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Button[contains(@resource-id, 'button1')]")));
            driver.FindElementByXPath("//android.widget.Button[contains(@resource-id, 'button1')]").Click();

            // возвращаемся в Instagram
            wait.Timeout = new TimeSpan(0, 0, 30);
            log.Info("Возвращаемся в Instagram.");
            driver.PressKeyCode(AndroidKeyCode.Keycode_APP_SWITCH);
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Instagram')]")));
            driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Instagram')]").Click();
            #endregion

            // вводим код подтверждения
            try
            {
                log.Info(string.Format("Вводим код подтверждения: {0}.", code));
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.EditText[contains(@resource-id, 'confirmation_field')]")));
                driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'confirmation_field')]").Click();

                #region **** code input ****
                switch (code[0])
                {
                    case '0': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_0); break;
                    case '1': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_1); break;
                    case '2': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_2); break;
                    case '3': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_3); break;
                    case '4': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_4); break;
                    case '5': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_5); break;
                    case '6': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_6); break;
                    case '7': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_7); break;
                    case '8': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_8); break;
                    case '9': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_9); break;
                }
                Thread.Sleep(500);
                switch (code[1])
                {
                    case '0': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_0); break;
                    case '1': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_1); break;
                    case '2': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_2); break;
                    case '3': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_3); break;
                    case '4': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_4); break;
                    case '5': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_5); break;
                    case '6': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_6); break;
                    case '7': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_7); break;
                    case '8': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_8); break;
                    case '9': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_9); break;
                }
                Thread.Sleep(500);
                switch (code[2])
                {
                    case '0': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_0); break;
                    case '1': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_1); break;
                    case '2': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_2); break;
                    case '3': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_3); break;
                    case '4': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_4); break;
                    case '5': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_5); break;
                    case '6': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_6); break;
                    case '7': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_7); break;
                    case '8': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_8); break;
                    case '9': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_9); break;
                }
                Thread.Sleep(500);
                switch (code[3])
                {
                    case '0': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_0); break;
                    case '1': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_1); break;
                    case '2': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_2); break;
                    case '3': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_3); break;
                    case '4': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_4); break;
                    case '5': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_5); break;
                    case '6': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_6); break;
                    case '7': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_7); break;
                    case '8': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_8); break;
                    case '9': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_9); break;
                }
                Thread.Sleep(500);
                switch (code[4])
                {
                    case '0': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_0); break;
                    case '1': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_1); break;
                    case '2': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_2); break;
                    case '3': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_3); break;
                    case '4': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_4); break;
                    case '5': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_5); break;
                    case '6': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_6); break;
                    case '7': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_7); break;
                    case '8': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_8); break;
                    case '9': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_9); break;
                }
                Thread.Sleep(500);
                switch (code[5])
                {
                    case '0': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_0); break;
                    case '1': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_1); break;
                    case '2': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_2); break;
                    case '3': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_3); break;
                    case '4': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_4); break;
                    case '5': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_5); break;
                    case '6': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_6); break;
                    case '7': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_7); break;
                    case '8': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_8); break;
                    case '9': driver.PressKeyCode(AndroidKeyCode.KeycodeNumpad_9); break;
                }
                #endregion

                Thread.Sleep(1000);

                //wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.EditText[contains(@resource-id, 'confirmation_field')]")));
                //driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'confirmation_field')]").SendKeys(code);
            }
            catch
            {
                //log.Error("Ошибка ввода кода подтверждения.");
                log.Error("Аккаунт подтверждён автоматически.");
            }

            // ввод основных данных аккаунта
            #region **** name, password, login ****
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'next_button')]")));
            driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'next_button')]").Click();
            Thread.Sleep(1000);
            log.Info("Вводим фамилию и имя.");
            // вводим фамилию и имя
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.EditText[contains(@resource-id, 'full_name')]")));
            driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'full_name')]").SendKeys(fullname);
            insta.Name = fullname;
            log.Info("Задаём пароль.");
            // задаём пароль
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.EditText[contains(@resource-id, 'password')]")));
            driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'password')]").SendKeys(insta.Password);
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'next_button')]")));
            driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'next_button')]").Click();
            log.Info("Запоминаем автоматически созданный логин.");
            // запоминаем автоматически созданный логин
            insta.Login = driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'username')]").Text;
            log.Info(string.Format("Логин: {0}", insta.Login));
            // завершаем регистрацию
            log.Info("Завершаем регистрацию.");
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'continue_without_ci')]")));
            driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'continue_without_ci')]").Click();
            #endregion

            // пропускаем добавление контактов и загрузку фотографии
            #region **** other clicks ****
            try
            {
                log.Info("Пропускаем добавление контактов и загрузку фотографии.");
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'skip_button')]")));
                driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'skip_button')]").Click();
            }
            catch
            {
                log.Info("Кнопка 'пропустить' не найдена.");
            }

            try
            {
                log.Info("Пропускаем добавление контактов и загрузку фотографии 2.");
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'button_negative')]")));
                driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'button_negative')]").Click();
            }
            catch { }

            try
            {
                log.Info("Пропускаем добавление контактов и загрузку фотографии 3.");
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'skip_button')]")));
                driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'skip_button')]").Click();
            }
            catch { }
            #endregion
            #endregion

            return insta;
        }

        // функция заполнения аккаунта
        [STAThread]
        public static bool Fill(AndroidDriver<IWebElement> driver, WebDriverWait wait, InstagramData insta, InstaFB.Settings settings)
        {
            bool result = false;

            #region **** inputs ****
            log.Info(string.Format("Выбрали аккаунт: {0}.", insta.Login));

            // загрузка пароля учётной записи и идентификатора телефона из БД по заданному логину
            InstagramData insta_fromdb = InstaAccountsBase.getInstagram(insta.Login);
            insta.ID = insta_fromdb.ID;
            insta.Password = insta_fromdb.Password;
            insta.Android_id = insta_fromdb.Android_id;
            log.Info(string.Format("Получили пароль аккаунта: {0}.", insta.Password));
            log.Info(string.Format("Загрузили идентификатор телефона: {0}.", insta.Android_id));
            if (insta.Password == "")
            {
                log.Error("Пароль аккаунта пустой.");
                log.Info("**** **** **** ****");
                return false;
            }
            if (insta.Android_id == "")
            {
                log.Error("AndroidID для обновления пустой.");
                log.Info("**** **** **** ****");
                return false;
            }

            // выборка и удаление случайного электронного адреса из файла
            // в случае, если он ещё не задан
            if (insta_fromdb.Email == "")
            {
                insta.Email = getRandomEmail(settings.pathEmails);
                if (insta.Email != "")
                    removeEmailFromFile(settings.pathEmails, insta.Email);
                log.Info(string.Format("Выбрали электронный адрес: {0}.", insta.Email));
            }
            else
            {
                insta.Email = "";
            }

            // выборка случайного адреса сайта из файла
            insta.WebSite = getRandomWebSite(settings.pathWebSites);
            if (insta.WebSite != "")
                removeWebSiteFromFile(settings.pathWebSites, insta.WebSite);
            log.Info(string.Format("Выбрали адрес сайта: {0}.", insta.WebSite));

            // загрузка списка файлов для аватарки
            foreach (string f in Directory.GetFiles(settings.pathAvatars, "*.*", SearchOption.AllDirectories).Where(i => i.EndsWith(".jpg") || i.EndsWith(".jpeg") || i.EndsWith(".png")))
                listOfAvatars.Add(f);
            // выбираем одну случайную аватарку из списка
            Random r = new Random();
            insta.AvatarFileName = listOfAvatars[r.Next(listOfAvatars.Count - 1)];
            log.Info(string.Format("Выбрали случайную аватарку: {0}.", insta.AvatarFileName));

            // загружаем из файла описание для профиля
            getInstaInfo(settings.pathInstaInfo, insta);
            log.Info(string.Format("Получили имя профиля: {0}.", insta.Name));
            log.Info(string.Format("Получили описание для профиля: {0}.", insta.Description));
            // если "имя" пустое, то загружаем его из другого файла
            if (insta.Name == "")
            {
                log.Info(string.Format("Получаем имя профиля из файла: {0}.", settings.pathNames));
                insta.Name = getRandomName(settings.pathNames);
                if (insta.Name != "")
                {
                    log.Info(string.Format("Получили имя профиля: {0}.", insta.Name));
                    removeNameFromFile(settings.pathNames, insta.Name);
                    log.Info("Удалили имя из файла.");
                }
            }

            // формирование имени файла для загрузки в телефон
            string filename = Functions.generateFileName(6, 12);
            log.Info(string.Format("Сгенерировали имя файла для загрузки в телефон: {0}.", filename));
            #endregion

            // **** загрузка выбранной аватарки на телефон **** //
            #region **** avatar upload ****
            log.Info(string.Format("Запуск задачи: {0}", "загрузка выбранной аватарки на телефон"));
            log.Info(string.Format("Загрузка файла: {0}", insta.AvatarFileName));
            fileAvatar = new FileInfo(insta.AvatarFileName);
            try
            {
                driver.PushFile(pathTargetAvatars + filename, fileAvatar);
                log.Info(string.Format("Загрузка файла {0} успешно завершена.", filename));
            }
            catch (WebDriverException e)
            {
                log.Error(string.Format("Ошибка загрузки файла: {0}", insta.AvatarFileName));
                log.Error(string.Format("{0}"), e.ToString());
            }

            log.Info("Пауза 5 секунд.");
            Thread.Sleep(5000);

            // удаляем файл
            log.Info(string.Format("Удаляем файл: {0}.", insta.AvatarFileName));
            fileAvatar.Delete();

            // **** запуск OSGUtility для обновления галереи **** //
            log.Info(string.Format("Запуск задачи: {0}", "запуск OSGUtility для обновления галереи"));
            driver.StartActivity("ru.osg.projects.android.osgutility", ".MainActivity");
            #endregion

            #region **** instagram profile fill ****
            driver.StartActivity("com.instagram.android", ".activity.MainTabActivity");
            wait.Timeout = new TimeSpan(0, 0, 20); // таймаут ожидания элемента

            // 1) проверяем, появилось ли окно подтверждения аккаунта перед редактированием профиля
            try
            {
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.view.View[contains(@content-desc, 'Подтвердите свой аккаунт')]")));
                log.Error("Появилось окно подтверждения аккаунта.");
                log.Info("Запускаем функцию подтверждения аккаунта.");
                if (InstaLogin.AccountVerify(driver, wait, insta))
                {
                    log.Info("Аккаунт подтверждён.");
                }
                else
                {
                    log.Error("Ошибка подтверждения аккаунта.");
                }
            }
            catch
            {
                log.Info("Окно подтверждения аккаунта не появилось.");
            }

            // заходим в редактирование профиля
            log.Info("Редактирование профиля.");
            // нажимаем кнопку профиля
            wait.Timeout = new TimeSpan(0, 0, 30); // таймаут ожидания элемента
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.FrameLayout[contains(@content-desc, 'Профиль')]")));
            driver.FindElementByXPath("//android.widget.FrameLayout[contains(@content-desc, 'Профиль')]").Click();
            // нажимаем кнопку "редактировать профиль"
            log.Info("Кликнули первый раз по кнопке 'редактировать профиль'");
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'row_profile_header_edit_profile')]")));
            driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'row_profile_header_edit_profile')]").Click();

            // выбор фотографии
            try
            {
                log.Info("Выбор фотографии.");
                log.Info("Кликнули первый раз по кнопке изменения аватара.");
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'change_avatar_button')]")));
                driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'change_avatar_button')]").Click();
            }
            catch
            {
                // если не появилось окно редактирования, то пробуем нажать кнопку "редактировать профиль" ещё раз
                log.Error("Не появилась кнопка изменения аватара.");
                log.Info("Кликнули второй раз по кнопке 'редактировать профиль'");
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'row_profile_header_edit_profile')]")));
                driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'row_profile_header_edit_profile')]").Click();
                log.Info("Выбор фотографии.");
                log.Info("Кликнули первый раз по кнопке изменения аватара.");
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'change_avatar_button')]")));
                driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'change_avatar_button')]").Click();
            }

            bool avatarchangeclicked = true;
            try
            {
                // кликаем 'новое фото'
                log.Info("Пробуем кликнуть 'новое фото'");
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Новое фото профиля')]")));
                driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Новое фото профиля')]").Click();
            }
            catch
            {
                log.Error("Не нашли кнопку выбора нового фото. Кликнули второй раз по кнопке изменения аватара.");
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'change_avatar_button')]")));
                driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'change_avatar_button')]").Click();
                avatarchangeclicked = false;
            }
            if (!avatarchangeclicked)
                try
                {
                    // пробуем второй раз кликнуть 'новое фото'
                    log.Info("Пробуем второй раз кликнуть 'новое фото'");
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Новое фото профиля')]")));
                    driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Новое фото профиля')]").Click();
                    avatarchangeclicked = true;
                }
                catch
                {
                    log.Error("Не нашли кнопку изменения аватара. Кликаем по этой же кнопке, но с поиском по тексту 'другое фото'");
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Другое фото')]")));
                    driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Другое фото')]").Click();
                    avatarchangeclicked = false;
                }
            if (!avatarchangeclicked)
            {
                try
                {
                    // пробуем третий раз кликнуть 'новое фото'
                    log.Info("Пробуем третий раз кликнуть 'новое фото'");
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Новое фото профиля')]")));
                    driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Новое фото профиля')]").Click();
                    avatarchangeclicked = true;
                }
                catch
                {
                    avatarchangeclicked = false;
                }
            }
            if (avatarchangeclicked)
            {
                // далее
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'next_button_textview')]")));
                driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'next_button_textview')]").Click();
                // далее
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'next_button_textview')]")));
                driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'next_button_textview')]").Click();
                Thread.Sleep(1000);
                log.Info("Фотография выбрана.");
                // после выбора фотографии снова нажимаем кнопку "редактировать профиль"
                wait.Timeout = new TimeSpan(0, 2, 0);
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'row_profile_header_edit_profile')]")));
                driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'row_profile_header_edit_profile')]").Click();
            }
            else
            {
                log.Error("Фотография не выбрана.");
            }

            wait.Timeout = new TimeSpan(0, 0, 30);
            log.Info("Заполняем поле веб-сайта.");
            if (insta.WebSite != "")
            {
                // заполняем поле веб-сайта
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.EditText[contains(@resource-id, 'website')]")));
                driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'website')]").Clear();
                driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'website')]").SendKeys(insta.WebSite);
            }
            else
            {
                log.Debug(string.Format("Входная строка для сайта пустая. Проверьте файл {0} с входной строкой.", settings.pathWebSites));
            }

            // заполняем поле имени профиля
            log.Info("Заполняем поле имени профиля.");
            if (insta.Name != "")
            {
                string parsed_name = new StringAnalize(insta.Name).getParsedString();
                log.Info(string.Format("Преобразованная строка для имени: {0}", parsed_name));
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.EditText[contains(@resource-id, 'full_name')]")));
                driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'full_name')]").Clear();
                driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'full_name')]").SendKeys(parsed_name);
                insta.Name = parsed_name;
            }
            else
            {
                log.Debug(string.Format("Входная строка для имени пустая. Проверьте файл {0} с входной строкой.", settings.pathInstaInfo));
            }

            // заполняем поле описания профиля
            log.Info("Заполняем поле описания профиля.");
            if (insta.Description != "")
            {
                // преобразуем входную строку
                string parsed_description = new StringAnalize(insta.Description).getParsedString();
                log.Info(string.Format("Преобразованная строка для описания: {0}", parsed_description));
                // заполняем поле описания
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.EditText[contains(@resource-id, 'bio')]")));
                driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'bio')]").Clear();
                driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'bio')]").SendKeys(parsed_description);
                insta.Description = parsed_description;
            }
            else
            {
                log.Debug(string.Format("Входная строка для описания пустая. Проверьте файл {0} с входной строкой.", settings.pathInstaInfo));
            }

            if (insta.Email != "")
            {
                // заполняем поле электронного адреса
                log.Info("Заполняем поле электронного адреса.");
                driver.Swipe(60, 800, 60, 400, 300);
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'email')]")));
                driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'email')]").Click();
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.EditText[contains(@resource-id, 'current_email')]")));
                driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'current_email')]").Clear();
                driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'current_email')]").SendKeys(insta.Email);
                driver.FindElementByXPath("//android.widget.ViewSwitcher[contains(@resource-id, 'action_bar_button_action')]").Click();

                // если появилось окно с ошибкой, что электронная почта уже используется
                bool emailisused = false;
                try
                {
                    wait.Timeout = new TimeSpan(0, 0, 10); // таймаут ожидания элемента = 10 секунд
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Эл. адрес используется')]")));
                    emailisused = driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Эл. адрес используется')]").Displayed;
                    // получаем текст сообщения об ошибке
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.CheckBox[contains(@resource-id, 'message')]")));
                    string text = driver.FindElementByXPath("//android.widget.CheckBox[contains(@resource-id, 'message')]").Text;
                    log.Error(text);
                    log.Error("E-mail занят.");
                }
                catch
                {
                    emailisused = false;
                    log.Info("E-mail свободен.");
                }
                // если первое окно с ошибкой не появилось, но появилось окно с сообщением о необходимости подтверждения электронной почты
                if (!emailisused)
                    try
                    {
                        wait.Timeout = new TimeSpan(0, 0, 10); // таймаут ожидания элемента = 10 секунд
                        wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Подтвердите свой эл. адрес')]")));
                        emailisused = driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Подтвердите свой эл. адрес')]").Displayed;
                        // получаем текст сообщения об ошибке
                        wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.CheckBox[contains(@resource-id, 'message')]")));
                        string text = driver.FindElementByXPath("//android.widget.CheckBox[contains(@resource-id, 'message')]").Text;
                        log.Error(text);
                        log.Error("E-mail занят.");
                    }
                    catch
                    {
                        emailisused = false;
                        log.Info("E-mail свободен.");
                    }
                if (emailisused) // если одно из двух окон появилось
                {
                    // тогда e-mail не будем записывать в БД
                    insta.Email = "";
                    // нажимаем ок
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'button_positive')]")));
                    driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'button_positive')]").Click();
                    // отменяем ввод электронной почты
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.ImageView[contains(@resource-id, 'action_bar_button_back')]")));
                    driver.FindElementByXPath("//android.widget.ImageView[contains(@resource-id, 'action_bar_button_back')]").Click();
                    log.Info("Поле электронной почты оставили пустым.");
                }
                else
                {
                    // закрываем окно с информацией о подтверждении электронной почты
                    Thread.Sleep(1000);
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'button_positive')]")));
                    driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'button_positive')]").Click();
                    log.Info("Заполнили поле электронной почты.");
                }
            }
            else
            {
                log.Debug(string.Format("Входной файл {0} с электронными ящиками пустой.", settings.pathEmails));
            }

            // сохраняем изменения
            log.Info("Сохраняем изменения.");
            Thread.Sleep(1000);
            wait.Timeout = new TimeSpan(0, 0, 20); // таймаут ожидания элемента
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.ViewSwitcher[contains(@resource-id, 'action_bar_button_action')]")));
            driver.FindElementByXPath("//android.widget.ViewSwitcher[contains(@resource-id, 'action_bar_button_action')]").Click();

            // 2) проверяем, появилось ли окно подтверждения аккаунта после заполнения
            try
            {
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.view.View[contains(@content-desc, 'Подтвердите свой аккаунт')]")));
                log.Error("Появилось окно подтверждения аккаунта.");
                log.Info("Запускаем функцию подтверждения аккаунта.");
                if (InstaLogin.AccountVerify(driver, wait, insta))
                {
                    log.Info("Аккаунт подтверждён.");
                }
                else
                {
                    log.Error("Ошибка подтверждения аккаунта.");
                }
            }
            catch
            {
                log.Info("Окно подтверждения аккаунта не появилось.");
            }

            // сохраняем изменения в БД
            if (InstaAccountsBase.updateInstagram(insta.ID, insta.Email, insta.WebSite, insta.Name, insta.Description))
                log.Info("Успешная запись данных в БД.");
            else
                log.Error("Ошибка записи в БД.");

            log.Info(string.Format("{0} Профиль изменён", DateTime.Now));

            result = true;
            #endregion

            return result;
        }

        // функция публикации постов
        [STAThread]
        public static bool Post2(AndroidDriver<IWebElement> driver, WebDriverWait wait, InstagramData insta, InstaFB.Settings settings)
        {
            bool result = false;
            string status = "";

            log.Info(string.Format("Выбрали аккаунт: {0}", insta.Login));

            #region **** inputs ****
            // загрузка пароля учётной записи и идентификатора телефона из БД по заданному логину
            InstagramData insta_fromdb = InstaAccountsBase.getInstagram(insta.Login);
            insta.ID = insta_fromdb.ID;
            insta.Password = insta_fromdb.Password;
            insta.Android_id = insta_fromdb.Android_id;
            log.Info(string.Format("Получили пароль аккаунта: {0}", insta.Password));
            log.Info(string.Format("Загрузили идентификатор телефона: {0}", insta.Android_id));
            if (insta.Password == "")
            {
                log.Error("Пароль аккаунта пустой.");
                log.Info("**** **** **** ****");
                return false;
            }
            if (insta.Android_id == "")
            {
                log.Error("AndroidID для обновления пустой.");
                log.Info("**** **** **** ****");
                return false;
            }

            // загрузка из файла шаблона текста для постов
            log.Info("Загрузка шаблона текста для постов.");
            getPostText(settings.pathPostText);
            log.Info(string.Format("Загруженный шаблон: {0}", postText));
            #endregion

            // **** загрузка на телефон указанного количества изображений **** //
            #region **** pictures upload ****
            // загружаем список папок из каталога и выбираем случайную папку
            log.Info(string.Format("Загружаем список папок из каталога: {0}.", settings.pathPictures));
            foreach (string d in Directory.GetDirectories(settings.pathPictures, "*", SearchOption.TopDirectoryOnly))
                listOfDirectories.Add(d);

            if (listOfDirectories.Count <= 0)
            {
                log.Error("В указанном каталоге нет папок.");
                return false;
            }

            log.Info("Выбираем случайную папку.");
            Random rnd_dir = new Random();
            int dir = rnd_dir.Next(0, listOfDirectories.Count);

            // загрузка списка изображений из верхнего уровня выбранной папки
            log.Info(string.Format("Загружаем список файлов из папки: {0}.", listOfDirectories[dir]));
            foreach (string f in Directory.GetFiles(listOfDirectories[dir], "*.*", SearchOption.TopDirectoryOnly).Where(i => i.EndsWith(".jpg") || i.EndsWith(".jpeg") || i.EndsWith(".png")))
                listOfPictures.Add(f);

            if (listOfPictures.Count <= 0)
            {
                log.Error("В указанной папке нет файлов.");
                return false;
            }

            log.Info(string.Format("Количество файлов: {0}", listOfPictures.Count));

            // загрузка изображений в папку телефона
            log.Info(string.Format("Запуск задачи: загрузка {0} изображений в папку телефона: {1}.", pictures_count, pathTargetKross));

            for (int i = 0; i < Math.Min(listOfPictures.Count, pictures_count); i++)
            {
                log.Info(string.Format("Загрузка файла: {0} {1}.", (i + 1), listOfPictures[i]));
                // создание объекта файла для загрузки
                try
                {
                    log.Info("Создание объекта файла для загрузки.");
                    fileKross = new FileInfo(listOfPictures[i]);
                }
                catch
                {
                    log.Error("Ошибка создания объекта файла для загрузки.");
                    break;
                }
                // имя файла для загрузки в телефон
                string filename = Functions.generateFileName(6, 12);
                log.Info(string.Format("Сгенерировали имя файла для загрузки в телефон: {0}", filename));
                try
                {
                    driver.PushFile(pathTargetKross + filename, fileKross);
                    status = string.Format("Загрузка файла {0} {1} успешно завершена.", i, filename);
                    log.Info(status);
                }
                catch (WebDriverException e)
                {
                    status = string.Format("Ошибка загрузки файла: {0}.", listOfPictures[i]);
                    log.Error(status);
                    log.Error(e.ToString());
                }

                // **** запуск OSGUtility для обновления галереи **** //
                log.Info(string.Format("Запуск задачи: {0}", "запуск OSGUtility для обновления галереи."));
                driver.StartActivity("ru.osg.projects.android.osgutility", ".MainActivity");

                Thread.Sleep(500);
            }

            // удаляем выбранную папку со всем файлами
            log.Info(string.Format("Удаляем папку: {0}.", listOfDirectories[dir]));
            dirKross = new DirectoryInfo(listOfDirectories[dir]);
            dirKross.Delete(true);
            #endregion

            // **** разместить посты с загруженными изображениями **** //
            #region **** posting ****
            driver.StartActivity("com.instagram.android", ".activity.MainTabActivity");
            wait.Timeout = new TimeSpan(0, 0, 20); // таймаут ожидания элемента

            int k = 0;

            for (int i = 0; i < Math.Min(listOfPictures.Count, pictures_count); i++)
            {
                wait.Timeout = new TimeSpan(0, 0, 30); // таймаут ожидания элемента

                // нажимаем кнопку выбора фотографии
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.FrameLayout[contains(@content-desc, 'Камера')]")));
                driver.FindElementByXPath("//android.widget.FrameLayout[contains(@content-desc, 'Камера')]").Click();
                log.Info(string.Format("{0} Нажимаем кнопку выбора фотографии.", i + 1));

                // убеждаемся, что точно нажали выбора фотографии
                int countoftrying = 10;
                int nexttry = 0;
                bool isopengallery = false;
                while (!isopengallery)
                {
                    try
                    {
                        wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.support.v7.widget.RecyclerView[contains(@resource-id, 'media_picker_grid_view')]")));
                        log.Info("Открылась форма выбора фотографии из галереи или форма создания нового фото.");
                        isopengallery = true;
                    }
                    catch
                    {
                        log.Error("Открылась другая форма. Пытаемся снова открыть форму выбора фотографии.");
                        isopengallery = false;
                        nexttry++;
                        // нажимаем кнопку выбора фотографии
                        wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.FrameLayout[contains(@content-desc, 'Камера')]")));
                        driver.FindElementByXPath("//android.widget.FrameLayout[contains(@content-desc, 'Камера')]").Click();
                        log.Info("Нажимаем кнопку выбора фотографии.");
                    }
                    // если превысили количество попыток
                    if (nexttry == countoftrying)
                    {
                        log.Error(string.Format("Превысили количество попыток открытия формы выбора фотографии: {0}.", countoftrying));
                        break;
                    }
                }

                // если не открылось окно после всех попыток, то переходим к следующему шагу
                if (!isopengallery)
                    continue;

                // проверяем, что открыта вкладка 'галерея'
                try
                {
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Галерея')]")));
                    log.Info("Открылась вкладка 'галерея'.");
                }
                catch
                {
                    log.Error("Открылась другая вкладка. Пытаемся открыть вкладку 'галерея'.");
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.view.View[contains(@text, 'Галерея')]")));
                    driver.FindElementByXPath("//android.view.View[contains(@text, 'Галерея')]").Click();
                }

                // выбираем очередную фотографию
                // учитываем, что предыдущая загруженная фотография появляется в галерее
                Thread.Sleep(1000);
                // если загружаем пятую фотографию или больше, то необходимо пролистать фотографии
                if (i >= 4)
                {
                    log.Info("Необходимо пролистать фотографии.");
                    driver.Swipe(50, 650, 50, 150, 2000);
                }
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.ImageView[contains(@resource-id, 'crop_image_view')]")));
                driver.FindElementByXPath(string.Format("//android.view.View[contains(@NAF, 'true') and contains(@index, '{0}')]", i + k)).Click();
                log.Info("Выбираем очередную фотографию.");
                // нажимаем далее
                Thread.Sleep(1000);
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'next_button_textview')]")));
                driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'next_button_textview')]").Click();
                log.Info("Нажимаем далее.");
                // нажимаем далее
                Thread.Sleep(1000);
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'next_button_textview')]")));
                driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'next_button_textview')]").Click();
                log.Info("Нажимаем далее.");
                Thread.Sleep(1000);

                // **** сформировать текст для поста **** //
                log.Info("Сформировать текст для поста.");
                if (postText != "")
                {
                    StringAnalize stringAnalize = new StringAnalize(postText);
                    parsed_posttext = stringAnalize.getParsedString();
                    log.Info(string.Format("Сформировали текст для поста: {0}.", parsed_posttext));
                }
                else
                {
                    parsed_posttext = "";
                    log.Info("Текст для поста пустой.");
                }

                // заполняем поле "добавьте подпись"
                if (parsed_posttext != "")
                {
                    log.Info("Ждём 2 секунды перед заполнением поля подписи.");
                    Thread.Sleep(2000);
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.EditText[contains(@resource-id, 'caption_text_view')]")));
                    driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'caption_text_view')]").Click();
                    driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'caption_text_view')]").SendKeys(parsed_posttext);
                    log.Info(string.Format("Заполняем поле подписи: {0}.", parsed_posttext));
                }
                else
                {
                    log.Info("Поле подписи не заполняли, так как входящая строка пустая.");
                }

                // нажимаем 'поделиться'
                Thread.Sleep(1000);
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'next_button_textview')]")));
                driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'next_button_textview')]").Click();
                log.Info("Нажимаем 'поделиться'.");

                // **** записать в БД результат **** //
                if (InstaAccountsBase.insertPost(insta.ID, postText, parsed_posttext))
                {
                    status = "Успешная запись размещённых постов в БД.";
                    log.Info(status);
                }
                else
                {
                    status = "Ошибка записи размещённых постов в БД.";
                    log.Error(status);
                }

                k++;
            }
            #endregion

            result = true;

            return result;
        }

        // функция публикации постов
        [STAThread]
        public static bool Post(AndroidDriver<IWebElement> driver, WebDriverWait wait, InstagramData insta, InstaFB.Settings settings)
        {
            bool result = false;

            log.Info(string.Format("Выбрали аккаунт: {0}", insta.Login));

            #region **** inputs ****
            // загрузка пароля учётной записи и идентификатора телефона из БД по заданному логину
            InstagramData insta_fromdb = InstaAccountsBase.getInstagram(insta.Login);
            insta.ID = insta_fromdb.ID;
            insta.Password = insta_fromdb.Password;
            insta.Android_id = insta_fromdb.Android_id;
            log.Info(string.Format("Получили пароль аккаунта: {0}", insta.Password));
            log.Info(string.Format("Загрузили идентификатор телефона: {0}", insta.Android_id));
            if (insta.Password == "")
            {
                log.Error("Пароль аккаунта пустой.");
                log.Info("**** **** **** ****");
                return false;
            }
            if (insta.Android_id == "")
            {
                log.Error("AndroidID для обновления пустой.");
                log.Info("**** **** **** ****");
                return false;
            }

            // загрузка из файла шаблона текста для постов
            log.Info("Загрузка шаблона текста для постов.");
            getPostText(settings.pathPostText);
            log.Info(string.Format("Загруженный шаблон: {0}", postText));
            #endregion

            // **** загрузка на телефон указанного количества изображений **** //
            #region **** pictures upload ****
            // загрузка списка изображений
            log.Info(string.Format("Загружаем список файлов из папки: {0}.", settings.pathPictures));
            foreach (string f in Directory.GetFiles(settings.pathPictures, "*.*", SearchOption.AllDirectories).Where(i => i.EndsWith(".jpg") || i.EndsWith(".jpeg") || i.EndsWith(".png")))
                listOfPictures.Add(f);
            log.Info(string.Format("Количество файлов: {0}", listOfPictures.Count));
            // выбираем из списка указанное количество случайных изображений
            List<string> fileKrossNames = new List<string>(pictures_count);
            Random r = new Random();
            for (int i = 0; i < Math.Min(listOfPictures.Count, pictures_count); i++)
            {
                int rndFile = r.Next(0, listOfPictures.Count - 1); // взяли случайный файл
                fileKrossName = listOfPictures[rndFile];
                fileKrossNames.Add(fileKrossName); // запомнили его в список для загрузки
                log.Info(string.Format("Выбрали {0} изображение: {1}.", i, fileKrossName));
                listOfPictures.RemoveAt(rndFile); // удалили его из первого списка
            }

            log.Info(string.Format("Запуск задачи: загрузка на телефон {0} изображений в папку телефона: {1}.", pictures_count, pathTargetKross));

            for (int i = 0; i < Math.Min(fileKrossNames.Count, pictures_count); i++)
            {
                log.Info(string.Format("Загрузка файла: {0} {1}.", i, fileKrossNames[i]));
                fileKross = new FileInfo(fileKrossNames[i]);
                // формирование имени файла для загрузки в телефон
                string filename = Functions.generateFileName(6, 12);
                log.Info(string.Format("Сгенерировали имя файла для загрузки в телефон: {0}.", filename));
                try
                {
                    driver.PushFile(pathTargetKross + filename, fileKross);
                    log.Info(string.Format("Загрузка файла {0} {1} успешно завершена.", i, filename));
                }
                catch (WebDriverException e)
                {
                    log.Error(string.Format("Ошибка загрузки файла: {0}.", fileKrossNames[i]));
                    log.Error(string.Format("{0}"), e.ToString());
                }

                // удаляем файл
                log.Info("Пауза 2 секунды.");
                Thread.Sleep(2000);
                log.Info(string.Format("Удаляем файл: {0}.", fileKrossNames[i]));
                fileKross.Delete();
            }
            #endregion

            // **** запуск OSGUtility для обновления галереи **** //
            log.Info(string.Format("Запуск задачи: {0}", "запуск OSGUtility для обновления галереи."));
            driver.StartActivity("ru.osg.projects.android.osgutility", ".MainActivity");

            // **** разместить посты с загруженными изображениями **** //
            #region **** posting ****
            driver.StartActivity("com.instagram.android", ".activity.MainTabActivity");
            wait.Timeout = new TimeSpan(0, 0, 20); // таймаут ожидания элемента

            // 3) проверяем, появилось ли окно подтверждения аккаунта перед загрузкой постов
            try
            {
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.view.View[contains(@content-desc, 'Подтвердите свой аккаунт')]")));
                log.Error("Появилось окно подтверждения аккаунта.");
                log.Info("Запускаем функцию подтверждения аккаунта.");
                if (InstaLogin.AccountVerify(driver, wait, insta))
                {
                    log.Info("Аккаунт подтверждён.");
                }
                else
                {
                    log.Error("Ошибка подтверждения аккаунта.");
                }
            }
            catch
            {
                log.Info("Окно подтверждения аккаунта не появилось.");
            }

            int k = 0;

            for (int i = 0; i < Math.Min(listOfPictures.Count, pictures_count); i++)
            {
                wait.Timeout = new TimeSpan(0, 0, 30); // таймаут ожидания элемента

                // нажимаем кнопку выбора фотографии
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.FrameLayout[contains(@content-desc, 'Камера')]")));
                driver.FindElementByXPath("//android.widget.FrameLayout[contains(@content-desc, 'Камера')]").Click();
                log.Info(string.Format("{0} Нажимаем кнопку выбора фотографии.", i + 1));

                // убеждаемся, что точно нажали выбора фотографии
                int countoftrying = 10;
                int nexttry = 0;
                bool isopengallery = false;
                while (!isopengallery)
                {
                    try
                    {
                        wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.support.v7.widget.RecyclerView[contains(@resource-id, 'media_picker_grid_view')]")));
                        log.Info("Открылась форма выбора фотографии из галереи или форма создания нового фото.");
                        isopengallery = true;
                    }
                    catch
                    {
                        log.Error("Открылась другая форма. Пытаемся снова открыть форму выбора фотографии.");
                        isopengallery = false;
                        nexttry++;
                        // нажимаем кнопку выбора фотографии
                        wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.FrameLayout[contains(@content-desc, 'Камера')]")));
                        driver.FindElementByXPath("//android.widget.FrameLayout[contains(@content-desc, 'Камера')]").Click();
                        log.Info("Нажимаем кнопку выбора фотографии.");
                    }
                    // если превысили количество попыток
                    if (nexttry == countoftrying)
                    {
                        log.Error(string.Format("Превысили количество попыток открытия формы выбора фотографии: {0}.", countoftrying));
                        break;
                    }
                }

                // если не открылось окно после всех попыток, то переходим к следующему шагу
                if (!isopengallery)
                    continue;

                // проверяем, что открыта вкладка 'галерея'
                try
                {
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Галерея')]")));
                    log.Info("Открылась вкладка 'галерея'.");
                }
                catch
                {
                    log.Error("Открылась другая вкладка. Пытаемся открыть вкладку 'галерея'.");
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.view.View[contains(@text, 'Галерея')]")));
                    driver.FindElementByXPath("//android.view.View[contains(@text, 'Галерея')]").Click();
                }

                // выбираем очередную фотографию
                // учитываем, что предыдущая загруженная фотография появляется в галерее
                Thread.Sleep(1000);
                // если загружаем пятую фотографию или больше, то необходимо пролистать фотографии
                if (i >= 4)
                {
                    log.Info("Необходимо пролистать фотографии.");
                    driver.Swipe(50, 650, 50, 150, 2000);
                }
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.ImageView[contains(@resource-id, 'crop_image_view')]")));
                driver.FindElementByXPath(string.Format("//android.view.View[contains(@NAF, 'true') and contains(@index, '{0}')]", i + k)).Click();
                log.Info("Выбираем очередную фотографию.");
                // нажимаем далее
                Thread.Sleep(1000);
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'next_button_textview')]")));
                driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'next_button_textview')]").Click();
                log.Info("Нажимаем далее.");
                // нажимаем далее
                Thread.Sleep(1000);
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'next_button_textview')]")));
                driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'next_button_textview')]").Click();
                log.Info("Нажимаем далее.");
                Thread.Sleep(1000);

                // **** сформировать текст для поста **** //
                log.Info("Сформировать текст для поста.");
                StringAnalize stringAnalize = new StringAnalize(postText);
                parsed_posttext = stringAnalize.getParsedString();
                log.Info(string.Format("Сформировали текст для поста: {0}.", parsed_posttext));

                // заполняем поле "добавьте подпись"
                log.Info("Ждём 2 секунды перед заполнением поля подписи.");
                Thread.Sleep(2000);
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.EditText[contains(@resource-id, 'caption_text_view')]")));
                driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'caption_text_view')]").Click();
                driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'caption_text_view')]").SendKeys(parsed_posttext);
                log.Info(string.Format("Заполняем поле подписи: {0}.", parsed_posttext));

                // нажимаем 'поделиться'
                Thread.Sleep(1000);
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'next_button_textview')]")));
                driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'next_button_textview')]").Click();
                log.Info("Нажимаем 'поделиться'.");

                if ((i + 1) % 3 == 0)
                {
                    wait.Timeout = new TimeSpan(0, 0, 20); // таймаут ожидания элемента
                    // 4) проверяем, появилось ли окно подтверждения аккаунта после размещения каждого третьего поста
                    try
                    {
                        wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.view.View[contains(@content-desc, 'Подтвердите свой аккаунт')]")));
                        log.Error("Появилось окно подтверждения аккаунта.");
                        log.Info("Запускаем функцию подтверждения аккаунта.");
                        if (InstaLogin.AccountVerify(driver, wait, insta))
                        {
                            log.Info("Аккаунт подтверждён.");
                        }
                        else
                        {
                            log.Error("Ошибка подтверждения аккаунта.");
                        }
                    }
                    catch
                    {
                        log.Info("Окно подтверждения аккаунта не появилось.");
                    }
                }

                // **** записать в БД результат **** //
                if (InstaAccountsBase.insertPost(insta.ID, postText, parsed_posttext))
                {
                    log.Info("Успешная запись размещённых постов в БД.");
                }
                else
                {
                    log.Error("Ошибка записи размещённых постов в БД.");
                }

                k++;
            }
            #endregion

            result = true;

            return result;
        }

        // функция привязки аккаунта к аккаунту Facebook
        [STAThread]
        public static bool Bind(AndroidDriver<IWebElement> driver, WebDriverWait wait, FBData fb)
        {
            bool result = true;

            driver.StartActivity("com.instagram.android", ".activity.MainTabActivity");

            // открываем страницу профиля
            log.Info("Открываем страницу профиля.");
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.FrameLayout[contains(@content-desc, 'Профиль')]")));
            driver.FindElementByXPath("//android.widget.FrameLayout[contains(@content-desc, 'Профиль')]").Click();

            // открываем параметры
            log.Info("Открываем параметры.");
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.ImageView[contains(@content-desc, 'Параметры')]")));
            driver.FindElementByXPath("//android.widget.ImageView[contains(@content-desc, 'Параметры')]").Click();

            // листаем
            try
            {
                log.Info("Листаем вниз.");
                driver.Swipe(100, 700, 100, 150, 2000);
                driver.Swipe(100, 700, 100, 150, 2000);
            }
            catch
            {
                try
                {
                    log.Error("Ошибка листания. Листаем снова.");
                    driver.Swipe(100, 700, 100, 150, 2000);
                    driver.Swipe(100, 700, 100, 150, 2000);
                }
                catch
                {
                    log.Error("Ошибка листания.");
                }
            }

            // нажимаем "связанные аккаунты"
            log.Info("Нажимаем 'связанные аккаунты'.");
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Связанные аккаунты')]")));
            driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Связанные аккаунты')]").Click();

            log.Info("Увеличиваем время ожидания до 1 минуты.");
            wait.Timeout = new TimeSpan(0, 1, 0);

            // нажимаем "Facebook"
            log.Info("Выбираем 'Facebook'.");
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Facebook')]")));
            driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Facebook')]").Click();
            Thread.Sleep(5000);

            // нажимаем "далее"
            log.Info("Нажимаем 'далее'.");
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Button[contains(@content-desc, 'Далее')]")));
            driver.FindElementByXPath("//android.widget.Button[contains(@content-desc, 'Далее')]").Click();
            Thread.Sleep(5000);

            // нажимаем "ок"
            log.Info("Нажимаем 'ок'.");
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Button[contains(@content-desc, 'OK')]")));
            driver.FindElementByXPath("//android.widget.Button[contains(@content-desc, 'OK')]").Click();
            Thread.Sleep(5000);

            log.Info("Уменьшаем время ожидания до 30 секунд.");
            wait.Timeout = new TimeSpan(0, 0, 30);

            return result;
        }

        // преобразование строки количества в число
        private static double getCountFromString(string value)
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

        // функция подписки на указанный аккаунт
        [STAThread]
        public static string InstaFollowTo(AndroidDriver<IWebElement> driver, WebDriverWait wait, int instagram, string followto)
        {
            log.Info(string.Format("Подписка на аккаунт {0}", followto));
            driver.StartActivity("com.instagram.android", ".activity.MainTabActivity");

            wait.Timeout = new TimeSpan(0, 0, 30); // таймаут ожидания элемента = 30 секунд

            FollowingData following = new FollowingData();
            following.Login = followto;
            bool isfollowingavailable = false;

            string result = "";
            string button_text = "";

            log.Info(string.Format("Начинаем поиск: {0}.", followto));
            log.Info("Ищем кнопку 'Поиск и интересное'.");
            // нажимаем поиск
            #region **** search button ****
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.FrameLayout[contains(@content-desc, 'Поиск и интересное')]")));
            driver.FindElementByXPath("//android.widget.FrameLayout[contains(@content-desc, 'Поиск и интересное')]").Click();
            // проверяем, действительно ли нажали поиск
            try
            {
                log.Info("Ищем кнопку 'Найти'.");
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.EditText[contains(@text, 'Найти')]")));
                button_text = driver.FindElementByXPath("//android.widget.EditText[contains(@text, 'Найти')]").Text;
            }
            catch
            {
                try
                {
                    // если случайно попали в добавление фото
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Фото')]")));
                    button_text = driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Фото')]").Text;
                    // то закрываем это окно
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.ImageView[contains(@resource-id, 'action_bar_cancel')]")));
                    button_text = driver.FindElementByXPath("//android.widget.ImageView[contains(@resource-id, 'action_bar_cancel')]").Text;
                    // и пробуем ещё раз нажать поиск
                    log.Error("Не найдено поле для поиска. Пробуем еще раз нажать кнопку поиска.");
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.FrameLayout[contains(@content-desc, 'Поиск и интересное')]")));
                    driver.FindElementByXPath("//android.widget.FrameLayout[contains(@content-desc, 'Поиск и интересное')]").Click();
                    log.Info("Ищем кнопку 'Найти' второй раз.");
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.EditText[contains(@text, 'Найти')]")));
                    button_text = driver.FindElementByXPath("//android.widget.EditText[contains(@text, 'Найти')]").Text;
                }
                catch
                {
                    result = "Кнопка 'Поиск и интересное' не найдена.";
                    log.Error(result);
                    return result;
                }
            }
            #endregion

            // вводим в поле поиска указанный аккаунт
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.EditText[contains(@resource-id, 'action_bar_search_edit_text')]")));
            driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'action_bar_search_edit_text')]").SendKeys(followto);
            // ждём секунду
            Thread.Sleep(1000);
            int no_results = 0;
            // если получили сообщение, что результат не найден
            try
            {
                no_results = driver.FindElementsByXPath("//android.widget.TextView[contains(@resource-id, 'row_no_results_textview')]").Count;
            }
            catch { }
            if (no_results == 1)
            {
                result = "Результат не найден.";
                log.Info(result);
                return result;
            }
            else
            {
                try
                {
                    log.Info("Ждём появление элемента после поиска.");
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'row_search_user_username')]")));
                    // проверяем, найден ли элемент
                    try
                    {
                        driver.FindElementsByClassName("android.widget.TextView").First(f => f.Text == followto).Click();
                        log.Info(string.Format("Жмём на найденный элемент: {0}.", followto));
                    }
                    catch (Exception e)
                    {
                        result = string.Format("Результат не найден. Есть похожие, например {0}.", driver.FindElementsByClassName("android.widget.TextView").First().Text);
                        log.Error(result);
                        log.Error(e.Message);
                        return result;
                    }

                    // ждём появления кнопки 'подписаться'
                    #region **** follow button ****
                    try
                    {
                        // получаем текст кнопки
                        wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'row_profile_header_button_follow')]")));
                        button_text = driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'row_profile_header_button_follow')]").Text;
                        log.Info(string.Format("Текст кнопки: {0}.", button_text));

                        isfollowingavailable = true;

                        // считываем информацию
                        #region **** get info ****
                        log.Info("Считываем информацию.");
                        // имя
                        log.Info("Получаем имя.");
                        try
                        {
                            string t_name = driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'row_profile_header_textview_fullname')]").Text;
                            log.Info(t_name);
                            following.Name = t_name;
                        }
                        catch
                        {
                            log.Error("Имя аккаунта пустое.");
                            following.Name = "";
                        }

                        // описание
                        log.Info("Получаем описание.");
                        try
                        {
                            string t_description = driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'row_profile_header_textview_biography')]").Text;
                            log.Info(t_description);
                            following.Description = t_description;
                        }
                        catch
                        {
                            log.Error("Описание аккаунта пустое.");
                            following.Description = "";
                        }

                        // сайт
                        log.Info("Получаем адрес сайта.");
                        try
                        {
                            string t_namewebsite = driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'row_profile_header_textview_website')]").Text;
                            log.Info(t_namewebsite);
                            following.WebSite = t_namewebsite;
                        }
                        catch
                        {
                            log.Error("Адрес сайта не указан.");
                            following.WebSite = "";
                        }

                        // количество постов
                        log.Info("Количество постов");
                        try
                        {
                            string c_posts = driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'row_profile_header_textview_photos_count')]").Text;
                            log.Info(c_posts);
                            following.Countofposts = (int)getCountFromString(c_posts);
                        }
                        catch
                        {
                            log.Error("Количество постов недоступно.");
                            following.Countofposts = -1;
                        }

                        // количество подписчиков
                        log.Info("Количество подписчиков");
                        try
                        {
                            string c_followers = driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'row_profile_header_textview_followers_count')]").Text;
                            log.Info(c_followers);
                            following.Countoffollowers = (int)getCountFromString(c_followers);
                        }
                        catch
                        {
                            log.Error("Количество подписчиков недоступно.");
                            following.Countoffollowers = -1;
                        }

                        // количество подписок
                        log.Info("Количество подписок");
                        try
                        {
                            string c_following = driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'row_profile_header_textview_following_count')]").Text;
                            log.Info(c_following);
                            following.Countoffollowing = (int)getCountFromString(c_following);
                        }
                        catch
                        {
                            log.Error("Количество подписок недоступно.");
                            following.Countoffollowing = -1;
                        }
                        #endregion

                        // жмём кнопку 'подписаться'
                        #region **** follow click ****
                        log.Info(string.Format("Жмём кнопку 'подписаться': {0}", followto));
                        driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'row_profile_header_button_follow')]").Click();

                        // если мы уже подписаны, то нажать кнопку отмены в появившемся окошке
                        if ((button_text == "Подписки") || (button_text == "Запрошено"))
                        {
                            log.Info(string.Format("Аккаунт уже подписан на: {0}. Нажимаем отмену.", followto));
                            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'button_negative')]")));
                            driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'button_negative')]").Click();
                        }
                        else
                        {
                            // получаем текст нажатой кнопки
                            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'row_profile_header_button_follow')]")));
                            button_text = driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'row_profile_header_button_follow')]").Text;
                            log.Info(string.Format("Текст кнопки после нажатия: {0}.", button_text));

                            // если её текст не изменился, то пробуем нажать ещё раз
                            if ((button_text != "Подписки") && (button_text != "Запрошено"))
                            {
                                log.Error("Кнопка не поменялась на 'Подписки' или 'Запрошено'");
                                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'row_profile_header_button_follow')]")));
                                driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'row_profile_header_button_follow')]").Click();
                                log.Info("Жмём ещё раз на неё.");
                            }
                        }

                        log.Info(string.Format("Подписались: {0}", followto));
                        #endregion

                        // лайк фотографий
                        #region **** liking ****
                        // выбираем режим отображения постов
                        wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.ImageView[contains(@resource-id, 'layout_button_group_view_switcher_button_list')]")));
                        driver.FindElementByXPath("//android.widget.ImageView[contains(@resource-id, 'layout_button_group_view_switcher_button_list')]").Click();
                        log.Info("Переключение режима отображения постов: списком.");

                        // листаем
                        Thread.Sleep(1000);
                        try
                        {
                            log.Info("Листаем.");
                            driver.Swipe(50, 600, 50, 150, 1000);
                        }
                        catch
                        {
                            log.Error("Ошибка листания.");
                        }

                        // листаем страницу и ставим лайки
                        Random rnd_swipe = new Random();
                        int countSwipe = rnd_swipe.Next(10, 20);
                        log.Info(string.Format("Количество листаний: {0}.", countSwipe));

                        for (int i = 1; i <= countSwipe; i++)
                        {
                            Random rnd = new Random();
                            bool islike = Convert.ToBoolean(rnd.Next(0, 2));
                            log.Info(string.Format("Лайк поста: {0}.", islike));

                            wait.Timeout = new TimeSpan(0, 0, 10);
                            if (islike)
                            {
                                try
                                {
                                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.ImageView[contains(@resource-id, 'row_feed_button_like')]")));
                                    driver.FindElementByXPath("//android.widget.ImageView[contains(@resource-id, 'row_feed_button_like')]").Click();
                                    log.Info(string.Format("Лайк поста."));
                                    Thread.Sleep(4000);
                                    try
                                    {
                                        wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'alertTitle') and contains(@text, 'Действие заблокировано')]")));
                                        log.Info("Появилось окно 'Действие заблокировано'.");
                                        driver.PressKeyCode(AndroidKeyCode.Back);
                                    }
                                    catch
                                    {
                                        log.Info("Окно 'Действие заблокировано' не появилось.");
                                    }
                                }
                                catch
                                {
                                    log.Error("Ошибка лайка.");
                                }
                            }

                            // листаем
                            Thread.Sleep(1000);
                            try
                            {
                                log.Info("Листаем.");
                                driver.Swipe(50, 600, 50, 150, 1000);
                            }
                            catch
                            {
                                log.Error("Ошибка листания.");
                            }
                        }
                        #endregion
                    }
                    catch
                    {
                        result = "Не найдена кнопка 'подписаться'.";
                        log.Error(result);
                        return result;
                    }
                    #endregion
                }
                catch
                {
                    isfollowingavailable = false;
                    log.Info(string.Format("Пользователь {0} не найден", followto));
                    result = "Результат не найден.";
                    log.Info(result);
                    return result;
                }
            }

            // запись подписки в БД
            #region **** following db insert ****
            if (isfollowingavailable)
            {
                result = "Подписка завершена.";
                log.Info(string.Format("Запись в БД following: {0}.", following.Login));
                // записываем в БД
                if (InstaAccountsBase.insertFollowing(instagram,
                    following.Login,
                    Regex.Replace(following.Name, Functions.osg_regex, " "),
                    Regex.Replace(following.Description, Functions.osg_regex, " "),
                    Regex.Replace(following.WebSite, Functions.osg_regex, " "),
                    0,
                    following.Countofposts,
                    following.Countoffollowers,
                    following.Countoffollowing))
                {
                    log.Info(string.Format("Записали в БД following: {0}.", following.Login));
                }
                else
                {
                    log.Error(string.Format("Ошибка записи в БД following: {0}.", following.Login));
                }
            }
            #endregion

            return result;
        }

        // функция листания страницы поиска
        [STAThread]
        public static void SearchPageView(AndroidDriver<IWebElement> driver, WebDriverWait wait, int maxCountSwipes, int maxSwipeSpeed, int minSwipeSpeed)
        {
            driver.PressKeyCode(AndroidKeyCode.Keycode_APP_SWITCH);
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Instagram')]")));
            driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Instagram')]").Click();
            Thread.Sleep(1000);

            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.FrameLayout[contains(@content-desc, 'Поиск и интересное')]")));
            driver.FindElementByXPath("//android.widget.FrameLayout[contains(@content-desc, 'Поиск и интересное')]").Click();
            log.Info("Нажали 'Поиск и интересное'.");

            Random rnd_swipe = new Random();
            int countSwipe = rnd_swipe.Next(maxCountSwipes / 2, maxCountSwipes + 1);
            log.Info(string.Format("Количество листаний: {0}.", countSwipe));

            for (int i = 1; i <= countSwipe; i++)
            {
                log.Info(string.Format("Страница: {0}.", i));

                // кликаем случайную фотографию
                wait.Timeout = new TimeSpan(0, 0, 10);
                int count_photo_rows = 5;
                log.Info(string.Format("Количество строк с фотографиями: {0}.", count_photo_rows));
                Random rnd = new Random();
                for (int j = 2; j <= count_photo_rows; j++)
                {
                    int number_photo_click = rnd.Next(0, 4);
                    if (number_photo_click > 0)
                    {
                        int photo_row = 4 * (i - 1) + j;

                        log.Info(string.Format("Кликаем фото/видео {0} {1}.", photo_row, number_photo_click));

                        try
                        {
                            string xpath2 = string.Format("//android.widget.ImageView[contains(@content-desc, 'Миниатюра') and contains(@content-desc, 'в строке {0}') and contains(@content-desc, 'столбца {1}')]",
                                photo_row, number_photo_click);
                            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath(xpath2)));
                            driver.FindElementByXPath(xpath2).Click();

                            log.Info("Удачный клик.");
                            Thread.Sleep(1000);

                            // подписались или нет
                            #region **** follow ****
                            Random rnd_like = new Random();
                            bool isfollow = Convert.ToBoolean(rnd_like.Next(0, 2));

                            wait.Timeout = new TimeSpan(0, 0, 10);
                            if (isfollow)
                            {
                                try
                                {
                                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Подписаться')]")));
                                    driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Подписаться')]").Click();
                                    log.Info("Подписка.");
                                    Thread.Sleep(4000);
                                    try
                                    {
                                        wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'alertTitle') and contains(@text, 'Действие заблокировано')]")));
                                        log.Info("Появилось окно 'Действие заблокировано'.");
                                        driver.PressKeyCode(AndroidKeyCode.Back);
                                        log.Info("Назад для его закрытия.");
                                    }
                                    catch
                                    {
                                        log.Info("Окно 'Действие заблокировано' не появилось.");
                                    }
                                }
                                catch
                                {
                                    log.Error("Ошибка подписки.");
                                }
                            }
                            #endregion

                            // листнули
                            log.Info("Листнули.");
                            driver.Swipe(100, 700, 100, 650, 2000);

                            // лайкнули или нет
                            #region **** like ****
                            bool islike = Convert.ToBoolean(rnd_like.Next(0, 2));

                            wait.Timeout = new TimeSpan(0, 0, 10);
                            if (islike)
                            {
                                try
                                {
                                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.ImageView[contains(@resource-id, 'row_feed_button_like')]")));
                                    driver.FindElementByXPath("//android.widget.ImageView[contains(@resource-id, 'row_feed_button_like')]").Click();
                                    log.Info("Лайк поста.");
                                    Thread.Sleep(4000);
                                    try
                                    {
                                        wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'alertTitle') and contains(@text, 'Действие заблокировано')]")));
                                        log.Info("Появилось окно 'Действие заблокировано'.");
                                        driver.PressKeyCode(AndroidKeyCode.Back);
                                        log.Info("Назад для его закрытия.");
                                    }
                                    catch
                                    {
                                        log.Info("Окно 'Действие заблокировано' не появилось.");
                                    }
                                }
                                catch
                                {
                                    log.Error("Ошибка лайка.");
                                }
                            }
                            #endregion

                            // назад
                            Thread.Sleep(2000);
                            log.Info("Назад.");
                            driver.PressKeyCode(AndroidKeyCode.Back);
                        }
                        catch
                        {
                            log.Info("Неудачный клик.");
                        }
                    }
                }

                // выбираем случайную скорость листания
                Random rnd_swipe_speed = new Random();
                int swipeSpeed = (rnd_swipe_speed.Next(maxSwipeSpeed, minSwipeSpeed + 1) / 100) * 100;
                log.Info(string.Format("Скорость листания: {0}.", swipeSpeed));
                // листаем
                Thread.Sleep(2000);
                driver.Swipe(100, 700, 100, 200, swipeSpeed);
            }
        }
    }
}