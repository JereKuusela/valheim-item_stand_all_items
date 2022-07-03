using System.Collections.Generic;
using System.IO;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace ItemStandAllItems;
[BepInPlugin(GUID, NAME, VERSION)]
public class ItemStandAllItems : BaseUnityPlugin {
  public static bool IsServerDevcommands = false;
  const string GUID = "item_stand_all_items";
  const string LEGACY_GUID = "valheim.item_stand_all_items";
  const string NAME = "Item Stand AllItems";
  const string VERSION = "1.9";
  ServerSync.ConfigSync ConfigSync = new(GUID)
  {
    LegacyName = LEGACY_GUID,
    DisplayName = NAME,
    CurrentVersion = VERSION,
    MinimumRequiredVersion = "1.7.0"
  };
#nullable disable
  public static ManualLogSource Log;
#nullable enable
  public void Awake() {
    var legacyConfig = Path.Combine(Config.ConfigFilePath, $"{LEGACY_GUID}.cfg");
    var config = Path.Combine(Config.ConfigFilePath, $"{GUID}.cfg");
    if (File.Exists(legacyConfig))
      File.Move(legacyConfig, config);
    Log = Logger;
    Configuration.Init(ConfigSync, Config);
    Harmony harmony = new(GUID);
    harmony.PatchAll();
  }
  public void Start() {
    CommandWrapper.Init();
  }
}

///<summary>Adds additional check with the custom attach code.</summary>
[HarmonyPatch(typeof(ItemStand), nameof(ItemStand.CanAttach))]
public class ItemStand_CanAttach {
  static void Postfix(ItemStand __instance, ItemDrop.ItemData item, ref bool __result) {
    if (!Attacher.Enabled(__instance)) return;
    if (__result) return;
    __result = Attacher.GetAttach(item.m_dropPrefab) != null;
  }
}

///<summary>Replaces base game attach point finding with a custom one.</summary>
[HarmonyPatch(typeof(ItemStand), nameof(ItemStand.GetAttachPrefab))]
public class ItemStand_GetAttachPrefab {
  static void Postfix(ItemStand __instance, GameObject item, ref GameObject __result) {
    if (!Attacher.Enabled(__instance)) return;
    if (__result == null) __result = Attacher.GetAttach(item)!;
  }
}

///<summary>Post processed the attached item.</summary>
[HarmonyPatch(typeof(ItemStand), nameof(ItemStand.SetVisualItem))]
public class ItemStand_SetVisualItem {
  static void Postfix(ItemStand __instance) {
    Attacher.ReplaceItemDrop(__instance);
    Attacher.UpdateItemTransform(__instance);
    Attacher.HideIfItem(__instance);
  }
}

[HarmonyPatch(typeof(ItemStand), nameof(ItemStand.Awake))]
public class ItemStand_Awake {
  static void Postfix(ItemStand __instance) {
    Attacher.RemoveFromHideCache(__instance);
  }
}


///<summary>Allows removing from boss stones.</summary>
[HarmonyPatch(typeof(ItemStand), nameof(ItemStand.Interact))]
public class ItemStand_Interact {

  static void Postfix(ItemStand __instance, bool hold, ref bool __result) {
    if (__result) return;
    if (hold) return;
    if (!__instance.HaveAttachment()) return;
    if (!Attacher.Enabled(__instance)) return;
    if (!__instance.m_nview.IsOwner())
      __instance.m_nview.InvokeRPC("RequestOwn", new object[0]);
    ItemStand_DropItem.Invoking.Add(__instance);
    __instance.CancelInvoke("DropItem");
    __instance.InvokeRepeating("DropItem", 0f, 0.1f);
    __result = true;
  }
}


[HarmonyPatch(typeof(ItemStand), nameof(ItemStand.DropItem))]
public class ItemStand_DropItem {
  public static HashSet<ItemStand> Invoking = new();
  ///<summary>Allows removing from boss stones.</summary>
  static bool Prefix(ItemStand __instance) {
    if (Invoking.Contains(__instance) && !__instance.m_nview.IsOwner()) return false;
    __instance.CancelInvoke("DropItem");
    Invoking.Remove(__instance);
    return true;
  }
  ///<summary>Instantly shows the item stand when removing the item.</summary>
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
