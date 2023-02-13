## Memory allocation

### ヒープ

Large Object Heap (LOH): 85000バイト以上のオブジェクト

ガベージコレクションでメモリーのコンパクションが発生すると、オブジェクトの移動に時間がかかり、パフォーマンスが低下する。これを避けるために、LOHに関してはメモリーの移動を行わない。GCパフォーマンスの低下は防げるが、逆にメモリーが断片化して再利用が難しくなる。.NET 4.5以上では改善したらしいが、LOHを避けるに越したことはない。



### stackalloc

スタックに確保する。当然、扱いはスタック上に制約される。上限1MBくらいだが、1KiB程に押さえた方が良い。

newより早いが、差はわずか（32Bまででnewと比較して20%早い、それ以上だと差は少なくなる）。



### ArrayPool

`ArrayPool<T>.Shared.Rent`

共有のメモリープール。メモリー確保は固定時間。

Returnする必要がある。大きいサイズ（512B 以上）の場合は、newよりパフォーマンス良い。



### new

good old fashioned.

ガベージコレクションの管理下に置かれ、使い勝手は最も良い。無理にstackalloc/ArrayPoolを使う必要はないが、大きいサイズ（512B 以上）ではArrayPoolに明確に劣る。特にLarge Object Heap (LOH 8500バイト以上)では、ArrayPoolを使用する。



### 結論

32B 以下：stackallocがnewと比較して20%早い。パフォーマンス最重視の場合はstackallocを考慮する。

512B 以下：素直にnewを使用する。

512B 以上：ArrayPoolの領域。パフォーマンスが重要でない場合はnewも許容される。

85KB 以上：LOHを避けるため、ArrayPoolを使用する。



## IDisposable

```csharp
using var a = new class();
var b = new class();
b.Dispose();
using (var c = new class()) {}
// same performance.
```



## [SkipLocalsInit]

not working at this time.



## Atomic operations

Copy: 1 ns

Volatile.Write: 1.5 ns

Interlocked.Exchange: 5 ns

Interlocked.Increment: 5 ns

lock: 16 ns

ConcurrentQueue.TryDequeue: 10 ns (approx.)

ConcurrentQueue Enqueue/Dequeue: 16 ns

ConcurrentStack Push/Pop: 20 ns



## ByteCopy

`Array.Copy` が十分速い。

256 bytes以下では `Buffer.MemoryCopy` や `UnsafeLong` が速いが、それ以上は `Array.Copy` と同等。

基本的に `Array.Copy` を使う。



## Timer

GetTimestamp: 11 ns

TimeBegin/EndPeriod: 5.5 us



## Task

`ValueTask` is slightly slower than `Task`.

The win is the fact the `ValueTask` doesn't allocate anything.



## Mutual exclusion
Create object: 2ns, 24B
Create Semaphore: 490ns
Create SemaphoreSlim: 7ns, 88B

Lock: 14ns
Monitor Enter/Exit: 15ns
Semaphore Wait/Release: 367ns
SemaphoreSlim Wait/Release: 32ns
SemaphoreSlim WaitAsync/Release: 43ns

