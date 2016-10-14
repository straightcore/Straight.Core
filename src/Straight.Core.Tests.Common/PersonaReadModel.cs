using Straight.Core.Tests.Common.Domain;
using Straight.Core.Tests.Common.EventStore;
using System;
using System.Collections.Generic;

namespace Straight.Core.Tests.Common
{
    public static class PersonaReadModel
    {
        public static IEnumerable<ReadModelTest> GenerateReadModelTests()
        {
            return new[]
            {
                GenerateReadModelTest(),
                GenerateReadModelTest(),
                GenerateReadModelTest(),
                GenerateReadModelTest(),
                GenerateReadModelTest(),
                GenerateReadModelTest(),
                GenerateReadModelTest()
            };
        }

        public static ReadModelTest GenerateReadModelTest()
        {
            var readModel = new ReadModelTest();
            readModel.Update(new DomainEventTest {Id = Guid.NewGuid(), AggregateId = Guid.NewGuid(), Version = 1});
            return readModel;
        }

        public static ReadModelTest2 GenerateReadModelTest2()
        {
            var readModel = new ReadModelTest2();
            readModel.Update(new DomainEventTest2 {Id = Guid.NewGuid(), AggregateId = Guid.NewGuid(), Version = 1});
            return readModel;
        }
    }
}