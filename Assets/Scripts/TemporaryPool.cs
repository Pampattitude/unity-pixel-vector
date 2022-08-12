public struct TemporaryPool<T>
{
  public const int POOL_COUNT = 256; // i.e. 256^sizeof(byte)

  private static T[] array_ = new T[TemporaryPool<T>.POOL_COUNT];
  private static byte currentIndex_ = 0;

  [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
  public static T next() => TemporaryPool<T>.array_[TemporaryPool<T>.currentIndex_++];
}
