using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftPressTester : MonoBehaviour
{
	// get access to sprite renderer
	private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
		sr = GetComponent<SpriteRenderer>();
		Observer.Instance.LeftPressDelegate += swap;
    }

	private void swap()
	{
		Color temp = sr.color;
		temp.a = (sr.color.a == 0) ? 1 : 0;
		sr.color = temp;
	}

	// must remove self from delegate list when destroyed, causes memory leak
	private void OnDestroy()
	{
		Observer.Instance.LeftPressDelegate -= swap;
	}
}
