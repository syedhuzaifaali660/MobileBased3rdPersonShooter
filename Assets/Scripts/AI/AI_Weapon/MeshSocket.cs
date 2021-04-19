
using UnityEngine;

public class MeshSocket : MonoBehaviour
{
    public MeshSockets.SocketId socketId;
    //TESTING NEW CODE 1
    public HumanBodyBones bone;

    public Vector3 offset;
    public Vector3 rotation;

    
    Transform attachPoint;


    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(transform.GetChild(0));
        //TESTING NEW CODE 1
        Animator animator = GetComponentInParent<Animator>();
        attachPoint = new GameObject("socket" + socketId).transform;
        attachPoint.SetParent(animator.GetBoneTransform(bone));
        attachPoint.localPosition = offset;
        attachPoint.localRotation = Quaternion.Euler(rotation);

        //attachPoint = transform.GetChild(0);
    }

public void Attach (Transform objectTransform)
    {
        objectTransform.SetParent(attachPoint, false);

    }
}
