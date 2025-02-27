using System.Collections;
using System.Linq;
using UnityEngine;

public class EnemyShipMovement : MonoBehaviour
{
    [SerializeField]
    private string shipType; // Set this in the inspector
    private PathData pathData;
    private int SpawnIndex;
    private bool InvertPositions = false;

    public float BaseSpeed = 1.0f; // Public variable for base speed
    public float TargetOffset = 0.5f;
    private EnemyShip EnemyShip; // Reference to the EnemyShip component
    private EnemyShooting enemyShooting;

    [Tooltip("Angle offset to adjust the ship's facing direction. Adjust this value if the ship is not facing correctly.")]
    private float AngleOffset = -90f; // Default to -90 degrees, adjust as needed

    private Vector3 lastDirection;
    private float lastSpeedModifier;
    
    public void InitialiseMovement(string pathPreset)
    {
        // Get the EnemyShip component
        EnemyShip = GetComponent<EnemyShip>();
        enemyShooting = GetComponent<EnemyShooting>();

        var determinedPath = EnemyMovementManager.GetPathData(shipType, pathPreset);

        pathData = determinedPath.pathData;
        SpawnIndex = determinedPath.spawnIndex;

        PlaceAtSpawnPoint();

        // Start the movement coroutine
        StartCoroutine(MoveAlongPath());
    }

    private void PlaceAtSpawnPoint()
    {
        // Set InvertPositions if the chosen number is above 6
        if (SpawnIndex > 6)
        {
            InvertPositions = true;
        }

        var spawnCoordinate = GameConfig.Positions.spawns.FirstOrDefault(s => s.id == SpawnIndex);
        if (spawnCoordinate == null)
        {
            Debug.LogError($"No spawn coordinate found for spawn number: {SpawnIndex}");
            return;
        }

        transform.position = new Vector3(spawnCoordinate.x, spawnCoordinate.y, 10);
    }

    private IEnumerator MoveAlongPath()
    {
        foreach (var pathPoint in pathData.path)
        {
            // Wait for the specified delay
            yield return new WaitForSeconds(pathPoint.d);

            // Randomly choose one of the entries in the "p" array
            int positionIndex = Random.Range(0, pathPoint.p.Count);
            int positionNumber = pathPoint.p[positionIndex];

            // Determine the target position
            Vector3 targetPosition;
            if (positionNumber == 0)
            {
                // Fly straight down the y-axis to -10
                targetPosition = new Vector3(transform.position.x, -10, 10);
            }
            else
            {
                var positionCoordinate = GameConfig.Positions.positions.FirstOrDefault(p => p.id == positionNumber);
                if (positionCoordinate == null)
                {
                    Debug.LogError($"No position coordinate found for position number: {positionNumber}");
                    continue;
                }

                float x = InvertPositions ? -positionCoordinate.x : positionCoordinate.x;

                float randomX = Random.Range(x - TargetOffset, x + TargetOffset);
                float randomY = Random.Range(positionCoordinate.y - TargetOffset, positionCoordinate.y + TargetOffset);

                targetPosition = new Vector3(randomX, randomY, 10);
            }

            // Update shooting allowed status
            if (EnemyShip != null && EnemyShip.IsAllowedToShoot != pathPoint.sh)
            {
                enemyShooting.CalculateNextBatch();
                EnemyShip.ToggleShooting();
            }

            // Move to the target position with the specified speed modifier
            yield return StartCoroutine(pathPoint.c ? CurveToPosition(targetPosition, pathPoint.s * EnemyShip.MovementSpeedModifier, pathPoint.f) : MoveToPosition(targetPosition, pathPoint.s * EnemyShip.MovementSpeedModifier, pathPoint.f));
        }

        // Continue moving in the last direction after the last path point
        ContinueMoving();
    }

    private void ContinueMoving()
    {
        Vector3 finalTargetPosition = transform.position + lastDirection * 1000f; // A far away point in the same direction
        StartCoroutine(MoveToPosition(finalTargetPosition, lastSpeedModifier, 1)); // Move to this point with the last speed modifier and facing direction
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition, float speedModifier, int faceDirection)
    {
        while (transform.position != targetPosition)
        {
            Vector3 direction = targetPosition - transform.position;
            lastDirection = direction.normalized; // Capture the last direction
            lastSpeedModifier = speedModifier; // Capture the last speed modifier
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, BaseSpeed * speedModifier * Time.deltaTime);

            FaceDirection(faceDirection, direction);

            yield return null;
        }
    }

    private IEnumerator CurveToPosition(Vector3 targetPosition, float speedModifier, int faceDirection)
    {
        Vector3 startPosition = transform.position;
        Vector3 currentDirection = lastDirection != Vector3.zero ? lastDirection : (targetPosition - startPosition).normalized;
        Vector3 controlPoint = startPosition + currentDirection * Vector3.Distance(startPosition, targetPosition) / 2;

        float t = 0;
        while (t < 1)
        {
            t += BaseSpeed * speedModifier * Time.deltaTime / Vector3.Distance(startPosition, targetPosition);
            Vector3 direction = CalculateBezierPoint(t, startPosition, controlPoint, targetPosition) - transform.position;
            lastDirection = direction.normalized; // Capture the last direction
            lastSpeedModifier = speedModifier; // Capture the last speed modifier
            transform.position = Vector3.MoveTowards(transform.position, CalculateBezierPoint(t, startPosition, controlPoint, targetPosition), BaseSpeed * speedModifier * Time.deltaTime);

            FaceDirection(faceDirection, direction);

            yield return null;
        }
    }

    private void FaceDirection(int faceDirection, Vector3 direction)
    {
        switch (faceDirection)
        {
            case 0: // Face down
                Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, 180f));
                transform.rotation = SmoothRotateDirection(transform.rotation, targetRotation, 10f, 3f, 15f);
                break;
            case 1: // Face Direction
                FaceDir(direction);
                break;
            case 2: // Face Player
                if (PlayerManager.Inst.ActivePlayerShip != null)
                {
                    Vector3 playerDirection = PlayerManager.Inst.ActivePlayerShip.transform.position - transform.position;
                    float playerAngle = Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg;
                    targetRotation = Quaternion.Euler(new Vector3(0, 0, playerAngle + AngleOffset)); // Adjusting by AngleOffset
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 4f);
                }
                else
                {
                    FaceDir(direction);
                    break;
                }
                break;
        }
    }

    private void FaceDir(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle + AngleOffset));
        transform.rotation = SmoothRotateDirection(transform.rotation, targetRotation, 10f, 3f, 10f); // Use easeRate = 2f and initialSpeed = 0.5f
    }
    
    private Quaternion SmoothRotateDirection(Quaternion from, Quaternion to, float maxSpeed, float easeRate = 2f, float initialSpeed = 0.1f)
    {
        float angle = Quaternion.Angle(from, to);
        float t = Mathf.Pow(angle / 180f, easeRate); // Use Mathf.Pow to control the easing curve
        float speed = Mathf.Lerp(maxSpeed * initialSpeed, maxSpeed, t); // Interpolate speed from maxSpeed * initialSpeed to maxSpeed based on t
        return Quaternion.RotateTowards(from, to, speed * Time.deltaTime);
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0; // (1-t)^2 * p0
        p += 2 * u * t * p1; // 2 * (1-t) * t * p1
        p += tt * p2; // t^2 * p2

        return p;
    }    

        void OnDestroy()
        {
            // Ensure all coroutines started by this MonoBehaviour are stopped when the GameObject is destroyed
            StopAllCoroutines();
        }
    }
