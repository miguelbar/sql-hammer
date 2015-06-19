# sql-hammer
Runs a SQL statement against an ODBC data source

## Parameters

- Number of threads
- Number of iterations
- ODBC data source

## Example

c:\>Hammer.exe 2 5 "DSN=My data source"

The above command will execute the SQL specified in the code using 2 concurrent threads for 5 iterations against the ODBC data source called "My data source"
