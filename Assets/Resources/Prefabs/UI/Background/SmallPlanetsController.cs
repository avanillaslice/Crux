using UnityEngine;
using System.Collections.Generic;

public class SmallPlanetsController : MonoBehaviour, IBackgroundController
{
    public float scrollSpeed; // Speed at which the objects scroll
    public float upperYThreshold; // Y value to instantiate a new object
    public float lowerYThreshold; // Y value to remove the old object
    public float duration; // Duration to wait before checking positions again
    private float zAxisValue = 24f; // Constant z-axis value
    public float minDistanceBetweenObjects = 6f; // Minimum distance between objects
    private float SpriteScale = 0.4f; // Scale factor for the sprites

    private List<GameObject> activeObjects = new List<GameObject>();

    void Awake()
    {
        if (AssetManager.PlanetSprites.Count == 0)
        {
            Debug.LogError("No planet sprites assigned in GameConfig.");
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
            float randomX;
            if (Random.value < 0.5f)
            {
                randomX = Random.Range(-4f, -2f);
            }
            else
            {
                randomX = Random.Range(2f, 4f);
            }

            Vector3 newPosition = new Vector3(
                    randomX, // Adjust the range as needed
                    14, // Adjust the range as needed
                    zAxisValue
                );

            var newObject = Instantiate(AssetManager.PlanetPrefab, newPosition, Quaternion.identity);

            newObject.transform.localScale *= SpriteScale;

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