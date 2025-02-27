using UnityEngine;
using System.Collections.Generic;

public class StarMapController : MonoBehaviour, IBackgroundController
{
    public GameObject initialStarMap;
    public float scrollSpeed; // Speed at which the star map scrolls
    public float upperYThreshold; // Y value to instantiate a new star map
    public float lowerYThreshold; // Y value to remove the old star map
    public float duration; // Duration to wait before checking positions again
    public float zAxisValue = 29f; // Constant z-axis value

    private GameObject starMapPrefab; // Prefab of the star map
    private List<GameObject> activeStarMaps = new List<GameObject>();
    private bool isUpsideDown = false; // Flag to track the orientation of the next star map

    void Awake()
    {
        starMapPrefab = AssetManager.StarMapPrefab;

        if (starMapPrefab == null)
        {
            Debug.LogError("StarMapPrefab is not assigned in GameConfig.");
            return;
        }

        // Initialize the first star map
        if (initialStarMap == null)
        {
            initialStarMap = Instantiate(starMapPrefab);
        }

        initialStarMap.transform.position = new Vector3(0, 0, zAxisValue);
        activeStarMaps.Add(initialStarMap);
    }

    void Update()
    {
        InitiateScrolling();
    }

    public void InitiateScrolling()
    {
        if (activeStarMaps.Count > 0)
        {
            foreach (var starMap in activeStarMaps)
            {
                starMap.transform.position += Vector3.down * scrollSpeed * BackgroundManager.Inst.ScrollSpeedModifier * Time.deltaTime;
            }
        }
    }

    public void CheckAndAdd()
    {
        // Check if we need to add a new star map
        if (activeStarMaps[activeStarMaps.Count - 1].transform.position.y <= upperYThreshold)
        {
            var lastStarMap = activeStarMaps[activeStarMaps.Count - 1];
            var newStarMapPosition = lastStarMap.transform.position + new Vector3(0, lastStarMap.GetComponent<SpriteRenderer>().bounds.size.y, 0);
            var newStarMap = Instantiate(starMapPrefab);
            newStarMap.transform.position = new Vector3(newStarMapPosition.x, newStarMapPosition.y, zAxisValue);

            // Set the rotation of the new star map
            if (isUpsideDown)
            {
                newStarMap.transform.rotation = Quaternion.Euler(0, 0, 180);
            }
            else
            {
                newStarMap.transform.rotation = Quaternion.identity;
            }

            // Toggle the orientation flag
            isUpsideDown = !isUpsideDown;

            activeStarMaps.Add(newStarMap);
        }

        // Check if we need to remove the old star map
        if (activeStarMaps[0].transform.position.y <= lowerYThreshold)
        {
            Destroy(activeStarMaps[0]);
            activeStarMaps.RemoveAt(0);
        }
    }

    public float Duration => duration;
}