using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
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

    //GameObject brickHold1;
    public Transform brickHold1;
    Stack<GameObject> brickStack1;
    public GameObject brick;
    //int upCount = 0;

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
       //brickHold1 = transform.Find("BrickHold1").gameObject;
        brickStack1 = new Stack<GameObject>();
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



        if ((Physics.Raycast(brickHold1.position, Vector3.down, out hit, rayLength, collisionMask)) || brickStack1.Count <= 0)
        {
            OnRunning();
        }
        if ((Physics.Raycast(brickHold1.position, Vector3.down, out hit2, rayLength, collisionMask2)))
        {
            OnBurstUp();
        }
        if ((!Physics.Raycast(brickHold1.position, Vector3.down, out hit, rayLength, collisionMask)) && (!Physics.Raycast(brickHold1.transform.position, Vector3.down, out hit2, rayLength, collisionMask2)))
        {
            OnPutTheBrickDown();
            if (brickStack1.Count <= 0)
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
            if (brickStack1.Count > 0)
            {
                GameObject otherGameObject = brickStack1.Pop();
                otherGameObject.transform.position = brickHold1.position + (Vector3.down * setDownLength);
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


    void OnCollisionEnter(Collision other1)
    {
        if (other1.gameObject.tag == "Brick")
        {
            Vector3 levelSpawn = brickHold1.position;
            levelSpawn += Vector3.up * brick.transform.localScale.y * brickStack1.Count;
            other1.gameObject.transform.position = levelSpawn;
            other1.gameObject.transform.rotation = transform.rotation;
            other1.gameObject.transform.parent = brickHold1.transform;
            //upCount += 1;

            GameObject otherGameObject = other1.gameObject;
            otherGameObject.layer = 9;
            otherGameObject.transform.tag = "BrickDown";
            brickStack1.Push(otherGameObject);

            //Debug.Log(brickStack.Count);
        }
        if (other1.gameObject.tag == "WinningZone")
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
        //brickHold1 = GameObject.FindGameObjectWithTag("BrickHold");
        Gizmos.color = Color.red;
        Gizmos.DrawRay(brickHold1.transform.position, Vector3.down * rayLength);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(brickHold1.transform.position, Vector3.down * setDownLength);
    }
}
