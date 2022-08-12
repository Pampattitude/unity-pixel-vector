public class PixelQuadGrid : Util.Singleton<PixelQuadGrid>
{
  // TODO: implement
  public const int MAX_COLLIDERS_PER_NODE = 8;
  public const int MAX_COLLIDERS_PER_QUERY = 64;

  public ReservedArray<PixelCollider> colliders = new ReservedArray<PixelCollider>(PixelQuadGrid.MAX_COLLIDERS_PER_NODE);
  public ReservedArray<PixelCollider> queryColliders = new ReservedArray<PixelCollider>(PixelQuadGrid.MAX_COLLIDERS_PER_QUERY);

  public ReservedArray<PixelCollider> query(PixelVector a, PixelVector b)
  {
    var min = PixelVector.min(a, b, result: PixelVector.getTemporary());
    var max = PixelVector.max(a, b, result: PixelVector.getTemporary());

    this.queryColliders.empty();
    for (int i = 0; this.colliders.count > i; ++i)
      this.queryColliders.add(this.colliders[i]);

    return this.queryColliders;
  }

  public void register(PixelCollider collider)
  {
    PixelVector roundedCenter = collider.center;
    this.colliders.add(collider);
  }
  public void unregister(PixelCollider collider)
  {
    this.colliders.removeBy(c => c == collider);
  }
}
