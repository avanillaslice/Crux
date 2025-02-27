using System.Collections;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
  public static CoroutineManager Inst { get; private set; }

      void Awake()
    {
        if (Inst != null && Inst != this)
        {
            Destroy(gameObject);
            return;  // Ensure no further code execution in this instance
        }
        Inst = this;
    }

    // EFFECT COROUTINE
    public void DeactivateEffectAfterDelay(EffectBase effect, float delayInSeconds)
    {
      StartCoroutine(DeactivateEffectAfterDelayCoroutine(effect, delayInSeconds));
    }
    public IEnumerator DeactivateEffectAfterDelayCoroutine(EffectBase effect, float delayInSeconds)
    {
        Debug.Log("Inumerator yielding for " + delayInSeconds);
        yield return new WaitForSeconds(delayInSeconds);
        Debug.Log("Coroutine completed, calling Deactivate");
        effect.Deactivate();
        Destroy(gameObject); // Clean up the manager GameObject
    }
}