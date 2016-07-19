using UnityEngine;
using System.Collections.Generic;

class GraphGrammarFactory : MonoBehaviour
{
    public enum AddType
    {
        AddBetween,
        AddSeparate
    }

    public enum Rule
    {
        Add,
        Link
    }

    /// <summary>
    /// Number of blocks per second
    /// </summary>
    public const int BLOCKS_PER_SECOND = 16;

    /// <summary>
    /// Minimum number of seconds in a rhythm
    /// </summary>
    public const int MIN_SECOND = 5;

    /// <summary>
    /// Maximum number of seconds in a rhythm
    /// </summary>
    public const int MAX_SECOND = 30;

    /// <summary>
    /// Minimum number of blocks in a rhythm
    /// </summary>
    public const int MIN_LEVEL_PLATFORM_BLOCKS = BLOCKS_PER_SECOND * MIN_SECOND;

    /// <summary>
    /// Maximum number of blocks in a rhythm
    /// </summary>
    public const int MAX_LEVEL_PLATFORM_BLOCKS = BLOCKS_PER_SECOND * MAX_SECOND;

    public const float PLAT_W = 0.3f;
    public const float PLAT_H = 0.3f;

    public GameObject platformPrefab;
    public GameObject flagPrefab;

    public int InitialNodes;
    public int MaxNumNodes;
    public int MaxNumFloors = 1;
    public int MinNumSepNodes = 1;          //Minimum number of separate nodes
    public int MaxNumSepNodes = 3;          //Maximum number of separate nodes

    public int ReachGapJump = 2;            //Space between platforms to be reachable (in blocks)
    public int UnreachGapJump = 1;          //Space between platforms to be reachable (in blocks)

    public float FloorsDifferenceY = 2.25f;          //Space between platforms to be reachable
    public int MinPlatformBlocks = 20;          //Space between platforms to be reachable

    private int mainNodes;
    private int maxMainPlatformW;
    private Queue<GrammarNode> nodesQueue;
    private List<GrammarNode> linkNodes;
    private StartNode startNode;
    private EndNode endNode;
    private int numOfNodes;
    private Dictionary<int,PlatformController> platforms;

    public void Start()
    {
        GrammarNode lastNode, auxNode;
        Stack<GrammarNode> initialNodes;

        numOfNodes = InitialNodes;
        initialNodes = new Stack<GrammarNode>();
        nodesQueue = new Queue<GrammarNode>();
        linkNodes = new List<GrammarNode>();
        platforms = new Dictionary<int, PlatformController>();

        startNode = new StartNode();
        endNode = new EndNode();
        
        mainNodes = 0;              //We are not including Start and End, if you wanna do that initialize: 2
        lastNode = endNode;
        for (int i = 0; i < InitialNodes; i++)
        {
            auxNode = new GrammarNode((InitialNodes - i).ToString(), Globals.FIRST_FLOOR, GrammarNode.NodeType.Main);
            auxNode.AddForwardBackwardNode(lastNode);
            initialNodes.Push(auxNode);
            lastNode.SetPredecessor(auxNode);
            lastNode = auxNode;
            mainNodes++;
        }

        startNode.AddForwardBackwardNode(lastNode);
        lastNode.SetPredecessor(startNode);
        
        //Enqueuing nodes
        while (initialNodes.Count > 0)
            nodesQueue.Enqueue(initialNodes.Pop());
        
        //Creating the map
        GenerateTopologicalMap();

        //Print Graph
        //startNode.Print(new List<GrammarNode>());

        maxMainPlatformW = MAX_LEVEL_PLATFORM_BLOCKS / mainNodes;
        //mainNodes = startNode.GetNumberOfMainNodes();
        //Debug.Log("MAIN PLT W: " + *(MAX_LEVEL_PLATFORM_BLOCKS/mainNodes) - ((mainNodes + 1) * UnreachGapJump)));
        //Debug.Log("Nodes 1: " + startNode.GetNumberOfMainNodes());
        //Debug.Log("Nodes 2 plat: " + maxMainPlatformW);
        GenerateGraphicalMap(startNode, new List<GrammarNode>());
        //PrintQueue();
    }

