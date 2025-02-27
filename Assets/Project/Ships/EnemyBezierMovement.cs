using UnityEngine;

public class EnemyBezierMovement : MonoBehaviour
{
    public float travelSpeed = 5f;
    public float smoothing = 0.5f; // This might be used differently now, readjust according to new logic.
    public Vector2 xRange = new Vector2(-6f, 6f);
    public Vector2 yRange = new Vector2(-3f, 3f);
    public Vector2 idleTimeRange = new Vector2(1f, 3f);
    public float minTravelDistance = 2f;
    public float maxTravelDistance = 5f;

    private Rigidbody2D rb;
    private Vector2 startPosition;
    private Vector2 targetPosition;
    private Vector2 controlPoint;
    private float idleTime;
    private bool isIdle = true;
    private float t = 0;

    void Start()
    {
        SetIdleTime();
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        if (isIdle)
        {
            idleTime -= Time.deltaTime;
            if (idleTime <= 0)
            {
                isIdle = false;
                PrepareForMovement();
            }
        }
        else
        {
            if (t < 1)
            {
                // Using SmoothStep for a smoother start and stop.
                t += Time.deltaTime * smoothing;
                float smoothT = Mathf.SmoothStep(0, 1, t); // Smoothly interpolates t between 0 and 1.
                Vector2 BezierPoint = CalculateQuadraticBezierPoint(smoothT, startPosition, controlPoint, targetPosition);
                transform.position = new Vector3(BezierPoint.x, BezierPoint.y, 10);
            }
            else
            {
                SetIdleTime();
            }
        }
    }

    Vector2 CalculateQuadraticBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector2 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
        return p;
    }

    void SetIdleTime()
    {
        isIdle = true;
        idleTime = Random.Range(idleTimeRange.x, idleTimeRange.y);
        t = 0; // Reset for next movement phase.
    }

    void PrepareForMovement()
    {
        startPosition = transform.position;
        targetPosition = ChooseTargetWithinDistanceRange();
        controlPoint = new Vector2((startPosition.x + targetPosition.x) / 2, Random.Range(yRange.x, yRange.y));
    }

    Vector2 ChooseTargetWithinDistanceRange()
    {
        Vector2 potentialTarget;
        float distance;
        do
        {
            potentialTarget = new Vector2(Random.Range(xRange.x, xRange.y), Random.Range(yRange.x, yRange.y));
            distance = Vector2.Distance(startPosition, potentialTarget);
        } while (distance < minTravelDistance || distance > maxTravelDistance);

        return potentialTarget;
    }
}
