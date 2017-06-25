using OpenQA.Selenium;
using Rockabilly.CoarseGrind;
using SeleniumCommon;
using System;
using System.Threading;

namespace SeleniumCommon
{
    public static class Navigate
    {
        private static string local = "http://www.bing.com";          // replace with your local url
        private static string dev = "http://www.bing.com";            // replace with your dev url
        private static string test = "http://www.bing.com";           // replace with your test url
        private static string staging = "http://www.bing.com";        // replace with your staging url
        private static string production = "http://www.bing.com";     // replace with your production url

        public enum Environments
        {
            Local,
            Dev,
            Test,
            Staging,
            Production
        }

        /// <summary>
        /// Finds an element using retry, my version of implicit wait because it works more reliably
        /// </summary>
        /// <param name="find">Value to locate</param>
        /// <param name="by">Selenium Find By</param>
        /// <returns>IWebElement</returns>
        public static IWebElement MyFindElement(string find, string by)
        {
            int attempts = 0;
            IWebElement element = null;
            while (element == null && attempts <= 20)
            {
                Thread.Sleep(500);
                try
                {
                    switch (by.ToLower())
                    {
                        case "classname":
                            element = SeleniumInterface.WebDriver.FindElement(By.ClassName(find));
                            break;
                        case "cssselector":
                            element = SeleniumInterface.WebDriver.FindElement(By.CssSelector(find));
                            break;
                        case "id":
                            element = SeleniumInterface.WebDriver.FindElement(By.Id(find));
                            break;
                        case "linktext":
                            element = SeleniumInterface.WebDriver.FindElement(By.LinkText(find));
                            break;
                        case "name":
                            element = SeleniumInterface.WebDriver.FindElement(By.Name(find));
                            break;
                        case "partiallinktext":
                            element = SeleniumInterface.WebDriver.FindElement(By.PartialLinkText(find));
                            break;
                        case "tagname":
                            element = SeleniumInterface.WebDriver.FindElement(By.TagName(find));
                            break;
                        case "xpath":
                            element = SeleniumInterface.WebDriver.FindElement(By.XPath(find));
                            break;
                        default:
                            element = SeleniumInterface.WebDriver.FindElement(By.ClassName(find));
                            break;
                    }

                    if (!element.Enabled)
                    {
                        element = null;
                        throw new Exception();
                    }
                }
                catch
                {
                    attempts++;
                }
            }
            if (attempts >= 40)
                Test.Log.Message(find + " NOT found!");

            Thread.Sleep(1000);
            return element;
        }

        /// <summary>
        /// Sets the Url for the environment being tested
        /// </summary>
        /// <param name="env">Environments enum</param>
        /// <returns>Url for the given environment</returns>
        public static string SetEnvironment(Environments env)
        {
            switch (env)
            {
                case Environments.Local:
                    return local;
                case Environments.Dev:
                    return dev;
                case Environments.Test:
                    return test;
                case Environments.Staging:
                    return staging;
                case Environments.Production:
                    return production;
                default:
                    return "unsupported";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static bool HandleCertWarning()
        {
            IWebElement element = null;

            // Firefox - the Cert Warning is handled via custom profile with saved exception
            // Safari - support is not yet developed
            // Chrome - no need to code this, user saves once and it's done

            if (SeleniumInterface.WebDriverName == TestSuites.INTERNETEXPLORER.ToString())
            {
                Thread.Sleep(500);
                try { element = SeleniumInterface.WebDriver.FindElement(By.Id("overridelink")); }
                catch { }
                if (element != null)
                {
                    SeleniumInterface.WebDriver.Navigate().GoToUrl("javascript:document.getElementById('overridelink').click()");
                }
            }

            if (SeleniumInterface.WebDriverName == TestSuites.EDGE.ToString())
            {
                Thread.Sleep(500);
                try { element = SeleniumInterface.WebDriver.FindElement(By.Id("invalidcert_continue")); }
                catch { }
                if (element != null)
                {
                    SeleniumInterface.WebDriver.Navigate().GoToUrl("javascript:document.getElementById('invalidcert_continue').click()");
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static bool GoToUrl(string Url)
        {
            Test.Log.DebugLevelOnly("GoToUrl", (int)Test.GLL);
            bool loop = true;
            int attempts = 0;
            while (loop && attempts < 3)
            {
                try
                {
                    SeleniumInterface.WebDriver.Url = Url;
                    // 
                    HandleCertWarning();
                    if (SeleniumInterface.WebDriver.Url.Contains(Url))
                        loop = false;
                    attempts++;

                }
                catch (Exception E)
                {
                    if (attempts == 2)
                        throw E;
                    attempts++;
                }
            }
            Thread.Sleep(500);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static bool PerformSearch(string value)
        {
            IWebElement search = MyFindElement("sb_form_q", "Id");
            if (search == null)
                return false;
            search.SendKeys(value);
            search.SendKeys(Keys.Return);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static bool GotoSearchResultLink(string value)
        {
            Thread.Sleep(2000);
            System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> search = SeleniumInterface.WebDriver.FindElements(By.TagName("a"));
            if (search == null)
                return false;
            foreach (IWebElement s in search)
            {
                try
                {
                    if (s.Text.Contains(value))
                    {
                        s.Click();
                        if (SeleniumInterface.WebDriverName == TestSuites.INTERNETEXPLORER.ToString())
                        {
                            Thread.Sleep(100);
                            s.Click();
                        }
                        break;
                    }
                }
                catch { }
            }
            Thread.Sleep(500);
            return VerifyTitle(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static bool VerifyTitle(string value)
        {
            System.Collections.ObjectModel.ReadOnlyCollection<String> tabs2 = new System.Collections.ObjectModel.ReadOnlyCollection<String>(SeleniumInterface.WebDriver.WindowHandles);

            int count = 0;
            foreach (String s in tabs2)
            {
                SeleniumInterface.WebDriver.SwitchTo().Window(tabs2[count]);
                Thread.Sleep(500);
                if (SeleniumInterface.WebDriver.Title.Contains(value))
                    return true;
                count++;
            }
            return false;
        }

    }
}
