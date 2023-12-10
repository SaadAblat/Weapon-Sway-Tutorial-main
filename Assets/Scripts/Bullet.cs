using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Vector3 _bulletDir;
    [SerializeField] float _speed;
    Rigidbody rb;


    bool hasHitTarget;
    bool hasHitSomething;
    GameObject target;
    Vector3 lastFramePosition;
    public void SetBulletdir(Vector3 bulletDirection)
    {
        _bulletDir = bulletDirection;
    }
    public void SetBulletSpeed( float speed)
    {
        _speed = speed;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(_bulletDir * _speed, ForceMode.Impulse);
        lastFramePosition = transform.position;
        Destroy(gameObject, 5f);
    }



    void FixedUpdate()
    {
        RaycastLastPositionFrame();
        HandleImpact();
    }

    void RaycastLastPositionFrame()
    {
        // raycast the previous frame logic here
        float distance = Vector3.Distance(transform.position, lastFramePosition);
        RaycastHit hit;

        Ray ray = new Ray(transform.position, lastFramePosition - transform.position);
        RaycastHit[] hits = new RaycastHit[10];
        int numHits = Physics.RaycastNonAlloc(ray, hits, distance);

        for (int i = 0; i < numHits; i++)
        {
            hit = hits[i];
            if (!hit.collider.CompareTag("Player")) hasHitSomething = true;
            Debug.Log(hit.collider.name +" " +  hit.collider.tag);
            if (hit.collider.CompareTag("Target"))
            {
                hasHitTarget = true;
                target = hit.collider.gameObject;
        
            }


        }
 
    
        lastFramePosition = transform.position;
    }

    //void RaycastLastPositionFrame()
    //{
    //    // raycast the previous frame logic here
    //    float distance = Vector3.Distance(transform.position, lastFramePosition);
    //    RaycastHit hit;
    //    if (Physics.Raycast(transform.position, lastFramePosition - transform.position, out hit, distance))
    //    {
    //        // Check if the raycast hits something
    //        if (hit.collider.CompareTag("Target"))
    //        {
    //            hasHitTarget = true;
    //            target = hit.collider.gameObject;
    //        }
    //        hasHitSomething = true;
    //        Debug.Log(hit.collider.name);
    //    }
    //    lastFramePosition = transform.position;
    //}
    void HandleImpact()
    {
        if (hasHitSomething)
        {
            if (hasHitTarget)
            {
                CinemachineShake.CameraInstance.ShakeCamera(0.5f, 0.15f, 0.02f);
                Destroy(target);
 
            }
            Destroy(gameObject,0.001f);


        }
    }
}
