using System.IO;
using System;
using UnityEngine;
[System.Serializable]
public class LevelEditor : MonoBehaviour 
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
	void Start () 
	{
        Debug.Log(Application.persistentDataPath);
        fileSize = SETTINGS_BLOCK_SIZE + (TILE_LAYERS * LEVEL_SIZE * LEVEL_SIZE * TILE_SIZE) + (MAX_OBJECTS * OBJECT_SIZE);
        MakeSpriteArray ();
		workingLevel = new byte[fileSize];
        workingTiles = new Tile[TILE_LAYERS,LEVEL_SIZE,LEVEL_SIZE];
        workingObjs = new Obj[MAX_OBJECTS];

		//SaveLevel (workingLevel,"TEST.lvl");
		//LoadLevel (workingFileName);

	}

	Vector3 p = new Vector3 ();
	Camera c;
	Event e;
	Vector2 mousePos = new Vector2 ();
	//BASIC EDITOR SHIT
	void OnGUI()
	{
		c = Camera.main;
		e = Event.current;
		mousePos.x = e.mousePosition.x;
		mousePos.y = c.pixelHeight - e.mousePosition.y;
        if (mouseInCamera())
        {
            p = c.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, c.nearClipPlane));
            GUILayout.BeginArea(new Rect(20, 20, 250, 120));
            GUILayout.Label("Screen pixels: " + c.pixelWidth + ":" + c.pixelHeight);
            GUILayout.Label("Mouse position: " + mousePos);
            GUILayout.Label("World position: " + p.ToString("F0"));
            GUILayout.EndArea();
        }
		
	}
	void Update()
	{
        if (mouseInCamera())
        {
            if (Input.GetMouseButtonDown(0))
            {
                int x = (int)(p.x + .5);
                int y = (int)(p.y + .5);

                //BRUSH
                //row 1
                SetTile(1, x - 1, y + 1, 0, 24);
                SetTile(1, x + 0, y + 1, 0, 25);
                SetTile(1, x + 1, y + 1, 0, 26);
                //row 2
                SetTile(1, x - 1, y + 0, 0, 16);
                SetTile(1, x + 0, y + 0, 0, 17);
                SetTile(1, x + 1, y + 0, 0, 18);
                //row 3
                SetTile(1, x - 1, y - 1, 0, 8);
                SetTile(1, x + 0, y - 1, 0, 9);
                SetTile(1, x + 1, y - 1, 0, 10);
                Debug.Log("Placed at " + x +"," + y);
            }
            if (Input.GetMouseButtonDown(1))
            {
                int x = (int)(p.x + .5);
                int y = (int)(p.y + .5);
                //BRUSH
                //BRUSH
                //row 1
                SetTile(1, x - 1, y + 1, 0, 31);
                SetTile(1, x + 0, y + 1, 0, 31);
                SetTile(1, x + 1, y + 1, 0, 31);
                //row 2
                SetTile(1, x - 1, y + 0, 0, 31);
                SetTile(1, x + 0, y + 0, 0, 31);
                SetTile(1, x + 1, y + 0, 0, 31);
                //row 3
                SetTile(1, x - 1, y - 1, 0, 31);
                SetTile(1, x + 0, y - 1, 0, 31);
                SetTile(1, x + 1, y - 1, 0, 31);
                Debug.Log("Removed at " + x + "," + y);

            }
        }
        
	}
    bool mouseInCamera(){ if (mousePos.x<c.pixelWidth) {return true;} else {return false;} }

    //FILE SHIT
    //UI SHIT
    public void UpdateWorkingFileName(string update) {workingFileName = update;}
    public void SaveLevelButton(){SaveLevel(workingFileName);}
    public void LoadLevelButton(){LoadLevel(workingFileName);}
    public void UpdUp(string update) { UpdateNeighboringLevelFiles(update, 0); }
    public void UpdDw(string update) { UpdateNeighboringLevelFiles(update, 1); }
    public void UpdLf(string update) { UpdateNeighboringLevelFiles(update, 2); }
    public void UpdRg(string update) { UpdateNeighboringLevelFiles(update, 3); }
    void UpdateNeighboringLevelFiles(string update, int which)
    {
        Debug.Log(update.Length);
        byte b1 = Convert.ToByte(update.Substring(0,2), 16);
        byte b2 = Convert.ToByte(update.Substring(2,2), 16);
        int combined = b1 << 8 | b2;
        Debug.Log(combined);
        switch (which)
        {
            case 0:
                workingSettings.up    = combined;
                Debug.Log("Level above updated.");
                break;
            case 1:
                workingSettings.down  = combined;
                Debug.Log("Level below updated.");
                break;
            case 2:
                workingSettings.left  = combined;
                Debug.Log("Level left updated.");
                break;
            case 3:
                workingSettings.right = combined;
                Debug.Log("Level right updated.");
                break;
        }
    }

    //REAL FILE SHIT
    void SaveLevel(string fileName)
	{
        workingLevel = new byte[fileSize];
        Debug.Log("Saving...");
        //save settings block
        workingLevel[0] = (byte)((workingSettings.up    >> 8) | 0xFF);
        workingLevel[1] = (byte)((workingSettings.up    >> 0) | 0xFF);
        workingLevel[2] = (byte)((workingSettings.down  >> 8) | 0xFF);
        workingLevel[3] = (byte)((workingSettings.down  >> 0) | 0xFF);
        workingLevel[4] = (byte)((workingSettings.left  >> 8) | 0xFF);
        workingLevel[5] = (byte)((workingSettings.left  >> 0) | 0xFF);
        workingLevel[6] = (byte)((workingSettings.right >> 8) | 0xFF);
        workingLevel[7] = (byte)((workingSettings.right >> 0) | 0xFF);
        Debug.Log("Settings block converted.");
        //save tiles block
        for (int l = 0; l < TILE_LAYERS; l++)
        {
            Debug.Log("Saving layer " + l);
            for (int y = 0; y < LEVEL_SIZE; y++)
            {
                for (int x = 0; x < LEVEL_SIZE; x++)
                {
                    //getting the bytes of the tile
                    int tilepos = SETTINGS_BLOCK_SIZE + ((y * LEVEL_SIZE + x) * TILE_SIZE) + (LEVEL_SIZE * LEVEL_SIZE * TILE_SIZE * l);
                    byte setb1 = (byte)((workingTiles[l, y, x].tileSet >> 8) | 0xFF); 
                    byte setb2 = (byte)((workingTiles[l, y, x].tileSet >> 0) | 0xFF);
                    byte subb1 = workingTiles[l, y, x].subTile;
                    if (subb1 != 31) { Debug.Log("Saved a tile at "+l+","+x+","+y+" ("+tilepos+")"); }
                    workingLevel[tilepos + 0]=setb1;
                    workingLevel[tilepos + 1]=setb2;
                    workingLevel[tilepos + 2]=subb1;
                    
                }

            }
        }
        Debug.Log("Tiles block converted.");
        //save object block
        Debug.Log("Object block converted.");


        File.WriteAllBytes(Application.persistentDataPath + "/" + fileName,workingLevel);
        Debug.Log("Level saved.");
    }
	void LoadLevel(string fileName)
	{
        Debug.Log("Loading...");
        //destroy all currently loaded tiles
        foreach (Transform child in RendHolder.transform){GameObject.Destroy(child.gameObject);}
        Debug.Log("Old tiles cleared.");
        //creates blank file if level not found
        if (!File.Exists(Application.persistentDataPath + "/" + fileName))
        {
            Debug.Log("Level was blank, creating new level");
            byte [] blankLevel = new byte[fileSize];
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
        workingSettings.up    = ub1 << 8 | ub2;
        workingSettings.down  = db1 << 8 | db2;
        workingSettings.left  = lb1 << 8 | lb2;
        workingSettings.right = rb1 << 8 | rb2;
        Debug.Log("Loading tiles.");
        for (int l = 0; l < TILE_LAYERS; l++)
        {
            Debug.Log("Loading layer "+l);
            for (int y = 0; y < LEVEL_SIZE; y++)
            {
                for (int x = 0; x < LEVEL_SIZE; x++)
                {
                    //getting the bytes of the tile
                    int tilepos = SETTINGS_BLOCK_SIZE + ((y*LEVEL_SIZE+x)*TILE_SIZE) + (LEVEL_SIZE * LEVEL_SIZE * TILE_SIZE * l);
                    byte setb1 = workingLevel[tilepos + 0];
                    byte setb2 = workingLevel[tilepos + 1];
                    byte subb1 = workingLevel[tilepos + 2];
                    if (subb1 != 31) { Debug.Log("Loaded a tile at " + l + "," + x + "," + y + " (" + tilepos + ")"); }
                    //if (x == 0) { Debug.Log(subb1); }
                    int setcombined = setb1 << 8 | setb2;
                    workingTiles[l, y, x].tileSet = setcombined;
                    workingTiles[l, y, x].subTile = subb1;
                    GameObject n = new GameObject();
                    n.name = "" + l + "_" + x + "_" + y;
                    SpriteRenderer sr = n.AddComponent<SpriteRenderer>();
                    //Debug.Log ((int)workingLevel[SETTINGS_BLOCK_SIZE + (i*TILE_SIZE)]);
                    sr.sprite = SpriteArray[(int)subb1];
                    n.transform.position = new Vector3(x, y, l);
                    n.transform.parent = RendHolder.transform;
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
				SpriteArray[i*TILES_PER_LINE+j]=Sprite.Create(tempTileset, new Rect(j*TILE_RESOLUTION, i*TILE_RESOLUTION, TILE_RESOLUTION, TILE_RESOLUTION), new Vector2(0.5f, 0.5f),TILE_RESOLUTION);
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