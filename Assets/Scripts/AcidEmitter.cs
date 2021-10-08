using System.Collections;
using UnityEngine;

public class AcidEmitter : MonoBehaviour
{
    #region Fields
    [SerializeField] private GameObject acid;
    [SerializeField] private float emitFrequency = 2;
    [SerializeField] private float syncTime = 0;
    #endregion

    #region Initialization
    private void Start()
    {
        StartCoroutine(SyncEmitter());
    }
    #endregion

    #region Coroutines
    private IEnumerator EmitAcid()
    {
        yield return new WaitForSeconds(emitFrequency);
        Instantiate(acid, transform.position, transform.rotation);
        StartCoroutine(EmitAcid());
    }

    private IEnumerator SyncEmitter()
    {
        yield return new WaitForSeconds(syncTime);
        StartCoroutine(EmitAcid());
    }
    #endregion
}
