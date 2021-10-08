using UnityEngine;

public class AcidCameraMovement : MonoBehaviour
{
    #region Fields
    private GameObject targetFollow;
    #endregion

    #region Initialization
    private void Start()
    {
        targetFollow = GameObject.FindGameObjectWithTag("Player");
    }
    #endregion

    #region Update
    private void Update()
    {
        transform.position = new Vector2(targetFollow.transform.position.x, transform.position.y);
    }
    #endregion
}
