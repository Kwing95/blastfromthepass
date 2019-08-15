using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionSelection : MonoBehaviour
{

    public string[] textValues;
    public string[] options;
    public float selectionTime = 0.4f;
    private int value = 0;
    private bool allowSwitch = true;
    // Start is called before the first frame update
  
    public void SwitchOption(bool moveRight)
    {
        if (!allowSwitch) return;
        allowSwitch = false;
        if (moveRight)
        {
            value++;
        }
        else
        {
            value--;
        }
        if (textValues.Length != options.Length)
        {
            Debug.LogError("Number of sprites don't match number of options");
        }
        TextMesh texMesh = GetComponent<TextMesh>();
        if(value >= textValues.Length)
        {
            value = 0;
        }else if(value < 0){
            value = textValues.Length - 1;
        }
        texMesh.text = textValues[value];
        StartCoroutine(WaitForSwitch());
    }

    IEnumerator WaitForSwitch()
    {
        yield return new WaitForSecondsRealtime(selectionTime);
        allowSwitch = true;
    }

    public string GetOption()
    {

        return options[value];
    }
}
