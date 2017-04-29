using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSGTools
{
    public static class AndroidGallery
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public static bool clearGallery(AndroidDriver<IWebElement> driver, WebDriverWait wait)
        {
            driver.StartActivity("com.android.gallery3d", ".app.GalleryActivity");
            log.Info("Ждём открытия галереи с надписью 'Альбомы'.");
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[contains(@text, 'Альбомы')]")));

            try
            {
                driver.FindElementByXPath("//android.widget.ImageButton[contains(@content-desc, 'Ещё')]").Click();
                log.Info("Кликнули правое меню.");
                driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Выбрать альбомы')]").Click();
                log.Info("Кликнули 'Выбрать альбомы'.");
                driver.FindElementByXPath("//android.widget.Button[contains(@resource-id, 'selection_menu')]").Click();
                log.Info("Кликнули появившееся левое меню.");
                driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Выбрать все')]").Click();
                log.Info("Выбрали всё.");
                try
                {
                    driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'action_delete')]").Click();
                    log.Info("Кликнули значок 'Удалить'.");
                }
                catch
                {
                    driver.FindElementByXPath("//android.widget.ImageButton[contains(@content-desc, 'Ещё')]").Click();
                    log.Info("Кликнули правое меню второй раз.");
                    driver.FindElementByXPath("//android.widget.TextView[contains(@text, 'Удалить')]").Click();
                    log.Info("Кликнули 'Удалить'.");
                }
                driver.FindElementByXPath("//android.widget.Button[contains(@text, 'ОК')]").Click();
                log.Info("Подтвердили удаление.");
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}