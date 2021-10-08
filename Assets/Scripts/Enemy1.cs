using System.Collections;
using UnityEngine;

public class Enemy1 : MonoBehaviour
{
    #region Fields
    [SerializeField] private float speed;
    [SerializeField] private int maxHealth;
    [SerializeField] private float knockbackStrength;
    [SerializeField] private float invulnerableAfterDamagedDuration;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject[] waypoints;
    [SerializeField] private float pauseTime;
    [SerializeField] private float pauseAfterAttackTime;
    [SerializeField] private GameObject checkSafe;
    [SerializeField] private float checkSafeRange;
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private LayerMask whatIsSafe;
    [SerializeField] private float searchRange;
    [SerializeField] private int currencyOnDeath;
    [SerializeField] private GameObject currencySpawner;
    [SerializeField] private States _currentState = States.Patroling;
    private Rigidbody2D rb;
    private int currentHealth;
    private SpriteRenderer spriteRender;
    private int nextWaypoint = 0;
    private Vector3 mVelocity = Vector3.zero;
    private Color defaultColor;
    private float movingDirection;
    private GameObject player;
    private bool playerDetected;
    private Coroutine pauseCoroutine;
    private Coroutine attackCoroutine;
    private enum States { Idle, IdleChasing, Chasing, Patroling, Damaged, Attacking }
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
    #endregion

    #region Initialization
    private void Awake()
    {
        spriteRender = GetComponent<SpriteRenderer>();
        defaultColor = GetComponent<SpriteRenderer>().color;
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

    }
    #endregion

    #region Update
    private void Update()
    {
        CheckIfReachedWaypoint();
        CheckDirection();
        CheckIfCanSeePlayer();
    }

    private void FixedUpdate()
    {
        Move();
        Chase();
    }
    #endregion

    #region Checks
    private void CheckIfReachedWaypoint()
    {
        if (currentState != States.Patroling)
            return;

        if (Mathf.Abs(waypoints[nextWaypoint].transform.position.x - transform.position.x) <= 0.1)
        {
            pauseCoroutine = StartCoroutine(Pause());
        }
        else
        {
            currentState = States.Patroling;
        }
    }    

    private void CheckDirection()
    {
        if (currentState != States.Patroling)
            return;

        if (waypoints[nextWaypoint].transform.position.x > transform.position.x)
        {
            movingDirection = 1;
            transform.localScale = new Vector3(-1f, transform.localScale.y, transform.localScale.z);
        }
        else if (waypoints[nextWaypoint].transform.position.x < transform.position.x)
        {
            movingDirection = -1;
            transform.localScale = new Vector3(1f, transform.localScale.y, transform.localScale.z);
        }
    }

    private void CheckIfCanSeePlayer()
    {
        if (currentState != States.Patroling && currentState != States.Chasing && currentState != States.IdleChasing)
            return;
        Collider2D searchPlayer = Physics2D.OverlapCircle(transform.position, searchRange, whatIsPlayer);
        if (searchPlayer == null)
        {
            playerDetected = false;
            if (currentState == States.Chasing || currentState == States.IdleChasing)
            {
                currentState = States.Patroling;
            }
        }
        else
        {
            if ((movingDirection == 1 && transform.position.x - player.transform.position.x < 0) || (movingDirection == -1 && transform.position.x - player.transform.position.x > 0) || playerDetected)
            {
                playerDetected = true;
                if (currentState != States.IdleChasing)
                {
                    currentState = States.Chasing;
                }
            }
            else
            {
                currentState = States.Patroling;
            }
        }
    }
    private bool isSafeToChase()
    {
        Collider2D checkIfSafe = Physics2D.OverlapCircle(checkSafe.transform.position, checkSafeRange, whatIsSafe);
        if (checkIfSafe == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    #endregion

    #region Animation
    private void TriggerAnimation(States triggerState)
    {
        if (triggerState == States.Idle)
        {
            animator.SetTrigger("Idle");
        }
        else if (triggerState == States.IdleChasing)
        {
            animator.SetTrigger("IdleChase");
        }
        else if (triggerState == States.Patroling)
        {
            animator.SetTrigger("Run");
        }
        else if (triggerState == States.Chasing)
        {
            animator.SetTrigger("RunChase");
        }
        else if (triggerState == States.Attacking)
        {
            animator.SetTrigger("Idle");
        }
        else
        {
            animator.SetTrigger("Idle");
        }
    }
    #endregion

    #region Movement
    private void Move()
    {
        if (currentState == States.Patroling)
        {
            Vector3 targetVelocity = new Vector2(movingDirection * speed, rb.velocity.y);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref mVelocity, 0.05f);
        }
    }

    private void Chase()
    {
        if (currentState == States.Chasing || currentState == States.IdleChasing)
        {
            if (isSafeToChase())
            {
                currentState = States.Chasing;
                movingDirection = transform.position.x - player.transform.position.x;
                if (movingDirection <= 0)
                {
                    movingDirection = 1;
                    transform.localScale = new Vector3(-1f, transform.localScale.y, transform.localScale.z);
                }
                else
                {
                    movingDirection = -1;
                    transform.localScale = new Vector3(1f, transform.localScale.y, transform.localScale.z);
                }
                Vector3 targetVelocity = new Vector2(movingDirection * speed, rb.velocity.y);
                rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref mVelocity, 0.05f);
            }
            else
            {
                currentState = States.IdleChasing;
            }
        }
    }
    #endregion

    #region Coroutines
    private IEnumerator Pause()
    {
        currentState = States.Idle;
        nextWaypoint++;
        if (nextWaypoint > waypoints.Length - 1)
        {
            nextWaypoint = 0;
        }
        yield return new WaitForSeconds(pauseTime);
        currentState = States.Patroling;
    }

    private IEnumerator PauseAfterAttack()
    {
        currentState = States.Attacking;
        yield return new WaitForSeconds(pauseAfterAttackTime);
        currentState = States.Patroling;
    }

    private IEnumerator Damaged()
    {
        spriteRender.color = Color.green;
        yield return new WaitForSeconds(invulnerableAfterDamagedDuration);
        spriteRender.color = defaultColor;
        currentState = States.Chasing;
    }
    #endregion

    #region Called from other scripts
    private void ReceiveDamage(int value)
    {
        if (pauseCoroutine != null)
        {
            StopCoroutine(pauseCoroutine);
        }
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
        currentState = States.Damaged;
        StartCoroutine(Damaged());
        KnockBack();
        currentHealth -= value;
        if (currentHealth <= 0)
        {
            Death();
        }
    }
    #endregion

    #region On Damaged
    private void KnockBack()
    {
        Vector2 direction = player.transform.position - transform.position;
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

    private void Death()
    {
        GameObject go = Instantiate(currencySpawner, transform.position, transform.rotation);
        go.SendMessage("StartSpawning", currencyOnDeath);
        Destroy(gameObject);
    }
    #endregion

    #region Events
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StopAllCoroutines();
            StartCoroutine(PauseAfterAttack());
        }
        else if (collision.gameObject.tag == "Death")
        {
            Death();
        }
    }
    #endregion

    #region Editor
    private void OnDrawGizmosSelected()
    {
        if (checkSafe != null)
        {
            Gizmos.DrawWireSphere(checkSafe.transform.position, checkSafeRange);
        }
        Gizmos.DrawWireSphere(transform.position, searchRange);
    }
    #endregion
}