using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Filters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Mathematics;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;

namespace Newbe.Claptrap.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new Config();
            // BenchmarkRunner.Run<MethodInvokeBenchmark>(config);
            BenchmarkRunner.Run<SleepBenchmark>(config);
            // BenchmarkRunner.Run<ClaptrapBenchmark>(config);
        }
    }

    public class Config : IConfig
    {
        private class RankColumnProvider : IColumnProvider
        {
            public static IColumnProvider Default { get; } = new RankColumnProvider();

            public IEnumerable<IColumn> GetColumns(Summary summary)
            {
                yield return new RankColumn(NumeralSystem.Arabic);
            }
        }

        public IEnumerable<IColumnProvider> GetColumnProviders()
        {
            foreach (var columnProvider in DefaultColumnProviders.Instance)
            {
                yield return columnProvider;
            }

            yield return RankColumnProvider.Default;
        }

        public IEnumerable<IExporter> GetExporters()
        {
            yield return MarkdownExporter.GitHub;
        }

        public IEnumerable<ILogger> GetLoggers()
        {
            if (LinqPadLogger.IsAvailable)
                yield return LinqPadLogger.Instance;
            else
                yield return ConsoleLogger.Default;
        }

        public IEnumerable<IAnalyser> GetAnalysers()
        {
            yield return EnvironmentAnalyser.Default;
            yield return OutliersAnalyser.Default;
            yield return MinIterationTimeAnalyser.Default;
            yield return MultimodalDistributionAnalyzer.Default;
            yield return RuntimeErrorAnalyser.Default;
            yield return ZeroMeasurementAnalyser.Default;
        }

        public IEnumerable<IValidator> GetValidators()
        {
            yield return BaselineValidator.FailOnError;
            yield return SetupCleanupValidator.FailOnError;
            yield return JitOptimizationsValidator.FailOnError;
            yield return RunModeValidator.FailOnError;
            yield return GenericBenchmarksValidator.DontFailOnError;
            yield return DeferredExecutionValidator.FailOnError;
        }

        public IOrderer Orderer
        {
            get { return null; }
        }

        public ConfigUnionRule UnionRule => ConfigUnionRule.Union;

        public Encoding Encoding => Encoding.ASCII;

        public CultureInfo CultureInfo => CultureInfo.CurrentCulture;
        public ConfigOptions Options => ConfigOptions.Default;

        public SummaryStyle SummaryStyle => SummaryStyle.Default;

        public string ArtifactsPath =>
            Path.Combine(Directory.GetCurrentDirectory(), "BenchmarkDotNet.Artifacts");

        public IEnumerable<Job> GetJobs()
        {
            yield return Job.Default.WithRuntime(CoreRuntime.Core31);
        }

        public IEnumerable<BenchmarkLogicalGroupRule> GetLogicalGroupRules()
        {
            return Array.Empty<BenchmarkLogicalGroupRule>();
        }

        public IEnumerable<IDiagnoser> GetDiagnosers()
        {
            yield return MemoryDiagnoser.Default;
        }

        public IEnumerable<HardwareCounter> GetHardwareCounters()
        {
            return Array.Empty<HardwareCounter>();
        }

        public IEnumerable<IFilter> GetFilters()
        {
            return Array.Empty<IFilter>();
        }
    }
}