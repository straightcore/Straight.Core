using Straight.Core.Domain;
using Straight.Core.Domain.Storage;
using Straight.Core.EventStore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Straight.Core.Sample.RealEstateAgency.Test.Common
{
    public class TestReadModelRepository : IReadModelRepository<IDomainEvent>
    {
        private readonly Dictionary<Guid, object> cache = new Dictionary<Guid, object>();

        public TReadModel Get<TReadModel>(Guid id) where TReadModel : class, IReadModel<IDomainEvent>, new()
        {
            object obj;
            if (cache.TryGetValue(id, out obj))
                return obj as TReadModel;
            return null;
        }

        public IEnumerable<TReadModel> Get<TReadModel>() where TReadModel : class, IReadModel<IDomainEvent>, new()
        {
            return cache.Values.OfType<TReadModel>().ToList();
        }

        public void Add<TReadModel>(TReadModel readModel) where TReadModel : class, IReadModel<IDomainEvent>, new()
        {
            cache.Add(readModel.Id, readModel);
        }
    }
}