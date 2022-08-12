using System.Runtime.CompilerServices;

[System.Serializable]
public class ReservedArray<T>
{
  public int reserved = 0; // TODO: public get, private set
  public int count = 0;
  public T[] values;

  public ReservedArray(int reservedCount)
  {
    this.reserved = reservedCount;
    if (this.reserved > 0)
      this.values = new T[this.reserved];
  }

  public T this[int idx]
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get => this.values[idx]; // Note: unsafe 
    // get => idx >= this.count ? default(T) : this.values[idx]; // Note: safer
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    set
    {
      if (idx >= reserved)
        return;
      if (idx >= this.count)
        this.count = idx + 1;
      this.values[idx] = value;
      return;
    }
  }
  public static ReservedArray<T> operator +(ReservedArray<T> self, T v)
  {
    self.add(v);
    return self;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void add(T v)
  {
    if (this.reserved <= this.count) // Cannot insert, return
      return;
    this[this.count] = v;
    return;
  }

  public void removeAt(int idx)
  {
    if (this.count <= 0) // Nothing to do, return
      return;

    while (idx + 1 < this.count)
    {
      this[idx] = this[idx + 1];
      ++idx;
    }
    this.count -= 1;

    return;
  }

  public void removeBy(System.Func<T, bool> compareFct)
  {
    if (this.count <= 0) // Nothing to do, return
      return;

    for (int i = 0; this.count > i;)
    {
      if (compareFct(this.values[i]))
        this.removeAt(i);
      else
        ++i;
    }
    return;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void upsertBy(T v, System.Func<T, T, bool> compareFct) => this.upsertByIf(v, compareFct, (_1, _2) => true);
  public void upsertByIf(T v, System.Func<T, T, bool> compareFct, System.Func<T, T, bool> ifFct)
  {
    // Update
    for (int i = 0; this.count > i; ++i)
    {
      if (compareFct(this.values[i], v))
      {
        if (ifFct(this.values[i], v))
          this.values[i] = v;
        return;
      }
    }

    // Else, insert
    this.add(v);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void mergeBy(ReservedArray<T> v, System.Func<T, T, bool> compareFct) => this.mergeByIf(v, compareFct, (_1, _2) => true);
  public void mergeByIf(ReservedArray<T> v, System.Func<T, T, bool> compareFct, System.Func<T, T, bool> ifFct)
  {
    for (int i = 0; v.count > i; ++i)
      this.upsertByIf(v[i], compareFct, ifFct);
  }

  public void sortBy(System.Func<T, T, bool> compareFct)
  {
    for (int i = 0; this.count > i + 1;)
    {
      if (compareFct(this.values[i], this.values[i + 1]))
      {
        var tmp = this.values[i];
        this.values[i] = this.values[i + 1];
        this.values[i + 1] = tmp;
        if (i != 0)
          --i;
      }
      else
        ++i;
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void empty() => this.count = 0;
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool isFull() => this.count == this.reserved;

  public override string ToString()
  {
    string result = "[" + this.count + "]";
    for (int i = 0; this.count > i; ++i)
    {
      if (i != 0)
        result += "," + this.values[i];
      else
        result += this.values[i];
    }
    return result;
  }

  public T[] ToArray()
  {
    T[] result = new T[this.count];
    for (int i = 0; this.count > i; ++i)
      result[i] = this.values[i];
    return result;
  }
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public T[] toArray() => this.ToArray();
}
