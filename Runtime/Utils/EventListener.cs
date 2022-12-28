using System;
using UnityEngine;

namespace Utils
{
    public class EventListener : MonoBehaviour
    {
        private EventForwarder _eventForwarder;

        public void SetEventForwarder(EventForwarder eventForwarder)
        {
            _eventForwarder = eventForwarder;
            _eventForwarder.OnBecameInvisibleEvent += HandleOnBecameInvisibleEvent;
            _eventForwarder.OnBecameVisibleEvent += HandleOnBecameVisibleEvent;
            _eventForwarder.OnCollisionEnterEvent += HandleOnCollisionEnterEvent;
            _eventForwarder.OnCollisionEnter2DEvent += HandleOnCollisionEnter2DEvent;
            _eventForwarder.OnCollisionExitEvent += HandleOnCollisionExitEvent;
            _eventForwarder.OnCollisionExit2DEvent += HandleOnCollisionExit2DEvent;
            _eventForwarder.OnCollisionStayEvent += HandleCollisionStayEvent;
            _eventForwarder.OnCollisionStay2DEvent += HandleOnCollisionStay2DEvent;
            _eventForwarder.OnControllerColliderHitEvent += HandleOnControllerColliderHitEvent;
            _eventForwarder.OnJointBreakEvent += HandleOnJointBreakEvent;
            _eventForwarder.OnMouseDownEvent += HandleOnMouseDownEvent;
            _eventForwarder.OnMouseDragEvent += HandleOnMouseDragEvent;
            _eventForwarder.OnMouseEnterEvent += HandleOnMouseEnterEvent;
            _eventForwarder.OnMouseExitEvent += HandleOnMouseExitEvent;
            _eventForwarder.OnMouseOverEvent += HandleOnMouseOverEvent;
            _eventForwarder.OnMouseUpEvent += HandleOnMouseUpEvent;
            _eventForwarder.OnMouseUpAsButtonEvent += HandleOnMouseUpAsButtonEvent;
            _eventForwarder.OnParticleCollisionEvent += HandleOnParticleCollisionEvent;
            _eventForwarder.OnTriggerEnterEvent += HandleOnTriggerEnterEvent;
            _eventForwarder.OnTriggerEnter2DEvent += HandleOnTriggerEnter2DEvent;
            _eventForwarder.OnTriggerExitEvent += HandleOnTriggerExitEvent;
            _eventForwarder.OnTriggerExit2DEvent += HandleOnTriggerExit2DEvent;
            _eventForwarder.OnTriggerStayEvent += HandleOnTriggerStayEvent;
            _eventForwarder.OnTriggerStay2DEvent += HandleOnTriggerStay2DEvent;
            
        }
        
        

        
        private void HandleOnBecameInvisibleEvent()
        {
	        gameObject.SendMessage("OnBecameInvisible");
        }
		public void OnBecameInvisible()
		{
		}
 
		private void HandleOnBecameVisibleEvent()
		{
			gameObject.SendMessage("OnBecameVisible");
		}
		public void OnBecameVisible()
		{
		}
 
		private void HandleOnCollisionEnterEvent(Collision collision)
		{
			gameObject.SendMessage("OnCollisionEnter", collision);
		}
		public void OnCollisionEnter(Collision collision)
		{
		}
 
		private void HandleOnCollisionEnter2DEvent(Collision2D collision)
		{
			gameObject.SendMessage("OnCollisionEnter2D", collision);
		}
		public void OnCollisionEnter2D(Collision2D collision)
		{
		}
 
		private void HandleOnCollisionExitEvent(Collision collision)
		{
			gameObject.SendMessage("OnCollisionExit", collision);
		}
		public void OnCollisionExit(Collision collision)
		{
		}
 
		private void HandleOnCollisionExit2DEvent(Collision2D collision)
		{
			gameObject.SendMessage("OnCollisionExit2D", collision);
		}
		public void OnCollisionExit2D(Collision2D collision)
		{
		}
		private void HandleCollisionStayEvent(Collision collision)
		{
			gameObject.SendMessage("OnCollisionStay", collision);
		}
 
		public void OnCollisionStay(Collision collision)
		{
		}
 
		private void HandleOnCollisionStay2DEvent(Collision2D collision)
		{
			gameObject.SendMessage("OnCollisionStay2D", collision);
		}
		public void OnCollisionStay2D(Collision2D collision)
		{
		}

		private void HandleOnControllerColliderHitEvent(ControllerColliderHit hit)
		{
			gameObject.SendMessage("OnControllerColliderHit", hit);
		}
		public void OnControllerColliderHit(ControllerColliderHit hit)
		{
		}

		private void HandleOnJointBreakEvent(float breakForce)
		{
			gameObject.SendMessage("OnJointBreak", breakForce);
		}
		public void OnJointBreak(float breakForce)
		{
		}
 
		private void HandleOnMouseDownEvent()
		{
			gameObject.SendMessage("OnMouseDown");
		}
		public void OnMouseDown()
		{
		}
 
		private void HandleOnMouseDragEvent()
		{
			gameObject.SendMessage("OnMouseDrag");
		}
		public void OnMouseDrag()
		{
		}
 
		private void HandleOnMouseEnterEvent()
		{
			gameObject.SendMessage("OnMouseEnter");
		}
		public void OnMouseEnter()
		{
		}
 
		private void HandleOnMouseExitEvent()
		{
			gameObject.SendMessage("OnMouseExit");
		}
		public void OnMouseExit()
		{
		}
 
		private void HandleOnMouseOverEvent()
		{
			gameObject.SendMessage("OnMouseOver");
		}
		public void OnMouseOver()
		{
		}
 
		private void HandleOnMouseUpEvent()
		{
			gameObject.SendMessage("OnMouseUp");
		}
		public void OnMouseUp()
		{
		}
 
		private void HandleOnMouseUpAsButtonEvent()
		{
			gameObject.SendMessage("OnMouseUpAsButton");
		}
		public void OnMouseUpAsButton()
		{
		}
 
		private void HandleOnParticleCollisionEvent(GameObject other)
		{
			gameObject.SendMessage("OnParticleCollision", other);
		}
		public void OnParticleCollision(GameObject other)
		{
		}
 
		private void HandleOnTriggerEnterEvent(Collider other)
		{
			gameObject.SendMessage("OnTriggerEnter", other);
		}
		public void OnTriggerEnter(Collider other)
		{
		}
 
		private void HandleOnTriggerEnter2DEvent(Collider2D other)
		{
			gameObject.SendMessage("OnTriggerEnter2D", other);
		}
		public void OnTriggerEnter2D(Collider2D other)
		{
		}
 
		private void HandleOnTriggerExitEvent(Collider other)
		{
			gameObject.SendMessage("OnTriggerExit", other);
		}
		public void OnTriggerExit(Collider other)
		{
		}
 
		private void HandleOnTriggerExit2DEvent(Collider2D other)
		{
			gameObject.SendMessage("OnTriggerExit2D", other);
		}
		public void OnTriggerExit2D(Collider2D other)
		{
		}
 
		private void HandleOnTriggerStayEvent(Collider other)
		{
			gameObject.SendMessage("OnTriggerStay", other);
		}
		public void OnTriggerStay(Collider other)
		{
		}
 
		private void HandleOnTriggerStay2DEvent(Collider2D other)
		{
			gameObject.SendMessage("OnTriggerStay2D", other);
		}
		public void OnTriggerStay2D(Collider2D other)
		{
		}

    }
}