
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class AdvancedWeaponRecoil : MonoBehaviour
{
    [Header("Reference Points")]
    public Transform recoilPosition;
    public Transform rotationPoint;
    [Space(10)]

    [Header("Speed Settings")]
    public float positionalRecoilSpeed = 8f;
    public float rotationalRecoilSpeed = 8f;
    [Space(10)]

    public float positionalReturnSpeed = 18f;
    public float rotationalReturnSpeed = 38f;
    [Space(10)]

    [Header("Amount Settings:")]
    public Vector3 RecoilRotation = new Vector3(10, 5, 7);
    public Vector3 RecoilKickBack = new Vector3(0.015f, 0f, -0.2f);


    Vector3 rotationalRecoil;
    Vector3 positionalRecoil;
    Vector3 Rot;

    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePos;


    [Header("Magnet")]
    [SerializeField] bool isMagnetGun;
    [SerializeField] Transform magnet_MoveTo_Position;
    [SerializeField] float magnetSpeed;
    [SerializeField] float maxForceMagnitude = 500f;
    [SerializeField] float maxDistanceForFullForce = 5f;
    [SerializeField] float targetDrag = 2f;
    [SerializeField] LineRenderer lineRenderer;




    GameObject magnetTarget;
    float magnetTarget_originalDrag;
    bool GoMagnet;
    RaycastHit[] magnetHits;

    private void FixedUpdate()
    {
        ResetRecoil();
        if (GoMagnet)
        {
            Magnet();
        }
    }

    void Update()
    {

        GetInput();
    }

  
    void Magnet()
    {
        float step = magnetSpeed * Time.deltaTime;
        Vector3 newPosition = new Vector3(magnet_MoveTo_Position.position.x, magnet_MoveTo_Position.position.y, magnet_MoveTo_Position.position.z);
        Vector3 lerpedPos = Vector3.Slerp(magnetTarget.transform.position, newPosition, step);
        UpdateLineRenderer();
        if (magnetTarget.gameObject.TryGetComponent(out Rigidbody targetRb))
        {
            float remainingDistance = Vector3.Distance(magnetTarget.transform.position, lerpedPos);

            float forceMultiplier = Mathf.Clamp01(remainingDistance / maxDistanceForFullForce);

            Vector3 directionToLerpedPos = (lerpedPos - magnetTarget.transform.position).normalized;

            float forceMagnitude = forceMultiplier * maxForceMagnitude;

            targetRb.AddForce(directionToLerpedPos * forceMagnitude, ForceMode.Force);
        }

    }

    void UpdateLineRenderer()
    {
        if (lineRenderer != null && firePos != null && magnetTarget != null)
        {
            List<Transform> bezierPoints = new List<Transform>
            {
                firePos,
                magnet_MoveTo_Position,
                magnetTarget.transform
            };

            lineRenderer.gameObject.SetActive(true);
            lineRenderer.gameObject.GetComponent<SmoothLineRenderer>().point1 = bezierPoints[0];
            lineRenderer.gameObject.GetComponent<SmoothLineRenderer>().point2 = bezierPoints[1];
            lineRenderer.gameObject.GetComponent<SmoothLineRenderer>().point3 = bezierPoints[2];
            lineRenderer.gameObject.GetComponent<SmoothLineRenderer>().isActive = true;
        }

    }
    void GetInput()
    {
        if (isMagnetGun)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {


                Recoil();
                float distance = 200f;
                float sphereRadius = 0.01f; 
                Ray ray = new Ray(transform.position, Camera.main.transform.forward);
                magnetHits = new RaycastHit[10];
                int numHits = Physics.SphereCastNonAlloc(ray, sphereRadius, magnetHits, distance);

                for (int i = 0; i < numHits; i++)
                {
                    RaycastHit hit = magnetHits[i];

                    if (hit.collider.CompareTag("Target"))
                    {
                        magnetTarget = hit.collider.gameObject;
                        magnet_MoveTo_Position.position = magnetTarget.transform.position;
                        if (magnetTarget.TryGetComponent(out Rigidbody targetRb))
                        {
                            targetRb.useGravity = false;
                            magnetTarget_originalDrag = targetRb.drag;
                            targetRb.drag = targetDrag;
                        }

                        break; 
                    }
                }


           

                }
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                GoMagnet = false;
                if (magnetTarget != null)
                {
                    if (magnetTarget.gameObject.TryGetComponent(out Rigidbody targetRb))
                    {
                        targetRb.drag = magnetTarget_originalDrag;
                        targetRb.useGravity = true;
                    }
                    magnetTarget = null;
                    lineRenderer.gameObject.GetComponent<SmoothLineRenderer>().isActive = false;
                    lineRenderer.gameObject.SetActive(false);

                }
             
            }
            if (Input.GetKey(KeyCode.Mouse0) && magnetTarget != null)
            {
                GoMagnet = true;
            }

        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Fire();
            }
        }

    }

    public void Fire()
    {
        var bullet = Instantiate(bulletPrefab, firePos.position, transform.rotation) ;


        float distance = 200f;
        RaycastHit hit;
        Vector3 bulletDirection;
        
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, distance))
        {
            
            Vector3 impactPoint = hit.point;
            bulletDirection = (impactPoint - transform.position).normalized;
        }
        else
        {
            bulletDirection = transform.forward;
        }
        bullet.GetComponent<Bullet>().SetBulletdir(bulletDirection);

        Recoil();

    }
    private void Recoil()
    {
        rotationalRecoil += new Vector3(-RecoilRotation.x, Random.Range(-RecoilRotation.y, RecoilRotation.y), Random.Range(-RecoilRotation.z, RecoilRotation.z));
        rotationalRecoil += new Vector3(Random.Range(-RecoilKickBack.x, RecoilKickBack.x), Random.Range(-RecoilKickBack.y, RecoilKickBack.y), RecoilKickBack.z);
    }
    private void ResetRecoil()
    {
        rotationalRecoil = Vector3.Lerp(rotationalRecoil, Vector3.zero, rotationalReturnSpeed);
        positionalRecoil = Vector3.Lerp(positionalRecoil, Vector3.zero, positionalReturnSpeed);

        recoilPosition.localPosition = Vector3.Slerp(recoilPosition.localPosition, positionalRecoil, positionalRecoilSpeed);
        Rot = Vector3.Slerp(Rot, rotationalRecoil, rotationalRecoilSpeed);
        rotationPoint.localRotation = Quaternion.Euler(Rot);
    }

    }

