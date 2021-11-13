using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace ItemStandAllItems {

  [BepInPlugin("valheim.jere.item_stand_all_items", "ItemStandAllItems", "1.5.0.0")]
  public class ItemStandAllItems : BaseUnityPlugin {
    public void Awake() {
      var harmony = new Harmony("valheim.jere.item_stand_all_items");
      Settings.Init(Config);
      harmony.PatchAll();
    }
  }

  ///<summary>Adds additional check with the custom attach code.</summary>
  [HarmonyPatch(typeof(ItemStand), "CanAttach")]
  public class ItemStand_CanAttach {
    public static void Postfix(ItemStand __instance, ItemDrop.ItemData item, ref bool __result) {
      if (!Attacher.Enabled(__instance)) return;
      if (__result) return;
      __result = Attacher.GetAttach(item.m_dropPrefab) != null;
    }
  }

  ///<summary>Replaces base game attach point finding with a custom one.</summary>
  [HarmonyPatch(typeof(ItemStand), "GetAttachPrefab")]
  public class ItemStand_GetAttachPrefab {
    public static void Postfix(ItemStand __instance, GameObject item, ref GameObject __result) {
      if (!Attacher.Enabled(__instance)) return;
      if (__result == null) __result = Attacher.GetAttach(item);
    }
  }

  ///<summary>Post processed the attached item.</summary>
  [HarmonyPatch(typeof(ItemStand), "SetVisualItem")]
  public class ItemStand_SetVisualItem {
    public static void Postfix(ItemStand __instance) {
      Attacher.ReplaceItemDrop(__instance);
      Attacher.UpdateItemTransform(__instance);
      Attacher.HideIfItem(__instance);
    }
  }

  ///<summary>Instantly shows the item stand when removing the item.</summary>
  [HarmonyPatch(typeof(ItemStand), "DropItem")]
  public class ItemStand_DropItem {
    public static void Postfix(ItemStand __instance) {
      Attacher.HideIfItem(__instance);
    }
  }
}
