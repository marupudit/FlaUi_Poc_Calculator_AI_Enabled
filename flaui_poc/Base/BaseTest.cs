using System;
using System.Diagnostics;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using NUnit.Framework;

namespace flaui_poc.Fixtures;

[TestFixture]
internal abstract class BaseTest(string applicationPath) : ReportingFixture, IDisposable
{
    private readonly string applicationPath = applicationPath;
    protected Window? Window { get; set; }
    private Application? Application { get; set; }
    private UIA3Automation? Automation { get; set; }
    protected Process? ProcessReference { get; set; }

    [SetUp]
    public void LaunchApp()
    {
        Automation = new UIA3Automation();

        bool isCalc = applicationPath.Equals("calc.exe", StringComparison.OrdinalIgnoreCase) ||
                      applicationPath.Equals("calc", StringComparison.OrdinalIgnoreCase);

        Application = isCalc
            ? FlaUI.Core.Application.LaunchStoreApp("Microsoft.WindowsCalculator_8wekyb3d8bbwe!App")
            : FlaUI.Core.Application.Launch(applicationPath);

        if (Application != null)
        {
            if (isCalc)
            {
                var calcProcesses = FlaUI.Core.Tools.Retry.WhileEmpty(
                    () => Process.GetProcessesByName("CalculatorApp"),
                    TimeSpan.FromSeconds(10)
                ).Result;
                
                if (calcProcesses != null && calcProcesses.Length > 0)
                {
                    ProcessReference = calcProcesses[0];
                    try 
                    {
                        using (var tempApp = FlaUI.Core.Application.Attach(ProcessReference.Id))
                        {
                            Window = tempApp.GetMainWindow(Automation, TimeSpan.FromSeconds(10));
                        }
                    }
                    catch
                    {
                        // Fallback to desktop search
                        Window = FlaUI.Core.Tools.Retry.WhileNull(
                            () => Automation.GetDesktop().FindFirstChild(cf => cf.ByName("Calculator")),
                            TimeSpan.FromSeconds(10)
                        ).Result?.AsWindow();
                    }
                }
            }
            else
            {
                Application.WaitWhileMainHandleIsMissing(TimeSpan.FromSeconds(5));
                Window = Application.GetMainWindow(Automation);
                try { ProcessReference = Process.GetProcessById(Application.ProcessId); } catch { }
            }
        }
    }

    [TearDown]
    public void CleanupApp()
    {
        try
        {
            Application?.Close();
            Application?.Dispose();
        }
        catch { }
        finally { Application = null; }

        Automation?.Dispose();
        Automation = null;
        Window = null;

        CleanupLingeringProcesses();
    }

    private void CleanupLingeringProcesses()
    {
        try
        {
            if (ProcessReference != null && !ProcessReference.HasExited)
            {
                ProcessReference.Kill(true);
                ProcessReference.WaitForExit(3000);
            }
            ProcessReference?.Dispose();
        }
        catch { }
        finally { ProcessReference = null; }

        // Target both modern UWP and legacy calculator processes
        foreach (var processName in new[] { "CalculatorApp", "calc" })
        {
            foreach (var process in Process.GetProcessesByName(processName))
            {
                try
                {
                    if (!process.HasExited)
                    {
                        process.Kill(true);
                        process.WaitForExit(2000);
                    }
                    process.Dispose();
                }
                catch { }
            }
        }
    }

    public void Dispose()
    {
        try { CleanupApp(); } catch { }
    }
}