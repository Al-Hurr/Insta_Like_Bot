using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
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
        // Класс для повторных попыток
        private static RetryPolicy _retryPolicy;

        static void Main(string[] args)
        {
            // Повторить попыкту 2 раза в случае исключния
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
            // Установляем ведение журнала событий
            options.SetLoggingPreference(LogType.Browser, LogLevel.All);
            // Открываем браузер в режиме разработчика
            options.AddArguments("--auto-open-devtools-for-tabs");

            using (var driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), options, TimeSpan.FromMinutes(5)))
            {
                try
                {
                    // Программа будет выполнятся 7 мин
                    driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromMinutes(7);
                    // Открываем браузер в полноэкранном режиме
                    driver.Manage().Window.Maximize();
                    // Метод входа в аккаунт
                    await LogInAsync(driver);
                    await Delay(3);
                    // Нажимаем кнопку Instagram
                    _retryPolicy.Execute(() =>
                    {
                        ExecuteAsyncJS(driver, "if(document.querySelector('nav a')){document.querySelector('nav a').click();} arguments[arguments.length - 1]();");
                    });
                    // Если появится модалка, чтобы включить уведомления, говорим не включать
                    _retryPolicy.Execute(() =>
                    {
                        ExecuteAsyncJS(driver,
                            "if(document.querySelector('button[class=\"aOOlW   HoLwm \"]')){document.querySelector('button[class=\"aOOlW   HoLwm \"]').click()} arguments[arguments.length - 1]();");
                    });

                    await Delay(3);
                    // Скрипт для нажатия кнопки "мне нрав"
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
                finally
                {
                    driver.Quit();
                }
            }
        }

        static void ExecuteAsyncJS(IWebDriver driver, string js, params object[] parameters)
        {
            // Передаем скрипт для выполнения
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
