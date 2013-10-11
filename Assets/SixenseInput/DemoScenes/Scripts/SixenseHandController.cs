
//
// Copyright (C) 2013 Sixense Entertainment Inc.
// All Rights Reserved
//

using UnityEngine;
using System.Collections;

public class SixenseHandController : SixenseObjectController
{
	protected Animator			m_animator = null;
	
	

	
	
	private Transform grabbedObject;

	protected override void Start() 
	{
		// get the Animator
		m_animator = this.gameObject.GetComponent<Animator>();
		Debug.Log("Local position : " + transform.localPosition );
		base.Start();
	}
	
	protected override void UpdateObject( SixenseInput.Controller controller )
	{
		if ( m_animator == null )
		{
			return;
		}
		
		if ( controller.Enabled )  
		{		
			// Animation update
			UpdateAnimationInput( controller );
		}
				
		base.UpdateObject(controller);
	}
	
	
	void OnGUI()
	{
		if ( Hand == SixenseHands.UNKNOWN )
		{
			return;
		}
		
		if ( !m_enabled )
		{
			int labelWidth = 250;
			int labelPadding = 120;
			int horizOffset = Hand == SixenseHands.LEFT ? -labelWidth - labelPadding  : labelPadding;
			
			string handStr = Hand == SixenseHands.LEFT ? "left" : "right";
			GUI.Box( new Rect( Screen.width / 2 + horizOffset, Screen.height - 40, labelWidth, 30 ),  "Press " + handStr + " START to control " + gameObject.name );		
		}		
	}
	
	// Updates the animated object from controller input.
	protected void UpdateAnimationInput( SixenseInput.Controller controller)
	{
		// Point
		if ( Hand == SixenseHands.RIGHT ? controller.GetButton(SixenseButtons.ONE) : controller.GetButton(SixenseButtons.TWO) )
		{
			m_animator.SetBool( "Point", true );
		}
		else
		{
			m_animator.SetBool( "Point", false );
		}
		
		// Grip Ball
		if ( Hand == SixenseHands.RIGHT ? controller.GetButton(SixenseButtons.TWO) : controller.GetButton(SixenseButtons.ONE)  )
		{
			m_animator.SetBool( "GripBall", true );
		}
		else
		{
			m_animator.SetBool( "GripBall", false );
		}
				
		// Hold Book
		if ( Hand == SixenseHands.RIGHT ? controller.GetButton(SixenseButtons.THREE) : controller.GetButton(SixenseButtons.FOUR) )
		{
			m_animator.SetBool( "HoldBook", true );
		}
		else
		{
			m_animator.SetBool( "HoldBook", false );
		}
				
		// Fist
		float fTriggerVal = controller.Trigger;
		fTriggerVal = Mathf.Lerp( m_fLastTriggerVal, fTriggerVal, 0.1f );
		m_fLastTriggerVal = fTriggerVal;
		
		if ( fTriggerVal > 0.01f )
		{
			m_animator.SetBool( "Fist", true );
		}
		else
		{
			m_animator.SetBool( "Fist", false );
		}
		
		m_animator.SetFloat("FistAmount", fTriggerVal);
		
		// Idle
		if ( m_animator.GetBool("Fist") == false &&  
			 m_animator.GetBool("HoldBook") == false && 
			 m_animator.GetBool("GripBall") == false && 
			 m_animator.GetBool("Point") == false )
		{
			m_animator.SetBool("Idle", true);
		}
		else
		{
			m_animator.SetBool("Idle", false);
		}
	}
	//OWN TESTS
	
	public override void Update()
	{
		base.Update();
		
		SixenseInput.Controller controller = SixenseInput.GetController( Hand );
		if( controller != null)
		{
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
//				if(Physics.Raycast(positionPalm,-palm.transform.up,out hit,wallsLayer) && grabbedObject == null)
//				{
//					
//					//hit.transform.parent = transform;
//					//grabbedObject = hit.transform;
//					//grabbedObject.position = -palm.transform.up;
////					Debug.Log("hited the " + hit.transform);
//				}
//			}
		}
		
	}
	
	
//	void OnTriggerEnter(Collider collision)	
//	{
//		
//		
//	}
//	void OnTriggerStay(Collider collision) {

//		Debug.Log(fTriggerVal);
//		if (fTriggerVal > 0.9f ) {
//			
//			collision.transform.parent = transform;
//		}
//		else {
//			collision.transform.parent = null;
//		}
//	}
	
}

