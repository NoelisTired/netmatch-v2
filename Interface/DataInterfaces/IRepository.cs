using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Interface.DataInterfaces;

public interface IRepository<T>
{
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}

