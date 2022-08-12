using UnityEngine;

public class PixelUtils
{
  public const int PIXEL_PER_UNIT = 16;
  public const int SUBPIXEL_PER_PIXEL = 16;
  public const int SUBPIXEL_PER_UNIT = PixelUtils.PIXEL_PER_UNIT * PixelUtils.SUBPIXEL_PER_PIXEL;
  public const float FLOAT_FACTOR = (float)PixelUtils.SUBPIXEL_PER_UNIT;
  public const float FLOAT_ONE_OVER_FACTOR = 1f / PixelUtils.FLOAT_FACTOR;

  private static readonly Vector2 normal45DegFloat_ = new Vector2(1f, 1f).normalized * PixelUtils.FLOAT_FACTOR;
  public static readonly Vector2Int normal45Deg = new Vector2Int((int)PixelUtils.normal45DegFloat_.x, (int)PixelUtils.normal45DegFloat_.y);
}
