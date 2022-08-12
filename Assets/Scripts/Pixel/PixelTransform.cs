using System.Runtime.CompilerServices;
using UnityEngine;

public class PixelTransform : MonoBehaviour
{
  [SerializeField] private PixelVector position_ = new PixelVector();
  public PixelVector position
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get => this.position_;
    set
    {
      this.position_ = value;
      this.recomputeAbsolute_();
    }
  }
  public int rotation;
  public Vector2 scale;

  private PixelVector absolutePosition_ = new PixelVector();
  public PixelVector absolutePosition { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => this.absolutePosition_; }
  private int absoluteRotation_;
  public int absoluteRotation { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => this.absoluteRotation_; }

  [SerializeField]
  private PixelTransform parent_ = null;
  public PixelTransform parent
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get => this.parent_;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    set => this.setParent_(value);
  }

  private int layer_;
  public int layer { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => this.layer_; }

  void Awake()
  {
    this.rSync_();
    this.setParent_(transform.parent);
    this.sync_();
  }

  void Update() => this.sync_();

  private void sync_()
  {
    transform.localPosition = new Vector3(
        (float)this.position_.x * PixelUtils.FLOAT_ONE_OVER_FACTOR,
        (float)this.position_.y * PixelUtils.FLOAT_ONE_OVER_FACTOR,
        0f
    );
    transform.localEulerAngles = new Vector3(0f, 0f, (float)this.rotation);
    transform.localScale = new Vector3(this.scale.x, this.scale.y, 1f);

    transform.gameObject.layer = this.layer_;
  }

  private void rSync_()
  {
    this.position_ = new PixelVector(
        (int)(transform.localPosition.x * PixelUtils.FLOAT_FACTOR),
        (int)(transform.localPosition.y * PixelUtils.FLOAT_FACTOR)
    );
    this.rotation = (int)transform.localEulerAngles.z;
    this.scale = transform.localScale;

    this.layer_ = gameObject.layer;
  }

  private void setParent_(Transform transform) => this.setParent_(transform?.GetComponent<PixelTransform>());
  private void setParent_(PixelTransform pTransform)
  {
    if (pTransform != null)
    {
      if (this.parent_)
        this.add_(this.parent_);
      this.sub_(pTransform);
      this.parent_ = pTransform;
      transform.parent = pTransform.transform;
    }
    else
    {
      if (this.parent_)
        this.add_(this.parent_); // Detaching from parent, set position to world
      this.parent_ = null;
      transform.parent = null;
    }

    this.recomputeAbsolute_();
  }

  private void recomputeAbsolute_()
  {
    if (this.parent != null)
    {
      this.absolutePosition_ = this.position_ + this.parent.absolutePosition;
      this.absoluteRotation_ = this.rotation + this.parent.absoluteRotation;
    }
    else
    {
      this.absolutePosition_ = this.position_;
      this.absoluteRotation_ = this.rotation;
    }
  }

  private void add_(PixelTransform pTransform)
  {
    this.position_ += pTransform.position;
    this.rotation += pTransform.rotation;
    this.scale *= pTransform.scale;
  }

  private void sub_(PixelTransform pTransform)
  {
    this.position_ -= pTransform.position;
    this.rotation -= pTransform.rotation;
    this.scale /= pTransform.scale;
  }

  [NaughtyAttributes.Button("Sync with start")]
  void editorSync_()
  {
    this.rSync_();
    this.setParent_(transform.parent);
    this.sync_();
  }
}
