using UnityEngine;
using System.Collections.Generic;

public class PlanetsController : MonoBehaviour, IBackgroundController
{
    public float scrollSpeed; // Speed at which the objects scroll
    public float upperYThreshold; // Y value to instantiate a new object
    public float lowerYThreshold; // Y value to remove the old object
    public float duration; // Duration to wait before checking positions again
    private float zAxisValue = 23f; // Constant z-axis value
    public float minDistanceBetweenObjects = 6f; // Minimum distance between objects

    private List<GameObject> activeObjects = new List<GameObject>();

    private void Start()
    {
        if (AssetManager.PlanetSprites.Count == 0)
        {
            Debug.LogError("No planet sprites assigned in GameConfig.");
            return;
        }
    }

    private void Update()
    {
        InitiateScrolling();
    }

  public void InitiateScrolling()
  {
    if (activeObjects.Count > 0)
    {
      foreach (var obj in activeObjects)
      {
        obj.transform.position += Vector3.down * scrollSpeed * Time.deltaTime;
      }
    }
  }

    public void CheckAndAdd()
    {
        // Check if we need to add a new object
        if (activeObjects.Count == 0 || activeObjects[activeObjects.Count - 1].transform.position.y <= upperYThreshold)
        {
            float randomX;
            if (Random.value < 0.5f)
            {
                randomX = Random.Range(-7f, -5f);
            }
            else
            {
                randomX = Random.Range(5f, 7f);
            }

            Vector3 newPosition = new Vector3(
                    randomX, // Adjust the range as needed
                    9, // Adjust the range as needed
                    zAxisValue
                );

            var newObject = Instantiate(AssetManager.PlanetPrefab, newPosition, Quaternion.identity);

            // Randomly choose a sprite and apply it to the new object
            var randomSprite = AssetManager.PlanetSprites[Random.Range(0, AssetManager.PlanetSprites.Count)];
            var spriteRenderer = newObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = randomSprite;
            }
            else
            {
                Debug.LogError("PlanetPrefab does not have a SpriteRenderer component.");
            }

            activeObjects.Add(newObject);
        }

        // Check if we need to remove the old object
        if (activeObjects.Count > 0 && activeObjects[0].transform.position.y <= lowerYThreshold)
        {
            Destroy(activeObjects[0]);
            activeObjects.RemoveAt(0);
        }
    }

    public float Duration => duration;
}