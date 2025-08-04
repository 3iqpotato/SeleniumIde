using System;
using System.Collections.Generic;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

[TestFixture]
public class TC01IfUserIsInvalidTryAgainTest
{
    private IWebDriver driver;
    private IJavaScriptExecutor js;
    private IDictionary<string, object> vars;

    [SetUp]
    public void SetUp()
    {
        ChromeOptions options = new ChromeOptions();
        options.AddArgument("--headless"); // �� �������� ������
        driver = new ChromeDriver(options);
        js = (IJavaScriptExecutor)driver;
        vars = new Dictionary<string, object>();
    }

    [TearDown]
    public void TearDown()
    {
        driver.Quit();
    }

    [Test]
    public void TC01IfUserIsInvalidTryAgain()
    {
        driver.Navigate().GoToUrl("https://www.saucedemo.com/");
        driver.Manage().Window.Size = new System.Drawing.Size(1552, 832);

        // ���� � ������ username
        var usernameField = driver.FindElement(By.CssSelector("*[data-test='username']"));
        var passwordField = driver.FindElement(By.CssSelector("*[data-test='password']"));
        var loginButton = driver.FindElement(By.CssSelector("*[data-test='login-button']"));

        usernameField.SendKeys("user123");
        passwordField.SendKeys("secret_sauce");
        loginButton.Click();

        // �������� �� ��������� �� ������
        var errorElement = driver.FindElement(By.CssSelector("*[data-test='error']"));
        var errorMessage = errorElement.Text;

        if (errorMessage == "Epic sadface: Username and password do not match any user in this service")
        {
            Console.WriteLine("Wrong username");

            // ��������� ��������������� ��� � �������� ������
            usernameField = driver.FindElement(By.CssSelector("*[data-test='username']")); // ���������� �� ���������
            usernameField.Clear();
            usernameField.SendKeys("standard_user");
            loginButton = driver.FindElement(By.CssSelector("*[data-test='login-button']"));
            loginButton.Click();

            // ������� �����: ����������� ���� �� �������� �� ���������� � ��������
            var titleElement = driver.FindElement(By.ClassName("title"));
            Assert.That(titleElement.Text, Is.EqualTo("Products"));
            Console.WriteLine("Successful login");
        }

        driver.Close();
    }
}
