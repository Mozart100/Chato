using AutoMapper;
using Chato.Server.Infrastracture.Exceptions;

namespace Chato.Server.DataAccess.Repository
{
    public interface IRepositoryBase<TModel, TModelSlim> where TModel : TModelSlim
    {
        TModel Insert(TModel instance);

        Task<TModel> InsertAsync(TModel instance);


        TModelSlim Get(Predicate<TModel> selector);
        Task<TModelSlim> GetAsync(Predicate<TModel> selector);

        IEnumerable<TModelSlim> GetAll();

        Task<TModelSlim> GetOrDefaultAsync(Predicate<TModel> selector);


        Task<TModelSlim> GetFirstAsync(Predicate<TModel> selector);
        Task<IEnumerable<TModelSlim>> GetAllAsync(Func<TModel, bool> selector);

        Task<IEnumerable<TModelSlim>> GetAllAsync();

        Task<bool> RemoveAsync(Predicate<TModel> selector);


        Task UpdateAsync(Predicate<TModel> selector, Action<TModel> updateCallback);

    }



    public abstract class AutoRepositoryBase<TModel, TModelSlim> : IRepositoryBase<TModel, TModelSlim> where TModel : TModelSlim
    {
        private readonly IMapper _mapper;

        protected HashSet<TModel> Models;

        public AutoRepositoryBase(IMapper mapper)
        {
            Models = new HashSet<TModel>();
            this._mapper = mapper;
        }

        public async Task<bool> RemoveAsync(Predicate<TModel> selector)
        {
            var result = false;
            foreach (var model in Models)
            {
                if (selector(model))
                {
                    result = Models.Remove(model);
                }
            }

            return result;
        }

        public async Task<TModelSlim> GetFirstAsync(Predicate<TModel> selector)
        {
            return _mapper.Map<TModelSlim>(Get(selector));
        }

        public async Task<TModelSlim> GetOrDefaultAsync(Predicate<TModel> selector)
        {
            var result = default(TModelSlim);
            
            result = CoreGet(selector);
            if(result is null)
            {
                return result;
            }

            return _mapper.Map<TModelSlim>(result);
        }


        protected virtual TModel CoreGet(Predicate<TModel> selector)
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
        public virtual TModelSlim Get(Predicate<TModel> selector)
        {
            var model = CoreGet(selector);

            if (model is null)
            {
                throw new NoUserFoundException("no such user");
            }

            return _mapper.Map<TModelSlim>(model);
        }

        public virtual async Task<TModelSlim> GetAsync(Predicate<TModel> selector)
        {
            return Get(selector);
        }

        public virtual async Task<TModel> InsertAsync(TModel model)
        {
            return Insert(model);
        }

        public async Task<IEnumerable<TModelSlim>> GetAllAsync()
        {
            return Models.Select(item => _mapper.Map<TModelSlim>(item)).ToArray();
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

        public IEnumerable<TModelSlim> GetAll()
        {
            return Models.Select(x => _mapper.Map<TModelSlim>(x)).ToArray();
        }

        public async Task<IEnumerable<TModelSlim>> GetAllAsync(Func<TModel, bool> selector)
        {
            return Models.Where(x => selector(x)).Select(item => _mapper.Map<TModelSlim>(item)).ToArray();
        }

        public async Task UpdateAsync(Predicate<TModel> selector, Action<TModel> updateCallback)
        {
            var model = CoreGet(selector);

            if (model is not null)
            {
                updateCallback(model);
            }

        }
    }
}