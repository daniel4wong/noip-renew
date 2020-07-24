using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium;

namespace noip_renew.noip
{
    public class NoIpHelper
    {
        public static bool Login(IWebDriver driver, string username, string password)
        {
            IWebElement e;
            e = driver.FindElement(By.Name("username"));
            e.SendKeys(username);
            e = driver.FindElement(By.Name("password"));
            e.SendKeys(password);
            e = driver.FindElement(By.Name("Login"));
            e.Click();

            Thread.Sleep(1000);
            try
            {
                if (driver.FindElement(By.Id("user-email-container")) != null)
                    return true;
            }
            catch
            {
                //username or password not correct
                //account locked for 15 minutes due to attempt incorrect password several times
                return false;
            }

            return false;
        }

        public static ICollection<IWebElement> Get_hosts(IWebDriver driver)
        {
            return driver.FindElements(By.XPath("//td[@data-title='Host']"));
        }

        public static IWebElement Get_host_link(IWebElement host)
        {
            return host.FindElement(By.XPath(".//a[@class='text-info cursor-pointer']"));
        }

        public static string Get_host_name(IWebElement host)
        {
            return Get_host_link(host).Text;
        }

        public static IWebElement Get_host_button(IWebElement host)
        {
            return host.FindElement(By.XPath(".//following-sibling::td[4]/button[contains(@class, 'btn')]"));
        }

        public static IWebElement Get_host_modify_button(IWebElement host)
        {
            return host.FindElement(By.XPath(".//following-sibling::td[4]/button[contains(@class, 'btn-configure')]"));
        }

        public static IWebElement Get_host_confirm_button(IWebElement host)
        {
            return host.FindElement(By.XPath(".//following-sibling::td[4]/button[contains(@class, 'btn-confirm')]"));
        }

        public static int Get_host_expiration_days(IWebElement host)
        {
            string host_remaining_days;
            try
            {
                host_remaining_days = host.FindElement(By.XPath(".//a[@class='no-link-style']")).Text;
            }
            catch
            {
                host_remaining_days = "Expires in 0 days";
            }
            var regex_match = Regex.Match(host_remaining_days, "\\d+");
            var expiration_days = int.Parse(regex_match.Groups[0].Value);
            return expiration_days;
        }

        public static bool Update_host(IWebElement host)
        {
            var confirm_button = Get_host_confirm_button(host);
            confirm_button.Click();
            Thread.Sleep(3000);
            try
            {
                if (host.FindElements(By.XPath("..//h2[@class='big']"))[0].Text == "Upgrade Now")
                    return true;
            }
            catch {
                return false;
            }

            return false;
        }
    }
}
