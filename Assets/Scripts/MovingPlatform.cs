using System.Collections;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    #region Fields
    [SerializeField]private GameObject[] Waypoints;
    [SerializeField] private float speed = 0.05f;
    [SerializeField] private float pauseTime = 1;
    [SerializeField] private float syncTime = 0;
    private int nextWaypoint = 0;
    private bool synced = false;
    private bool paused = false;
    private Rigidbody2D rb;
    private Vector3 mVelocity = Vector3.zero;
    #endregion

    #region Initialization
    private void Start()
    {
        StartCoroutine(SyncPlatform());
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    #endregion

    #region Update
    private void Update()
    {
        CheckIfReachedWaypoint();
    }
    private void FixedUpdate()
    {
        Move();
    }
    #endregion

    #region Checks
    private void CheckIfReachedWaypoint()
    {
        if (paused || !synced)
            return;
        if (Mathf.Abs(Waypoints[nextWaypoint].transform.position.x - transform.position.x) <= 0.1 && Mathf.Abs(Waypoints[nextWaypoint].transform.position.y - transform.position.y) <= 0.1)
        {
            StartCoroutine(Pause());
        }
    }
    #endregion

    #region Coroutine
    private IEnumerator SyncPlatform()
    {
        yield return new WaitForSeconds(syncTime);
        synced = true;
    }

    private IEnumerator Pause()
    {
        paused = true;
        yield return new WaitForSeconds(pauseTime);
        nextWaypoint++;
        if (nextWaypoint > Waypoints.Length - 1)
        {
            nextWaypoint = 0;
        }
        paused = false;
    }
    #endregion

    #region Movement
    private void Move()
    {
        if (paused || !synced)
            return;
        transform.position = Vector3.MoveTowards(transform.position, Waypoints[nextWaypoint].transform.position, speed);
    }
    #endregion

    #region Events
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.transform.SetParent(null);
        }
    }
    #endregion
}
