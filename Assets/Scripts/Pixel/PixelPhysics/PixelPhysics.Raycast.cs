using UnityEngine;

// Performances (16 384 identical raycasts per frame):
//   * 16 384 rays, 0 colliders => ~4150μs
//   * 16 384 rays, 1 collider => ~7250μs
//   * 16 384 rays, 2 colliders => ~10500μs
//
// Conclusions
// * price per raycast: ~0.26μs
// * price per raycast per collider: ~0.26μs + ~0.20μs => ~0.46μs for 1 collider, ~0.66μs for 2 colliders
public partial class PixelPhysics : Util.Singleton<PixelPhysics>
{
  private const int MAX_RAY_CAST_HIT_COUNT = 64;
  private const int TOP = 0, BOTTOM = 1, LEFT = 2, RIGHT = 3, SIDE_TOTAL = 4;
  private const float OVERLAP_SKIN = 0.0625f; // i.e., 1px in world units
  public static readonly Vector2 OVERLAP_SKIN_VECTOR = new Vector2(OVERLAP_SKIN, OVERLAP_SKIN);

  public PixelQuadTree pQuadTreeInstance;

  private PixelPhysics.CastHit[] hitPossibilities_ = new PixelPhysics.CastHit[SIDE_TOTAL];
  private ReservedArray<PixelPhysics.CastHit> rayCastHits_ = new ReservedArray<PixelPhysics.CastHit>(PixelPhysics.MAX_RAY_CAST_HIT_COUNT);
  private bool inited_ = false;

  private struct TemporaryData_
  {
    public PixelVector min, max;
  }
  private TemporaryData_ tempData_;

  public class CastHit
  {
    public PixelVector point;
    public PixelVector normal;
    public PixelCollider collider;
    public int distance;

    public override string ToString()
    {
      return $"{point} {normal} {collider.name} {distance}";
    }
  }

  public ReservedArray<CastHit> rayCast(PixelVector origin, PixelVector direction, int originLayer)
  {
    if (!this.inited_)
    {
      this.pQuadTreeInstance = PixelQuadTree.instance;
      for (int i = 0; this.hitPossibilities_.Length > i; ++i)
        this.hitPossibilities_[i] = new CastHit { };
      this.inited_ = true;
    }

    this.rayCastHits_.empty();

    PixelVector target = origin + direction;
    this.tempData_.min = PixelVector.min(origin, target, result: PixelVector.getTemporary());
    this.tempData_.max = PixelVector.max(origin, target, result: PixelVector.getTemporary());

    bool prevQueriesStartInColliders = Physics2D.queriesStartInColliders;
    Physics2D.queriesStartInColliders = true;
    var results = this.pQuadTreeInstance.query(new PixelRect(this.tempData_.max, this.tempData_.min));
    Physics2D.queriesStartInColliders = prevQueriesStartInColliders;

    // Debug.Log(results.count);

    for (int i = 0; results.count > i; ++i)
    {
      var collider = results[i];
      if (!collider.enabled)
        continue;

      if (collider is PixelBoxCollider pbc)
        this.solveRayCastForBoxCollider_(origin, target, pbc);
    }

    return this.rayCastHits_;
  }

