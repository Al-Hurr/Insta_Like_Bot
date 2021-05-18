using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Polly;
using Polly.Retry;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Insta_Like_Bot
{
    class Program
    {
        private static string _containerLikeScript;
        //cmd -> setx value_name value
        private static string _login;
        private static string _password;
        private static readonly string _instaURL = "https://www.instagram.com";
        private static RetryPolicy _retryPolicy;

        static void Main(string[] args)
        {
            _retryPolicy = Policy.Handle<Exception>().Retry(2);
            _containerLikeScript = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "LikeScript.js"));

            _login = Environment.GetEnvironmentVariable("LOGIN_INST");
            _password = Environment.GetEnvironmentVariable("PASS_INST");

            if (String.IsNullOrEmpty(_login) || String.IsNullOrEmpty(_password))
            {
                Console.WriteLine("Login failed\nLogin or pass is empty.");
                return;
            }

            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            var options = new ChromeOptions();
            options.SetLoggingPreference(LogType.Browser, LogLevel.All);

            using (var driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), options, TimeSpan.FromMinutes(5)))
            {
                try
                {
                    driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromMinutes(5);
                    await LogInAsync(driver);
                    await Delay(3);

                    _retryPolicy.Execute(() =>
                    {
                        ExecuteAsyncJS(driver, "if(document.querySelector('nav a')){document.querySelector('nav a').click();} arguments[arguments.length - 1]();");
                    });

                    _retryPolicy.Execute(() =>
                    {
                        ExecuteAsyncJS(driver,
                            "if(document.querySelector('button[class=\"aOOlW   HoLwm \"]')){document.querySelector('button[class=\"aOOlW   HoLwm \"]').click()} arguments[arguments.length - 1]();");
                    });

                    await Delay(3);

                    ExecuteAsyncJS(driver, _containerLikeScript);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Logs from browser:");
                    var logs = driver.Manage().Logs.GetLog(LogType.Browser).ToList();
                    foreach (var log in logs)
                    {
                        Console.WriteLine(log);
                    }
                    throw;
                }
            }
        }

        static void ExecuteAsyncJS(IWebDriver driver, string js, params object[] parameters)
        {
            ((IJavaScriptExecutor)driver).ExecuteAsyncScript(js, parameters);
        }

        static async Task LogInAsync(IWebDriver driver)
        {
            _retryPolicy.Execute(() => driver.Navigate().GoToUrl(_instaURL));
            await Delay(2);

            var loginElement = driver.FindElement(By.Name("username"));
            loginElement.SendKeys(_login);

            var passElement = driver.FindElement(By.Name("password"));
            passElement.SendKeys(_password);

            ClickOnLoginButton(driver);
        }

        private static void ClickOnLoginButton(IWebDriver driver)
        {
            _retryPolicy.Execute(() =>
            {
                ExecuteAsyncJS(driver,
                    "if(document.querySelector(\"button[type = 'submit']\")){document.querySelector(\"button[type = 'submit']\").click();} " +
                    "arguments[arguments.length - 1]();");
            });
        }

        private static Task Delay(int sec)
        {
            return Task.Delay(sec * 1000);
        }
    }
}
