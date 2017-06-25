using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;
using Rockabilly.CoarseGrind;
using Rockabilly.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;

namespace SeleniumCommon
{
    public abstract class SeleniumInterface
    {
        private static IWebDriver webDriver = null;
        private static TestSuites webDriverCategory = default(TestSuites);
        private static SeleniumTest currentTestCase = null;
        private static By outfield = null;
        private static string browserVersion = null;
        private static int BrowserRetryMax = 2;
        public static bool OSX
        {
            get
            {
                // When it's time to run on Mac need to determine if we are actually on Mac
                return false;
            }
        }
        public static bool IE
        {
            get
            {
                return webDriverCategory == TestSuites.INTERNETEXPLORER;
            }
        }
        public static bool Chrome
        {
            get
            {
                return webDriverCategory == TestSuites.CHROME;
            }
        }
        public static bool Safari
        {
            get
            {
                return webDriverCategory == TestSuites.SAFARI;
            }
        }
        public static bool Firefox
        {
            get
            {
                return webDriverCategory == TestSuites.FIREFOX;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static List<IWebElement> ListCollection(ReadOnlyCollection<IWebElement> collection)
        {
            List<IWebElement> result = new List<IWebElement>();
            result.AddRange(collection);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static void DeclareOutfield(By thisWay)
        {
            outfield = thisWay;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static void ClearOutfield()
        {
            outfield = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static void ClickOutfield()
        {
            if (outfield == null)
            {
                throw new NoSuchElementException("You must call declareOutfield() first.");
            }
            else
            {
                FindOutfield().Click();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        private static IWebElement FindOutfield()
        {
            return webDriver.FindElement(outfield);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static void DeclareCurrentTestCaseIs(SeleniumTest thisTestCase)
        {
            ClearCurrentTestCase();
            currentTestCase = thisTestCase;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static void ClearCurrentTestCase()
        {
            currentTestCase = null;
            GC.Collect();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static SeleniumTest CurrentTestCase
        {
            get
            {
                return currentTestCase;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static string WebDriverName
        {
            get
            {
                return webDriverCategory.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static IWebDriver WebDriver
        {
            get
            {
                return webDriver;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        private static void KillSafariDriver()
        {
            Process.Start("pkill", "safaridriver");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static void SetSafariSize(int width, int height)
        {
            string paramString = "resize.scpt " + width + " " + height;
            Process process = Process.Start("osascript", paramString);
            Console.WriteLine("Running resize.scpt " + width + " " + height + "...");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static void FocusApplication()
        {
            if (OSX)
            {
                string name;
                switch (webDriverCategory)
                {
                    case TestSuites.SAFARI:
                        name = "Safari";
                        break;
                    case TestSuites.FIREFOX:
                        name = "Firefox";
                        break;
                    default:
                        name = "Chrome";
                        break;
                }
                string strCmdText = "-e 'activate application \"" + name + "\"'";
                System.Diagnostics.Process.Start("osascript", strCmdText);
                Thread.Sleep(2000);
            }
        }

        /* SAFARIDRIVER doesn't allow this. Acts as if normal input and gives the pop up warning.
                public static void SafariFullscreen(bool toggle = true)
                {
                    if (toggle != IsSafariFullscreen())
                    {
                        Process process = Process.Start("osascript", "fullscreen.scpt");
                        Console.WriteLine("Running fullscreent.scpt...");
                        do
                        {
                            if (!process.HasExited)
                            {
                                Console.Write(".");
                            }
                        }
                        while (!process.WaitForExit(100));
                    }
                }
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static bool IsSafariFullscreen()
        {
            Process process = Process.Start("osascript", "isFullscreen.scpt");
            Console.WriteLine("Running isFullscreen.scpt...");
            do
            {
                if (!process.HasExited)
                {
                    Console.Write(".");
                }
            }
            while (!process.WaitForExit(100));
            if (process.ExitCode == 0)
                return true;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static void LoadIEDriver(int CurrentAttempt)
        {
            if (CurrentAttempt <= BrowserRetryMax)
            {
                try
                {
                    InternetExplorerDriverService service = InternetExplorerDriverService.CreateDefaultService();
                    service.HideCommandPromptWindow = true;
                    InternetExplorerOptions options = new InternetExplorerOptions();
                    options.EnsureCleanSession = false;                                 // testing if this will help open Url in a new tab rather than new window
                    webDriver = new InternetExplorerDriver(service, options);
                    browserVersion = GetIEVersion();
                    Test.Log.Message("Internet Explorer " + browserVersion);
                }
                catch (Exception ex)
                {
                    Test.Log.Message("Error IE driver failed to load: " + ex.Message, LoggingLevel.Critical, Test.Log.StopSignIcon);
                    LoadIEDriver(CurrentAttempt++);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static void LoadChromeDriver(int CurrentAttempt)
        {
            if (CurrentAttempt <= BrowserRetryMax)
            {
                try
                {
                    ChromeDriverService service = ChromeDriverService.CreateDefaultService();
                    service.HideCommandPromptWindow = true;
                    webDriver = new ChromeDriver(service);
                    browserVersion = ((RemoteWebDriver)webDriver).Capabilities.Version;
                    Test.Log.Message("Chrome " + browserVersion);
                }
                catch (Exception ex)
                {
                    Test.Log.Message("Error Chrome driver failed to load: " + ex.Message);
                    LoadChromeDriver(CurrentAttempt++);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static void LoadPhantomJSDriver(int CurrentAttempt)
        {
            if (CurrentAttempt <= BrowserRetryMax)
            {
                try
                {
                    webDriver = new PhantomJSDriver();
                    browserVersion = ((RemoteWebDriver)webDriver).Capabilities.Version;
                    Test.Log.Message("PhantomJS " + browserVersion);
                }
                catch (Exception ex)
                {
                    Test.Log.Message("Error PhantomJS driver failed to load: " + ex.Message);
                    LoadPhantomJSDriver(CurrentAttempt++);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static void LoadFirefoxDriver(int CurrentAttempt)
        {
            if (CurrentAttempt <= BrowserRetryMax)
            {
                try
                {
                    string firefoxProfilePath = @"C:\Users\wreed\AppData\Roaming\Mozilla\Firefox\Profiles\jytz2xra.Automation";

                    if (Directory.Exists(firefoxProfilePath))
                    {
                        // These files are left behind when a test case terminates prematurely
                        // they prevent loading the browser next run
                        if (File.Exists(firefoxProfilePath + "\\AlternateServices.txt"))
                            File.Delete(firefoxProfilePath + "\\AlternateServices.txt");
                        if (File.Exists(firefoxProfilePath + "\\parent.lock"))
                            File.Delete(firefoxProfilePath + "\\parent.lock");
                        if (File.Exists(firefoxProfilePath + "\\SecurityPreloadState.txt"))
                            File.Delete(firefoxProfilePath + "\\SecurityPreloadState.txt");

                        FirefoxDriverService driverService = FirefoxDriverService.CreateDefaultService();
                        driverService.HideCommandPromptWindow = true;

                        webDriver = new FirefoxDriver(driverService);
                        FocusApplication();
                        browserVersion = GetFirefoxVersion();
                        Test.Log.Message("Firefox " + browserVersion);
                    }
                    else
                    {
                        string message = "Firefox profile path not found (see UserSettings.cfg for help): " +
                            firefoxProfilePath;
                        Test.Log.Message(message);
                    }
                }
                catch (Exception ex)
                {
                    Test.Log.Message("Error Firefox driver failed to load: " + ex.Message);
                    Test.Log.Message(ex.StackTrace);
                    LoadFirefoxDriver(CurrentAttempt++);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static string SetSafariJSClickEvent()
        {
            string compressedJS = "function fireEvent(e,t){var n;if(e.ownerDocument)n=e.ownerDocument;else{if(9!=e.nodeType)throw new Error(\"Invalid node passed to fireEvent: \"+e.id);n=e}if(e.dispatchEvent){var s=\"\";switch(t){case\"click\":case\"mouseover\":case\"mousedown\":case\"mouseup\":s=\"MouseEvents\";break;case\"focus\":case\"change\":case\"blur\":case\"select\":s=\"HTMLEvents\";break;default:throw\"fireEvent: Couldn't find an event class for event '\"+t+\"'.\"}(c=n.createEvent(s)).initEvent(t,!0,!0),c.synthetic=!0,e.dispatchEvent(c,!0)}else if(e.fireEvent){var c=n.createEventObject();c.synthetic=!0,e.fireEvent(\"on\"+t,c)}}";
            return compressedJS;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static void LoadSafariDriver(int CurrentAttempt)
        {
            if (CurrentAttempt <= BrowserRetryMax)
            {
                try
                {
                    if (CurrentAttempt == 0)
                        KillSafariDriver();
                    SafariDriverService serv = SafariDriverService.CreateDefaultService("/Applications/Safari Technology Preview.app/Contents/MacOS/", "safaridriver");
                    SafariOptions opts = new SafariOptions();
                    opts.AddAdditionalCapability(CapabilityType.AcceptSslCertificates, true);
                    opts.AddAdditionalCapability(CapabilityType.AcceptInsecureCertificates, true);
                    opts.AddAdditionalCapability("cleanSession", true);
                    webDriver = new SafariDriver(serv, opts);
                    Test.Log.Message("Safari " + browserVersion);
                }
                catch (Exception ex)
                {
                    Test.Log.Message("Error Safari driver failed to load: " + ex.Message);
                    LoadSafariDriver(CurrentAttempt++);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static void LoadEdgeDriver(int CurrentAttempt)
        {
            if (CurrentAttempt <= BrowserRetryMax)
            {
                try
                {
                    var driverService = EdgeDriverService.CreateDefaultService();
                    driverService.HideCommandPromptWindow = true;
                    webDriver = new EdgeDriver(driverService);
                    browserVersion = GetEdgeVersion();
                    Test.Log.Message("Edge " + browserVersion);
                }
                catch (Exception ex)
                {
                    Test.Log.Message("Error Edge driver failed to load: " + ex.Message);
                    LoadEdgeDriver(CurrentAttempt++);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static bool SetWebDriver(TestSuites category)
        {
            if (webDriver != null)
            {
                if (category == webDriverCategory)
                    return true;
                else
                    KillWebDriver();
            }

            webDriverCategory = category;
            Test.Log.Message("Starting Selenium WebDriver for " + WebDriverName);

            switch (webDriverCategory)
            {
                case TestSuites.CHROME:
                    LoadChromeDriver(0);
                    break;
                case TestSuites.EDGE:
                    LoadEdgeDriver(0);
                    break;
                case TestSuites.FIREFOX:
                    LoadFirefoxDriver(0);
                    break;
                case TestSuites.INTERNETEXPLORER:
                    LoadIEDriver(0);
                    break;
                case TestSuites.PHANTOMJS:
                    LoadPhantomJSDriver(0);
                    break;
                case TestSuites.SAFARI:
                    LoadSafariDriver(0);
                    break;
                default:
                    Test.Log.Message("Tests needing Selenium for " + WebDriverName + " are not supported at this time. Test Programmers must add support.",
                        LoggingLevel.Critical, Test.Log.StopSignIcon);
                    return false;
            }

            webDriver.Manage().Window.Maximize();
            Test.Log.DebugLevelOnly("Window size = " + webDriver.Manage().Window.Size, (int)Test.GLL);
            Test.Log.DebugLevelOnly("Window position = " + webDriver.Manage().Window.Position, (int)Test.GLL);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static bool KillWebDriver()
        {
            if (webDriver == null)
            {
                Test.Log.Message("Selenium IWebDriver for " + WebDriverName + " was never initialized. Nothing to do.");
                return true;
            }

            Test.Log.Message("Closing Selenium IWebDriver for " + WebDriverName);
            webDriver.Close();

            Test.Log.Message("Quitting Selenium IWebDriver for " + WebDriverName);
            webDriver.Quit();

            Test.Log.Message("Nulling out IWebDriver variable");
            webDriver = null;
            GC.Collect();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static void TakeScreenShot()
        {
            string fileName = Foundation.TimeStamp + " - SCREENSHOT.jpeg";
            try
            {
                ((ITakesScreenshot)webDriver).GetScreenshot().SaveAsFile(CurrentTestCase.ArtifactsDirectory + Path.DirectorySeparatorChar + fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                Test.Log.Message("Saved screenshot to <a href=\"" + fileName + "\">" + fileName + "</a>", LoggingLevel.Critical);
            }
            catch
            {
                Test.Log.Message("Failed to get Selenium screenshot", icon: Test.Log.WarningIcon); ;
            }
            finally
            {
                fileName = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static void ScrollToBottom()
        {
            scrollWindow(10000);
            Thread.Sleep(1000);
            //((JavascriptExecutor) webDriver).executeScript("window.scrollBy(0,1000)", "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static void scrollToTop()
        {
            scrollWindow(0);
            Thread.Sleep(1000);
            //((JavascriptExecutor) webDriver).executeScript("window.scrollBy(0,-1000)", "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static void scrollWindow(int ticks)
        {
            ((IJavaScriptExecutor)webDriver).ExecuteScript("window.scrollTo(0," + ticks + ");", "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static long ScrollPosition()
        {
            long Position;
            try
            {
                IJavaScriptExecutor Executor = (IJavaScriptExecutor)WebDriver;
                Position = (long)Executor.ExecuteScript("return window.pageYOffset;");
            }
            catch (Exception E)
            {
                throw new Exception("[WebDriver] Failed to get scroll position.", E);
            }
            return Position;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static void ForceOnscreenClick(IWebElement thisIWebElement)
        {
            Exception lastException = null;
            scrollToTop();
            //highlightElement(thisIWebElement);

            for (int retries = 0; retries < 100; retries++)
            {
                try
                {
                    thisIWebElement.Click();
                    //unHighlightElement(thisIWebElement);
                    return;
                }
                catch (Exception dontCare)
                {
                    scrollWindow(150);
                    lastException = dontCare;
                }
            }

            throw lastException;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static IWebElement ForceOnscreenGet(By how)
        {
            Exception lastException = null;
            scrollToTop();

            for (int retries = 0; retries < 100; retries++)
            {
                try
                {
                    return webDriver.FindElement(how);
                }
                catch (Exception dontCare)
                {
                    scrollWindow(150);
                    lastException = dontCare;
                }
            }

            throw lastException;
        }

        /// <summary>
        /// Based on http://stackoverflow.com/questions/4176560/webdriver-get-elements-xpath
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static string GetXpathOf(IWebElement target)
        {
            return (string)((IJavaScriptExecutor)webDriver).ExecuteScript("gPt=function(c){if(c.id!==''){return'id(\"'+c.id+'\")'}if(c===document.body){return c.tagName}var a=0;var e=c.parentNode.childNodes;for(var b=0;b<e.length;b++){var d=e[b];if(d===c){return gPt(c.parentNode)+'/'+c.tagName+'['+(a+1)+']'}if(d.nodeType===1&&d.tagName===c.tagName){a++}}};return gPt(arguments[0]).toLowerCase();", target);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static void RefreshPage()
        {
            Test.Log.Message("Refreshing the page");
            webDriver.Navigate().Refresh();
            Thread.Sleep(5000);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static string NormalizeBrowserName(TestSuites driver)
        {
            switch (driver)
            {
                case TestSuites.CHROME:
                    return "Chrome";
                case TestSuites.EDGE:
                    return "Edge";
                case TestSuites.FIREFOX:
                    return "FireFox";
                case TestSuites.INTERNETEXPLORER:
                    return "Internet Explorer";
                case TestSuites.SAFARI:
                    return "Safari";
                default:
                    return "Unsupport";
            }
        }

        /// <summary>
        /// Finds the Product Version of a specified file
        /// </summary>
        /// <param name="exe">Fully qualified path to exe file</param>
        /// <returns>Product Version</returns>
        private static string GetVersion(string exe)
        {
            string version = "";
            if (File.Exists(exe))
            {
                var versionInfo = FileVersionInfo.GetVersionInfo(exe);
                version = versionInfo.ProductVersion;
            }
            Test.Log.DebugLevelOnly("GetVersion = " + version, (int)Test.GLL);
            return version;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static string GetEdgeVersion()
        {
            return GetVersion(@"C:\Windows\SystemApps\Microsoft.MicrosoftEdge_8wekyb3d8bbwe\MicrosoftEdge.exe");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static string GetFirefoxVersion()
        {
            return GetVersion(System.Environment.GetEnvironmentVariable("ProgramFiles") + @"\Mozilla Firefox\firefox.exe");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static string GetIEVersion()
        {
            return GetVersion(System.Environment.GetEnvironmentVariable("ProgramFiles") + @"\Internet Explorer\iexplore.exe");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static void ResizeWindow(int Width, int Height)
        {
            Test.Log.DebugLevelOnly("ResizeWindow", (int)Test.GLL);
            WebDriver.Manage().Window.Size = new Size(Width, Height);
            if (!Safari)
                WebDriver.Manage().Window.Position = new Point(0, 0);
            Thread.Sleep(1000);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static int GetWindowHeight()
        {
            return WebDriver.Manage().Window.Size.Height;
        }

    }
}
