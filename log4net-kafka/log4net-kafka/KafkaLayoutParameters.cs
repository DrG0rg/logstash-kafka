using System;
using System.Collections.Generic;
using log4net.kafka.Dto;

namespace log4net.kafka
{
    /// <summary>
    ///     Parameters created from <see cref="KafkaLogstashLayout" />
    /// </summary>
    public class KafkaLayoutParameters
    {
        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="layout">LAyout to create parameters from</param>
        internal KafkaLayoutParameters(KafkaLogstashLayout layout)
        {
            Application = EnvironmentResolver.Resolve(layout.Application);
            CustomTags = EnvironmentResolver.Resolve(layout.CustomTags);
            IncludeProperties = layout.IncludeProperties;
            MessageType = Type.GetType(EnvironmentResolver.Resolve(layout.MessageType));

            if (MessageType == null)
                throw new ArgumentException($"Cannot create any type from {layout.MessageType}");
            if (!typeof(IKafkaMessage).IsAssignableFrom(MessageType))
                throw new ArgumentException(
                    $"Type [{MessageType.FullName}] does not inherit from [{typeof(IKafkaMessage).FullName}]");
        }

        /// <summary>
        ///     Name of the application in log messages
        /// </summary>
        public string Application { get; set; }

        /// <summary>
        ///     Custom tags send in log messages
        /// </summary>
        public string CustomTags { get; set; }

        /// <summary>
        ///     log4net property names to retrieve custom data from <see cref="LogicalThreadContext.Properties" /> or
        ///     <see cref="LogicalThreadContext.Stacks" />
        /// </summary>
        public List<string> IncludeProperties { get; set; }

        /// <summary>
        ///     Type of messages to send to logstash
        /// </summary>
        public Type MessageType { get; set; }
    }
}