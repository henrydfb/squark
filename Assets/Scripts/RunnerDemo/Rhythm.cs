using UnityEngine;
using System.Collections;

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
                numberOfActions = 4;
                break;
            case Density.Medium:
                numberOfActions = 6;
                break;
            case Density.High:
                numberOfActions = 11;
                break;
        }

        actions = new Action[numberOfActions];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="moveProb"></param>
    /// <param name="jumpProb"></param>
    public void Build(float moveProb, float jumpProb,float minJump, float maxJump)
    {
        float blockSize, beginTime, endTime;
        Action.Type actionType;
        Action action;

        blockSize = CreateBlockSize();

        for (int i = 0; i < actions.Length; i++)
        {
            actionType = CreateActionType(moveProb, jumpProb);
            beginTime = CreateBeginTime(i, blockSize);
            action = null;
            switch (actionType)
            { 
                case Action.Type.Jump:
                    endTime = Random.Range(minJump, maxJump);
                    action = new Action(actionType, beginTime,endTime);
                    break;
                case Action.Type.Move:
                    action = new Action(actionType, beginTime);
                    break;
            }
            
            actions[i] = action;
        }

        //Set end time for move beats
        for(int i = 0; i < actions.Length;i++)
        {
            if (actions[i].GetType() == Action.Type.Move)
            {
                if (i == actions.Length - 1)
                    endTime = length;
                else
                    endTime = actions[Random.Range(i + 1, actions.Length - 1)].GetBeginTime();
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
    private Action.Type CreateActionType(float moveProb, float jumpProb)
    {
        int actionRand;
        Action.Type actionType;

        //This is for generalization (in case we want to add more actions)
        actionRand = Random.Range(0, 100);
        if (actionRand >= 0 && actionRand < 100 * moveProb)
            actionType = Action.Type.Move;
        else if (actionRand >= 100 * moveProb && actionRand < 100 * (moveProb + jumpProb))
            actionType = Action.Type.Jump;
        else
            actionType = Action.Type.Move;

        return actionType;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Print()
    {
        foreach (Action a in actions)
            a.Print();
    }
}