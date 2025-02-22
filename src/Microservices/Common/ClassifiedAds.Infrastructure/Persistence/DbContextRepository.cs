﻿using ClassifiedAds.CrossCuttingConcerns.OS;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using EntityFrameworkCore.SqlServer.SimpleBulks.BulkDelete;
using EntityFrameworkCore.SqlServer.SimpleBulks.BulkInsert;
using EntityFrameworkCore.SqlServer.SimpleBulks.BulkMerge;
using EntityFrameworkCore.SqlServer.SimpleBulks.BulkUpdate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Persistence
{
    public class DbContextRepository<TDbContext, TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : AggregateRoot<TKey>
        where TDbContext : DbContext, IUnitOfWork
    {
        private readonly TDbContext _dbContext;
        private readonly IDateTimeProvider _dateTimeProvider;

        protected DbSet<TEntity> DbSet => _dbContext.Set<TEntity>();

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _dbContext;
            }
        }

        public DbContextRepository(TDbContext dbContext, IDateTimeProvider dateTimeProvider)
        {
            _dbContext = dbContext;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task AddOrUpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity.Id.Equals(default(TKey)))
            {
                entity.CreatedDateTime = _dateTimeProvider.OffsetNow;
                await DbSet.AddAsync(entity, cancellationToken);
            }
            else
            {
                entity.UpdatedDateTime = _dateTimeProvider.OffsetNow;
            }
        }

        public void Delete(TEntity entity)
        {
            DbSet.Remove(entity);
        }

        public IQueryable<TEntity> GetAll()
        {
            return _dbContext.Set<TEntity>();
        }

        public Task<T1> FirstOrDefaultAsync<T1>(IQueryable<T1> query)
        {
            return query.FirstOrDefaultAsync();
        }

        public Task<T1> SingleOrDefaultAsync<T1>(IQueryable<T1> query)
        {
            return query.SingleOrDefaultAsync();
        }

        public Task<List<T1>> ToListAsync<T1>(IQueryable<T1> query)
        {
            return query.ToListAsync();
        }

        public void BulkInsert(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>> columnNamesSelector)
        {
            _dbContext.BulkInsert(entities, columnNamesSelector);
        }

        public void BulkInsert(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>> columnNamesSelector, Expression<Func<TEntity, object>> idSelector)
        {
            _dbContext.BulkInsert(entities, columnNamesSelector, idSelector);
        }

        public void BulkUpdate(IList<TEntity> data, Expression<Func<TEntity, object>> idSelector, Expression<Func<TEntity, object>> columnNamesSelector)
        {
            _dbContext.BulkUpdate(data, idSelector, columnNamesSelector);
        }

        public void BulkDelete(IList<TEntity> data, Expression<Func<TEntity, object>> idSelector)
        {
            _dbContext.BulkDelete(data, idSelector);
        }

        public void BulkMerge(IEnumerable<TEntity> data, Expression<Func<TEntity, object>> idSelector, Expression<Func<TEntity, object>> updateColumnNamesSelector, Expression<Func<TEntity, object>> insertColumnNamesSelector)
        {
            _dbContext.BulkMerge(data, idSelector, updateColumnNamesSelector, insertColumnNamesSelector);
        }
    }
}
