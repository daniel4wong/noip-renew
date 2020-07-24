using System;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

using noip_renew.core;
using noip_renew.noip;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;

namespace noip_renew
{
    class Program
    {
        static void Main(string[] args)
        {
            bool debug = ConfigurationManager.AppSettings["debug"].ToLower() == "true";
            bool snapshot = ConfigurationManager.AppSettings["snapshot"].ToLower() == "true";
            string logDir = "_logs";

            if (!Directory.Exists(logDir))
                Directory.CreateDirectory(logDir);

            string username = ConfigurationManager.AppSettings["username"];
            string password = ConfigurationManager.AppSettings["password"];

            try
            {
                int count = 0;
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "--username" || args[i] == "-u")
                    {
                        username = args[i + 1];
                        count++;
                    }
                    if (args[i] == "--password" || args[i] == "-p")
                    {
                        password = args[i + 1];
                        count++;
                    }
                }
                if (count == 1)
                    throw new Exception();
            }
            catch
            {
                Console.WriteLine("Please input parameters as '--username myusername@email.com --password mypassword'...");
                return;
            }

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Console.WriteLine("Please configure your username and password in app.config first (noip-renew.dll.config or noip-renew.exe.config when pulished) ...");
                Console.WriteLine("Or you can input parameters as '--username myusername@email.com --password mypassword'...");
                return;
            }

            Console.WriteLine("Action starts...");

            //Run selenium
            ChromeOptions options = new ChromeOptions();
            if (!debug)
            {
                options.AddArguments(new List<string>()
                {
                    "headless",
                    "no-sandbox",
                    "disable-gpu",
                });
            }
            var service = ChromeDriverService.CreateDefaultService(Environment.CurrentDirectory);
            service.LogPath = $@"{logDir}\chromelog.txt";
            service.SuppressInitialDiagnosticInformation = false;
            service.HideCommandPromptWindow = !debug;
            service.EnableVerboseLogging = true;
            IWebDriver driver = new ChromeDriver(service, options)
            {
                Url = @"https://www.noip.com/login?ref_url=console#!/dynamic-dns"
            };
            driver.Navigate();

            if (snapshot)
                driver.GetScreenshot($@"{logDir}\01_BeforeLogin.png");

            Console.WriteLine("Login starts...");

            if (!NoIpHelper.Login(driver, username, password))
            {
                Console.WriteLine("Login failed.");
            }
            else
            {
                Console.WriteLine("Login completed.");

                if (snapshot)
                    driver.GetScreenshot($@"{logDir}\02_AfterLogin.png");

                //Check can extend expiry date
                var hosts = NoIpHelper.Get_hosts(driver);
                foreach (var host in hosts)
                {
                    var host_name = NoIpHelper.Get_host_name(host);
                    Console.WriteLine("> Handle host {0}...", host_name);

                    var expiration_days = NoIpHelper.Get_host_expiration_days(host);
                    Console.WriteLine("  Expiration days is {0}.", 7);

                    if (expiration_days < 7)
                    {
                        Console.WriteLine("  Update host now...", 7);

                        if (NoIpHelper.Update_host(host))
                        {
                            Console.WriteLine("  * Update host completed.");

                            if (snapshot)
                                driver.GetScreenshot(string.Format($@"{logDir}\03_AfterUpdate_{0}.png", host_name));
                        }
                    }
                    else
                    {
                        Console.WriteLine("  No need to update host.");
                    }
                }

                if (snapshot)
                    driver.GetScreenshot($@"{logDir}\04_Result.png");
            }

            driver.Close();

            Console.WriteLine("Action completed.");
        }
     }
}
