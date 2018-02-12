using System.IO;
using System.Text;
using Confluent.Kafka;
using log4net.Appender;
using log4net.Core;

namespace log4net.kafka
{
    /// <inheritdoc />
    /// <summary>
    ///     log4net appender to append messages to a kafka topic
    /// </summary>
    public class KafkaAppender : AppenderSkeleton
    {
        private Producer _out;

        /// <summary>
        ///     Settings of the <see cref="KafkaAppender" />.
        /// </summary>
        public KafkaAppenderSettings KafkaAppenderSettings { get; set; }

        private void _StartProducer()
        {
            if (_out != null)
                return;

            if (KafkaAppenderSettings == null) throw new LogException($"{nameof(KafkaAppenderSettings)} are missing");
            KafkaAppenderSettings.Validate();

            _out = new Producer(KafkaAppenderSettings.Params);
        }


        /// <inheritdoc />
        public override void ActivateOptions()
        {
            base.ActivateOptions();
            _StartProducer();
        }


        /// <inheritdoc />
        protected override void Append(LoggingEvent loggingEvent)
        {
            _out.ProduceAsync(KafkaAppenderSettings.Topic, Encoding.UTF8.GetBytes(KafkaAppenderSettings.Topic),
                Encoding.UTF8.GetBytes(GenerateMessage(loggingEvent)));
        }

        /// <summary>
        ///     Generates the message to be written to kafka.
        /// </summary>
        /// <param name="loggingEvent">Event that is about to be logged</param>
        /// <returns>The message generated using the <paramref name="loggingEvent" /></returns>
        protected virtual string GenerateMessage(LoggingEvent loggingEvent)
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter sr = new StringWriter(sb))
            {
                Layout.Format(sr, loggingEvent);

                if (Layout.IgnoresException && loggingEvent.ExceptionObject != null)
                    sr.Write(loggingEvent.GetExceptionString());

                return sr.ToString();
            }
        }
    }
}