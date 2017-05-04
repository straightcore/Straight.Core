using Xunit;
using Straight.Core.Exceptions;
using System;
using System.Collections.Generic;

namespace Straight.Core.Tests.Exceptions
{
    
    public class ExceptionTests
    {
        [Fact]
        public void Should_check_inner_exception_when_innerexception_is_set()
        {
            var applicationException = new Exception();
            const string exceptionmessage = "ExceptionMessage";
            var exceptions = new List<Exception>
            {
                new TransactionException(exceptionmessage, applicationException),
                new ViolationConcurrencyException(exceptionmessage, applicationException),
                new UnregisteredDomainEventException(exceptionmessage, applicationException)
            };

            foreach (var exception in exceptions)
            {
                Assert.NotNull(exception);
                Assert.Equal(exception.InnerException, applicationException);
            }
        }

        [Fact]
        public void Should_check_message_when_message_is_set()
        {
            const string exceptionmessage = "ExceptionMessage";
            var exceptions = new List<Exception>
            {
                new TransactionException(exceptionmessage),
                new ViolationConcurrencyException(exceptionmessage),
                new UnregisteredDomainEventException(exceptionmessage)
            };

            foreach (var exception in exceptions)
            {
                Assert.NotNull(exception);
                Assert.Equal(exception.Message, exceptionmessage);
            }
        }

        [Fact]
        public void Should_have_base_of_exception_when_constructor_without_parameters()
        {
            var exceptions = new List<Exception>
            {
                new TransactionException(),
                new ViolationConcurrencyException(),
                new UnregisteredDomainEventException(),
                new DomainModelAlreadyExistException(),
                new NotRegisteredRouteException()
            };

            foreach (var exception in exceptions)
            {
                Assert.NotNull(exception);
                Assert.Equal(exception.Message, $"A system exception ({exception.GetType().FullName}) occurred");
            }
        }
    }
}