using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace OSGTools.Insta
{
    public static class InstaLogin
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        // вход в Инстраграм с указанными логиным и паролем
        public static bool LogIn(AndroidDriver<IWebElement> driver, WebDriverWait wait, string login, string password)
        {
            bool result = true;
            string status = "";

            log.Info(string.Format("Запуск Instagram для авторизации аккаунта {0}.", login));
            driver.StartActivity("com.instagram.android", ".activity.MainTabActivity");

            // проверяем на наличие доступного аккаунта для входа
            bool isaccountavailable = true;
            try
            {
                isaccountavailable = driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'one_tap_log_in_button')]").Displayed;
                log.Info("Есть доступный аккаунт. Необходимо его удалить из окна входа.");
            }
            catch
            {
                isaccountavailable = false;
                log.Info("Нет доступного аккаунта.");
            }
            if (!isaccountavailable)
                try
                {
                    isaccountavailable = driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Войти как ')]").Displayed;
                    log.Info("Есть доступный аккаунт. Необходимо его удалить из окна входа.");
                }
                catch
                {
                    isaccountavailable = false;
                    log.Info("Нет доступного аккаунта.");
                }
            // если есть доступный для входа аккаунт
            if (isaccountavailable)
            {
                // то удаляем его из окна входа, то есть нажимаем кнопку "удалить"
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'remove_button')]")));
                driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'remove_button')]").Click();
                // нажимаем кнопку подтверждения
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'button_positive')]")));
                driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'button_positive')]").Click();
                log.Info("Удалили доступный аккаунт.");
                // ****
                // проверить, открыто ли первое окно (с предложением войти через Facebook, зарегистрироваться или открыть окно входа)
                bool isnotopenloginwindow = true;
                try
                {
                    isnotopenloginwindow = driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Уже есть аккаунт? Войдите.')]").Displayed;
                    log.Info("Открыто первое окно (с предложением войти через Facebook, зарегистрироваться или открыть окно входа).");
                }
                catch
                {
                    isnotopenloginwindow = false;
                    log.Info("Открыто окно входа.");
                }
                // если открыто первое окно, то кликаем по кнопке "Уже есть аккаунт? Войдите."
                if (isnotopenloginwindow)
                {
                    log.Info("Кликаем по кнопке 'Уже есть аккаунт? Войдите.'");
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'log_in_button')]")));
                    driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'log_in_button')]").Click();
                }
                // ****
                // далее входим с указанным логином и паролем
                log.Info("Входим с указанным логином и паролем.");
                // логин
                log.Info(string.Format("Вводим логин {0}.", login));
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.EditText[contains(@resource-id, 'login_username')]")));
                driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'login_username')]").Clear();
                driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'login_username')]").SendKeys(login);
                // пароль
                log.Info(string.Format("Вводим пароль {0}.", password));
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.EditText[contains(@resource-id, 'login_password')]")));
                driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'login_password')]").SendKeys(password);
                // нажимаем вход
                log.Info("Нажимаем кнопку входа.");
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'next_button')]")));
                driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'next_button')]").Click();
                // в случае неудачного входа
                bool loginerror = false;
                try // если появилось окно "Неверное имя пользователя"
                {
                    wait.Timeout = new TimeSpan(0, 0, 10); // таймаут ожидания элемента = 10 секунд
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Неверное имя пользователя')]")));
                    // получаем текст сообщения об ошибке
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.CheckBox[contains(@resource-id, 'message')]")));
                    status = driver.FindElementByXPath("//android.widget.CheckBox[contains(@resource-id, 'message')]").Text;
                    log.Error("Ошибка входа.");
                    log.Error(status);
                    loginerror = true;
                }
                catch
                {
                    result = true;
                    loginerror = false;
                }
                if (!loginerror)
                    try // если появилось окно "Проблемы с авторизацией?"
                    {
                        wait.Timeout = new TimeSpan(0, 0, 10); // таймаут ожидания элемента = 10 секунд
                        wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Проблемы с авторизацией?')]")));
                        // получаем текст сообщения об ошибке
                        wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.CheckBox[contains(@resource-id, 'message')]")));
                        status = driver.FindElementByXPath("//android.widget.CheckBox[contains(@resource-id, 'message')]").Text;
                        log.Error("Ошибка входа.");
                        log.Error(status);
                        loginerror = true;
                    }
                    catch
                    {
                        result = true;
                        loginerror = false;
                    }
                if (loginerror)
                {
                    return false;
                }
            }
            else
            {
                // если нет доступного для входа аккаунта
                // ****
                // проверить, открыто ли первое окно (с предложением войти через Facebook, зарегистрироваться или открыть окно входа)
                bool isnotopenloginwindow = true;
                try
                {
                    isnotopenloginwindow = driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Уже есть аккаунт? Войдите.')]").Displayed;
                    log.Info("Открыто первое окно (с предложением войти через Facebook, зарегистрироваться или открыть окно входа).");
                }
                catch
                {
                    isnotopenloginwindow = false;
                    log.Info("Открыто окно входа.");
                }
                // если открыто первое окно, то кликаем по кнопке "Уже есть аккаунт? Войдите."
                if (isnotopenloginwindow)
                {
                    log.Info("Кликаем по кнопке 'Уже есть аккаунт? Войдите.'");
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'log_in_button')]")));
                    driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'log_in_button')]").Click();
                }
                // ****
                // входим с указанным логином и паролем
                log.Info("Входим с указанным логином и паролем.");
                // логин
                log.Info(string.Format("Вводим логин {0}.", login));
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.EditText[contains(@resource-id, 'login_username')]")));
                driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'login_username')]").Clear();
                driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'login_username')]").SendKeys(login);
                // пароль
                log.Info(string.Format("Вводим пароль {0}.", password));
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.EditText[contains(@resource-id, 'login_password')]")));
                driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'login_password')]").SendKeys(password);
                // нажимаем вход
                log.Info("Нажимаем кнопку входа.");
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'next_button')]")));
                driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'next_button')]").Click();
                // в случае неудачного входа
                bool loginerror = false;
                try // если появилось окно "Неверное имя пользователя"
                {
                    wait.Timeout = new TimeSpan(0, 0, 10); // таймаут ожидания элемента = 10 секунд
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Неверное имя пользователя')]")));
                    // получаем текст сообщения об ошибке
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.CheckBox[contains(@resource-id, 'message')]")));
                    status = driver.FindElementByXPath("//android.widget.CheckBox[contains(@resource-id, 'message')]").Text;
                    log.Error("Ошибка входа.");
                    log.Error(status);
                    loginerror = true;
                }
                catch
                {
                    result = true;
                    loginerror = false;
                }
                if (!loginerror)
                    try // если появилось окно "Проблемы с авторизацией?"
                    {
                        wait.Timeout = new TimeSpan(0, 0, 10); // таймаут ожидания элемента = 10 секунд
                        wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Проблемы с авторизацией?')]")));
                        // получаем текст сообщения об ошибке
                        wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.CheckBox[contains(@resource-id, 'message')]")));
                        status = driver.FindElementByXPath("//android.widget.CheckBox[contains(@resource-id, 'message')]").Text;
                        log.Error("Ошибка входа.");
                        log.Error(status);
                        loginerror = true;
                    }
                    catch
                    {
                        result = true;
                        loginerror = false;
                    }
                if (loginerror)
                {
                    return false;
                }
            }

            return result;
        }

        // выход из Инстраграма
        public static bool LogOut(AndroidDriver<IWebElement> driver, WebDriverWait wait)
        {
            bool result = true;

            driver.StartActivity("com.instagram.android", ".activity.MainTabActivity");

            // ждём загрузки 5 секунд
            Thread.Sleep(5000);
            log.Info("Ждём загрузки Instagram 5 секунд.");

            // если аккаунт залогинен, то выходим из него
            bool isnotlogined = true;
            try
            {
                isnotlogined = driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'log_in_button')]").Displayed;
                log.Info("Нет активного аккаунта.");
            }
            catch
            {
                isnotlogined = false;
                log.Info("Первая проверка на активный аккаунт.");
            }
            if (!isnotlogined)
                try
                {
                    isnotlogined = driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Вход')]").Displayed;
                    log.Info("Нет активного аккаунта.");
                }
                catch
                {
                    isnotlogined = false;
                    log.Info("Вторая проверка на активный аккаунт.");
                }
            if (!isnotlogined)
            {
                log.Info("Выходим из активного аккаунта.");
                // входим в параметры профиля
                try
                {
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.FrameLayout[contains(@content-desc, 'Профиль')]")));
                    driver.FindElementByXPath("//android.widget.FrameLayout[contains(@content-desc, 'Профиль')]").Click();
                    log.Info("Нажали кнопку профиль.");
                    Thread.Sleep(500);
                    do
                    {
                        wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.ImageView[contains(@content-desc, 'Параметры')]")));
                        driver.FindElementByXPath("//android.widget.ImageView[contains(@content-desc, 'Параметры')]").Click();
                        log.Info("Нажали кнопку параметры");
                    } while (!driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Параметры')]").Displayed);

                    log.Info("Нажали кнопку параметры");
                }
                catch
                {
                    log.Info("Не нашли кнопку Профиль или Параметры, жмем еще раз профиль и ищем параметры");
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.FrameLayout[contains(@content-desc, 'Профиль')]")));
                    driver.FindElementByXPath("//android.widget.FrameLayout[contains(@content-desc, 'Профиль')]").Click();
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.ImageView[contains(@content-desc, 'Параметры')]")));
                    driver.FindElementByXPath("//android.widget.ImageView[contains(@content-desc, 'Параметры')]").Click();
                }
                // находим кнопку выхода
                Thread.Sleep(1000);
                driver.Swipe(250, 700, 250, 250, 300);
                driver.Swipe(250, 700, 250, 250, 300);
                driver.Swipe(250, 700, 250, 250, 300);
                driver.Swipe(250, 700, 250, 250, 300);
                driver.Swipe(250, 700, 250, 250, 300);
                log.Info("Свайп до самого низа.");
                try
                {
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Выйти')]")));
                    driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Выйти')]").Click();
                    log.Info("Нажали Выйти.");
                }
                catch
                {
                    log.Info("Не нажалось Выйти. Еще разок пробуем ткнуть Выйти.");
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.FrameLayout[contains(@content-desc, 'Профиль')]")));
                    driver.FindElementByXPath("//android.widget.FrameLayout[contains(@content-desc, 'Профиль')]").Click();
                    Thread.Sleep(500);
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.ImageView[contains(@content-desc, 'Параметры')]")));
                    driver.FindElementByXPath("//android.widget.ImageView[contains(@content-desc, 'Параметры')]").Click();
                    Thread.Sleep(1000);
                    driver.Swipe(250, 700, 250, 250, 300);
                    driver.Swipe(250, 700, 250, 250, 300);
                    driver.Swipe(250, 700, 250, 250, 300);
                    driver.Swipe(250, 700, 250, 250, 300);
                    driver.Swipe(250, 700, 250, 250, 300);
                    log.Info("Свайп до самого низа.");
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Выйти')]")));
                    driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Выйти')]").Click();
                }
                try
                {
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'button_positive')]")));
                    driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'button_positive')]").Click();
                    log.Info("Нажали Да.");
                }
                catch
                {
                    log.Info("Не нажалось да. Еще раз");
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'button_positive')]")));
                    driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'button_positive')]").Click();
                }
            }

            driver.PressKeyCode(AndroidKeyCode.Home);

            return result;
        }

        public static bool AccountVerify(AndroidDriver<IWebElement> driver, WebDriverWait wait, InstagramData insta)
        {
            bool result = false;

            log.Info("Запуск подтверждения аккаунта с помощью сообщения на телефон.");
            log.Info(string.Format("Логин аккаунта: {0}", insta.Login));
            log.Info(string.Format("Номер телефона, привязанный к аккаунту: {0}", insta.Telephone));

            // смотрим показанный номер телефона
            //log.Info("Смотрим показанный номер телефона.");
            //wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.EditText[contains(@content-desc, '+7')]")));
            //string phonenumberfromverify = driver.FindElementByXPath("//android.widget.EditText[contains(@content-desc, '+7')]").Text;
            //wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.EditText[contains(@text, '+7')]")));
            //string phonenumber = driver.FindElementByXPath("//android.widget.EditText[contains(@text, '+7')]").Text;
            //log.Info(phonenumberfromverify);

            // нажимаем кнопку отправки
            log.Info("Нажимаем кнопку отправки.");
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Button[contains(@content-desc, 'Отправить')]")));
            driver.FindElementByXPath("//android.widget.Button[contains(@content-desc, 'Отправить')]").Click();

            // ждём 20 секунд
            log.Info("Ждём сообщение 20 + 60 секунд.");
            Thread.Sleep(20000);

            wait.Timeout = new TimeSpan(0, 1, 0);

            // обрабатываем сообщение
            string code = "";

            // получаем текст сообщения
            driver.StartActivity("com.android.mms", ".ui.ConversationList");
            try
            {
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'subject') and contains(@text, 'K')]")));
                //wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'subject')]")));
                log.Info("Пришло сообщение.");
            }
            catch (WebDriverException e)
            {
                log.Error("Не пришло сообщение с кодом подтверждения.");
                log.Error(e.ToString());
                return false;
            }

            //string t_code = driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'subject')]").Text;
            string t_code = driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'subject') and contains(@text, 'K')]").Text;
            log.Info(string.Format("Получили сообщение: {0}.", t_code));
            // берём шесть цифр из сообщения
            code = Regex.Match(t_code, @"\d\d\d \d\d\d").Value;
            code = code.Replace(" ", "");
            log.Info(string.Format("Получили код подтверждения из сообщения: {0}.", code));

            // удаляем все сообщения после получения
            wait.Timeout = new TimeSpan(0, 0, 10);
            log.Info("Удаляем все сообщения после получения.");
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.ImageButton[contains(@content-desc, 'Ещё')]")));
            driver.FindElementByXPath("//android.widget.ImageButton[contains(@content-desc, 'Ещё')]").Click();
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Удалить все цепочки')]")));
            driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Удалить все цепочки')]").Click();
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Button[contains(@resource-id, 'button1')]")));
            driver.FindElementByXPath("//android.widget.Button[contains(@resource-id, 'button1')]").Click();

            // возвращаемся в Инстаграм
            //driver.StartActivity("com.instagram.android", ".activity.MainTabActivity");
            log.Info("Возвращаемся в Instagram.");
            driver.PressKeyCode(AndroidKeyCode.Keycode_APP_SWITCH);
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Instagram')]")));
            driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Instagram')]").Click();
            Thread.Sleep(1000);

            wait.Timeout = new TimeSpan(0, 0, 10);
            if (code != "")
            {
                // вводим полученный код
                // возможно появление поля ввода без resource-id
                try
                {
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.EditText[contains(@index, '0')]")));
                    driver.FindElementByXPath("//android.widget.EditText[contains(@index, '0')]").Click();
                    log.Info("Вводим полученный код.");
                    driver.FindElementByXPath("//android.widget.EditText[contains(@index, '0')]").SendKeys(code);
                }
                catch
                {
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.EditText[contains(@resource-id, 'id_security_code')]")));
                    driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'id_security_code')]").Click();
                    log.Info("Вводим полученный код.");
                    driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'id_security_code')]").SendKeys(code);
                }

                // нажимаем кнопку подтверждения аккаунта
                // возможно появление двух разных кнопок подтверждения
                // 1: с кнопкой 'Подтвердить аккаунт'
                log.Info("Нажимаем кнопку подтверждения аккаунта.");
                try
                {
                    //wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Button[contains(@resource-id, 'verify_account')]")));
                    //driver.FindElementByXPath("//android.widget.Button[contains(@resource-id, 'verify_account')]").Click();
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Button[contains(@content-desc, 'Подтвердить аккаунт')]")));
                    driver.FindElementByXPath("//android.widget.Button[contains(@content-desc, 'Подтвердить аккаунт')]").Click();
                    log.Info("Окно с кнопкой 'Подтвердить аккаунт'.");
                }
                catch
                {
                    log.Info("Нет окна с кнопкой 'Подтвердить аккаунт'.");
                }
                // 2: с кнопкой 'Отправить'
                try
                {
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Button[contains(@content-desc, 'Отправить')]")));
                    driver.FindElementByXPath("//android.widget.Button[contains(@content-desc, 'Отправить')]").Click();
                    log.Info("Окно с кнопкой 'Отправить'.");
                }
                catch
                {
                    log.Info("Нет окна с кнопкой 'Отправить'.");
                }
                // после подтверждения возможно появление окна с ошибкой сети
                wait.Timeout = new TimeSpan(0, 0, 5);
                try
                {
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.CheckBox[contains(@text, 'Неизвестная ошибка сети.')]")));
                    log.Info("Появилось окно с ошибкой сети.");
                    //wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Отклонить')]")));
                    driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Отклонить')]").Click();
                    log.Info("Закрыли окно с ошибкой сети.");
                }
                catch
                {
                    log.Info("Окно с ошибкой сети не появилось.");
                }

                wait.Timeout = new TimeSpan(0, 0, 5);
                try
                {
                    // нажимаем кнопку входа
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Вход')]")));
                    driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Вход')]").Click();
                    log.Info("Нажали кнопку входа.");
                }
                catch
                {
                    log.Error("Ошибка нажатия кнопки входа.");
                }
            }
            else
            {
                return false;
            }

            result = true;

            return result;
        }

        // проверка, появилось ли окно подтверждения через электронную почту
        public static bool isEmailWindowOpen(AndroidDriver<IWebElement> driver, WebDriverWait wait)
        {
            bool result = false;

            wait.Timeout = new TimeSpan(0, 0, 5);
            try
            {
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'alertTitle') and contains(@text, 'Забыли пароль для аккаунта')]")));
                log.Info("Появилось окно 'Забыли пароль для аккаунта'.");
                result = true;
                try
                {
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.CheckBox[contains(@resource-id, 'message')]")));
                    string message = driver.FindElementByXPath("//android.widget.CheckBox[contains(@resource-id, 'message')]").Text;
                    log.Info(string.Format("{0}", message));
                }
                catch
                {
                    log.Error("Не удалось получить текст сообщения.");
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }
    }
}