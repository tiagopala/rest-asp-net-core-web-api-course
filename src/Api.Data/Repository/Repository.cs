using Api.Business.Interfaces;
using Api.Business.Models;
using Api.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Api.Data.Repository
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity, new()
    {
        protected readonly ApiDbContext Db;
        protected readonly DbSet<TEntity> DbSet;

        protected Repository(ApiDbContext db)
        {
            Db = db;
            DbSet = db.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> Buscar(Expression<Func<TEntity, bool>> predicate)
        {
            // Após consultas do EntityFramework e o resultado já estar em memória, o EntityFramework começa a realizar o "tracking" ou seja, rastrear esse objeto para possíveis alterações / mudanças de estado
            // Nesse caso, estamos utilizando o AsNoTracking pois o objeto estar na memória ele não será rastreado, dessa forma tendo melhor performance
            return await DbSet.AsNoTracking().Where(predicate).ToListAsync();
        }

        public virtual async Task<TEntity> ObterPorId(Guid id)
        {
            return await DbSet.FindAsync(id);
        }

        public virtual async Task<List<TEntity>> ObterTodos()
        {
            return await DbSet.ToListAsync();
        }

        public virtual async Task Adicionar(TEntity entity)
        {
            DbSet.Add(entity);
            await SaveChanges();
        }

        public virtual async Task Atualizar(TEntity entity)
        {
            DbSet.Update(entity);
            await SaveChanges();
        }

        public virtual async Task Remover(Guid id)
        {
            DbSet.Remove(new TEntity { Id = id });
            await SaveChanges();
        }

        public async Task<int> SaveChanges()
        {
            return await Db.SaveChangesAsync();
        }

        public void Dispose()
        {
            // Estamos verificando se o Db existe antes de realizar o dispose para não gerarmos uma exceção do tipo NullReferenceException
            Db?.Dispose();
        }
    }
}
