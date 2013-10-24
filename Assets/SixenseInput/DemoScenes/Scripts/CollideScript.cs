using UnityEngine;
using System.Collections;

public class CollideScript : MonoBehaviour {
    public Transform player;
    public Transform hand;

    public Vector3 tempLocation;
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
            tempLocation = hand.position;
            Debug.Log("TempPosition" + tempLocation);
            Debug.Log("rigidbody magnitude" + rigidbody.velocity.magnitude);
        }

    }
    void OnCollisionStay(Collision collisionInfo)
    {
        if (collisionInfo.transform.tag == tagWall)
        {
            Debug.Log("Colliding");
            Vector3 distance;
            distance = tempLocation - hand.position;
            player.position += distance;
            player.rigidbody.useGravity = false;
            Debug.Log(distance);
        }
    }
    void OnCollisionExit(Collision col)
    {
        player.rigidbody.useGravity = true;
    }
}
