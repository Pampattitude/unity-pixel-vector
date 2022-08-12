using System.Runtime.CompilerServices;

public struct PixelRect
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static PixelRect getTemporary() => TemporaryPoolShort<PixelRect>.next();
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static PixelRect getTemporary(PixelVector tr, PixelVector bl) => PixelRect.getTemporary().set(tr, bl);
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static PixelRect getTemporary(int cx, int cy, int sx, int sy) => PixelRect.getTemporary().set(cx, cy, sx, sy);

  private PixelVector tr_, bl_; // Note: tr and bl because tr is the max vector and bl the min
  public PixelVector tr { get => this.tr_; } // TODO: optimize
  public PixelVector bl { get => this.bl_; } // TODO: optimize
  private PixelVector center_, size_, halfSize_;
  public PixelVector center { get => this.center_; set { this.center_ = value; this.computeCorners_(); } }
  public PixelVector size { get => this.size_; set { this.size_ = value; this.computeCorners_(); } }
  public PixelVector halfSize { get => this.halfSize_; }

  public PixelRect(PixelVector tr, PixelVector bl)
  {
    this.tr_ = tr;
    this.bl_ = bl;

    PixelVector signedSize = (this.tr_ - this.bl_);
    this.center_ = this.bl_ + signedSize / 2;
    this.size_ = PixelVector.abs(signedSize);
    this.halfSize_ = this.size_ / 2; // TODO: rounding to int can lead to a missing pixel here, implement a compensation system
  }
  public PixelRect(int centerX, int centerY, int sizeX, int sizeY)
  {
    this.center_ = new PixelVector(centerX, centerY);
    this.size_ = new PixelVector(sizeX, sizeY);
    this.halfSize_ = this.size_ / 2; // TODO: rounding to int can lead to a missing pixel here, implement a compensation system

    this.tr_ = this.center_ + this.halfSize_;
    this.bl_ = this.center_ - this.halfSize_;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool contains(PixelVector point) =>
    this.bl_.x <= point.x && point.x < this.tr_.x &&
    this.bl_.y <= point.y && point.y < this.tr_.y;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool contains(PixelRect rect) =>
    this.contains(rect.tr_) && this.contains(rect.bl_);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool intersects(PixelRect rect) =>
    this.bl.x <= rect.tr.x && this.tr.x > rect.bl.x &&
    this.tr.y > rect.bl.y && this.bl.y <= rect.tr.y;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public PixelRect set(PixelVector tr, PixelVector bl)
  {
    this.tr_ = tr;
    this.bl_ = bl;
    this.computeCenterAndSize_();
    return this;
  }
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public PixelRect set(int centerX, int centerY, int sizeX, int sizeY)
  {
    this.center_ = new PixelVector(centerX, centerY);
    this.size_ = new PixelVector(sizeX, sizeY);
    this.halfSize_ = this.size_ / 2; // TODO: rounding to int can lead to a missing pixel here, implement a compensation system
    this.computeCorners_();
    return this;
  }

  public override string ToString() => $"[{this.center_}:{this.size_}]";

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private void computeCenterAndSize_()
  {
    PixelVector signedSize = (this.tr_ - this.bl_);

    this.center_ = this.bl_ + signedSize / 2;
    this.size_ = PixelVector.abs(signedSize);
    this.halfSize_ = this.size_ / 2; // TODO: rounding to int can lead to a missing pixel here, implement a compensation system
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private void computeCorners_()
  {
    this.tr_ = this.center_ + this.halfSize_;
    this.bl_ = this.center_ - this.halfSize_;
  }
}
