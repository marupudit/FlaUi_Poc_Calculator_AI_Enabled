# Example Project using FlaUI

A demo/skeleton project using [FlaUI](https://github.com/FlaUI/FlaUI) to automate Windows Calculator.

See project docs and `AutomationContext.md` for more info on how to use and the coding standards applied in this project.

# Prerequisites

[.NET](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) needs to be installed to be able to build and run the tests.

# Installation

All project dependencies will be restored by running
```
dotnet restore
```

# Project Structure
```
project
|-- flaui_poc
|   -- Base
|       -- BaseTest.cs
|   -- Pages        <!--- Page Object Layer for UI interactions ---> 
|       -- BasePage.cs
|       -- CalculatorPage.cs
|   -- Tests         <!--- Test Layer containing orchestrations --->
|       -- CalculatorTests.cs
|   -- flaui_poc.csproj
|-- README.md
|-- AutomationContext.md
|-- flaui_poc.sln
```

# Running tests

To run the calculator automation tests:

```
dotnet test
```