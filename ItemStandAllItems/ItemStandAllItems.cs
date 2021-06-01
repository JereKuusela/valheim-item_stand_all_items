using System;
using BepInEx;
using UnityEngine;
using HarmonyLib;

namespace ItemStandAllItems
{
  [BepInPlugin("valheim.jere.item_stand_all_items", "ItemStandAllItems", "1.1.0.0")]
  public class ItemStandAllItems : BaseUnityPlugin
  {
    void Awake()
    {
      var harmony = new Harmony("valheim.jere.item_stand_all_items");
      harmony.PatchAll();
    }
  }

  [HarmonyPatch(typeof(ItemStand), "CanAttach")]
  public class ItemStand_CanAttach
  {
    public static void Postfix(ItemStand __instance, ref bool __result)
    {
      if (__instance.m_name == "$piece_itemstand") __result = true;
    }
  }

  [HarmonyPatch(typeof(ItemStand), "GetAttachPrefab")]
  public class ItemStand_GetAttachPrefab
  {
    public static void Postfix(GameObject item, ref GameObject __result)
    {
      if (__result == null)
      {
        var collider = item.transform.GetComponentInChildren<Collider>();
        if (collider) __result = collider.transform.gameObject;
      }
    }
  }
}
