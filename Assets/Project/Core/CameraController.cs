using UnityEngine;

namespace Project.Core
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController Inst { get; private set; } // Singleton Inst

        public GameObject PlayerBoundary; // Reference to the PlayerBoundary game object

        private float LeftBorder, RightBorder, TopBorder, BottomBorder;

        // Movement
        private Vector3 TargetPosition;
        private float TotalDistance;
        private bool IsResetting = false;
        private bool HasReset = false;
        private float RequiredSpeed;
        private float Midpoint;

        void Start()
        {
            // Singleton pattern implementation
            if (Inst != null && Inst != this)
            {
                Debug.LogWarning("Multiple Insts of CameraController detected. Destroying duplicate.");
                Destroy(gameObject);
                return;
            }
            Inst = this;

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
            if (!GameConfig.CameraMovementEnabled) return;
        
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
            Vector3 newPosition = PlayerManager.Inst.ActivePlayerShip.CameraAnchor.transform.position;
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

            // Determine the speed factor based on the distance to the midpoint
            float speedFactor;
            if (currentDistance > Midpoint)
            {
                // Accelerate towards the midpoint
                speedFactor = 1 - ((currentDistance - Midpoint) / Midpoint);
            }
            else
            {
                // Decelerate after passing the midpoint
                speedFactor = currentDistance / Midpoint;
            }

            // Adjust the speed based on the speed factor
            float speed = RequiredSpeed * (1 + speedFactor); // Adjust the range as needed

            transform.position = Vector3.MoveTowards(transform.position, TargetPosition, speed * Time.deltaTime);
        }

        private void InitialiseCameraReset() {
            TargetPosition = PlayerManager.Inst.DefaultSpawnTarget;
            TargetPosition.z = transform.position.z; // Ensure z position remains constant
            TotalDistance = Vector3.Distance(transform.position, TargetPosition); // Calculate total distance

            // Calculate the required speed to reach the target before the respawn timer ends
            RequiredSpeed = TotalDistance / GameConfig.RespawnTimer;

            Midpoint = TotalDistance / 2;
            IsResetting = true;
        }
    
        private void HandleResetComplete() {
            IsResetting = false;
            HasReset = true;
        }
    }
}