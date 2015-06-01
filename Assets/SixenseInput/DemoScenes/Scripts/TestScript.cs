using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {
    public Transform begin;
    public Transform hand;
	// Use this for initialization
	void Start () {
        Debug.Log(Vector3.Cross(begin.position, hand.position).normalized);
	}
	
	// Update is called once per frame
	void Update () {
        //rigidbody.AddForce(Vector3.forward*10);
        Vector3 beginpos = begin.transform.position;
        Vector3 endpos = hand.transform.position;

        if (hand.InverseTransformPoint(beginpos).x > hand.transform.localPosition.x)
        {
            Debug.Log("dir is left");
        }
        else
            Debug.Log("dir is right");
	}
	void OnTriggerEnter(Collider collision)	
	{
		
		
	}
}
