using TMPro;
using UnityEngine;

public class CurrencyAnimation : MonoBehaviour
{
    #region Fields
    [SerializeField] private float speed;
    private GameObject currencyIcon;
    private GameObject currencyText;
    private Vector2 targetPosition;
    private Vector2 mVelocity = Vector3.zero;
    #endregion

    #region Initialization
    void Start()
    {
        currencyIcon = GameObject.Find("ScrapsIcon");
        currencyText = GameObject.Find("NumberOfScrapText");
        targetPosition = currencyIcon.transform.position;
    }

    void FixedUpdate()
    {
        //Move to target
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed);
        if (Vector2.Distance(targetPosition, transform.position) > 1 )
        {
            //Wobble effect
            transform.position = Vector2.SmoothDamp(transform.position, new Vector2(transform.position.x, transform.position.y + Random.Range(-10.0f, 10.0f)), ref mVelocity, 0.01f);
        }
        else if (Vector2.Distance(targetPosition, transform.position) < 0.1f)
        {
            //Destroy when it reaches destination and update currency text
            TextMeshProUGUI currencyTxt = currencyText.GetComponent<TextMeshProUGUI>();
            currencyTxt.text = (int.Parse(currencyTxt.text) + 1 ).ToString() ;
            Destroy(gameObject);
        }
    }
    #endregion
}
