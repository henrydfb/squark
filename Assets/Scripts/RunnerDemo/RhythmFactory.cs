using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

struct EnemyDescriptor
{
    public Jump heightType;
    public string action;

    public EnemyDescriptor(Jump heightType, string action)
    {
        this.heightType = heightType;
        this.action = action;
    }
}

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

    public const int MIN_PLATFORM_WIDTH = 30;

    public const float PLAT_W = 0.3f;
    public const float PLAT_H = 0.3f;

    public const float JUMP_FALL_DIFF = 1;

    public const int MAX_LEVEL_SECONDS = 15;
    public const int MAX_LEVEL_PLATFORM_BLOCKS = 200;
    public const int MIN_LEVEL_SECONDS = 10;
    public const int MIN_LEVEL_PLATFORM_BLOCKS = 133;

    //public Dictionary<string,GameObject> prefabs;
    public GameObject platformPrefab;
    public GameObject lowEnemyPrefab;
    public GameObject medEnemyPrefab;
    public GameObject higEnemyPrefab;
    public GameObject lowSpikePrefab;
    public GameObject medSpikePrefab;
    public GameObject higSpikePrefab;
    public GameObject lowJumpTxtPrefab;
    public GameObject medJumpTxtPrefab;
    public GameObject higJumpTxtPrefab;
    public GameObject coin;
    public GameObject platformSpritePrefab;
    public Vector3 currentPosition = new Vector3();

    private PlayerController player;

    private Vector3 initialPosition = new Vector3();

    private float platformW = 0;

    private Vector3 initialGeometryPosition;

    private float lowestPosY;

    private List<GameObject> elements;

    private GameObject firstPlatform;

    private List<Vector3> enemyInitialPos;

    private PlatformController platformCont;

    /// <summary>
    /// 
    /// </summary>
    public void Start()
    {
        float firstPlatW,firstPlatH;
        player = GameObject.Find(Names.Player).GetComponent<PlayerController>();
        currentPosition = new Vector3();//player.transform.position;
        initialPosition = currentPosition;
        initialGeometryPosition = currentPosition;
        lowestPosY = float.PositiveInfinity;

        elements = new List<GameObject>();
        elements.Add(GameObject.Find(Names.Player));

        GenerateRhythm();

        //Fix Z value for all elements
        foreach (GameObject e in elements)
        {
            //Debug.Log(e.name);
            e.transform.position = new Vector3(e.transform.position.x, e.transform.position.y,-3);
        }

        //Set player's position
        if(firstPlatform != null)
        {
            firstPlatW = firstPlatform.GetComponent<BoxCollider2D>().bounds.size.x;
            firstPlatH = firstPlatform.GetComponent<BoxCollider2D>().bounds.size.y;
           // player.transform.position = new Vector3(firstPlatform.transform.position.x - firstPlatW/2 + 0.3f, firstPlatform.transform.position.y + firstPlatH + 1, player.transform.position.z);
            Camera.main.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, Camera.main.transform.position.z);
        }

        int[] gaps = { 5, 2, 2, 5, 2, 2, 2 }; //Contains width (number of blocks) of each gap

        ConstructGaps(20,gaps);

        gaps[1] = 3;
        gaps[2] = 3;
        gaps[6] = 15;

        ConstructGaps(25, gaps);
    }

    public void GenerateRhythm()
    {
        Rhythm rhythm;
        Geometry geometry;

        //Debug.Log("**ATTENTION**: " + Camera.main.GetComponent<CameraController>().GetAverageAttention());
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
            print("\nGeometries");
            foreach (Geometry g in rhythm.GetGeometries())
            {
                print(g.GetPrint());
            }
            //print(geometry.GetPrint());
            Debug.Log("***********");

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
        Jump jump;
        Move move;
        float attention;

        attention = Camera.main.GetComponent<CameraController>().GetAverageAttention();

        platformCont = null;
        foreach (Geometry geometry in rhythm.GetGeometries())
        {
            actions = rhythm.GetActions();
            interpretations = geometry.GetInterpretations();
            enemyInitialPos = new List<Vector3>();

            //Level Structure
            for (int i = 0; i < interpretations.Length; i++)
            {
                switch (interpretations[i].GetType())
                {
                    case Fall.TYPE:
                        Debug.Log("Fall");
                        jump = (Jump)actions[i];

                        switch (jump.GetHeightType())
                        {
                            case Jump.HeightType.Short:
                                currentPosition.y -= 1f;
                                Instantiate(lowJumpTxtPrefab, currentPosition - new Vector3(0,1.5f), Quaternion.identity);
                                break;
                            case Jump.HeightType.Medium:
                                currentPosition.y -= 1.5f;
                                Instantiate(medJumpTxtPrefab, currentPosition - new Vector3(0, 1.5f), Quaternion.identity);
                                break;
                            case Jump.HeightType.Long:
                                currentPosition.y -= 2;
                                Instantiate(higJumpTxtPrefab, currentPosition - new Vector3(0, 1.5f), Quaternion.identity);
                                break;
                        }


                        i = ConstructPlatform(interpretations,actions, i, 0, new List<EnemyDescriptor>());
                        break;
                    case FlatGap.TYPE:
                        Debug.Log("Flat");
                        jump = (Jump)actions[i];

                        switch (jump.GetHeightType())
                        {
                            case Jump.HeightType.Short:
                                currentPosition.x += 1.5f;
                                Instantiate(lowJumpTxtPrefab, currentPosition - new Vector3(1.5f/2, 1.5f), Quaternion.identity);
                                break;
                            case Jump.HeightType.Medium:
                                currentPosition.x += 2.25f;
                                Instantiate(medJumpTxtPrefab, currentPosition - new Vector3(2.25f / 2, 1.5f), Quaternion.identity);
                                break;
                            case Jump.HeightType.Long:
                                currentPosition.x += 3f;
                                Instantiate(higJumpTxtPrefab, currentPosition - new Vector3(3f / 2, 1.5f), Quaternion.identity);
                                break;
                        }

                        i = ConstructPlatform(interpretations, actions, i, 0, new List<EnemyDescriptor>());
                        break;
                    case NoGap.TYPE:
                        Debug.Log("NoGap");
                        jump = (Jump)actions[i];

                        switch (jump.GetHeightType())
                        {
                            case Jump.HeightType.Short:
                                currentPosition.y += 1f;
                                Instantiate(lowJumpTxtPrefab, currentPosition - new Vector3(0, 1.5f), Quaternion.identity);
                                break;
                            case Jump.HeightType.Medium:
                                currentPosition.y += 1.5f;
                                Instantiate(medJumpTxtPrefab, currentPosition - new Vector3(0, 1.5f), Quaternion.identity);
                                break;
                            case Jump.HeightType.Long:
                                currentPosition.y += 2;
                                Instantiate(higJumpTxtPrefab, currentPosition - new Vector3(0, 1.5f), Quaternion.identity);
                                break;
                        }

                        i = ConstructPlatform(interpretations, actions, i, 0, new List<EnemyDescriptor>());

                        break;
                    case NotFlatGap.TYPE:
                        Debug.Log("NoGapFlat");
                        jump = (Jump)actions[i];

                        switch (jump.GetHeightType())
                        {
                            case Jump.HeightType.Short:
                                if (attention > 50)
                                    currentPosition.y += 1;
                                else
                                    currentPosition.y -= 1.5f;

                                currentPosition.x += 1f;

                                Instantiate(lowJumpTxtPrefab, currentPosition - new Vector3(1/2, 1.5f), Quaternion.identity);
                                break;
                            case Jump.HeightType.Medium:
                                if (attention > 50)
                                    currentPosition.y += 1.5f;
                                else
                                    currentPosition.y -= 2.25f;

                                currentPosition.x += 1.5f;

                                Instantiate(medJumpTxtPrefab, currentPosition - new Vector3(1.5f/2, 1.5f), Quaternion.identity);
                                break;
                            case Jump.HeightType.Long:
                                if (attention > 50)
                                    currentPosition.y += 2;
                                else
                                    currentPosition.y -= 3;

                                currentPosition.x += 2;
                                Instantiate(higJumpTxtPrefab, currentPosition - new Vector3(2/2, 1.5f), Quaternion.identity);
                                break;
                        }

                        i = ConstructPlatform(interpretations, actions, i, 0, new List<EnemyDescriptor>());

                        break;
                    case Platform.TYPE:
                        Debug.Log("Plat");
                        i = ConstructPlatform(interpretations, actions, i, 0, new List<EnemyDescriptor>());
                        break;
                }
            }

            //Set flag to last platform
            if (platformCont != null)
            {
                currentPosition.x += 1;
                platformCont.SetLast(true);
            }
        }
    }

    private int ConstructPlatform(GeometryInterpretation[] interpretations,Action[] actions, int i, int prevPlatWidth, List<EnemyDescriptor> enemyDescriptors)
    {
        int newPlatWidth,createdW,r,n;
        float attention;
        GameObject newObject;
        Jump jum;
        Vector3 enePos;

        attention = Camera.main.GetComponent<CameraController>().GetAverageAttention();
        createdW = GetPlatformWidth(interpretations, i);
        newPlatWidth = createdW + prevPlatWidth;
        n = 3;

        if (i + 1 < interpretations.Length)
        {
            if (interpretations[i + 1].GetType() == Platform.TYPE)
            {
                if (enemyDescriptors.Count > 0)
                {
                    enePos = initialPosition + new Vector3(platformW / (enemyDescriptors.Count + 1), 2);

                    foreach (EnemyDescriptor e in enemyDescriptors)
                    {
                        switch (e.action)
                        {
                            case AvoidEnemy.TYPE:
                                CreateAvoidEnemy(e.heightType, enePos);
                                break;
                            case KillEnemy.TYPE:
                                CreateKillEnemy(e.heightType, enePos);
                                break;
                        }

                        enePos += new Vector3(platformW / (enemyDescriptors.Count + 1), 0);
                    }

                    enemyDescriptors.Clear();
                }
                else
                {
                    /*if (interpretations[i].GetType() != AvoidEnemy.TYPE && interpretations[i].GetType() != KillEnemy.TYPE)
                    {
                        if (attention <= 100 / 3)
                        {
                            n = 3;
                        }
                        else if (attention > 100 / 3 && attention <= 2 * (100 / 3))
                        {
                            n = 2;
                        }
                        else
                        {
                            n = 1;
                        }

                        for (int j = 0; j < n; j++)
                        {
                            if (Random.Range(0, 100) > 50)
                            {
                                r = Random.Range(0, 100);

                                if (r > 0 && r < 30)
                                    jum = new Jump(0, 0, Jump.HeightType.Short);
                                else if (r >= 30 && r < 60)
                                    jum = new Jump(0, 0, Jump.HeightType.Medium);
                                else
                                    jum = new Jump(0, 0, Jump.HeightType.Long);

                                enePos = initialPosition + new Vector3((j + 1) * (platformW / (n + 1)), 2);

                                CreateAvoidEnemy(jum, enePos);
                                Instantiate(lowJumpTxtPrefab, enePos + new Vector3(0, 2), Quaternion.identity);
                            }
                            else
                            {
                                r = Random.Range(0, 100);

                                if (r > 0 && r < 30)
                                    jum = new Jump(0, 0, Jump.HeightType.Short);
                                else if (r >= 30 && r < 60)
                                    jum = new Jump(0, 0, Jump.HeightType.Medium);
                                else
                                    jum = new Jump(0, 0, Jump.HeightType.Long);

                                enePos = initialPosition + new Vector3((j + 1) * (platformW / (n + 1)), 2);

                                CreateKillEnemy(jum, enePos);

                                Instantiate(lowJumpTxtPrefab, enePos + new Vector3(0, 2), Quaternion.identity);
                            }
                        }
                    }*/
                }

                return ConstructPlatform(interpretations, actions, i + 1, newPlatWidth,enemyDescriptors);
            }
            else if (interpretations[i + 1].GetType() == AvoidEnemy.TYPE)
            {
                enemyDescriptors.Add(new EnemyDescriptor((Jump)actions[i + 1], AvoidEnemy.TYPE));

                return ConstructPlatform(interpretations, actions, i + 1, newPlatWidth - createdW,enemyDescriptors);
            }
            else if (interpretations[i + 1].GetType() == KillEnemy.TYPE)
            {
                enemyDescriptors.Add(new EnemyDescriptor((Jump)actions[i + 1], KillEnemy.TYPE));

                return ConstructPlatform(interpretations, actions, i + 1, newPlatWidth - createdW,enemyDescriptors);
            }
            else
            {
                newObject = (GameObject)Instantiate(platformPrefab, currentPosition + new Vector3(newPlatWidth * PLAT_W / 2, 0), Quaternion.identity);

                if (firstPlatform == null)
                    firstPlatform = newObject;

                platformCont = newObject.GetComponent<PlatformController>();
                platformCont.Contruct(newPlatWidth);
                initialPosition = new Vector3(platformCont.transform.position.x - platformCont.GetComponent<BoxCollider2D>().bounds.size.x / 2, platformCont.transform.position.y);
                platformW = newPlatWidth * PLAT_W;
                currentPosition.x += platformCont.GetComponent<BoxCollider2D>().bounds.size.x;
                if (platformCont.transform.position.y < lowestPosY)
                    lowestPosY = platformCont.transform.position.y;

                enemyInitialPos.Add(initialPosition + new Vector3(platformCont.GetComponent<BoxCollider2D>().bounds.size.x / 4, 0));


                if (enemyDescriptors.Count > 0)
                {
                    enePos = initialPosition + new Vector3(platformW / (enemyDescriptors.Count + 1), 2);

                    foreach (EnemyDescriptor e in enemyDescriptors)
                    {
                        switch (e.action)
                        {
                            case AvoidEnemy.TYPE:
                                CreateAvoidEnemy(e.heightType, enePos);
                                break;
                            case KillEnemy.TYPE:
                                CreateKillEnemy(e.heightType, enePos);
                                break;
                        }

                        enePos += new Vector3(platformW / (enemyDescriptors.Count + 1), 0);
                    }

                    enemyDescriptors.Clear();
                }
                else
                {
                    /*if (interpretations[i].GetType() != AvoidEnemy.TYPE && interpretations[i].GetType() != KillEnemy.TYPE)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            if (Random.Range(0, 100) > 50)
                            {
                                r = Random.Range(0, 100);

                                if (r > 0 && r < 30)
                                    jum = new Jump(0, 0, Jump.HeightType.Short);
                                else if (r >= 30 && r < 60)
                                    jum = new Jump(0, 0, Jump.HeightType.Medium);
                                else
                                    jum = new Jump(0, 0, Jump.HeightType.Long);

                                enePos = initialPosition + new Vector3((j + 1) * (platformW / (n + 1)), 2);

                                CreateAvoidEnemy(jum, enePos);
                                Instantiate(lowJumpTxtPrefab, enePos + new Vector3(0, 2), Quaternion.identity);
                            }
                            else
                            {
                                r = Random.Range(0, 100);

                                if (r > 0 && r < 30)
                                    jum = new Jump(0, 0, Jump.HeightType.Short);
                                else if (r >= 30 && r < 60)
                                    jum = new Jump(0, 0, Jump.HeightType.Medium);
                                else
                                    jum = new Jump(0, 0, Jump.HeightType.Long);

                                enePos = initialPosition + new Vector3((j + 1) * (platformW / (n + 1)), 2);

                                CreateKillEnemy(jum, enePos);

                                Instantiate(lowJumpTxtPrefab, enePos + new Vector3(0, 2), Quaternion.identity);
                            }
                        }
                    }*/
                }


                return i;
            }
        }
        else
        {
            newObject = (GameObject)Instantiate(platformPrefab, currentPosition + new Vector3(newPlatWidth * PLAT_W / 2, 0), Quaternion.identity);

            if (firstPlatform == null)
                firstPlatform = newObject;

            platformCont = newObject.GetComponent<PlatformController>();
            platformCont.Contruct(newPlatWidth);

            initialPosition = new Vector3(platformCont.transform.position.x - platformCont.GetComponent<BoxCollider2D>().bounds.size.x / 2, platformCont.transform.position.y);
            platformW = newPlatWidth * PLAT_W;
            currentPosition.x += platformCont.GetComponent<BoxCollider2D>().bounds.size.x;

            if (platformCont.transform.position.y < lowestPosY)
                lowestPosY = platformCont.transform.position.y;

            enemyInitialPos.Add(initialPosition + new Vector3(platformCont.GetComponent<BoxCollider2D>().bounds.size.x / 4, 0));

            if (enemyDescriptors.Count > 0)
            {
                enePos = initialPosition + new Vector3(platformW / (enemyDescriptors.Count + 1), 2);

                foreach (EnemyDescriptor e in enemyDescriptors)
                {
                    switch (e.action)
                    {
                        case AvoidEnemy.TYPE:
                            CreateAvoidEnemy(e.heightType, enePos);
                            break;
                        case KillEnemy.TYPE:
                            CreateKillEnemy(e.heightType, enePos);
                            break;
                    }

                    enePos += new Vector3(platformW / (enemyDescriptors.Count + 1), 0);
                }

                enemyDescriptors.Clear();
            }
            else
            {
                /*if (interpretations[i].GetType() != AvoidEnemy.TYPE && interpretations[i].GetType() != KillEnemy.TYPE)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (Random.Range(0, 100) > 50)
                        {
                            r = Random.Range(0, 100);

                            if (r > 0 && r < 30)
                                jum = new Jump(0, 0, Jump.HeightType.Short);
                            else if (r >= 30 && r < 60)
                                jum = new Jump(0, 0, Jump.HeightType.Medium);
                            else
                                jum = new Jump(0, 0, Jump.HeightType.Long);

                            enePos = initialPosition + new Vector3((j + 1) * (platformW / (n + 1)), 2);

                            CreateAvoidEnemy(jum, enePos);
                            Instantiate(lowJumpTxtPrefab, enePos + new Vector3(0, 2), Quaternion.identity);
                        }
                        else
                        {
                            r = Random.Range(0, 100);

                            if (r > 0 && r < 30)
                                jum = new Jump(0, 0, Jump.HeightType.Short);
                            else if (r >= 30 && r < 60)
                                jum = new Jump(0, 0, Jump.HeightType.Medium);
                            else
                                jum = new Jump(0, 0, Jump.HeightType.Long);

                            enePos = initialPosition + new Vector3((j + 1) * (platformW / (n + 1)), 2);

                            CreateKillEnemy(jum, enePos);

                            Instantiate(lowJumpTxtPrefab, enePos + new Vector3(0, 2), Quaternion.identity);
                        }
                    }
                }*/
            }

            return i;
        }
    }

    private void CreateKillEnemy(Jump jump,Vector3 enePos)
    {
        GameObject newObject;
        //Vector3 enePos;
        float enemyH;

        enemyH = 0;
        switch (jump.GetHeightType())
        {
            case Jump.HeightType.Short:
                enemyH = lowEnemyPrefab.GetComponent<Renderer>().bounds.size.y;
                newObject = (GameObject)Instantiate(lowEnemyPrefab, enePos, Quaternion.identity);
                elements.Add(newObject);
                Instantiate(lowJumpTxtPrefab, enePos, Quaternion.identity);
                break;
            case Jump.HeightType.Medium:
                enemyH = medEnemyPrefab.GetComponent<Renderer>().bounds.size.y;
                newObject = (GameObject)Instantiate(medEnemyPrefab, enePos, Quaternion.identity);
                elements.Add(newObject);
                Instantiate(medJumpTxtPrefab, enePos, Quaternion.identity);
                break;
            case Jump.HeightType.Long:
                enemyH = higEnemyPrefab.GetComponent<Renderer>().bounds.size.y;
                newObject = (GameObject)Instantiate(higEnemyPrefab, enePos, Quaternion.identity);
                elements.Add(newObject);
                Instantiate(higJumpTxtPrefab, enePos, Quaternion.identity);
                break;
        }
    }

    private void CreateAvoidEnemy(Jump jump,Vector3 enePos)
    {
        GameObject newObject;

        switch (jump.GetHeightType())
        {
            case Jump.HeightType.Short:
                newObject = (GameObject)Instantiate(lowSpikePrefab, enePos, Quaternion.identity);
                elements.Add(newObject);
                Instantiate(lowJumpTxtPrefab, enePos, Quaternion.identity);
                break;
            case Jump.HeightType.Medium:
                newObject = (GameObject)Instantiate(medSpikePrefab, enePos, Quaternion.identity);
                elements.Add(newObject);
                Instantiate(medJumpTxtPrefab, enePos, Quaternion.identity);
                break;
            case Jump.HeightType.Long:
                newObject = (GameObject)Instantiate(higSpikePrefab, enePos, Quaternion.identity);
                elements.Add(newObject);
                Instantiate(higJumpTxtPrefab, enePos, Quaternion.identity);
                break;
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



    public void ConstructGaps(float startY,int[] gaps)
    {
        GameObject platform, enemy;
        Vector3 enePos;
        int numberOfgaps, currBlock,platBlocks, gapBlocks, totalBlocks;
        //int [] gaps = {5,2,2,5,2,2,2}; //Contains width (number of blocks) of each gap
        float platX;
        bool prevEne;
        numberOfgaps = gaps.Length;
        currBlock = 0;
        platX = 0.0f;
        totalBlocks = 0;
        prevEne = false;
        platBlocks = 0;
        for (int i = 0; i < numberOfgaps; i++)
        {
            gapBlocks = gaps[i];
            if (gapBlocks == 2)
            {
                platBlocks += (MAX_LEVEL_PLATFORM_BLOCKS / (numberOfgaps + 1));

                /*TODO: 
                 * Hay que corregir la posicion del enemigo, se coloca donde no es (un poco mas a la derecha, poner en la plat. anterior)
                 * Modificar la dificultad de creacion de ritmos aumentando y disminuyendo cantidad de: enemigo y gaps (por ahora)
                 * Agregar enemigos moviles
                 * Agregar enemigo que dispare o algun elemento de nivel
                 * Revisar la altura exacta para construir un path mas arriba y uno mas abajo del path principal (con ritmos) y conectarlos a traves de una plataforma
                 * DDA Involucrar el EEG de manera muy sencilla por ahora: alto -> dificil (mas enemigos, mas largo nivel), bajo -> facil (menos enemigos, mas corto nivel)
                 * Implementar agarrar la bandera al final
                 * */
                enePos = new Vector3(platX + (PLAT_W * (platBlocks)), startY +  0.5f);
                enemy = (GameObject)Instantiate(medSpikePrefab, enePos, Quaternion.identity);
                elements.Add(enemy);
                Instantiate(medJumpTxtPrefab, enePos, Quaternion.identity);

                Debug.Log("ENEMY!");
                prevEne = true;
            }
            else
            {
                if (prevEne)
                    platBlocks += numberOfgaps == 0 ? MAX_LEVEL_PLATFORM_BLOCKS : (MAX_LEVEL_PLATFORM_BLOCKS / (numberOfgaps + 1)) - gapBlocks;
                else
                    platBlocks = numberOfgaps == 0 ? MAX_LEVEL_PLATFORM_BLOCKS : (MAX_LEVEL_PLATFORM_BLOCKS / (numberOfgaps + 1)) - gapBlocks;

                totalBlocks += (platBlocks + gapBlocks);
                platform = (GameObject)Instantiate(platformPrefab, new Vector3(platX + (PLAT_W * platBlocks / 2), startY), Quaternion.identity);
                platformCont = platform.GetComponent<PlatformController>();
                platformCont.Contruct(platBlocks);
                currBlock += platBlocks;
                platX += (PLAT_W * (platBlocks + gapBlocks));
                platBlocks = 0;
                prevEne = false;
            }
        }
        

        platBlocks = MAX_LEVEL_PLATFORM_BLOCKS - totalBlocks;
        platform = (GameObject)Instantiate(platformPrefab, new Vector3(platX + (PLAT_W * platBlocks / 2), startY), Quaternion.identity);

        platformCont = platform.GetComponent<PlatformController>();
        platformCont.Contruct(platBlocks);
    }

    /// <summary>/
    /// 
    /// </summary>
    public Geometry InterpretRhythm(Rhythm rhythm)
    {
        Action[] actions;
        Geometry geometry;
        GeometryInterpretation[] geometries;
        GeometryInterpretation geometryInt;
        int nGeometries = 6, maxProbVal = 100, avoidProb , killProb , fallProb , flatGProb , noGProb , noFlatGProb,p;
        float geoProbability, intProb;
        float attention;

        attention = Camera.main.GetComponent<CameraController>().GetAverageAttention();
        actions = rhythm.GetActions();
        geometries = new GeometryInterpretation[actions.Length];
        intProb = ((float)maxProbVal / (float)nGeometries);
        int[] probs;

        if (attention <= 100 / 3)
        {
            avoidProb = 10;
            killProb = 10;
            fallProb = 10;
            flatGProb = 10;
            noGProb = 10;
            noFlatGProb = 10;
        }
        else if (attention > 100 / 3 && attention <= 2 * (100 / 3))
        {
            avoidProb = 15;
            killProb = 20;
            fallProb = 10;
            flatGProb = 10;
            noGProb = 10;
            noFlatGProb = 10;
        }
        else
        {
            avoidProb = 20;
            killProb = 35;
            fallProb = 10;
            flatGProb = 10;
            noGProb = 10;
            noFlatGProb = 10;
        }
        
        probs = CreateActionsProbabilityArray(avoidProb, killProb, fallProb,flatGProb,noGProb,noFlatGProb);

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
                        //if (geoProbability >= j * intProb && geoProbability <= (j + 1) * intProb)
                        //{
                            p = Random.Range(0,probs.Length - 1);
                            switch (probs[p])
                            {
                                /*case AvoidEnemy.TYPE_NUMBER:
                                    geometryInt = new AvoidEnemy();
                                    break;
                                case KillEnemy.TYPE_NUMBER:
                                    geometryInt = new KillEnemy();
                                    break;
                                case Fall.TYPE_NUMBER:
                                    geometryInt = new Fall();
                                    break;
                                case FlatGap.TYPE_NUMBER:
                                    geometryInt = new FlatGap();
                                    break;
                                case NoGap.TYPE_NUMBER:
                                    geometryInt = new NoGap();
                                    break;
                                case NotFlatGap.TYPE_NUMBER:
                                    geometryInt = new NotFlatGap();
                                    break;*/
                                default:
                                    /*if(attention <= 50)
                                        geometryInt = new AvoidEnemy();
                                    else
                                        geometryInt = new KillEnemy();*/
                                    geometryInt = new FlatGap();
                                    break;
                            }
                        //}
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

    private int[] CreateActionsProbabilityArray(int avoidProb, int killProb, int fallProb,int flatGProb,int noGProb,int noFlatGProb)
    { 
        int[] probs = new int[avoidProb + killProb + fallProb + flatGProb + noGProb + noFlatGProb];
        List<int> actions = new List<int>();
        int ran;

        actions.Add(AvoidEnemy.TYPE_NUMBER);
        actions.Add(KillEnemy.TYPE_NUMBER);
        actions.Add(Fall.TYPE_NUMBER);
        actions.Add(FlatGap.TYPE_NUMBER);
        actions.Add(NoGap.TYPE_NUMBER);
        actions.Add(NotFlatGap.TYPE_NUMBER);

        for (int i = 0; i < probs.Length; i++)
        {
            ran = Random.Range(0,actions.Count - 1);
            probs[i] = actions[ran];
            switch(actions[ran])
            {
                case AvoidEnemy.TYPE_NUMBER:
                    avoidProb--;
                    if (avoidProb <= 0)
                        actions.RemoveAt(ran);
                    break;
                case KillEnemy.TYPE_NUMBER:
                    killProb--;
                    if (killProb <= 0)
                        actions.RemoveAt(ran);
                    break;
                case Fall.TYPE_NUMBER:
                    fallProb--;
                    if (fallProb <= 0)
                        actions.RemoveAt(ran);
                    break;
                case FlatGap.TYPE_NUMBER:
                    flatGProb--;
                    if (flatGProb <= 0)
                        actions.RemoveAt(ran);
                    break;
                case NoGap.TYPE_NUMBER:
                    noGProb--;
                    if (noGProb <= 0)
                        actions.RemoveAt(ran);
                    break;
                case NotFlatGap.TYPE_NUMBER:
                    noFlatGProb--;
                    if (noFlatGProb <= 0)
                        actions.RemoveAt(ran);
                    break;
            }
        }

        return probs;
    }
}
