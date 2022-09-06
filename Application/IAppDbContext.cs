using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application;

public interface IAppDbContext
{
    DbSet<User> Users { get;}
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    public DbSet<T> Set<T>()
        where T : class;
}