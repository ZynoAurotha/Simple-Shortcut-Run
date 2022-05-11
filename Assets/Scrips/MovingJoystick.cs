using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingJoystick : MonoBehaviour
{
    void Start()
    {
        // bo trong
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            gameObject.transform.position = Input.mousePosition;
        }

    }
}
