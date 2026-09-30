using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
namespace flaui_poc.Fixtures;

internal abstract class ReportingFixture
{
    protected ExtentReports _extent;
    protected ExtentTest _test;

    [OneTimeSetUp]
    protected void ReportingSetup()
    {
        var dir = TestContext.CurrentContext.TestDirectory + "\\";
        var fileName = GetType().ToString() + ".html";
        var htmlReporter = new ExtentHtmlReporter(dir + fileName);

        _extent = new ExtentReports();
        _extent.AttachReporter(htmlReporter);
    }

    [OneTimeTearDown]
    protected void ReportingTearDown()
    {
        _extent.Flush();
        var dir = TestContext.CurrentContext.TestDirectory + "\\";
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
        {
            WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
            FileName = "cmd.exe",
            Arguments = "/C start \"\" \"" + dir + "index.html\""
        };
        process.StartInfo = startInfo;
        process.Start();
    }

    [SetUp]
    public void BeforeTestReporting()
    {
        _test = _extent.CreateTest(TestContext.CurrentContext.Test.Name);
    }


    [TearDown]
    public void AfterTestReporting()
    {
        var status = TestContext.CurrentContext.Result.Outcome.Status;
        var stacktrace = string.IsNullOrEmpty(TestContext.CurrentContext.Result.StackTrace)
                ? ""
                : string.Format("{0}", TestContext.CurrentContext.Result.StackTrace);
        Status logstatus;

        switch (status)
        {
            case TestStatus.Failed:
                logstatus = Status.Fail;
                break;
            case TestStatus.Inconclusive:
                logstatus = Status.Warning;
                break;
            case TestStatus.Skipped:
                logstatus = Status.Skip;
                break;
            default:
                logstatus = Status.Pass;
                break;
        }

        _test.Log(logstatus, "Test ended with " + logstatus + stacktrace ?? "");
        _extent.Flush();
    }
}
