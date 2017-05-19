﻿using Straight.Core.Command;
using Straight.Core.Domain;
using Straight.Core.Sample.RealEstateAgency.Model;
using System.Collections.Generic;
using Straight.Core.Common.Command;

namespace Straight.Core.Sample.RealEstateAgency.Account.Domain.Command
{
    public class AttachCustomersCommand : DomainCommandBase, ICommand
    {
        public string ModifierFirstName { get; set; }
        public string ModifierLastName { get; set; }
        public string ModifierUsername { get; set; }

        public IEnumerable<Customer> Customers { get; set; }
    }
}