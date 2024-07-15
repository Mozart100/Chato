using Chato.Server.DataAccess.Models;
using Chato.Server.Infrastracture;
using Chato.Server.Infrastracture.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Chato.Server.DataAccess.Repository
{

    public interface IRepositoryBase<TModel> where TModel : class
    {
        TModel Get(Predicate<TModel> selector);
        Task<TModel> GetAsync(Predicate<TModel> selector);

        TModel Insert(TModel instance);
        IEnumerable<TModel> GetAll();

        Task<TModel> GetOrDefaultAsync(Predicate<TModel> selector);




        Task<TModel> GetFirstAsync(Predicate<TModel> selector);
        Task<IEnumerable<TModel>> GetAllAsync(Func<TModel, bool> selector);

        Task<TModel> InsertAsync(TModel instance);
        Task<IEnumerable<TModel>> GetAllAsync();

        Task<bool> RemoveAsync(Predicate<TModel> selector);
    }



    public abstract class RepositoryBase<TModel> where TModel : EntityDbBase
    {
        
        protected HashSet<TModel> Models;

        public RepositoryBase()
        {
            Models = new HashSet<TModel>();
        }

        public async virtual Task<bool> RemoveAsync(Predicate<TModel> selector)
        {
            var result = false;
            foreach (var model in Models)
            {
                if(selector(model))
                {
                    result = Models.Remove(model);
                }
            }

            return result;
        }

        public async Task<TModel> GetFirstAsync(Predicate<TModel> selector)
        {
            return Get(selector);
        }

        public async Task<TModel> GetOrDefaultAsync(Predicate<TModel> selector)
        {
            return CoreGet(selector);
        }


        protected virtual TModel CoreGet(Predicate<TModel> selector )
        {
            var result = default(TModel);
            foreach (var model in Models)
            {
                if (selector(model))
                {
                    result = model;
                }
            }

            return result;
        }
        public virtual TModel Get(Predicate<TModel> selector)
        {
            var result = CoreGet(selector);

            if(result is null)
            {
                throw new NoUserFoundException("no such user");
            }

            return result;
        }

        public virtual async Task<TModel> GetAsync(Predicate<TModel> selector)
        {
            return Get(selector);
        }

        public virtual async Task<TModel> InsertAsync(TModel model)
        {
            return Insert(model);
        }

        public async Task<IEnumerable<TModel>> GetAllAsync()
        {
            return GetAll();
        }
        /// <summary>
        /// Auto Id Generator
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual TModel Insert(TModel instance)
        {
           
            if (Models.Add(instance) == false)
            {
                throw new Exception("Key already present.");
            }
            
            return instance;
        }
        public virtual IEnumerable<TModel> GetAll()
        {
            return Models.SafeToArray();
        }

        public async Task<IEnumerable<TModel>> GetAllAsync(Func<TModel, bool> selector)
        {
            return GetAll().Where(x => selector(x)).SafeToArray();
        }


    }
}