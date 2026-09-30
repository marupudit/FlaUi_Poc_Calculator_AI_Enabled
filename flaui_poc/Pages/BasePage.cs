using System;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;

namespace flaui_poc.Pages;

enum IdentifierType
{
    AutomationId,
    XPath,
    Name,
    Text
}

internal class BasePage(Window window) : IDisposable
{
    protected readonly Window Window = window;

    protected T FindElement<T>(IdentifierType type, string identifier) where T : AutomationElement
    {
        T? element = null;

        // Timeout of 10s, interval of 2s, throw error on failure
        Retry.WhileTrue(() =>
        {
            element = type switch
            {
                IdentifierType.Text => Window.FindFirstDescendant(cf => cf.ByText(identifier)).As<T>(),
                IdentifierType.XPath => Window.FindFirstByXPath(identifier).As<T>(),
                IdentifierType.Name => Window.FindFirstDescendant(cf => cf.ByName(identifier)).As<T>(),
                IdentifierType.AutomationId => Window.FindFirstDescendant(cf => cf.ByAutomationId(identifier)).As<T>(),
                _ => throw new NotImplementedException()
            };
            return element == null;
        }, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2), true); 

        return element!;
    }

    public void Dispose() { }
}
