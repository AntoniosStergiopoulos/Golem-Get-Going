using UnityEngine;

public class Acid : MonoBehaviour
{
    #region Fields
    private GameObject sfx;
    #endregion

    #region Events
    private void OnCollisionEnter2D(Collision2D collision)
    {
        sfx = GameObject.FindGameObjectWithTag("SFX");
        sfx.GetComponent<AudioSource>().Play();
        Destroy(gameObject);
    }
    #endregion
}
