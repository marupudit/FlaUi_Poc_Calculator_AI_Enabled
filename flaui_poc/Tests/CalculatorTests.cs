using flaui_poc.Fixtures;
using flaui_poc.Pages;
using NUnit.Framework;

namespace flaui_poc.Tests;

internal class CalculatorTests : BaseTest
{
    private CalculatorPage? CalculatorPage;

    public CalculatorTests() : base("calc.exe") { }

    [SetUp]
    public new void Setup()
    {
        if (Window != null)
        {
            CalculatorPage = new CalculatorPage(Window);
        }
    }

    [Test]
    public void CalculatorApplicationShouldStart()
    {
        Assert.That(ProcessReference, Is.Not.Null, "Process should have started");
        Assert.That(ProcessReference!.Id, Is.GreaterThan(0), "Process ID should be valid");
    }

    [Test]
    public void CalculatorPageTitleShouldNotBeEmpty()
    {
        CalculatorPage = new CalculatorPage(Window ?? throw new System.InvalidOperationException("Window is null"));
        string title = CalculatorPage.GetTitle();
        Assert.That(title, Is.EqualTo("Calculator"), "Calculator window title should be 'Calculator'");
    }

    [Test]
    public void CalculatorPageShouldBeValid()
    {
        CalculatorPage = new CalculatorPage(Window ?? throw new System.InvalidOperationException("Window is null"));
        Assert.That(CalculatorPage.IsValid(), Is.True, "Calculator page should be valid");
    }

    [Test]
    public void Addition_ShouldReturnCorrectSum()
    {
        CalculatorPage = new CalculatorPage(Window ?? throw new System.InvalidOperationException("Window is null"));
        
        // Enter 1
        CalculatorPage.EnterNumber(1);
        
        // Click Add
        CalculatorPage.ClickPlus();
        
        // Enter 2
        CalculatorPage.EnterNumber(2);
        
        // Click Equals
        CalculatorPage.ClickEquals();
        
        // Verify result is 3
        string result = CalculatorPage.GetResult();
        Assert.That(result, Does.Contain("3"), "Calculator result should contain 3");
    }

    [Test]
    public void Subtraction_ShouldReturnCorrectDifference()
    {
        CalculatorPage = new CalculatorPage(Window ?? throw new System.InvalidOperationException("Window is null"));
        
        // Enter 786
        CalculatorPage.EnterNumber(786);
        
        // Click Subtract
        CalculatorPage.ClickMinus();
        
        // Enter 512
        CalculatorPage.EnterNumber(512);
        
        // Click Equals
        CalculatorPage.ClickEquals();
        
        // Verify result is 274
        string result = CalculatorPage.GetResult();
        Assert.That(result, Does.Contain("274"), "Calculator result should contain 274");
    }

    [Test]
    public void Multiplication_ShouldReturnCorrectProduct()
    {
        CalculatorPage = new CalculatorPage(Window ?? throw new System.InvalidOperationException("Window is null"));
        
        // Enter 2.5
        CalculatorPage.EnterNumber(2.5);
        
        // Click Multiply
        CalculatorPage.ClickMultiply();
        
        // Enter 3.4
        CalculatorPage.EnterNumber(3.4);
        
        // Click Equals
        CalculatorPage.ClickEquals();
        
        // Verify result is 8.5
        string result = CalculatorPage.GetResult();
        Assert.That(result, Does.Contain("8.5"), "Calculator result should contain 8.5");
    }
}
