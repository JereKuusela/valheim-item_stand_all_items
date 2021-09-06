using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using HarmonyLib;

namespace ItemStandAllItems
{
  [BepInPlugin("valheim.jere.item_stand_all_items", "ItemStandAllItems", "1.3.0.0")]
  public class ItemStandAllItems : BaseUnityPlugin
  {
    public static ConfigEntry<bool> configUseLegacyAttaching;
    public static ConfigEntry<bool> configPreventItemsWithItemDrop;
    public static ConfigEntry<bool> configRemoveRootOffset;
    public static ConfigEntry<bool> configRemoveRootRotation;
    public static ConfigEntry<bool> configPreferRootObject;
    void Awake()
    {
      configUseLegacyAttaching = Config.Bind("General", "UseLegacyAttaching", false, "Use the previous attach way on version 1.1.0 (works for less items)");
      configPreventItemsWithItemDrop = Config.Bind("General", "PreventItemDrops", false, "Enable for less instrusive approach (works for less items).");
      configRemoveRootOffset = Config.Bind("General", "RemoveRootOffset", true, "Some items (like Carrot seeds) have a weird offset at the root level.");
      configRemoveRootRotation = Config.Bind("General", "RemoveRootRotation", true, "Some items (like Iron ingots) have a weird rotation at the root level.");
      configPreferRootObject = Config.Bind("General", "PreferRootObject", false, "Use the root object instead of searching for a more suitable child object (1.2.0 behavior).");
      var harmony = new Harmony("valheim.jere.item_stand_all_items");
      harmony.PatchAll();
    }

    private static GameObject GetAttachObject(GameObject item)
    {
      if (configUseLegacyAttaching.Value)
      {
        // Legacy way only finds the object with a collider.
        // May not contain all models of the item resulting only in a partial item (like Graydward eye will miss the eye).
        var collider = item.transform.GetComponentInChildren<Collider>();
        if (collider) return collider.transform.gameObject;
      }
      else
      {
        if (!configPreferRootObject.Value)
        {
          var child = GetRenderingChild(item);
          if (child != item) return child;
        }
        if (configRemoveRootOffset.Value)
          item.transform.localPosition = Vector3.zero;
        if (configRemoveRootRotation.Value)
          item.transform.localRotation = Quaternion.identity;
        // New way always returns the whole item. This means extra components must be removed.
        return item;
      }
      return null;
    }
    // Copypaste from base game code.
    private static GameObject GetTransform(GameObject item, string name)
    {
      Transform transform = item.transform.Find(name);
      if (transform) return transform.gameObject;
      return null;
    }
    // In most cases the child object is better since it doesn't contain bad transform or ItemDrop component.
    // However this won't work if there are multiple children (like with Carrot seeds).
    private static GameObject GetRenderingChild(GameObject item)
    {
      if (item.GetComponent<MeshRenderer>() || item.transform.childCount != 1) return item;
      return item.transform.GetChild(0).gameObject;
    }
    public static GameObject GetAttach(GameObject item)
    {
      // Base game also uses "attach" transform but explicitly disabled for some items.
      // Check it first as it's the safest pick.
      var obj = GetTransform(item, "attach");
      if (obj) return obj;
      obj = GetAttachObject(item);
      if (obj && configPreventItemsWithItemDrop.Value && obj.GetComponent<ItemDrop>()) return null;
      return obj;
    }
  }

  // Adds additional check with the custom attach code.
  [HarmonyPatch(typeof(ItemStand), "CanAttach")]
  public class ItemStand_CanAttach
  {
    public static void Postfix(ItemStand __instance, ItemDrop.ItemData item, ref bool __result)
    {
      if (!__result && __instance.m_name == "$piece_itemstand") __result = ItemStandAllItems.GetAttach(item.m_dropPrefab) != null;
    }
  }


  // Adds additional check with the custom attach code.
  [HarmonyPatch(typeof(ItemStand), "GetAttachPrefab")]
  public class ItemStand_GetAttachPrefab
  {
    public static void Postfix(GameObject item, ref GameObject __result)
    {
      if (__result == null) __result = ItemStandAllItems.GetAttach(item);
    }
  }

  [HarmonyPatch(typeof(ItemStand), "SetVisualItem")]
  public class ItemStand_SetVisualItem
  {
    public static Dictionary<ZDO, ZNetView> m_instances(ZNetScene obj) => Traverse.Create(obj).Field<Dictionary<ZDO, ZNetView>>("m_instances").Value;
    public static void Postfix(GameObject ___m_visualItem)
    {
      if (___m_visualItem && !ItemStandAllItems.configPreventItemsWithItemDrop.Value)
      {
        UnityEngine.Object.Destroy(___m_visualItem.GetComponent<ItemDrop>());
        UnityEngine.Object.Destroy(___m_visualItem.GetComponent<Rigidbody>());
        UnityEngine.Object.Destroy(___m_visualItem.GetComponent<ZSyncTransform>());
        UnityEngine.Object.Destroy(___m_visualItem.GetComponent<ParticleSystem>());
        var zNetView = ___m_visualItem.GetComponent<ZNetView>();
        if (zNetView)
        {
          // This is needed to stop placed item stop acting as a item drop in multiplayer.
          // Copypaste from ZNetScene::Destroy (but without actually destroying the object).
          ZDO zdo = zNetView.GetZDO();
          if (zdo != null)
          {
            zNetView.ResetZDO();
            m_instances(ZNetScene.instance).Remove(zdo);
            if (zdo.IsOwner()) ZDOMan.instance.DestroyZDO(zdo);
          }
          UnityEngine.Object.Destroy(zNetView);
        }
      }
    }
  }
}