    public void GeneratePlatform(GrammarNode node)
    {
        Vector3 pos, prevPos;
        GameObject platform;
        PlatformController platformCont;
        List<GrammarNode> successors;
        int predNumBlocks, curNumBlocks;

        pos = new Vector3();
        prevPos = new Vector3();
        predNumBlocks = 0;
        if (node.GetPredecessor() != null)
        {
            if (node.GetPredecessor().GetPlatform() != null)
            {
                prevPos = node.GetPredecessor().GetPlatform().transform.position;
                predNumBlocks = node.GetPredecessor().GetPlatform().GetPieces().Count;
            }
            else
                Debug.Log("aja: " + node.GetID());
        }
        else
            Debug.Log("aja2: " + node.GetID());

        switch (node.GetType())
        { 
            case GrammarNode.NodeType.Main:
                //Start
                if (node.GetID() == StartNode.ID)
                {
                    pos = new Vector3(0, 0, 0);

                    platform = (GameObject)Instantiate(platformPrefab, pos, Quaternion.identity);
                    platformCont = platform.GetComponent<PlatformController>();
                    platformCont.Contruct(MinPlatformBlocks);
                    node.SetPlatform(platformCont);

                    Debug.Log("start");
                }
                //End
                else if (node.GetID() == EndNode.ID)
                {
                    pos = new Vector3(prevPos.x + (predNumBlocks * PLAT_W) / 2 + (MinPlatformBlocks * PLAT_W) / 2 + (ReachGapJump * PLAT_W), prevPos.y, 0);

                    platform = (GameObject)Instantiate(platformPrefab, pos, Quaternion.identity);
                    platformCont = platform.GetComponent<PlatformController>();
                    platformCont.Contruct(MinPlatformBlocks);
                    node.SetPlatform(platformCont);

                    Instantiate(flagPrefab, new Vector3(pos.x,pos.y + 2), Quaternion.identity);

                    Debug.Log("end");
                }
                //Middle
                else
                {
                    pos = new Vector3(prevPos.x + (predNumBlocks * PLAT_W) / 2 + (maxMainPlatformW * PLAT_W) / 2 + (ReachGapJump * PLAT_W), prevPos.y, 0);

                    platform = (GameObject)Instantiate(platformPrefab, pos, Quaternion.identity);
                    platformCont = platform.GetComponent<PlatformController>();
                    platformCont.Contruct(maxMainPlatformW);
                    node.SetPlatform(platformCont);

                    Debug.Log("Node: " + node.GetID() + " floor: " + node.GetFloor());

                    //foreach(GrammarNode n in node.GetNextFloorNodes())
                    //    Debug.Log("Succ: " + n.GetID() + " floor: " + n.GetFloor() + " pred: " + n.GetPredecessor().GetID());
                    //Successors
                    //Double link
                    foreach (GrammarNode n in node.GetNextFloorNodes())
                        GeneratePlatform(n);
                }
                break;
            default:

                successors = node.GetPredecessor().GetNextFloorNodes();

                if(successors.Count > 0)
                {
                    curNumBlocks = Mathf.RoundToInt(predNumBlocks / successors.Count) - ((successors.Count + 1) * ReachGapJump);//Random.Range(MinPlatformBlocks, Mathf.RoundToInt(predNumBlocks / successors.Count)) - (successors.Count + 1) * ReachGapJump;

                    //Debug.Log("suucccc: " + successors.Count + "  pred blocks: " + predNumBlocks + "  CURRR: " + curNumBlocks);
                        
                    switch (node.GetType())
                    { 
                        case GrammarNode.NodeType.Left:
                            pos = new Vector3((prevPos.x + (ReachGapJump * PLAT_W) - (predNumBlocks * PLAT_W) / 2 + (curNumBlocks * PLAT_W)/2) , prevPos.y + FloorsDifferenceY);
                            break;
                        case GrammarNode.NodeType.Right:
                            pos = new Vector3((prevPos.x - (ReachGapJump * PLAT_W) + (predNumBlocks * PLAT_W) / 2 - (curNumBlocks * PLAT_W)/2) , prevPos.y + FloorsDifferenceY);
                            break;
                        case GrammarNode.NodeType.Center:
                            pos = new Vector3(prevPos.x, prevPos.y + FloorsDifferenceY);
                            break;
                        case GrammarNode.NodeType.LeftRight:
                            pos = new Vector3(prevPos.x, prevPos.y + FloorsDifferenceY);
                            break;
                        default:
                            Debug.Log("PROBLEMA " + node.GetType());
                            break;
                    }

                    platform = (GameObject)Instantiate(platformPrefab, pos, Quaternion.identity);
                    platformCont = platform.GetComponent<PlatformController>();
                    platformCont.Contruct(curNumBlocks);
                    node.SetPlatform(platformCont);

                    //Connection platform
                    platform = (GameObject)Instantiate(platformPrefab, pos + new Vector3((curNumBlocks + 3 + ReachGapJump) * PLAT_W / 2, -(FloorsDifferenceY / 2f)), Quaternion.identity);
                    platformCont = platform.GetComponent<PlatformController>();
                    platformCont.Contruct(3);
                    node.SetPlatform(platformCont);
                }

                //Successors
                //Double link
                foreach (GrammarNode n in node.GetNextFloorNodes())
                {
                    if(n.GetFloor() > node.GetFloor())
                        GeneratePlatform(n);
                }
                
                break;

        }

        //prev

        
    }

