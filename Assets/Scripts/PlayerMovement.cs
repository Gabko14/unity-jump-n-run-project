using System;
using System.Runtime.InteropServices.ComTypes;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private Rigidbody2D body;
    private BoxCollider2D slimesBoxCollider2D;
    private Animator anim;
    private float currentheight;
    private float previousheight;
    private float travel;
    public GameObject scoreText;
    public GameObject highScoreText;
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower = 4;
    [SerializeField] private LayerMask objectLayerMask;

    private void Awake()
    {
        //Grab references for rigidbody and animatoe foe objexct
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        slimesBoxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        //Checking if player is above the camera and moving it
        currentheight = body.position.y;
        /*
        if (currentheight > 5 && currentheight < 15)
        {
            Camera.main.transform.position = new Vector3(0, 10, -10);
        }
        else if (currentheight > 15)
        {
            var myVar = (((currentheight - 5) / 10) +1);
            var myNewVar = (float)Math.Round(myVar, MidpointRounding.AwayFromZero);
            Camera.main.transform.position = new Vector3(0, myNewVar * 10, -10);
        } else
        {
            Camera.main.transform.position = new Vector3(0, 0, -10);
        }
        */
        Camera.main.transform.position = new Vector3(0, currentheight, -10);
        scoreText.GetComponent<TextMeshProUGUI>().text = Math.Round(Camera.main.transform.position.y + 3).ToString();
        if  (Int16.Parse(scoreText.GetComponent<TextMeshProUGUI>().text) > Int16.Parse(highScoreText.GetComponent<TextMeshProUGUI>().text)) {
            print(Int16.Parse(scoreText.GetComponent<TextMeshProUGUI>().text));
            print(Int16.Parse(highScoreText.GetComponent<TextMeshProUGUI>().text));
            highScoreText.GetComponent<TextMeshProUGUI>().text = Math.Round(Camera.main.transform.position.y + 3).ToString();
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
        if (IsonGround())
        {
            anim.SetBool("flying", false);
            if (Input.GetKey(KeyCode.UpArrow))
            {
                if (jumpPower <= 12) {
                    jumpPower += (float)0.0085; //normally 0.0085
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
                    body.velocity = new Vector2(body.velocity.x, body.velocity.y + jumpPower);
                }
                else
                {
                    body.velocity = new Vector2(body.velocity.x, body.velocity.y + jumpPower);
                }
                jumpPower = 4;
            }
        } else
        {
            anim.SetBool("flying", true);
        }

        //test if player bumped into a wall
        if (IsHittingLeftWall())
        {
            anim.SetBool("HittingWallLeft", true);
        } else
        {
            anim.SetBool("HittingWallLeft", false);
        }

        //test if player bumped into a wall
        if (IsHittingRightWall())
        {
            anim.SetBool("HittingWallRight", true);
        }
        else
        {
            anim.SetBool("HittingWallRight", false);
        }
        previousheight = currentheight;
    }



    /*    //Hitting a collider 2D
        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.name == "Ground")
                onGround = true;
        }
        //Just stop hitting a collider 2D
        private void OnCollisionExit2D(Collision2D collision)
        {
            onGround = false;
        }*/

    private bool IsonGround() //RETURNS TRUE IF PLAYER IS HITTING SOMETHING FROM THE BOTTOM got it from here: https://youtu.be/c3iEl5AwUF8
    {
        float extraHeightText = 0.1f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(slimesBoxCollider2D.bounds.center, slimesBoxCollider2D.bounds.size, 0, Vector2.down, extraHeightText, objectLayerMask);
        Color rayColor;
        if (raycastHit.collider != null) //for debugging purposes
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }
        Debug.DrawRay(slimesBoxCollider2D.bounds.center + new Vector3(slimesBoxCollider2D.bounds.extents.x, 0), Vector2.down * (slimesBoxCollider2D.bounds.extents.y + extraHeightText), rayColor); //just for debugging purposes too
        Debug.DrawRay(slimesBoxCollider2D.bounds.center - new Vector3(slimesBoxCollider2D.bounds.extents.x, 0), Vector2.down * (slimesBoxCollider2D.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(slimesBoxCollider2D.bounds.center - new Vector3(slimesBoxCollider2D.bounds.extents.x, slimesBoxCollider2D.bounds.extents.y + extraHeightText), Vector2.right * (slimesBoxCollider2D.bounds.extents.x) * 2, rayColor);

        return raycastHit.collider != null;
    }

    private bool IsHittingLeftWall() //RETURNS TRUE IF PLAYER IS HITTING SOMETHING FROM THE BOTTOM got it from here: https://youtu.be/c3iEl5AwUF8
    {
        float extraHeightText = 0.1f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(new Vector3(slimesBoxCollider2D.bounds.center.x - extraHeightText, slimesBoxCollider2D.bounds.center.y, slimesBoxCollider2D.bounds.center.z), slimesBoxCollider2D.bounds.size, 0, Vector2.left, 0, objectLayerMask);
        return raycastHit.collider != null;
    }

    private bool IsHittingRightWall() //RETURNS TRUE IF PLAYER IS HITTING SOMETHING FROM THE BOTTOM got it from here: https://youtu.be/c3iEl5AwUF8
    {
        float extraHeightText = 0.1f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(new Vector3(slimesBoxCollider2D.bounds.center.x + extraHeightText, slimesBoxCollider2D.bounds.center.y, slimesBoxCollider2D.bounds.center.z), slimesBoxCollider2D.bounds.size, 0, Vector2.left, 0, objectLayerMask);
        return raycastHit.collider != null;
    }

}
