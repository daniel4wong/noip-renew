using System;
using System.Net;
using System.Threading;
using OpenQA.Selenium;

namespace noip_renew.core
{
    public static class IWebDriverExtensions
    {
        public static bool GetScreenshot(this IWebDriver driver, string filename, ScreenshotImageFormat format = ScreenshotImageFormat.Png)
        {
            try
            {
                ITakesScreenshot sd = driver as ITakesScreenshot;
                Thread.Sleep(1000);
                sd.GetScreenshot().SaveAsFile(filename, format);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static CookieContainer GetCookieContainer(this IWebDriver driver)
        {
            CookieContainer cc = new CookieContainer();
            foreach (OpenQA.Selenium.Cookie c in driver.Manage().Cookies.AllCookies)
            {
                string name = c.Name;
                string value = c.Value;
                cc.Add(new System.Net.Cookie(name, value, c.Path, c.Domain));
            }
            return cc;
        }
    }
}
