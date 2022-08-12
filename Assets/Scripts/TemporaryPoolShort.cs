public struct TemporaryPoolShort<T>
{
  public const int POOL_COUNT = 65536; // i.e. 256^sizeof(ushort)

  private static T[] array_ = new T[TemporaryPoolShort<T>.POOL_COUNT];
  private static ushort currentIndex_ = 0;

  [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
  public static T next() => TemporaryPoolShort<T>.array_[TemporaryPoolShort<T>.currentIndex_++];
}
