using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelNavigationFade : MonoBehaviour
{
    #region Fields
    [SerializeField] private float fadeDuration;
    private Image imageToFade;
    private int fullAlpha = 1;
    private int emptyAlpha = 0;
    #endregion

    #region initialization
    private void Start()
    {
        imageToFade = GameObject.FindGameObjectWithTag("FadeInOut").GetComponent<Image>();
        StartCoroutine(FadeOut());
    }
    #endregion

    #region Events
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(FadeIn());
        }
    }
    #endregion

    #region Coroutines
    private IEnumerator FadeOut()
    {
        float counter = 0f;
        imageToFade.enabled = true;
        Color imageColor = imageToFade.color;
        while (counter < fadeDuration)
        {
            counter += Time.deltaTime;
            imageColor.a = Mathf.Lerp(fullAlpha, emptyAlpha, counter / fadeDuration);
            imageToFade.color = imageColor;
            yield return null;
        }
        imageToFade.enabled = false;
    }

    private IEnumerator FadeIn()
    {
        float counter = 0f;
        imageToFade.enabled = true;
        Color imageColor = imageToFade.color;
        while (counter < fadeDuration)
        {
            counter += Time.deltaTime;
            imageColor.a = Mathf.Lerp(emptyAlpha, fullAlpha, counter / fadeDuration);
            imageToFade.color = imageColor;
            yield return null;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private IEnumerator DeadFade()
    {
        float counter = 0f;
        imageToFade.enabled = true;
        Color imageColor = imageToFade.color;
        while (counter < fadeDuration)
        {
            counter += Time.deltaTime;
            imageColor.a = Mathf.Lerp(emptyAlpha, fullAlpha, counter / fadeDuration);
            imageToFade.color = imageColor;
            yield return null;
        }
        SceneManager.LoadScene("DeathScene");
    }
    #endregion

    #region Called from other scripts
    public void Dead()
    {
        StartCoroutine(DeadFade());
    }
    #endregion
}
