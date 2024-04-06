using Arkovean.Chat.DataAccess.Models;

namespace Arkovean.Chat.DataAccess.Repository
{
    public interface IRepositoryBase<TModel> where TModel : class
    {
        TModel Get(Predicate<TModel> selector);

        TModel Insert(TModel instance);
        IEnumerable<TModel> GetAll();


        Task<TModel> GetFirstAsync(Predicate<TModel> selector);
        Task<IEnumerable<TModel>> GetAllAsync(Func<TModel, bool> selector);

        Task<TModel> InsertAsync(TModel instance);
        Task<IEnumerable<TModel>> GetAllAsync();


    }



    public abstract class RepositoryBase<TModel> where TModel : EntityDbBase
    {
        protected HashSet<TModel> Models;

        public RepositoryBase()
        {
            Models = new HashSet<TModel>();
        }

        public async Task<TModel> GetFirstAsync(Predicate<TModel> selector)
        {
            return Get(selector);
        }

        public virtual TModel Get(Predicate<TModel> selector)
        {
            foreach (var model in Models)
            {
                if (selector(model))
                {
                    return model;
                }
            }

            return null;
        }

        public async Task<TModel> InsertAsync(TModel model)
        {
            return Insert(model);
        }

        public async Task<IEnumerable<TModel>> GetAllAsync()
        {
            return Models.ToArray();
        }
        /// <summary>
        /// Auto Id Generator
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public TModel Insert(TModel instance)
        {
            var newId = Models.Count + 1;
            instance.Id = newId;

            Models.Add(instance);
            return instance;
        }
        public IEnumerable<TModel> GetAll()
        {
            return Models.ToArray();
        }

        public async Task<IEnumerable<TModel>> GetAllAsync(Func<TModel, bool> selector)
        {
            return Models.Where(x => selector(x)).ToArray();
        }

    }
}