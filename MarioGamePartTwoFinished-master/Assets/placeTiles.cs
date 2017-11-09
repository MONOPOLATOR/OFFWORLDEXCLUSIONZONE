using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class placeTiles : MonoBehaviour 
{
	Vector3 p = new Vector3 ();
	Camera c = Camera.main;
	Event e = Event.current;
	Vector2 mousePos = new Vector2 ();
	void OnGUI()
	{
		
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
		if (Input.GetMouseButtonDown(0))
			Debug.Log("Pressed left click.");
	}
}