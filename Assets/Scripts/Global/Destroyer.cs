
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    float timerOfDestruction = 0.2f;
    // Start is called before the first frame update
    void Awake()
    {
        Destroy(gameObject, timerOfDestruction);
    }


}
