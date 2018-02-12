# Introduction
This package provide quick and flexibal access to log4net with kafka and logstash

# Getting Started
TODO: Guide users through getting your code up and running on their own system. In this section you can talk about:
1.	Installation process: download from nuget
2.	Software dependencies: ConfluentKafka, log4net, Newtonsoft.Json

#Example Configuration
```xml
<?xml version="1.0" encoding="utf-8" ?>
<log4net>
  <appender name="KafkaAppender" type="log4net.kafka.KafkaAppender, log4net-kafka">
    <KafkaAppenderSettings>
      <BootstrapServers>
        <add value="PLAINTEXT://kafka:9092" />
      </BootstrapServers>
      <Topic value="logstash" />
    </KafkaAppenderSettings>
    <layout type="log4net.kafka.KafkaLogstashLayout,log4net-kafka" >
      <CustomTags value="ENV=${ASPNETCORE_ENVIRONMENT}" /> <!-- here you can insert references to environment variables -->
      <IncludeProperties>
        <!-- IncludeProperties contain the name of the log4net config variables you declared in code like: -->
        <!-- GlobalContext.Properties["example1"] = "foo"               -->
        <!-- LogicalThreadContext.Properties["example2] = "bar"         -->
        <!-- LogicalThreadContext.Stacks["example3"].Push("foo-bar")    -->
        <add value="example1" />
        <add value="example2" />
        <add value="example3" />
      </IncludeProperties>
    </layout>
    <root>
        <level value="DEBUG"/>
        <appender-ref ref="KafkaAppender" />
    </root>
</log4net>
```

# Build and Test
Coming soon

# Contribute
Coming soon
