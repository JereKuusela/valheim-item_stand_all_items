using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
namespace ItemStandAllItems;
[BepInPlugin("valheim.jere.item_stand_all_items", "ItemStandAllItems", "1.6.0.0")]
public class ItemStandAllItems : BaseUnityPlugin {
  public static bool IsServerDevcommands = false;
  ServerSync.ConfigSync ConfigSync = new("valheim.jere.item_stand_all_items")
  {
    DisplayName = "ItemStandAllItems",
    CurrentVersion = "1.6.0",
    MinimumRequiredVersion = "1.6.0"
  };
  public static ManualLogSource Log;
  public void Awake() {
    Log = Logger;
    Settings.Init(ConfigSync, Config);
    Harmony harmony = new("valheim.jere.item_stand_all_items");
    harmony.PatchAll();
  }
  public void Start() {
    CommandWrapper.Init();
  }
}

///<summary>Adds additional check with the custom attach code.</summary>
[HarmonyPatch(typeof(ItemStand), "CanAttach")]
public class ItemStand_CanAttach {
  static void Postfix(ItemStand __instance, ItemDrop.ItemData item, ref bool __result) {
    if (!Attacher.Enabled(__instance)) return;
    if (__result) return;
    __result = Attacher.GetAttach(item.m_dropPrefab) != null;
  }
}

///<summary>Replaces base game attach point finding with a custom one.</summary>
[HarmonyPatch(typeof(ItemStand), "GetAttachPrefab")]
public class ItemStand_GetAttachPrefab {
  static void Postfix(ItemStand __instance, GameObject item, ref GameObject __result) {
    if (!Attacher.Enabled(__instance)) return;
    if (__result == null) __result = Attacher.GetAttach(item);
  }
}

///<summary>Post processed the attached item.</summary>
[HarmonyPatch(typeof(ItemStand), "SetVisualItem")]
public class ItemStand_SetVisualItem {
  static void Postfix(ItemStand __instance) {
    Attacher.ReplaceItemDrop(__instance);
    Attacher.UpdateItemTransform(__instance);
    Attacher.HideIfItem(__instance);
  }
}

///<summary>Instantly shows the item stand when removing the item.</summary>
[HarmonyPatch(typeof(ItemStand), "DropItem")]
public class ItemStand_DropItem {
  static void Postfix(ItemStand __instance) {
    Attacher.HideIfItem(__instance);
  }
}

[HarmonyPatch(typeof(Terminal), nameof(Terminal.InitTerminal))]
public class SetCommands {
  static void Postfix() {
    new ItemStandCommand();
  }
}
