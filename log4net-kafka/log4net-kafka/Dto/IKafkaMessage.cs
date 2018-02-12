using System;
using System.Collections.Generic;
using log4net.Core;
using Newtonsoft.Json;

namespace log4net.kafka.Dto
{
    /// <summary>
    ///     Interface for messages being sent to kafka by log4net
    /// </summary>
    public interface IKafkaMessage
    {
        /// <summary>
        ///     Gets or sets a list of additional parameters
        /// </summary>
        [JsonProperty(PropertyName = "addIns")]
        [JsonIgnore]
        Dictionary<string, object> AdditionalParameters { get; set; }

        /// <summary>
        ///     Gets or sets the name of the application generating the message
        /// </summary>
        [JsonProperty(PropertyName = "application")]
        string Application { get; set; }

        /// <summary>
        /// Set of tags set in the configuration
        /// </summary>
        [JsonProperty(PropertyName = "customTags")]
        string CustomTags { get; set; }

        /// <summary>
        ///     Gets or sets the exception included in the message
        /// </summary>
        [JsonProperty(PropertyName = "exception")]
        KafkaMessageExceptionDto Exception { get; set; }

        /// <summary>
        ///     Gets or sets the name of the host on which the message was generated
        /// </summary>
        [JsonProperty(PropertyName = "host")]
        string Host { get; set; }

        /// <summary>
        ///     Gets or sets the log level of the message
        /// </summary>
        [JsonProperty(PropertyName = "level")]
        string Level { get; set; }

        /// <summary>
        ///     Gets or sets the name of the logger who generated this message
        /// </summary>
        [JsonProperty(PropertyName = "logger_name")]
        string LoggerName { get; set; }

        /// <summary>
        ///     Gets or sets the actual message content
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        string Message { get; set; }

        /// <summary>
        ///     Gets or sets the operating system of the host this messages was created on
        /// </summary>
        [JsonProperty(PropertyName = "os")]
        string OperatingSystem { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DateTime" /> when the message was generated (format <c>DateTime.ToString("r")</c> |
        ///     utc)
        /// </summary>
        [JsonProperty(PropertyName = "timestamp")]
        string Timestamp { get; set; }

        /// <summary>
        ///     Gets or sets the version of the message
        /// </summary>
        [JsonProperty(PropertyName = "@version")]
        int Version { get; set; }

        /// <summary>
        ///     Method to initialize an <see cref="IKafkaMessage" /> from log4nets <see cref="LoggingEvent" /> and
        ///     <see cref="KafkaLayoutParameters" />.
        /// </summary>
        /// <param name="fromEvent">Log event used to initialize the <see cref="IKafkaMessage" /> instance</param>
        /// <param name="parameters">Environment parameters used to initialize the <see cref="IKafkaMessage" /> instance</param>
        void Initialize(LoggingEvent fromEvent, KafkaLayoutParameters parameters);
    }
}