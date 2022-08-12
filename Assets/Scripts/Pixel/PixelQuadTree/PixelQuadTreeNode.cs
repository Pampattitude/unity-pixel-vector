// #define DEBUG_PIXEL_QUAD_TREE_NODE

using System.Runtime.CompilerServices;

public class PixelQuadTreeNode
{
  public PixelRect rect;
  public PixelQuadTreeNode
      tl = null,
      tr = null,
      br = null,
      bl = null
    ;
  public ReservedArray<PixelCollider> colliders; // TODO: use resizeable arrays instead of reserved

  public PixelQuadTreeNode(PixelRect rect)
  {
    this.rect = rect;

    int maxColliderCount = PixelVector.max(PixelQuadTree.MAX_COLLIDERS_PER_MIN_SIZE * this.rect.size.x / PixelQuadTree.MIN_SIZE, PixelQuadTree.MAX_COLLIDERS_PER_MIN_SIZE);
    this.colliders = new ReservedArray<PixelCollider>(maxColliderCount);
  }

  public void copyColliders(ReservedArray<PixelCollider> colliders)
  {
    for (int i = 0; colliders.count > i; ++i)
      this.colliders.add(colliders[i]);
  }

  public void add(PixelCollider pc)
  {
    if (!this.rect.intersects(pc.bounds))
      return;

    this.colliders.add(pc);

    if (this.rect.size.x <= PixelQuadTree.MIN_SIZE)
    {
#if DEBUG_PIXEL_QUAD_TREE_NODE
      UnityEngine.Debug.LogWarning("Min size reached");
#endif
      return; // Already the smallest size, do not reduce further
    }

    PixelRect pcBounds = pc.bounds;
    { // tl
      PixelRect tlRect = PixelRect.getTemporary(
        this.rect.center + PixelVector.getTemporary(0, this.rect.halfSize.y),
        this.rect.center + PixelVector.getTemporary(-this.rect.halfSize.x, 0)
      );
      if (this.tl == null)
      {
        if (tlRect.contains(pc.bounds))
        {
          this.tl = new PixelQuadTreeNode(tlRect);
          this.tl.add(pc);
        }
      }
      else
        this.tl.add(pc);
    }

    { // tr
      PixelRect trRect = PixelRect.getTemporary(
        this.rect.center + PixelVector.getTemporary(this.rect.halfSize.x, this.rect.halfSize.y),
        this.rect.center
      );
      if (this.tr == null)
      {
        if (trRect.contains(pc.bounds)) // fully contains the collider, need to create corner
        {
          this.tr = new PixelQuadTreeNode(trRect);
          this.tr.add(pc);
        }
      }
      else
        this.tr.add(pc);
    }

    { // br
      PixelRect brRect = PixelRect.getTemporary(
        this.rect.center + PixelVector.getTemporary(this.rect.halfSize.x, 0),
        this.rect.center + PixelVector.getTemporary(0, -this.rect.halfSize.y)
      );
      if (this.br == null)
      {
        if (brRect.contains(pc.bounds))
        {
          this.br = new PixelQuadTreeNode(brRect);
          this.br.add(pc);
        }
      }
      else
        this.br.add(pc);
    }

    { // bl
      PixelRect blRect = PixelRect.getTemporary(
        this.rect.center,
        this.rect.center + PixelVector.getTemporary(-this.rect.halfSize.x, -this.rect.halfSize.y)
      );
      if (this.bl == null)
      {
        if (blRect.contains(pc.bounds))
        {
          this.bl = new PixelQuadTreeNode(blRect);
          this.bl.add(pc);
        }
      }
      else
        this.bl.add(pc);
    }
  }

  public void remove(PixelCollider pc)
  {
    bool found = false;
    for (int i = 0; this.colliders.count > i; ++i)
      if (pc == this.colliders[i])
      { // TODO: check comparison operator on PixelCollider
        this.colliders.removeAt(i);
        found = true;
      }

    if (!found)
      return;

    if (this.tl != null && this.tl.intersects(pc.bounds))
    {
      this.tl.remove(pc);
      if (this.tl.colliders.count == 0)
        this.tl = null;
    }
    if (this.tr != null && this.tr.intersects(pc.bounds))
    {
      this.tr.remove(pc);
      if (this.tr.colliders.count == 0)
        this.tr = null;
    }
    if (this.br != null && this.br.intersects(pc.bounds))
    {
      this.br.remove(pc);
      if (this.br.colliders.count == 0)
        this.br = null;
    }
    if (this.bl != null && this.bl.intersects(pc.bounds))
    {
      this.bl.remove(pc);
      if (this.bl.colliders.count == 0)
        this.bl = null;
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool contains(PixelCollider pc) => this.rect.contains(pc.bounds);
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool contains(PixelRect rect) => this.rect.contains(rect);
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool intersects(PixelRect rect) => this.rect.intersects(rect);

  public ReservedArray<PixelCollider> getSmallestContain(PixelRect rect)
  {
    if (!this.contains(rect))
      return null;

    if (this.tl != null)
    {
      var subresults = this.tl.getSmallestContain(rect);
      if (subresults != null)
        return subresults;
    }
    if (this.tr != null)
    {
      var subresults = this.tr.getSmallestContain(rect);
      if (subresults != null)
        return subresults;
    }
    if (this.br != null)
    {
      var subresults = this.br.getSmallestContain(rect);
      if (subresults != null)
        return subresults;
    }
    if (this.bl != null)
    {
      var subresults = this.bl.getSmallestContain(rect);
      if (subresults != null)
        return subresults;
    }

    return this.colliders;
  }
}
