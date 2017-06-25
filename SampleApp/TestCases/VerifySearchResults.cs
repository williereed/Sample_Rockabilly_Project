using System;
using Rockabilly.CoarseGrind;
using SeleniumCommon;
using System.Threading;

namespace SampleApp
{
    public class VerifySearchResults : SeleniumTest
    {
        private bool CurrentCriteria;
        private Navigate.Environments ThisEnv = Navigate.Environments.Production;

        public VerifySearchResults(TestSuites category) : base(category)
		{
        }

        public override void PerformTest()
        {
            CurrentCriteria = Navigate.GoToUrl(Navigate.SetEnvironment(ThisEnv));
            CheckPassCriterion("Go To Url expects: The Url page to have loaded", CurrentCriteria);

            CurrentCriteria = Navigate.PerformSearch("Quack-a-Doodle Do IMDb");
            CheckPassCriterion("Perform Search expects: To have produced search results", CurrentCriteria);

            CurrentCriteria = Navigate.GotoSearchResultLink("IMDb");
            CheckPassCriterion("Goto First Link In Results expects: To be on the page specified", CurrentCriteria);

            Thread.Sleep(3000);     // just to let the user see the result
        }

        public override bool Cleanup()
        {
            base.Cleanup();
            return true;
        }

        public override string DetailedDescription
        {
            get
            {
                return "Verifies Yahoo search results includes an imdb.com entry for .";
            }
        }

        public override string Identifier
        {
            get
            {
                return "TC01";
            }
        }


        public override string Name
        {
            get
            {
                return "Search Results" +
                    " - " + ThisEnv +
                    " - " + SeleniumInterface.NormalizeBrowserName(webDriverCategory);
            }
        }

        public override bool Setup()
        {
            return base.Setup(); ;
        }

        public override string[] TestSuiteMemberships
        {
            get
            {
                return new String[] { "All", Identifier };
            }
        }

        public override string[] TestCategoryMemberships
        {
            get
            {
                return new String[] { "Example" };
            }
        }

    }
}