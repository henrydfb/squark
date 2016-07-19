
public class StartNode : GrammarNode
{
    public const string ID = "start";

    public StartNode()
        : base(ID, Globals.FIRST_FLOOR, NodeType.Main)
    { 
    }
}
