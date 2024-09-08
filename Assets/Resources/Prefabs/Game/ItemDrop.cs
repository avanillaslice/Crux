using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ItemDrop : MonoBehaviour
{
    private EffectBase AssignedEffect;

    [SerializeField]
    private float Speed = 1.0f; // Speed of the downward movement, accessible in the inspector

    private Rigidbody2D rb;

    void Start()
    {
      rb = GetComponent<Rigidbody2D>();
      rb.velocity = new Vector2(0, -Speed); // Set the initial downward velocity
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
      if (collision.gameObject.CompareTag("Player"))
      {
        AssignedEffect.Activate(collision.gameObject);
        HUDManager.Inst.ShowPickupMessage(AssignedEffect.PickupMessage);
        Destroy(gameObject);
      }
    }
    
    public void InitialiseItem(EffectData effectData)
    {
      AssignedEffect = CreateEffect(effectData);

      if (AssignedEffect == null)
      {
        Debug.LogError("EffectData not found");
        Destroy(gameObject);
      }
    }

    private EffectBase CreateEffect(EffectData effectData)
    {
        EffectBase effect = null;

        switch (effectData.Type)
        {
            case EffectType.Weapon:
                effect = gameObject.AddComponent<WeaponEffect>();
                break;
            case EffectType.Passive:
                effect = gameObject.AddComponent<ShipEffect>();
                break;
            default:
                Debug.LogError("Unknown effect type");
                break;
        }

        if (effect != null)
        {
            effect.Type = effectData.Type;
            effect.SubType = effectData.SubType;
            effect.Expiry = effectData.Expiry;
            effect.Duration = effectData.Duration;
            effect.Amt = effectData.Amt;
            effect.PickupMessage = effectData.PickupMessage;
        }

        return effect;
    }
}
