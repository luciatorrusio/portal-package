using System.Collections;
using System.Collections.Generic;
using AlsetRGames.Portal.Core;
using UnityEngine;

public class move : MonoBehaviour, TransitionListener
{

    [SerializeField]private float movementSpeed = 5f;
    [SerializeField] private float rotationSpeed = 180f;

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Move forward or backward
        transform.Translate(Vector3.forward * verticalInput * movementSpeed * Time.deltaTime);

        // Rotate left or right
        transform.Rotate(Vector3.up * horizontalInput * rotationSpeed * Time.deltaTime);
    }

    public void OnPortalEnter(Transition transitioning)
    {
        // print("onPortalEnter");
    }

    public void OnPortalTransitioning(Transition transitioning)
    {
        // print("OnPortalTransitioning");
    }

    public void OnPortalExit(Transition transitioning)
    {
        // print("OnPortalExit");
    }

    public void OnPortalCrossed(Transition transitioning)
    {
        // print("OnPortalCrossed");
    }
}
