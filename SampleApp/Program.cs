using Rockabilly.CoarseGrind;
using Rockabilly.Common;
using System.Collections.Generic;

namespace SampleApp
{
    class Program : TestProgram
    {
        protected override List<Test> AllTests
        {
            get
            {
                List<Test> listOfTests = new List<Test>();

                foreach (TestSuites webDriverCategory in EnumUtil.GetValues<TestSuites>())
                {
                    if (CmdLineBrowsersSpecified.Contains(webDriverCategory.ToString().ToUpper()))
                    {
                        listOfTests.Add(new VerifySearchResults(webDriverCategory));
                    }
                }

                return listOfTests;
            }
        }

        protected override void ProcessArguments(List<string> args)
        {
            // DELIBERATE NO-OP
        }

        static void Main(string[] args)
        {
            new Program().RunTestProgram(args);
        }
    }
}
