using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager Inst { get; private set; }
    public List<MonoBehaviour> controllerScripts; // List of MonoBehaviour scripts
    public float ScrollSpeedModifier = 1f;

    private List<IBackgroundController> controllers;

    void Awake()
    {
        if (Inst != null && Inst != this)
        {
            Debug.Log("BackgroundManager already exists");
            Destroy(gameObject);
            return;  // Ensure no further code execution in this instance
        }
        Inst = this;

        // Convert MonoBehaviour list to IBackgroundController list
        controllers = new List<IBackgroundController>();
        foreach (var script in controllerScripts)
        {
            if (script is IBackgroundController)
            {
                controllers.Add(script as IBackgroundController);
            }
            else
            {
                Debug.LogError("One of the scripts does not implement IBackgroundController interface.");
            }
        }
    }

    private void Start()
    {
        foreach (var controller in controllers)
        {
            StartCoroutine(HandleScrolling(controller));
        }
    }

    private IEnumerator HandleScrolling(IBackgroundController controller)
    {
        while (true)
        {
            controller.CheckAndAdd();
            yield return new WaitForSeconds(controller.Duration);
        }
    }
}
