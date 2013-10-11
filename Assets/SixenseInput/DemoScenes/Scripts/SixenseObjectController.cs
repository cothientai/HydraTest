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
	
	public Transform palm;
	public Transform middleFinger;
	public LayerMask wallsLayer;
	
	protected bool bLockZAxis;
	protected Vector3 lockedPosition;
	protected bool hasTempLockedPos;
	
	public Transform camera;
	// Use this for initialization
	protected virtual void Start() 
	{
		m_initialRotation = this.gameObject.transform.localRotation;
		m_initialPosition = this.gameObject.transform.localPosition;
		Debug.Log("Initial position : " + m_initialPosition);
		Debug.Log("Locked position : " + lockedPosition);
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
			// enable position and orientation control
			m_enabled = !m_enabled;
			
			// delta controller position is relative to this point
			m_baseControllerPosition = new Vector3( controller.Position.x * Sensitivity.x,
													controller.Position.y * Sensitivity.y,
													controller.Position.z * Sensitivity.z );
//			Debug.Log("basecontroller : " + m_baseControllerPosition);
			// this is the new start position
			m_initialPosition = this.gameObject.transform.localPosition;
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
//				Debug.Log("X joystick position of controller" + controller.JoystickX);
	  			camera.transform.localEulerAngles = new Vector3(angV, angH, 0);
			}
		}
	}
	
	
	protected void UpdatePosition( SixenseInput.Controller controller )
	{
		CheckHitOnWall(controller);
		Vector3 controllerPosition = new Vector3( controller.Position.x * Sensitivity.x,
												  controller.Position.y * Sensitivity.y,
												  controller.Position.z * Sensitivity.z );
		if(bLockZAxis && !hasTempLockedPos )
		{
			//get the locked position if the boolean is true
			lockedPosition = controllerPosition;
			hasTempLockedPos = true;
		}
		else if(controllerPosition.z <= lockedPosition.z)
		{
			bLockZAxis = false;
			hasTempLockedPos = false;
		}
		
		// distance controller has moved since enabling positional control
		Vector3 vDeltaControllerPos;
		vDeltaControllerPos.x = controllerPosition.x - m_baseControllerPosition.x;
		vDeltaControllerPos.y = controllerPosition.y - m_baseControllerPosition.y;
		if (bLockZAxis) {
			vDeltaControllerPos.z = lockedPosition.z - m_baseControllerPosition.z;
		}
		else {
			vDeltaControllerPos.z = controllerPosition.z - m_baseControllerPosition.z;
		}
		
		// update the localposition of the object
		this.gameObject.transform.localPosition = m_initialPosition + vDeltaControllerPos;
	}
	protected void UpdateRotation( SixenseInput.Controller controller )
	{
		this.gameObject.transform.localRotation = controller.Rotation * m_initialRotation;
	}
	
	
	//TESTS
	void CheckHitOnWall(SixenseInput.Controller controller)
	{
		Ray ray = new Ray(middleFinger.position,middleFinger.transform.forward);
		Debug.DrawRay(middleFinger.position,middleFinger.transform.forward*0.1f);
		RaycastHit hit;
		if (Physics.Raycast(ray,out hit,0.1f,wallsLayer)) {
			bLockZAxis = true;
			//Debug.Log("LOCKED Z AXIS");
		}
//		else {
//			bLockZAxis=false;
//		}
	}
	void GrabWall(SixenseInput.Controller controller)
	{
			float fTriggerVal = controller.Trigger;
			fTriggerVal = Mathf.Lerp( m_fLastTriggerVal, fTriggerVal, 0.1f );
			
			Vector3 positionPalm = palm.position;
			Debug.DrawRay(positionPalm,-palm.transform.up);
			if(fTriggerVal >0.9f)
			{
				
				m_fLastTriggerVal = fTriggerVal;
//				Debug.Log("controllers position "+controller.Position);
				RaycastHit hit;
				if(Physics.Raycast(positionPalm,-palm.transform.up,out hit,wallsLayer) )
				{
					
					//hit.transform.parent = transform;
					//grabbedObject = hit.transform;
					//grabbedObject.position = -palm.transform.up;
					Debug.Log("hited the " + hit.transform);
				}
			}
	}
}