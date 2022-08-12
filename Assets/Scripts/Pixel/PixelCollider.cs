using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(PixelTransform))]
[RequireComponent(typeof(Collider2D))]
public abstract class PixelCollider : MonoBehaviour
{
  public static int idIndex = 0;
  [NaughtyAttributes.ShowNonSerializedField] private int id_ = ++PixelCollider.idIndex;
  public int id { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => this.id_; }

  public PixelTransform pTransform;

  public PixelVector center;
  public PixelRect bounds = new PixelRect(PixelVector.zero, PixelVector.zero); // TODO: implement in children

  protected virtual void Awake()
  {
    this.pTransform = gameObject.GetComponent<PixelTransform>();
    this.center = this.pTransform.position;
  }

  void Start() => PixelQuadTree.instance.add(this);
  void Update() => PixelQuadTree.instance.update(this); // TODO: greatly optimize
  void OnDestroy()
  {
    if (PixelQuadTree.hasInstance)
      PixelQuadTree.instance.remove(this); // TODO: make it better
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool operator ==(PixelCollider a, PixelCollider b) => a.id == b.id;
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool operator !=(PixelCollider a, PixelCollider b) => a.id != b.id;
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public override bool Equals(object obj) => obj != null && obj is PixelCollider pc && this.id == pc.id;
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public override int GetHashCode() => base.GetHashCode();
}
