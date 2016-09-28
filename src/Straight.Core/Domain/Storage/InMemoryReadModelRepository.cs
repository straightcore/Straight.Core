using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Straight.Core.Exceptions;
using Straight.Core.Extensions.Collections;
using Straight.Core.Extensions.Guard;

namespace Straight.Core.Domain.Storage
{
    public class InMemoryReadModelRepository<TDomainEvent> : IReadModelRepository<TDomainEvent>
        where TDomainEvent : IDomainEvent
    {
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<Guid, IReadModel<TDomainEvent>>> _memory;

        public InMemoryReadModelRepository()
        {
            _memory = new ConcurrentDictionary<Type, ConcurrentDictionary<Guid, IReadModel<TDomainEvent>>>();
        }

        public InMemoryReadModelRepository(
            IReadOnlyDictionary<Type, IReadOnlyDictionary<Guid, IReadModel<TDomainEvent>>> source)
            : this()
        {
            source.CheckIfArgumentIsNull(nameof(source));
            foreach (var typeToData in source)
            {
                _memory.TryAdd(typeToData.Key,
                    new ConcurrentDictionary<Guid, IReadModel<TDomainEvent>>(typeToData.Value));
            }
        }

        public TReadModel GetById<TReadModel>(Guid id) where TReadModel : class, IReadModel<TDomainEvent>, new()
        {
            var repo = GetReadModelRepository<TReadModel>(_memory);
            return repo == null ? null : GetReadModel<TReadModel>(id, repo);
        }

        public IEnumerable<TReadModel> Get<TReadModel>() where TReadModel : class, IReadModel<TDomainEvent>, new()
        {
            var repo = GetReadModelRepository<TReadModel>(_memory);
            return repo?.Select(pair => pair.Value as TReadModel).ToList().AsReadOnly() 
                ?? Enumerable.Empty<TReadModel>();
        }

        public void Add<TReadModel>(TReadModel readModel) 
            where TReadModel : class, IReadModel<TDomainEvent>, new()
        {
            readModel.CheckIfArgumentIsNull(nameof(readModel));
            var repo = GetReadModelRepository<TReadModel>(_memory);
            CheckIfAlreadyExist<TReadModel>(readModel, repo);
            if (repo == null)
            {
                repo = new ConcurrentDictionary<Guid, IReadModel<TDomainEvent>>();
                _memory.TryAdd(typeof(TReadModel), repo);
            }
            repo.TryAdd(readModel.Id, readModel);
        }
        
        private static ConcurrentDictionary<Guid, IReadModel<TDomainEvent>> GetReadModelRepository<TReadModel>(
                IReadOnlyDictionary<Type, ConcurrentDictionary<Guid, IReadModel<TDomainEvent>>> memory)
            where TReadModel : class, IReadModel<TDomainEvent>, new()
        {
            ConcurrentDictionary<Guid, IReadModel<TDomainEvent>> repo;
            return !memory.TryGetValue(typeof(TReadModel), out repo) ? null : repo;
        }
        
        private static TReadModel GetReadModel<TReadModel>(Guid id, IReadOnlyDictionary<Guid, IReadModel<TDomainEvent>> repo)
            where TReadModel : class, IReadModel<TDomainEvent>, new()
        {
            IReadModel<TDomainEvent> readModel;
            return repo.TryGetValue(id, out readModel) ? readModel as TReadModel : null;
        }

        private static void CheckIfAlreadyExist<TReadModel>(IIdentifiable model, IReadOnlyDictionary<Guid, IReadModel<TDomainEvent>> output)
            where TReadModel : class, IReadModel<TDomainEvent>, new()
        {
            if (output == null 
                || GetReadModel<TReadModel>(model.Id, output) == null)
            {
                return;
            }
            throw new DomainModelAlreadyExistException(model.Id);
        }

    }
}