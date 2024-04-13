﻿namespace Chato.Automation.Infrastructure.Instruction;

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

    public async Task<IEnumerable<InstructionNodeInfo>> MoveNext()
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
               FromArrived = item.FromArrived
            };

            instructions.Add(instructionInfo);
            foreach (var child in item.Children)
            {
                _current.Enqueue(child);
            }
        }

        return instructions;
    }

}
