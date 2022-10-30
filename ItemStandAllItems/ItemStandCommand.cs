using System.Linq;
using UnityEngine;
namespace ItemStandAllItems;
public class ItemStandCommand {
  public static int HashHide = "itemstand_hide".GetStableHashCode();
  public static int HashHideLegacy = "hide".GetStableHashCode();
  public static int HashOffset = "itemstand_offset".GetStableHashCode();
  public static int HashOffsetLegacy = "offset".GetStableHashCode();
  public static int HashRotation = "itemstand_rotation".GetStableHashCode();
  public static int HashRotationLegacy = "rotation".GetStableHashCode();
  public static int HashScale = "itemstand_scale".GetStableHashCode();
  public static int HashScaleLegacy = "scale".GetStableHashCode();
  private static ItemStand? GetHovered(Terminal context) {
    if (Player.m_localPlayer == null) return null;
    var hovered = Player.m_localPlayer.m_hovering;
    if (hovered == null || !Attacher.Enabled(hovered.GetComponentInParent<ItemStand>())) {
      Helper.AddMessage(context, "No itemstand is being hovered.");
      return null;
    }
    return hovered.GetComponentInParent<ItemStand>();
  }
  private static ZDO? GetZDO(ItemStand? stand) => stand?.GetComponent<ZNetView>()?.GetZDO();
  public ItemStandCommand() {
    CommandWrapper.Register("itemstand_offset", (int index, int subIndex) => {
      if (index == 0) return CommandWrapper.FRU("Sets the offset", subIndex);
      return null;
    });
    new Terminal.ConsoleCommand("itemstand_offset", "[forward,right,up=0,0,0] - Sets the offset.", (args) => {
      var obj = GetHovered(args.Context);
      var zdo = GetZDO(obj);
      if (obj == null || zdo == null) return;
      var value = Vector3.zero;
      if (args.Length > 1) value = Helper.ParseYXZ(args[1]);
      zdo.Set(HashOffset, value);
      Attacher.Refresh(obj);
    });
    CommandWrapper.Register("itemstand_rotation", (int index, int subIndex) => {
      if (index == 0) return CommandWrapper.RollPitchYaw("Sets the rotation", subIndex);
      return null;
    });
    new Terminal.ConsoleCommand("itemstand_rotation", "[roll,pitch,yaw=0,0,0] - Sets the rotation.", (args) => {
      var obj = GetHovered(args.Context);
      var zdo = GetZDO(obj);
      if (obj == null || zdo == null) return;
      var value = Vector3.zero;
      if (args.Length > 1) value = Helper.ParseYXZ(args[1]);
      zdo.Set(HashRotation, value);
      Attacher.Refresh(obj);
    });
    CommandWrapper.Register("itemstand_scale", (int index, int subIndex) => {
      if (index == 0) return CommandWrapper.Scale("Sets the scale", subIndex);
      return null;
    });
    new Terminal.ConsoleCommand("itemstand_scale", "[x,y,z=1,1,1] - Sets the offset.", (args) => {
      var obj = GetHovered(args.Context);
      var zdo = GetZDO(obj);
      if (obj == null || zdo == null) return;
      var value = Vector3.one;
      if (args.Length > 1) value = Helper.ParseScale(args[1]);
      zdo.Set(HashScale, value);
      Attacher.Refresh(obj);
    });
    new Terminal.ConsoleCommand("itemstand_hide", "[-1/0/1] - Sets whether the item stand automatically hides when it has an item.", (args) => {
      var obj = GetHovered(args.Context);
      var zdo = GetZDO(obj);
      if (obj == null || zdo == null) return;
      var value = 0;
      if (args.Length > 1) value = (int)Helper.ParseFloat(args[1]);
      zdo.Set(HashHide, value);
      Attacher.Refresh(obj);
    });
    new Terminal.ConsoleCommand("itemstand_info", "- Prints the item stand properties.", (args) => {
      var stand = GetHovered(args.Context);
      var zdo = GetZDO(stand);
      if (zdo == null) return;
      var item = zdo.GetString("item", "");
      var variant = zdo.GetInt("variant", 0);
      var offset = zdo.GetVec3(HashOffset, Vector3.zero);
      var rotation = zdo.GetVec3(HashRotation, Vector3.zero);
      var scale = zdo.GetVec3(HashScale, Vector3.one);
      var hide = zdo.GetInt(HashHide, 0);
      var info = $"Item: {item}|{variant}";
      info += $"\nOffset: {offset.y}, {offset.x}, {offset.z}";
      info += $"\nRotation: {rotation.y}, {rotation.x}, {rotation.z}";
      info += $"\nScale: {scale.y}, {scale.x}, {scale.z}";
      info += $"\nHide: {hide}";
      Helper.AddMessage(args.Context, info);
    });
    new Terminal.ConsoleCommand("itemstand_migrate", "- Converts old item stand data to the new format.", (args) => {
      if (!ZNet.instance.IsServer() && !Configuration.CanMigrate) {
        Helper.AddMessage(args.Context, "This command is not available for clients.");
        return;
      }
      var zdos = ZDOMan.instance.m_objectsByID.Values;
      var hashes = Configuration.ItemStandIds.Select(id => id.GetStableHashCode()).ToHashSet();
      var updated = 0;
      foreach (var zdo in zdos) {
        if (!hashes.Contains(zdo.GetPrefab())) continue;
        var update = true;
        if (zdo.m_vec3 != null && zdo.m_vec3.ContainsKey(HashOffset)) update = false;
        if (zdo.m_vec3 != null && zdo.m_vec3.ContainsKey(HashRotation)) update = false;
        if (zdo.m_vec3 != null && zdo.m_vec3.ContainsKey(HashScale)) update = false;
        if (zdo.m_ints != null && zdo.m_ints.ContainsKey(HashHide)) update = false;
        var change = false;
        if (update) {
          if (zdo.m_vec3 != null && zdo.m_vec3.ContainsKey(HashOffsetLegacy)) {
            if (!zdo.m_vec3.ContainsKey(HashOffset)) {
              change = true;
              zdo.m_vec3[HashOffset] = zdo.m_vec3[HashOffsetLegacy];
            }
          }
          if (zdo.m_vec3 != null && zdo.m_vec3.ContainsKey(HashRotationLegacy)) {
            if (!zdo.m_vec3.ContainsKey(HashRotation)) {
              change = true;
              zdo.m_vec3[HashRotation] = zdo.m_vec3[HashRotationLegacy];
            }
          }
          if (zdo.m_vec3 != null && zdo.m_vec3.ContainsKey(HashScaleLegacy)) {
            if (!zdo.m_vec3.ContainsKey(HashScale)) {
              change = true;
              zdo.m_vec3[HashScale] = zdo.m_vec3[HashScaleLegacy];
            }
          }
          if (zdo.m_ints != null && zdo.m_ints.ContainsKey(HashHideLegacy)) {
            if (!zdo.m_ints.ContainsKey(HashHide)) {
              change = true;
              zdo.m_ints[HashHide] = zdo.m_ints[HashHideLegacy];
            }
          }
        }
        if (zdo.m_vec3 != null) {
          zdo.m_vec3.Remove(HashOffsetLegacy);
          zdo.m_vec3.Remove(HashRotationLegacy);
          zdo.m_vec3.Remove(HashScaleLegacy);
        }
        if (zdo.m_ints != null)
          zdo.m_ints.Remove(HashHideLegacy);
        if (change) {
          updated += 1;
          zdo.IncreseDataRevision();
        }
      }
      Attacher.Refresh();
      Helper.AddMessage(args.Context, $"Updated {updated} item stands.");
    });
  }
}
