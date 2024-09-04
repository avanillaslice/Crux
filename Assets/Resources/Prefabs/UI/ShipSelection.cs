using UnityEngine;
using System.Collections.Generic;

public class ShipSelection : MonoBehaviour
{
    public List<GameObject> Ships;
    public GameObject Cursor;

    private int currentIndex = 0;
    private GameObject instantiatedCursor;

    void Start()
    {
        if (Ships == null || Ships.Count == 0 || Cursor == null)
        {
            Debug.LogError("Ships list or Cursor prefab is not set.");
            return;
        }

        instantiatedCursor = Instantiate(Cursor, Ships[currentIndex].transform.position, Quaternion.identity);
    }

    public void SelectShip()
    {
        GameManager.InitiateGameplay(false);
    }

    public void MoveCursorLeft()
    {
        if (Ships == null || Ships.Count == 0)
        {
            Debug.LogError("Ships list is not set.");
            return;
        }

        currentIndex = (currentIndex - 1 + Ships.Count) % Ships.Count;
        UpdateCursorPosition();
    }

    public void MoveCursorRight()
    {
        if (Ships == null || Ships.Count == 0)
        {
            Debug.LogError("Ships list is not set.");
            return;
        }

        currentIndex = (currentIndex + 1) % Ships.Count;
        UpdateCursorPosition();
    }

    private void UpdateCursorPosition()
    {
        if (instantiatedCursor != null)
        {
            instantiatedCursor.transform.position = Ships[currentIndex].transform.position;
        }
    }
}
