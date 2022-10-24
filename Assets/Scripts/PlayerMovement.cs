using System.Runtime.InteropServices.ComTypes;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private Rigidbody2D body;
    private bool onGround;
    private Animator anim;
    private float currentheight;
    private float previousheight;
    private float travel;
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower = 4;
    private void Awake()
    {
        //Grab references for rigidbody and animatoe foe objexct
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        //Checking if player is above the camera and moving it
        currentheight = body.position.y;
        if (currentheight > 5)
        {
            Camera.main.transform.position = new Vector3(0, 10, -10);
        }
        if (currentheight < 5)
        {
            Camera.main.transform.position = new Vector3(0, 0, -10);
        }

        //Checking if player is falling down or flying up
        travel = currentheight - previousheight;
        if (travel < 0)
        {
            anim.SetBool("falling", true);
        } else {
            anim.SetBool("falling", false);
        }
        //Checking in which direction the character is moving
        float horizontalInput = Input.GetAxis("Horizontal");

        //if (horizontalInput != 0)
        //    anim.SetBool("running", true);
        //if (horizontalInput == 0)
        //    anim.SetBool("running", false);

        //Jump
        if (onGround)
        {
            anim.SetBool("flying", false);
            if (Input.GetKey(KeyCode.UpArrow))
            {
                if (jumpPower <= 12) {
                    jumpPower += (float)0.0085;
                }
                anim.SetBool("loadingJump", true);
                //Flip player when moving left/right
                if (horizontalInput > 0)
                {
                    transform.localScale = Vector3.one;
                    anim.SetBool("flyingStraight", false);
                }
                else if (horizontalInput < 0)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                    anim.SetBool("flyingStraight", false);
                } else
                {
                    anim.SetBool("flyingStraight", true);
                }
            }
            else
            {
                body.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, body.velocity.y);
            }
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                anim.SetBool("loadingJump", false);
                if (transform.localScale == Vector3.one)
                {
                    body.velocity = new Vector2(transform.position.x, body.velocity.y + jumpPower);
                }
                else
                {
                    body.velocity = new Vector2(transform.position.x, body.velocity.y + jumpPower);
                }
                jumpPower = 4;
            }
        } else
        {
            anim.SetBool("flying", true);
        }
        previousheight = currentheight;
    }
    //Hitting a collider 2D
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Ground")
            onGround = true;
    }
    //Just stop hitting a collider 2D
    private void OnCollisionExit2D(Collision2D collision)
    {
        onGround = false;
    }

    private void OnTriggerEnter(Collider collider)
    {
        print("Trigger");
    }

}
