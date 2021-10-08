using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    #region Fields
    [SerializeField] private float smoothSpeed;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float leftBoundary;
    [SerializeField] private float rightBoundary;
    [SerializeField] private float bottomBoundary;
    [SerializeField] private float topBoundary;
    private GameObject targetFollow;
    private Vector3 velocity = Vector3.zero;
    #endregion

    #region Initialization
    private void Start()
    {
        targetFollow = GameObject.FindGameObjectWithTag("Player");
    }
    #endregion

    #region Update
    private void LateUpdate()
    {
        Vector3 desiredPosition = targetFollow.transform.position + offset;
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, leftBoundary, rightBoundary);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, bottomBoundary, topBoundary);
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity,  smoothSpeed);
        transform.position = smoothedPosition;
    }
    #endregion
}
