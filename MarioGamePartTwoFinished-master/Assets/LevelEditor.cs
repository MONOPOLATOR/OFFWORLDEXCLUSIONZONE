using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
[System.Serializable]
public class LevelEditor : MonoBehaviour 
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
		p = c.ScreenToWorldPoint (new Vector3 (mousePos.x, mousePos.y, c.nearClipPlane));
		GUILayout.BeginArea(new Rect(20, 20, 250, 120));
		GUILayout.Label("Screen pixels: " + c.pixelWidth + ":" + c.pixelHeight);
		GUILayout.Label("Mouse position: " + mousePos);
		GUILayout.Label("World position: " + p.ToString("F0"));
		GUILayout.EndArea();
	}
	void Update()
	{
		if (Input.GetMouseButtonDown (0)) 
		{
			int posToArrayPos = SETTINGS_BLOCK_SIZE + (((int)(p.x+.5)) + ((int)(p.y+.5) * MAX_SIZE ))* TILE_SIZE;
			int WorkingPos = (posToArrayPos/2)-SETTINGS_BLOCK_SIZE;
			//BRUSH
			workingLevel[posToArrayPos-TILE_SIZE] = 16;
			RendHolder.transform.Find ("" + (WorkingPos-1)).gameObject.GetComponent<SpriteRenderer>().sprite = SpriteArray[16];
			workingLevel[posToArrayPos] = 17;
			RendHolder.transform.Find ("" + (WorkingPos+0)).gameObject.GetComponent<SpriteRenderer>().sprite = SpriteArray[17];
			workingLevel[posToArrayPos+TILE_SIZE] = 18;
			RendHolder.transform.Find ("" + (WorkingPos+1)).gameObject.GetComponent<SpriteRenderer>().sprite = SpriteArray[18];
			workingLevel[posToArrayPos-TILE_SIZE+MAX_SIZE*TILE_SIZE] = 24;
			RendHolder.transform.Find ("" + (WorkingPos-1+MAX_SIZE)).gameObject.GetComponent<SpriteRenderer>().sprite = SpriteArray[24];
			workingLevel[posToArrayPos+MAX_SIZE*TILE_SIZE] = 25;
			RendHolder.transform.Find ("" + (WorkingPos+0+MAX_SIZE)).gameObject.GetComponent<SpriteRenderer>().sprite = SpriteArray[25];
			workingLevel[posToArrayPos+TILE_SIZE+MAX_SIZE*TILE_SIZE] = 26;
			RendHolder.transform.Find ("" + (WorkingPos+1+MAX_SIZE)).gameObject.GetComponent<SpriteRenderer>().sprite = SpriteArray[26];
			workingLevel[posToArrayPos-TILE_SIZE-MAX_SIZE*TILE_SIZE] = 8;
			RendHolder.transform.Find ("" + (WorkingPos-1-MAX_SIZE)).gameObject.GetComponent<SpriteRenderer>().sprite = SpriteArray[8];
			workingLevel[posToArrayPos-MAX_SIZE*TILE_SIZE] = 9;
			RendHolder.transform.Find ("" + (WorkingPos+0-MAX_SIZE)).gameObject.GetComponent<SpriteRenderer>().sprite = SpriteArray[9];
			workingLevel[posToArrayPos+TILE_SIZE-MAX_SIZE*TILE_SIZE] = 10;
			RendHolder.transform.Find ("" + (WorkingPos+1-MAX_SIZE)).gameObject.GetComponent<SpriteRenderer>().sprite = SpriteArray[10];
			//RendHolder.transform.Find ("" + ((posToArrayPos/2)-SETTINGS_BLOCK_SIZE)).gameObject.GetComponent<SpriteRenderer>().sprite = SpriteArray[0];
			Debug.Log ("Placed at "+posToArrayPos);
		}
		if (Input.GetMouseButtonDown (1)) 
		{
			int posToArrayPos = SETTINGS_BLOCK_SIZE + (((int)(p.x+.5)) + ((int)(p.y+.5) * MAX_SIZE ))* TILE_SIZE;
			int WorkingPos = (posToArrayPos/2)-SETTINGS_BLOCK_SIZE;
			//BRUSH
			workingLevel[posToArrayPos-TILE_SIZE] = 31;
			RendHolder.transform.Find ("" + (WorkingPos-1)).gameObject.GetComponent<SpriteRenderer>().sprite = SpriteArray[31];
			workingLevel[posToArrayPos] = 31;
			RendHolder.transform.Find ("" + (WorkingPos+0)).gameObject.GetComponent<SpriteRenderer>().sprite = SpriteArray[31];
			workingLevel[posToArrayPos+TILE_SIZE] = 31;
			RendHolder.transform.Find ("" + (WorkingPos+1)).gameObject.GetComponent<SpriteRenderer>().sprite = SpriteArray[31];
			workingLevel[posToArrayPos-TILE_SIZE+MAX_SIZE*TILE_SIZE] = 31;
			RendHolder.transform.Find ("" + (WorkingPos-1+MAX_SIZE)).gameObject.GetComponent<SpriteRenderer>().sprite = SpriteArray[31];
			workingLevel[posToArrayPos+MAX_SIZE*TILE_SIZE] = 31;
			RendHolder.transform.Find ("" + (WorkingPos+0+MAX_SIZE)).gameObject.GetComponent<SpriteRenderer>().sprite = SpriteArray[31];
			workingLevel[posToArrayPos+TILE_SIZE+MAX_SIZE*TILE_SIZE] = 31;
			RendHolder.transform.Find ("" + (WorkingPos+1+MAX_SIZE)).gameObject.GetComponent<SpriteRenderer>().sprite = SpriteArray[31];
			workingLevel[posToArrayPos-TILE_SIZE-MAX_SIZE*TILE_SIZE] = 31;
			RendHolder.transform.Find ("" + (WorkingPos-1-MAX_SIZE)).gameObject.GetComponent<SpriteRenderer>().sprite = SpriteArray[31];
			workingLevel[posToArrayPos-MAX_SIZE*TILE_SIZE] = 31;
			RendHolder.transform.Find ("" + (WorkingPos+0-MAX_SIZE)).gameObject.GetComponent<SpriteRenderer>().sprite = SpriteArray[31];
			workingLevel[posToArrayPos+TILE_SIZE-MAX_SIZE*TILE_SIZE] = 31;
			RendHolder.transform.Find ("" + (WorkingPos+1-MAX_SIZE)).gameObject.GetComponent<SpriteRenderer>().sprite = SpriteArray[31];
			//RendHolder.transform.Find ("" + ((posToArrayPos/2)-SETTINGS_BLOCK_SIZE)).gameObject.GetComponent<SpriteRenderer>().sprite = SpriteArray[0];
			Debug.Log ("Placed at "+posToArrayPos);
			SaveLevel (workingLevel,"TEST.lvl");
			Debug.Log ("Saved.");
		}
	}


	//FILE SHIT
	void SaveLevel(byte[] levelData, string fileName)
	{
		File.WriteAllBytes(Application.persistentDataPath + "/" + fileName,levelData);

	}
	void LoadLevel(string fileName)
	{
		workingLevel=File.ReadAllBytes(Application.persistentDataPath + "/" + fileName);
		for (int i = 0; i < MAX_SIZE*MAX_SIZE; i++)
		{
			//if ((int)workingLevel [SETTINGS_BLOCK_SIZE + (i * TILE_SIZE)] != 31) 
			//{
				GameObject n = new GameObject ();
				n.name = "" + i;
				SpriteRenderer sr = n.AddComponent<SpriteRenderer> ();
				//Debug.Log ((int)workingLevel[SETTINGS_BLOCK_SIZE + (i*TILE_SIZE)]);
				sr.sprite = SpriteArray [(int)workingLevel [SETTINGS_BLOCK_SIZE + (i * TILE_SIZE)]];
				n.transform.position = new Vector3 (i % MAX_SIZE, i / MAX_SIZE, 0);
				n.transform.parent = RendHolder.transform;
			//}
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