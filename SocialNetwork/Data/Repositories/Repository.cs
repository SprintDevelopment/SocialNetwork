using Microsoft.EntityFrameworkCore;
using SocialNetwork.Assets.Extensions;
using SocialNetwork.Assets.Values.Constants;
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

        IQueryable<T> GetAll(int pageNumber = 0);

        IQueryable<T> Find(Expression<Func<T, bool>> predicate, int pageNumber = 0);

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

        public IQueryable<T> Find(Expression<Func<T, bool>> predicate, int pageNumber = 0)
        {
            if (pageNumber == 0)
                return Context.Set<T>().Where(predicate);
            else
                return Context.Set<T>().Where(predicate).Skip((pageNumber - 1) * SizeConstants.PAGE_SIZE).Take(SizeConstants.PAGE_SIZE);
        }

        public async Task<T> GetAsync(dynamic id)
        {
            return await Context.Set<T>().FindAsync(id);
        }

        public IQueryable<T> GetAll(int pageNumber = 0)
        {
            return Find(entity => true, pageNumber);
        }

        public void Remove(T entity)
        {
            Context.Entry(entity).State = EntityState.Detached;
            //var entityClone = entity.LightClone();
            Context.Set<T>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            Context.Set<T>().RemoveRange(entities);
        }
    }
}
