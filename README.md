Kaleidoscope.NET
================

Implementation of the [Kaleidoscope language](http://llvm.org/docs/tutorial/index.html) in C# with CIL/CLR as target.

###Changes
External functions must have a .NET function reference.
<br>
Example: _extern print(x) :: System.Console.Write_

###Not implemented features
* Mutable variables
* Generate .NET assembly from code
