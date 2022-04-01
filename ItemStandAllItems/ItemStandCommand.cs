using UnityEngine;
namespace ItemStandAllItems;
public class ItemStandCommand {
  private static ItemStand GetHovered(Terminal context) {
    if (Player.m_localPlayer == null) return null;
    var hovered = Player.m_localPlayer.m_hovering;
    if (hovered == null || !Attacher.Enabled(hovered.GetComponentInParent<ItemStand>())) {
      Helper.AddMessage(context, "No itemstand is being hovered.");
      return null;
    }
    return hovered.GetComponentInParent<ItemStand>();
  }
  public ItemStandCommand() {
    CommandWrapper.Register("itemstand_offset", (int index, int subIndex) => {
      if (index == 0) return CommandWrapper.FRU("Sets the offset", subIndex);
      return null;
    });
    new Terminal.ConsoleCommand("itemstand_offset", "[forward,up,right=0,0,0] - Sets the offset.", (Terminal.ConsoleEventArgs args) => {
      var itemStand = GetHovered(args.Context);
      if (!itemStand) return;
      var value = Vector3.zero;
      if (args.Length > 1) value = Helper.ParseZYX(args[1]);
      itemStand.m_nview.GetZDO().Set("offset", value);
    });
    CommandWrapper.Register("itemstand_rotation", (int index, int subIndex) => {
      if (index == 0) return CommandWrapper.FRU("Sets the rotation", subIndex);
      return null;
    });
    new Terminal.ConsoleCommand("itemstand_rotation", "[forward,up,right=0,0,0] - Sets the rotation.", (Terminal.ConsoleEventArgs args) => {
      var itemStand = GetHovered(args.Context);
      if (!itemStand) return;
      var value = Vector3.zero;
      if (args.Length > 1) value = Helper.ParseZYX(args[1]);
      itemStand.m_nview.GetZDO().Set("rotation", value);
    });
    CommandWrapper.Register("itemstand_scale", (int index, int subIndex) => {
      if (index == 0) return CommandWrapper.Scale("Sets the scale", subIndex);
      return null;
    });
    new Terminal.ConsoleCommand("itemstand_scale", "[forward,up,right=1,1,1] - Sets the offset.", (Terminal.ConsoleEventArgs args) => {
      var itemStand = GetHovered(args.Context);
      if (!itemStand) return;
      var value = Vector3.one;
      if (args.Length > 1) value = Helper.ParseScale(args[1]);
      itemStand.m_nview.GetZDO().Set("scale", value);
    });
  }
}
