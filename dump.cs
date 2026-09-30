using System;
using System.Diagnostics;
using FlaUI.Core;
using FlaUI.UIA3;
using System.Threading;

class Program
{
    static void Main()
    {
        using (var automation = new UIA3Automation())
        {
            var desktop = automation.GetDesktop();
            var calc = desktop.FindFirstChild(cf => cf.ByName("Calculator"));
            if (calc == null) 
            {
                Console.WriteLine("Calculator not found");
                return;
            }
            
            var elements = calc.FindAllDescendants();
            foreach (var element in elements)
            {
                if (!string.IsNullOrEmpty(element.Properties.AutomationId.ValueOrDefault))
                {
                    Console.WriteLine($"ID: {element.Properties.AutomationId.ValueOrDefault} - Name: {element.Properties.Name.ValueOrDefault}");
                }
            }
        }
    }
}

