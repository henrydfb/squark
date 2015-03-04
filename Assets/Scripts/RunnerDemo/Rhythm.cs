using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class Rhythm
{

    public enum Type
    { 
        Regular,    //Evenly spaced beats
        Swing,      //Short followed by long beats
        Random      //Randomly spaced beats
    }

    public enum Density
    { 
        Low,        //Four actions
        Medium,     //Six actions
        High        //Eleven actions
    }

    /// <summary>
    /// 
    /// </summary>
    private Type type;

    /// <summary>
    /// 
    /// </summary>
    private Density density;

    /// <summary>
    /// 
    /// </summary>
    private float length;

    /// <summary>
    /// 
    /// </summary>
    private Action[] actions;       //Beats in a rhythm

    /// <summary>
    /// This is used to assure that moving actions do not overlap
    /// </summary>
    private int maxMoveEndPos;

    /// <summary>
    /// 
    /// </summary>
    private List<Geometry> geometries;

    public Rhythm(Type type, Density density, float length)
    {
        int numberOfActions;

        this.type = type;
        this.density = density;
        this.length = length;

        numberOfActions = 0;

        //Choosing how many actions the rhythm has, depending on the density
        switch(density)
        {
            case Density.Low:
                numberOfActions = ((int)length/3);
                break;
            case Density.Medium:
                numberOfActions = ((int)length * 2 / 3);
                break;
            case Density.High:
                numberOfActions = (int)length;
                break;
        }

        actions = new Action[numberOfActions];
        geometries = new List<Geometry>();
        maxMoveEndPos = 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="moveProb"></param>
    /// <param name="jumpProb"></param>
    public void Build(float minJump, float maxJump)
    {
        float blockSize, beginTime, endTime;
        int jumpType;
        string actionType;
        Action action;
        Move moveAction;
        Jump.HeightType heightType;

        blockSize = CreateBlockSize();
        Debug.Log(blockSize);
        Debug.Log("actions" + actions.Length);

        for (int i = 0; i < actions.Length; i++)
        {
            beginTime = CreateBeginTime(i, blockSize);

            actionType = CreateActionType(i);

            action = null;
            switch (actionType)
            { 
                case Jump.TYPE:
                    jumpType = Random.Range(0,100);
                    
                    //Short Jump
                    if(jumpType >= 0 && jumpType < 100/3)
                    {
                        heightType = Jump.HeightType.Short;
                        endTime = minJump;
                    }
                    //Medium
                    else if(jumpType >= 100/3 && jumpType < 2 * (100/3))
                    {
                        heightType = Jump.HeightType.Medium;
                        endTime = (maxJump - minJump)/2;
                    }
                    //Long
                    else
                    {
                        heightType = Jump.HeightType.Long;
                        endTime = maxJump;
                    }

                    action = new Jump(beginTime,endTime,heightType);
                    break;
                case Move.TYPE:
                    maxMoveEndPos = Random.Range(i + 1, actions.Length + 1);
                    action = new Move(beginTime, maxMoveEndPos);
                    break;
            }
            
            actions[i] = action;
        }

        //Set end time for move beats
        //We do this after the whole rythm is build because we need to look ahead in time to get the end time for the "move" action
        for(int i = 0; i < actions.Length;i++)
        {
            if (actions[i].GetType() == Move.TYPE)
            {
                moveAction = (Move)actions[i];

                if (i == actions.Length - 1)
                    endTime = length;
                else
                {
                    if(moveAction.GetEndTimePosition() == actions.Length)
                        endTime = length;
                    else
                        endTime = actions[moveAction.GetEndTimePosition()].GetBeginTime();
                }

                actions[i].SetEndTime(endTime);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="beatNumber"></param>
    /// <param name="blockSize"></param>
    /// <returns></returns>
    private float CreateBeginTime(int beatNumber, float blockSize)
    {
        float beginTime;

        beginTime = 0;
        
        switch (type)
        {
            case Type.Regular:
                beginTime = beatNumber * blockSize;
                break;
            case Type.Swing:
                //Long beat
                if (beatNumber % 2 == 0)
                    beginTime = (beatNumber / 2 * blockSize);
                //Short beat
                else
                    beginTime = (beatNumber / 2 * blockSize) + (blockSize * 0.7f);
                break;
            case Type.Random:
                if (beatNumber == 0)
                    beginTime = 0;
                else
                    beginTime = Random.Range((beatNumber * blockSize), (beatNumber * blockSize) + blockSize);
                break;
        }

        return beginTime;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private float CreateBlockSize()
    {
        float blockSize;

        blockSize = 0;

        switch (type)
        {
            case Type.Regular:
                blockSize = length / actions.Length;
                break;
            case Type.Swing:
                blockSize = length / (actions.Length / 2);
                break;
            case Type.Random:
                blockSize = length / actions.Length;
                break;
        }

        return blockSize;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="moveProb"></param>
    /// <param name="jumpProb"></param>
    /// <returns></returns>
    private string CreateActionType(int currPos)
    {
        string actionType;

        if (currPos == 0)
            actionType = Move.TYPE;
        else
        {
            //Check for move action overlapping
            if (currPos >= maxMoveEndPos)
                actionType = Move.TYPE;
            else
                actionType = Jump.TYPE;
        }

        return actionType;
    }

    /// <summary>
    /// 
    /// </summary>
    public string GetPrint()
    {
        string print;

        print = "[";
        foreach (Action a in actions)
            print += a.GetPrintAction();

        print += "]";

        return print;
        //Debug.Log(print);
            //a.Print();
    }

    /// <summary>
    /// 
    /// </summary>
    public void GetPrintGeometries()
    {
        //foreach (Geometry g in geometries)
          //  print(g.GetPrint());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Action[] GetActions()
    {
        return actions;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public List<Geometry> GetGeometries()
    {
        return geometries;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="geometry"></param>
    public void AddGeometry(Geometry geometry)
    {
        geometries.Add(geometry);
    }

    /// <summary>
    /// 
    /// </summary>
    public void ConstructLevel()
    {
        Geometry geometry;

        //We have to decide how to handle this
        geometry = geometries[0];


    }
}