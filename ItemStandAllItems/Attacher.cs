using UnityEngine;

namespace ItemStandAllItems {
  public static class Attacher {
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
    // Copypaste from base game code.
    private static GameObject GetTransform(GameObject item, string name) {
      var transform = item.transform.Find(name);
      return transform ? transform.gameObject : null;
    }
    public static GameObject GetAttach(GameObject item) {
      // Base game also uses "attach" transform but explicitly disabled for some items.
      // Check it first as it's the safest pick.
      var obj = GetTransform(item, "attach");
      if (obj) return obj;
      if (Settings.UseLegacyAttaching) return GetAttachObjectLegacy(item);
      if (item.GetComponent<MeshRenderer>() == null) {
        // Child object is preferred as it won't contain ItemDrop script or weird transformation.
        var childModel = GetChildModel(item);
        if (childModel)
          return childModel;
      }
      return item;
    }
  }
}