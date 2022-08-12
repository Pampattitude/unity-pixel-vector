#define DEBUG_TMP_PIXEL_CAST

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PixelTransform))]
public class TMPPixelCast : MonoBehaviour
{
  public static int RAY_COUNT = 1024; // 1024 * 4;

  private PixelTransform pTransform_;

  private PixelVector castDirection = new PixelVector(0, 2 * PixelUtils.SUBPIXEL_PER_UNIT);

  void Start()
  {
    this.pTransform_ = gameObject.GetComponent<PixelTransform>();
    this.pTransform_.position = new PixelVector(-10 * PixelUtils.SUBPIXEL_PER_UNIT, -1 * PixelUtils.SUBPIXEL_PER_UNIT);

    /* TMP */
    // Debug.Log($"ppu:{PixelTransform.PIXEL_PER_UNIT}, spp:{PixelTransform.SUBPIXEL_PER_PIXEL}, spu:{PixelTransform.SUBPIXEL_PER_UNIT}" + " " +
    // $"ff:{PixelTransform.FLOAT_FACTOR} f1/f:{PixelTransform.FLOAT_ONE_OVER_FACTOR}");

    // var pv1 = new PixelVector(Random.Range(0, 256 * 256), Random.Range(0, 256 * 256));
    // {
    //   const int iterationCount = 1 * 1000 * 1000;

    //   {
    //     int a = 0;
    //     System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
    //     stopWatch.Start();
    //     for (int j = 0; iterationCount > j; ++j)
    //       a += pv1.magnitude();
    //     Debug.Log("SLOW " + stopWatch.Elapsed.TotalMilliseconds + "ms " + a);
    //   }
    // }

    // {
    //   for (int i = 0; 100 > i; ++i)
    //   {
    //     var pv = new PixelVector(Random.Range(0, 256), Random.Range(0, 256));
    //     var v2 = (Vector2)pv;
    //     Debug.Log(
    //       $"{v2.magnitude * PixelTransform.FLOAT_FACTOR - pv.magnitude()}spx to {v2.magnitude - PixelVector.fromPixel(pv.magnitude())}f => " +
    //       $"{pv} ({pv.ToFloatString()}) => {pv.magnitude()}spx to {PixelVector.fromPixel(pv.magnitude())}f vs Vector2({v2.magnitude})"
    //     );
    //   }
    // }

    // {
    //   for (int i = 0; 100 > i; ++i)
    //   {
    //     var pv = new PixelVector(Random.Range(0, 256), Random.Range(0, 256));
    //     var v2 = (Vector2)pv;
    //     Debug.Log(
    //       $"F{v2.magnitude * PixelTransform.FLOAT_FACTOR - pv.fMagnitude()}spx to {v2.magnitude - PixelVector.fromPixel(pv.fMagnitude())}f => " +
    //       $"{pv} ({pv.ToFloatString()}) => {pv.fMagnitude()}spx to {PixelVector.fromPixel(pv.fMagnitude())}f vs Vector2({v2.magnitude})"
    //     );
    //   }
    // }
    /* EOTMP */
  }

  void Update()
  {
    this.pTransform_.position += new PixelVector((int)Mathf.Max((1f * PixelUtils.FLOAT_FACTOR * Time.deltaTime), 1f), 0);

    int count = TMPPixelCast.RAY_COUNT;
#if DEBUG_TMP_PIXEL_CAST
    System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
    stopWatch.Start();
#endif
    ReservedArray<PixelPhysics.CastHit> results = null;
    for (int i = 0; count > i; ++i)
      results = PixelPhysics.instance.rayCast(this.pTransform_.absolutePosition, this.castDirection, gameObject.layer);

#if DEBUG_TMP_PIXEL_CAST
    stopWatch.Stop();
    Debug.Log($"PIXEL {stopWatch.Elapsed.TotalMilliseconds}ms ({TMPPixelCast.RAY_COUNT} casts, {results.count} results)");
    // Debug.Log($"{TMPPixelCast.RAY_COUNT} casts, {results.count} results");
    DebugExtension.DebugArrow((Vector2)(this.pTransform_.absolutePosition), (Vector2)(this.castDirection), Color.red);
#endif
  }
}
