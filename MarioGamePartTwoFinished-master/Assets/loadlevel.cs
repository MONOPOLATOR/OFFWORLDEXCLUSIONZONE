using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
[System.Serializable]
public class loadlevel : MonoBehaviour 
{
	public GameObject RendHolder;
	public Texture2D tempTileset;
	byte[] workingLevel;
	const int TILES_PER_FILE = 32;
	const int TILES_PER_LINE = 8;
	const int TILE_RESOLUTION = 16;
	const int SETTINGS_BLOCK_SIZE = 0;
	const int MAX_SIZE = 128;
	const int TILE_SIZE = 2;
	//FEATURE NOT YET IMPLEMENTED
	const int TILE_LAYERS = 3;
	const int MAX_OBJECTS = 2048;
	const int OBJECT_SIZE = 12;
	//EXTREMELY TEMPORARY; FIX IN NEXT REVISION
	Sprite[] SpriteArray;
	// Use this for initialization
	void Start () 
	{
		MakeSpriteArray ();
		workingLevel = new byte[SETTINGS_BLOCK_SIZE + (MAX_SIZE * MAX_SIZE * TILE_SIZE) + (MAX_OBJECTS * OBJECT_SIZE)];
		int j = 0;
		//initialize level
		for (int i = 0; i < MAX_SIZE*MAX_SIZE; i++)
		{
			/*if (j >= TILES_PER_FILE) 
			{
				j = 0;
			}
			workingLevel[SETTINGS_BLOCK_SIZE + (i*TILE_SIZE)] = (byte)j; 
			j++;*/
			workingLevel [SETTINGS_BLOCK_SIZE + (i * TILE_SIZE)] = 31;
		}
		//SaveLevel (workingLevel,"TEST.lvl");
		LoadLevel ("TEST.lvl");
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	void SaveLevel(byte[] levelData, string fileName)
	{
		File.WriteAllBytes(Application.persistentDataPath + "/" + fileName,levelData);

	}
	void LoadLevel(string fileName)
	{
		workingLevel=File.ReadAllBytes(Application.persistentDataPath + "/" + fileName);
		for (int i = 0; i < MAX_SIZE*MAX_SIZE; i++)
		{
			if ((int)workingLevel [SETTINGS_BLOCK_SIZE + (i * TILE_SIZE)] != 31) 
			{
				GameObject n = new GameObject ();
				n.AddComponent<BoxCollider2D> ();
				SpriteRenderer sr = n.AddComponent<SpriteRenderer> ();
				//Debug.Log ((int)workingLevel[SETTINGS_BLOCK_SIZE + (i*TILE_SIZE)]);
				sr.sprite = SpriteArray [(int)workingLevel [SETTINGS_BLOCK_SIZE + (i * TILE_SIZE)]];
				n.transform.position = new Vector3 (i % MAX_SIZE, i / MAX_SIZE, 0);
				n.transform.parent = RendHolder.transform;
			}
		}
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
}
