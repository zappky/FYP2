using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class my_paragraph
{
	public List<string> texts = new List<string>();
}
public class my_dialogScript
{
	public string personName = null;
	public List<my_paragraph> blobs = new List<my_paragraph>();
}

public class DialogDatabase : MonoBehaviour {

	public List<my_dialogScript> dialogDatabase = new List<my_dialogScript>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
