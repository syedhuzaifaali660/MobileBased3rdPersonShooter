using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    Rigidbody[] rigidBodies;
    Animator animator;


    void Start()
    {
        rigidBodies = GetComponentsInChildren<Rigidbody>();
        animator = GetComponent<Animator>();
        DeactivateRagdoll();
    }

    public void DeactivateRagdoll()
    {
        foreach (var rb in rigidBodies)
        {
            rb.isKinematic = true;
        }
        animator.enabled = true;

    }

    public void ActivateRagdoll()
    {
        foreach (var rb in rigidBodies)
        {
            rb.isKinematic = false;
        }
        animator.enabled = false;
    }

    public void ApplyForce(Vector3 force)    {
        var rigidBody = animator.GetBoneTransform(HumanBodyBones.Hips).GetComponent<Rigidbody>();
        rigidBody.AddForce(force,ForceMode.VelocityChange);

    }
}
