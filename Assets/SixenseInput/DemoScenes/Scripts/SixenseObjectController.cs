using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum Direction
{
    None,
    Up,
    Right,
    Down,
    Left
}

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

    //protected bool bLockZAxis;
    //protected Vector3 lockedPosition;
    //protected bool hasTempLockedPos;
	
	public Transform cameraObject;
    public Transform player;
    public Transform trail;
    public Transform leftHand;

    //public Transform[] waypoints;
    //private bool waypointTracker = false;

    public Vector3 previousPoint;
    private Direction previousDirection = Direction.None;
    public List<Direction> points;
    public Attack[] attacks;
    public bool canFireAttack = false;
    public float offset = 30f;
    public float minDistance = 0.5f;
	// Use this for initialization
	protected virtual void Start() 
	{
		m_initialRotation = this.gameObject.transform.localRotation;
		m_initialPosition = this.gameObject.transform.localPosition;

        startPosition = this.gameObject.transform.localPosition;
		startRotation = this.gameObject.transform.rotation;
		Debug.Log("init rotation : " + m_initialRotation);
       	Debug.Log("start Rotation : " + startRotation);

        points = new List<Direction>();
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
        if(Hand == SixenseHands.RIGHT)
        {

            GUI.Label(new Rect(10, 10, 200, 20), "Spell direction : "+ previousDirection.ToString());
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
            ResetPositionAndRotation(controller);
        }
		if ( m_enabled )
		{
			UpdatePosition( controller );
			UpdateRotation( controller );
			//angle to watch with right joystick
            UpdatePlayerMovement(controller);
            
            RecordMotion(controller);
		}
	}

    private void ResetPositionAndRotation(SixenseInput.Controller controller)
    {
        m_enabled = false;
        player.position = spawnpoint.position;
        player.rotation = spawnpoint.rotation;

        transform.GetChild(0).localRotation = Quaternion.Euler(0, 0, 0);

        //m_enabled = false;
        //Quaternion localRotation = new Quaternion(0, 0, 0, 1f);
        //            transform.localRotation = localRotation;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        if (controller.Hand == SixenseHands.LEFT)
        {
            transform.GetChild(0).rotation = Quaternion.Euler(0, 0, 90);

        }
        else
        {
            transform.GetChild(0).rotation = Quaternion.Euler(0, 0, 270);
        }

        Debug.Log("init rotation : " + m_initialRotation);
        Debug.Log("start Rotation : " + startRotation);
    }
    private void UpdatePlayerMovement(SixenseInput.Controller controller)
    {
        if (controller.Hand == SixenseHands.RIGHT)
        {
            float angH = controller.JoystickX * 60;
            float angV = controller.JoystickY * 45;
            cameraObject.transform.localEulerAngles = new Vector3(angV, 0, 0);
            player.transform.Rotate(new Vector3(0, angH, 0) * Time.deltaTime);
        }
        if(controller.Hand == SixenseHands.LEFT)
        {
            float joystickXAxisL = controller.JoystickX * 10;
            float joystickYAxisL = controller.JoystickY * 10;

            player.transform.rigidbody.AddRelativeForce(joystickXAxisL,0,joystickYAxisL);
        }
    }
    public void RecordMotion(SixenseInput.Controller controller)
    {
        //right bumper
        if (controller.Hand == SixenseHands.RIGHT && controller.GetButtonDown(SixenseButtons.BUMPER))
        {
            previousPoint = this.transform.localPosition;
            
            //enable Trailrenderer + waypoints

            trail.GetComponent<TrailRenderer>().enabled = true;
            //foreach(Transform waypoint in waypoints)
            //{
            //    waypoint.gameObject.SetActive(true);
            //}
            //waypointTracker = true;
        }
        if(controller.GetButton(SixenseButtons.BUMPER))
        {
            Vector3 currentPoint = this.transform.localPosition;
            //if the distance from the current point is far eneogh from the previous
            if(Vector3.Distance(currentPoint,previousPoint)>minDistance)
            {
                //Debug.Log("Distance : " + Vector3.Distance(currentPoint, previousPoint));
                Vector3 tempVector = new Vector3(currentPoint.x - previousPoint.x, currentPoint.y - previousPoint.y, 0);
                float angle = Vector3.Angle(Vector3.up, tempVector.normalized);
                float dotProduct = Vector2.Dot(Vector3.right, tempVector.normalized);
                //New point to check the distance
                previousPoint = currentPoint;
                //checks the direction
                if (angle >= 0 && angle < offset && previousDirection != Direction.Up)
                {                    
                    //direction is UP
                    points.Add(Direction.Up);
                    //Debug.Log("UP");
                    previousDirection = Direction.Up;
                }
                else if (angle <= 180 && angle >= 180 - offset && previousDirection != Direction.Down)
                {
                    //Direction is DOWN
                    points.Add(Direction.Down);
                    //Debug.Log("DOWN");
                    previousDirection = Direction.Down;
                }
                else if (angle >= 90 - offset && angle < 90 + offset && dotProduct > 0 && previousDirection != Direction.Right)
                {
                    //Direction is RIGHT
                    points.Add(Direction.Right);
                    //Debug.Log("RIGHT");
                    previousDirection = Direction.Right;
                }
                else if (angle >= 90 - offset && angle < 90 + offset && dotProduct < 0 && previousDirection != Direction.Left)
                {
                    //Direction is LEFT
                    points.Add(Direction.Left);
                    //Debug.Log("LEFT");
                    previousDirection = Direction.Left;
                }
                //else if (previousDirection != Direction.None)
                //{
                //    points.Add(Direction.None);
                //    Debug.Log("NONE");
                //    previousDirection = Direction.None;
                //}
            }
        }
        if (controller.Hand == SixenseHands.RIGHT && controller.GetButtonUp(SixenseButtons.BUMPER))
        {
            
            CheckAttacks();
        }
    }
    private void CheckAttacks()
    {
        //Check every attack
        foreach(Attack attack in attacks)
        {
            //if there is an attack with the same length
            if(attack.directions.Length == points.Count)
            {
                //Check for the right directions for the attack
                for(int i = 0;i<attack.directions.Length;i++)
                {
                    canFireAttack = true;
                    //if it has the same direction
                    if(attack.directions[i] != points[i])
                    {
                        canFireAttack = false;
                    }                   
                }
                if (canFireAttack)
                {
                    Debug.Log("FIRE ATTACK : " + attack.attackName);
                    Transform attackObject;
                    attackObject = Instantiate(attack.fireObject, leftHand.position + -leftHand.up, leftHand.rotation) as Transform;
                    attackObject.rigidbody.AddForce(-leftHand.up * 3000);
                }
            }
        }

        points.Clear();
        previousDirection = Direction.None;
        trail.GetComponent<TrailRenderer>().enabled = false;
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