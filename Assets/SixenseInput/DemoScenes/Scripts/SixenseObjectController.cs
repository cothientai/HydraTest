using UnityEngine;
using System.Collections;

public class SixenseObjectController : MonoBehaviour {

	public SixenseHands			Hand;
	public Vector3				Sensitivity = new Vector3( 0.01f, 0.01f, 0.01f );
	
	protected bool				m_enabled = false;
	protected Quaternion		m_initialRotation;
	protected Vector3			m_initialPosition;
	protected Vector3			m_baseControllerPosition;
		
	protected float				m_fLastTriggerVal = 0.0f;

    protected Vector3 previousPosition;
    protected Vector3 controllerPosition;

    public Vector3 startPosition;
    public Quaternion startRotation;
    public Transform spawnpoint;

	protected bool bLockZAxis;
	protected Vector3 lockedPosition;
	protected bool hasTempLockedPos;
	
	public Transform cameraObject;
    public Transform player;
    public Transform trail;

    public Transform[] waypoints;
    private bool waypointTracker = false;
	// Use this for initialization
	protected virtual void Start() 
	{
		m_initialRotation = this.gameObject.transform.localRotation;
		m_initialPosition = this.gameObject.transform.localPosition;

        startPosition = this.gameObject.transform.localPosition;
		startRotation = this.gameObject.transform.rotation;
		Debug.Log("init rotation : " + m_initialRotation);
       	Debug.Log("start Rotation : " + startRotation);
	}
	
	// Update is called once per frame
	public virtual void Update () 
	{
		if ( Hand == SixenseHands.UNKNOWN )
		{
			return;
		}
		
		SixenseInput.Controller controller = SixenseInput.GetController( Hand );
		if ( controller != null && controller.Enabled )  
		{		
			UpdateObject(controller);
		}	
	}
	
	
	void OnGUI()
	{
		if ( !m_enabled )
		{
			GUI.Box( new Rect( Screen.width / 2 - 100, Screen.height - 40, 200, 30 ),  "Press Start To Move/Rotate" );
		}
	}
	
	
	protected virtual void UpdateObject(  SixenseInput.Controller controller )
	{
		
		if ( controller.GetButtonDown( SixenseButtons.START ) )
		{
            this.gameObject.transform.localPosition = startPosition;
			// enable position and orientation control
			m_enabled = !m_enabled;
			
			// delta controller position is relative to this point
			m_baseControllerPosition = new Vector3( controller.Position.x * Sensitivity.x,
													controller.Position.y * Sensitivity.y,
													controller.Position.z * Sensitivity.z );

			// this is the new start position
			m_initialPosition = this.gameObject.transform.localPosition;
            previousPosition = m_baseControllerPosition;
		}
        if (controller.GetButtonDown(SixenseButtons.ONE))// && controller.Hand == SixenseHands.LEFT)
        {
			m_enabled = false;
            player.position = spawnpoint.position;
            player.rotation = spawnpoint.rotation;

            //m_enabled = false;
//			Quaternion localRotation = new Quaternion(0,0,0,1f) ;
//            transform.localRotation = localRotation;
			 transform.rotation = Quaternion.Euler(0,0,0);
			if(controller.Hand == SixenseHands.LEFT)
			{
				transform.GetChild(0).rotation = Quaternion.Euler(0,0,90);
			}
			else
			{
				transform.GetChild(0).rotation = Quaternion.Euler(0,0,270);
			}
					
			Debug.Log("init rotation : " + m_initialRotation);
       		Debug.Log("start Rotation : " + startRotation);
        }
		if ( m_enabled )
		{
			UpdatePosition( controller );
			UpdateRotation( controller );
			//angle to watch with right joystick
			if ( controller.Hand == SixenseHands.RIGHT )
			{
				var angH = controller.JoystickX * 60;
	  			var angV = controller.JoystickY * 45;
	  			cameraObject.transform.localEulerAngles = new Vector3(angV, angH, 0);
			}
            CheckAttacks(controller);
		}
	}

