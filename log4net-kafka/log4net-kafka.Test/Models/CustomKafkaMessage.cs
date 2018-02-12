using System.Collections.Generic;
using log4net.Core;
using log4net.kafka.Dto;

namespace log4net.kafka.Test.Models
{
    public class CustomKafkaMessage : IKafkaMessage
    {
        public Dictionary<string, object> AdditionalParameters { get; set; }
        public string Application { get; set; }
        public string CustomTags { get; set; }
        public KafkaMessageExceptionDto Exception { get; set; }
        public string Host { get; set; }
        public string Level { get; set; }
        public string LoggerName { get; set; }
        public string Message { get; set; }
        public string OperatingSystem { get; set; }
        public string Timestamp { get; set; }
        public int Version { get; set; }

        public void Initialize(LoggingEvent fromEvent, KafkaLayoutParameters parameters)
        {
        }
    }
}