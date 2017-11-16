using System.IO;
using System;
using UnityEngine;
[System.Serializable]
public class loadlevel: MonoBehaviour
{
    /*
     * 
     * 
     * 
     * 
     * 
     * 
    */

    public GameObject RendHolder;
    public Texture2D tempTileset;
    byte[] workingLevel;
    public struct Tile
    {
        public int tileSet;
        public byte subTile;
    }
    public struct Obj
    {
        public byte x;
        public byte y;
        public int V1;
        public int V2;
        public int V3;
        public int ObjId;
    }
    public struct SettingsBlock
    {
        //what level to load in each position, 2 bytes
        public int up;
        public int down;
        public int left;
        public int right;
    }
    Tile[,,] workingTiles;
    Obj[] workingObjs;
    SettingsBlock workingSettings;
    //file name format is 4 hex numbers followed by an underscore and then just whatever
    public string workingFileName = "0000_TEST.lvl";
    const int TILES_PER_FILE = 32;
    const int TILES_PER_LINE = 8;
    const int TILE_RESOLUTION = 16;
    const int SETTINGS_BLOCK_SIZE = 8;
    const int LEVEL_SIZE = 128;
    const int TILE_SIZE = 3;
    //FEATURE NOT YET IMPLEMENTED
    const int TILE_LAYERS = 3;
    const int MAX_OBJECTS = 2048;
    const int OBJECT_SIZE = 12;
    int fileSize;
    //EXTREMELY TEMPORARY; FIX IN NEXT REVISION
    Sprite[] SpriteArray;
    // Use this for initialization
    void Start()
    {
        fileSize = SETTINGS_BLOCK_SIZE + (TILE_LAYERS * LEVEL_SIZE * LEVEL_SIZE * TILE_SIZE) + (MAX_OBJECTS * OBJECT_SIZE);
        MakeSpriteArray();
        workingLevel = new byte[fileSize];
        workingTiles = new Tile[TILE_LAYERS, LEVEL_SIZE, LEVEL_SIZE];
        workingObjs = new Obj[MAX_OBJECTS];
        LoadLevel("0000_START.lvl");
        //SaveLevel (workingLevel,"TEST.lvl");
        //LoadLevel (workingFileName);

    }
    void Update()
    {
        

    }
    //FILE SHIT

    //REAL FILE SHIT
    
    void LoadLevel(string fileName)
    {
        Debug.Log("Loading...");
        //creates blank file if level not found
        if (!File.Exists(Application.persistentDataPath + "/" + fileName))
        {
            Debug.Log("Level was blank, creating new level");
            byte[] blankLevel = new byte[fileSize];
            for (int i = 0; i < LEVEL_SIZE * LEVEL_SIZE * TILE_LAYERS; i++)
            {
                blankLevel[SETTINGS_BLOCK_SIZE + (i * TILE_SIZE) + 0] = 0;
                blankLevel[SETTINGS_BLOCK_SIZE + (i * TILE_SIZE) + 1] = 0;
                blankLevel[SETTINGS_BLOCK_SIZE + (i * TILE_SIZE) + 2] = 31;
            }
            File.WriteAllBytes(Application.persistentDataPath + "/" + fileName, blankLevel);
            Debug.Log("New level created.");
        }
        //ok now we know it exists, so...
        workingLevel = File.ReadAllBytes(Application.persistentDataPath + "/" + fileName);
        Debug.Log("Level file found. Loading level file...");
        //actual loading
        Debug.Log("Loading settings.");
        byte ub1 = workingLevel[0];
        byte ub2 = workingLevel[1];
        byte db1 = workingLevel[2];
        byte db2 = workingLevel[3];
        byte lb1 = workingLevel[4];
        byte lb2 = workingLevel[5];
        byte rb1 = workingLevel[6];
        byte rb2 = workingLevel[7];
        workingSettings.up = ub1 << 8 | ub2;
        workingSettings.down = db1 << 8 | db2;
        workingSettings.left = lb1 << 8 | lb2;
        workingSettings.right = rb1 << 8 | rb2;
        Debug.Log("Loading tiles.");
        for (int l = 0; l < TILE_LAYERS; l++)
        {
            Debug.Log("Loading layer " + l);
            for (int y = 0; y < LEVEL_SIZE; y++)
            {
                for (int x = 0; x < LEVEL_SIZE; x++)
                {
                    //getting the bytes of the tile
                    int tilepos = SETTINGS_BLOCK_SIZE + ((y * LEVEL_SIZE + x) * TILE_SIZE) + (LEVEL_SIZE * LEVEL_SIZE * TILE_SIZE * l);
                    byte setb1 = workingLevel[tilepos + 0];
                    byte setb2 = workingLevel[tilepos + 1];
                    byte subb1 = workingLevel[tilepos + 2];
                    if (subb1 != 31) { Debug.Log("Loaded a tile at " + l + "," + x + "," + y + " (" + tilepos + ")"); }
                    //if (x == 0) { Debug.Log(subb1); }
                    int setcombined = setb1 << 8 | setb2;
                    workingTiles[l, y, x].tileSet = setcombined;
                    workingTiles[l, y, x].subTile = subb1;
                    if (subb1 != 31)
                    {
                        GameObject n = new GameObject();
                        n.name = "" + l + "_" + x + "_" + y;
                        SpriteRenderer sr = n.AddComponent<SpriteRenderer>();
                        //Debug.Log ((int)workingLevel[SETTINGS_BLOCK_SIZE + (i*TILE_SIZE)]);
                        sr.sprite = SpriteArray[(int)subb1];
                        n.transform.position = new Vector3(x, y, l);
                        n.transform.parent = RendHolder.transform;
                        n.AddComponent<BoxCollider2D>();
                    }
                    
                }

            }
        }
        Debug.Log("Level loaded.");
    }
    //EXTREMELY TEMPORARY
    void MakeSpriteArray()
    {
        SpriteArray = new Sprite[TILES_PER_FILE];
        for (int i = 0; i < TILES_PER_FILE / TILES_PER_LINE; i++)
        {
            for (int j = 0; j < TILES_PER_LINE; j++)
            {
                SpriteArray[i * TILES_PER_LINE + j] = Sprite.Create(tempTileset, new Rect(j * TILE_RESOLUTION, i * TILE_RESOLUTION, TILE_RESOLUTION, TILE_RESOLUTION), new Vector2(0.5f, 0.5f), TILE_RESOLUTION);
                //Debug.Log (i + " " + j);
            }
        }
    }
    void SetTile(int layer, int x, int y, int tileSet, byte subTile)
    {
        workingTiles[layer, y, x].tileSet = tileSet;
        workingTiles[layer, y, x].subTile = subTile;
        RendHolder.transform.Find("" + layer + "_" + x + "_" + y).gameObject.GetComponent<SpriteRenderer>().sprite = SpriteArray[subTile];
    }
}