// #define DEBUG_PIXEL_BOX_COLLIDER

using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PixelBoxCollider : PixelCollider
{
  private BoxCollider2D collider2D_;

  public PixelVector offset;
  public PixelVector size;
  private PixelVector halfSize_;
  public PixelVector halfSize { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => this.halfSize_; }
  public int skin;

  private PixelVector topLeft_;
  public PixelVector topLeft { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => this.topLeft_; }
  private PixelVector topRight_;
  public PixelVector topRight { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => this.topRight_; }
  private PixelVector bottomRight_;
  public PixelVector bottomRight { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => this.bottomRight_; }
  private PixelVector bottomLeft_;
  public PixelVector bottomLeft { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => this.bottomLeft_; }

  protected override void Awake()
  {
    base.Awake();
    this.collider2D_ = transform.GetComponent<BoxCollider2D>();

    this.rSync_();
    this.sync_();
  }

  void Update()
  {
    this.sync_();
#if DEBUG_PIXEL_BOX_COLLIDER
    DebugExtension.DebugBounds(new Bounds((Vector2)this.center, (Vector2)this.size), new Color(1f, .5f, 0f, .75f));
#endif
  }

  private void sync_()
  {
    this.collider2D_.offset = new Vector2((float)this.offset.x * PixelUtils.FLOAT_ONE_OVER_FACTOR, (float)this.offset.y * PixelUtils.FLOAT_ONE_OVER_FACTOR);
    this.collider2D_.size = new Vector2((float)this.size.x * PixelUtils.FLOAT_ONE_OVER_FACTOR, (float)this.size.y * PixelUtils.FLOAT_ONE_OVER_FACTOR);
    this.collider2D_.edgeRadius = (float)this.skin;

    this.center = this.pTransform.position + this.offset;
  }

  private void rSync_()
  {
    this.offset = new PixelVector((int)(this.collider2D_.offset.x * PixelUtils.FLOAT_FACTOR), (int)(this.collider2D_.offset.y * PixelUtils.FLOAT_FACTOR));
    this.size = new PixelVector((int)(this.collider2D_.size.x * PixelUtils.FLOAT_FACTOR), (int)(this.collider2D_.size.y * PixelUtils.FLOAT_FACTOR));
    this.halfSize_ = this.size / 2;
    this.skin = (int)(this.collider2D_.edgeRadius * PixelUtils.FLOAT_FACTOR);

    this.recomputeCorners_();
  }

  private void recomputeCorners_()
  {
    this.topLeft_ = this.pTransform.position + this.offset + new PixelVector(-this.halfSize.x, this.halfSize.y);
    this.topRight_ = this.pTransform.position + this.offset + new PixelVector(this.halfSize.x, this.halfSize.y);
    this.bottomRight_ = this.pTransform.position + this.offset + new PixelVector(this.halfSize.x, -this.halfSize.y);
    this.bottomLeft_ = this.pTransform.position + this.offset + new PixelVector(-this.halfSize.x, -this.halfSize.y);

    this.center = this.pTransform.position + this.offset;
    this.bounds.set(this.topRight_, this.bottomLeft_);
  }
}
