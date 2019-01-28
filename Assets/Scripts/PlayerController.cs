using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public bool allowMove;
    private bool isDead;

    public float speed = 100f;
    public int minX, maxX, minY;
    
    public Text txtCoinCounter, txtTime, txtAttempts, txtDeath, txtWin, txtFps;
    public int coins, totalCoinCount, attempts;

    private bool showFps;
    private int frameCount;
    private double nextUpdate = 0d, fps = 0d;
    private double updateRate = 4d;

    public float time;

    private bool jumping, doubleJumping;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public RuntimeAnimatorController animationIdle, animationRun, animationJump;

    public GameManager gameManager;

    // PlayerController controls everything about the player as well as the in game interface.
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = animationIdle;

        spriteRenderer = GetComponent<SpriteRenderer>();

        // Refreshes overlays
        txtDeath.text = "";
        coins = 0;
        ShowCoins();

        txtWin.text = "";

        // Allow player to move
        allowMove = true;

        // FPS start
        nextUpdate = Time.time;
    }

    void Update()
    {
        // Checks if not dead and they can move.
        if (!isDead && allowMove)
        {
            // Increments and dsplays time.
            time += 1 * Time.deltaTime;
            ShowTime();
        }

        if (Input.GetKeyDown(KeyCode.F12))
        {
            showFps = !showFps;
            txtFps.enabled = !txtFps.enabled;
        }

        /* 23.01.2019 below moved from FixedUpdate as not directly related to physics */



        // If they're jumping and the animation is not set to jump, set animation to jump.
        if (IsJumping() && animator.runtimeAnimatorController != animationJump)
        {
            animator.runtimeAnimatorController = animationJump;
        }

        // If they're theoretically idle, return.
        if (animator.runtimeAnimatorController == animationIdle) return;

        if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.LeftArrow))
        {
            if (IsJumping()) return;
            animator.runtimeAnimatorController = animationIdle;
        }


    }

    void LateUpdate()
    {
        if (!showFps) return;

        frameCount++;
        if (Time.time > nextUpdate)
        {
            nextUpdate += 1.0 / updateRate;
            fps = frameCount * updateRate;
            frameCount = 0;
            txtFps.text = "FPS: " + fps;
        }

    }

    void FixedUpdate () {
        // Checks if they are permitted to mvoe.
        if (!allowMove) return;

        /* Handle input */

        /* Move right */
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            spriteRenderer.flipX = false;

            Vector3 targetPos = this.transform.position + Vector3.right * speed * Time.deltaTime;
            if (IsLegalMovement(targetPos))
            {
                this.transform.Translate(targetPos - this.transform.position);

                // Now they are moving, (and not jumping), set animation to running.
                if (animator.runtimeAnimatorController != animationRun && !jumping)
                    animator.runtimeAnimatorController = animationRun;
            }
        }

        /* Move left */
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            spriteRenderer.flipX = true;

            Vector3 targetPos = this.transform.position + Vector3.left * speed * Time.deltaTime;
            if (IsLegalMovement(targetPos))
            {
                this.transform.Translate(targetPos - this.transform.position);

                if (animator.runtimeAnimatorController != animationRun && !jumping)
                    animator.runtimeAnimatorController = animationRun;
            }
        }

        /* Jump */
        if (Input.GetKeyDown(KeyCode.Space) && !doubleJumping)
        {
            // If they're already jumping, allow them to double jump.
            if (jumping)
                doubleJumping = true;
            // Set jumping to true anyway.
            jumping = true;

            // If they going to jump set the initial velocity to 350 up, if they are already jumping have addition velocity of 175.
            this.AddVelocity(Vector3.up * (doubleJumping ? 175 : 350));
            // Set animation to jumping.
            animator.runtimeAnimatorController = animationJump;
        }

        // Check if they have fallen out the lowest possible floor value.
        if (transform.position.y < minY)
        {
            Kill("fell out the world!");
            return;
        }

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead || !allowMove) return;

        // Checks if they hit the ground and they're registered as jumping.
        if ((collision.transform.CompareTag(GameTag.Ground)) && IsJumping())
        {
            // Registers player as not jumping, allowing them to jump again.
            jumping = false;
            doubleJumping = false;
            animator.runtimeAnimatorController = animationIdle;
            // Debug.Log("Event hit ground called");
        }
        // Checks if hits jump pad, then gives jump boost
        else if (collision.transform.CompareTag(GameTag.JumpPad))
        {
            this.AddVelocity(Vector2.up * 500);
        }
        // Checks if they hit a spike, then kills them.
        else if (collision.transform.CompareTag(GameTag.Spike))
        {
            // Kills player
            Kill("hit a spike!");
           //  Debug.Log("Event hit spike called");
        }
        // Checks if hit with an aggressive entity, then kills them.
        else if (collision.transform.CompareTag(GameTag.AggressiveEntity))
        {
            Kill("were bitten by a " + collision.gameObject.name);
            // Debug.Log("Event touched aggressive entity called");
        }
        // Checks if have interacted with the final objective, then finish level.
        else if (collision.transform.CompareTag(GameTag.FinishObject))
        {
            Pause();
            animator.runtimeAnimatorController = animationIdle;

            txtWin.text = "You win!";

            bool finalLevel = SceneManager.GetActiveScene().name == "Level_2";

            if (finalLevel)
            {
                txtWin.transform.Translate(new Vector3(0, 20));
                txtWin.text += "\n\nThank you for playing!";
            }

            // Call finish event
            gameManager.OnGameFinishWin(coins, time, (finalLevel ? 2 : 1));
            // Debug.Log("Finish game");
        }

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Checks if they came in contact with a coin.
        if (collision.transform.CompareTag(GameTag.Coin))
        {
            // Destroys object.
            Destroy(collision.gameObject);
            // Increments coin count by 1.
            AddCoins(1);
            // Debug.Log("Event pickup coin called");
        }

    }

    // Kills the player which stops movement and displays death reason on screen.
    public void Kill(string deathReason)
    {
        isDead = true;
        Pause();
        txtDeath.text = "You " + (deathReason ?? "Died!");
        animator.runtimeAnimatorController = animationIdle;

        transform.rotation = Quaternion.Euler(0, 0, 180); 

        // Debug.Log("Player killed");
        gameManager.OnPlayerDeathProcess();
    }

    // Pauses game for the player by denying movemeent and freezing their body.
    public void Pause()
    {
        allowMove = !allowMove;
        rb.constraints = allowMove ? RigidbodyConstraints2D.FreezeRotation : RigidbodyConstraints2D.FreezeAll;
    }

    // Checks whether the Vector3 position is out of bounds of the allowed bounds.
    public bool IsLegalMovement(Vector3 position)
    {
        return position.x <= maxX && position.x >= minX;
    }

    // Increments the coin count by i then refreshes the display.
    public void AddCoins(int i)
    {
        this.coins += i;
        ShowCoins();
    }

    // Increments the attempt count and refreshs the display.
    public void AddAttempt()
    {
        this.attempts += 1;
        ShowAttempts();
    }

    // Resets the user stats and update the UI
    public void ResetAndShow()
    {
        allowMove = false;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        coins = 0;
        ShowCoins();

        time = 0;
        ShowTime();

        txtDeath.text = "";
        isDead = false;
        animator.runtimeAnimatorController = animationIdle;
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    // Refreshs coin display.
    private void ShowCoins()
    {
        // Debug.Log("Coin display refreshed");
        this.txtCoinCounter.text = String.Format("Coins: {0}/{1}", coins, totalCoinCount);
    }

    // Refresh time display
    private void ShowTime()
    {
        this.txtTime.text = String.Format("Time: {0}s", time.ToString("0.00"));
    }

    // Refreshs attempt display.
    public void ShowAttempts()
    {
        this.txtAttempts.text = String.Format("Attempts: {0}", attempts.ToString());
    }

    // Adds velocity to the user directing them in a direction.
    public void AddVelocity(Vector2 force)
    {
        rb.AddForce(force);
    }

    // Checks if the user is jumping and or double jumping.
    private bool IsJumping()
    {
        return jumping || doubleJumping;
    }

}
