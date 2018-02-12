using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using log4net.Core;
using Newtonsoft.Json;

namespace log4net.kafka
{
    /// <summary>
    ///     Settings of the <see cref="KafkaAppender" />
    /// </summary>
    public class KafkaAppenderSettings
    {
        /// <summary>
        ///     Confluent kafka setting for initial server connection
        ///     (https://docs.confluent.io/current/kafka-rest/docs/config.html)
        /// </summary>
        public List<string> BootstrapServers { get; set; }

        /// <summary>
        ///     Confluent kafka setting for max buffering time (https://docs.confluent.io/current/kafka-rest/docs/config.html)
        /// </summary>
        public int BufferingMaxMs { get; set; } = 100;

        /// <summary>
        ///     Confluent kafka setting for id of this client instance
        ///     (https://docs.confluent.io/current/kafka-rest/docs/config.html)
        /// </summary>
        public string ClientId { get; set; } = string.Empty;

        /// <summary>
        ///     Confluent kafka setting for max blocking time when sending messages
        ///     (https://docs.confluent.io/current/kafka-rest/docs/config.html)
        /// </summary>
        public int MaxBlockingMs { get; set; } = 10;

        /// <summary>
        ///     Confluent prepared dictionary of parameters.
        /// </summary>
        internal Dictionary<string, object> Params { get; } = new Dictionary<string, object>();

        /// <summary>
        ///     Parsed an validated list of <see cref="BootstrapServers" />.
        /// </summary>
        internal List<Uri> ParsedBootStrapServers { get; } = new List<Uri>();

        /// <summary>
        ///     Confluent kafka setting for number of retries when sending a message
        ///     (https://docs.confluent.io/current/kafka-rest/docs/config.html)
        /// </summary>
        public int Retries { get; set; } = 3;

        /// <summary>
        ///     Confluent kafka setting for the topic name a message is sent to
        ///     (https://docs.confluent.io/current/kafka-rest/docs/config.html)
        /// </summary>
        public string Topic { get; set; } = "logstash";

        /// <summary>
        ///     Validates the input, resolves all string inputs through the <see cref="EnvironmentResolver" /> and fills the
        ///     <see cref="Params" /> and <see cref="ParsedBootStrapServers" /> collections.
        /// </summary>
        public void Validate()
        {
            if (BootstrapServers == null || !BootstrapServers.Any())
                throw new LogException($"No {nameof(BootstrapServers)} configured");

            foreach (string bootstrapServer in BootstrapServers.Select(EnvironmentResolver.Resolve))
            {
                if (string.IsNullOrEmpty(bootstrapServer) ||
                    !Uri.TryCreate(bootstrapServer, UriKind.Absolute, out Uri serverUri))
                    throw new LogException($"Invalid {nameof(BootstrapServers)} element: \"{bootstrapServer}\"");
                ParsedBootStrapServers.Add(serverUri);
            }

            if (string.IsNullOrEmpty(Topic))
            {
                Console.WriteLine(
                    $"[{nameof(KafkaAppenderSettings)}] Invalid topic [{Topic}] resetting to default topic [logstash]");
                Topic = "logstash";
            }

            Topic = EnvironmentResolver.Resolve(Topic);
            ClientId = EnvironmentResolver.Resolve(ClientId);

            Params["bootstrap.servers"] = string.Join(",", ParsedBootStrapServers.Select(x => x.ToString()));
            Params["retries"] = Retries;
            Params["client.id"] = string.Join(",", ParsedBootStrapServers.Select(x => x.ToString()));
            Params["socket.blocking.max.ms"] = MaxBlockingMs;
            Params["queue.buffering.max.ms"] = BufferingMaxMs;

            string preStartMsg =
                $"Starting {GetType().FullName} using config: {JsonConvert.SerializeObject(this)}";
            Console.WriteLine(preStartMsg);
            Trace.TraceInformation(preStartMsg);
        }
    }
}