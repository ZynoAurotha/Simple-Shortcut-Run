using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
 
    public Transform playerPos;
    Player player;
    public float rotateSpeed;

    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //Vector3 cameraInput = player.GetInput();
        //Vector3 rotatevelocity = cameraInput * player.rotateSpeed;

        transform.position = new Vector3(playerPos.position.x, playerPos.position.y + 7.7f, playerPos.position.z -6.5f);
       // transform.Rotate(playerPos.transform.position.y * Vector3.up, rotatevelocity.x * Time.deltaTime, Space.Self);
    }

   
}