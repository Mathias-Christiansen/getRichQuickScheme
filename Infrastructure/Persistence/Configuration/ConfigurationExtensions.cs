using System.Linq.Expressions;
using Domain.Common;
using Domain.ValueObj;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration;

public static class ConfigurationExtensions
{
    public static void ConfigureMoney<TEntity>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, Money>> expression) 
        where TEntity : Entity
    {
        builder.OwnsOne(expression!, b =>
        {
            b.Property(x => x.SubUnit).IsRequired();
        });
    }
}