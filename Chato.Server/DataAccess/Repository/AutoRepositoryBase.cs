using System.Reflection;
using AutoMapper;
using Chato.Server.Infrastracture.Exceptions;
using Chatto.Shared;

namespace Chato.Server.DataAccess.Repository
{
    public interface IRepositoryBase<TModel, TModelDto> where TModel : IAutomapperEntities where TModelDto : IAutomapperEntities
    {
        TModelDto Insert(TModel instance);

        Task<TModelDto> InsertAsync(TModel instance);



        IEnumerable<TModelDto> GetAll();

        Task<TModelDto> FirstOrDefualtAsync(Predicate<TModel> selector);


        Task<IEnumerable<TModelDto>> GetAllAsync(Func<TModel, bool> selector);

        Task<IEnumerable<TModelDto>> GetAllAsync();



        Task<bool> UpdateAsync(Predicate<TModel> selector, Action<TModel> updateCallback);
        Task<bool> RemoveAsync(Predicate<TModel> selector);
        bool Remove(Predicate<TModel> selector);
        TModelDto FirstOrDefualt(Predicate<TModel> selector);
        IEnumerable<TModelDto> GetAll(Func<TModel, bool> selector);
        bool Update(Predicate<TModel> selector, Action<TModel> updateCallback);
    }



    public abstract class AutoRepositoryBase<TModel, TModelDto> : IRepositoryBase<TModel, TModelDto> where TModel : IAutomapperEntities where TModelDto : IAutomapperEntities
    {
        private readonly IMapper _mapper;

        protected HashSet<TModel> Models;

        public AutoRepositoryBase(IMapper mapper)
        {
            Models = new HashSet<TModel>();
            this._mapper = mapper;
        }

        public async virtual Task<bool> RemoveAsync(Predicate<TModel> selector)
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

        public virtual bool Remove(Predicate<TModel> selector)
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

        //public async Task<TModelDto> FirstAsync(Predicate<TModel> selector)
        //{
        //    return _mapper.Map<TModelDto>(Get(selector));
        //}

        //public TModelDto First(Predicate<TModel> selector)
        //{
        //    return _mapper.Map<TModelDto>(Get(selector));
        //}


        public async Task<TModelDto> FirstOrDefualtAsync(Predicate<TModel> selector)
        {
            var result = default(TModelDto);

            var model = CoreGet(selector);
            if (model is null)
            {
                return result;
            }

            return _mapper.Map<TModelDto>(model);
        }

        public TModelDto FirstOrDefualt(Predicate<TModel> selector)
        {
            var result = default(TModelDto);

            var model = CoreGet(selector);
            if (model is null)
            {
                return result;
            }

            return _mapper.Map<TModelDto>(model);
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

        public virtual async Task<TModelDto> InsertAsync(TModel model)
        {
            return Insert(model);
        }

        public async Task<IEnumerable<TModelDto>> GetAllAsync()
        {
            return Models.Select(item => _mapper.Map<TModelDto>(item)).ToArray();
        }
        /// <summary>
        /// Auto Id Generator
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual TModelDto Insert(TModel instance)
        {

            if (Models.Add(instance) == false)
            {
                throw new Exception("Key already present.");
            }


            var model = _mapper.Map<TModelDto>(instance);
            return model;
        }

        public virtual IEnumerable<TModelDto> GetAll()
        {
            return Models.Select(x => _mapper.Map<TModelDto>(x)).ToArray();
        }

        public async Task<IEnumerable<TModelDto>> GetAllAsync(Func<TModel, bool> selector)
        {
            return Models.Where(x => selector(x)).Select(item => _mapper.Map<TModelDto>(item)).ToArray();
        }

        public IEnumerable<TModelDto> GetAll(Func<TModel, bool> selector)
        {
            return Models.Where(x => selector(x)).Select(item => _mapper.Map<TModelDto>(item)).ToArray();
        }

        public async Task<bool> UpdateAsync(Predicate<TModel> selector, Action<TModel> updateCallback)
        {
            var model = CoreGet(selector);

            if (model is not null)
            {
                updateCallback(model);
                return true;
            }

            return false;
        }

        public bool Update(Predicate<TModel> selector, Action<TModel> updateCallback)
        {
            var model = CoreGet(selector);

            if (model is not null)
            {
                updateCallback(model);
                return true;
            }

            return false;
        }
    }
}