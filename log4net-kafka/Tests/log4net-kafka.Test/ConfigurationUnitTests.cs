using System;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net.Config;
using log4net.kafka.Test.Models;
using log4net.Repository;
using Xunit;

namespace log4net.kafka.Test
{
    public class ConfigurationUnitTests
    {
        public ConfigurationUnitTests()
        {
            new LaunchSettingsFixture();
        }

        private static KafkaAppender _GetAppender(string configPath)
        {
            ILoggerRepository logRepo = LogManager.GetRepository(Assembly.GetEntryAssembly());

            using (FileStream fs = new FileStream(configPath, FileMode.Open))
            {
                XmlConfigurator.Configure(logRepo, fs);
            }

            return logRepo.GetAppenders().FirstOrDefault() as KafkaAppender;
        }

        [Fact]
        public void TestLoadConfigAllParameters()
        {
            KafkaAppender appender = _GetAppender("Configurations/Config1.xml");

            Assert.Equal(new[] {"PLAINTEXT://kafka:9092"}, appender.KafkaAppenderSettings.BootstrapServers);
            Assert.Equal(0, appender.KafkaAppenderSettings.BufferingMaxMs);
            Assert.Equal("tstClient", appender.KafkaAppenderSettings.ClientId);
            Assert.Equal(10000, appender.KafkaAppenderSettings.MaxBlockingMs);
            Assert.Equal(10, appender.KafkaAppenderSettings.Retries);
            Assert.Equal("myTopic", appender.KafkaAppenderSettings.Topic);

            Assert.True(appender.Layout is KafkaLogstashLayout);
            KafkaLogstashLayout layout = appender.Layout as KafkaLogstashLayout;

            Assert.Equal("xUnit", layout.Application);
            Assert.Equal("ENV=TEST", layout.CustomTags);
            Assert.Equal(new[] {"example1", "example2", "example3"}, layout.IncludeProperties);
            Assert.Equal($"{typeof(CustomKafkaMessage).FullName},{Assembly.GetExecutingAssembly().GetName().Name}",
                layout.MessageType);
        }

        [Fact]
        public void TestLoadConfigAllParametersEnvironmentReplacement()
        {
            KafkaAppender appender = _GetAppender("Configurations/ConfigEnvVariables.xml");

            Assert.Equal(new[] {new Uri("PLAIN://TSTKAFKA:4711", UriKind.Absolute)},
                appender.KafkaAppenderSettings.ParsedBootStrapServers);
            Assert.Equal(0, appender.KafkaAppenderSettings.BufferingMaxMs);
            Assert.Equal("envclient", appender.KafkaAppenderSettings.ClientId);
            Assert.Equal(10000, appender.KafkaAppenderSettings.MaxBlockingMs);
            Assert.Equal(10, appender.KafkaAppenderSettings.Retries);
            Assert.Equal("logstashEnv", appender.KafkaAppenderSettings.Topic);

            Assert.True(appender.Layout is KafkaLogstashLayout);
            KafkaLogstashLayout layout = appender.Layout as KafkaLogstashLayout;

            Assert.Equal("xUnitEnv", layout.ParsedParameters.Application);
            Assert.Equal("ENV=xUnitTest", layout.ParsedParameters.CustomTags);
            Assert.Equal(new[] {"example1", "example2", "example3"}, layout.ParsedParameters.IncludeProperties);
            Assert.Equal(typeof(CustomKafkaMessage), layout.ParsedParameters.MessageType);
        }
    }
}