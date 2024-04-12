namespace Chato.Automation.Infrastructure.Instruction;

public class InstructionGraph
{
    private readonly InstructionNode _root;
    private Queue<InstructionNode> _current;

    public InstructionGraph(InstructionNode root)
    {
        _root = root;   
        _current = new Queue<InstructionNode>();
        Initialize();
    }

    public void Initialize()
    {
        _current.Enqueue(_root);
    }

    public async Task<IEnumerable<InstructionNodeInfo>> Pulse()
    {
        var instructions = new List<InstructionNodeInfo>();

        var items = _current.ToArray();

        _current.Clear();
        foreach (var item in items)
        {
            var instructionInfo = new InstructionNodeInfo
            {
               Instruction = item.Instruction,
               Message = item.Message,
               UserName = item.UserName,
            };

            instructions.Add(instructionInfo);
            foreach (var child in item.Children)
            {
                _current.Enqueue(child);
            }
        }

        return instructions;
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
