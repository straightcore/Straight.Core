using NUnit.Framework;
using Straight.Core.Exceptions;
using System;
using System.Collections.Generic;

namespace Straight.Core.Tests.Exceptions
{
    [TestFixture]
    public class ExceptionTests
    {
        [Test]
        public void Should_check_inner_exception_when_innerexception_is_set()
        {
            var applicationException = new ApplicationException();
            const string exceptionmessage = "ExceptionMessage";
            var exceptions = new List<Exception>
            {
                new TransactionException(exceptionmessage, applicationException),
                new ViolationConcurrencyException(exceptionmessage, applicationException),
                new UnregisteredDomainEventException(exceptionmessage, applicationException)
            };

            foreach (var exception in exceptions)
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(exception.InnerException, Is.EqualTo(applicationException));
            }
        }

        [Test]
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
                Assert.That(exception, Is.Not.Null);
                Assert.That(exception.Message, Is.EqualTo(exceptionmessage));
            }
        }

        [Test]
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
                Assert.That(exception, Is.Not.Null);
                Assert.That(exception.Message,
                    Is.EqualTo($"A system exception ({exception.GetType().FullName}) occurred"));
            }
        }
    }
}