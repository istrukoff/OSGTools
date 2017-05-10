using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OSGTools.VK
{
    public static class VKLogin
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        // вход с указанными телефоном и паролем
        public static bool LogIn(AndroidDriver<IWebElement> driver, WebDriverWait wait, string telephone, string password)
        {
            bool result = true;

            log.Info(string.Format("Запуск VK для авторизации аккаунта {0}.", telephone));
            //driver.StartActivity("com.vkontakte.android", ".AuthActivity");

            return result;
        }

        // выход из аккаунта
        public static bool LogOut(AndroidDriver<IWebElement> driver, WebDriverWait wait)
        {
            bool result = true;

            log.Info("Запуск VK для выхода из аккаунта.");
            //driver.StartActivity("com.vkontakte.android", ".AuthActivity");
            log.Info("Ищем ВКонтакте в списке открытых приложений.");
            driver.PressKeyCode(AndroidKeyCode.Keycode_APP_SWITCH);
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'ВКонтакте') and contains(@resource-id, 'activity_description')]")));
            driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'ВКонтакте') and contains(@resource-id, 'activity_description')]").Click();
            log.Info("Открыли окно ВКонтакте");

            // нажимаем "меню"
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.ImageButton[@NAF='true']")));
            driver.FindElementByXPath("//android.widget.ImageButton[@NAF='true']").Click();
            log.Info("Открыли 'меню'.");
            Thread.Sleep(2000);

            // листаем вниз
            driver.Swipe(50, 800, 50, 400, 2000);
            log.Info("Листаем вниз.");

            // заходим в настройки
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'leftmenu_text') and @text='Настройки']")));
            driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'leftmenu_text') and @text='Настройки']").Click();
            log.Info("Выбрали 'настройки'.");
            Thread.Sleep(2000);

            // листаем вниз
            driver.Swipe(60, 800, 60, 300, 2000);
            log.Info("Листаем вниз.");

            // нажимаем "выйти"
            try
            {
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'text1') and @text='Выйти']")));
                driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'text1') and @text='Выйти']").Click();
                log.Info("Нажали 'выйти'.");
            }
            catch
            {
                log.Error("Не нашли кнопку 'выйти'.");
                log.Info("Пробуем снова.");
                driver.Swipe(60, 800, 60, 300, 2000);
                log.Info("Листаем вниз.");
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'text1') and @text='Выйти']")));
                driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'text1') and @text='Выйти']").Click();
                log.Info("Нажали 'выйти'.");
            }
            Thread.Sleep(1000);

            // нажимаем "да"
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Button[contains(@resource-id, 'button1') and @text='Да']")));
            driver.FindElementByXPath("//android.widget.Button[contains(@resource-id, 'button1') and @text='Да']").Click();
            log.Info("Подтвердили выход из аккаунта.");

            return result;
        }
    }
}