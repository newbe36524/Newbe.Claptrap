using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newbe.ObjectVisitor;

namespace Newbe.Claptrap.StorageTestConsole.Services
{
    public class SavingEventResultReportFormat : IReportFormat<SavingEventResult>
    {
        private static readonly ICachedObjectVisitor<SavingEventResult, StringBuilder> Visitor =
            default(SavingEventResult)!
                .V()
                .WithExtendObject<SavingEventResult, StringBuilder>()
                .FilterProperty(p => p.GetCustomAttribute<ReportIgnoreAttribute>() == null)
                .ForEach((name, value, sb) => sb.AppendLine($"{name} : {value}"))
                .Cache();

        public Task<string> FormatAsync(SavingEventResult result)
        {
            var sb = new StringBuilder();
            Visitor.Run(result, sb);
            var re = sb.ToString();
            return Task.FromResult(re);
        }
    }
}