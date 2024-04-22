using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_CRope : MonoBehaviour
{
[Header("General Refernces:")]
    public S_CHook grapplingGun;
    public LineRenderer m_lineRenderer;
    public GameObject hookSprite;
    public Vector3 Look;

    [Header("General Settings:")]
    [SerializeField] private int percision = 40;
    [Range(0, 20)] [SerializeField] private float straightenLineSpeed = 5;

    [Header("Rope Animation Settings:")]
    public AnimationCurve ropeAnimationCurve;
    [Range(0.01f, 4)] [SerializeField] private float StartWaveSize = 2;
    float waveSize = 0;

    [Header("Rope Progression:")]
    public AnimationCurve ropeProgressionCurve;
    [SerializeField] [Range(1, 50)] private float ropeProgressionSpeed = 1;

    float moveTime = 0;

    bool strightLine = true;

    private void OnEnable()
    {
        moveTime = 0;
        m_lineRenderer.positionCount = percision;
        waveSize = StartWaveSize;
        strightLine = false;
        
        LinePointsToFirePoint();

        m_lineRenderer.enabled = true;
    }

    private void OnDisable()
    {
        hookSprite.SetActive(false);
        m_lineRenderer.enabled = false;
    }

    public void LinePointsToFirePoint()
    {
        for (int i = 0; i < percision; i++)
        {
            m_lineRenderer.SetPosition(i, grapplingGun.firePoint.position);
        }
    }

    private void Update()
    {
        moveTime += Time.deltaTime;
        DrawRope();
        if(grapplingGun.isHooked)
        {
            grapplingGun.m_springJoint2D.enabled = true;
            hookSprite.transform.rotation = Quaternion.Euler(0,0,grapplingGun.inpactRotation.x * 90 + (grapplingGun.inpactRotation.y > 0 ? grapplingGun.inpactRotation.y * 180 : grapplingGun.inpactRotation.y * 0));
        }
    }

    public void DrawRope()
    {
        if (!strightLine)
        {
            if (m_lineRenderer.GetPosition(percision - 1).x == grapplingGun.grapplePoint.x)
            {
                strightLine = true;
            }
            else
            {
                DrawRopeWaves();
            }
        }
        else
        {
            if (waveSize > 0)
            {
                waveSize -= Time.deltaTime * straightenLineSpeed;
                DrawRopeWaves();
                
            }
            else
            {
                waveSize = 0;

                if (m_lineRenderer.positionCount != 2) { m_lineRenderer.positionCount = 2; }
                DrawRopeNoWaves();
                grapplingGun.isHooked = true;
            }
        }
    }

    void DrawRopeWaves()
    {
        for (int i = 0; i < percision; i++)
        {
            float delta = (float)i / ((float)percision - 1f);
            Vector2 offset = Vector2.Perpendicular(grapplingGun.grappleDistanceVector).normalized * ropeAnimationCurve.Evaluate(delta) * waveSize;
            Vector2 targetPosition = Vector2.Lerp(grapplingGun.firePoint.position, grapplingGun.grapplePoint, delta) + offset;
            Vector2 currentPosition = Vector2.Lerp(grapplingGun.firePoint.position, targetPosition, ropeProgressionCurve.Evaluate(moveTime) * ropeProgressionSpeed);
            hookSprite.SetActive(true);
            hookSprite.transform.position = currentPosition;
            hookSprite.transform.rotation = grapplingGun.gunPivot.rotation * Quaternion.Euler(0,0,-90);
            m_lineRenderer.SetPosition(i, currentPosition);
            Look = currentPosition;
        }
    }

    void DrawRopeNoWaves()
    {
        m_lineRenderer.SetPosition(0, grapplingGun.firePoint.position);
        m_lineRenderer.SetPosition(1, grapplingGun.grapplePoint);
    }
}
