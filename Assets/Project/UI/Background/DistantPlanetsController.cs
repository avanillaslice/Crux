using UnityEngine;
using System.Collections.Generic;

public class DistantPlanetsController : MonoBehaviour, IBackgroundController
{
    public float scrollSpeed; // Speed at which the objects scroll
    public float upperYThreshold = 3f; // Y value to instantiate a new object
    public float lowerYThreshold = -6f; // Y value to remove the old object
    public float duration = 1; // Duration to wait before checking positions again
    private readonly float ZAxisValue = 25f; // Constant z-axis value

    private List<GameObject> ActiveObjects = new List<GameObject>();
    private GameObject LastActiveObject;

    void Awake()
    {
        if (AssetManager.DistantPlanetSprites.Count == 0)
        {
            Debug.LogError("No distant planet sprites assigned in GameConfig.");
            return;
        }
    }

    void Update()
    {
        InitiateScrolling();
    }

  public void InitiateScrolling()
  {
    if (ActiveObjects.Count > 0)
    {
      foreach (var obj in ActiveObjects)
      {
        obj.transform.position += Vector3.down * scrollSpeed * BackgroundManager.Inst.ScrollSpeedModifier * Time.deltaTime;
      }
    }
  }

    public void CheckAndAdd()
    {
        // Check if we need to add a new object
        if (ActiveObjects.Count == 0 || LastActiveObject.transform.position.y <= upperYThreshold)
        {
            Vector3 newPosition = new Vector3(
                    Random.Range(-6.5f, 6.5f), // Adjust the range as needed
                    Random.Range(5.5f, 6.5f), // Adjust the range as needed
                    ZAxisValue
                );

            var newObject = Instantiate(AssetManager.DistantPlanetPrefab, newPosition, Quaternion.identity);
            LastActiveObject = newObject;
            ActiveObjects.Add(newObject);

            // Randomly choose a sprite and apply it to the new object
            var randomSprite = AssetManager.DistantPlanetSprites[Random.Range(0, AssetManager.DistantPlanetSprites.Count)];
            var spriteRenderer = newObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = randomSprite;
            }
            else
            {
                Debug.LogError("DistantPlanetPrefab does not have a SpriteRenderer component.");
            }
        }

        // Check if we need to remove the old object
        if (ActiveObjects.Count > 0 && ActiveObjects[0].transform.position.y <= lowerYThreshold)
        {
            Destroy(ActiveObjects[0]);
            ActiveObjects.RemoveAt(0);
        }
    }

    public float Duration => duration;
}