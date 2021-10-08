using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region Fields
    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask whatIsEnemies;
    [SerializeField] private Sprite[] fullHearts;
    [SerializeField] private Sprite[] emptyHearts;
    [SerializeField] private float knockbackStrength;
    [SerializeField] private float invulnerableAfterDamagedDuration = 0.3f;
    [SerializeField] private Animator animator;
    [SerializeField] private float readyJumpDuration = 0.2f;
    [SerializeField] private float lightAttackDuration = 0.3f;
    [SerializeField] private float heavyAttackDuration = 1.1f;
    [SerializeField] private float dashForce = 40f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private GameObject lightAttackPoint;
    [SerializeField] private float lightAttackRange;
    [SerializeField] private GameObject heavyAttackPoint;
    [SerializeField] private float heavyAttackRange;
    [SerializeField] private float attackDamage;
    [SerializeField] private PhysicsMaterial2D frictionMaterial;
    [SerializeField] private bool learnedToDoubleJump;
    [SerializeField] private bool learnedToDash;
    [SerializeField] private States _currentState = States.Idle;
    private KeyBindings keyBindings;
    private Rigidbody2D rb;
    private GameObject navigationObject;
    private float horizontalInput;
    private Collider2D col;
    private Vector3 mVelocity = Vector3.zero;
    private bool doubleJumpled;
    private int currentHealth = 4;
    private int maxHealth = 4;
    private Image[] hearts;
    private SpriteRenderer spriteRender;
    private Color defaultColor;
    private bool canHitEnemy;
    private bool isDead;
    private enum States { Idle, Running, ReadyingJump, Jumping, LightAttacking, HeavyAttacking, Dashing, Damaged }
    private States currentState
    {
        get => _currentState;
        set
        {
            if (_currentState != value)
            {
                _currentState = value;
                TriggerAnimation();
            }
        }
    }
    #endregion

    #region Animation
    private void TriggerAnimation()
    {
        if (isDead)
        {
            animator.SetTrigger("Death");
        }
        else if (currentState == States.Idle)
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
    }

    private void SetAnimationParameters()
    {
        if (horizontalInput == 1)
        {
            transform.localScale = new Vector3(1f, transform.localScale.y, transform.localScale.z);
        }
        else if (horizontalInput == -1)
        {
            transform.localScale = new Vector3(-1f, transform.localScale.y, transform.localScale.z);
        }
    }
    #endregion

    #region Initialization
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
        keyBindings.Player.HeavyAttack.performed += cxt => HeavyAttack(cxt.ReadValue<float>());
        //gameManager = GameManager.instance;
        GameObject hui = GameObject.Find("HealthUI");
        //GameObject[] heartObjs = new GameObject[maxHealth];
        hearts = new Image[maxHealth];
        for (int i = 0; i < maxHealth; i++)
        {
            //heartObjs[i] = hui.transform.GetChild(i).gameObject;
            hearts[i] = hui.transform.GetChild(i).gameObject.GetComponent<Image>();
        }
        currentHealth =  PlayerManager.instance.playerCurrentHealth;
        navigationObject = GameObject.FindGameObjectWithTag("Navigation");
    }
    #endregion

    #region Update
    private void Update()
    {
        if (isDead)
            return;
        ReadInput();
        SetAnimationParameters();
        CheckIfHittingEnemy();
    }
    private void FixedUpdate()
    {
        if (isDead)
            return;
        Move();
    }

    #endregion

    #region Input
    private void ReadInput()
    {
        horizontalInput = keyBindings.Player.Move.ReadValue<float>();
    }
    #endregion

    #region Checks
    private void CheckIfHittingEnemy()
    {
        if (!canHitEnemy)
            return;
        if (currentState == States.LightAttacking)
        {
            Collider2D hitEnemy = Physics2D.OverlapCircle(lightAttackPoint.transform.position, lightAttackRange, whatIsEnemies);
            if (hitEnemy != null)
            {
                hitEnemy.SendMessage("ReceiveDamage", 1);
                canHitEnemy = false;
            }
        }
        else if (currentState == States.HeavyAttacking)
        {
            Collider2D hitEnemy = Physics2D.OverlapCircle(heavyAttackPoint.transform.position, heavyAttackRange, whatIsEnemies);
            if (hitEnemy != null)
            {
                hitEnemy.SendMessage("ReceiveDamage", 2);
                canHitEnemy = false;
            }
        }
        
    }
    #endregion

    #region Movement
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
        else if (currentState == States.Damaged)
        {
            //Do Nothing
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }
    #endregion

    #region Abilities
    private void Dash(float value)
    {
        if (value == 1 && isGrounded() && learnedToDash && !isDead)
        {
            rb.AddForce(new Vector2(dashForce * horizontalInput, 0f), ForceMode2D.Impulse);
        }
    }

    private void LightAttack(float value)
    {
        if (value == 1 && isGrounded() && currentState != States.Damaged && currentState != States.LightAttacking && currentState != States.HeavyAttacking && !isDead)
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

    private void HeavyAttack(float value)
    {
        if (value == 1 && isGrounded() && currentState != States.Damaged && currentState != States.LightAttacking && currentState != States.HeavyAttacking && !isDead)
        {
            if (OverHeatBar.instance.HaveEnoughSteam(30))
            {
                OverHeatBar.instance.UseSteam(30);
                StartCoroutine(HeavyAttacking());
            }
            else
            {
                if (!OverHeatBar.instance.overHeated)
                {
                    StartCoroutine(HeavyAttacking());
                    OverHeatBar.instance.OverHeat();
                }
            }
        }
    }

    private void Jump(float value)
    {
        if (value == 1 && !isDead)
        {
            if (isGrounded())
            {
                StartCoroutine(ReadyJump());
            }
            else
            {
                if (!doubleJumpled && learnedToDoubleJump)
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0f);
                    rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                    doubleJumpled = true;
                }
            }
        }
    }
    #endregion

    #region Events
    private void OnEnable()
    {
        keyBindings.Enable();
    }

    private void OnDisable()
    {
        keyBindings.Disable();
    }

    private bool isGrounded()
    {
        Vector2 feetPos = transform.position;
        feetPos.y -= col.bounds.extents.y;
        bool isGrounded = Physics2D.OverlapCircle(feetPos, .1f, ground);
        if (isGrounded)
        {
            //animator.SetBool("IsJumping", false);
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
            if (currentState == States.Jumping && isGrounded())
            {
                currentState = States.Idle;
            }
        }
        else if (collision.gameObject.tag == "Death")
        {
            Death();
        }
    }
    #endregion

    #region On Damage/Heal
    private void HealDamage()
    {
        if (currentHealth < maxHealth)
        {
            hearts[currentHealth].sprite = fullHearts[currentHealth];
            hearts[currentHealth].transform.GetChild(0).gameObject.SetActive(true);
            currentHealth++;
            PlayerManager.instance.playerCurrentHealth = currentHealth;
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
        if (currentHealth > 0)
        {
            currentHealth--;
            PlayerManager.instance.playerCurrentHealth = currentHealth;
            hearts[currentHealth].sprite = emptyHearts[currentHealth];
            hearts[currentHealth].transform.GetChild(0).gameObject.SetActive(false);
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
        if (isDead)
            return;
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
        if (isDead)
            return;
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].sprite = emptyHearts[i];
            hearts[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        currentHealth = 0;
        PlayerManager.instance.playerCurrentHealth = currentHealth;
        isDead = true;
        TriggerAnimation();
        rb.velocity = Vector2.zero;
        GetComponent<CapsuleCollider2D>().sharedMaterial = frictionMaterial;
        gameObject.tag = "Untagged";
        navigationObject.SendMessage("Dead");
    }
    #endregion

    #region Called from other scripts
    private void ReceiveDamageFromEnemyScript(float dir)
    {
        KnockBack(dir);
        ReceiveDamage();        
    }
    #endregion

    #region Coroutines
    private IEnumerator HeavyAttacking()
    {
        currentState = States.HeavyAttacking;
        yield return new WaitForSeconds(0.4f);
        canHitEnemy = true;
        yield return new WaitForSeconds(0.3f);
        canHitEnemy = false;
        yield return new WaitForSeconds(0.4f);
        currentState = States.Idle;
    }

    private IEnumerator LightAttacking()
    {
        currentState = States.LightAttacking;
        yield return new WaitForSeconds(lightAttackDuration / 2);
        canHitEnemy = true;
        yield return new WaitForSeconds(lightAttackDuration / 2);
        currentState = States.Idle;
        canHitEnemy = false;
    }

    private IEnumerator ReadyJump()
    {
        currentState = States.ReadyingJump;
        yield return new WaitForSeconds(readyJumpDuration);
        currentState = States.Jumping;
        doubleJumpled = false;
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
    }

    private IEnumerator Damaged()
    {
        spriteRender.color = Color.cyan;
        currentState = States.Damaged;
        yield return new WaitForSeconds(invulnerableAfterDamagedDuration);
        spriteRender.color = defaultColor;
        currentState = States.Idle;
    }
    #endregion

    #region Editor
    private void OnDrawGizmosSelected()
    {
        if (lightAttackPoint == null)
            return;
        Gizmos.DrawWireSphere(lightAttackPoint.transform.position, lightAttackRange);
        if (heavyAttackPoint == null)
            return;
        Gizmos.DrawWireSphere(heavyAttackPoint.transform.position, heavyAttackRange);
    }
    #endregion
}
