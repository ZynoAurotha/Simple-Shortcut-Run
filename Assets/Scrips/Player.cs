using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{

    public static float ranking;

    //float multi = 1;
    //float money = 0;

    bool isBonusMode;
    public float moveSpeedX;
    public float rotateSpeed;
    public float moveSpeedZ;
    public float burstSpeed;
    public float runSpeed;

    public FixedJoystick Joystick;
    public float jumpHeight;

    private Vector3 input;
    private Vector3 moveVelocity;

    public GameObject bonusZone;
    public GameObject brickHold;
    
    Stack<GameObject> brickStack;
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


    Vector3 playerRotation;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
       // brickHold = GameObject.FindGameObjectWithTag("BrickHold");
        brickStack = new Stack<GameObject>();
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

        input = new Vector3(Joystick.Horizontal, 0, 0);
        Vector3 directionX = input.normalized;

        Vector3 rotateVelocity = directionX * rotateSpeed;
        Vector3 rotateAngle = rotateVelocity * Time.deltaTime;

        

        RaycastHit hit;
        RaycastHit hit2;
        


        if ((Physics.Raycast(brickHold.transform.position, Vector3.down, out hit, rayLength, collisionMask)) || brickStack.Count <= 0)
        {
            OnRunning();
        }
        if ((Physics.Raycast(brickHold.transform.position, Vector3.down, out hit2, rayLength, collisionMask2)))
        {
            OnBurstUp();
        }
        if ((!Physics.Raycast(brickHold.transform.position, Vector3.down, out hit, rayLength, collisionMask)) && (!Physics.Raycast(brickHold.transform.position, Vector3.down, out hit2, rayLength, collisionMask2)))
        {          
            OnPutTheBrickDown();
            if (brickStack.Count <= 0)
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

        
        transform.Translate(moveVelocity * Time.deltaTime, Space.Self);

        Debug.Log(rotateAngle.x);
        //if (transform.rotation.y >= -0.5 && transform.rotation.y <= 0.5)
        //{
        //transform.Rotate(Vector3.up, rotateAngle.x, Space.Self);
        //}

        playerRotation = transform.eulerAngles;
        playerRotation.y += rotateAngle.x;
       // ClampingRotation();

        transform.eulerAngles = playerRotation;

        
    }

    void ClampingRotation()
    {
        if (transform.eulerAngles.y >= 20.5f)
        {
            playerRotation.y = 20;
        }
        if (transform.eulerAngles.y <= -20.5f)
        {
            playerRotation.y = -20;
        }
    }

    void OnRunning()
    {
        moveSpeedZ = runSpeed;
        moveVelocity = Vector3.forward * moveSpeedZ;
        actionInfo.runing = true;
        rb.isKinematic = false;
    }

    void OnPutTheBrickDown()
    {
        if (Time.time > nextSetDownTime)
        {
            nextSetDownTime = Time.time + timeBetweenSetDown;
            if (brickStack.Count > 0)
            {
                GameObject otherGameObject = brickStack.Pop();
                otherGameObject.transform.position = brickHold.transform.position + (Vector3.down * setDownLength);
                otherGameObject.GetComponent<BoxCollider>().size = new Vector3(1,1,3.5f);
               // upCount -= 1;
                otherGameObject.transform.parent = null;        
                actionInfo.liftingBrick = true;
                rb.isKinematic = true;
            }
        }
    }

    void OnBurstUp()
    {
        moveSpeedZ = burstSpeed;
        moveVelocity = Vector3.forward * moveSpeedZ;
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


    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Brick")
        {
            Vector3 levelSpawn = brickHold.transform.position;
            levelSpawn += Vector3.up * brick.transform.localScale.y * brickStack.Count;
            other.gameObject.transform.position = levelSpawn;
            other.gameObject.transform.rotation = transform.rotation;
            other.gameObject.transform.parent = brickHold.transform;
            //upCount += 1;
            //money += 1;

            GameObject otherGameObject = other.gameObject;
            otherGameObject.layer = 9;
            otherGameObject.transform.tag = "BrickDown";
            brickStack.Push(otherGameObject);

            //Debug.Log(brickStack.Count);
        }
        if (other.gameObject.tag == "DeathZone")
        {
            SceneManager.LoadScene(0);
        }
        if (other.gameObject.tag == "WinningZone" && ranking < 1)
        {
            bonusZone.SetActive(true);
            isBonusMode = true;
        }
        if (other.gameObject.tag == "WinningZone" && ranking > 0)
        {
            SceneManager.LoadScene(0);
        }
        if (other.gameObject.tag == "BonusZone")
        {
            other.gameObject.tag = "Untagged";
            //multi += 1;
            other.gameObject.GetComponent<Renderer>().material.color = Color.yellow;
            if (isBonusMode == true && brickStack.Count <= 0)
            {
                SceneManager.LoadScene(0);
            }
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
        brickHold = GameObject.FindGameObjectWithTag("BrickHold");
        Gizmos.color = Color.red;
        Gizmos.DrawRay(brickHold.transform.position, Vector3.down * rayLength);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(brickHold.transform.position, Vector3.down * setDownLength);
    }
}
