using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace ItemStandAllItems;
public class Attacher {
  public static bool Enabled(ItemStand obj) => obj && obj.m_name == "$piece_itemstand";
  ///<summary>Legacy only finds the object with a collider. May not contain all models of the item resulting only in a partial item (like Graydward eye will miss the eye).</summary>
  private static GameObject GetAttachObjectLegacy(GameObject item) {
    var collider = item.transform.GetComponentInChildren<Collider>();
    return collider ? collider.transform.gameObject : null;
  }

  ///<summary>Returns the only child (if possible).</summary>
  private static GameObject GetChildModel(GameObject item) {
    GameObject onlyChild = null;
    foreach (Transform child in item.transform) {
      if (child.gameObject.layer != item.layer) continue;
      if (onlyChild) return null;
      onlyChild = child.gameObject;
    }
    return onlyChild;
  }
  ///<summary>Finds a given transform. Copypaste from base game code.</summary>
  private static GameObject GetTransform(GameObject item, string name) {
    var transform = item.transform.Find(name);
    return transform ? transform.gameObject : null;
  }
  public static GameObject GetAttach(GameObject item) {
    // Base game also uses "attach" transform but explicitly disabled for some items.
    // Check it first as it's the safest pick.
    var obj = GetTransform(item, "attach");
    if (obj) return obj;
    if (Configuration.UseLegacyAttaching) return GetAttachObjectLegacy(item);
    // Child object is preferred as it won't contain ItemDrop script or weird transformation.
    var childModel = GetChildModel(item);
    if (childModel)
      return childModel;
    return item;
  }
  private static HashSet<ZDOID> HideCache = new();

  ///<summary>Hides the item stand if it has an item.</summary>
  public static void HideIfItem(ItemStand obj) {
    if (!Enabled(obj)) return;
    var zdo = obj.m_nview.GetZDO();
    var hideValue = zdo.GetInt("hide", 0);
    if (hideValue == 0) hideValue = Configuration.HideAutomatically ? 1 : -1;
    var item = obj.m_visualItem;
    var show = !obj.HaveAttachment() || hideValue < 1;
    var previous = !HideCache.Contains(zdo.m_uid);
    if (show == previous) return;
    if (show) HideCache.Remove(zdo.m_uid);
    else HideCache.Add(zdo.m_uid);
    // Layer check to filter the attached item.
    var renderers = obj.GetComponentsInChildren<MeshRenderer>().Where(renderer => item == null || renderer.gameObject.layer == obj.gameObject.layer);
    foreach (var renderer in renderers) {
      if (renderer.enabled != show)
        renderer.enabled = show;
    }
  }
  ///<summary>Updates local transformation according to settings.</summary>
  public static void UpdateItemTransform(ItemStand obj) {
    if (!Attacher.Enabled(obj)) return;
    if (obj.m_visualItem == null) return;
    var transformations = Configuration.CustomTransformations();
    Configuration.Offset(transformations, obj);
    Configuration.Rotate(transformations, obj);
    Configuration.Scale(transformations, obj);
  }
  ///<summary>Replaces ItemDrop script with an empty dummy object.</summary>
  public static void ReplaceItemDrop(ItemStand obj) {
    if (!Attacher.Enabled(obj)) return;
    var item = obj.m_visualItem;
    if (item == null || item.GetComponent<ItemDrop>() == null) return;
    var attach = item.transform.parent;
    var dummy = Object.Instantiate<GameObject>(new(), attach.position, attach.rotation, attach);
    dummy.layer = item.layer;
    List<GameObject> children = new();
    foreach (Transform child in item.transform) {
      if (child.gameObject.layer != dummy.layer) continue;
      children.Add(child.gameObject);
    }
    foreach (GameObject child in children)
      child.transform.SetParent(dummy.transform, false);
    ZNetScene.instance.Destroy(item);
    obj.m_visualItem = dummy;
  }
}
