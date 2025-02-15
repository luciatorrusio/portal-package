using UnityEngine;
using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace AlsetRGames.Portal.Support
{
	public class EventForwarder : MonoBehaviour
	{
		
		private GameObject original;
		
		public void SetOriginalObject(GameObject gameObject)
		{
			original = gameObject;
		}

		public void OnAnimatorIK(int layerIndex)
		{
			original.SendMessage("OnAnimatorIK", layerIndex, SendMessageOptions.DontRequireReceiver);
		}
 
		public void OnAnimatorMove()
		{
			original.SendMessage("OnAnimatorMove", null, SendMessageOptions.DontRequireReceiver);
		}
 
		public void OnApplicationFocus(bool focusStatus)
		{
			original.SendMessage("OnApplicationFocus", focusStatus, SendMessageOptions.DontRequireReceiver);
		}
 
		public void OnApplicationPause(bool pauseStatus)
		{
			original.SendMessage("OnApplicationPause", pauseStatus, SendMessageOptions.DontRequireReceiver);
		}
 
		public void OnApplicationQuit()
		{
			original.SendMessage("OnApplicationQuit", null, SendMessageOptions.DontRequireReceiver);
		}
 
		public void OnBecameInvisible()
		{
			original.SendMessage("OnBecameInvisible", null, SendMessageOptions.DontRequireReceiver);
		}
 
		public void OnBecameVisible()
		{
			original.SendMessage("OnBecameVisible", null, SendMessageOptions.DontRequireReceiver);
		}
 
		public void OnCollisionEnter(Collision collision)
		{
			original.SendMessage("OnCollisionEnter", collision, SendMessageOptions.DontRequireReceiver);
		}
 
		public void OnCollisionEnter2D(Collision2D collision)
		{
			original.SendMessage("OnCollisionEnter2D", collision, SendMessageOptions.DontRequireReceiver);
		}
 
		public void OnCollisionExit(Collision collision)
		{
			original.SendMessage("OnCollisionExit", collision, SendMessageOptions.DontRequireReceiver);
		}
 
		public void OnCollisionExit2D(Collision2D collision)
		{
			original.SendMessage("OnCollisionExit2D", collision, SendMessageOptions.DontRequireReceiver);
		}
 
		public void OnCollisionStay(Collision collision)
		{
			original.SendMessage("OnCollisionStay", collision, SendMessageOptions.DontRequireReceiver);
		}
 
		public void OnCollisionStay2D(Collision2D collision)
		{
			original.SendMessage("OnCollisionStay2D", collision, SendMessageOptions.DontRequireReceiver);
		}
 
		public void OnConnectedToServer()
		{
			original.SendMessage("OnConnectedToServer", null, SendMessageOptions.DontRequireReceiver);
		}
 
		public void OnControllerColliderHit(ControllerColliderHit hit)
		{
			original.SendMessage("OnControllerColliderHit", hit, SendMessageOptions.DontRequireReceiver);
		}

		public void OnJointBreak(float breakForce)
		{
			original.SendMessage("breakForce", null, SendMessageOptions.DontRequireReceiver);
		}
 
		public void OnMouseDown()
		{
			original.SendMessage("OnMouseDown", null, SendMessageOptions.DontRequireReceiver);
		}
 
		public void OnMouseDrag()
		{
			original.SendMessage("OnMouseDrag", null, SendMessageOptions.DontRequireReceiver);
		}
 
		public void OnMouseEnter()
		{
			original.SendMessage("OnMouseEnter", null, SendMessageOptions.DontRequireReceiver);
		}
 
		public void OnMouseExit()
		{
			original.SendMessage("OnMouseExit", null, SendMessageOptions.DontRequireReceiver);
		}
 
		public void OnMouseOver()
		{
			original.SendMessage("OnMouseOver", null, SendMessageOptions.DontRequireReceiver);
		}
 
		public void OnMouseUp()
		{
			original.SendMessage("OnMouseUp", null, SendMessageOptions.DontRequireReceiver);
		}
 
		public void OnMouseUpAsButton()
		{
			original.SendMessage("OnMouseUpAsButton", null, SendMessageOptions.DontRequireReceiver);
		}
 
		public void OnParticleCollision(GameObject other)
		{
			original.SendMessage("OnParticleCollision", other, SendMessageOptions.DontRequireReceiver);
		}
 
		public void OnTriggerEnter(Collider other)
		{
			original.SendMessage("OnTriggerEnter", other, SendMessageOptions.DontRequireReceiver);
		}
 
		public void OnTriggerEnter2D(Collider2D other)
		{
			original.SendMessage("OnTriggerEnter2D", other, SendMessageOptions.DontRequireReceiver);
		}
 
		public void OnTriggerExit(Collider other)
		{
			original.SendMessage("OnTriggerExit", other, SendMessageOptions.DontRequireReceiver);
		}
 
		public void OnTriggerExit2D(Collider2D other)
		{
			original.SendMessage("OnTriggerExit2D", other, SendMessageOptions.DontRequireReceiver);
		}
 
		public void OnTriggerStay(Collider other)
		{
			original.SendMessage("OnTriggerStay", other, SendMessageOptions.DontRequireReceiver);
		}
 
		public void OnTriggerStay2D(Collider2D other)
		{
			original.SendMessage("OnTriggerStay2D", other, SendMessageOptions.DontRequireReceiver);
		}
 
		public void OnWillRenderObject()
		{
			original.SendMessage("OnWillRenderObject", null, SendMessageOptions.DontRequireReceiver);
		}

		
		
 
	}
}