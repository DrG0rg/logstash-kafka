using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using log4net.Core;
using log4net.kafka.Dto;
using log4net.Layout;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("log4net-kafka.Test")]

namespace log4net.kafka
{
    /// <inheritdoc />
    /// <summary>
    ///     Logstash layout for <see cref="KafkaAppender" />.
    /// </summary>
    public class KafkaLogstashLayout : LayoutSkeleton
    {
        private Dictionary<string, Func<IKafkaMessage, object>> _messageValueAccesors =
            new Dictionary<string, Func<IKafkaMessage, object>>();

        /// <summary>
        ///     Gets or sets the name of the application in which context log messages are written.
        /// </summary>
        /// <remarks>Can contain environment variables formatted like <c>${ENV_NAME}</c></remarks>
        public string Application { get; set; }

        /// <summary>
        ///     Gets or sets a string containing custom tags associated with each message.
        /// </summary>
        /// <remarks>Can contain environment variables formatted like <c>${ENV_NAME}</c></remarks>
        public string CustomTags { get; set; }

        /// <summary>
        ///     Gets or sets the list of properties from <see cref="LogicalThreadContext" /> to be used with each message
        /// </summary>
        public List<string> IncludeProperties { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="Type.FullName" /> of the type of messages to produce with the
        ///     <see cref="KafkaAppender" />.
        /// </summary>
        /// <remarks>Defaults to <see cref="KafkaMessageDto" />'s <see cref="Type.FullName" /></remarks>
        public string MessageType { get; set; } = "log4net.kafka.Dto.KafkaMessageDto";

        /// <summary>
        ///     Gets the inherent <see cref="KafkaLogstashLayout" /> having all environment variables already resolved.s
        /// </summary>
        public KafkaLayoutParameters ParsedParameters { get; private set; }

        /// <summary>
        ///     Indicates whether null values are sent to logstash. If set to <c>false</c> parameters having <c>null</c> set are
        ///     not transmitted.
        /// </summary>
        public bool SendNullValues { get; set; } = true;

        /// <inheritdoc />
        public override void ActivateOptions()
        {
            ParsedParameters = new KafkaLayoutParameters(this);

            _messageValueAccesors = ParsedParameters.MessageType.GetProperties()
                .Where(prop => prop.GetCustomAttribute<JsonIgnoreAttribute>(true) == null)
                .ToDictionary(prop => prop.GetCustomAttribute<JsonPropertyAttribute>(true)?.PropertyName ?? prop.Name,
                    prop => (Func<IKafkaMessage, object>) prop.GetValue);
        }

        /// <inheritdoc />
        public override void Format(TextWriter writer, LoggingEvent loggingEvent)
        {
            if (!(Activator.CreateInstance(ParsedParameters.MessageType) is IKafkaMessage messageObject))
                throw new InvalidCastException(
                    $"Failed to parse {nameof(Activator)} created instance of [{ParsedParameters.MessageType.FullName}] to [{typeof(IKafkaMessage).FullName}]");

            messageObject.Initialize(loggingEvent, ParsedParameters);
            Dictionary<string, object> msg = new Dictionary<string, object>();

            foreach (KeyValuePair<string, object> messageObjectAdditionalParameter in messageObject.AdditionalParameters
            )
                msg[messageObjectAdditionalParameter.Key] = messageObjectAdditionalParameter.Value;

            // write message properties second to override additional parameters on naming collision
            foreach (KeyValuePair<string, Func<IKafkaMessage, object>> messageValueAccesor in _messageValueAccesors)
                msg[messageValueAccesor.Key] = messageValueAccesor.Value.Invoke(messageObject);

            writer.Write(JsonConvert.SerializeObject(msg));
        }
    }
}