using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        // Path to the Symbols.csv file
        string filePath = @"C:\Users\malfa\csharpweb\Symbols.csv";

        // Read all lines (symbols) from the file
        var symbols = File.ReadAllLines(filePath);

        // Initialize the ChromeDriver
        ChromeOptions options = new ChromeOptions();
        options.AddArguments("headless"); // Run in headless mode (without opening a UI window)
        using IWebDriver driver = new ChromeDriver(options);

        // Iterate over each symbol and scrape data
        foreach (var symbol in symbols)
        {
            // Construct the URL for each symbol
            string url = $"https://www.tradingview.com/symbols/TADAWUL-{symbol}/financials-dividends/";

            // Navigate to the URL
            driver.Navigate().GoToUrl(url);

            // Wait for the page to load
            Thread.Sleep(5000); // Increase time if necessary

            // Find the element and extract the data
            try
            {
                IWebElement targetElement = driver.FindElement(By.XPath("//*[@id='js-category-content']/div[2]/div/div/div[8]/div[2]/div/div[1]/div[2]/div[1]/div[2]/span/span"));
                Console.WriteLine($"URL: {url}");
                Console.WriteLine("Target Data: " + targetElement.Text.Trim());
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine($"Element not found for symbol {symbol}.");
            }
            // XPath to locate the company name element
            string xpathForCompanyName = "//*[@id='js-category-content']/div[1]/div[1]/div/div/div/h2";

            try
            {
                // Find the element using XPath
                var companyNameElement = driver.FindElement(By.XPath(xpathForCompanyName));

                // Extract the text from the element
                string companyName = companyNameElement.Text;

                Console.WriteLine("Company Name: " + companyName);
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Element with specified XPath not found.");

                Console.WriteLine("---------------------------------------");
            }

            // Close the browser
            driver.Quit();
        }
    }
}
