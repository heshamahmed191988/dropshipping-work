using Jumia.Application.Contract;
using Jumia.model;
using Jumia.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Jumia.InfraStructure.Repository
{
    public class Repository<TEntity, Tid> : IRepository<TEntity, Tid> where TEntity : class, IBaseEntity
    {
        protected readonly JumiaContext _jumiaContext;
        protected readonly DbSet<TEntity> _Dbset;

        public Repository(JumiaContext jumiaContext)
        {
            _jumiaContext = jumiaContext;
            _Dbset = _jumiaContext.Set<TEntity>();
        }
        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            return (await _Dbset.AddAsync(entity)).Entity;
        }

        public Task<TEntity> DeleteAsync(TEntity entity)
        {

            return Task.FromResult(_Dbset.Remove(entity).Entity);
        }

        public Task<IQueryable<TEntity>> GetAllAsync()
        {
            return Task.FromResult(_Dbset.Select(s => s));
        }

        public async Task<TEntity> GetByIdAsync(Tid id)
        {
            return await _Dbset.FindAsync(id);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _jumiaContext.SaveChangesAsync();
        }

        public Task<TEntity> UpdateAsync(TEntity entity)
        {
            return Task.FromResult(_Dbset.Update(entity).Entity);

        }
    }
}
