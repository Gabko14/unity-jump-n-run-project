using System;
using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private bool pouseMenu = false;
    private bool alive = true;
    private bool alive1 = true;
    private Rigidbody2D body;
    private BoxCollider2D slimesBoxCollider2D;
    private Animator anim;
    private float currentheight;
    private float previousheight;
    private float travel;
    private float movementspeed;
    private string highscore;
    public GameObject scoreText;
    public GameObject highScoreText;
    public GameObject Panel;
    public GameObject bigText;
    public GameObject smallText;
    public GameObject resumeButton;
    public GameObject restartButton;
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower = 4;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask objectLayerMask;

    
    private void mainMenu()
    {
        Time.timeScale = 0f;
        bigText.GetComponent<TextMeshProUGUI>().text = "Welcome to Slimy Jump!";
        smallText.SetActive(false);
        restartButton.SetActive(false);
        Panel.SetActive(true);
    }
    void SaveGameDataAndQuit()
    {
        PlayerPrefs.SetString("highscoreData", highscore.ToString());
        PlayerPrefs.Save();
        Debug.Log("Game data saved!");
        Application.Quit();
    }
    void LoadGameData()
    {
        if (PlayerPrefs.HasKey("highscoreData"))
        {
            highscore = PlayerPrefs.GetString("highscoreData");
            highScoreText.GetComponent<TextMeshProUGUI>().text = highscore;
            Debug.Log("Game data loaded!");
        }
        else
            Debug.LogError("There is no save data!");
    }
    public void Awake()
    {
        //Grab references for rigidbody and animatoe foe objexct
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        slimesBoxCollider2D = GetComponent<BoxCollider2D>();
        movementspeed = (float) (9);
        mainMenu();
        LoadGameData();
    }
    private void pauseGame()
    {
        if (pouseMenu)
        {
            Time.timeScale = 1f;
            pouseMenu = false;
            Panel.SetActive(false);
        }
        else
        {
            Time.timeScale = 0f;
            pouseMenu = true;
            Panel.SetActive(true);
        }
    }

    public void ExitGame()
    {
        SaveGameDataAndQuit();
        
    }
    public void resumeGame()
    {
        bigText.GetComponent<TextMeshProUGUI>().text = "Pause";
        restartButton.SetActive(true);
        if (alive == false)
        {
            restartGame();
            return;
        }
        Time.timeScale = 1f;
        pouseMenu = false;
        Panel.SetActive(false);
        smallText.SetActive(true);
    }
    public void restartGame()
    {
        smallText.SetActive(true);
        bigText.GetComponent<TextMeshProUGUI>().color = new Color(255, 255, 255, 255);
        bigText.GetComponent<TextMeshProUGUI>().text = "Pause";
        alive = true;
        alive1 = true;
        anim.SetBool("alive", true);
        jumpPower = 4;
        body.transform.position = new Vector3(0, 0, 0);
        Time.timeScale = 1f;
        pouseMenu = false;
        Panel.SetActive(false);
        body.velocity = new Vector2 (0, 0);
        anim.Play("Green Idle - Animation");
        resumeButton.SetActive(true);
    }

     private void die()
    {
        bigText.GetComponent<TextMeshProUGUI>().color = new Color(255, 0, 0, 255);
        bigText.GetComponent<TextMeshProUGUI>().text = "You Died";
        resumeButton.SetActive(false);
        anim.SetBool("alive", false);
        Panel.SetActive(true);
        alive1 = false;
    }

    private void Update()
    {
        if (body.velocity.y < -15)
        {
            alive = false;
        }
        if (alive1)
        {
            //Checking if player is above the camera and moving it
            currentheight = body.position.y;

            Camera.main.transform.position = new Vector3(0, currentheight, -10);
            scoreText.GetComponent<TextMeshProUGUI>().text = Math.Round(Camera.main.transform.position.y + 3).ToString();
            if (Int16.Parse(scoreText.GetComponent<TextMeshProUGUI>().text) > Int16.Parse(highScoreText.GetComponent<TextMeshProUGUI>().text))
            {
                highscore = Math.Round(Camera.main.transform.position.y + 3).ToString();
                highScoreText.GetComponent<TextMeshProUGUI>().text = highscore;
            }

            if (Input.GetKeyUp(KeyCode.P))
            {
                pauseGame();
            }

            //Checking if player is falling down or flying up
            travel = currentheight - previousheight;
            if (travel < 0)
            {
                anim.SetBool("falling", true);
            }
            else
            {
                anim.SetBool("falling", false);
            }
            //Checking in which direction the character is moving
            float horizontalInput = Input.GetAxis("Horizontal");

            if (pouseMenu == false)
            {
                if (IsonGround())
                {
                    if (alive == false)
                    {
                        die();
                    }
                    //Jump
                    anim.SetBool("flying", false);
                    if (Input.GetKey(KeyCode.UpArrow))
                    {
                        if (jumpPower <= 12)
                        {
                            jumpPower += movementspeed * Time.deltaTime;
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
                        }
                        else
                        {
                            anim.SetBool("flyingStraight", true);
                        }
                    }
                    else
                    {
                        body.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, body.velocity.y);
                    }
                    //JUMP
                    if (Input.GetKeyUp(KeyCode.UpArrow))
                    {
                        anim.SetBool("loadingJump", false);
                        body.velocity = new Vector2(body.velocity.x, body.velocity.y + jumpPower);
                        jumpPower = 4;
                    }
                }
                else
                {
                    anim.SetBool("flying", true);
                }
            }
            //test if player bumped into a wall
            if (IsHittingLeftWall())
            {
                anim.SetBool("HittingWallLeft", true);
            }
            else
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
    }


    private bool IsonGround() //RETURNS TRUE IF PLAYER IS HITTING SOMETHING FROM THE BOTTOM got it from here: https://youtu.be/c3iEl5AwUF8
    {
        float extraHeightText = 0.1f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(slimesBoxCollider2D.bounds.center, slimesBoxCollider2D.bounds.size, 0, Vector2.down, extraHeightText, groundLayerMask);
        Color rayColor;
        if (raycastHit.collider != null) //for debugging purposes
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }
        bool myVar = IsHittingLeftWall() != true && IsHittingRightWall() != true;
        return raycastHit.collider != null && myVar;
    }

    private bool IsHittingLeftWall() //RETURNS TRUE IF PLAYER IS HITTING SOMETHING FROM THE BOTTOM got it from here: https://youtu.be/c3iEl5AwUF8
    {
        float extraHeightText = 0.1f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(new Vector3(slimesBoxCollider2D.bounds.center.x - extraHeightText, slimesBoxCollider2D.bounds.center.y, slimesBoxCollider2D.bounds.center.z), new Vector2(slimesBoxCollider2D.bounds.size.x, slimesBoxCollider2D.bounds.size.y / 2 - extraHeightText), 0, Vector2.left, 0, objectLayerMask);
        return raycastHit.collider != null; ;
    }

    private bool IsHittingRightWall() //RETURNS TRUE IF PLAYER IS HITTING SOMETHING FROM THE BOTTOM got it from here: https://youtu.be/c3iEl5AwUF8
    {
        float extraHeightText = 0.1f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(new Vector3(slimesBoxCollider2D.bounds.center.x + extraHeightText, slimesBoxCollider2D.bounds.center.y, slimesBoxCollider2D.bounds.center.z), new Vector2(slimesBoxCollider2D.bounds.size.x, slimesBoxCollider2D.bounds.size.y / 2 - extraHeightText), 0, Vector2.left, 0, objectLayerMask);
        
        return raycastHit.collider != null;
    }

}
