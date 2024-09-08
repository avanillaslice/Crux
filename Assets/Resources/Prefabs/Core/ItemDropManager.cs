using System.Collections.Generic;
using UnityEngine;

public class ItemDropManager : MonoBehaviour
{
  public static ItemDropManager Inst { get; private set; }
  private List<GameObject> ActiveItemDrops = new List<GameObject>();

  void Awake()
  {
    if (Inst != null && Inst != this)
    {
      Debug.Log("ItemDropManager already exists");
      Destroy(gameObject);
      return;
    }
    Inst = this;
  }

  public void CreateItemDrop(Vector3 position, EffectData effectData)
  {
    GameObject itemDrop = Instantiate(AssetManager.ItemDropPrefab, position, Quaternion.identity);
    ActiveItemDrops.Add(itemDrop);
    itemDrop.GetComponent<ItemDrop>().InitialiseItem(effectData);
  }

  public void DestroyAllActiveItemDrops()
  {
    Debug.Log("ActiveItemDrops.Count: " + ActiveItemDrops.Count);
    foreach (GameObject itemDrop in ActiveItemDrops)
    {
      if (itemDrop != null) Destroy(itemDrop);
    }
    ActiveItemDrops.Clear();
  }
}