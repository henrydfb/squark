
public class EndNode : GrammarNode
{
    public const string ID = "end";

    public EndNode()
        :base(ID,Globals.FIRST_FLOOR,NodeType.Main)
    { 
    }
}
