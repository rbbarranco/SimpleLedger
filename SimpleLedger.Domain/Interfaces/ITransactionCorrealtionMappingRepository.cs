using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleLedger.Domain.Entities;

namespace SimpleLedger.Domain.Interfaces
{
    public interface ITransactionCorrealtionMappingRepository
    {
        Task<TransactionCorrelationIdMapping> GetCorrelation(Guid correlationId);
        Task CreateCorrelation(Guid correlationId);
    }
}
