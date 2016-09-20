using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Straight.Core.Transactional
{
    public interface ITransaction
    {
        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}
