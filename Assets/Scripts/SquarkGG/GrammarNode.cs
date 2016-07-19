using System.Collections.Generic;
using UnityEngine;
using System;

public class GrammarNode
{
    public enum NodeType
    { 
        Main,
        Left,
        Right,
        LeftRight,
        Center
    }

    private string id;
    private int floor;
    private List<GrammarNode> forwardNodes;             //Only forward
    private List<GrammarNode> forwardBackwardNodes;     //Both directions
    private GrammarNode neighbor;                       //Nodes in the same floor connected through the main predecessor
    private NodeType type;
    private GrammarNode mainPredecessor;
    private GrammarNode leftMost;
    private GrammarNode rightMost;
    private int order;                          //Successors order (for non-main nodes)
    private PlatformController platform;

    public GrammarNode(string id, int floor, NodeType type)
    {
        this.id = id;
        this.floor = floor;
        this.type = type;
        mainPredecessor = null;
        forwardNodes = new List<GrammarNode>();
        forwardBackwardNodes = new List<GrammarNode>();
    }

    
    public void SetOrder(int order)
    {
        this.order = order;
    }

    public int GetOrder()
    {
        return order;
    }

    public void SetPlatform(PlatformController platform)
    {
        this.platform = platform;
    }

    public PlatformController GetPlatform()
    {
        return platform;
    }

    public void SetPredecessor(GrammarNode node)
    {
        this.mainPredecessor = node;
    }

    public void SetLeftMost(GrammarNode node)
    {
        this.leftMost = node;
    }

    public void SetRightMost(GrammarNode node)
    {
        this.rightMost = node;
    }

    public GrammarNode GetPredecessor()
    {
        return mainPredecessor;
    }

    public GrammarNode GetLeftMost()
    {
        return leftMost;
    }

    public GrammarNode GetRightMost()
    {
        return rightMost;
    }

    public List<GrammarNode> GetRightMostBottomTopNeighbors(GrammarNode baseNode)
    {
        List<GrammarNode> neighbors, auxNeighbors;
        
        neighbors = new List<GrammarNode>();
        neighbors.Add(this);
        if (rightMost != null)
        {
            auxNeighbors = rightMost.GetRightMostBottomTopNeighbors(baseNode);
            foreach (GrammarNode n in auxNeighbors)
                neighbors.Add(n);
        }
        
        return neighbors;
    }

    public List<GrammarNode> GetLeftMostBottomTopNeighbors(GrammarNode baseNode)
    {
        List<GrammarNode> neighbors, auxNeighbors;

        neighbors = new List<GrammarNode>();
        neighbors.Add(this);
        
        if (leftMost != null)
        {
            auxNeighbors = leftMost.GetLeftMostBottomTopNeighbors(baseNode);
            foreach (GrammarNode n in auxNeighbors)
                neighbors.Add(n);
        }
       
        return neighbors;
    }

    public List<GrammarNode> GetLeftMostTopBottomNeighbors(GrammarNode baseNode)
    { 
        List<GrammarNode> neighbors, auxNeighbors;
        
        neighbors = new List<GrammarNode>();
        
        if (mainPredecessor != null)
        {
            //Adding the predecessor if it's not connected directly to the base node
            if (mainPredecessor != baseNode.mainPredecessor)
            {
                //Adding predecessor
                AddNodeToNeighbors(baseNode, mainPredecessor, neighbors);
                
                //Adding the left most node
                AddNodeToNeighbors(baseNode, mainPredecessor.leftMost, neighbors);
            }

            //Main node
            if (mainPredecessor.type == NodeType.Main)
            {
                //In the main path from the base node to the base, add the predecessor of that main node
                //Get the right most nodes for that node
                if (mainPredecessor.mainPredecessor != null)
                {
                    auxNeighbors = mainPredecessor.mainPredecessor.GetRightMostBottomTopNeighbors(baseNode);
                    foreach (GrammarNode n in auxNeighbors)
                        AddNodeToNeighbors(baseNode, n, neighbors);
                }
            }
            else
            {
                auxNeighbors = mainPredecessor.GetLeftMostTopBottomNeighbors(baseNode);
                foreach (GrammarNode n in auxNeighbors)
                    AddNodeToNeighbors(baseNode, n, neighbors);
            }
        }

        return neighbors;
    }

    private void AddNodeToNeighbors(GrammarNode baseNode,GrammarNode node, List<GrammarNode> neighbors)
    {
        if (node != null)
        {
            if (!neighbors.Contains(node) && node != baseNode.mainPredecessor)
            {
                //Main path
                if (baseNode.floor == Globals.FIRST_FLOOR)
                    neighbors.Add(node);
                //Above
                else if (baseNode.floor > Globals.FIRST_FLOOR)
                {
                    if (node.floor <= baseNode.floor)
                        neighbors.Add(node);
                }
                //Below
                else
                {
                    if (node.floor >= baseNode.floor)
                        neighbors.Add(node);
                }
            }
        }
    }

