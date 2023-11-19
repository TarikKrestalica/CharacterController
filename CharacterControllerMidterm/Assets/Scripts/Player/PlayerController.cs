using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // Refine and resolve floaty movement & jump arc: https://youtu.be/pwZpJzpE2lQ?si=r3l9NrYArcLCDF3f
    // Helped fixed the issues of floaty movement, because I was continously applying forces when I pressed down on a and d keys

    #region Variables
    public static GameObject instance;
    [SerializeField] private Transform groundCheckTransform;
    public LayerMask playerMask;
    private RectTransform transform;
    private Rigidbody2D rigidbody;
    public GameObject environment;

    private AnimationManager animationManager;

    // Collection of Items
    public Inventory playerInventory;
    [Header("Item Equipping Method")]
    public SpriteRenderer itemHolder;

    [Header("Stats & Progress")]
    public float horizontalInput;
    [Range(0.9f, .99f)]
    [SerializeField] private float frictionLevel;
    public bool jumpKeyPressed;
    public StatSystem playerStats;
    private bool canMoveToNextScene;

    // Refining the wall jumping and sliding mechanics: https://youtu.be/sfDnN-Im7rY?si=ZcGZeY6pC5iJkESh
    // Wall Sliding
    [Header("Wall Sliding System")]
    [SerializeField] private float wallSlidingSpeed;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallMask;

    // Wall Jumping
    [Header("Wall Jumping System")]
    [SerializeField] private float wallJumpDuration;
    private bool isWallJumping;
    [SerializeField] private Vector2 wallJumpPower;

    // Implementing the Kill Floor
    private Vector3 startingPosition;
    private static bool fallingOff;

    // Obstacle Collision System
    [Header("Obstacle Collision System")]
    public float obstacleHitDuration;
    public float curTimeHitDuration = 0f;

    [Header("Springboard Hit System")]
    [SerializeField] private LayerMask springboardMask;
    [SerializeField] private Vector3 curSize;
    [SerializeField] private Vector3 crouchedSize;
    [SerializeField] private bool isCrouched = false;

    #endregion

    //TODO: Write less stuff in Start/Update. Write smaller methods that do each part
    //of what you were previously doing in Start/Update - Aria
    void Start()
    {
        GetReferences();
        FindCrouchingFactors();

        ResetPlayer();
        fallingOff = false;
        canMoveToNextScene = false;
    }

    private void FindCrouchingFactors()
    {
        curSize = this.transform.localScale;
        crouchedSize = new Vector3(curSize.x, 1 / 2f * curSize.y, curSize.z);
    }

    private void GetReferences()
    {
        transform = this.GetComponent<RectTransform>();
        rigidbody = this.GetComponent<Rigidbody2D>();
        animationManager = this.GetComponent<AnimationManager>();
        playerInventory = this.GetComponent<Inventory>();
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this.gameObject;
        DontDestroyOnLoad(this.gameObject);
    }

    public void Crouch()
    {
        transform.localScale = GetCrouchedSize();
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - (GetCurrentSize().y - GetCrouchedSize().y), transform.localPosition.z);

        playerStats.SetStat(StatType.CurrentSpeed, playerStats.FindCurrentValue(StatType.CurrentSpeed) - playerStats.FindMaxValue(StatType.DashSpeed));
        isCrouched = true;
    }

    public void RevertCrouch()
    {
        transform.localScale = curSize;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + (GetCurrentSize().y - GetCrouchedSize().y), transform.localPosition.z);

        playerStats.SetStat(StatType.CurrentSpeed, playerStats.FindCurrentValue(StatType.WalkSpeed));
        isCrouched = (false);
    }

    public void Jump()
    {
        float jumpPower = playerStats.FindCurrentValue(StatType.JumpPower);
        if (IsGrounded())  // If I'm grounded, make it snappier
        {
            if (playerStats.FindMaxValue(StatType.JumpPower) > 0)
                rigidbody.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
        else if (HitSpringBoard())
        {
            if (playerStats.FindMaxValue(StatType.JumpPower) > 0)
                rigidbody.AddForce(Vector2.up * (jumpPower * 1.2f), ForceMode2D.Impulse);
        }
        else if (IsWallSliding())
        {
            isWallJumping = true;
            Invoke(nameof(StopWallJumping), wallJumpDuration);  // No longer wall jumping after wallJumpDuration seconds
        }

        jumpKeyPressed = false;
    }

    void StopWallJumping()
    {
        isWallJumping = false;
    }

    private void Update()
    {
        if (curTimeHitDuration > 0f)  // Reduce time the player has to recover from an obstacle
        {
            curTimeHitDuration -= Time.deltaTime;
        }
        else
        {
            animationManager.OnHit(false);
        }
    }
    //TODO: don't put animation stuff in this script - Aria
    private void FixedUpdate()
    {
        if (Physics.OverlapSphere(groundCheckTransform.position, 0.1f).Length == 1 && !IsWalled())  // Does player collide with ground or with wall?
        {
            return;
        }

        MovementAndSlidingLogic();
        Friction();
        CapVelocity();

        if (HitSpringBoard())  // Apply a jump boost
        {
            rigidbody.AddForce(Vector3.up * 4.0f, ForceMode2D.Impulse);
        }

        if (this.transform.position.y <= -5)
        {
            fallingOff = true;
            ResetPlayer();
            if(playerStats.FindCurrentValue(StatType.Health) <= playerStats.FindMinValue(StatType.Health))
            {
                playerStats.SetStat(StatType.Health, playerStats.FindMaxValue(StatType.Health));
            }
            else
            {
                playerStats.LowerStat(StatType.Health, 20f);
            }
        }
    }

    private void CapVelocity()
    {
        if (!playerStats.StatInCollection(StatType.WalkSpeed))
        {
            return;
        }

        float maxSpeed = playerStats.FindMaxValue(StatType.WalkSpeed);
        if (Mathf.Abs(rigidbody.velocity.x) > maxSpeed)
        {
            rigidbody.velocity =
                rigidbody.velocity.x > 0 ?
                new Vector2(maxSpeed, rigidbody.velocity.y) :
                new Vector2(-maxSpeed, rigidbody.velocity.y);
        }
    }

    private void Friction()
    {
        if (!IsGrounded())
            return;

        Vector2 vel = rigidbody.velocity;
        vel.x *= frictionLevel;
        rigidbody.velocity = vel;
    }

    private void MovementAndSlidingLogic()
    {
        rigidbody.velocity = new Vector3((horizontalInput * Time.deltaTime * 100) * playerStats.FindCurrentValue(StatType.CurrentSpeed), rigidbody.velocity.y, 0);
        Flip(horizontalInput);
        if (IsWallSliding())
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, Mathf.Clamp(rigidbody.velocity.y, -wallSlidingSpeed, float.MaxValue), 0);
        }

        if (isWallJumping)
        {
            rigidbody.velocity = new Vector2(-horizontalInput * wallJumpPower.x, wallJumpPower.y);
            Flip(-horizontalInput);
        }
    }

    void Flip(float horInput)  // Have player face the right way
    {
        if (horInput < 0)
        {
            transform.rotation = new Quaternion(0, 180, 0, 0);
        }
        else if (horInput > 0)
        {
            transform.rotation = new Quaternion(0, 0, 0, 0);
        }
    }

    // As I'm dashing, I seek to cap my velocity as well.
    public void Dash(float dashFactor)
    {
        playerStats.TryToUpdateStats(StatType.CurrentSpeed, playerStats.FindCurrentValue(StatType.CurrentSpeed) + dashFactor);
    }

    public bool CanJump()
    {
        if (IsGrounded() || IsWalled() || HitSpringBoard())  // Is it grounded, near wall, or near springboard?
        {
            return true;
        }

        return false;
    }

    public void SlowDown()
    {
        if (GetCrouchedState())
        {
            playerStats.SetStat(StatType.CurrentSpeed, playerStats.FindCurrentValue(StatType.CrouchedSpeed));
        }
        else
        {
            playerStats.SetStat(StatType.CurrentSpeed, playerStats.FindCurrentValue(StatType.WalkSpeed)); ;
        }
        playerStats.SetStat(StatType.DashSpeed, 0);
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheckTransform.position, 0.2f, playerMask);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapBox(wallCheck.position, new Vector2(0.3f, 1.3f), 0f, wallMask);
    }

    private bool HitSpringBoard()
    {
        return Physics2D.OverlapCircle(groundCheckTransform.position, 0.1f, springboardMask);
    }

    private bool IsWallSliding()
    {
        if (IsWalled() && !IsGrounded() && horizontalInput != 0f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void SetFallenOffState(bool toggle)
    {
        fallingOff = toggle;
    }

    public static bool FallenOff()
    {
        return fallingOff;
    }

    // Scene transitions
    public bool CanMoveToNextScene()
    {
        return canMoveToNextScene;
    }

    public void SetMoveToNextScene(bool toggle)
    {
        canMoveToNextScene = toggle;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            SetMoveToNextScene(true);
        }

        if (collision.gameObject.tag == "Item" || collision.gameObject.tag == "Powerup")
        {
            ItemDisplay itemDis = collision.GetComponent<ItemDisplay>();
            var item = itemDis.itemDef;

            if (playerInventory == null)
            {
                return;
            }

            playerInventory.RunPickupLogic(item);
            if (collision.gameObject.tag == "Item")
                itemHolder.sprite = item.sprite;

            Destroy(collision.gameObject);
            GameObject.FindGameObjectWithTag("CurItemDisplay").GetComponent<CurItemSlotScript>().UpdateCurItemSlot(item);
        }

        if (collision.gameObject.tag == "Obstacle")
        {
            if(collision.GetComponent<StatSystem>() == null)  // No stat system
            {
                return;
            }

            StatSystem system = collision.GetComponent<StatSystem>();
            if (curTimeHitDuration > 0)
                return;

            float damage = 0;
            if (system.StatInCollection(StatType.Damage))
            {
                damage = system.FindCurrentValue(StatType.Damage);
            }

            if (playerStats.FindCurrentValue(StatType.Health) - damage <= playerStats.FindMinValue(StatType.Health))
            {
                playerStats.SetStat(StatType.Health, playerStats.FindMaxValue(StatType.Health));
                ResetPlayer();
                return;
            }
            else
            {
                playerStats.LowerStat(StatType.Health, damage);
            }

            animationManager.OnHit(true);
            curTimeHitDuration = obstacleHitDuration;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            SetMoveToNextScene(false);
        }
    }

    public bool GetCrouchedState()
    {
        return isCrouched;
    }

    public Vector3 GetCurrentSize()
    {
        return curSize;
    }

    public Vector3 GetCrouchedSize()
    {
        return crouchedSize;
    }

    void ResetPlayer()
    {
        this.transform.position = new Vector3(-7.48f, -1.8f, 0);
    }
}
