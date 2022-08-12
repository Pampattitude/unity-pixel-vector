using System.Runtime.CompilerServices;
using UnityEngine;

public partial struct PixelVector
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static PixelVector operator +(PixelVector pv) => new PixelVector(pv);
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static PixelVector operator -(PixelVector pv) => new PixelVector(-pv.x, -pv.y);
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static PixelVector operator +(PixelVector pv1, PixelVector pv2) => new PixelVector(pv1.x + pv2.x, pv1.y + pv2.y);
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static PixelVector operator -(PixelVector pv1, PixelVector pv2) => new PixelVector(pv1.x - pv2.x, pv1.y - pv2.y);
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static PixelVector operator *(PixelVector pv1, int v) => new PixelVector(pv1.x * v, pv1.y * v);
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static PixelVector operator /(PixelVector pv1, int v) => new PixelVector(pv1.x / v, pv1.y / v);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool operator ==(PixelVector pv1, PixelVector pv2) => pv1.x == pv2.x && pv1.y == pv2.y;
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool operator !=(PixelVector pv1, PixelVector pv2) => pv1.x != pv2.x || pv1.y != pv2.y;

  public static explicit operator Vector2Int(PixelVector pv) => new Vector2Int(pv.x, pv.y);
  public static explicit operator Vector2(PixelVector pv) => new Vector2((float)pv.x * PixelUtils.FLOAT_ONE_OVER_FACTOR, (float)pv.y * PixelUtils.FLOAT_ONE_OVER_FACTOR);
}
