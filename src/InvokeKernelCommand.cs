using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using System.Management.Automation;
using System.Threading.Tasks;

namespace DotNetInteractivePSCmdlet
{
    [Cmdlet(VerbsLifecycle.Invoke, "Kernel")]
    public class InvokeKernelCommand : Cmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, ValueFromRemainingArguments = true, Position = 0)]
        [Alias("Code", "Contents")]
        public string InputObject;

        [Parameter(ValueFromPipelineByPropertyName = true, Position = 1)]
        [Alias("Language")]
        public string Kernel = "pwsh";

        private readonly Kernel kernel;
        private readonly Repl repl;

        public InvokeKernelCommand()
        {
            repl = new Repl();
            kernel = repl.CreateKernel();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            var targetCmd = new SubmitCode(InputObject);
            repl.RunKernelCommand(targetCmd).RunSynchronously();
            // var result = Task.Run(() => );

            // var result = Task.Run(async () => await kernel.SendAsync(yyz)).Result;
            // var result = Task.Run(async () => await kernel.SendAsync(yyz));
            // var result = await kernel.SubmitCodeAsync("null");
            // var result = Task.Run(() => kernel.SubmitCodeAsync(Code));
            // var events = result.KernelEvents.ToSubscribedList();

            // var result = kernel.SendAsync(yyz);
            // var result = await kernel.SubmitCodeAsync("null");

            // WriteObject(yyz);
            //var result = Task.Run(() => kernel.SendAsync(yyz));

            // WriteObject(result);
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