    private void CheckAttacks(SixenseInput.Controller controller)
    {
        if (controller.Hand == SixenseHands.RIGHT && controller.GetButtonDown(SixenseButtons.BUMPER))
        {
            Debug.Log("button down");
            trail.GetComponent<TrailRenderer>().enabled = true;
            foreach(Transform waypoint in waypoints)
            {
                waypoint.gameObject.SetActive(true);
            }
            waypointTracker = true;
        }
        if(waypointTracker == true)
        {
            bool canBefired = true;
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i].GetComponent<Waypoint>().isTriggerd == false)
                {
                    canBefired = false;
                }
            }
            if (canBefired)
            {
                waypointTracker = false;
                Debug.Log("SHOTS FIRED");

                foreach(Transform waypoint in waypoints)
                {
                    waypoint.GetComponent<Waypoint>().isTriggerd = false;
                    waypoint.gameObject.SetActive(false);
                    trail.GetComponent<TrailRenderer>().enabled = false;
                }
            }
        }
    }
	
	
	protected void UpdatePosition( SixenseInput.Controller controller )
	{
		controllerPosition = new Vector3( controller.Position.x * Sensitivity.x,
												  controller.Position.y * Sensitivity.y,
												  controller.Position.z * Sensitivity.z );
       
		// distance controller has moved since enabling positional control
        Vector3 vDeltaControllerPos;
        vDeltaControllerPos.x = controllerPosition.x - m_baseControllerPosition.x;
        vDeltaControllerPos.y = controllerPosition.y - m_baseControllerPosition.y;
        vDeltaControllerPos.z = controllerPosition.z - m_baseControllerPosition.z;

        this.gameObject.transform.localPosition = m_initialPosition + vDeltaControllerPos;
	}
	protected void UpdateRotation( SixenseInput.Controller controller )
	{
		this.gameObject.transform.localRotation = controller.Rotation * m_initialRotation;
		Debug.Log("Raw rotation " + controller.RotationRaw);
//		Debug.Log("local Rot : " + this.gameObject.transform.localRotation);
//		Debug.Log("controller rotation : " + controller.Rotation);
//		Debug.Log("init rotation : " + m_initialRotation);
	}
	
	
	//TESTS
    
//    void OnCollisionEnter(Collision collision)
//    {
//        Debug.Log("collision");
//        foreach (ContactPoint contact in collision.contacts)
//        {
//            Debug.DrawRay(contact.point, contact.normal, Color.red);
//            Debug.Log("contact.point : " + contact.point);
//            Debug.Log("contact Normal : " + contact.normal);
//            if (contact.normal.z < -0.1f)
//            {
//                bLockZAxis = true;
//                Debug.Log("Locked Z axis");
//                lockedPosition = controllerPosition;
//            }
//        }
//
//    }
//	void CheckHitOnWall(SixenseInput.Controller controller)
//	{
//		Ray ray = new Ray(middleFinger.position,middleFinger.transform.forward);
//		Debug.DrawRay(middleFinger.position,middleFinger.transform.forward*0.1f);
//		RaycastHit hit;
//		if (Physics.Raycast(ray,out hit,0.1f,wallsLayer)) {
//			bLockZAxis = true;
//			//Debug.Log("LOCKED Z AXIS");
//		}
////		else {
////			bLockZAxis=false;
////		}
//	}
//	void GrabWall(SixenseInput.Controller controller)
//	{
//			float fTriggerVal = controller.Trigger;
//			fTriggerVal = Mathf.Lerp( m_fLastTriggerVal, fTriggerVal, 0.1f );
//			
//			Vector3 positionPalm = palm.position;
//			Debug.DrawRay(positionPalm,-palm.transform.up);
//			if(fTriggerVal >0.9f)
//			{
//				
//				m_fLastTriggerVal = fTriggerVal;
////				Debug.Log("controllers position "+controller.Position);
//				RaycastHit hit;
//				if(Physics.Raycast(positionPalm,-palm.transform.up,out hit,wallsLayer) )
//				{
//					
//					//hit.transform.parent = transform;
//					//grabbedObject = hit.transform;
//					//grabbedObject.position = -palm.transform.up;
//					Debug.Log("hited the " + hit.transform);
//				}
//			}
//	}
}