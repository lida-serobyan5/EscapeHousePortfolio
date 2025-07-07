using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class PickUpObjects : MonoBehaviour
{
    public GameObject heldObject;
    private UnityEngine.Quaternion originalRotation;
    public float distance = 2f;
    public float height = 1f;
    public AudioSource dropSound;

    public void TryPickUp(GameObject target)
    {
        if (heldObject == null)
        {
            heldObject = target;
            originalRotation = heldObject.transform.rotation;

            Rigidbody rigidBody = heldObject.GetComponent<Rigidbody>();
            rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
            rigidBody.drag = 25f;
            rigidBody.useGravity = false;
            rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    public void DropHeldObject()
    {
        if (heldObject != null)
        {
            Rigidbody rigidBody = heldObject.GetComponent<Rigidbody>();
            rigidBody.drag = 1f;
            rigidBody.useGravity = true;
            rigidBody.constraints = RigidbodyConstraints.None;

            dropSound.Play();

            heldObject = null;
        }
    }

    public bool IsHoldingObject()
    {
        return heldObject != null;
    }

    private void FixedUpdate()
    {
        if (heldObject != null)
        {
            Transform t = transform;
            Rigidbody rigidBody = heldObject.GetComponent<Rigidbody>();

            UnityEngine.Vector3 moveTo = t.position + distance * t.forward + height * t.up;
            UnityEngine.Vector3 direction = moveTo - heldObject.transform.position;

            rigidBody.velocity = direction * 10f;

            heldObject.transform.rotation = originalRotation;
        }
    }
}
