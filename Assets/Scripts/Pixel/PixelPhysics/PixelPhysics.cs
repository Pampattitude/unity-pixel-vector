using UnityEngine;

public partial class PixelPhysics
{
  public static Vector2Int gravity = new Vector2Int(0, (int)(-9.8f * PixelUtils.FLOAT_FACTOR));

  public static bool queriesHitTriggers => Physics2D.queriesHitTriggers;
  public static bool queriesStartInColliders => Physics2D.queriesStartInColliders;
  public static bool autoSyncTransforms => Physics2D.autoSyncTransforms;

  public static int getLayerMask(int layer) => Physics2D.GetLayerCollisionMask(layer);
}
