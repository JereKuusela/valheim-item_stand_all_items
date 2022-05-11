using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BepInEx.Configuration;
using ServerSync;
using Service;
using UnityEngine;
namespace ItemStandAllItems;
public class CustomTransformation {
  public Vector3 Position;
  public Quaternion Rotation;
  public Vector3 Scale;
}
public static class Configuration {
#nullable disable
  public static ConfigEntry<bool> configLocked;
  public static ConfigEntry<bool> configUseLegacyAttaching;
  public static bool UseLegacyAttaching => configUseLegacyAttaching.Value;
  public static ConfigEntry<bool> configHideAutomatically;
  public static bool HideAutomatically => configHideAutomatically.Value;
  public static ConfigEntry<bool> configMoveCloser;
  public static bool MoveCloser => configMoveCloser.Value;
  public static ConfigEntry<bool> configEnableTransformations;
  public static bool EnableTransformations => configEnableTransformations.Value;
  public static ConfigEntry<string> configMaximumScale;
  public static float MaximumScale => ConfigWrapper.TryParseFloat(configMaximumScale);
  public static ConfigEntry<string> configMaximumOffset;
  public static float MaximumOffset => ConfigWrapper.TryParseFloat(configMaximumOffset);
  public static ConfigEntry<string> configCustomTransformations;
#nullable enable
  private static bool Parse(List<string> args, int index, out float number) {
    var arg = index < args.Count() ? args[index] : "";
    return float.TryParse(arg, NumberStyles.Float, CultureInfo.InvariantCulture, out number);
  }

  ///<summary>Parses custom transformations from the config.</summary>
  public static Dictionary<string, CustomTransformation> CustomTransformations() {
    var split = configCustomTransformations.Value.Split('|');
    Dictionary<string, CustomTransformation> dict = new();
    foreach (var item in split) {
      var args = item.Split(',').Select(value => value.Trim()).ToList();
      if (args.Count() < 1) continue;
      if (args[0] == "") continue;
      var position = Vector3.zero;
      if (Parse(args, 1, out var number))
        position.y = number;
      if (Parse(args, 2, out number))
        position.x = number;
      if (Parse(args, 3, out number))
        position.z = number;
      var rotation = Quaternion.identity;
      if (Parse(args, 4, out number))
        rotation.x = number;
      if (Parse(args, 5, out number))
        rotation.y = number;
      if (Parse(args, 6, out number))
        rotation.z = number;
      var scale = Vector3.one;
      if (Parse(args, 7, out number))
        scale.x = number;
      if (Parse(args, 8, out number))
        scale.y = number;
      if (Parse(args, 9, out number))
        scale.z = number;
      CustomTransformation transformation = new()
      {
        Position = position,
        Rotation = rotation,
        Scale = scale
      };
      if (!dict.ContainsKey(args[0].ToLower()))
        dict.Add(args[0].ToLower(), transformation);
    }
    return dict;
  }
  public static void Init(ConfigSync configSync, ConfigFile configFile) {
    ConfigWrapper wrapper = new("itemstand_config", configFile, configSync);
    var section = "General";
    configLocked = wrapper.BindLocking(section, "Config locked", false, "When true, server sets the config values.");
    configHideAutomatically = wrapper.Bind(section, "Hide automatically", false, "If true, hide stands are hidden when they have an item.");
    configUseLegacyAttaching = wrapper.Bind(section, "Use legacy attaching", false, "Use the previous attach way on version 1.1.0 (works for less items).");
    configMoveCloser = wrapper.Bind(section, "Move items closer", false, "If true, attached items will be closer to the item stand.");
    configCustomTransformations = wrapper.Bind(section, "Custom transformations", "", "Apply custom position and rotation to attached items with format: id,distance,offset_x,offset_y,angle_1,angle_2,angle_3,scale_1,scale_2,scale_3|id,distance,...");
    configEnableTransformations = wrapper.Bind(section, "Enable transformations", false, "If true, custom transformations are applied (may slightly affect performance).");
    configMaximumOffset = wrapper.Bind(section, "Maximum offset", "", "Maximum distance for the item offset.");
    configMaximumScale = wrapper.Bind(section, "Maximum scale", "", "Maximum multiplier for the item size.");
  }
  private static Dictionary<string, Vector3> OriginalPositions = new();
  ///<summary>Offsets the attached item according to the config.</summary>
  public static void Offset(Dictionary<string, CustomTransformation> transformations, ItemStand obj) {
    if (!EnableTransformations) return;
    var name = obj.m_visualName.ToLower();
    var item = obj.m_visualItem;
    if (!OriginalPositions.ContainsKey(name)) OriginalPositions.Add(name, item.transform.localPosition);
    var offset = Vector3.zero;
    if (transformations.TryGetValue(name, out var transformation))
      offset = transformation.Position;
    var custom = obj.m_nview.GetZDO().GetVec3("offset", Vector3.zero);
    var max = Configuration.MaximumOffset;
    if (max > 0f && custom.sqrMagnitude > max * max) custom *= max / custom.magnitude;
    var original = OriginalPositions[name];
    // Rotation causes y-coordinate to determine the distance.
    Vector3 parent = new(item.transform.parent.localPosition.y, item.transform.parent.localPosition.z, item.transform.parent.localPosition.x);
    item.transform.localPosition = original + offset + custom - (MoveCloser ? parent : Vector3.zero);
  }
  private static Dictionary<string, Quaternion> OriginalRotations = new();
  ///<summary>Rotates the attached item according to the config.</summary>
  public static void Rotate(Dictionary<string, CustomTransformation> transformations, ItemStand obj) {
    if (!EnableTransformations) return;
    var name = obj.m_visualName.ToLower();
    var item = obj.m_visualItem;
    if (!OriginalRotations.ContainsKey(name)) OriginalRotations.Add(name, item.transform.localRotation);
    var rotation = Quaternion.identity;
    if (transformations.TryGetValue(name, out var transformation))
      rotation = transformation.Rotation;
    var custom = Quaternion.Euler(obj.m_nview.GetZDO().GetVec3("rotation", Vector3.zero));
    var original = OriginalRotations[name];
    item.transform.localRotation = original * rotation * custom;
  }
  private static Dictionary<string, Vector3> OriginalScales = new();
  ///<summary>Scales the attached item according to the config.</summary>
  public static void Scale(Dictionary<string, CustomTransformation> transformations, ItemStand obj) {
    if (!EnableTransformations) return;
    var name = obj.m_visualName.ToLower();
    var item = obj.m_visualItem;
    if (!OriginalScales.ContainsKey(name)) OriginalScales.Add(name, item.transform.localScale);
    var scale = Vector3.one;
    if (transformations.TryGetValue(name, out var transformation))
      scale = transformation.Scale;
    var original = OriginalScales[name];
    var custom = obj.m_nview.GetZDO().GetVec3("scale", Vector3.one);
    var max = Configuration.MaximumScale;
    if (max > 0f && custom.sqrMagnitude > max * max) custom *= max / custom.magnitude;
    item.transform.localScale = new(original.x * scale.x * custom.x, original.y * scale.y * custom.y, original.z * scale.z * custom.z);
  }
}
