# HyperFormulaCS

**HyperFormulaCS** is a high-performance, pure C# spreadsheet calculation engine inspired by [HyperFormula](https://github.com/handsontable/hyperformula). It allows you to parse, evaluate, and manage complex spreadsheet formulas within your .NET applications without relying on external spreadsheet software.

![Build Status](https://img.shields.io/badge/build-passing-brightgreen)
![Example UI](docs/ui-preview.png)

## Features

- **Robust Parsing**: Recursive descent parser supporting complex arithmetic, operator precedence, and nested function calls.
- **Dependency Tracking**: Automatic dependency graph management ensures efficient recalculation (only affected cells are updated).
- **Extensive Function Library**:
    - **Math**: `SUM`, `ABS`, `POWER`, `ROUND`, `SQRT`, `PI`, `COS`, `ACOS`.
    - **Logical**: `IF`, `AND`, `OR`, `NOT`, `TRUE`, `FALSE`.
    - **Text**: `CONCATENATE`, `LEN`, `LEFT`, `RIGHT`, `MID`, `TRIM`, `LOWER`, `UPPER`, `REPT`.
    - **Statistical**: `AVERAGE`, `COUNT`, `COUNTA`, `COUNTBLANK`, `MAX`, `MIN`.
    - **Date & Time**: `DATE`, `TIME`, `NOW`, `TODAY`, `YEAR`, `MONTH`, `DAY`, `HOUR`, `MINUTE`, `SECOND`.
    - **Lookup**: `VLOOKUP`, `MATCH`, `INDEX`.
    - **Financial**: `PMT`, `PV`, `FV`, `NPV`.
- **Cross-Platform**: Targets .NET 7.0+, running on Windows, Linux, and macOS.
- **Web Demo**: Includes an ASP.NET Core Web API + Frontend demo.

## Getting Started

### Installation

Clone the repository and build the solution:

```bash
git clone https://github.com/yourusername/HyperFormulaCS.git
cd HyperFormulaCS
dotnet build
```

### Basic Usage

```csharp
using HyperFormulaCS.Calculation;

var engine = new Engine();

// Set values and formulas
engine.SetCell("A1", "10");
engine.SetCell("B1", "20");
engine.SetCell("C1", "=SUM(A1:B1) + 5");

// Get results
var result = engine.GetCellValue("C1");
Console.WriteLine(result); // Outputs: 35 (10+20+5)

// Dependencies update automatically
engine.SetCell("A1", "100");
var newResult = engine.GetCellValue("C1");
Console.WriteLine(newResult); // Outputs: 125
```

## Running the Web Demo

The project comes with a built-in web-based spreadsheet UI.

1. Navigate to the web project: `cd HyperFormulaCS.Web`
2. Run the server: `dotnet run`
3. Open `http://localhost:5xxx` in your browser.

## Documentation

- [Architecture Overview](docs/ARCHITECTURE.md)
- [API Reference](docs/API.md)
- [Function Reference](docs/FUNCTIONS.md)
- [Extending / Adding Functions](docs/EXTENDING.md)

## Contributing

Contributions are welcome! Please run unit tests before submitting PRs.

```bash
dotnet test
```
