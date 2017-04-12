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
    public static class InternetCheckStart
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        // строки для сравнения с результатом проверки доступа в интернет
        public static string yesInetConnAlert = "Есть подключение к интернету.";
        public static string noInetConnAlert = "Нет подключения к интернету.";
        private static int countTryInetConn = 5;

        public static string checkInternet(AndroidDriver<IWebElement> driver, WebDriverWait wait)
        {
            string result = "";

            log.Info(string.Format("Запуск задачи: {0}", "запуск OSGUtility для проверки доступа в интернет"));
            driver.StartActivity("ru.osg.projects.android.osgutility", ".MainActivity");
            int c = 0;

            while (true)
            {
                // если интернет не появился после (countTryInetConn-2) попыток, то перезагружаем сеть с помощью режима полёта
                if (countTryInetConn == 1 || c == Math.Max(countTryInetConn - 2, 2))
                {
                    //
                    log.Info("Включаем режим самолёта.");
                    // открываем системное меню
                    driver.Swipe(360, 5, 330, 550, 300);
                    Thread.Sleep(1000);
                    driver.Swipe(360, 5, 330, 700, 300);
                    // включаем режим самолёта
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.view.View[contains(@content-desc, 'Режим полета отключен.')]")));
                    driver.FindElementByXPath("//android.view.View[contains(@content-desc, 'Режим полета отключен.')]").Click();
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.view.View[contains(@content-desc, 'Режим полета включен.')]")));
                    // ждём 10 секунд
                    Thread.Sleep(10000);
                    //
                    log.Info("Отключаем режим самолёта.");
                    // выключаем режим самолёта
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.view.View[contains(@content-desc, 'Режим полета включен.')]")));
                    driver.FindElementByXPath("//android.view.View[contains(@content-desc, 'Режим полета включен.')]").Click();
                    // ждём 10 секунд
                    Thread.Sleep(10000);
                    driver.PressKeyCode(AndroidKeyCode.Home);
                    driver.StartActivity("ru.osg.projects.android.osgutility", ".MainActivity");
                }

                // нажимаем на кнопку проверки доступа в интернет
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Button[contains(@resource-id, 'btn_checkInet')]")));
                driver.FindElementByXPath("//android.widget.Button[contains(@resource-id, 'btn_checkInet')]").Click();

                Thread.Sleep(2000);

                // получаем сообщение о результате проверки
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@resource-id, 'message')]")));
                string inetConnAlert = driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'message')]").Text;

                c++; // для следующей попытки проверки

                if (inetConnAlert == noInetConnAlert)
                {
                    // если нет подключения к интернету, то ждём 5 секунд
                    log.Info(string.Format("{0} Попытка №{1}", noInetConnAlert, c));
                    Thread.Sleep(5000);
                }
                else
                {
                    // если есть подключение к интернету, то продолжаем дальше
                    result = yesInetConnAlert;
                    log.Info(yesInetConnAlert);
                    break;
                }

                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Button[contains(@resource-id, 'button1')]")));
                driver.FindElementByXPath("//android.widget.Button[contains(@resource-id, 'button1')]").Click();

                if (c == countTryInetConn)
                {
                    result = noInetConnAlert;
                    log.Error(noInetConnAlert);
                    return result;
                }
            }

            return result;
        }
    }
}