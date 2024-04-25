namespace Chato.Automation.Infrastructure.Instruction;

public class InstructionGraph
{
    private readonly InstructionNode _root;
    private readonly Queue<InstructionNode> _current;

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

    public async Task<IEnumerable<InstructionNode>> MoveNext()
    {
        var instructions = new List<InstructionNode>();

        var items = _current.ToArray();

        _current.Clear();
        foreach (var item in items)
        {
            var instructionInfo = new InstructionNode(item.UserName, item.GroupName, item.Instruction, item.Message, item.FromArrived, item.Children,item.Operation);


            instructions.Add(instructionInfo);
            foreach (var child in item.Children)
            {
                _current.Enqueue(child);
            }
        }

        return instructions;
    }

}
