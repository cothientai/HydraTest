using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		rigidbody.AddForce(Vector3.forward*10);
	}
	void OnTriggerEnter(Collider collision)	
	{
		
		
	}
}
