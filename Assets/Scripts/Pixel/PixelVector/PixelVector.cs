using System.Runtime.CompilerServices;
using UnityEngine;

// Note: this class has a lot of assumptions:
//   * it is based on the PixelUtils unit, meaning it will always use SUBPIXEL_PER_UNIT as the unit and FLOAT_* as the conversion factors
//   * it supposes units cannot overflow, except when computing the sqrMagnitude (because squares are waaaaay too large, and operation order cannot be changed)
//     so it uses longs in that specific case, but ints everywhere else
//   * so far, aggressive inlining has produced way better results than not inlining (about 1/3 of the perf is saved with aggressive inlining)
//   * it may be used with temporary pixel vectors, but this can lead to undebuggable issues if looping around the pool size
[System.Serializable]
public partial struct PixelVector
{
  public const int UNIT = PixelUtils.SUBPIXEL_PER_UNIT;
  public static readonly PixelVector zero = new PixelVector(0, 0);
  public static readonly PixelVector one = new PixelVector(PixelUtils.SUBPIXEL_PER_UNIT, PixelUtils.SUBPIXEL_PER_UNIT);
  public static readonly PixelVector
    up = new PixelVector(0, PixelUtils.SUBPIXEL_PER_UNIT),
    down = new PixelVector(0, -PixelUtils.SUBPIXEL_PER_UNIT),
    left = new PixelVector(-PixelUtils.SUBPIXEL_PER_UNIT, 0),
    right = new PixelVector(PixelUtils.SUBPIXEL_PER_UNIT, 0)
  ;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static PixelVector getTemporary() => TemporaryPoolShort<PixelVector>.next();
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static PixelVector getTemporary(int x, int y) => PixelVector.getTemporary().set(x, y);

  public int x;
  public int y;

  public PixelVector(int x, int y)
  {
    this.x = x;
    this.y = y;
  }
  public PixelVector(PixelVector pv)
  {
    this.x = pv.x;
    this.y = pv.y;
  }
  // public PixelVector(Vector2 v)
  // {
  //   this.x = (int)(v.x * PixelUtils.FLOAT_FACTOR);
  //   this.y = (int)(v.y * PixelUtils.FLOAT_FACTOR);
  // }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static float fromPixel(int a) => (float)a * PixelUtils.FLOAT_ONE_OVER_FACTOR;
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static float fromPixel(float a) => a * PixelUtils.FLOAT_ONE_OVER_FACTOR;
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int toPixel(float a) => (int)(a * PixelUtils.FLOAT_FACTOR);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int abs(int a) => a >= 0 ? a : -a;
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static PixelVector abs(PixelVector pv) => new PixelVector(PixelVector.abs(pv.x), PixelVector.abs(pv.y));

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int min(int a, int b) => a < b ? a : b;
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static PixelVector min(PixelVector a, PixelVector b) => new PixelVector(PixelVector.min(a.x, b.x), PixelVector.min(a.y, b.y));
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static PixelVector min(PixelVector a, PixelVector b, PixelVector result)
  { result.x = PixelVector.min(a.x, b.x); result.y = PixelVector.min(a.y, b.y); return result; } // Note: used with temporary vectors
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int max(int a, int b) => a > b ? a : b;
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static PixelVector max(PixelVector a, PixelVector b) => new PixelVector(PixelVector.max(a.x, b.x), PixelVector.max(a.y, b.y));
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static PixelVector max(PixelVector a, PixelVector b, PixelVector result)
  { result.x = PixelVector.max(a.x, b.x); result.y = PixelVector.max(a.y, b.y); return result; } // Note: used with temporary vectors

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public int sqrMagnitude() => this.x * this.x + this.y * this.y; // TODO: power of two WILL overflow for values greater than 181 units
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public long lSqrMagnitude() => (long)this.x * (long)this.x + (long)this.y * (long)this.y;
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public double dSqrMagnitude() => (double)this.lSqrMagnitude();
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public int magnitude() => (int)Unity.Mathematics.math.sqrt(this.dSqrMagnitude()); // TODO: imprecise
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public float fMagnitude() => (float)Unity.Mathematics.math.sqrt(this.dSqrMagnitude()); // TODO: imprecise
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public int distance(PixelVector pv) => (this - pv).magnitude();
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public int distance(PixelVector pv, PixelVector result) => result.set(this).sub(pv).magnitude(); // Note: used with temporary vectors
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public float fDistance(PixelVector pv) => (this - pv).fMagnitude();

  // TODO: normalize

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public PixelVector set(PixelVector pv)
  { this.x = pv.x; this.y = pv.y; return this; }
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public PixelVector set(int x, int y)
  { this.x = x; this.y = y; return this; }
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public PixelVector add(PixelVector pv)
  { this.x += pv.x; this.y += pv.y; return this; }
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public PixelVector add(int x, int y)
  { this.x += x; this.y += y; return this; }
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public PixelVector sub(PixelVector pv)
  { this.x -= pv.x; this.y -= pv.y; return this; }
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public PixelVector sub(int x, int y)
  { this.x -= x; this.y -= y; return this; }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public override bool Equals(object obj) => obj != null && obj is PixelVector && this == (PixelVector)obj;
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public override int GetHashCode() => base.GetHashCode();

  public override string ToString() => this.toString();
  public string toString() => $"({this.x};{this.y})";
  public string ToFloatString() => $"({((float)this.x * PixelUtils.FLOAT_ONE_OVER_FACTOR)};{((float)this.y * PixelUtils.FLOAT_ONE_OVER_FACTOR)})";
}
