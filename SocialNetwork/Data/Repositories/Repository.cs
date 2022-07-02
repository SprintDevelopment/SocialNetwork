using Microsoft.EntityFrameworkCore;
using SocialNetwork.Assets.Extensions;
using SocialNetwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SocialNetwork.Data.Repositories
{
    public interface IRepository<T> where T : BaseModel
    {
        void Add(T entity, bool autoComplete = false);
        void AddRange(IEnumerable<T> entities);
        void Update(T oldEntity, T newEntity, bool autoComplete = false);

        Task<T> GetAsync(dynamic id);

        IQueryable<T> Find(Expression<Func<T, bool>> predicate = null);

        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }

    public class Repository<T> : IRepository<T> where T : BaseModel, new()
    {
        protected readonly DbContext Context;

        public Repository(DbContext context)
        {
            Context = context;
        }

        public void Add(T entity, bool autoComplete = false)
        {
            var entityClone = entity.LightClone();
            Context.Set<T>().Add(entityClone);

            if (autoComplete)
            {
                Context.SaveChanges();

                var keyProperty = entity.GetKeyProperty();
                if (keyProperty != null)
                {
                    var keyValue = keyProperty.GetValue(entityClone);
                    keyProperty.SetValue(entity, keyValue);
                }
            }
        }

        public void AddRange(IEnumerable<T> entities)
        {
            Context.Set<T>().AddRange(entities);
        }

        public void Update(T oldEntity, T newEntity, bool autoComplete = false)
        {
            var entityClone = newEntity.LightClone();

            Context.Entry(oldEntity).State = EntityState.Detached;
            Context.Entry(entityClone).State = EntityState.Modified;

            Context.Set<T>().Update(entityClone);

            if (autoComplete)
            {
                Context.SaveChanges();

                var keyProperty = newEntity.GetKeyProperty();
                if (keyProperty != null)
                {
                    var keyValue = keyProperty.GetValue(entityClone);
                    keyProperty.SetValue(newEntity, keyValue);
                }
            }
        }

        public IQueryable<T> Find(Expression<Func<T, bool>> predicate = null)
        {
            return Context.Set<T>().Where(predicate ?? (x => true));
        }

        public async Task<T> GetAsync(dynamic id)
        {
            return await Context.Set<T>().FindAsync(id);
        }

        public void Remove(T entity)
        {
            Context.Entry(entity).State = EntityState.Detached;
            Context.Set<T>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            Context.Set<T>().RemoveRange(entities);
        }
    }
}
