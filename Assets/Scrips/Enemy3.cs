using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy3 : MonoBehaviour
{
    public float moveSpeedX;
    public float rotateSpeed;
    public float moveSpeedZ;
    public float burstSpeed;
    public float runSpeed;

    public float jumpHeight;

    private Vector3 input;
    private Vector3 moveVelocity;
    NavMeshAgent nav;
    public Transform finish;

   // public GameObject brickHold3;
    Stack<GameObject> brickStack3;
    public GameObject brick;
   // int upCount = 0;

    public Transform brickHold3;
    public ActionInfomation actionInfo;
    public float rayLength;
    public float setDownLength;
    public LayerMask collisionMask;
    public LayerMask collisionMask2;
    Rigidbody rb;

    float nextSetDownTime;
    public float timeBetweenSetDown;


    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        //brickHold3 = transform.Find("BrickHold3").gameObject;
        brickStack3 = new Stack<GameObject>();
    }

    void Start()
    {
        //camFollow = FindObjectOfType<CameraFollow>();
        //camFollow.transform.parent = transform;
    }

    // Update is called once per frame

    void FixedUpdate()
    {
        actionInfo.Reset();
        rb.isKinematic = false;

        RaycastHit hit;
        RaycastHit hit2;



        if ((Physics.Raycast(brickHold3.position, Vector3.down, out hit, rayLength, collisionMask)) || brickStack3.Count <= 0)
        {
            OnRunning();
        }
        if ((Physics.Raycast(brickHold3.position, Vector3.down, out hit2, rayLength, collisionMask2)))
        {
            OnBurstUp();
        }
        if ((!Physics.Raycast(brickHold3.position, Vector3.down, out hit, rayLength, collisionMask)) && (!Physics.Raycast(brickHold3.transform.position, Vector3.down, out hit2, rayLength, collisionMask2)))
        {
            OnPutTheBrickDown();
            if (brickStack3.Count <= 0)
            {
                OnJumping();
            }
        }

        //if ((!Physics.Raycast(brickHold.transform.position, Vector3.down, out hit, rayLength, collisionMask)) && (actionInfo.liftingBrick == false) && (actionInfo.burstUp == false))
        //{
        //    if (brickStack.Count <= 0)
        //    {
        //        OnJumping();
        //       // Debug.Log("is execute");
        //    }        
        //}

        nav.SetDestination(finish.position);
    }

    void OnRunning()
    {
        nav.speed = 8.5f;
        actionInfo.runing = true;
        rb.isKinematic = false;
    }

    void OnPutTheBrickDown()
    {
        if (Time.time > nextSetDownTime)
        {
            nextSetDownTime = Time.time + timeBetweenSetDown;
            if (brickStack3.Count > 0)
            {
                GameObject otherGameObject = brickStack3.Pop();
                otherGameObject.transform.position = brickHold3.transform.position + (Vector3.down * setDownLength);
                otherGameObject.GetComponent<BoxCollider>().size = new Vector3(1, 1, 3.5f);
               // upCount -= 1;
                otherGameObject.transform.parent = null;
                actionInfo.liftingBrick = true;
                rb.isKinematic = true;
            }
        }
    }

    void OnBurstUp()
    {
        nav.speed = 13f;
        actionInfo.burstUp = true;
        rb.isKinematic = true;
    }

    void OnJumping()
    {
        Vector3 jumpVelocity = Vector3.up * jumpHeight;
        moveVelocity += jumpVelocity;
        actionInfo.jumping = true;
        // animator.SetBool("isJumping" , actionInfo.jumping);
        rb.isKinematic = false;
        //Debug.Log("is execute");
    }


    void OnCollisionEnter(Collision other3)
    {
        if (other3.gameObject.tag == "Brick")
        {
            Vector3 levelSpawn = brickHold3.position;
            levelSpawn += Vector3.up * brick.transform.localScale.y * brickStack3.Count;
            other3.gameObject.transform.position = levelSpawn;
            other3.gameObject.transform.rotation = transform.rotation;
            other3.gameObject.transform.parent = brickHold3.transform;
            //upCount += 1;

            GameObject otherGameObject = other3.gameObject;
            otherGameObject.layer = 9;
            otherGameObject.transform.tag = "BrickDown";
            brickStack3.Push(otherGameObject);

            //Debug.Log(brickStack.Count);
        }
        if (other3.gameObject.tag == "WinningZone")
        {
            Player.ranking += 1;
        }
    }



    public Vector3 GetInput()
    {
        return input;
    }

    public struct ActionInfomation
    {
        public bool liftingBrick;
        public bool runing;
        public bool burstUp;
        public bool jumping;

        public void Reset()
        {
            runing = true;
            burstUp = liftingBrick = jumping = false;
        }
    }

    //IEnumerator SpeedUp()
    //{
    //    yield return new WaitForSeconds(0.5f);
    //    GameObject otherGameObject = brickStack.Pop();
    //    otherGameObject.transform.position = brickHold.transform.position + (Vector3.down * setDownLength);
    //    otherGameObject.transform.parent = null;       
    //}

    void OnDrawGizmos()
    {
      //  brickHold3 = GameObject.FindGameObjectWithTag("BrickHold");
        Gizmos.color = Color.red;
        Gizmos.DrawRay(brickHold3.position, Vector3.down * rayLength);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(brickHold3.position, Vector3.down * setDownLength);
    }
}
