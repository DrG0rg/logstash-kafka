using System;
using System.Collections.Generic;
using System.Linq;
using log4net.Core;
using log4net.Util;
using Newtonsoft.Json;

namespace log4net.kafka.Dto
{
    /// <inheritdoc />
    public class KafkaMessageDto : IKafkaMessage
    {
        /// <inheritdoc />
        [JsonProperty(PropertyName = "addIns")]
        [JsonIgnore]
        public Dictionary<string, object> AdditionalParameters { get; set; } = new Dictionary<string, object>();

        /// <inheritdoc />
        [JsonProperty(PropertyName = "application")]
        public string Application { get; set; }

        /// <inheritdoc />
        [JsonProperty(propertyName: "customTags")]
        public string CustomTags { get; set; }

        /// <inheritdoc />
        [JsonProperty(PropertyName = "exception")]
        public KafkaMessageExceptionDto Exception { get; set; }

        /// <inheritdoc />
        [JsonProperty(PropertyName = "host")]
        public string Host { get; set; } = Environment.MachineName;

        /// <inheritdoc />
        [JsonProperty(PropertyName = "level")]
        public string Level { get; set; }

        /// <inheritdoc />
        [JsonProperty(PropertyName = "logger_name")]
        public string LoggerName { get; set; }

        /// <inheritdoc />
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        /// <inheritdoc />
        [JsonProperty(PropertyName = "ow")]
        public string OperatingSystem { get; set; } = Environment.OSVersion.VersionString;

        /// <inheritdoc />
        [JsonProperty(PropertyName = "timestamp")]
        public string Timestamp { get; set; }

        /// <inheritdoc />
        [JsonProperty(PropertyName = "@version")]
        public int Version { get; set; } = 1;

        /// <inheritdoc />
        public virtual void Initialize(LoggingEvent fromEvent, KafkaLayoutParameters parameters)
        {
            if (fromEvent == null) throw new ArgumentNullException(nameof(fromEvent));

            Application = parameters?.Application;
            CustomTags = parameters?.CustomTags;
            Exception = fromEvent.ExceptionObject != null
                ? new KafkaMessageExceptionDto(fromEvent.ExceptionObject)
                : null;
            Level = fromEvent.Level.Name;
            LoggerName = fromEvent.Repository.Name;
            Message = fromEvent.RenderedMessage;
            Timestamp = fromEvent.TimeStampUtc.ToString("s");

            foreach (string key in fromEvent.Properties.GetKeys().Distinct())
                AdditionalParameters[key] = fromEvent.Properties[key];

            foreach (string propName in parameters?.IncludeProperties ?? new List<string>())
            {
                object value = GlobalContext.Properties[propName] ?? LogicalThreadContext.Properties[propName];

                if (value is LogicalThreadContextStack ltc && ltc.Count > 0)
                    value = ltc.ToString();

                AdditionalParameters[propName] = value;
            }
        }
    }
}