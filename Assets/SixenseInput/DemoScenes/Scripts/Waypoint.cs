using UnityEngine;
using System.Collections;

public class Waypoint : MonoBehaviour {
    public bool isTriggerd = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Debug.DrawRay(transform.position, transform.forward);
	}
    void OnCollisionEnter(Collision collision)
    {
        float angle = Vector3.Angle(collision.contacts[0].normal, transform.forward);
        Debug.Log("Angle : " + angle);
        if(Mathf.Approximately(angle,180))
        {
            Debug.Log("Hitted the front!");
        }
    }
    void OnTriggerEnter(Collider collider)
    {
        Debug.Log("hitted the collider");
        isTriggerd = true;
		gameObject.SetActive(false);
    }
}
