
using UnityEngine;

public class CustonTouchTest1 : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        Touch[] myTouches = Input.touches;
        for (int i = 0; i < Input.touchCount; i++)
        {
            // in this we can treat each touch individually
            if(myTouches[i].position.x > Screen.width / 2 && myTouches[i].phase == TouchPhase.Moved)
            {
                int fingerId = myTouches[i].fingerId;
                Debug.Log("half of screen");
                Debug.Log(fingerId);
                


            }
        }
    }
}
