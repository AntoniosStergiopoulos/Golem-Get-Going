using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControllerOld : MonoBehaviour
{
    private KeyBindings keyBindings;
    private Rigidbody2D rb;
    private float horizontalInput;
    [SerializeField]
    private float speed = 10f;
    [SerializeField]
    private float jumpForce = 10f;
    [SerializeField]
    private LayerMask ground;
    [SerializeField]
    private LayerMask whatIsEnemies;
    private Collider2D col;
    private Vector3 mVelocity = Vector3.zero;
    private bool doubleJumpled;
    private int currentHealth = 3;
    private int maxHealth = 3;
    private Image[] hearts;
    [SerializeField]
    private Sprite fullHeart;
    [SerializeField]
    private Sprite emptyHeart;
    [SerializeField]
    private float knockbackStrength;
    [SerializeField]
    private float invulnerableAfterDamagedDuration = 0.3f;
    //private GameManager gameManager;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private float readyJumpDuration = 0.2f;
    [SerializeField]
    private float lightAttackDuration = 0.3f;
    [SerializeField]
    private float dashForce = 40f;
    [SerializeField]
    private float dashDuration = 0.3f;
    [SerializeField]
    private GameObject attackPoint;
    [SerializeField]
    private float attackRange;
    [SerializeField]
    private float attackDamage;
    private SpriteRenderer spriteRender;
    private Color defaultColor;
    private bool canHitEnemy;
    private enum States { Idle, Running, ReadyingJump, Jumping, LightAttacking, HeavyAttacking, Dashing, Damaged, Death }
    [SerializeField]
    private States _currentState = States.Idle;
    private States currentState
    {
        get => _currentState;
        set
        {
            if (_currentState != value)
            {
                _currentState = value;
                TriggerAnimation(_currentState);
            }
        }
    }

    private void TriggerAnimation(States currentState)
    {
        if (currentState == States.Idle)
        {
            animator.SetTrigger("Idle");
        }
        else if (currentState == States.Running)
        {
            animator.SetTrigger("Run");
        }
        else if (currentState == States.ReadyingJump)
        {
            animator.SetTrigger("Jump");
        }
        else if (currentState == States.LightAttacking)
        {
            animator.SetTrigger("LightAttack");
        }
        else if (currentState == States.HeavyAttacking)
        {
            animator.SetTrigger("HeavyAttack");
        }
        else if (currentState == States.Death)
        {
            animator.SetTrigger("Death");
        }
    }

    private void Awake()
    {
        keyBindings = new KeyBindings();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriteRender = GetComponent<SpriteRenderer>();
        defaultColor = GetComponent<SpriteRenderer>().color;
    }

    private void Start()
    {
        keyBindings.Player.Jump.performed += cxt => Jump(cxt.ReadValue<float>());
        keyBindings.Player.LightAttack.performed += cxt => LightAttack(cxt.ReadValue<float>());
        keyBindings.Player.Dash.performed += cxt => Dash(cxt.ReadValue<float>());
        //gameManager = GameManager.instance;
        var hui = GameObject.Find("HealthUI");
        hearts = hui.GetComponentsInChildren<Image>();
    }

    private void Dash(float value)
    {
        if (value == 1 && isGrounded() && currentState != States.Death)
        {
            rb.AddForce(new Vector2(dashForce * horizontalInput, 0f), ForceMode2D.Impulse);
        }
    }

    private void LightAttack(float value)
    {
        if (value == 1 && isGrounded() && currentState != States.Damaged && currentState != States.LightAttacking && currentState != States.Death)
        {
            if (OverHeatBar.instance.HaveEnoughSteam(20))
            {
                OverHeatBar.instance.UseSteam(20);
                StartCoroutine(LightAttacking());
            }
            else
            {
                if (!OverHeatBar.instance.overHeated)
                {
                    StartCoroutine(LightAttacking());
                    OverHeatBar.instance.OverHeat();
                }
            }
        }
    }

    private IEnumerator LightAttacking()
    {
        currentState = States.LightAttacking;
        //animator.SetBool("IsLightAttacking", true);
        yield return new WaitForSeconds(lightAttackDuration/2);
        canHitEnemy = true;
        yield return new WaitForSeconds(lightAttackDuration/2);
        currentState = States.Idle;
        //animator.SetBool("IsLightAttacking", false);
        canHitEnemy = false;
    }
    private void CheckIfHittingPlayer()
    {
        if (!canHitEnemy)
            return;
        Collider2D hitEnemy = Physics2D.OverlapCircle(attackPoint.transform.position, attackRange, whatIsEnemies);
        if (hitEnemy != null)
        {
            hitEnemy.SendMessage("ReceiveDamage", 1);
            canHitEnemy = false;
        }
    }

    private void OnEnable()
    {
        keyBindings.Enable();
    }

    private void OnDisable()
    {
        keyBindings.Disable();
    }

    private void Update()
    {
        if (currentState == States.Death)
            return;
        ReadInput();
        SetAnimationParameters();
        CheckIfHittingPlayer();
    }

    private void SetAnimationParameters()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        if (horizontalInput == 1)
        {
            transform.localScale = new Vector3(1f, transform.localScale.y, transform.localScale.z);
            //spriteRender.flipX = false;
        }
        else if (horizontalInput == -1)
        {
            transform.localScale = new Vector3(-1f, transform.localScale.y, transform.localScale.z);
            //spriteRender.flipX = true;
        }
    }

    private void ReadInput()
    {
        horizontalInput = keyBindings.Player.Move.ReadValue<float>();
    }

    private void FixedUpdate()
    {
        if (currentState == States.Death)
            return;
        Move();
    }

    private void Move()
    {
        if (currentState == States.Idle || currentState == States.Running || currentState == States.Jumping)
        {
            if (currentState != States.Jumping)
            {
                if (horizontalInput != 0)
                {
                    currentState = States.Running;
                }
                else
                {
                    currentState = States.Idle;
                }
            }
            Vector3 targetVelocity = new Vector2(horizontalInput * speed, rb.velocity.y);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref mVelocity, 0.05f);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }
    private void Jump(float value)
    {
        if (value == 1 && currentState != States.Death)
        {
            if (isGrounded())
            {
                StartCoroutine(ReadyJump());
            }
            else
            {
                if (!doubleJumpled)
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0f);
                    rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                    doubleJumpled = true;
                }
            }
        }        
    }

    private IEnumerator ReadyJump()
    {
        currentState = States.ReadyingJump;
        animator.SetBool("IsJumping", true);
        yield return new WaitForSeconds(readyJumpDuration);
        currentState = States.Jumping;
        doubleJumpled = false;
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
    }

    private bool isGrounded()
    {
        Vector2 feetPos = transform.position;
        feetPos.y -= col.bounds.extents.y;
        bool isGrounded = Physics2D.OverlapCircle(feetPos, .1f, ground);
        if (isGrounded)
        {
            animator.SetBool("IsJumping", false);
        }
        return isGrounded;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Collectable")
        {
            Destroy(collision.gameObject);
            HealDamage();
        }
    }

    private void HealDamage()
    {
        if (currentHealth < maxHealth)
        {
            hearts[currentHealth].sprite = fullHeart;
            currentHealth++;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Hazard" || collision.gameObject.layer == 7)
        {
            if (currentState == States.Damaged)
                return;
            KnockBack(collision);
            ReceiveDamage();
        }
        else if (collision.gameObject.layer == 6)
        {
            animator.SetBool("IsJumping", false);
            if (currentState == States.Jumping)
            {
                currentState = States.Idle;
            }
        }
        else if (collision.gameObject.tag == "Death")
        {
            Death();
        }
    }



    private void KnockBack(Collision2D collision)
    {
        Vector2 direction = collision.transform.position - transform.position;
        if (direction.x <= 0)
        {
            direction = new Vector2(1, 0.3f);
        }
        else
        {
            direction = new Vector2(-1, 0.3f);
        }
        rb.velocity = Vector2.zero;
        rb.AddForce(direction * knockbackStrength, ForceMode2D.Impulse);
    }

    private void ReceiveDamage()
    {
        if (currentHealth == 0)
        {
            Death();
            return;
        }

        currentHealth--;
        hearts[currentHealth].sprite = emptyHeart;
        StartCoroutine(Damaged());
    }

    private IEnumerator Damaged()
    {
        spriteRender.color = Color.cyan;
        currentState = States.Damaged;
        yield return new WaitForSeconds(invulnerableAfterDamagedDuration);
        spriteRender.color = defaultColor;
        currentState = States.Idle;
    }
    private void ReceiveDamageFromEnemyScript(float dir)
    {
        KnockBack(dir);

        if (currentHealth > 0)
        {
            currentHealth--;
            hearts[currentHealth].sprite = emptyHeart;
        }

        if (currentHealth == 0)
        {
            Death();
            return;
        }
        StartCoroutine(Damaged());
    }
    private void KnockBack(float dir)
    {
        Vector2 direction;
        if (dir <= 0)
        {
            direction = new Vector2(1, 0.7f);
        }
        else
        {
            direction = new Vector2(-1, 0.7f);
        }
        rb.velocity = Vector2.zero;
        rb.AddForce(direction * knockbackStrength, ForceMode2D.Impulse);
    }

    private void Death()
    {
        foreach (var heart in hearts)
        {
            heart.sprite = emptyHeart;
        }
        //Destroy(gameObject);
        currentState = States.Death;
        rb.velocity = Vector2.zero;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.transform.position, attackRange);
    }
}
