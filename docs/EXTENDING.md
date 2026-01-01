# Extending HyperFormulaCS

HyperFormulaCS is designed to be easily extensible. The most common extension scenario is adding new implementation for spreadsheet functions.

## Adding a New Function

Functions are registered in the `FunctionRegistry` and implemented as static methods matching the `FunctionDelegate` signature.

### 1. Define the Function Implementation

A function takes a list of `AstNode` arguments and a `FunctionContext` (for evaluating args or resolving references), and returns a `CellValue`.

```csharp
using HyperFormulaCS.Ast;
using HyperFormulaCS.Models;
using HyperFormulaCS.Calculation;
using System.Collections.Generic;

public static class MyCustomFunctions
{
    public static CellValue MyMultiply(List<AstNode> args, FunctionContext ctx)
    {
        // 1. Validate argument count
        if (args.Count != 2) return ErrorValue.Value; // #VALUE!

        // 2. Evaluate arguments (they come as AST nodes, need to be computed)
        // Use helper GetNumericArg to safely simpler numeric functions
        try
        {
            double a = FunctionRegistry.GetNumericArg(args[0], ctx);
            double b = FunctionRegistry.GetNumericArg(args[1], ctx);
            
            // 3. Compute logic
            return new NumberValue(a * b);
        }
        catch
        {
            return ErrorValue.Value;
        }
    }
}
```

### 2. Register the Function

You must register the function before using it in the engine. This is typically done at application startup or in a static constructor.

```csharp
FunctionRegistry.Register("MYMULTIPLY", MyCustomFunctions.MyMultiply);
```

### 3. Usage

Now you can use the function in formulas:

```csharp
engine.SetCell("A1", "=MYMULTIPLY(10, 2)");
// Result: 20
```

## Function Context

The `FunctionContext` provides access to key engine services:
- `Evaluate(AstNode node)`: Recursively evaluates a node (useful for arguments).
- `Resolve(CellAddress address)`: Resolves a cell reference.

## Handling Ranges

If your function accepts ranges (e.g., `SUM(A1:A5)`), you need to handle `RangeNode`.

```csharp
private static CellValue CountNodes(List<AstNode> args, FunctionContext ctx)
{
    int count = 0;
    foreach(var arg in args)
    {
        if (arg is RangeNode r)
        {
            // Iterate range logic
            for(int row = r.Start.Row; row <= r.End.Row; row++)
            {
                 // ... check cells
                 count++;
            }
        }
        else
        {
            count++;
        }
    }
    return new NumberValue(count);
}
```
