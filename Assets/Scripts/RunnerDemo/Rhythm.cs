using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class Rhythm
{
    /// <summary>
    /// 
    /// </summary>
    private int length;

    /// <summary>
    /// 
    /// </summary>
    private Action[] actions;       //Beats in a rhythm

    private float time;

    /// <summary>
    /// This is used to assure that moving actions do not overlap
    /// </summary>
    private int maxMoveEndPos;

    private int lowActNumber;
    private int medActNumber;
    private int higActNumber;

    private float lowActRate;
    private float medActRate;
    private float higActRate;

    private float globalPerformance;

    private float difficultyPercentage;

    /// <summary>
    /// 
    /// </summary>
    private List<Geometry> geometries;

    public Rhythm()
    {
        geometries = new List<Geometry>();
        maxMoveEndPos = 0;
    }

    public float GetGlobalPerformance()
    {
        return globalPerformance;
    }

    public float GetDifficulty()
    {
        return difficultyPercentage;
    }

    public int GetLowActionNumber()
    {
        return lowActNumber;
    }

    public int GetMedActionNumber()
    {
        return medActNumber;
    }

    public int GetHigActionNumber()
    {
        return higActNumber;
    }

    public float GetLowActionRate()
    {
        return lowActRate;
    }

    public float GetMedActionRate()
    {
        return medActRate;
    }

    public float GetHigActionRate()
    {
        return higActRate;
    }

    public int GetLength()
    {
        return length;
    }

    public float GetTime()
    {
        return time;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="previousRhythm"></param>
    /// <param name="currentGlobalPerformance">This performance is from 0 - 100</param>
    /// <param name="attentionMatrix"></param>
    /// <param name="deathMatrix"></param>
    /// <param name="level"></param>
    public void Initialize(Rhythm previousRhythm, float currentGlobalPerformance,List<float>[] attentionMatrix,int[] deathMatrix, int level)
    {
        const float INITIAL_RATE = 0.5f;    //Initial difficulty
        const float STEP = 0.125f;
        const float W1 = 0.5f;
        const float W2 = 0.5f;
        int numberOfActions, ran;
        float perVar, lowAvg, medAvg, higAvg, lowDeath,medDeath,higDeath, low, med, hig, attSum, lengthFactor,auxLength, auxMaxTime;

        if (previousRhythm == null)
        {
            difficultyPercentage = INITIAL_RATE;
            currentGlobalPerformance = difficultyPercentage * 100;  // Performance is calculated from 0 to 100
            length = (int)(RhythmFactory.MAX_LEVEL_PLATFORM_BLOCKS * difficultyPercentage);
            lengthFactor = (float)length / (float)RhythmFactory.MAX_LEVEL_PLATFORM_BLOCKS;
            time = (float)RhythmFactory.MAX_SECOND * difficultyPercentage;
            numberOfActions = (int)(RhythmFactory.MAX_NUMBER_OF_ACTIONS * lengthFactor * difficultyPercentage);
            lowActRate = 1.0f/3.0f;
            medActRate = 1.0f/3.0f;
            higActRate = 1.0f/3.0f;
        }
        else
        {
            //Percentage variation
            if ((currentGlobalPerformance / 100) > previousRhythm.GetGlobalPerformance() / 100)
            {
                //Method 1
                if(Globals.METHOD == "1")
                    perVar = (STEP * Mathf.Abs(currentGlobalPerformance)) / 100;
                else
                //Method 2
                perVar = Mathf.Abs((currentGlobalPerformance / 100) - previousRhythm.GetDifficulty()); //THIS IS SOMETHING NEW WE SHOULD TRY
            }
            else
            {
                //Method 1
                if (Globals.METHOD == "1")
                    perVar = -(STEP * Mathf.Abs(100 - currentGlobalPerformance)) / 100;
                else
                //Method 2
                    perVar = -Mathf.Abs((currentGlobalPerformance / 100) - previousRhythm.GetDifficulty()); //THIS IS SOMETHING NEW WE SHOULD TRY
            }

            difficultyPercentage = (previousRhythm.GetDifficulty() + perVar);

            auxMaxTime = RhythmFactory.MIN_SECOND + RhythmFactory.MAX_SECOND - previousRhythm.GetTime();
            time = previousRhythm.GetTime() + auxMaxTime * perVar;
            length = (int)(time * RhythmFactory.BLOCKS_PER_SECOND);
            lengthFactor = (float)length / (float)RhythmFactory.MAX_LEVEL_PLATFORM_BLOCKS;
            auxLength = (RhythmFactory.MIN_NUMBER_OF_ACTIONS + (RhythmFactory.MAX_NUMBER_OF_ACTIONS - RhythmFactory.MIN_NUMBER_OF_ACTIONS)) * lengthFactor;
            numberOfActions = (int)(auxLength * difficultyPercentage);

            lowDeath = 100 / (1 + deathMatrix[(int)GameController.DeathType.Low]);
            medDeath = 100 / (1 + deathMatrix[(int)GameController.DeathType.Med]);
            higDeath = 100 / (1 + deathMatrix[(int)GameController.DeathType.Hig]);
            lowAvg = GetAttentionAvg(attentionMatrix, GameController.AttentionType.Low);
            medAvg = GetAttentionAvg(attentionMatrix, GameController.AttentionType.Med);
            higAvg = GetAttentionAvg(attentionMatrix, GameController.AttentionType.Hig);

            low = (lowAvg * W1 + lowDeath * W2)/3;
            med = (medAvg * W1 + medDeath * W2)/3;
            hig = (higAvg * W1 + higDeath * W2)/3;

            attSum = (100 - (low + med + hig))/3;

            low += attSum;
            med += attSum;
            hig += attSum;
            //Action types
            lowActRate = previousRhythm.GetLowActionRate() + (low/100 - previousRhythm.GetLowActionRate());
            medActRate = previousRhythm.GetMedActionRate() + (med/100 - previousRhythm.GetMedActionRate());
            higActRate = previousRhythm.GetHigActionRate() + (hig/100 - previousRhythm.GetHigActionRate());
        }

        lowActNumber = (int)(numberOfActions * lowActRate);
        medActNumber = (int)(numberOfActions * medActRate);
        higActNumber = (int)(numberOfActions * higActRate);

        if (numberOfActions > (lowActNumber + medActNumber + higActNumber))
        { 
            ran = Random.Range(0,100);
            if (ran >= 0 && ran < 100 / 33)
                lowActNumber += numberOfActions - (lowActNumber + medActNumber + higActNumber);
            else if(ran >= 100/33 && ran < 2*(100 / 33))
                medActNumber += numberOfActions - (lowActNumber + medActNumber + higActNumber);
            else
                higActNumber += numberOfActions - (lowActNumber + medActNumber + higActNumber);
        }

        //We sum one to include the first action (move)
        actions = new Action[numberOfActions + 1];

        //Update performance with current performance
        globalPerformance = currentGlobalPerformance;
    }

    private float GetAttentionAvg(List<float>[] attentionMatrix,GameController.AttentionType type)
    {
        float sum = 0.0f;
        float res;

        foreach (float a in attentionMatrix[(int)type])
            sum += a;

        if (attentionMatrix[(int)type].Count > 0)
            res = sum / attentionMatrix[(int)type].Count;
        else
            res = 0;

        return res;
    }

    public void Shuffle(this System.Random rng, Jump.HeightType[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            int k = rng.Next(n--);
            Jump.HeightType temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="moveProb"></param>
    /// <param name="jumpProb"></param>
    public void Build(float minJump, float maxJump)
    {
        float blockSize, beginTime, endTime;
        int jumpType,jumpIdx;
        string actionType;
        Action action;
        Move moveAction;
        Jump.HeightType heightType;
        Jump.HeightType[] jumps;

        blockSize = CreateBlockSize();

        jumps = new Jump.HeightType[lowActNumber + medActNumber + higActNumber];
        jumpIdx = 0;
        for(int i=0;i<lowActNumber;i++)
        {
            jumps[jumpIdx] = Jump.HeightType.Short;
            jumpIdx++;
        }
        for(int i=0;i<medActNumber;i++)
        {
            jumps[jumpIdx] = Jump.HeightType.Medium;
            jumpIdx++;
        }
        for(int i=0;i<higActNumber;i++)
        {
            jumps[jumpIdx] = Jump.HeightType.Long;
            jumpIdx++;
        }
        
        //Shuffle
        Shuffle(new System.Random(),jumps);

        for (int i = 0; i < actions.Length; i++)
        {
            beginTime = CreateBeginTime(i, blockSize);

            actionType = CreateActionType(i);

            action = null;
            switch (actionType)
            { 
                case Jump.TYPE:
                    heightType = jumps[i - 1];
                    endTime = maxJump;

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
        
        /*switch (type)
        {
            case Type.Regular:*/
                beginTime = beatNumber * blockSize;
                /*break;
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
        }*/

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

        /*switch (type)
        {
            case Type.Regular:*/
                blockSize = length / actions.Length;
                /*break;
            case Type.Swing:
                blockSize = length / (actions.Length / 2);
                break;
            case Type.Random:
                blockSize = length / actions.Length;
                break;
        }*/

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
            /*if (currPos >= maxMoveEndPos)
                actionType = Move.TYPE;*
            //else*/
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