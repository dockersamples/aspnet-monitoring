using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DockerSamples.AspNetExporter.App
{
    public class Config
    {
        private static Dictionary<string, string> _Values = new Dictionary<string, string>();

        // $env:METRICS_PORT="50504"
        public static int MetricsPort
        {
            get
            {
                if (!int.TryParse(Get("METRICS_PORT"), out int port))
                {
                    port = 50505;
                }
                return port;
            }
        }

        // $env:COLLECTOR_CONFIG_PATH="collectors.json"
        public static string CollectorConfigPath
        {
            get
            {
                var value = Get("COLLECTOR_CONFIG_PATH");
                if (string.IsNullOrEmpty(value))
                {
                    value = "aspnet-collectors.json";
                }
                return value;
            }
        }

        // $env:RECORD_COLLECTIONS="true"
        public static bool RecordCollections
        {
            get
            {
                if (!bool.TryParse(Get("RECORD_COLLECTIONS"), out bool record))
                {
                    record = false;
                }
                return record;
            }
        }

        private static string Get(string variable)
        {
            if (!_Values.ContainsKey(variable))
            {
                var value = Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.Machine);
                if (string.IsNullOrEmpty(value))
                {
                    value = Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.Process);
                }
                _Values[variable] = value;
            }
            return _Values[variable];
        }
    }
}