  // To read: https://stackoverflow.com/a/10906476
  // Source: https://gamedev.stackexchange.com/a/111106
  private void solveRayCastForBoxCollider_(PixelVector a, PixelVector b, PixelBoxCollider fCollider)
  {
    int aaXMinusAbX = a.x - b.x;
    int aaYMinusAbY = a.y - b.y;

    int aaXTimesAbY = a.x * b.y;
    int aaYTimesAbX = a.y * b.x;

    PixelVector? topIntersect = this.solveRayCastForLine_(a, b, fCollider.topLeft, fCollider.topRight, aaXMinusAbX, aaYMinusAbY, aaXTimesAbY, aaYTimesAbX);
    PixelVector? bottomIntersect = this.solveRayCastForLine_(a, b, fCollider.bottomLeft, fCollider.bottomRight, aaXMinusAbX, aaYMinusAbY, aaXTimesAbY, aaYTimesAbX);
    PixelVector? leftIntersect = this.solveRayCastForLine_(a, b, fCollider.topLeft, fCollider.bottomLeft, aaXMinusAbX, aaYMinusAbY, aaXTimesAbY, aaYTimesAbX);
    PixelVector? rightIntersect = this.solveRayCastForLine_(a, b, fCollider.topRight, fCollider.bottomRight, aaXMinusAbX, aaYMinusAbY, aaXTimesAbY, aaYTimesAbX);

    for (int i = 0; PixelPhysics.SIDE_TOTAL > i; ++i)
      this.hitPossibilities_[i].normal = PixelVector.zero;

    if (topIntersect.HasValue)
    {
      this.hitPossibilities_[PixelPhysics.TOP].point = topIntersect.Value;
      this.hitPossibilities_[PixelPhysics.TOP].normal = PixelVector.down;
      this.hitPossibilities_[PixelPhysics.TOP].collider = fCollider;
      this.hitPossibilities_[PixelPhysics.TOP].distance = this.hitPossibilities_[PixelPhysics.TOP].point.distance(a, PixelVector.getTemporary());

      if (leftIntersect.HasValue && leftIntersect.Value.x == topIntersect.Value.x && leftIntersect.Value.y == topIntersect.Value.y) // Top left
        this.hitPossibilities_[PixelPhysics.TOP].normal = PixelVector.getTemporary(-PixelUtils.normal45Deg.x, PixelUtils.normal45Deg.y);
      if (rightIntersect.HasValue && rightIntersect.Value.x == topIntersect.Value.x && rightIntersect.Value.y == topIntersect.Value.y) // Top right
        this.hitPossibilities_[PixelPhysics.TOP].normal = PixelVector.getTemporary(PixelUtils.normal45Deg.x, PixelUtils.normal45Deg.y);
    }
    if (bottomIntersect.HasValue)
    {
      this.hitPossibilities_[PixelPhysics.BOTTOM].point = bottomIntersect.Value;
      this.hitPossibilities_[PixelPhysics.BOTTOM].normal = PixelVector.down;
      this.hitPossibilities_[PixelPhysics.BOTTOM].collider = fCollider;
      this.hitPossibilities_[PixelPhysics.BOTTOM].distance = this.hitPossibilities_[PixelPhysics.BOTTOM].point.distance(a, PixelVector.getTemporary());

      if (leftIntersect.HasValue && leftIntersect.Value.x == bottomIntersect.Value.x && leftIntersect.Value.y == bottomIntersect.Value.y) // Bottom left
        this.hitPossibilities_[PixelPhysics.BOTTOM].normal = PixelVector.getTemporary(-PixelUtils.normal45Deg.x, -PixelUtils.normal45Deg.y);
      if (rightIntersect.HasValue && rightIntersect.Value.x == bottomIntersect.Value.x && rightIntersect.Value.y == bottomIntersect.Value.y) // Bottom right
        this.hitPossibilities_[PixelPhysics.BOTTOM].normal = PixelVector.getTemporary(PixelUtils.normal45Deg.x, -PixelUtils.normal45Deg.y);
    }
    if (leftIntersect.HasValue)
    {
      this.hitPossibilities_[PixelPhysics.LEFT].point = leftIntersect.Value;
      this.hitPossibilities_[PixelPhysics.LEFT].normal = PixelVector.left;
      this.hitPossibilities_[PixelPhysics.LEFT].collider = fCollider;
      this.hitPossibilities_[PixelPhysics.LEFT].distance = this.hitPossibilities_[PixelPhysics.LEFT].point.distance(a, PixelVector.getTemporary());
    }
    if (rightIntersect.HasValue)
    {
      this.hitPossibilities_[PixelPhysics.RIGHT].point = rightIntersect.Value;
      this.hitPossibilities_[PixelPhysics.RIGHT].normal = PixelVector.right;
      this.hitPossibilities_[PixelPhysics.RIGHT].collider = fCollider;
      this.hitPossibilities_[PixelPhysics.RIGHT].distance = this.hitPossibilities_[PixelPhysics.RIGHT].point.distance(a, PixelVector.getTemporary());
    }

    PixelPhysics.CastHit result = this.hitPossibilities_[PixelPhysics.TOP];
    if (
      !(result.normal != PixelVector.zero) || (
        (this.hitPossibilities_[PixelPhysics.BOTTOM].normal != PixelVector.zero) &&
        this.hitPossibilities_[PixelPhysics.BOTTOM].distance < result.distance
      )
    )
      result = this.hitPossibilities_[PixelPhysics.BOTTOM];
    if (
      !(result.normal != PixelVector.zero) || (
        (this.hitPossibilities_[PixelPhysics.LEFT].normal != PixelVector.zero) &&
        this.hitPossibilities_[PixelPhysics.LEFT].distance < result.distance
      )
    )
      result = this.hitPossibilities_[PixelPhysics.LEFT];
    if (
      !(result.normal != PixelVector.zero) || (
        (this.hitPossibilities_[PixelPhysics.RIGHT].normal != PixelVector.zero) &&
        this.hitPossibilities_[PixelPhysics.RIGHT].distance < result.distance
      )
    )
      result = this.hitPossibilities_[PixelPhysics.RIGHT];

    result.collider = fCollider;
    result.distance = a.distance(result.point, PixelVector.getTemporary()); // TODO: square distance?

    if ((result.normal != PixelVector.zero))
      this.rayCastHits_.add(result);
  }

