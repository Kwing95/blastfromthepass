using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticScroll : MonoBehaviour
{

    public float scrollSpeed = 10f;
    public float pageEdge;
    //public Vector2 originalPosition;
    private RectTransform rectTrans;
    private float originalYPosition;
    IEnumerator Scroll() 
    {

        print(transform.position);

        while (transform.position.y < pageEdge)
        {
            float yPosition = rectTrans.position.y + scrollSpeed;
            float xPosition = rectTrans.position.x;

            rectTrans.position = new Vector2(xPosition, yPosition);
            yield return new WaitForSecondsRealtime(0.01f);
        }
        print(transform.position);

        yield return new WaitForSecondsRealtime(.5f);
        _StartScreenManager.Instance.ShowStartPanel();
    }


    public void StartScroll()
    {
        rectTrans = GetComponent<RectTransform>();
        float xPosition = rectTrans.position.x;

        rectTrans.position = new Vector2(xPosition, originalYPosition);
        StartCoroutine(Scroll());
    }


    // Start is called before the first frame update
    void Start()
    {

        originalYPosition = transform.position.y;
    }

}
