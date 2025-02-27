using UnityEngine;
using System.Collections.Generic;

public class DistantStarsController : MonoBehaviour, IBackgroundController
{
    public float scrollSpeed; // Speed at which the objects scroll
    public float upperYThreshold; // Y value to instantiate a new object
    public float lowerYThreshold; // Y value to remove the old object
    public float duration; // Duration to wait before checking positions again
    private float zAxisValue = 27f; // Constant z-axis value
    public float minDistanceBetweenObjects = 6f; // Minimum distance between objects

    private List<GameObject> activeObjects = new List<GameObject>();

    void Awake()
    {
        if (AssetManager.DistantStarSprites.Count == 0)
        {
            Debug.LogError("No distant star sprites assigned in GameConfig.");
            return;
        }
    }

    void Update()
    {
        InitiateScrolling();
    }

  public void InitiateScrolling()
  {
    if (activeObjects.Count > 0)
    {
      foreach (var obj in activeObjects)
      {
        obj.transform.position += Vector3.down * scrollSpeed * BackgroundManager.Inst.ScrollSpeedModifier * Time.deltaTime;
      }
    }
  }

    public void CheckAndAdd()
    {
        // Check if we need to add a new object
        if (activeObjects.Count == 0 || activeObjects[activeObjects.Count - 1].transform.position.y <= upperYThreshold)
        {
            Vector3 newPosition;
            bool positionIsValid;

            do
            {
                positionIsValid = true;
                newPosition = new Vector3(
                    Random.Range(-6.5f, 6.5f), // Adjust the range as needed
                    Random.Range(5.5f, 8f), // Adjust the range as needed
                    zAxisValue
                );

                foreach (var obj in activeObjects)
                {
                    if (Vector3.Distance(newPosition, obj.transform.position) < minDistanceBetweenObjects)
                    {
                        positionIsValid = false;
                        break;
                    }
                }
            } while (!positionIsValid);

            var newObject = Instantiate(AssetManager.DistantStarPrefab, newPosition, Quaternion.identity);

            // Randomly choose a sprite and apply it to the new object
            var randomSprite = AssetManager.DistantStarSprites[Random.Range(0, AssetManager.DistantStarSprites.Count)];
            var spriteRenderer = newObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = randomSprite;
            }
            else
            {
                Debug.LogError("DistantStarPrefab does not have a SpriteRenderer component.");
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