
using UnityEngine;

public class UiButtonFuctionsScript : MonoBehaviour
{

    int buttonCount;
    int buttonMaxCount = 1;

    //CHECKING IF BUTTON IS PRESSED 1 TIME THEN SET BOOL TO TRUE IR PRESSED AGAIN SET BOOL TO FALSE
    public bool ButtonPressCountChecker(bool temp)
    {
        if (buttonCount == 0)
        {
            temp = true;
            buttonCount += 1;
            return temp;
        }
        else if (buttonCount <= buttonMaxCount)
        {

            temp = false;
            buttonCount = 0;
            return temp;
        }
        else
        {
            return false;
        }
    }

}






