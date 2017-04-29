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
    public static class ProxyDroidStart
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public static bool runProxyDroid(AndroidDriver<IWebElement> driver, WebDriverWait wait, string port)
        {
            // запускаем ProxyDroid
            driver.PressKeyCode(AndroidKeyCode.Home);
            log.Info("Запускаем ProxyDroid.");
            try
            {
                driver.StartActivity("org.proxydroid", ".ProxyDroid");
                log.Info("ProxyDroid запущен.");
            }
            catch
            {
                log.Error("Ошибка запуска ProxyDroid.");
                try
                {
                    log.Info("Вторая попытка запуска ProxyDroid.");
                    driver.StartActivity("org.proxydroid", ".ProxyDroid");
                    log.Info("ProxyDroid запущен.");
                }
                catch
                {
                    log.Error("Ошибка запуска ProxyDroid.");
                    return false;
                }
            }

            // выключаем
            try
            {
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Switch[contains(@resource-id, 'switchWidget') and @text='I']")));
                driver.FindElementByXPath("//android.widget.Switch[contains(@resource-id, 'switchWidget') and @text='I']").Click();
                log.Info("Пауза 3 секунды.");
                Thread.Sleep(3000);
            }
            catch
            {
                log.Error("Ошибка выключения ProxyDroid. Возможно, сервис уже выключен.");
            }

            // указываем порт
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.LinearLayout[@index='6']")));
            driver.FindElementByXPath("//android.widget.LinearLayout[@index='6']").Click();
            log.Info("Открыли окно изменения порта.");
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.EditText[contains(@resource-id, 'edit')]")));
            driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'edit')]").Click();
            log.Info("Указываем значение.");
            driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'edit')]").Clear();
            driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'edit')]").SendKeys(port);
            log.Info(string.Format("Указали порт: {0}.", port));
            log.Info("Нажимаем 'ОК'.");
            driver.FindElementByXPath("//android.widget.Button[contains(@resource-id, 'button1')]").Click();
            Thread.Sleep(1000);

            // включаем
            try
            {
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Switch[contains(@resource-id, 'switchWidget')]")));
                driver.FindElementByXPath("//android.widget.Switch[contains(@resource-id, 'switchWidget')]").Click();
                Thread.Sleep(1000);
            }
            catch
            {
                log.Error("Ошибка включения ProxyDroid.");
                log.Info("Вторая попытка включения.");
                try
                {
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Switch[contains(@resource-id, 'switchWidget')]")));
                    driver.FindElementByXPath("//android.widget.Switch[contains(@resource-id, 'switchWidget')]").Click();
                    Thread.Sleep(1000);
                }
                catch
                {
                    log.Error("Ошибка включения ProxyDroid.");
                }
            }

            return true;
        }
    }
}