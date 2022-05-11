using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Bootstrap;
namespace ItemStandAllItems;
public static class CommandWrapper {
  public static Assembly? ServerDevcommands = null;
  public static void Init() {
    if (Chainloader.PluginInfos.TryGetValue("valheim.jerekuusela.server_devcommands", out var info)) {
      if (info.Metadata.Version.Major == 1 && info.Metadata.Version.Minor < 13) {
        ItemStandAllItems.Log.LogWarning($"Server devcommands v{info.Metadata.Version.Major}.{info.Metadata.Version.Minor} is outdated. Please update for better command instructions!");
      } else {
        ServerDevcommands = info.Instance.GetType().Assembly;
      }
    }
  }
  private static BindingFlags PublicBinding = BindingFlags.Static | BindingFlags.Public;
  private static Type Type() => ServerDevcommands!.GetType("ServerDevcommands.AutoComplete");
  private static Type InfoType() => ServerDevcommands!.GetType("ServerDevcommands.ParameterInfo");
  private static MethodInfo GetMethod(Type type, string name, Type[] types) => type.GetMethod(name, PublicBinding, null, CallingConventions.Standard, types, null);
  public static void Register(string command, Func<int, int, List<string>?> action) {
    if (ServerDevcommands == null) return;
    GetMethod(Type(), "Register", new[] { typeof(string), typeof(Func<int, int, List<string>>) }).Invoke(null, new object[] { command, action });
  }
  public static void Register(string command, Func<int, List<string>> action) {
    if (ServerDevcommands == null) return;
    GetMethod(Type(), "Register", new[] { typeof(string), typeof(Func<int, List<string>>) }).Invoke(null, new object[] { command, action });
  }
#nullable disable
  public static List<string> Scale(string description, int index) {
    if (ServerDevcommands == null) return null;
    return GetMethod(InfoType(), "Scale", new[] { typeof(string), typeof(int) }).Invoke(null, new object[] { description, index }) as List<string>;
  }
  public static List<string> FRU(string description, int index) {
    if (ServerDevcommands == null) return null;
    return GetMethod(InfoType(), "FRU", new[] { typeof(string), typeof(int) }).Invoke(null, new object[] { description, index }) as List<string>;
  }

  public static List<string> Info(string value) {
    if (ServerDevcommands == null) return null;
    return GetMethod(InfoType(), "Create", new[] { typeof(string) }).Invoke(null, new[] { value }) as List<string>;
  }
  public static List<string> RollPitchYaw(string description, int index) {
    if (ServerDevcommands == null) return null;
    return GetMethod(InfoType(), "RollPitchYaw", new[] { typeof(string), typeof(int) }).Invoke(null, new object[] { description, index }) as List<string>;
  }
#nullable enable
}