    public void GenerateGraphicalMap(GrammarNode node, List<GrammarNode> generatedNodes)
    {
        if(!generatedNodes.Contains(node))
        {
            generatedNodes.Add(node);
            GeneratePlatform(node);

            if(node.GetSameFloorNode() != null)
                GenerateGraphicalMap(node.GetSameFloorNode(), generatedNodes);
            //Double link
            //foreach (GrammarNode n in node.GetForwardBackwardNodes())
              //  GenerateGraphicalMap(n, generatedNodes);

            //Single link
            //foreach (GrammarNode n in node.GetForwardNodes())
             //   GenerateGraphicalMap(n, generatedNodes);
        }
    }

    public void PrintQueue()
    {
        foreach (GrammarNode n in nodesQueue)
            Debug.Log("id: " + n.GetID());
    }

    public void GenerateTopologicalMap()
    {
        CreateGraph();
        LinkGraph();
    }

    //Create the graph (only creation)
    public void CreateGraph()
    {
        GrammarNode q;
        Rule rule;

        while (nodesQueue.Count > 0)
        {
            q = nodesQueue.Dequeue();

            //Is it possible to use the Add Rule?
            if (numOfNodes < MaxNumNodes)
            {
                //If this is the first floor, these are the only rules we can use
                if (q.GetFloor() == Globals.FIRST_FLOOR)
                {
                    //If this node has successors, we can use both add rules
                    if (q.GetForwardBackwardNodes().Count > 0)
                    {
                        //If we can add more new floors
                        if (MaxNumFloors > 1)
                        {
                            if (Random.Range(0, 100) > 50)
                                RuleAddBetween(q, GrammarNode.NodeType.Main);
                            else
                            {
                                //Check that new new node stays in range
                                if (q.GetFloor() + 1 < Mathf.Abs(MaxNumFloors) && q.GetFloor() - 1 > -Mathf.Abs(MaxNumFloors))
                                    AddSeparateNodes(q);
                            }
                        }
                        else
                            RuleAddBetween(q, GrammarNode.NodeType.Main);
                    }
                    else
                    {
                        if (MaxNumFloors > 1)
                        {
                            if (q.GetFloor() + 1 < Mathf.Abs(MaxNumFloors) && q.GetFloor() - 1 > -Mathf.Abs(MaxNumFloors))
                                AddSeparateNodes(q);
                        }
                    }
                }
                //Above Path
                else if (q.GetFloor() > Globals.FIRST_FLOOR)
                {
                    rule = Random.Range(0, 100) > 50 ? Rule.Add : Rule.Link;

                    //If this node has successors, we can use both add rules
                    if (q.GetForwardBackwardNodes().Count > 0)
                    {
                        Debug.Log("NO ES POSIBLE");
                        if(rule == Rule.Add)
                        {
                            //If we can add more new floors
                            if (MaxNumFloors > 1)
                            {
                                if (q.GetFloor() + 1 < Mathf.Abs(MaxNumFloors))
                                    AddSeparateNodes(q);
                                else
                                    linkNodes.Add(q);
                            }
                        }
                        else if (rule == Rule.Link)
                            linkNodes.Add(q);
                    }
                    else
                    {
                        if (rule == Rule.Add)
                        {
                            //If we can add more new floors
                            if (MaxNumFloors > 1)
                            {
                                if (q.GetFloor() + 1 < Mathf.Abs(MaxNumFloors))
                                    AddSeparateNodes(q);
                                else
                                    linkNodes.Add(q);
                            }
                        }
                        else if (rule == Rule.Link)
                            linkNodes.Add(q);
                    }
                }
                //Below Path
                else
                {
                    rule = Random.Range(0, 100) > 50 ? Rule.Add : Rule.Link;

                    //If this node has successors, we can use both add rules
                    if (q.GetForwardBackwardNodes().Count > 0)
                    {
                        Debug.Log("NO ES POSIBLE");
                        if (rule == Rule.Add)
                        {
                            //If we can add more new floors
                            if (MaxNumFloors > 1)
                            {
                                if (q.GetFloor() - 1 > -Mathf.Abs(MaxNumFloors))
                                    AddSeparateNodes(q);
                                else
                                    linkNodes.Add(q);
                            }
                        }
                        else if (rule == Rule.Link)
                            linkNodes.Add(q);
                    }
                    else
                    {
                        if (rule == Rule.Add)
                        {
                            //If we can add more new floors
                            if (MaxNumFloors > 1)
                            {
                                if (q.GetFloor() - 1 > -Mathf.Abs(MaxNumFloors))
                                    AddSeparateNodes(q);
                                else
                                    linkNodes.Add(q);   
                            }
                        }
                        else if (rule == Rule.Link)
                            linkNodes.Add(q);
                    }
                }
            }
            else
            {
                //The rest of the rules
                if (q.GetFloor() != Globals.FIRST_FLOOR)
                    linkNodes.Add(q);
            }
        }
    }

