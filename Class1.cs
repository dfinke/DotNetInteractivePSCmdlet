using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Connection;
using Microsoft.DotNet.Interactive.Events;
using Microsoft.DotNet.Interactive.PowerShell;
using System;
using System.IO;
using System.Management.Automation;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetInteractivePSCmdlet
{
    [Cmdlet(VerbsLifecycle.Invoke, "RunCode")]
    public class RunCodeCmdlet : Cmdlet
    {
        [Parameter(Position = 0)]
        public string Code;

        [Parameter(Position = 1)]
        public string TargetKernelName;

        private readonly CompositeKernel kernel;
        private readonly Repl repl;

        public RunCodeCmdlet()
        {
            repl = new Repl();
            kernel = repl.CreateKernel();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            var targetCmd = new SubmitCode(Code, TargetKernelName);
            var result = Task.Run(() => repl.RunKernelCommand(targetCmd));

            // var result = Task.Run(async () => await kernel.SendAsync(yyz)).Result;
            // var result = Task.Run(async () => await kernel.SendAsync(yyz));
            // var result = await kernel.SubmitCodeAsync("null");
            // var result = Task.Run(() => kernel.SubmitCodeAsync(Code));
            // var events = result.KernelEvents.ToSubscribedList();

            // var result = kernel.SendAsync(yyz);
            // var result = await kernel.SubmitCodeAsync("null");

            // WriteObject(yyz);
            //var result = Task.Run(() => kernel.SendAsync(yyz));

            WriteObject(result);
        }
    }

    [Cmdlet(VerbsLifecycle.Invoke, "TheNotebook")]
    public class InvokeNotebookCmdlet : Cmdlet
    {
        [Parameter(ValueFromPipelineByPropertyName = true, Position = 0)]
        public string FullName;

        private CompositeKernel kernel;

        public InvokeNotebookCmdlet()
        {
            var repl = new Repl();
            kernel = repl.CreateKernel();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            var content = File.ReadAllText(FullName);
            var rawData = Encoding.UTF8.GetBytes(content);
            var notebook = kernel.ParseNotebook(FullName, rawData);

            WriteObject(notebook);
        }
    }

    public class Repl : IDisposable
    {
        private CompositeKernel _kernel;

        public CompositeKernel CreateKernel()
        {
            _kernel = new CompositeKernel()
                //.UseAboutMagicCommand()
                .UseDebugDirective()
                //.UseHelpMagicCommand()
                //.UseQuitCommand()
                .UseKernelClientConnection(new ConnectNamedPipe());

            _kernel.Add(
                new PowerShellKernel()
                    .UseProfiles()
                    .UseDotNetVariableSharing(),
                new[] { "powershell" });

            return _kernel;
        }

        public async Task RunKernelCommand(KernelCommand command)
        {
            StringBuilder stdOut = default;
            StringBuilder stdErr = default;

            Task<KernelCommandResult> result = default;

            var events = _kernel.KernelEvents.Replay();
            using var _ = events.Connect();

            var t = events.FirstOrDefaultAsync(
                e => e is DisplayEvent or CommandFailed or CommandSucceeded);

            result = _kernel.SendAsync(command);

            await t;

            using var bs = events.Subscribe(@event =>
            {
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

            await result!;
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