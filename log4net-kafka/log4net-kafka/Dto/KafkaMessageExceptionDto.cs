﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace log4net.kafka.Dto
{
    /// <summary>
    ///     Exception object of a <see cref="IKafkaMessage" />
    /// </summary>
    public class KafkaMessageExceptionDto
    {
        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="fromException">.net exception causing this exception</param>
        public KafkaMessageExceptionDto(Exception fromException)
        {
            if (fromException == null)
                throw new ArgumentNullException(nameof(fromException));

            Message = fromException.Message;
            ExceptionType = fromException.GetType().FullName;
            Stacktrace = fromException.StackTrace;

            if(fromException is AggregateException aggException)
            {
                InnerExceptions.AddRange(aggException.InnerExceptions.Select(x => new KafkaMessageExceptionDto(x)));
            }
            else if(fromException.InnerException != null)
            {
                InnerExceptions.Add(new KafkaMessageExceptionDto(fromException.InnerException));
            }
        }

        public KafkaMessageExceptionDto()
        {

        }

        /// <summary>
        ///     Type of the exception
        /// </summary>
        public string ExceptionType { get; set; }

        /// <summary>
        ///     List of inner exceptions
        /// </summary>
        public List<KafkaMessageExceptionDto> InnerExceptions { get; set; } = new List<KafkaMessageExceptionDto>();

        /// <summary>
        ///     Message of the exception
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Stacktrace belonging to the exception
        /// </summary>
        public string Stacktrace { get; set; }
    }
}