    public void LinkGraph()
    {
        List<GrammarNode> leftMostNeighbors, rightMostNeighbors;

        foreach (GrammarNode n in linkNodes)
        {
            switch (n.GetType())
            { 
                //Add closest neighbor
                case GrammarNode.NodeType.Center:
                    //Double connection
                    if (Random.Range(0, 100) > 50)
                        n.AddForwardBackwardNode(n.GetNeighbor());
                    //Single connection
                    else
                        n.AddForwardNode(n.GetNeighbor());

                        break;
                case GrammarNode.NodeType.Left:
                        leftMostNeighbors = n.GetLeftMostTopBottomNeighbors(n);
                        foreach (GrammarNode l in leftMostNeighbors)
                        {
                            if (Mathf.Abs(n.GetFloor() - l.GetFloor()) <= 1)
                            {
                                //Double connection
                                if (Random.Range(0, 100) > 50)
                                    n.AddForwardBackwardNode(l);
                                //Single connection
                                else
                                    n.AddForwardNode(l);
                            }
                            else
                                n.AddForwardNode(l);
                        }
                    break;
                case GrammarNode.NodeType.Right:
                    rightMostNeighbors = n.GetRightMostTopBottomNeighbors(n);
                    foreach (GrammarNode r in rightMostNeighbors)
                    {
                        if (Mathf.Abs(n.GetFloor() - r.GetFloor()) <= 1)
                        {
                            //Double connection
                            if (Random.Range(0, 100) > 50)
                                n.AddForwardBackwardNode(r);
                            //Single connection
                            else
                                n.AddForwardNode(r);
                        }
                        else
                            n.AddForwardNode(r);
                    }
                    break;
                case GrammarNode.NodeType.LeftRight:
                    leftMostNeighbors = n.GetLeftMostTopBottomNeighbors(n);
                    rightMostNeighbors = n.GetRightMostTopBottomNeighbors(n);

                    foreach (GrammarNode l in leftMostNeighbors)
                    {
                        if (Mathf.Abs(n.GetFloor() - l.GetFloor()) <= 1)
                        {
                            //Double connection
                            if (Random.Range(0, 100) > 50)
                                n.AddForwardBackwardNode(l);
                            //Single connection
                            else
                                n.AddForwardNode(l);
                        }
                        else
                            n.AddForwardNode(l);
                    }

                    foreach (GrammarNode r in rightMostNeighbors)
                    {
                        if (Mathf.Abs(n.GetFloor() - r.GetFloor()) <= 1)
                        {
                            //Double connection
                            if (Random.Range(0, 100) > 50)
                                n.AddForwardBackwardNode(r);
                            //Single connection
                            else
                                n.AddForwardNode(r);
                        }
                        else
                            n.AddForwardNode(r);
                    }
                    break;
            }

        }
    }

