# Architecture Overview

HyperFormulaCS is a pure C# recalculation engine designed to be lightweight, extensible, and compatible with modern .NET 7.0+ environments. It mimics the core behavior of spreadsheet engines like Excel or HyperFormula (TS).

## Core Components

### 1. Engine (`Engine.cs`)
The `Engine` class is the main entry point. It maintains the state of the spreadsheet (cell data) and coordinates the parsing and calculation processes.

- **Responsibility**: managing cell storage, triggering parsing, and handling recalculation.
- **Key Methods**: `SetCell(address, formula)`, `GetCellValue(address)`.

### 2. Parsing Layer
The parsing pipeline converts formula strings into an Abstract Syntax Tree (AST).

- **Lexer (`Parsing/Lexer.cs`)**: Tokenizes the input string (e.g., `=SUM(A1, 10)` -> `IDENTIFIER`, `LPAREN`, `REF`, `COMMA`, `NUMBER`, `RPAREN`).
- **Parser (`Parsing/Parser.cs`)**: A recursive descent parser that consumes tokens and builds `AstNode` objects. It handles operator precedence (PEMDAS) and function calls.
- **AST (`Ast/AstNodes.cs`)**: Represents the formula structure.
  - `BinaryOpNode`: `+`, `-`, `*`, `/`, `>`, `=`, etc.
  - `FunctionCallNode`: `SUM(args)`, `IF(args)`.
  - `CellReferenceNode`: `A1`.
  - `RangeNode`: `A1:B2`.

### 3. Dependency Graph (`Calculation/DependencyGraph.cs`)
To ensure efficient recalculation, the engine tracks dependencies between cells.

- **Precedents**: Cells that a specific cell refers to (Example: A1 refers to B1).
- **Dependents**: Cells that refer to a specific cell (Example: B1 is referred to by A1).
- **Topological Sort**: When a cell changes, the graph determines the correct order to recalculate dependent cells to ensure consistency.

### 4. Evaluator (`Calculation/Evaluator.cs`)
The `Evaluator` traverses the AST to compute the final `CellValue`.

- It resolves cell references using current values from the `Engine`.
- It executes operations (`+`, `-`, etc.).
- It delegates function calls to the **Function Registry**.

### 5. Function Registry (`Calculation/FunctionRegistry.cs`)
A centralized registry that maps function names (e.g., "SUM", "VLOOKUP") to their C# implementations. This allows for modular expansion of the function library without modifying the core `Evaluator`.

---

## Data Flow

1. **User calls `SetCell("A1", "=B1+1")`**:
   - The formula is passed to the **Parser**, generating an AST.
   - The **DependencyGraph** is updated: A1 now depends on B1.
   - A1 is marked as "Dirty" (needs recalculation).
   - Any dependents of A1 are also marked Dirty.

2. **User calls `GetCellValue("A1")`**:
   - If A1 is Dirty, the **Evaluator** visits the AST.
   - It sees reference to B1.
   - It fetches B1's value (recalculating B1 if needed, though usually eager propagation or lazy checks handle this).
   - It computes the result and caches it.
   - Returns the value.

## Extensibility

New functions can be added by implementing a static method matching the `FunctionDelegate` signature and registering it via `FunctionRegistry.Register("NAME", Method)`.

See [Extending HyperFormulaCS](EXTENDING.md) for details.
