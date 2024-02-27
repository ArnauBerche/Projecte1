using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_CHook : MonoBehaviour
{
    [Header("Scripts Ref:")]
    public S_CRope grappleRope;
    public S_CMovement moveChar;
    public GameObject mainCharacter;


    [Header("Layers Settings:")]
    [SerializeField] private int grappableLayerNumber = 6;

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
    [SerializeField] private float maxDistance = 5;
    [SerializeField] private float minDistance = 1;

    [SerializeField] public Vector3 inpactRotation;

    [Header("Launching:")]
    [SerializeField] public bool isHooked = false;

    [Header("No Launch To Point")]

    [HideInInspector] public Vector2 grapplePoint;
    [HideInInspector] public Vector2 grappleDistanceVector;

    public Vector3 dirToMouse;

    private void Start()
    {
        grappleRope.enabled = false;
        m_springJoint2D.enabled = false;
        m_springJoint2D.frequency = 4;
    }

    private void Update()
    {
        Debug.Log(isHooked);
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

        HookChecks();
    }

    public void HookChecks()
    {
        if(isHooked)
        {
            moveChar.parachute = false;
            m_rigidbody.gravityScale = 5;
            if(transform.position.y <= grapplePoint.y)
            {
                if(Input.GetAxisRaw("Vertical") > 0 && m_springJoint2D.distance > minDistance)
                {
                    Debug.Log("Top");
                    m_springJoint2D.distance -= 5 * Time.deltaTime;
                }
                else if(Input.GetAxisRaw("Vertical") < 0 && m_springJoint2D.distance < maxDistance)
                {
                    Debug.Log("down");
                    m_springJoint2D.distance += 5 * Time.deltaTime;
                }
            }
            else if(transform.position.y >= grapplePoint.y + 3)
            {
                isHooked = false;
                grappleRope.enabled = false;
                m_springJoint2D.enabled = false;
            }

            if(m_springJoint2D.distance >= maxDistance + 1)
            {

                m_springJoint2D.distance = 5;

            }
        }        
    }

    public void RotateGun(Vector3 lookPoint, bool allowRotationOverTime)
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

    public void SetGrapplePoint()
    {
        Vector2 distanceVector = m_camera.ScreenToWorldPoint(Input.mousePosition) - gunPivot.position;

        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, distanceVector.normalized);
        
        if(hit.transform != null)
        {
            if ((hit.transform.gameObject.layer == grappableLayerNumber))
            {
                if (Vector2.Distance(hit.point, firePoint.position) <= maxDistance || !hasMaxDistance)
                {
                    grapplePoint = hit.point;
                    grappleDistanceVector = grapplePoint - (Vector2)gunPivot.position;
                    grappleRope.enabled = true;
                    m_springJoint2D.distance = Vector2.Distance(grapplePoint,transform.position);
                    m_springJoint2D.connectedAnchor = grapplePoint;
                    inpactRotation = hit.normal;
                }

            }             
        }
               

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