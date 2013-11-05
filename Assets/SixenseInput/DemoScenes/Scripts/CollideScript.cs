using UnityEngine;
using System.Collections;

public class CollideScript : MonoBehaviour {
    public Transform player;
    public Transform hand;
    private bool isHanging = false;
    public Vector3 tempLocation;
    private string tagWall = "Wall";
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (isHanging)
        {
            Vector3 distance;
            distance = (tempLocation - hand.position) + player.position;
            player.rigidbody.MovePosition(distance);
        }
	}
    void FixedUpdate()
    {
        
    }
    void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag == tagWall)
        {
            isHanging = true;
            tempLocation = hand.position;
            //Debug.Log("TempPosition" + tempLocation);
            //Debug.Log("rigidbody magnitude" + rigidbody.velocity.magnitude);
            float angle = Vector3.Angle(col.contacts[0].normal, Vector3.forward);
            Debug.Log("Angle : " + angle);
        }

    }
    void OnCollisionStay(Collision collisionInfo)
    {
        if (collisionInfo.transform.tag == tagWall)
        {
            //Debug.Log("Colliding");
            
           // player.rigidbody.useGravity = false;
            //Debug.Log(distance);
        }
    }
    void OnCollisionExit(Collision col)
    {
        isHanging = false;
        player.rigidbody.useGravity = true;
    }
}
