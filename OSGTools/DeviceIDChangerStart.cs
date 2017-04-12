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
    public static class DeviceIDChangerStart
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        // изменение идентификатора на указанный с указанным значением паузы после выключения режима полёта
        public static string runIDChanger(AndroidDriver<IWebElement> driver, WebDriverWait wait, string newdeviceid, bool longpause)
        {
            string result = "";

            log.Info(string.Format("Запуск задачи: запуск DeviceIDChanger на телефоне."));
            wait.Timeout = new TimeSpan(0, 0, 90);

            log.Info("Включаем режим самолёта.");
            // открываем системное меню
            driver.Swipe(360, 5, 330, 550, 300);
            Thread.Sleep(1000);
            driver.Swipe(360, 5, 330, 700, 300);
            // включаем режим самолёта
            try
            {
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.view.View[contains(@content-desc, 'Режим полета отключен.')]")));
                driver.FindElementByXPath("//android.view.View[contains(@content-desc, 'Режим полета отключен.')]").Click();
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.view.View[contains(@content-desc, 'Режим полета включен.')]")));
            }
            catch
            {
                log.Error("Ошибка при включении режима самолёта. Возможно, он уже включен.");
            }
            driver.PressKeyCode(AndroidKeyCode.Home);

            log.Info("Запускаем IDChanger.");
            // запускаем IDChanger
            try
            {
                driver.StartActivity("com.VTechno.androididchanger", ".MainActivity");
            }
            catch
            {
                result = "Ошибка запуска IDChanger. Пробуем запустить снова.";
                log.Error(result);
                try
                {
                    driver.StartActivity("com.VTechno.androididchanger", ".MainActivity");
                }
                catch
                {
                    result = "Ошибка запуска IDChanger.";
                    log.Error(result);
                    return result;
                }
            }

            log.Info(string.Format("Запуск задачи: меняем идентификатор на загруженный из БД."));
            // отправляем в поле ввода идентификатор из БД
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.EditText[contains(@resource-id, 'edit_text')]")));
            driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'edit_text')]").Clear();
            driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'edit_text')]").SendKeys(newdeviceid);

            // нажимаем кнопку "применить"
            try
            {
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.ImageView[contains(@resource-id, 'apply_from_edit')]")));
                driver.FindElementByXPath("//android.widget.ImageView[contains(@resource-id, 'apply_from_edit')]").Click();
            }
            catch
            {
                result = "Не нажалась кнопка применения нового ID. Пробуем снова. Ошибка изменения ID.";
                log.Error(result);
                try
                {
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.ImageView[contains(@resource-id, 'apply_from_edit')]")));
                    driver.FindElementByXPath("//android.widget.ImageView[contains(@resource-id, 'apply_from_edit')]").Click();
                }
                catch
                {
                    result = "Не нажалась кнопка применения нового ID. Ошибка изменения ID.";
                    log.Error(result);
                    return result;
                }
            }

            // идентификатор, полученный из окна результата
            try
            {
                string changedid = driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'content_text')]").Text.Replace("You have successfully changed your phone ID. New ID : ", "");
                // нажимаем кнопку "ок"
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Button[contains(@resource-id, 'confirm_button')]")));
                driver.FindElementByXPath("//android.widget.Button[contains(@resource-id, 'confirm_button')]").Click();
                result = string.Format("Идентификатор изменён на: {0}={1}", newdeviceid, changedid);
                log.Info(result);
            }
            catch
            {
                result = "Не появилось окно с результатом работы программы.";
                log.Info(result);
            }

            log.Info("Отключаем режим самолёта.");
            // открываем системное меню
            driver.Swipe(360, 5, 330, 550, 300);
            Thread.Sleep(1000);
            driver.Swipe(360, 5, 330, 700, 300);
            // выключаем режим самолёта
            try
            {
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.view.View[contains(@content-desc, 'Режим полета включен.')]")));
                driver.FindElementByXPath("//android.view.View[contains(@content-desc, 'Режим полета включен.')]").Click();
                log.Info("Пауза 30 секунд.");
                Thread.Sleep(30000);
            }
            catch
            {
                log.Error("Ошибка при выключении режима самолёта.");
            }

            log.Info(string.Format("Результат: {0}.", result));
            driver.PressKeyCode(AndroidKeyCode.Home);

            if (longpause)
            {
                log.Info("Пауза 30 секунд.");
                Thread.Sleep(30000);
            }
            driver.PressKeyCode(AndroidKeyCode.Home);

            return result;
        }

        // изменение идентификатора на указанный
        public static string runIDChanger(AndroidDriver<IWebElement> driver, WebDriverWait wait, string newdeviceid)
        {
            string result = "";

            log.Info(string.Format("Запуск задачи: запуск DeviceIDChanger на телефоне."));

            log.Info("Включаем режим самолёта.");
            // открываем системное меню
            driver.Swipe(360, 5, 330, 550, 300);
            Thread.Sleep(1000);
            driver.Swipe(360, 5, 330, 700, 300);
            // включаем режим самолёта
            try
            {
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.view.View[contains(@content-desc, 'Режим полета отключен.')]")));
                driver.FindElementByXPath("//android.view.View[contains(@content-desc, 'Режим полета отключен.')]").Click();
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.view.View[contains(@content-desc, 'Режим полета включен.')]")));
            }
            catch
            {
                log.Error("Ошибка при включении режима самолёта. Возможно, он уже включен.");
            }
            driver.PressKeyCode(AndroidKeyCode.Home);

            log.Info("Запускаем IDChanger.");
            // запускаем IDChanger
            try
            {
                driver.StartActivity("com.VTechno.androididchanger", ".MainActivity");
            }
            catch
            {
                result = "Ошибка запуска IDChanger. Пробуем запустить снова.";
                log.Error(result);
                try
                {
                    driver.StartActivity("com.VTechno.androididchanger", ".MainActivity");
                }
                catch
                {
                    result = "Ошибка запуска IDChanger.";
                    log.Error(result);
                    return result;
                }
            }

            log.Info(string.Format("Запуск задачи: меняем идентификатор на загруженный из БД."));
            // отправляем в поле ввода идентификатор из БД
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.EditText[contains(@resource-id, 'edit_text')]")));
            driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'edit_text')]").Clear();
            driver.FindElementByXPath("//android.widget.EditText[contains(@resource-id, 'edit_text')]").SendKeys(newdeviceid);

            // нажимаем кнопку "применить"
            try
            {
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.ImageView[contains(@resource-id, 'apply_from_edit')]")));
                driver.FindElementByXPath("//android.widget.ImageView[contains(@resource-id, 'apply_from_edit')]").Click();
            }
            catch
            {
                result = "Не нажалась кнопка применения нового ID. Пробуем снова. Ошибка изменения ID.";
                log.Error(result);
                try
                {
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.ImageView[contains(@resource-id, 'apply_from_edit')]")));
                    driver.FindElementByXPath("//android.widget.ImageView[contains(@resource-id, 'apply_from_edit')]").Click();
                }
                catch
                {
                    result = "Не нажалась кнопка применения нового ID. Ошибка изменения ID.";
                    log.Error(result);
                    return result;
                }
            }

            // идентификатор, полученный из окна результата
            try
            {
                string changedid = driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'content_text')]").Text.Replace("You have successfully changed your phone ID. New ID : ", "");
                // нажимаем кнопку "ок"
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.Button[contains(@resource-id, 'confirm_button')]")));
                driver.FindElementByXPath("//android.widget.Button[contains(@resource-id, 'confirm_button')]").Click();
                result = string.Format("Идентификатор изменён на: {0}={1}", newdeviceid, changedid);
                log.Info(result);
            }
            catch
            {
                result = "Не появилось окно с результатом работы программы.";
                log.Info(result);
            }

            log.Info("Отключаем режим самолёта.");
            // открываем системное меню
            driver.Swipe(360, 5, 330, 550, 300);
            Thread.Sleep(1000);
            driver.Swipe(360, 5, 330, 700, 300);
            // выключаем режим самолёта
            try
            {
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.view.View[contains(@content-desc, 'Режим полета включен.')]")));
                driver.FindElementByXPath("//android.view.View[contains(@content-desc, 'Режим полета включен.')]").Click();
                Thread.Sleep(15000);
            }
            catch
            {
                log.Error("Ошибка при выключении режима самолёта.");
            }

            log.Info(string.Format("Результат: {0}.", result));
            driver.PressKeyCode(AndroidKeyCode.Home);
            driver.PressKeyCode(AndroidKeyCode.Home);

            return result;
        }

        // изменение идентификатора на сгенерированный
        public static string runIDChanger(AndroidDriver<IWebElement> driver, WebDriverWait wait)
        {
            string result = "";

            log.Info(string.Format("Запуск задачи: запуск DeviceIDChanger на телефоне."));

            log.Info("Открываем системное меню.");
            // открываем системное меню
            driver.Swipe(360, 5, 330, 550, 300);
            Thread.Sleep(500);
            driver.Swipe(360, 5, 330, 700, 300);
            Thread.Sleep(500);
            log.Info("Включаем режим самолёта.");
            // включаем режим самолёта
            try
            {
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.view.View[contains(@content-desc, 'Режим полета отключен.')]")));
                driver.FindElementByXPath("//android.view.View[contains(@content-desc, 'Режим полета отключен.')]").Click();
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.view.View[contains(@content-desc, 'Режим полета включен.')]")));
            }
            catch
            {
                log.Error("Ошибка при включении режима самолёта. Возможно, он уже включен.");
            }
            driver.PressKeyCode(AndroidKeyCode.Home);

            log.Info("Запускаем IDChanger.");
            // запускаем IDChanger
            try
            {
                driver.StartActivity("com.VTechno.androididchanger", ".MainActivity");
            }
            catch
            {
                result = "Ошибка запуска IDChanger. Пробуем запустить снова.";
                log.Error(result);
                try
                {
                    driver.StartActivity("com.VTechno.androididchanger", ".MainActivity");
                }
                catch
                {
                    result = "Ошибка запуска IDChanger.";
                    log.Error(result);
                    return result;
                }
            }

            // нажимаем кнопку генерации случайного id
            log.Info(string.Format("Запуск задачи: меняем идентификатор на случайно сгенерированный."));
            log.Info("Нажимаем кнопку apply_random.");
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.ImageView[contains(@resource-id, 'apply_random')]")));
            driver.FindElementByXPath("//android.widget.ImageView[contains(@resource-id, 'apply_random')]").Click();

            // получаем значение id из появившегося окна
            string t = "";
            try
            {
                log.Info("Получаем строку, содержащую значение ID, из появившегося окна.");
                t = driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'content_text')]").Text;
            }
            catch (WebDriverException e)
            {
                log.Error(string.Format("Не найдено окно с ID: {0}.", e.ToString()));
                // ждём 1 секунду и снова пытаемся получить строку, содержащую значение id
                Thread.Sleep(1000);
                try
                {
                    log.Info("Снова пытаемся получить строку, содержащую значение id.");
                    t = driver.FindElementByXPath("//android.widget.TextView[contains(@resource-id, 'content_text')]").Text;
                }
                catch
                {
                    result = "Не удалось получить строку, содержащую значение ID, из появившегося окна.";
                    log.Error(result);
                    return result;
                }
            }

            // получаем значение id из полученной строки
            log.Info(string.Format("Полученная строка, содержащая значение ID: {0}", t));
            result = t.Replace("You have successfully changed your phone ID. New ID : ", "");
            log.Info(string.Format("Полученное значение ID: {0}", result));
            Thread.Sleep(1000);

            driver.Swipe(360, 5, 330, 550, 300);
            Thread.Sleep(500);
            driver.Swipe(360, 5, 330, 700, 300);
            log.Info("Выключаем режим самолёта.");
            // выключаем режим самолёта
            try
            {
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.view.View[contains(@content-desc, 'Режим полета включен.')]")));
                driver.FindElementByXPath("//android.view.View[contains(@content-desc, 'Режим полета включен.')]").Click();
                Thread.Sleep(15000);
            }
            catch
            {
                log.Error("Ошибка при выключении режима самолёта.");
            }

            log.Info(string.Format("Результат: {0}.", result));
            driver.PressKeyCode(AndroidKeyCode.Home);
            driver.PressKeyCode(AndroidKeyCode.Home);

            return result;
        }
    }
}