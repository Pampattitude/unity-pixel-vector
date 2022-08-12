#define DEBUG_PIXEL_QUAD_TREE
using UnityEngine;

public class PixelQuadTree : Util.Singleton<PixelQuadTree>
{
  public const int MAX_COLLIDERS_PER_MIN_SIZE = 1; // SHITTY
  public const int MAX_SIZE = PixelUtils.SUBPIXEL_PER_UNIT * 8192; // i.e. 8192spx
  public const int START_SIZE = PixelQuadTree.MIN_SIZE;
  public const int MIN_SIZE = PixelUtils.SUBPIXEL_PER_UNIT / PixelUtils.SUBPIXEL_PER_PIXEL; // i.e. 1spx

  public PixelQuadTreeNode root = new PixelQuadTreeNode(new PixelRect(0, 0, PixelQuadTree.START_SIZE, PixelQuadTree.START_SIZE));
  private byte expandDirection_ = 0; // 0 -> bl, 1 -> tr

  private ReservedArray<PixelCollider> emptyResult_ = new ReservedArray<PixelCollider>(0);

  // private ReservedArray<PixelCollider> queryResults_ = new ReservedArray<PixelCollider>(128); // TODO: resizeable array?

  public void Update()
  {
#if DEBUG_PIXEL_QUAD_TREE
    this.drawDebug_(this.root);
#endif
  }

#if DEBUG_PIXEL_QUAD_TREE
  private void drawDebug_(PixelQuadTreeNode node, int depth = 0)
  {
    if (node.tl != null)
      this.drawDebug_(node.tl, depth + 1);
    if (node.tr != null)
      this.drawDebug_(node.tr, depth + 1);
    if (node.br != null)
      this.drawDebug_(node.br, depth + 1);
    if (node.bl != null)
      this.drawDebug_(node.bl, depth + 1);

    var color = new UnityEngine.Color(1f, (float)depth / 16f, (float)depth / 16f, 0.25f);
    for (int i = 0; node.colliders.count > i; ++i)
    {
      DebugExtension.DebugCircle(
        (Vector2)(node.rect.tr + new PixelVector(-node.rect.size.x, 0)) + new Vector2(0.2f, -0.2f) + new Vector2(0.2f, 0f) * (float)i,
        Vector3.forward,
        new Color(color.r, color.g, color.b, color.a * 2f),
        radius: 0.05f,
        depthTest: false
      );

      DebugExtension.DebugBounds(
        new UnityEngine.Bounds((Vector2)node.colliders[i].bounds.center, (Vector2)node.colliders[i].bounds.size),
        Color.cyan
      );
    }
    DebugExtension.DebugBounds(
      new UnityEngine.Bounds((UnityEngine.Vector2)node.rect.center, (UnityEngine.Vector2)node.rect.size),
      color
    );
  }
#endif

  public void add(PixelCollider pc)
  {
#if DEBUG_PIXEL_QUAD_TREE
    int maxLoops = 64;
#endif
    while (!this.root.contains(pc.bounds))
    {
      this.expand_();
#if DEBUG_PIXEL_QUAD_TREE
      --maxLoops;
      if (maxLoops <= 0)
      {
        Debug.LogWarning("Max expand loops reached");
        break;
      }
#endif
    }
    this.root.add(pc);
  }

  public void remove(PixelCollider pc) => this.root.remove(pc);

  public void update(PixelCollider pc)
  {
    this.remove(pc);
    this.add(pc);
  }

  public ReservedArray<PixelCollider> query(PixelRect rect)
  {
    var results = this.root.getSmallestContain(rect);
    if (results == null)
      return this.emptyResult_;
    return results;
  }

  private void expand_()
  {
    PixelQuadTreeNode root;
    if (this.expandDirection_ == (byte)1) // tr
    {
      root = new PixelQuadTreeNode(new PixelRect(this.root.rect.tr + this.root.rect.size, this.root.rect.bl));
      root.copyColliders(this.root.colliders);
      root.bl = this.root;
    }
    else // bl
    {
      root = new PixelQuadTreeNode(new PixelRect(this.root.rect.tr, this.root.rect.bl - this.root.rect.size));
      root.copyColliders(this.root.colliders);
      root.tr = this.root;
    }
    if (root.rect.size.x > PixelQuadTree.MAX_SIZE || root.rect.size.y > PixelQuadTree.MAX_SIZE)
      throw new System.Exception($"PixelQuadTree is too big to expand to {root.rect.size} (max is {PixelQuadTree.MAX_SIZE})");

    this.root = root;

    this.expandDirection_ = this.expandDirection_ == (byte)1 ? (byte)0 : (byte)1;
  }
}
