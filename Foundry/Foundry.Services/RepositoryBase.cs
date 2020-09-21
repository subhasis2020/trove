using Foundry.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected FoundryDbContext FoundryDbContext { get; set; }

        public RepositoryBase(FoundryDbContext repositoryContext)
        {
            this.FoundryDbContext = repositoryContext;
        }

        public async Task<IEnumerable<T>> FindAllAsync()
        {
            return await this.FoundryDbContext.Set<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> expression)
        {
            return await this.FoundryDbContext.Set<T>().Where(expression).ToListAsync();
        }

        public void Create(T entity)
        {
            this.FoundryDbContext.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            this.FoundryDbContext.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            this.FoundryDbContext.Set<T>().Remove(entity);
        }

        public async Task SaveAsync()
        {
            await this.FoundryDbContext.SaveChangesAsync();
        }
    }
}
