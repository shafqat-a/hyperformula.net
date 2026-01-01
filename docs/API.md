# API Reference

## HyperFormulaCS.Calculation.Engine

The `Engine` class encapsulates the spreadsheet logic.

### Constructor

```csharp
public Engine();
```
Initializes the engine and registers standard function libraries.

### Methods

#### SetCell
```csharp
public void SetCell(CellAddress address, string formula)
public void SetCell(string address, string formula)
```
Sets the formula for a specific cell.
- **address**: The cell address (e.g., "A1", "C10").
- **formula**: The formula string starting with `=` (e.g., `=A2+10`) or a primitive value (`100`, `Hello`).

**Side Effects**: 
- Parses the formula.
- Updates dependency graph.
- Marks affected cells as dirty.

#### GetCellValue
```csharp
public CellValue GetCellValue(CellAddress address)
public CellValue GetCellValue(string address)
```
Retrieves the calculated value of a cell.
- **Returns**: A `CellValue` (NumberValue, StringValue, BooleanValue, or ErrorValue).
- If the cell is dirty, it triggers recalculation of that cell and its precedents if needed.

#### GetAllCells
```csharp
public Dictionary<string, string> GetAllCells()
```
Returns a dictionary of all defined cells and their current string representation values. Useful for data export or UI rendering.

### Models

#### CellValue
Base record for cell results.
- **NumberValue**: `record NumberValue(double Value)`
- **StringValue**: `record StringValue(string Value)`
- **BooleanValue**: `record BooleanValue(bool Value)`
- **ErrorValue**: `record ErrorValue(string Message)`
  - Common Errors: `#DIV/0!`, `#VALUE!`, `#REF!`, `#NAME?`, `#N/A`, `#NUM!`.

#### CellAddress
Struct representing coordinates.
- `Row` (0-indexed internally, usually displayed 1-based)
- `Column` (0-indexed internally)
- `ToString()`: Returns "A1" style.
- `Parse(string)`: Parses "A1" style.
