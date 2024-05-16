using Chato.Server.DataAccess.Models;
using Chato.Server.Infrastracture;
using System.Reflection;

namespace Chato.Server.DataAccess.Repository;


public interface IUserRepository : IRepositoryBase<UserDb>
{
    Task AssignConnectionId(string userName, string conectionnId);
}


public class UserRepository : RepositoryBase<UserDb>, IUserRepository
{
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(ILogger<UserRepository> logger)
    {
        _logger = logger;
    }

    public virtual async Task AssignConnectionId(string userName, string conectionnId)
    {
        var user = await GetAsync(x => x.UserName == userName);
        user.ConnectionId = conectionnId;
    }
}


public class UserRepositoryManager : IUserRepository
{
    private readonly ILogger<UserRepositoryManager> logger;
    private readonly IDelegateQueue _delegateQueue;
    private readonly IUserRepository _userRepository;

    public UserRepositoryManager(ILogger<UserRepositoryManager> logger, IDelegateQueue delegateQueue, IUserRepository userRepository)
    {
        this.logger = logger;
        this._delegateQueue = delegateQueue;
        this._userRepository = userRepository;
    }

    public async Task AssignConnectionId(string userName, string connectionId)
    {
        await _delegateQueue.InvokeAsync(async () => await _userRepository.AssignConnectionId(userName, connectionId));
    }

    public UserDb Get(Predicate<UserDb> selector)
    {
        var result = default(UserDb);

        _delegateQueue.Invoke(() => result = _userRepository.Get(selector));
        return result;
    }

    public IEnumerable<UserDb> GetAll()
    {
        var result = default(IEnumerable<UserDb>);

        _delegateQueue.Invoke(() => result = _userRepository.GetAll());
        return result;
    }

    public async Task<IEnumerable<UserDb>> GetAllAsync(Func<UserDb, bool> selector)
    {
        var result = default(IEnumerable<UserDb>);

        await _delegateQueue.InvokeAsync(async () => result = await _userRepository.GetAllAsync(selector));
        return result;
    }

    public async Task<IEnumerable<UserDb>> GetAllAsync()
    {
        var result = default(IEnumerable<UserDb>);

        await _delegateQueue.InvokeAsync(async () => result = await _userRepository.GetAllAsync());
        return result;
    }

    public async Task<UserDb> GetAsync(Predicate<UserDb> selector)
    {
        var result = default(UserDb);

        await _delegateQueue.InvokeAsync(async () => result = await _userRepository.GetAsync(selector));
        return result;
    }

    public async Task<UserDb> GetFirstAsync(Predicate<UserDb> selector)
    {
        var result = default(UserDb);

        await _delegateQueue.InvokeAsync(async () => result = await _userRepository.GetFirstAsync(selector));
        return result;
    }

    public async Task<UserDb> GetOrDefaultAsync(Predicate<UserDb> selector)
    {
        var result = default(UserDb);

        await _delegateQueue.InvokeAsync(async () => result = await _userRepository.GetOrDefaultAsync(selector));
        return result;
    }



    public async Task<bool> RemoveAsync(Predicate<UserDb> selector)
    {
        var result = default(bool);

        await _delegateQueue.InvokeAsync(async () => result = await _userRepository.RemoveAsync(selector));
        return result;
    }

    public  async Task<UserDb> InsertAsync(UserDb model)
    {
        var result = default(UserDb);

        await _delegateQueue.InvokeAsync(async () => result = await _userRepository.InsertAsync(model));
        return result;
    }

    public  UserDb Insert(UserDb model)
    {
        var result = default(UserDb);

        _delegateQueue.Invoke(() => result = _userRepository.Insert(model));
        return result;
    }

    //public override async Task AssignConnectionId(string userName, string connectionId)
    //{
    //    await _delegateQueue.InvokeAsync(async () => await base.AssignConnectionId(userName, connectionId));
    //}
}