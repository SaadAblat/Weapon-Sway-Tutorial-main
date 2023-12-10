using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetTarget : MonoBehaviour
{
    internal Rigidbody rb;
    internal bool CollisionDetected;
    void Start()
    {
        rb  = GetComponent<Rigidbody>();
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    collision.collider.Clos
    //}
}
