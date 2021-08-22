using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Connection;
using Microsoft.DotNet.Interactive.Events;
using Microsoft.DotNet.Interactive.Formatting;
using Microsoft.DotNet.Interactive.FSharp;
using Microsoft.DotNet.Interactive.PowerShell;
using System;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetInteractivePSCmdlet
{
    public class Repl : IDisposable
    {
        private CompositeKernel _kernel;
        private Kernel _pwsh;
        private Kernel _fsharp;

        public CompositeKernel CreateKernel()
        {
            _kernel = new CompositeKernel()
                //.UseAboutMagicCommand()
                .UseDebugDirective()
                //.UseHelpMagicCommand()
                //.UseQuitCommand()
                .UseKernelClientConnection(new ConnectNamedPipe());

            _pwsh = new PowerShellKernel()
                    .UseProfiles()
                    .UseDotNetVariableSharing();

            _fsharp = new FSharpKernel()
                    .UseDefaultFormatting()
                    .UseNugetDirective()
                    .UseKernelHelpers()
                    .UseWho();

            _kernel.Add(_pwsh, new[] { "powershell" });

            Formatter.SetPreferredMimeTypeFor(typeof(object), "text/plain");
            Formatter.Register<object>(o => o.ToString());

            return _kernel;
        }

        public async Task RunKernelCommand(KernelCommand command)
        {
            /*
            StringBuilder stdOut = default;
            StringBuilder stdErr = default;

            var events = _kernel.KernelEvents.Replay();
            using var _ = events.Connect();

            var t = events.FirstOrDefaultAsync(
                e => e is DisplayEvent or CommandFailed or CommandSucceeded);

            using var bs = events.Subscribe(@event =>
            {
                Console.WriteLine(@event.ToString());
                switch (@event)
                {
                    // events that tell us whether the submission was valid
                    case IncompleteCodeSubmissionReceived incomplete when incomplete.Command == command:
                        break;

                    case CompleteCodeSubmissionReceived complete when complete.Command == command:
                        break;

                    case CodeSubmissionReceived codeSubmissionReceived:
                        break;

                    // output / display events

                    case ErrorProduced errorProduced:
                        //ctx.UpdateTarget(GetErrorDisplay(errorProduced, Theme));

                        break;

                    case StandardOutputValueProduced standardOutputValueProduced:

                        stdOut ??= new StringBuilder();
                        stdOut.Append(standardOutputValueProduced.PlainTextValue());
                        Console.WriteLine(stdOut.ToString());

                        break;

                    case StandardErrorValueProduced standardErrorValueProduced:

                        stdErr ??= new StringBuilder();
                        stdErr.Append(standardErrorValueProduced.PlainTextValue());

                        break;

                    case DisplayedValueProduced displayedValueProduced:
                        //ctx.UpdateTarget(GetSuccessDisplay(displayedValueProduced, Theme));
                        //ctx.Refresh();
                        break;

                    case DisplayedValueUpdated displayedValueUpdated:
                        //ctx.UpdateTarget(GetSuccessDisplay(displayedValueUpdated, Theme));
                        break;

                    case ReturnValueProduced returnValueProduced:

                        if (returnValueProduced.Value is DisplayedValue)
                        {
                            break;
                        }

                        //ctx.UpdateTarget(GetSuccessDisplay(returnValueProduced, Theme));
                        break;

                    // command completion events

                    case CommandFailed failed when failed.Command == command:
                        //AnsiConsole.RenderBufferedStandardOutAndErr(Theme, stdOut, stdErr);
                        //ctx.UpdateTarget(GetErrorDisplay(failed.Message, Theme));
                        //tcs.SetResult();

                        break;

                    case CommandSucceeded succeeded when succeeded.Command == command:
                        //AnsiConsole.RenderBufferedStandardOutAndErr(Theme, stdOut, stdErr);
                        //tcs.SetResult();

                        break;
                }
            });
*/
            Console.WriteLine( "KERNEL: ");
            Console.WriteLine( _fsharp.Name );
            Console.WriteLine( "COMMAND: ");
            Console.WriteLine(command.ToString());
            var result = await _fsharp.SendAsync(command);
            Console.WriteLine( "RESULT: ");
            Console.WriteLine(result.Display().ToString());
        }

        public void Dispose()
        {

        }
    }

    // protected override void ProcessRecord()
    // {
    //     var errorRecords = Task.Run(async () => await ProcessRecordCore()).Result;

    //     foreach (var errorRecord in errorRecords)
    //     {
    //         WriteError(errorRecord);
    //     }
    // }

    // private async Task<BlockingCollection<ErrorRecord>> ProcessRecordCore()
    // {
    //     var tasks = CreateProcessTasks();

    //     var results = await Task.WhenAll(tasks.ToArray());

    //     var errorRecords = new BlockingCollection<ErrorRecord>();

    //     foreach (var result in results)
    //     {
    //         try
    //         {
    //             PostProcessTask(result);
    //         }
    //         catch (Exception e) when (e is PipelineStoppedException || e is PipelineClosedException)
    //         {
    //             // do nothing if pipeline stops
    //         }
    //         catch (Exception e)
    //         {
    //             errorRecords.Add(new ErrorRecord(e, e.GetType().Name, ErrorCategory.NotSpecified, this));
    //         }
    //     }

    //     return errorRecords;
    // }
}