using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace ItemStandAllItems {

  [BepInPlugin("valheim.jere.item_stand_all_items", "ItemStandAllItems", "1.4.0.0")]
  public class ItemStandAllItems : BaseUnityPlugin {
    public void Awake() {
      var harmony = new Harmony("valheim.jere.item_stand_all_items");
      Settings.Init(Config);
      harmony.PatchAll();
    }
  }

  // Adds additional check with the custom attach code.
  [HarmonyPatch(typeof(ItemStand), "CanAttach")]
  public class ItemStand_CanAttach {
    public static void Postfix(ItemStand __instance, ItemDrop.ItemData item, ref bool __result) {
      if (__result) return;
      if (__instance.m_name != "$piece_itemstand") return;
      __result = Attacher.GetAttach(item.m_dropPrefab) != null;
    }
  }

  // Adds additional check with the custom attach code.
  [HarmonyPatch(typeof(ItemStand), "GetAttachPrefab")]
  public class ItemStand_GetAttachPrefab {
    public static void Postfix(GameObject item, ref GameObject __result) {
      if (__result == null) {
        __result = Attacher.GetAttach(item);
      }
    }
  }

  [HarmonyPatch(typeof(ItemStand), "SetVisualItem")]
  public class ItemStand_SetVisualItem {
    ///<summary>Replaces ItemDrop script with an empty dummy object.</summary>
    private static void ReplaceItemDrop(ItemStand obj) {
      var item = obj.m_visualItem;
      if (item == null || item.GetComponent<ItemDrop>() == null) return;
      var attach = item.transform.parent;
      var dummy = Object.Instantiate<GameObject>(new GameObject(), attach.position, attach.rotation, attach);
      dummy.layer = item.layer;
      var children = new List<GameObject>();
      foreach (Transform child in item.transform) {
        if (child.gameObject.layer != dummy.layer) continue;
        children.Add(child.gameObject);
      }
      foreach (GameObject child in children)
        child.transform.SetParent(dummy.transform, false);
      ZNetScene.instance.Destroy(item);
      obj.m_visualItem = dummy;
    }
    ///<summary>Updates local transformation according to settings.</summary>
    private static void UpdateTransform(ItemStand obj) {
      var item = obj.m_visualItem;
      if (item == null) return;
      var transformations = Settings.CustomTransformations();
      if (!transformations.TryGetValue(obj.m_visualName.ToLower(), out var transformation)) return;
      item.transform.localPosition = transformation.Position;
      item.transform.localRotation = transformation.Rotation;
      item.transform.localScale = transformation.Scale;
    }
    public static void Postfix(ItemStand __instance) {
      ReplaceItemDrop(__instance);
      UpdateTransform(__instance);
    }
  }
}
