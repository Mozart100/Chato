using Microsoft.AspNetCore.Localization;

namespace Chato.Automation.Infrastructure.Instruction;

public class InstructionGraph
{
    public class Node
    {
        public Node(UserHubChat user)
        {
            User = user;
            Children = new HashSet<Node>();
        }

        public string UserName => User.Name;

        public UserHubChat User { get; }
        public HashSet<Node> Children { get; }
    }

    private readonly Node _root;
    private int _level;
    private Queue<Node> _current;

    public InstructionGraph()
    {
        _current = new Queue<Node>();
        Initialize();
    }

    public void Initialize()
    {
        _current.Enqueue(_root);
    }

    public async Task Pulse()
    {
        var count = _current.Count;
        var items = _current.ToArray();
        
        _current.Clear();   
        foreach (var item in items)
        {
            await item.User.ExecuteAsync();
            foreach (var child in item.Children)
            {
                _current.Enqueue(child);    
            }
        }
    }
    //public void Pulse_Origin()
    //{

    //    var queue = new Queue<Node>();
    //    queue.Enqueue(_root);


    //    while (queue.Count > 0)
    //    {
    //        var item = queue.Dequeue();
    //        if (item.Name == "xxx")
    //        {
    //            return;
    //        }

    //        foreach (var child in item.Children)
    //        {
    //            queue.Enqueue(child);
    //        }
    //    }
    //}
}
