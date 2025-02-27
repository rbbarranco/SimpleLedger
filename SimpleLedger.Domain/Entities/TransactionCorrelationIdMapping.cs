using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLedger.Domain.Entities
{
    public class TransactionCorrelationIdMapping
    {
        public Guid CorrelationId { get; init; }

    }
}
