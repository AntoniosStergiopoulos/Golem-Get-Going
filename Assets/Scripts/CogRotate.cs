using UnityEngine;

public class CogRotate : MonoBehaviour
{
    #region Fields
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float rotationSync;
    [SerializeField] [Range(-1,1)] private int direction;
    #endregion

    #region Update
    private void FixedUpdate()
    {
        transform.Rotate(0f, 0f, direction * rotationSpeed);
    }
    #endregion
}
