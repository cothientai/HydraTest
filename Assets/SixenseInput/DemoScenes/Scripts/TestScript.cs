using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {
    public Transform begin;
    public Transform end;
	// Use this for initialization
	void Start () {
        Debug.Log(Vector3.Cross(begin.position, end.position).normalized);
	}
	
	// Update is called once per frame
	void Update () {
		rigidbody.AddForce(Vector3.forward*10);
	}
	void OnTriggerEnter(Collider collision)	
	{
		
		
	}
}
