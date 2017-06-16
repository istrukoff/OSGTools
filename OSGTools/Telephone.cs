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

namespace OSGTools
{
    public static class Telephone
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public static string getNumber(AndroidDriver<IWebElement> driver, WebDriverWait wait, string ussd)
        {
            string result = "";

            log.Info("Вызываем набор номера.");
            // вызываем набор номера
            try
            {
                Thread.Sleep(1000);
                driver.StartActivity("com.android.dialer", ".DialtactsActivity");
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.ImageButton[contains(@resource-id, 'floating_action_button')]")));
            }
            catch (WebDriverException e)
            {
                log.Info(e.ToString());
                Thread.Sleep(1000);
                log.Info("Пробуем еще раз набор номера!");
                driver.StartActivity("com.android.dialer", ".DialtactsActivity");
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.ImageButton[contains(@resource-id, 'floating_action_button')]")));
            }
            driver.FindElementByXPath("//android.widget.ImageButton[contains(@resource-id, 'floating_action_button')]").Click();
            Thread.Sleep(15000);
            log.Info(string.Format("Набираем указанный номер ussd: {0}.", ussd));
            // набираем указанный номер ussd
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.EditText[contains(@resource-id, 'digits')]")));
            driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'digits')]").SendKeys(ussd);
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.ImageButton[contains(@resource-id, 'dialpad_floating_action_button')]")));
            driver.FindElementByXPath("//android.widget.ImageButton[contains(@resource-id, 'dialpad_floating_action_button')]").Click();
            try
            {
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Button[contains(@text, 'ОК')]")));
            }
            catch
            {
                Thread.Sleep(15000);
                driver.FindElementByXPath("//android.widget.ImageButton[contains(@resource-id, 'floating_action_button')]").Click();
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.EditText[contains(@resource-id, 'digits')]")));
                driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'digits')]").SendKeys(ussd);
                Thread.Sleep(15000);
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.ImageButton[contains(@resource-id, 'dialpad_floating_action_button')]")));
                driver.FindElementByXPath("//android.widget.ImageButton[contains(@resource-id, 'dialpad_floating_action_button')]").Click();
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Button[contains(@text, 'ОК')]")));
            }
            driver.FindElementByXPath("//android.widget.Button[contains(@text, 'ОК')]").Click();

            // ждём сообщение
            log.Info("Ждём сообщение с номером телефона 20 + 60 секунд.");
            wait.Timeout = new TimeSpan(0, 1, 0);
            Thread.Sleep(20000);
            driver.StartActivity("com.android.mms", ".ui.ConversationList");

            // обрабатываем сообщение с полученным номером телефона
            try
            {
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'subject') and contains(@text, 'Ваш номер')]")));
            }
            catch (WebDriverException e)
            {
                log.Error("Не пришло сообщение с номером телефона.");
                log.Error(e.ToString());
                return null;
            }

            string t = driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'subject') and contains(@text, 'Ваш номер')]").Text;
            log.Info("Обрабатываем сообщение с полученным номером телефона.");
            // получаем номер телефона
            result = t.Replace("Ваш номер ", "").Replace(".", "");
            log.Info(string.Format("Номер телефона: {0}.", result));

            Thread.Sleep(5000);

            log.Info("Удаляем все сообщения после получения.");
            // удаляем все сообщения после получения
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.ImageButton[contains(@content-desc, 'Ещё')]")));
            driver.FindElementByXPath("//android.widget.ImageButton[contains(@content-desc, 'Ещё')]").Click();
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Удалить все цепочки')]")));
            driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Удалить все цепочки')]").Click();
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Button[contains(@resource-id, 'button1')]")));
            driver.FindElementByXPath("//android.widget.Button[contains(@resource-id, 'button1')]").Click();

            return result;
        }
    }
}