EZLog
=====

<b>EZLog</b> is a C# library that can be used to log any errors or information throughout an application that is coded in C#.

The mission of creating this simple to use logger is so that later on it can be fully customized according to the user that are using it in his/her application.

More information will be added later.

Code Example
=====
```csharp
try
{
    // Something went wrong in the try block.  
}
catch (Excepction ex)
{
    // The static Logger.Log method can be called and it will log what you want it to log.
    Logger.Log(LogLevel.Error, "Custom Message", ex);
}
```


License
=====
This project is licensed under the <a href='https://github.com/Darkreaper101/EZLog/blob/master/LICENSE'>MIT license</a>.
