using Prometheus;
using Prometheus.Advanced;
using DockerSamples.AspNetExporter.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Serilog;

namespace DockerSamples.AspNetExporter.Common.Collectors
{
    /// <summary>
    /// Collects metrics on configured Performance Counters
    /// </summary>
    public class ConfiguredPerformanceCounterCollector : IOnDemandCollector
    {
        private readonly CollectorConfiguration _config;
        private readonly ILogger _logger;
        private readonly bool _recordCollections;
        private readonly string _host = Environment.MachineName;

        readonly List<Tuple<Gauge, PerformanceCounter>> _collectors = new List<Tuple<Gauge, PerformanceCounter>>();
        private Counter _performanceCounter;

        public ConfiguredPerformanceCounterCollector(CollectorConfiguration config, bool recordCollections, ILogger logger)
        {
            _config = config;
            _logger = logger;
            _recordCollections = recordCollections;
        }

        public void RegisterMetrics()
        {
            if (_recordCollections)
            {
                _performanceCounter = Metrics.CreateCounter("performance_counter", "Performance counter", "host", "category", "counter", "instance", "status");
            }

            foreach (var collector in _config.PerformanceCounterCollectors)
            {                
                if (string.IsNullOrEmpty(collector.InstanceName))
                {
                    collector.InstanceName = string.Empty;
                }
                foreach (var counter in collector.CounterNames)
                {
                    try
                    {                        
                        var perfCounter = collector.InstanceName == string.Empty ?
                                            new PerformanceCounter(collector.CategoryName, counter, true) :
                                            new PerformanceCounter(collector.CategoryName, counter, collector.InstanceName, true);

                        // TODO - use .CounterType to determine the type of metric to create
                        var gauge = Metrics.CreateGauge(GetName(collector.CategoryName, counter), GetHelp(counter), "host", "instance");

                        _collectors.Add(Tuple.Create(gauge, perfCounter));
                        RecordCollection(collector.CategoryName, counter, collector.InstanceName, "setup-succeeded");
                    }
                    catch (Exception ex)
                    {
                        RecordCollection(collector.CategoryName, counter, collector.InstanceName, "setup-failed");
                        _logger.Error(ex, "Performance counter setup failed. Category: {Category}, counter: {Counter}, instance: {Instance}", collector.CategoryName, counter, collector.InstanceName);
                    }
                }
            }            
        }

        public void UpdateMetrics()
        {
            foreach (var collector in _collectors)
            {
                var gauge = collector.Item1;
                var counter = collector.Item2;
                try
                {
                    gauge.Labels(_host, counter.InstanceName).Set(counter.NextValue());
                    RecordCollection(counter.CategoryName, counter.CounterName, counter.InstanceName, "read-succeeded");
                }
                catch (Exception ex)
                {
                    RecordCollection(counter.CategoryName, counter.CounterName, counter.InstanceName, "read-failed");
                    _logger.Error(ex, "Performance counter read failed. Category: {Category}, counter: {Counter}, instance: {Instance}", counter.CategoryName, counter.CounterName, counter.InstanceName);
                }
            }
        }

        private void RecordCollection(string categoryName, string counterName, string instanceName, string status)
        {
            if (_performanceCounter != null)
            {
                _performanceCounter.Labels(_host, categoryName, counterName, instanceName, status).Inc();
            }
        }

        private string GetHelp(string name)
        {
            return name + " Perf Counter";
        }

        private string GetName(string category, string name)
        {
            return ToPromName(category) + "_" + ToPromName(name);
        }

        private string ToPromName(string name)
        {
            return name.Replace("%", "pct").Replace(" ", "_").Replace(".", "dot").Replace("/", "per").ToLowerInvariant();
        }
    }
}