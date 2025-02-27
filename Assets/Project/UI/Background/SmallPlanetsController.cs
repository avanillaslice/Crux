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
	private Vector3 previousPlayerPosition;

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
		if (PlayerManager.Inst == null || PlayerManager.Inst.ActivePlayerShip == null) InitiateScrolling();
		else InitiateScrollingWithParralax();
	}

	void Start()
	{
		// Initialize the previous player position
		if (PlayerManager.Inst == null || PlayerManager.Inst.ActivePlayerShip == null) previousPlayerPosition = new Vector3(0,0,0);
		else previousPlayerPosition = PlayerManager.Inst.ActivePlayerShip.transform.position;
	}

	public void InitiateScrolling()
	{
		if (activeObjects.Count > 0)
		{
			foreach (var obj in activeObjects)
			{
				// Move down based on scroll speed
				obj.transform.position += Vector3.down * scrollSpeed * BackgroundManager.Inst.ScrollSpeedModifier * Time.deltaTime;
			}
		}
	}

	public void InitiateScrollingWithParralax() {
		if (activeObjects.Count > 0)
		{
			// Get the player's current position
			Vector3 currentPlayerPosition = PlayerManager.Inst.ActivePlayerShip.transform.position;

			// Calculate the player's movement (delta)
			float deltaX = currentPlayerPosition.x - previousPlayerPosition.x;

			foreach (var obj in activeObjects)
			{
				// Move down based on scroll speed
				obj.transform.position += Vector3.down * scrollSpeed * BackgroundManager.Inst.ScrollSpeedModifier * Time.deltaTime;

				// Calculate the parallax effect based on player's movement
				float parallaxFactor = 0.0025f; // Adjust this value for more or less parallax effect

				// Update the object's x position based on the player's movement for parallax effect
				obj.transform.position += new Vector3(-deltaX * parallaxFactor, 0, 0);
			}

			// Update the previous player position for the next frame
			previousPlayerPosition = currentPlayerPosition;
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