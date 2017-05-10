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

namespace OSGTools.FB
{
    public static class FBLogin
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        // выход из аккаунта
        public static bool LogOut(AndroidDriver<IWebElement> driver, WebDriverWait wait)
        {
            bool result = true;

            log.Info("Возвращаемся в Facebook.");
            driver.PressKeyCode(AndroidKeyCode.Keycode_APP_SWITCH);
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Facebook')]")));
            driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Facebook')]").Click();
            Thread.Sleep(1000);

            log.Info("Заходим в меню.");
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.view.View[contains(@resource-id, 'bookmarks_tab')]")));
            driver.FindElementByXPath("//android.view.View[contains(@resource-id, 'bookmarks_tab')]").Click();
            Thread.Sleep(2000);

            log.Info("Листаем вниз.");
            driver.Swipe(100, 800, 100, 200, 2000);
            driver.Swipe(100, 800, 100, 200, 2000);
            driver.Swipe(100, 800, 100, 200, 2000);
            driver.Swipe(100, 800, 100, 200, 2000);
            driver.Swipe(100, 800, 100, 200, 2000);
            driver.Swipe(100, 800, 100, 200, 2000);

            log.Info("Нажимаем кнопку выхода.");
            try
            {
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//com.facebook.fbui.widget.contentview.ContentView[contains(@content-desc, 'Выход')]")));
                driver.FindElementByXPath("//com.facebook.fbui.widget.contentview.ContentView[contains(@content-desc, 'Выход')]").Click();
                result = true;
            }
            catch
            {
                result = false;
            }
            Thread.Sleep(2000);

            return result;
        }
    }
}