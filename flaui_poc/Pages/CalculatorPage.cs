using FlaUI.Core.AutomationElements;

namespace flaui_poc.Pages;

internal class CalculatorPage(Window window) : BasePage(window)
{
    private const string PlusButtonId = "plusButton";
    private const string MinusButtonId = "minusButton";
    private const string MultiplyButtonId = "multiplyButton";
    private const string DecimalButtonId = "decimalSeparatorButton";
    private const string EqualsButtonId = "equalButton";
    private const string ResultsTextId = "CalculatorResults";

    public bool IsValid()
    {
        return Window != null;
    }

    public string GetTitle()
    {
        try
        {
            return Window?.Properties.Name.ValueOrDefault ?? "No Title";
        }
        catch
        {
            return "Error";
        }
    }

    public void ClickPlus()
    {
        AutomationElement element = Window.FindFirstDescendant(cf => cf.ByAutomationId(PlusButtonId));
        if (element == null) throw new System.Exception("Button 'Plus' not found");
        Button button = element.AsButton();
        button.Invoke();
    }

    public void ClickMinus()
    {
        AutomationElement element = Window.FindFirstDescendant(cf => cf.ByAutomationId(MinusButtonId));
        if (element == null) throw new System.Exception("Button 'Minus' not found");
        Button button = element.AsButton();
        button.Invoke();
    }

    public void ClickMultiply()
    {
        AutomationElement element = Window.FindFirstDescendant(cf => cf.ByAutomationId(MultiplyButtonId));
        if (element == null) throw new System.Exception("Button 'Multiply' not found");
        Button button = element.AsButton();
        button.Invoke();
    }

    public void ClickDecimal()
    {
        AutomationElement element = Window.FindFirstDescendant(cf => cf.ByAutomationId(DecimalButtonId));
        if (element == null) throw new System.Exception("Button 'Decimal' not found");
        Button button = element.AsButton();
        button.Invoke();
    }

    public void ClickDigit(char digit)
    {
        string automationId = $"num{digit}Button";
        AutomationElement element = Window.FindFirstDescendant(cf => cf.ByAutomationId(automationId));
        if (element == null) throw new System.Exception($"Button '{digit}' not found");
        Button button = element.AsButton();
        button.Invoke();
    }

    public void EnterNumber(double number)
    {
        string numberString = number.ToString(System.Globalization.CultureInfo.InvariantCulture);
        foreach (char ch in numberString)
        {
            if (ch == '.')
            {
                ClickDecimal();
            }
            else
            {
                ClickDigit(ch);
            }
        }
    }

    public void ClickEquals()
    {
        AutomationElement element = Window.FindFirstDescendant(cf => cf.ByAutomationId(EqualsButtonId));
        if (element == null) throw new System.Exception("Button 'Equals' not found");
        Button button = element.AsButton();
        button.Invoke();
    }

    public string GetResult()
    {
        AutomationElement resultElement = Window.FindFirstDescendant(cf => cf.ByAutomationId(ResultsTextId));
        if (resultElement == null) throw new System.Exception("Result element not found");
        return resultElement.Properties.Name.ValueOrDefault ?? string.Empty;
    }
}


