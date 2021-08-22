using Microsoft.DotNet.Interactive;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace DotNetInteractivePSCmdlet
{
    [Cmdlet(VerbsCommon.Get, "Notebook")]
    public class GetNotebookCommand : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [Alias("PSPath")]
        public string Path;

        private CompositeKernel kernel;

        public GetNotebookCommand()
        {
            var repl = new Repl();
            kernel = repl.CreateKernel();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            ProviderInfo provider;
            foreach (var path in GetResolvedProviderPathFromPSPath(Path, out provider))
            {
                string content;
                var bytes = new List<byte>();
                using var reader = InvokeProvider.Content.GetReader(path).Single();
                while ((content = reader.Read(1).Cast<string>().SingleOrDefault()) != null)
                {
                    bytes.AddRange(Encoding.UTF8.GetBytes(content));
                    bytes.Add(10); // the content providers usually split on newlines
                }

                var notebook = kernel.ParseNotebook(path, bytes.ToArray());
                WriteObject(notebook);
            }
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