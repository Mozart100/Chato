//using AutoMapper;
//using Chato.Server.DataAccess.Models;
//using Chato.Server.Infrastracture;

//namespace Chato.Server.DataAccess.Repository;


//public class DelegateQueueRoomRepository : IRoomRepository
//{
//    private readonly IDelegateQueue _delegateQueue;
//    private readonly IRoomRepository _roomRepository;
//    private readonly IMapper mapper;

//    public DelegateQueueRoomRepository(IDelegateQueue delegateQueue, 
//        IRoomRepository roomRepository

//        //IMapper mapper
//        )
//    {
//        this._delegateQueue = delegateQueue;
//        this._roomRepository = roomRepository;
//        this.mapper = mapper;
//    }

//    public ChatRoomDb Get(Predicate<ChatRoomDb> selector)
//    {
//        var result = default(ChatRoomDb);

//        _delegateQueue.Invoke(() => result = _roomRepository.Get(selector));

//        //result = mapper.Map<ChatRoomDb>(result);
//        return Clone(result);
//    }

//    public IEnumerable<ChatRoomDb> GetAll()
//    {
//        var result = default(IEnumerable<ChatRoomDb>);

//        _delegateQueue.Invoke(() => result = _roomRepository.GetAll());
//        return Clone(result);
//    }

//    public async Task<IEnumerable<ChatRoomDb>> GetAllAsync(Func<ChatRoomDb, bool> selector)
//    {
//        var result = default(IEnumerable<ChatRoomDb>);

//        await _delegateQueue.InvokeAsync(async () => result = await _roomRepository.GetAllAsync(selector));
//        return Clone(result);
//    }

//    public async Task<IEnumerable<ChatRoomDb>> GetAllAsync()
//    {
//        var result = default(IEnumerable<ChatRoomDb>);

//        await _delegateQueue.InvokeAsync(async () => result = await _roomRepository.GetAllAsync());
//        return Clone(result);
//    }

//    public async Task<ChatRoomDb> GetAsync(Predicate<ChatRoomDb> selector)
//    {
//        var result = default(ChatRoomDb);

//        await _delegateQueue.InvokeAsync(async () => result = await _roomRepository.GetAsync(selector));
      
//        //result = mapper.Map<ChatRoomDb>(result);
//        return Clone(result);
//    }

//    public async Task<ChatRoomDb> GetFirstAsync(Predicate<ChatRoomDb> selector)
//    {
//        var result = default(ChatRoomDb);

//        await _delegateQueue.InvokeAsync(async () => result = await _roomRepository.GetFirstAsync(selector));
//        return Clone(result);
//    }

//    public async Task<ChatRoomDb> GetOrDefaultAsync(Predicate<ChatRoomDb> selector)
//    {
//        var result = default(ChatRoomDb);

//        await _delegateQueue.InvokeAsync(async () => result = await _roomRepository.GetOrDefaultAsync(selector));
//        return Clone(result);
//    }

//    public async Task<bool> RemoveAsync(Predicate<ChatRoomDb> selector)
//    {
//        var result = default(bool);

//        await _delegateQueue.InvokeAsync(async () => result = await _roomRepository.RemoveAsync(selector));
//        return result;
//    }

//    public async Task<ChatRoomDb> InsertAsync(ChatRoomDb model)
//    {
//        var result = default(ChatRoomDb);

//        await _delegateQueue.InvokeAsync(async () => result = await _roomRepository.InsertAsync(model));
//        return Clone(result);
//    }

//    public ChatRoomDb Insert(ChatRoomDb model)
//    {
//        var result = default(ChatRoomDb);

//        _delegateQueue.Invoke(() => result = _roomRepository.Insert(model));
//        return Clone(result);
//    }

//    public async Task SendMessageAsync(string group, string user, byte[] ptr)
//    {
//        _delegateQueue.Invoke(async () =>
//        {
//            var chatRoom = await GetOrDefaultAsync(x => x.Id == group);

//            if (chatRoom is null)
//            {
//                chatRoom = Insert(new ChatRoomDb { Id = group });
//            }

//            chatRoom.SenderInfo.Add(new SenderInfo(user, ptr));
//        });
//    }

//    private ChatRoomDb Clone(ChatRoomDb db) => mapper.Map<ChatRoomDb>(db);
//    private IEnumerable<ChatRoomDb> Clone(IEnumerable<ChatRoomDb> instances) => mapper.Map<IEnumerable<ChatRoomDb>>(instances);
//}