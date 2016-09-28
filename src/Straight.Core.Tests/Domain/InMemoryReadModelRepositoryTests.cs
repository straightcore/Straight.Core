using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Straight.Core.Domain;
using Straight.Core.Domain.Storage;
using Straight.Core.EventStore;
using Straight.Core.Exceptions;
using Straight.Core.Tests.Common;
using Straight.Core.Tests.Common.Domain;

namespace Straight.Core.Tests.Domain
{
    [TestFixture]
    public class InMemoryReadModelRepositoryTests
    {
        private InMemoryReadModelRepository<IDomainEvent> _repository;
        private IEnumerable<ReadModelTest> _beginSource;

        [SetUp]
        public void Setup()
        {
            _beginSource = PersonaReadModel.GenerateReadModelTests();
            IReadOnlyDictionary<Type, IReadOnlyDictionary<Guid, IReadModel<IDomainEvent>>> dico = new Dictionary<Type, IReadOnlyDictionary<Guid, IReadModel<IDomainEvent>>>
            {
                {typeof(ReadModelTest), _beginSource.Cast<IReadModel<IDomainEvent>>().ToDictionary(r => r.Id, r => r)}
            };
            _repository = new InMemoryReadModelRepository<IDomainEvent>(dico);
        }

        [Test]
        public void Should_get_read_model_when_get_by_id()
        {
            var actual = _repository.GetById<ReadModelTest>(_beginSource.First().Id);
            Assert.That(actual, Is.Not.Null);
        }

        [Test]
        public void Should_return_null_when_id_is_not_found()
        {
            var actual = _repository.GetById<ReadModelTest>(Guid.NewGuid());
            Assert.That(actual, Is.Null);
        }


        [Test]
        public void Should_does_not_throw_exception_when_add_new_readmodel()
        {
            var expected = PersonaReadModel.GenerateReadModelTest();
            Assert.DoesNotThrow(() => _repository.Add(expected));
        }

        [Test]
        public void Should_throw_argument_null_when_readmodel_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => _repository.Add<ReadModelTest>(null));
        }
        
        [Test]
        public void Should_throw_already_exception_when_add_readmodel_already_exist_in_database()
        {
            Assert.Throws<DomainModelAlreadyExistException>(() => _repository.Add(_beginSource.First()));
        }


        [Test]
        public void Should_does_not_throw_exception_when_add_new_element_with_new_type()
        {
            Assert.DoesNotThrow(() => _repository.Add(PersonaReadModel.GenerateReadModelTest2()));
        }
        
        [Test]
        public void Should_all_is_not_null_or_empty_when_get_all_by_type()
        {
            var actual = _repository.Get<ReadModelTest>();
            Assert.That(actual, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void Should_have_all_reamodel_when_get_all_by_type()
        {
            var actual = _repository.Get<ReadModelTest>();
            Assert.That(actual, Is.EquivalentTo(_beginSource));
        }



    }
}
