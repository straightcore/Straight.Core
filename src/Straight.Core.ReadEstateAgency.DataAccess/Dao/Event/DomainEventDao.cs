using System;
using System.Collections.Generic;

namespace Straight.Core.ReadEstateAgency.DataAccess.Dao.Event
{
    public class DomainEventDao
    {
        public Guid AggregatorId { get; set; }

        public string StringFormatEvent { get; set; }

        public string DeserializerType { get; set; }
    }
}