    public List<GrammarNode> GetRightMostTopBottomNeighbors(GrammarNode baseNode)
    {
        List<GrammarNode> neighbors, auxNeighbors;
        
        neighbors = new List<GrammarNode>();
        
        if (mainPredecessor != null)
        {
            //Adding the predecessor if it's not connected directly to the base node
            if (mainPredecessor != baseNode.mainPredecessor)
            {
                //Adding predecessor
                AddNodeToNeighbors(baseNode, mainPredecessor, neighbors);

                //Adding the right most node
                AddNodeToNeighbors(baseNode, mainPredecessor.rightMost, neighbors);
            }

            //Main node
            if (mainPredecessor.type == NodeType.Main)
            {
                //In the main path from the base node to the base, add the successor of that main node
                //Get the left most nodes for that node
                if (mainPredecessor.GetSameFloorNode() != null)
                {
                    auxNeighbors = mainPredecessor.GetSameFloorNode().GetLeftMostBottomTopNeighbors(baseNode);
                    foreach (GrammarNode n in auxNeighbors)
                        AddNodeToNeighbors(baseNode, n, neighbors);
                }
            }
            else
            {
                auxNeighbors = mainPredecessor.GetRightMostTopBottomNeighbors(baseNode);
                foreach (GrammarNode n in auxNeighbors)
                    AddNodeToNeighbors(baseNode, n, neighbors);
            }
        }

        return neighbors;
    }

    public void AddNeighbor(GrammarNode node)
    {
        neighbor = node;
    }

    public GrammarNode GetNeighbor()
    {
        return neighbor;
    }

    public NodeType GetType()
    {
        return type;
    }

    public string GetID()
    {
        return id;
    }

    public int GetFloor()
    {
        return floor;
    }

    public List<GrammarNode> GetForwardBackwardNodes()
    {
        return forwardBackwardNodes;
    }

    public List<GrammarNode> GetForwardNodes()
    {
        return forwardNodes;
    }

    public void AddForwardBackwardNode(GrammarNode node) 
    {
        forwardBackwardNodes.Add(node);
    }

    public void AddForwardNode(GrammarNode node)
    {
        forwardNodes.Add(node);
    }

    public void Print(List<GrammarNode> printedNodes)
    {
        string nodes;
        Debug.Log("\nid: " + id + " floor: " + floor + " type: " + type);
        if (forwardBackwardNodes.Count > 0)
        {
            nodes = "";
            foreach (GrammarNode n in forwardBackwardNodes)
                nodes += "(" + n.GetID() + "," + n.GetFloor() + ") ";

            Debug.Log("Double connection");
            Debug.Log(nodes);
        }

        if (forwardNodes.Count > 0)
        {
            nodes = "";
            foreach (GrammarNode n in forwardNodes)
                nodes += "(" + n.GetID() + "," + n.GetFloor() + ") ";

            Debug.Log("Single connection");
            Debug.Log(nodes);
        }
        
        Debug.Log("**************************");

        printedNodes.Add(this);

        //Double connection nodes
        foreach (GrammarNode n in forwardBackwardNodes)
        {
            if(!printedNodes.Contains(n))
                n.Print(printedNodes);
        }

        //Single connection nodes
        foreach (GrammarNode n in forwardNodes)
        {
            if (!printedNodes.Contains(n))
                n.Print(printedNodes);
        }
    }

    public void AddNodeBetween(GrammarNode newNode)
    {
        GrammarNode sameFloorNode;
        
        //Find the next node (node linked to this node)
        sameFloorNode = GetSameFloorNode();

        //Connect this node to the previous node and the new node to the next node (of this one)
        if (sameFloorNode != null)
        {
            newNode.AddForwardBackwardNode(sameFloorNode);
            forwardBackwardNodes.Remove(sameFloorNode);
            sameFloorNode.SetPredecessor(newNode);

            AddForwardBackwardNode(newNode);
            newNode.SetPredecessor(this);
        }
    }

    public List<GrammarNode> GetNextFloorNodes()
    {
        Predicate<GrammarNode> predicate;

        predicate = (GrammarNode n) => n.floor == (floor + 1);

        return forwardBackwardNodes.FindAll(predicate);
    }

    public GrammarNode GetSameFloorNode()
    {
        Predicate<GrammarNode> predicate;

        predicate = (GrammarNode n) => n.floor == floor;

        return forwardBackwardNodes.Find(predicate);
    }

    public int GetNumberOfMainNodes()
    {
        int number;
        GrammarNode next;

        next = GetSameFloorNode();

        if (next == null)
            number = 1;
        else
            number = 1 + next.GetNumberOfMainNodes();

        return number;
    }
}
