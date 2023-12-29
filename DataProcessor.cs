using HtmlAgilityPack;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class DataProcessor
{
    public static List<string[]> ProcessUrl(string number, IWebDriver browser)
    {
        Console.WriteLine($"Processing symbol {number}...");
        string url = $"https://www.tradingview.com/symbols/TADAWUL-{number}/financials-dividends/";
        browser.Navigate().GoToUrl(url);

        List<string[]> outputData = new List<string[]>();
        string primaryXPathBase = "//*[@id='js-category-content']/div[2]/div/div/div[8]/div[2]/div/div[1]/div[{0}]";
        string secondaryXPath = "//*[@id='js-category-content']/div[2]/div/div/div[3]/div/strong";
        int divIndex = 1;

        try
        {
            while (true)
            {
                string primaryElementXPath = String.Format(primaryXPathBase, divIndex);
                try
                {
                    var element = browser.FindElement(By.XPath(primaryElementXPath));
                    ProcessElement(element.GetAttribute("outerHTML"), number, outputData);
                    divIndex++;
                }
                catch (NoSuchElementException)
                {
                    break;
                }
            }
        }
        catch (Exception)
        {
            try
            {
                var element = browser.FindElement(By.XPath(secondaryXPath));
                ProcessElement(element.GetAttribute("outerHTML"), number, outputData);
            }
            catch (Exception)
            {
                // Handle exceptions or ignore
            }
        }

        return outputData;
    }

    private static void ProcessElement(string elementHtml, string number, List<string[]> outputData)
    {
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(elementHtml);
        var elementText = doc.DocumentNode.InnerText;

        string pattern = @"(\d{1,2}/\d{1,2}/\d{4})|(\d+\.\d+)|(\w+)";
        var matches = Regex.Matches(elementText, pattern);
        List<string> flattenedMatches = new List<string>();

        foreach (Match match in matches)
        {
            foreach (Group group in match.Groups)
            {
                if (!string.IsNullOrEmpty(group.Value))
                    flattenedMatches.Add(group.Value);
            }
        }

        string separatedText = string.Join(" ", flattenedMatches);
        outputData.Add(new string[] { number, separatedText });
    }
}
