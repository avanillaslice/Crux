using UnityEngine;
using System.Collections.Generic;

public class ShipSelection : UIWindowBase
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

    public override void HandleSelect()
    {
        GameManager.InitiateGameplay(false);
    }

    public override void HandleMoveLeft()
    {
        if (Ships == null || Ships.Count == 0)
        {
            Debug.LogError("Ships list is not set.");
            return;
        }

        currentIndex = (currentIndex - 1 + Ships.Count) % Ships.Count;
        UpdateCursorPosition();
    }

    public override void HandleMoveRight()
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

    public override void HandleMoveUp()
    {
        // Do nothing
    }

    public override void HandleMoveDown()
    {
        // Do nothing
    }

    public override void HandleBackClicked()
    {
        // Do nothing
    }   
}
