using UnityEngine;
namespace ItemStandAllItems;
public class ItemStandCommand {
  private static ZDO? GetHovered(Terminal context) {
    if (Player.m_localPlayer == null) return null;
    var hovered = Player.m_localPlayer.m_hovering;
    if (hovered == null || !Attacher.Enabled(hovered.GetComponentInParent<ItemStand>())) {
      Helper.AddMessage(context, "No itemstand is being hovered.");
      return null;
    }
    return hovered.GetComponentInParent<ItemStand>()?.GetComponent<ZNetView>()?.GetZDO();
  }
  public ItemStandCommand() {
    CommandWrapper.Register("itemstand_offset", (int index, int subIndex) => {
      if (index == 0) return CommandWrapper.FRU("Sets the offset", subIndex);
      return null;
    });
    new Terminal.ConsoleCommand("itemstand_offset", "[forward,right,up=0,0,0] - Sets the offset.", (args) => {
      var zdo = GetHovered(args.Context);
      if (zdo == null) return;
      var value = Vector3.zero;
      if (args.Length > 1) value = Helper.ParseYXZ(args[1]);
      zdo.Set("offset", value);
    });
    CommandWrapper.Register("itemstand_rotation", (int index, int subIndex) => {
      if (index == 0) return CommandWrapper.RollPitchYaw("Sets the rotation", subIndex);
      return null;
    });
    new Terminal.ConsoleCommand("itemstand_rotation", "[roll,pitch,yaw=0,0,0] - Sets the rotation.", (args) => {
      var zdo = GetHovered(args.Context);
      if (zdo == null) return;
      var value = Vector3.zero;
      if (args.Length > 1) value = Helper.ParseYXZ(args[1]);
      zdo.Set("rotation", value);
    });
    CommandWrapper.Register("itemstand_scale", (int index, int subIndex) => {
      if (index == 0) return CommandWrapper.Scale("Sets the scale", subIndex);
      return null;
    });
    new Terminal.ConsoleCommand("itemstand_scale", "[x,y,z=1,1,1] - Sets the offset.", (args) => {
      var zdo = GetHovered(args.Context);
      if (zdo == null) return;
      var value = Vector3.one;
      if (args.Length > 1) value = Helper.ParseScale(args[1]);
      zdo.Set("scale", value);
    });
    new Terminal.ConsoleCommand("itemstand_hide", "[-1/0/1] - Sets whether the item stand automatically hides when it has an item.", (args) => {
      var zdo = GetHovered(args.Context);
      if (zdo == null) return;
      var value = 0;
      if (args.Length > 1) value = (int)Helper.ParseFloat(args[1]);
      zdo.Set("hide", value);
    });
    new Terminal.ConsoleCommand("itemstand_info", "- Prints the item stand properties.", (args) => {
      var zdo = GetHovered(args.Context);
      if (zdo == null) return;
      var item = zdo.GetString("item", "");
      var variant = zdo.GetInt("variant", 0);
      var offset = zdo.GetVec3("offset", Vector3.zero);
      var rotation = zdo.GetVec3("rotation", Vector3.zero);
      var scale = zdo.GetVec3("scale", Vector3.one);
      var hide = zdo.GetInt("hide", 0);
      var info = $"Item: {item}|{variant}";
      info += $"\nOffset: {offset.y}, {offset.x}, {offset.z}";
      info += $"\nRotation: {rotation.y}, {rotation.x}, {rotation.z}";
      info += $"\nScale: {scale.y}, {scale.x}, {scale.z}";
      info += $"\nHide: {hide}";
      Helper.AddMessage(args.Context, info);
    });
  }
}
