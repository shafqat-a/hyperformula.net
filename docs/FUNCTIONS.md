# Function Reference

This document lists all functions currently implemented in HyperFormulaCS, categorized by type.

## Math & Trigonometry

| Function | Usage | Description |
|----------|-------|-------------|
| **ABS** | `=ABS(number)` | Returns the absolute value of a number. |
| **ACOS** | `=ACOS(number)` | Returns the arccosine, or inverse cosine, of a number. |
| **COS** | `=COS(number)` | Returns the cosine of the given angle (in radians). |
| **PI** | `=PI()` | Returns the value of Pi (3.14159...). |
| **POWER** | `=POWER(number, power)` | Returns the result of a number raised to a power. |
| **ROUND** | `=ROUND(number, num_digits)` | Rounds a number to a specified number of digits. |
| **ROUNDUP** | `=ROUNDUP(number, num_digits)` | Rounds a number up, away from zero. |
| **ROUNDDOWN** | `=ROUNDDOWN(number, num_digits)` | Rounds a number down, toward zero. |
| **INT** | `=INT(number)` | Rounds a number down to the nearest integer. |
| **MOD** | `=MOD(number, divisor)` | Returns the remainder from division. |
| **SQRT** | `=SQRT(number)` | Returns a positive square root. |
| **SUM** | `=SUM(number1, [number2], ...)` | Adds all the numbers in a range of cells. |
| **RAND** | `=RAND()` | Returns a random number between 0 and 1. |
| **RANDBETWEEN** | `=RANDBETWEEN(bottom, top)` | Returns a random number between the numbers you specify. |

## Logical

| Function | Usage | Description |
|----------|-------|-------------|
| **AND** | `=AND(logical1, [logical2], ...)` | Returns TRUE if all of its arguments are TRUE. |
| **OR** | `=OR(logical1, [logical2], ...)` | Returns TRUE if any argument is TRUE. |
| **NOT** | `=NOT(logical)` | Reverses the logic of its argument. |
| **IF** | `=IF(logical_test, value_if_true, [value_if_false])` | Specifies a logical test to perform. |
| **TRUE** | `=TRUE()` or `TRUE` | Returns the logical value TRUE. |
| **FALSE** | `=FALSE()` or `FALSE` | Returns the logical value FALSE. |

## Information

| Function | Usage | Description |
|----------|-------|-------------|
| **ISNUMBER** | `=ISNUMBER(value)` | Returns TRUE if the value is a number. |
| **ISTEXT** | `=ISTEXT(value)` | Returns TRUE if the value is text. |
| **ISLOGICAL** | `=ISLOGICAL(value)` | Returns TRUE if the value is a logical value. |
| **ISERROR** | `=ISERROR(value)` | Returns TRUE if the value is any error value. |
| **ISBLANK** | `=ISBLANK(value)` | Returns TRUE if the value is blank. |

## Text

| Function | Usage | Description |
|----------|-------|-------------|
| **CONCATENATE** | `=CONCATENATE(text1, [text2], ...)` | Joins several text strings into one text string. |
| **LEN** | `=LEN(text)` | Returns the number of characters in a text string. |
| **LOWER** | `=LOWER(text)` | Converts text to lowercase. |
| **UPPER** | `=UPPER(text)` | Converts text to uppercase. |
| **LEFT** | `=LEFT(text, [num_chars])` | Returns the leftmost characters from a text value. |
| **RIGHT** | `=RIGHT(text, [num_chars])` | Returns the rightmost characters from a text value. |
| **MID** | `=MID(text, start_num, num_chars)` | Returns a specific number of characters from a text string starting at the position you specify. |
| **TRIM** | `=TRIM(text)` | Removes spaces from text. |
| **REPT** | `=REPT(text, number_times)` | Repeats text a given number of times. |
| **FIND** | `=FIND(find_text, within_text, [start_num])` | Finds one text value within another (case-sensitive). |
| **SEARCH** | `=SEARCH(find_text, within_text, [start_num])` | Finds one text value within another (case-insensitive). |
| **SUBSTITUTE** | `=SUBSTITUTE(text, old_text, new_text, [instance_num])` | Substitutes new_text for old_text in a text string. |

## Statistical

| Function | Usage | Description |
|----------|-------|-------------|
| **AVERAGE** | `=AVERAGE(number1, [number2], ...)` | Returns the average (arithmetic mean) of the arguments. |
| **COUNT** | `=COUNT(value1, [value2], ...)` | Counts how many numbers are in the list of arguments. |
| **COUNTA** | `=COUNTA(value1, [value2], ...)` | Counts how many values are in the list of arguments (non-empty). |
| **COUNTBLANK** | `=COUNTBLANK(range)` | Counts empty cells in a specified range of cells. |
| **MAX** | `=MAX(number1, [number2], ...)` | Returns the maximum value in a list of arguments. |
| **MIN** | `=MIN(number1, [number2], ...)` | Returns the minimum value in a list of arguments. |

## Date & Time

| Function | Usage | Description |
|----------|-------|-------------|
| **DATE** | `=DATE(year, month, day)` | Returns the serial number of a particular date. |
| **YEAR** | `=YEAR(serial_number)` | Converts a serial number to a year. |
| **MONTH** | `=MONTH(serial_number)` | Converts a serial number to a month. |
| **DAY** | `=DAY(serial_number)` | Converts a serial number to a day of the month. |
| **TODAY** | `=TODAY()` | Returns the serial number of today's date. |
| **NOW** | `=NOW()` | Returns the serial number of the current date and time. |
| **TIME** | `=TIME(hour, minute, second)` | Returns the serial number of a particular time. |
| **HOUR** | `=HOUR(serial_number)` | Converts a serial number to an hour. |
| **MINUTE** | `=MINUTE(serial_number)` | Converts a serial number to a minute. |
| **SECOND** | `=SECOND(serial_number)` | Converts a serial number to a second. |

## Lookup & Reference

| Function | Usage | Description |
|----------|-------|-------------|
| **VLOOKUP** | `=VLOOKUP(lookup_value, table_array, col_index_num, [range_lookup])` | Looks in the first column of an array and moves across the row to return the value of a cell. |
| **MATCH** | `=MATCH(lookup_value, lookup_array, [match_type])` | Looks up values in a reference or array. |
| **INDEX** | `=INDEX(array, row_num, [column_num])` | Returns a value or reference of the cell at the intersection of a particular row and column in a given range. |

## Financial

| Function | Usage | Description |
|----------|-------|-------------|
| **PMT** | `=PMT(rate, nper, pv, [fv], [type])` | Calculates the payment for a loan based on constant payments and a constant interest rate. |
| **FV** | `=FV(rate, nper, pmt, [pv], [type])` | Returns the future value of an investment based on periodic, constant payments and a constant interest rate. |
| **PV** | `=PV(rate, nper, pmt, [fv], [type])` | Returns the present value of an investment. |
| **NPV** | `=NPV(rate, value1, [value2], ...)` | Returns the net present value of an investment based on a discount rate and a series of future payments (negative values) and income (positive values). |
