using System.Globalization;
using UnityEngine;
namespace ItemStandAllItems;
public class Helper {
  public static Vector3 ParseYXZ(string value) {
    var vector = Vector3.zero;
    var split = value.Split(',');
    if (split.Length > 1) vector.x = Helper.ParseFloat(split[1]);
    if (split.Length > 0) vector.y = Helper.ParseFloat(split[0]);
    if (split.Length > 2) vector.z = Helper.ParseFloat(split[2]);
    return vector;
  }
  public static Vector3 ParseScale(string value) => SanityCheck(ParseYXZ(value));
  private static Vector3 SanityCheck(Vector3 scale) {
    // Sanity check and also adds support for setting all values with a single number.
    if (scale.y == 0) scale.y = 1;
    if (scale.x == 0) scale.x = scale.y;
    if (scale.z == 0) scale.z = scale.y;
    return scale;
  }
  public static float ParseFloat(string value, float defaultValue = 0) {
    if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result)) return result;
    return defaultValue;
  }
  public static void AddMessage(Terminal context, string message, bool priority = true) {
    context.AddString(message);
    var hud = MessageHud.instance;
    if (!hud) return;
    if (priority) {
      var items = hud.m_msgQeue.ToArray();
      hud.m_msgQeue.Clear();
      Player.m_localPlayer?.Message(MessageHud.MessageType.TopLeft, message);
      foreach (var item in items)
        hud.m_msgQeue.Enqueue(item);
      hud.m_msgQueueTimer = 10f;
    } else {
      Player.m_localPlayer?.Message(MessageHud.MessageType.TopLeft, message);
    }
  }

  public static string Print(float value) => value.ToString("F1", CultureInfo.InvariantCulture);
}