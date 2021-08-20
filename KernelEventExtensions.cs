using System.Linq;
using Microsoft.DotNet.Interactive.Events;

namespace DotNetInteractivePSCmdlet
{
    internal static class KernelEventExtensions
    {
        public static string PlainTextValue(this DisplayEvent @event)
        {
            return @event.FormattedValues.FirstOrDefault()?.Value ?? string.Empty;
        }
    }
}