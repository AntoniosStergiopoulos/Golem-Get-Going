using System.Collections;
using TMPro;
using UnityEngine;

public class FadeInAndOut : MonoBehaviour
{
    #region Fields
    [SerializeField] private float fadeDuration;
    private TextMeshProUGUI textToFade;
    private int startingAlpha = 1;
    private int finalAlpha = 0;
    #endregion

    #region initialization
    private void Start()
    {
        textToFade = GetComponent<TextMeshProUGUI>();
        StartCoroutine(Fade());
    }
    #endregion

    #region Coroutines
    private IEnumerator Fade()
    {
        float counter = 0f;
        while (counter < fadeDuration)
        {
            counter += Time.deltaTime;
            textToFade.alpha = Mathf.Lerp(startingAlpha, finalAlpha, counter/fadeDuration);
            yield return null;
        }
        var temp = startingAlpha;
        startingAlpha = finalAlpha;
        finalAlpha = temp;
        StartCoroutine(Fade());
    }
    #endregion
}
