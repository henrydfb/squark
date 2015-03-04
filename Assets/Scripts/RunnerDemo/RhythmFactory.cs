using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class RhythmFactory : MonoBehaviour
{
    public Rhythm.Type rhythmType;

    public Rhythm.Density rhythmDensity;

    public float rhythmLength;

    public float minJump;

    public float maxJump;

    public const int MIN_PLATFORM_WIDTH = 29;

    public const float JUMP_FALL_DIFF = 1;

    //public Dictionary<string,GameObject> prefabs;
    public GameObject platformPrefab;
    public GameObject lowEnemyPrefab;
    public GameObject medEnemyPrefab;
    public GameObject higEnemyPrefab;
    public GameObject lowSpikePrefab;
    public GameObject medSpikePrefab;
    public GameObject higSpikePrefab;


    public Vector3 currentPosition = new Vector3();
    private Vector3 initialPosition = new Vector3();

    private Vector3 initialGeometryPosition;

    private float lowestPosY;

    /// <summary>
    /// 
    /// </summary>
    public void Start()
    {
        Rhythm rhythm;
        Geometry geometry;

        currentPosition = GameObject.Find(Names.Player).transform.position + new Vector3(0,-5,0);
        initialPosition = currentPosition;
        initialGeometryPosition = currentPosition;
        lowestPosY = float.PositiveInfinity;

        GenerateRhythm();       
    }

    public void GenerateRhythm()
    {
        Rhythm rhythm;
        Geometry geometry;

        for (int i = 0; i < 1; i++)
        {
            rhythm = new Rhythm(rhythmType, rhythmDensity, rhythmLength);
            rhythm.Build(minJump, maxJump);
            //print("Actions");
            //print(rhythm.GetPrint());
            for (int j = 0; j < 1; j++)
            {
                geometry = InterpretRhythm(rhythm);
                rhythm.AddGeometry(geometry);
            }
            /*print("\nGeometries");
            foreach (Geometry g in rhythm.GetGeometries())
            {
                print(g.GetPrint());
            }
            //print(geometry.GetPrint());
            Debug.Log("***********");*/

            ConstructLevel(rhythm);
        }
    }

    public float GetLowestPosY()
    {
        return lowestPosY;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Update()
    {}

    /// <summary>
    /// 
    /// </summary>
    public void ConstructLevel(Rhythm rhythm)
    {
        //Geometry geometry;
        GeometryInterpretation[] interpretations;
        Action[] actions;
        GameObject newObject;
        Jump jump;
        Move move;
        PlatformController platformCont;
        int newPlatWidth, enemyOrder;

        foreach (Geometry geometry in rhythm.GetGeometries())
        {
            actions = rhythm.GetActions();
            interpretations = geometry.GetInterpretations();
            enemyOrder = 1;
            for (int i = 0; i < interpretations.Length; i++)
            {
                switch (interpretations[i].GetType())
                {
                    case AvoidEnemy.TYPE:
                        jump = (Jump)actions[i];

                        //currentPosition.x = initialPosition.x + (i * MIN_PLATFORM_WIDTH * (platformPrefab.renderer.bounds.size.x));
                        switch (jump.GetHeightType())
                        {
                            case Jump.HeightType.Short:
                                newObject = (GameObject)Instantiate(lowSpikePrefab, initialPosition + new Vector3(enemyOrder * MIN_PLATFORM_WIDTH * platformPrefab.renderer.bounds.size.x - lowSpikePrefab.renderer.bounds.size.x, JUMP_FALL_DIFF), Quaternion.identity);
                                break;
                            case Jump.HeightType.Medium:
                                newObject = (GameObject)Instantiate(medSpikePrefab, initialPosition + new Vector3(enemyOrder * MIN_PLATFORM_WIDTH * platformPrefab.renderer.bounds.size.x - lowSpikePrefab.renderer.bounds.size.x, JUMP_FALL_DIFF), Quaternion.identity);
                                break;
                            case Jump.HeightType.Long:
                                newObject = (GameObject)Instantiate(higSpikePrefab, initialPosition + new Vector3(enemyOrder * MIN_PLATFORM_WIDTH * platformPrefab.renderer.bounds.size.x - lowSpikePrefab.renderer.bounds.size.x, JUMP_FALL_DIFF), Quaternion.identity);
                                break;
                        }
                        enemyOrder++;
                        break;
                    case Fall.TYPE:
                        jump = (Jump)actions[i];

                        switch (jump.GetHeightType())
                        {
                            case Jump.HeightType.Short:
                                currentPosition.y -= JUMP_FALL_DIFF;
                                break;
                            case Jump.HeightType.Medium:
                                currentPosition.y -= 1.5f * JUMP_FALL_DIFF;
                                break;
                            case Jump.HeightType.Long:
                                currentPosition.y -= 2 * JUMP_FALL_DIFF;
                                break;
                        }

                        newObject = (GameObject)Instantiate(platformPrefab, currentPosition, Quaternion.identity);
                        platformCont = newObject.GetComponent<PlatformController>();
                        newPlatWidth = GetPlatformWidth(interpretations, i);
                        platformCont.Contruct(newPlatWidth);
                        platformCont.transform.position += new Vector3((platformCont.renderer.bounds.size.x / 2), 0);
                        initialPosition = new Vector3(platformCont.transform.position.x - platformCont.renderer.bounds.size.x / 2, platformCont.transform.position.y);
                        currentPosition.x += platformCont.renderer.bounds.size.x;
                        enemyOrder = 1;

                        if (platformCont.transform.position.y < lowestPosY)
                            lowestPosY = platformCont.transform.position.y;
                        break;
                    case FlatGap.TYPE:
                        jump = (Jump)actions[i];

                        switch (jump.GetHeightType())
                        {
                            case Jump.HeightType.Short:
                                currentPosition.x += 0.25f * MIN_PLATFORM_WIDTH * platformPrefab.renderer.bounds.size.x;
                                break;
                            case Jump.HeightType.Medium:
                                currentPosition.x += 0.45f * MIN_PLATFORM_WIDTH * platformPrefab.renderer.bounds.size.x;
                                break;
                            case Jump.HeightType.Long:
                                currentPosition.x += 0.65f * MIN_PLATFORM_WIDTH * platformPrefab.renderer.bounds.size.x;
                                break;
                        }
                        newObject = (GameObject)Instantiate(platformPrefab, currentPosition, Quaternion.identity);
                        platformCont = newObject.GetComponent<PlatformController>();
                        newPlatWidth = GetPlatformWidth(interpretations, i);
                        platformCont.Contruct(newPlatWidth);
                        platformCont.transform.position += new Vector3((platformCont.renderer.bounds.size.x / 2), 0);
                        initialPosition = new Vector3(platformCont.transform.position.x - platformCont.renderer.bounds.size.x / 2, platformCont.transform.position.y);
                        currentPosition.x += platformCont.renderer.bounds.size.x;
                        enemyOrder = 1;

                        if (platformCont.transform.position.y < lowestPosY)
                            lowestPosY = platformCont.transform.position.y;
                        break;
                    case KillEnemy.TYPE:
                        jump = (Jump)actions[i];

                        //currentPosition.x = initialPosition.x + (i * MIN_PLATFORM_WIDTH * (platformPrefab.renderer.bounds.size.x / 2));

                        switch (jump.GetHeightType())
                        {
                            case Jump.HeightType.Short:
                                newObject = (GameObject)Instantiate(lowEnemyPrefab, initialPosition + new Vector3(enemyOrder * MIN_PLATFORM_WIDTH * platformPrefab.renderer.bounds.size.x - lowSpikePrefab.renderer.bounds.size.x, JUMP_FALL_DIFF), Quaternion.identity);
                                break;
                            case Jump.HeightType.Medium:
                                newObject = (GameObject)Instantiate(medEnemyPrefab, initialPosition + new Vector3(enemyOrder * MIN_PLATFORM_WIDTH * platformPrefab.renderer.bounds.size.x - lowSpikePrefab.renderer.bounds.size.x, JUMP_FALL_DIFF), Quaternion.identity);
                                break;
                            case Jump.HeightType.Long:
                                newObject = (GameObject)Instantiate(higEnemyPrefab, initialPosition + new Vector3(enemyOrder * MIN_PLATFORM_WIDTH * platformPrefab.renderer.bounds.size.x - lowSpikePrefab.renderer.bounds.size.x, JUMP_FALL_DIFF), Quaternion.identity);
                                break;
                        }
                        enemyOrder++;
                        break;
                    case NoGap.TYPE:
                        jump = (Jump)actions[i];

                        switch (jump.GetHeightType())
                        {
                            case Jump.HeightType.Short:
                                currentPosition.y += JUMP_FALL_DIFF;
                                //currentPosition.x += MIN_PLATFORM_WIDTH * platformPrefab.renderer.bounds.size.x;
                                break;
                            case Jump.HeightType.Medium:
                                currentPosition.y += 1.5f * JUMP_FALL_DIFF;
                                //currentPosition.x += 2 * MIN_PLATFORM_WIDTH * platformPrefab.renderer.bounds.size.x;
                                break;
                            case Jump.HeightType.Long:
                                currentPosition.y += 2 * JUMP_FALL_DIFF;
                                //currentPosition.x += 3 * MIN_PLATFORM_WIDTH * platformPrefab.renderer.bounds.size.x;
                                break;
                        }
                        newObject = (GameObject)Instantiate(platformPrefab, currentPosition, Quaternion.identity);
                        platformCont = newObject.GetComponent<PlatformController>();
                        newPlatWidth = GetPlatformWidth(interpretations, i);
                        platformCont.Contruct(newPlatWidth);
                        platformCont.transform.position += new Vector3((platformCont.renderer.bounds.size.x / 2), 0);
                        initialPosition = new Vector3(platformCont.transform.position.x - platformCont.renderer.bounds.size.x / 2, platformCont.transform.position.y);
                        currentPosition.x += platformCont.renderer.bounds.size.x;
                        enemyOrder = 1;

                        if (platformCont.transform.position.y < lowestPosY)
                            lowestPosY = platformCont.transform.position.y;
                        break;
                    case NotFlatGap.TYPE:
                        jump = (Jump)actions[i];

                        switch (jump.GetHeightType())
                        {
                            case Jump.HeightType.Short:
                                if (Random.Range(0, 100) > 50)
                                    currentPosition.y += JUMP_FALL_DIFF / 2;
                                else
                                    currentPosition.y -= JUMP_FALL_DIFF / 2;
                                currentPosition.x += 0.25f * MIN_PLATFORM_WIDTH / 2 * platformPrefab.renderer.bounds.size.x;
                                break;
                            case Jump.HeightType.Medium:
                                if (Random.Range(0, 100) > 50)
                                    currentPosition.y += 1.5f * JUMP_FALL_DIFF / 2;
                                else
                                    currentPosition.y -= 1.5f * JUMP_FALL_DIFF / 2;
                                currentPosition.x += 0.5f * MIN_PLATFORM_WIDTH / 2 * platformPrefab.renderer.bounds.size.x;
                                break;
                            case Jump.HeightType.Long:
                                if (Random.Range(0, 100) > 50)
                                    currentPosition.y += 2 * JUMP_FALL_DIFF / 2;
                                else
                                    currentPosition.y -= 2 * JUMP_FALL_DIFF / 2;
                                currentPosition.x += 0.75f * MIN_PLATFORM_WIDTH / 2 * platformPrefab.renderer.bounds.size.x;
                                break;
                        }
                        newObject = (GameObject)Instantiate(platformPrefab, currentPosition, Quaternion.identity);
                        platformCont = newObject.GetComponent<PlatformController>();
                        newPlatWidth = GetPlatformWidth(interpretations, i);
                        platformCont.Contruct(newPlatWidth);
                        platformCont.transform.position += new Vector3((platformCont.renderer.bounds.size.x / 2), 0);
                        initialPosition = new Vector3(platformCont.transform.position.x - platformCont.renderer.bounds.size.x / 2, platformCont.transform.position.y);
                        currentPosition.x += platformCont.renderer.bounds.size.x;
                        enemyOrder = 1;

                        if (platformCont.transform.position.y < lowestPosY)
                            lowestPosY = platformCont.transform.position.y;
                        break;
                    case Platform.TYPE:
                        move = (Move)actions[i];
                        newObject = (GameObject)Instantiate(platformPrefab, currentPosition, Quaternion.identity);
                        newPlatWidth = GetPlatformWidth(interpretations, i);
                        platformCont = newObject.GetComponent<PlatformController>();
                        platformCont.Contruct(newPlatWidth);
                        platformCont.transform.position += new Vector3((platformCont.renderer.bounds.size.x / 2), 0);
                        initialPosition = new Vector3(platformCont.transform.position.x - platformCont.renderer.bounds.size.x / 2, platformCont.transform.position.y);
                        currentPosition.x += platformCont.renderer.bounds.size.x;
                        enemyOrder = 1;

                        if (platformCont.transform.position.y < lowestPosY)
                            lowestPosY = platformCont.transform.position.y;
                        break;
                }
            }

            //currentPosition = initialGeometryPosition + new Vector3(0, -10);
        }
    }

    private int GetPlatformWidth(GeometryInterpretation[] interpretations,int currentIndex)
    {
        if (currentIndex + 1 < interpretations.Length)
        {
            if (interpretations[currentIndex + 1].GetType() == AvoidEnemy.TYPE || interpretations[currentIndex + 1].GetType() == KillEnemy.TYPE)
                return MIN_PLATFORM_WIDTH + GetPlatformWidth(interpretations, currentIndex + 1);
            else
                return MIN_PLATFORM_WIDTH;
        }
        else
            return MIN_PLATFORM_WIDTH;
    }

    /// <summary>
    /// 
    /// </summary>
    public Geometry InterpretRhythm(Rhythm rhythm)
    {
        Action[] actions;
        Geometry geometry;
        GeometryInterpretation[] geometries;
        GeometryInterpretation geometryInt;
        int nGeometries = 6, maxProbVal = 100;
        float geoProbability, intProb;
        
        actions = rhythm.GetActions();
        geometries = new GeometryInterpretation[actions.Length];
        intProb = ((float)maxProbVal / (float)nGeometries);


        for(int i = 0; i < actions.Length; i++)
        {
            geoProbability = Random.Range(0,maxProbVal);
            geometryInt = null;
            //Check where does the geometry probability belongs to
            for (int j = 0; j < nGeometries; j++)
            {
                switch (actions[i].GetType())
                { 
                    case Move.TYPE:
                        geometryInt = new Platform();
                        break;
                    case Jump.TYPE:
                        if (geoProbability >= j * intProb && geoProbability <= (j + 1) * intProb)
                        {
                            switch (j)
                            {
                                case AvoidEnemy.TYPE_NUMBER:
                                    geometryInt = new AvoidEnemy();
                                    break;
                                case Fall.TYPE_NUMBER:
                                    geometryInt = new Fall();
                                    break;
                                case FlatGap.TYPE_NUMBER:
                                    geometryInt = new FlatGap();
                                    break;
                                case KillEnemy.TYPE_NUMBER:
                                    geometryInt = new KillEnemy();
                                    break;
                                case NoGap.TYPE_NUMBER:
                                    geometryInt = new NoGap();
                                    break;
                                case NotFlatGap.TYPE_NUMBER:
                                    geometryInt = new NotFlatGap();
                                    break;
                                default:
                                    geometryInt = new AvoidEnemy();
                                    break;
                            }
                        }
                        break;
                }
            }

            if (geometryInt == null)
                throw new System.Exception("The Geometry cannot be null");
            else
                geometries[i] = geometryInt;
        }

        geometry = new Geometry(geometries);

        return geometry;
    }
}
