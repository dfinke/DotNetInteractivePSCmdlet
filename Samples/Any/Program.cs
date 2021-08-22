using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.CSharp;
using Microsoft.DotNet.Interactive.Events;
using Microsoft.DotNet.Interactive.Formatting;
using Microsoft.DotNet.Interactive.FSharp;
using Microsoft.DotNet.Interactive.PowerShell;
using System;
using System.Linq;

var pwsh = new PowerShellKernel()
                    .UseProfiles()
                    .UseDotNetVariableSharing();

var csharp = new CSharpKernel()
                    .UseNugetDirective()
                    .UseKernelHelpers()
                    .UseWho();

var fsharp = new FSharpKernel()
                    .UseDefaultFormatting()
                    .UseNugetDirective()
                    .UseKernelHelpers()
                    .UseWho();

var kernel = new CompositeKernel {
    pwsh,
    csharp,
    fsharp,
};
kernel.DefaultKernelName = pwsh.Name;

Formatter.SetPreferredMimeTypeFor(typeof(object), "text/plain");
Formatter.Register<object>(o => o.ToString());

while (true)
{
    if (ReadLine() is not { } request)
        continue;
    var toSubmit = new SubmitCode(request);
    var response = await kernel.SendAsync(toSubmit);
    response.KernelEvents.Subscribe(
        e =>
        {
            switch (e)
            {
                case CommandFailed failed:
                    WriteLineError(failed.Message);
                    break;
                case DisplayEvent display:
                    WriteLine(display.FormattedValues.First().Value);
                    break;
            }
        });
}


static string? ReadLine()
{
    Console.Write("\nInput: ");
    return Console.ReadLine();
}

static void WriteLine(string input)
{
    Console.ForegroundColor = ConsoleColor.DarkGreen;
    Console.WriteLine($"\nOutput: {input}");
    Console.ForegroundColor = ConsoleColor.Gray;
}

static void WriteLineError(string input)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\nError: {input}");
    Console.ForegroundColor = ConsoleColor.Gray;
}