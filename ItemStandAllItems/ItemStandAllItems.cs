using System;
using BepInEx;
using UnityEngine;
using HarmonyLib;

namespace ItemStandAllItems
{
  [BepInPlugin("valheim.jere.item-stand-all-items", "ItemStandAllItems", "1.0.0.0")]
  public class ItemStandAllItems : BaseUnityPlugin
  {
    void Awake()
    {
      var harmony = new Harmony("valheim.jere.item-stand-all-items");
      harmony.PatchAll();
    }
  }

  [HarmonyPatch(typeof(ItemStand), "CanAttach", new Type[] { typeof(ItemDrop.ItemData) })]
  public class ItemStand_CanAttach
  {
    public static bool Prefix(ItemDrop.ItemData item, ref bool __result, ItemStand __instance)
    {
      if (__instance.m_name == "$piece_itemstand") {
        __result = true;
        return false;
      }
      return true;
    }
  }
}
