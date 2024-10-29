using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace ItemStandAllItems;
[BepInPlugin(GUID, NAME, VERSION)]
public class ItemStandAllItems : BaseUnityPlugin
{
  const string GUID = "item_stand_all_items";
  const string NAME = "Item Stand All Items";
  const string VERSION = "1.21";
  readonly ServerSync.ConfigSync ConfigSync = new(GUID)
  {
    DisplayName = NAME,
    CurrentVersion = VERSION,
    MinimumRequiredVersion = VERSION
  };
#nullable disable
  public static ManualLogSource Log;
#nullable enable
  public void Awake()
  {
    Log = Logger;
    Configuration.Init(ConfigSync, Config);
    new Harmony(GUID).PatchAll();
  }
  public void Start()
  {
    CommandWrapper.Init();
  }
}

[HarmonyPatch(typeof(ItemStand))]
public class Patches
{

  ///<summary>Adds additional check with the custom attach code.</summary>
  [HarmonyPatch(nameof(ItemStand.CanAttach)), HarmonyPostfix]
  static void CanAttach(ItemStand __instance, ItemDrop.ItemData item, ref bool __result)
  {
    if (Configuration.Mode == "Vanilla") return;
    if (!Attacher.Enabled(__instance)) return;
    if (__result) return;
    __result = Attacher.GetAttach(item.m_dropPrefab) != null;
  }

  ///<summary>Replaces base game attach point finding with a custom one.</summary>
  [HarmonyPatch(nameof(ItemStand.GetAttachPrefab)), HarmonyPostfix]
  static GameObject GetAttachPrefab(GameObject result, GameObject item)
  {
    if (!Attacher.Enabled(LastStand)) return result;
    if (result) return result;
    return Attacher.GetAttach(item)!;
  }
  // GetAttachPrefab is now static, so the instance is stored here.
  static ItemStand? LastStand;
  ///<summary>Only post process on a change.</summary>
  [HarmonyPatch(nameof(ItemStand.SetVisualItem)), HarmonyPrefix]
  static void SetVisualItemPre(ItemStand __instance, string itemName, int variant, ref bool __state)
  {
    LastStand = __instance;
    // For some objects, the root object is returned which has a ZNetView.
    // This prevents a new ZDO being created.
    ZNetView.m_forceDisableInit = true;
    __state = __instance.m_visualName == itemName && __instance.m_visualVariant == variant;
  }
  ///<summary>Post processed the attached item.</summary>
  [HarmonyPatch(nameof(ItemStand.SetVisualItem)), HarmonyPostfix]
  static void SetVisualItem(ItemStand __instance, bool __state)
  {
    LastStand = null;
    if (!__state) Attacher.Refresh(__instance);
    ZNetView.m_forceDisableInit = false;
  }

  ///<summary>Allows removing from boss stones.</summary>
  [HarmonyPatch(nameof(ItemStand.Interact)), HarmonyPostfix]
  static void Interact(ItemStand __instance, bool hold, bool __runOriginal, ref bool __result)
  {
    if (!__runOriginal || __result || hold) return;
    if (!__instance.HaveAttachment()) return;
    if (!Attacher.Enabled(__instance)) return;
    if (!__instance.m_nview.IsOwner())
      __instance.m_nview.InvokeRPC("RequestOwn", []);
    Invoking.Add(__instance);
    __instance.CancelInvoke("DropItem");
    __instance.InvokeRepeating("DropItem", 0f, 0.1f);
    __result = true;
  }


  public static HashSet<ItemStand> Invoking = [];
  ///<summary>Allows removing from boss stones.</summary>
  [HarmonyPatch(nameof(ItemStand.DropItem)), HarmonyPrefix]
  static bool Prefix(ItemStand __instance)
  {
    if (Invoking.Contains(__instance) && !__instance.m_nview.IsOwner()) return false;
    __instance.CancelInvoke("DropItem");
    Invoking.Remove(__instance);
    return true;
  }
  ///<summary>Instantly shows the item stand when removing the item.</summary>
  [HarmonyPatch(nameof(ItemStand.DropItem)), HarmonyPostfix]
  static void Postfix(ItemStand __instance)
  {
    if (!Attacher.Enabled(__instance)) return;
    Attacher.HideIfItem(__instance);
  }
}
[HarmonyPatch(typeof(Terminal), nameof(Terminal.InitTerminal))]
public class SetCommands
{
  static void Postfix()
  {
    new ItemStandCommand();
  }
}
