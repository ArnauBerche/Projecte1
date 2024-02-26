using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_CHook : MonoBehaviour
{
    [Header("Scripts Ref:")]
    public S_CRope grappleRope;
    public GameObject mainCharacter;

    [Header("Layers Settings:")]
    [SerializeField] private bool grappleToAll = false;
    [SerializeField] private int grappableLayerNumber = 9;

    [Header("Main Camera:")]
    public Camera m_camera;

    [Header("Transform Ref:")]
    public Transform gunHolder;
    public Transform gunPivot;
    public Transform firePoint;

    [Header("Physics Ref:")]
    public SpringJoint2D m_springJoint2D;
    public Rigidbody2D m_rigidbody;

    [Header("Rotation:")]
    [SerializeField] private bool rotateOverTime = true;
    [Range(0, 60)] [SerializeField] private float rotationSpeed = 4;

    [Header("Distance:")]
    [SerializeField] private bool hasMaxDistance = false;
    [SerializeField] private float maxDistance = 20;
    [SerializeField] private float minDistance = 1;

    [Header("Launching:")]
    [SerializeField] public bool isHooked = false;
    [SerializeField] private float launchSpeed = 10;

    [Header("No Launch To Point")]

    [HideInInspector] public Vector2 grapplePoint;
    [HideInInspector] public Vector2 grappleDistanceVector;

    public Vector3 dirToMouse;

    private void Start()
    {
        grappleRope.enabled = false;
        m_springJoint2D.enabled = false;

    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            SetGrapplePoint();
        }
        else if (Input.GetButton("Fire2"))
        {
            if (grappleRope.enabled)
            { 
                RotateGun(grapplePoint, false);
            }
            else
            {
                Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
                RotateGun(mousePos, true); 
            }
        }
        else if (Input.GetButtonUp("Fire2"))
        {
            isHooked = false;
            grappleRope.enabled = false;
            m_springJoint2D.enabled = false;
        }
        else
        {
            Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
            RotateGun(mousePos, true);
        }

        if(isHooked && Input.GetButtonDown("Fire1"))
        {
            isHooked = false;
        }

        if(transform.position.y <= grapplePoint.y)
        {
            m_springJoint2D.frequency = 0;

            if(Input.GetAxisRaw("Vertical") == 1 && m_springJoint2D.distance > minDistance)
            {
                m_springJoint2D.distance -= 5 * Time.deltaTime;
            }
            else if(Input.GetAxisRaw("Vertical") == -1 && m_springJoint2D.distance < maxDistance)
            {
                m_springJoint2D.distance += 5 * Time.deltaTime;
            }
        }
        else
        {
            m_springJoint2D.frequency = 0;
        }


        if(!isHooked)
        {
            grappleRope.enabled = false;
            m_springJoint2D.enabled = false;
        }
        else
        {
            m_rigidbody.gravityScale = 5;
        }
    }

    void RotateGun(Vector3 lookPoint, bool allowRotationOverTime)
    {
        Vector3 distanceVector = lookPoint - gunPivot.position;

        float angle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;
        if (rotateOverTime && allowRotationOverTime)
        {
            gunPivot.rotation = Quaternion.Lerp(gunPivot.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * rotationSpeed);
        }
        else
        {
            gunPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void SetGrapplePoint()
    {
        Vector2 distanceVector = m_camera.ScreenToWorldPoint(Input.mousePosition) - gunPivot.position;

        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, distanceVector.normalized);

            if ((hit.transform.gameObject.layer == grappableLayerNumber || grappleToAll ))
            {
                if (Vector2.Distance(hit.point, firePoint.position) <= maxDistance || !hasMaxDistance)
                {
                    grapplePoint = hit.point;
                    grappleDistanceVector = grapplePoint - (Vector2)gunPivot.position;
                    grappleRope.enabled = true;
                    isHooked = true;
                }

            }                

    }

    public void Grapple()
    {
        m_springJoint2D.autoConfigureDistance = true;
        m_springJoint2D.connectedAnchor = grapplePoint;
        m_springJoint2D.enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint != null && hasMaxDistance)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(firePoint.position, maxDistance);
        }
    }
}