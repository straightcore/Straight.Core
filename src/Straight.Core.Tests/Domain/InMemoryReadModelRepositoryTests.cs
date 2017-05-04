using Xunit;
using Straight.Core.Domain;
using Straight.Core.Domain.Storage;
using Straight.Core.EventStore;
using Straight.Core.Exceptions;
using Straight.Core.Tests.Common;
using Straight.Core.Tests.Common.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Straight.Core.Tests.Domain
{
    
    public class InMemoryReadModelRepositoryTests
    {
        
        public InMemoryReadModelRepositoryTests()
        {
            _beginSource = PersonaReadModel.GenerateReadModelTests();
            IReadOnlyDictionary<Type, IReadOnlyDictionary<Guid, IReadModel<IDomainEvent>>> dico = new Dictionary
                <Type, IReadOnlyDictionary<Guid, IReadModel<IDomainEvent>>>
                {
                    {
                        typeof(ReadModelTest),
                        _beginSource.Cast<IReadModel<IDomainEvent>>().ToDictionary(r => r.Id, r => r)
                    }
                };
            _repository = new InMemoryReadModelRepository<IDomainEvent>(dico);
        }

        private InMemoryReadModelRepository<IDomainEvent> _repository;
        private IEnumerable<ReadModelTest> _beginSource;

        [Fact]
        public void Should_all_is_not_null_or_empty_when_get_all_by_type()
        {
            var actual = _repository.Get<ReadModelTest>();
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
        }

        [Fact]
        public void Should_does_not_throw_exception_when_add_new_element_with_new_type()
        {
            _repository.Add(PersonaReadModel.GenerateReadModelTest2());
        }

        [Fact]
        public void Should_does_not_throw_exception_when_add_new_readmodel()
        {
            var expected = PersonaReadModel.GenerateReadModelTest();
            _repository.Add(expected);
        }

        [Fact]
        public void Should_get_read_model_when_get_by_id()
        {
            var actual = _repository.Get<ReadModelTest>(_beginSource.First().Id);
            Assert.NotNull(actual);
        }

        [Fact]
        public void Should_have_all_reamodel_when_get_all_by_type()
        {
            var actual = _repository.Get<ReadModelTest>().ToList();
            var hash = new HashSet<ReadModelTest>(_beginSource);
            Assert.Equal(actual.Count, hash.Count);
            Assert.All(actual, test => hash.Contains(test)); // Equivalent
        }

        [Fact]
        public void Should_return_null_when_id_is_not_found()
        {
            var actual = _repository.Get<ReadModelTest>(Guid.NewGuid());
            Assert.Null(actual);
        }

        [Fact]
        public void Should_throw_already_exception_when_add_readmodel_already_exist_in_database()
        {
            Assert.Throws<DomainModelAlreadyExistException>(() => _repository.Add(_beginSource.First()));
        }

        [Fact]
        public void Should_throw_argument_null_when_readmodel_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => _repository.Add<ReadModelTest>(null));
        }
    }
}