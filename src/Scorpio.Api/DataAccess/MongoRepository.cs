using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Scorpio.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Scorpio.Api.DataAccess
{
    public class MongoRepository<TEntity> : IGenericRepository<TEntity, string> where TEntity : EntityBase
    {
        private IMongoDatabase _database;
        protected IMongoCollection<TEntity> Collection;

        public MongoRepository(IOptions<MongoDbConfiguration> options)
        {
            var mongoClient = new MongoClient(options.Value.ConnectionString);
            _database = mongoClient.GetDatabase(options.Value.Database);
            var collectionName = typeof(TEntity).Name;
            Collection = _database.GetCollection<TEntity>(collectionName);
        }

        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            var insertOptions = new InsertOneOptions
            {
                BypassDocumentValidation = false
            };

            await Collection.InsertOneAsync(entity, insertOptions);
            return entity;
        }

        public virtual async Task DeleteAsync(TEntity entity)
        {
            await Collection.DeleteOneAsync(x => x.Id == entity.Id);
        }
        
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await Collection.Find(_ => true).ToListAsync();
        }

        public virtual async Task<TEntity> GetByIdAsync(string id)
        {
            return await Collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public virtual async Task<TEntity> GetFiltered(Expression<Func<TEntity, bool>> predicate)
        {
            return await Collection.Find(predicate).FirstOrDefaultAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> GetManyFiltered(Expression<Func<TEntity, bool>> predicate)
        {
            return await Collection.Find(predicate).ToListAsync();
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            await Collection.ReplaceOneAsync(x => x.Id == entity.Id, entity);
            return entity;
        }

        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _database = null;
                    Collection = null;
                }
                
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
