using UnityEngine;

namespace Core.Portal.Utils
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
			original.SendMessage("OnAnimatorIK", layerIndex);
		}
 
		public void OnAnimatorMove()
		{
			original.SendMessage("OnAnimatorMove");
		}
 
		public void OnApplicationFocus(bool focusStatus)
		{
			original.SendMessage("OnApplicationFocus", focusStatus);
		}
 
		public void OnApplicationPause(bool pauseStatus)
		{
			original.SendMessage("OnApplicationPause", pauseStatus);
		}
 
		public void OnApplicationQuit()
		{
			original.SendMessage("OnApplicationQuit");
		}
 
		public void OnBecameInvisible()
		{
			original.SendMessage("OnBecameInvisible");
		}
 
		public void OnBecameVisible()
		{
			original.SendMessage("OnBecameVisible");
		}
 
		public void OnCollisionEnter(Collision collision)
		{
			original.SendMessage("OnCollisionEnter", collision);
		}
 
		public void OnCollisionEnter2D(Collision2D collision)
		{
			original.SendMessage("OnCollisionEnter2D", collision);
		}
 
		public void OnCollisionExit(Collision collision)
		{
			original.SendMessage("OnCollisionExit", collision);
		}
 
		public void OnCollisionExit2D(Collision2D collision)
		{
			original.SendMessage("OnCollisionExit2D", collision);
		}
 
		public void OnCollisionStay(Collision collision)
		{
			original.SendMessage("OnCollisionStay", collision);
		}
 
		public void OnCollisionStay2D(Collision2D collision)
		{
			original.SendMessage("OnCollisionStay2D", collision);
		}
 
		public void OnConnectedToServer()
		{
			original.SendMessage("OnConnectedToServer");
		}
 
		public void OnControllerColliderHit(ControllerColliderHit hit)
		{
			original.SendMessage("OnControllerColliderHit", hit);
		}

		public void OnJointBreak(float breakForce)
		{
			original.SendMessage("breakForce");
		}
 
		public void OnMouseDown()
		{
			original.SendMessage("OnMouseDown");
		}
 
		public void OnMouseDrag()
		{
			original.SendMessage("OnMouseDrag");
		}
 
		public void OnMouseEnter()
		{
			original.SendMessage("OnMouseEnter");
		}
 
		public void OnMouseExit()
		{
			original.SendMessage("OnMouseExit");
		}
 
		public void OnMouseOver()
		{
			original.SendMessage("OnMouseOver");
		}
 
		public void OnMouseUp()
		{
			original.SendMessage("OnMouseUp");
		}
 
		public void OnMouseUpAsButton()
		{
			original.SendMessage("OnMouseUpAsButton");
		}
 
		public void OnParticleCollision(GameObject other)
		{
			original.SendMessage("OnParticleCollision", other);
		}
 
		public void OnTriggerEnter(Collider other)
		{
			original.SendMessage("OnTriggerEnter", other);
		}
 
		public void OnTriggerEnter2D(Collider2D other)
		{
			original.SendMessage("OnTriggerEnter2D", other);
		}
 
		public void OnTriggerExit(Collider other)
		{
			original.SendMessage("OnTriggerExit", other);
		}
 
		public void OnTriggerExit2D(Collider2D other)
		{
			original.SendMessage("OnTriggerExit2D", other);
		}
 
		public void OnTriggerStay(Collider other)
		{
			original.SendMessage("OnTriggerStay", other);
		}
 
		public void OnTriggerStay2D(Collider2D other)
		{
			original.SendMessage("OnTriggerStay2D", other);
		}
 
		public void OnWillRenderObject()
		{
			original.SendMessage("OnWillRenderObject");
		}
		
 
	}
}