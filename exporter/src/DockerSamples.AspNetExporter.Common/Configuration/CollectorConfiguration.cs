namespace DockerSamples.AspNetExporter.Common.Configuration
{
    public class CollectorConfiguration
    {
        public PerformanceCounterCollector[] PerformanceCounterCollectors { get; set; }

        public class PerformanceCounterCollector
        {
            public string CategoryName { get; set; }

            public string InstanceName { get; set; }

            public string[] CounterNames { get; set; }
        }
    }
}