using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainCamera : MonoBehaviour
{
    [SerializeField] private float turnSpeed = 20f;
    [SerializeField] private float speed = 40f;
    [SerializeField] private KeyCode forwardKey = KeyCode.Space;
    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow)) {
            transform.Rotate(0.0f, -turnSpeed * Time.deltaTime, 0.0f);
        }
        if (Input.GetKey(KeyCode.RightArrow)) {
            transform.Rotate(0.0f, turnSpeed * Time.deltaTime, 0.0f);
        }
        if (Input.GetKey(KeyCode.UpArrow)) {
            transform.Rotate(-turnSpeed * Time.deltaTime, 0f, 0.0f);
        }
        if (Input.GetKey(KeyCode.DownArrow)) {
            transform.Rotate(turnSpeed * Time.deltaTime, 0, 0.0f);
        }
        if (Input.GetKey(forwardKey))
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }

    }
}
