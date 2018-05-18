using Straight.Core.Domain;
using Straight.Core.Domain.Storage;
using Straight.Core.EventStore;
using Straight.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Straight.Core.Tests.Common.Domain;
using Straight.Core.Tests.Common;

namespace Straight.Core.Tests.Domain
{
    [TestFixture]
    public class InMemoryReadModelRepositoryTests
    {

        private class RepoContext
        {
            public RepoContext()
            {
                BeginSource = PersonaReadModel.GenerateReadModelTests();
                IReadOnlyDictionary<Type, IReadOnlyDictionary<Guid, IReadModel<IDomainEvent>>> dico = new Dictionary
                    <Type, IReadOnlyDictionary<Guid, IReadModel<IDomainEvent>>>
                {
                    {
                        typeof(ReadModelTest),
                        BeginSource.Cast<IReadModel<IDomainEvent>>().ToDictionary(r => r.Id, r => r)
                    }
                };
                Repository = new InMemoryReadModelRepository<IDomainEvent>(dico);
            }

            public InMemoryReadModelRepository<IDomainEvent> Repository { get; }
            public IEnumerable<ReadModelTest> BeginSource { get; }
        }

        [Test]
        public void Should_all_is_not_null_or_empty_when_get_all_by_type()
        {
            var context = new RepoContext();
            var actual = context.Repository.Get<ReadModelTest>();
            Assert.That(actual, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void Should_does_not_throw_exception_when_add_new_element_with_new_type()
        {
            var context = new RepoContext();
            Assert.DoesNotThrow(() => context.Repository.Add(PersonaReadModel.GenerateReadModelTest2()));
        }

        [Test]
        public void Should_does_not_throw_exception_when_add_new_readmodel()
        {
            var context = new RepoContext();
            var testValues = PersonaReadModel.GenerateReadModelTest();
            Assert.DoesNotThrow(() => context.Repository.Add(testValues));
        }

        [Test]
        public void Should_get_read_model_when_get_by_id()
        {
            var context = new RepoContext();
            var actual = context.Repository.Get<ReadModelTest>(context.BeginSource.First().Id);
            Assert.NotNull(actual);
        }

        [Test]
        public void Should_have_all_reamodel_when_get_all_by_type()
        {
            var context = new RepoContext();
            var actual = context.Repository.Get<ReadModelTest>().ToList();
            var expectedHash = new HashSet<ReadModelTest>(context.BeginSource);
            Assert.That(actual.Count, Is.EqualTo(expectedHash.Count));
            Assert.That(actual, Is.EquivalentTo(expectedHash)); // Equivalent
        }

        [Test]
        public void Should_return_null_when_id_is_not_found()
        {
            var context = new RepoContext();
            var actual = context.Repository.Get<ReadModelTest>(Guid.NewGuid());
            Assert.Null(actual);
        }

        [Test]
        public void Should_throw_already_exception_when_add_readmodel_already_exist_in_database()
        {
            var context = new RepoContext();
            Assert.Throws<DomainModelAlreadyExistException>(() => context.Repository.Add(context.BeginSource.First()));
        }

        [Test]
        public void Should_throw_argument_null_when_readmodel_is_null()
        {
            var context = new RepoContext();
            Assert.Throws<ArgumentNullException>(() => context.Repository.Add<ReadModelTest>(null));
        }
    }
}