    private void AddSeparateNodes(GrammarNode node)
    {
        int separateNodes;
        GrammarNode newNode, prevNode;

        prevNode = null;
        separateNodes = Random.Range(MinNumSepNodes, MaxNumSepNodes);

        //Check that the number of nodes stays less than the max
        if (numOfNodes + separateNodes >= MaxNumNodes)
            separateNodes = MaxNumNodes - numOfNodes - 1;

        for (int i = 0; i <= separateNodes; i++)
        {
            if (i == 0 && i == separateNodes)
            {
                newNode = RuleAddSeparate(node, GrammarNode.NodeType.LeftRight);
                //Left most Node
                node.SetLeftMost(newNode);
                //Right most Node
                node.SetRightMost(newNode);
            }
            else if (i == 0)
            {
                newNode = RuleAddSeparate(node, GrammarNode.NodeType.Left);
                //Left most Node
                node.SetLeftMost(newNode);
            }
            else if (i == separateNodes)
            {
                newNode = RuleAddSeparate(node, GrammarNode.NodeType.Right);
                //Right most Node
                node.SetRightMost(newNode);
            }
            else
                newNode = RuleAddSeparate(node, GrammarNode.NodeType.Center);

            //Adding neighbors for each node
            if (prevNode != null)
                prevNode.AddNeighbor(newNode);

            newNode.SetOrder(i);
            prevNode = newNode;    
        }
    }

    public void RuleLink(GrammarNode node)
    {
        //Debug.Log("Link: " + node.GetID());
    }

    //Adding bewteen nodes 1 ->[4] -> 2 -> 3 
    public void RuleAddBetween(GrammarNode node, GrammarNode.NodeType type)
    {
        GrammarNode newNode;

        numOfNodes++;

        newNode = new GrammarNode(numOfNodes.ToString(), node.GetFloor(),type);
        newNode.SetPredecessor(node);
        node.AddNodeBetween(newNode);
                
        nodesQueue.Enqueue(newNode);

        mainNodes++;
    }

    //Adding separately 1 -> 2 -> 3 -> [4]
    public GrammarNode RuleAddSeparate(GrammarNode node, GrammarNode.NodeType type)
    {
        GrammarNode newNode;
        int newFloor;

        numOfNodes++;
        newFloor = node.GetFloor();
        
        //We can create new floors above or below the main path
        if (node.GetFloor() == Globals.FIRST_FLOOR)
            newFloor = Random.Range(0, 100) > 50 ? node.GetFloor() + 1 : node.GetFloor() + 1;
            //TODO: descomentar luego
            //newFloor = Random.Range(0, 100) > 50 ? node.GetFloor() + 1 : node.GetFloor() - 1;
        else if (node.GetFloor() > Globals.FIRST_FLOOR)
            newFloor = node.GetFloor() + 1;
        else
            newFloor = node.GetFloor() + 1;
        //TODO: descomentar luego
            //newFloor = node.GetFloor() - 1;
                
        newNode = new GrammarNode(numOfNodes.ToString(), newFloor,type);
        newNode.SetPredecessor(node);
        node.AddForwardBackwardNode(newNode);

        nodesQueue.Enqueue(newNode);

        return newNode;
    }
}
