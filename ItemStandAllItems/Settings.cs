
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BepInEx.Configuration;
using UnityEngine;

namespace ItemStandAllItems {
  public class CustomTransformation {
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;
  }
  public static class Settings {

    public static ConfigEntry<bool> configUseLegacyAttaching;
    public static bool UseLegacyAttaching => configUseLegacyAttaching.Value;
    public static ConfigEntry<string> configCustomTransformations;
    private static bool Parse(string arg, out float number) => float.TryParse(arg, NumberStyles.Float, CultureInfo.InvariantCulture, out number);

    public static Dictionary<string, CustomTransformation> CustomTransformations() {
      var split = configCustomTransformations.Value.Split('|');
      var dict = new Dictionary<string, CustomTransformation>();
      foreach (var item in split) {
        var args = item.Split(',').Select(value => value.Trim()).ToList();
        if (args.Count() < 1) continue;
        var position = Vector3.zero;
        if (args.Count() >= 4) {
          if (!Parse(args[1], out var x)) continue;
          if (!Parse(args[2], out var y)) continue;
          if (!Parse(args[3], out var z)) continue;
          position = new Vector3(x, y, z);
        }
        var rotation = Quaternion.identity;
        if (args.Count() >= 7) {
          if (!Parse(args[4], out var a)) continue;
          if (!Parse(args[5], out var b)) continue;
          if (!Parse(args[6], out var c)) continue;
          rotation = Quaternion.Euler(a, b, c);
        }
        var scale = Vector3.one;
        if (args.Count() >= 10) {
          if (!Parse(args[7], out var i)) continue;
          if (!Parse(args[8], out var j)) continue;
          if (!Parse(args[9], out var k)) continue;
          scale = new Vector3(i, j, k);
        }
        var transformation = new CustomTransformation()
        {
          Position = position,
          Rotation = rotation,
          Scale = scale
        };
        dict.Add(args[0].ToLower(), transformation);
      }
      return dict;
    }
    public static void Init(ConfigFile config) {
      configUseLegacyAttaching = config.Bind("General", "Use legacy attaching", false, "Use the previous attach way on version 1.1.0 (works for less items)");
      configCustomTransformations = config.Bind("General", "Custom transformations", "", "Apply custom position and rotation to attached items with format id1,x1,y1,z1,a1,b1,c1,i1,k1,j1|id2,x2,y2,z2,a2,b2,c2,i2,k2,j2");
    }
  }
}