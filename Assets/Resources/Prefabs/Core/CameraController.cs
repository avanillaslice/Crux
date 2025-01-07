using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; } // Singleton instance
    private readonly bool CameraMovementEnabled = true;

    public GameObject PlayerBoundary; // Reference to the PlayerBoundary game object

    private float LeftBorder, RightBorder, TopBorder, BottomBorder;
    private Vector3 TargetPosition;
    private float TotalDistance;
    private bool IsResetting = false;
    private bool HasReset = false;

    void Start()
    {
        // Singleton pattern implementation
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple instances of CameraController detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Initialize boundary positions
        if (PlayerBoundary != null)
        {
            Transform left = PlayerBoundary.transform.Find("Left");
            Transform right = PlayerBoundary.transform.Find("Right");
            Transform bottom = PlayerBoundary.transform.Find("Bottom");
            Transform top = PlayerBoundary.transform.Find("Top");

            if (left != null) LeftBorder = left.position.x;
            if (right != null) RightBorder = right.position.x;
            if (bottom != null) BottomBorder = bottom.position.y;
            if (top != null) TopBorder = top.position.y;
        }
        else
        {
            Debug.LogError("PlayerBoundary is not assigned.");
        }
    }

    void LateUpdate()
    {
        if (!CameraMovementEnabled) return;
        
        if (IsResetting || PlayerManager.Inst.ActivePlayerShip == null) {
            ResetCamera();
            return;
        }
        
        if (HasReset && PlayerManager.Inst.ActivePlayerShip != null) {
            UpdateCameraPos();
            HasReset = false;
            return;
        }

        if (!HasReset && !IsResetting) UpdateCameraPos();
    }

    private void UpdateCameraPos()
    {
        Vector3 newPosition = PlayerManager.Inst.ActivePlayerShip.transform.position;
        newPosition.z = transform.position.z; // Keep the camera's z position constant

        // Clamp the camera's position within the defined borders
        newPosition.x = Mathf.Clamp(newPosition.x, LeftBorder, RightBorder);
        newPosition.y = Mathf.Clamp(newPosition.y, BottomBorder, TopBorder);

        transform.position = newPosition;
    }

    private void ResetCamera()
    {
        if (!IsResetting) InitialiseCameraReset();

        float currentDistance = Vector3.Distance(transform.position, TargetPosition);
        if (currentDistance < 0.01f)
        {
            HandleResetComplete();
            return;
        }

        // Calculate the required speed to reach the target before the respawn timer ends
        float requiredSpeed = TotalDistance / GameConfig.RespawnTimer;

        // Calculate the midpoint
        float midpoint = TotalDistance / 2;

        // Determine the speed factor based on the distance to the midpoint
        float speedFactor;
        if (currentDistance > midpoint)
        {
            // Accelerate towards the midpoint
            speedFactor = 1 - ((currentDistance - midpoint) / midpoint);
        }
        else
        {
            // Decelerate after passing the midpoint
            speedFactor = currentDistance / midpoint;
        }

        // Adjust the speed based on the speed factor
        float speed = requiredSpeed * (1 + speedFactor); // Adjust the range as needed

        transform.position = Vector3.MoveTowards(transform.position, TargetPosition, speed * Time.deltaTime);
    }

    private void InitialiseCameraReset() {
        TargetPosition = PlayerManager.Inst.DefaultSpawnTarget;
        TargetPosition.z = transform.position.z; // Ensure z position remains constant
        TotalDistance = Vector3.Distance(transform.position, TargetPosition); // Calculate total distance
        IsResetting = true;
    }
    
    private void HandleResetComplete() {
        IsResetting = false;
        HasReset = true;
    }
}