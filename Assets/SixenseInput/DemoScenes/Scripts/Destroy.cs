using UnityEngine;
using System.Collections;

public class Destroy : MonoBehaviour {
    private string tagWall = "Wall";
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag == tagWall)
        {
            Destroy(col.gameObject);
        }
        Destroy(this.gameObject, 5f);
    }
}
