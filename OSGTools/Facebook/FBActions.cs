using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace OSGTools.FB
{
    public static class FBActions
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public static FBData Registration(AndroidDriver<IWebElement> driver, WebDriverWait wait, InstaFB.Settings settings)
        {
            // объект для создания аккаунта
            FBData result = new FBData();

            // имя, фамилия, пол
            result.FirstName = settings.FirstName;
            result.LastName = settings.LastName;
            result.Sex = settings.sex;
            log.Info(string.Format("Имя и фамилия: {0} {1}. Пол: {2}.", result.FirstName, result.LastName, result.Sex));

            // номер телефона
            result.Telephone = settings.telephone;
            log.Info(string.Format("Используемый номер телефона: {0}.", result.Telephone));

            // идентификатор телефона
            result.Android_id = settings.android_id;
            log.Info(string.Format("Используемый идентификатор телефона: {0}.", result.Android_id));

            log.Info("Открываем Facebook через ярлык на начальном экране.");
            driver.PressKeyCode(AndroidKeyCode.Home);
            driver.PressKeyCode(AndroidKeyCode.Home);
            try
            {
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Facebook')]")));
                driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Facebook')]").Click();
                log.Info("Открыли окно Facebook.");
            }
            catch
            {
                log.Error("Ошибка запуска Facebook.");
                return null;
            }

            // кликаем кнопку создания аккаунта
            // @text='Создать аккаунт Facebook'
            try
            {
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Button[contains(@resource-id, 'login_signup_button')]")));
                driver.FindElementByXPath("//android.widget.Button[contains(@resource-id, 'login_signup_button')]").Click();
                log.Info("Кликнули кнопку создания аккаунта.");
            }
            catch
            {
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Button[contains(@resource-id, 'login_create_account_button')]")));
                driver.FindElementByXPath("//android.widget.Button[contains(@resource-id, 'login_create_account_button')]").Click();
                log.Info("Кликнули кнопку создания аккаунта.");
            }

            // далее
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Button[contains(@resource-id, 'finish_button')]")));
            driver.FindElementByXPath("//android.widget.Button[contains(@resource-id, 'finish_button')]").Click();
            log.Info("Кликнули 'далее'.");

            // вводим имя
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.EditText[contains(@resource-id, 'first_name_input')]")));
            driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'first_name_input')]").Click();
            driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'first_name_input')]").SendKeys(result.FirstName);
            log.Info(string.Format("Ввели имя: {0}.", result.FirstName));
            // вводим фамилию
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.EditText[contains(@resource-id, 'last_name_input')]")));
            driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'last_name_input')]").Click();
            driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'last_name_input')]").SendKeys(result.LastName);
            log.Info(string.Format("Ввели фамилию: {0}.", result.LastName));
            // далее
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Button[contains(@resource-id, 'finish_button')]")));
            driver.FindElementByXPath("//android.widget.Button[contains(@resource-id, 'finish_button')]").Click();
            log.Info("Кликнули 'далее'.");

            // вводим номер телефона
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.EditText[contains(@resource-id, 'phone_input')]")));
            driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'phone_input')]").Click();
            driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'phone_input')]").SendKeys(result.Telephone);
            log.Info(string.Format("Ввели номер телефона: {0}.", result.Telephone));
            // далее
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Button[contains(@resource-id, 'finish_button')]")));
            driver.FindElementByXPath("//android.widget.Button[contains(@resource-id, 'finish_button')]").Click();
            log.Info("Кликнули 'далее'.");

            // выбираем дату рождения
            log.Info("Выбираем дату рождения.");
            Random rnd_day = new Random();
            Random rnd_month = new Random();
            int day_index = rnd_day.Next(1, 10);
            int month_index = rnd_month.Next(1, 10);

            // день
            log.Info(string.Format("Листаем дни {0} раз.", day_index));
            for (int i = 0; i < day_index; i++)
                driver.Swipe(115, 550, 115, 350, 1000);

            // месяц
            log.Info(string.Format("Листаем месяцы {0} раз.", month_index));
            for (int i = 0; i < month_index; i++)
                driver.Swipe(215, 550, 215, 350, 1000);

            // смотрим, какой год указан по умолчанию
            IReadOnlyCollection<IWebElement> collection_birthday_before = driver.FindElementsByXPath(string.Format("//android.widget.EditText[contains(@resource-id, 'numberpicker_input')]"));
            int collection_birthday_count_before = collection_birthday_before.Count;
            int default_year = int.Parse(collection_birthday_before.ElementAt(2).Text);
            log.Info(string.Format("Указанный год по умолчанию: {0}.", default_year));

            // год
            Random rnd_birthyear = new Random();
            int year = settings.birthday + rnd_birthyear.Next(-3, 3);
            result.BirthDay = year.ToString();
            log.Info(string.Format("Выбираем год рождения: {0}.", result.BirthDay));
            // листаем до выбранного года
            int year_index = default_year - year + 1;
            log.Info(string.Format("Листаем годы {0} раз.", year_index));
            for (int i = 1; i <= year_index; i++)
            {
                driver.Swipe(330, 400, 330, 500, 1000);
                //log.Info(string.Format("Кликаем кнопку с годом {0}.", default_year - i));
                //wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath(string.Format("//android.widget.Button[contains(@text, '{0}')]", (default_year - i).ToString()))));
                //driver.FindElementByXPath(string.Format("//android.widget.Button[contains(@text, '{0}')]", (default_year - i).ToString())).Click();
            }

            // формируем дату рождения, которую выбрали
            IReadOnlyCollection<IWebElement> collection_birthday = driver.FindElementsByXPath(string.Format("//android.widget.EditText[contains(@resource-id, 'numberpicker_input')]"));
            int collection_birthday_count = collection_birthday.Count;
            string selected_month = collection_birthday.ElementAt(1).Text;
            int month = 1;
            switch (selected_month)
            {
                case "янв.": { month = 1; break; }
                case "февр.": { month = 2; break; }
                case "марта": { month = 3; break; }
                case "апр.": { month = 4; break; }
                case "мая": { month = 5; break; }
                case "июня": { month = 6; break; }
                case "июля": { month = 7; break; }
                case "авг.": { month = 8; break; }
                case "сент.": { month = 9; break; }
                case "окт.": { month = 10; break; }
                case "нояб.": { month = 11; break; }
                case "дек.": { month = 12; break; }
            }
            result.BirthDay = string.Format("{0}-{1}-{2}",
                collection_birthday.ElementAt(2).Text,
                month,
                collection_birthday.ElementAt(0).Text);
            log.Info(string.Format("Сформированная дата рождения: {0}.", result.BirthDay));

            // далее
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Button[contains(@resource-id, 'finish_button')]")));
            driver.FindElementByXPath("//android.widget.Button[contains(@resource-id, 'finish_button')]").Click();
            log.Info("Кликнули 'далее'.");

            // указываем пол
            string selected_sex = "";
            switch (settings.sex)
            {
                case 0: { selected_sex = "female"; break; }
                case 1: { selected_sex = "male"; break; }
            }
            log.Info(string.Format("Пол: {0}.", selected_sex));
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath(string.Format("//android.widget.RadioButton[contains(@resource-id, 'gender_{0}')]", selected_sex))));
            driver.FindElementByXPath(string.Format("//android.widget.RadioButton[contains(@resource-id, 'gender_{0}')]", selected_sex)).Click();
            log.Info("Выбрали пол.");
            // далее
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Button[contains(@resource-id, 'finish_button')]")));
            driver.FindElementByXPath("//android.widget.Button[contains(@resource-id, 'finish_button')]").Click();
            log.Info("Кликнули 'далее'.");

            // генерируем пароль
            result.Password = Functions.generatePassword(6, 12);
            log.Info(string.Format("Сгенерировали пароль: {0}.", result.Password));
            // вводим пароль
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.EditText[contains(@resource-id, 'password_input')]")));
            driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'password_input')]").Click();
            driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'password_input')]").SendKeys(result.Password);
            log.Info("Задали пароль.");
            // далее
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Button[contains(@resource-id, 'finish_button')]")));
            driver.FindElementByXPath("//android.widget.Button[contains(@resource-id, 'finish_button')]").Click();
            log.Info("Кликнули 'далее'.");

            // пропускаем добавление почты
            try
            {
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'secondary_button')]")));
                driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'secondary_button')]").Click();
                log.Info("Пропустили добавление почты.");
            }
            catch
            {
                log.Error("Окно добавления почты не появилось.");
            }

            // нажимаем "зарегистрироваться"
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Button[contains(@resource-id, 'finish_button')]")));
            driver.FindElementByXPath("//android.widget.Button[contains(@resource-id, 'finish_button')]").Click();
            log.Info("Кликнули 'зарегистрироваться'.");

            // ожидание завершения регистрации
            log.Info("Ожидание 30 секунд завершения регистрации и входа в аккаунт.");
            Thread.Sleep(30000);

            log.Info("Увеличиваем время ожидания до 1 минуты.");
            wait.Timeout = new TimeSpan(0, 1, 0);

            // в окне сохранения пароля нажимаем "не сейчас"
            try
            {
                log.Info("Отказ от сохранения пароля.");
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Button[contains(@resource-id, 'button2')]")));
                driver.FindElementByXPath("//android.widget.Button[contains(@resource-id, 'button2')]").Click();
                Thread.Sleep(1000);
            }
            catch
            {
                log.Error("Окно отказа от сохранения пароля не появилось. Ждём снова");
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Button[contains(@resource-id, 'button2')]")));
                driver.FindElementByXPath("//android.widget.Button[contains(@resource-id, 'button2')]").Click();
                Thread.Sleep(1000);
            }

            log.Info("Уменьшаем время ожидания до 30 секунд.");
            wait.Timeout = new TimeSpan(0, 0, 30);

            // пропускаем добавление фото
            try
            {
                log.Info("Пропускаем добавление фото.");
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'primary_named_button')]")));
                driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'primary_named_button')]").Click();
                Thread.Sleep(1000);
            }
            catch
            {
                log.Error("Окно добавления фото не появилось.");
            }

            // нажимаем кнопку "готово"
            try
            {
                log.Info("Кликаем 'готово'.");
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'primary_named_button')]")));
                driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'primary_named_button')]").Click();
                Thread.Sleep(1000);
            }
            catch
            {
                log.Error("Окно с кнопкой 'готово' не появилось.");
            }

            // пропускаем добавление друзей
            try
            {
                log.Info("Пропускаем добавление друзей.");
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'primary_named_button')]")));
                driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'primary_named_button')]").Click();
                Thread.Sleep(1000);
            }
            catch
            {
                log.Error("Окно добавления друзей не появилось.");
            }

            // получаем код подтверждения
            log.Info("Получаем код подтверждения из сообщений. Ждём 60 секунд.");
            wait.Timeout = new TimeSpan(0, 1, 0);
            Thread.Sleep(2000);
            // получаем код подтверждения из сообщений
            driver.StartActivity("com.android.mms", ".ui.ConversationList");
            try
            {
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'subject') and contains(@text, 'Facebook')]")));
            }
            catch
            {
                log.Error("Не пришло сообщение с кодом подтверждения.");
                return null;
            }
            string message = driver.FindElementByXPath("//android.widget.TextView [contains(@resource-id, 'subject') and contains(@text, 'Facebook')]").Text;
            string code = Regex.Match(message, @"\d\d\d\d\d").Value;
            log.Info(string.Format("Получили код подтверждения из сообщений: {0}.", code));

            // удаляем все сообщения после получения
            log.Info("Удаляем все сообщения после получения.");
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.ImageButton[contains(@content-desc, 'Ещё')]")));
            driver.FindElementByXPath("//android.widget.ImageButton[contains(@content-desc, 'Ещё')]").Click();
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Удалить все цепочки')]")));
            driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Удалить все цепочки')]").Click();
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Button[contains(@resource-id, 'button1')]")));
            driver.FindElementByXPath("//android.widget.Button[contains(@resource-id, 'button1')]").Click();

            // возвращаемся в Facebook
            wait.Timeout = new TimeSpan(0, 0, 30);
            log.Info("Возвращаемся в Facebook.");
            driver.PressKeyCode(AndroidKeyCode.Keycode_APP_SWITCH);
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Facebook')]")));
            driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Facebook')]").Click();

            // вводим код подтверждения
            try
            {
                log.Info(string.Format("Вводим код подтверждения: {0}.", code));
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.EditText[contains(@resource-id, 'code_input')]")));
                driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'code_input')]").Click();
                driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'code_input')]").SendKeys(code);

                // нажимаем "подтвердить"
                log.Info("Кликаем 'подтвердить'.");
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Button[contains(@resource-id, 'continue_button')]")));
                driver.FindElementByXPath("//android.widget.Button[contains(@resource-id, 'continue_button')]").Click();
                Thread.Sleep(5000);
            }
            catch
            {
                log.Error("Аккаунт подтверждён автоматически.");
                Thread.Sleep(1000);
            }

            // отказываемся от определения местоположения
            log.Info("Отказываемся от определения местоположения.");
            try
            {
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Button[contains(@resource-id, 'button2')]")));
                driver.FindElementByXPath("//android.widget.Button[contains(@resource-id, 'button2')]").Click();
            }
            catch
            {
                log.Error("Окно определения местоположения не появилось.");
            }

            Thread.Sleep(1000);

            return result;
        }

        public static string getUserID(AndroidDriver<IWebElement> driver, WebDriverWait wait)
        {
            string result = "";

            log.Info("Возвращаемся в Facebook.");
            driver.PressKeyCode(AndroidKeyCode.Keycode_APP_SWITCH);
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Facebook')]")));
            driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Facebook')]").Click();
            Thread.Sleep(1000);

            log.Info("Заходим в меню.");
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.view.View[contains(@resource-id, 'bookmarks_tab')]")));
            driver.FindElementByXPath("//android.view.View[contains(@resource-id, 'bookmarks_tab')]").Click();
            Thread.Sleep(2000);

            log.Info("Кликаем по имени профиля.");
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//com.facebook.fbui.widget.contentview.ContentView[contains(@content-desc, 'Ваш профиль')]")));
            driver.FindElementByXPath("//com.facebook.fbui.widget.contentview.ContentView[contains(@content-desc, 'Ваш профиль')]").Click();

            log.Info("Закрываем окно с предложением заполнить профиль.");
            try
            {
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Button[contains(@resource-id, 'pnux_modal_thanks_button')]")));
                driver.FindElementByXPath("//android.widget.Button[contains(@resource-id, 'pnux_modal_thanks_button')]").Click();
            }
            catch
            {
                log.Error("Не появилось окно с предложением заполнить профиль.");
            }

            log.Info("Закрываем подсказку.");
            try
            {
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'fbui_tooltip_title')]")));
                driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'fbui_tooltip_title')]").Click();
            }
            catch
            {
                log.Error("Не появилось окно с подсказкой.");
            }

            log.Info("Кликаем на кнопку 'дополнительно'.");
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.ImageButton[contains(@content-desc, 'Смотреть еще действия')]")));
            driver.FindElementByXPath("//android.widget.ImageButton[contains(@content-desc, 'Смотреть еще действия')]").Click();

            log.Info("Кликаем на кнопку копирования ссылки профиля.");
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'fbui_popover_list_item_title') and @text='Скопировать ссылку на профиль']")));
            driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'fbui_popover_list_item_title') and @text='Скопировать ссылку на профиль']").Click();

            log.Info("Ссылка на профиль скопирована в буфер обмена.");

            log.Info("Нажимаем 'назад'.");
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.ImageView[contains(@resource-id, 'fb_logo_up_button')]")));
            driver.FindElementByXPath("//android.widget.ImageView[contains(@resource-id, 'fb_logo_up_button')]").Click();

            log.Info("Запуск Clipper.");
            driver.PressKeyCode(AndroidKeyCode.Home);
            driver.PressKeyCode(AndroidKeyCode.Home);
            //driver.StartActivity("fi.rojekti.clipper", ".library.ui.MainActivity");
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Clipper+')]")));
            driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Clipper+')]").Click();

            log.Info("Получаем текст из буфера обмена.");
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'contents') and @index='0']")));
            result = driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'contents') and @index='0']").Text;

            log.Info(string.Format("Текст в буфере обмена: {0}.", result));
            result = result.Split('=')[1];

            driver.PressKeyCode(AndroidKeyCode.Home);

            return result;
        }
    }
}