  private PixelVector? solveRayCastForLine_(PixelVector aa, PixelVector ab, PixelVector ba, PixelVector bb, int aaXMinusAbX, int aaYMinusAbY, int aaXTimesAbY, int aaYTimesAbX)
  {
    int baXMinusBbX = ba.x - bb.x;
    int baYMinusBbY = ba.y - bb.y;

    long denominator = (long)aaXMinusAbX * (long)baYMinusBbY - (long)aaYMinusAbY * (long)baXMinusBbX;
    if (denominator == 0)
      return null;

    long baXTimesBbY = (long)ba.x * (long)bb.y;
    long baYTimesBbX = (long)ba.y * (long)bb.x;

    long aaXTimesAbYMINUSaaYTimesAbX = aaXTimesAbY - aaYTimesAbX;
    long baXTimesBbYMINUSbaYTimesBbX = baXTimesBbY - baYTimesBbX;

    long xNominator = aaXTimesAbYMINUSaaYTimesAbX * baXMinusBbX - aaXMinusAbX * baXTimesBbYMINUSbaYTimesBbX;
    long yNominator = aaXTimesAbYMINUSaaYTimesAbX * baYMinusBbY - aaYMinusAbY * baXTimesBbYMINUSbaYTimesBbX;

    PixelVector point = PixelVector.getTemporary((int)(xNominator / denominator), (int)(yNominator / denominator));
    PixelVector baBbMin = PixelVector.getTemporary(PixelVector.min(ba.x, bb.x), PixelVector.min(ba.y, bb.y));
    PixelVector baBbMax = PixelVector.getTemporary(PixelVector.max(ba.x, bb.x), PixelVector.max(ba.y, bb.y));
    if (!this.isPointInSegment_(point, this.tempData_.min, this.tempData_.max) || !this.isPointInSegment_(point, baBbMin, baBbMax))
      return null;

    return point;
  }

  [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
  private bool isPointInSegment_(PixelVector point, PixelVector min, PixelVector max) =>
    min.x <= point.x && point.x <= max.x &&
    min.y <= point.y && point.y <= max.y; // TODO: exclusive max?
}
