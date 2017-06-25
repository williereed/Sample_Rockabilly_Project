using Rockabilly.CoarseGrind;
using System.Collections.Generic;
using System.Text;

namespace SeleniumCommon
{
    public abstract class SeleniumTest : Test
    {

        protected TestSuites webDriverCategory = default(TestSuites);

        public string SubIdentifier
        {
            get
            {
                return "." + WebDriverName;
            }
        }

        public string SubName
        {
            get
            {
                return " (" + WebDriverName + ")";
            }
        }

        public string WebDriverName
        {
            get
            {
                return "STUB";
                //return webDriverCategory.ToString();
            }
        }

        public override string[] TestSuiteMemberships
        {
            get
            {
                List<string> categories = new List<string>();

                return categories.ToArray();
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="category">The TestSuite value for this test</param>
        /// <returns></returns>
        public SeleniumTest(TestSuites category)
        {
            webDriverCategory = category;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public override bool Setup()
        {
            SeleniumInterface.DeclareCurrentTestCaseIs(this);
            return SeleniumInterface.SetWebDriver(webDriverCategory);
            //Global.Log.message("Instantiating project-specific WebElement finders");
            //instantiateWebElementFinders(webDriver);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public override bool Cleanup()
        {
            return SeleniumInterface.KillWebDriver();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        private bool addVerbosity(bool condition)
        {
            if (!condition)
            {
                Log.SkipLine();
                SeleniumInterface.TakeScreenShot();
            }
            return condition;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        private string combinedDescription(bool succeeded, bool isPrereq, string description)
        {
            StringBuilder result = new StringBuilder();

            if (succeeded)
            {
                result.Append("Pass");
            }
            else
            {
                result.Append("Fail");
            }

            if (isPrereq)
            {
                result.Append("ed Prerequisite");
            }

            result.Append(" - ");
            result.Append(description);

            return result.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public override void CheckPrerequisite(string conditionDescription, bool condition)
        {
            base.CheckPrerequisite(conditionDescription, addVerbosity(condition));
            Log.SkipLine();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public override void CheckPassCriterion(string conditionDescription, bool condition)
        {
            base.CheckPassCriterion(conditionDescription, addVerbosity(condition));
            Log.SkipLine();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public override void CheckCondition(string conditionDescription, bool condition)
        {
            base.CheckCondition(conditionDescription, addVerbosity(condition));
            Log.SkipLine();
        }
    }
}
