using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy1Old : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField]
    private float speed = 10f;
    [SerializeField]
    private int maxHealth = 3;
    private int currentHealth;
    [SerializeField]
    private float knockbackStrength = 50;
    [SerializeField]
    private float invulnerableAfterDamagedDuration = 1f;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private GameObject[] waypoints;
    private int nextWaypoint = 0;
    [SerializeField]
    private float pauseTime = 1;
    private enum States {Paused, Chasing, Patroling, Damaged}
    private States currentState = States.Patroling;
    private Vector3 mVelocity = Vector3.zero;
    private SpriteRenderer spriteRender;
    private Color defaultColor;
    private float movingDirection;
    private GameObject player;
    private Coroutine pauseCoroutine;
    [SerializeField]
    private int currencyOnDeath;
    [SerializeField]
    private GameObject currencySpawner;



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

    private void Update()
    {
        CheckIfReachedWaypoint();
        CheckDirection();
        SetAnimationParameters();
    }

    private void CheckIfReachedWaypoint()
    {
        if (currentState == States.Paused || currentState == States.Damaged)
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

    private IEnumerator Pause()
    {
        currentState = States.Paused;
        nextWaypoint++;
        if (nextWaypoint > waypoints.Length - 1)
        {
            nextWaypoint = 0;
        }
        yield return new WaitForSeconds(pauseTime);
        currentState = States.Patroling;
    }

    private void CheckDirection()
    {
        if (currentState == States.Paused || currentState == States.Damaged)
            return;

        if (waypoints[nextWaypoint].transform.position.x > transform.position.x)
        {
            movingDirection = 1;
        }
        else if (waypoints[nextWaypoint].transform.position.x < transform.position.x)
        {
            movingDirection = -1;
        }
    }

    private void SetAnimationParameters()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        if (currentState == States.Paused || currentState == States.Damaged)
            return;
        if (rb.velocity.x > 0)
        {
            transform.localScale = new Vector3(1f, transform.localScale.y, transform.localScale.z);
            //spriteRender.flipX = false;
        }
        else if (rb.velocity.x < 0)
        {
            transform.localScale = new Vector3(-1f, transform.localScale.y, transform.localScale.z);
            //spriteRender.flipX = true;
        }
    }


    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (currentState == States.Patroling)
        {
            Vector3 targetVelocity = new Vector2(movingDirection * speed, rb.velocity.y);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref mVelocity, 0.05f);
        }
    }
    private void ReceiveDamage(int value)
    {
        if (pauseCoroutine != null)
        {
            StopCoroutine(pauseCoroutine);
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

    private IEnumerator Damaged()
    {
        spriteRender.color = Color.green;
        yield return new WaitForSeconds(invulnerableAfterDamagedDuration);
        spriteRender.color = defaultColor;
        currentState = States.Patroling;
    }

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
}
