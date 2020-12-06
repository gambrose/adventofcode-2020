using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AdventOfCode2020
{
    public static class ReadOnlyMemoryExtensions
    {
        public static IEnumerable<ReadOnlyMemory<T>> Split<T>(this ReadOnlyMemory<T> memory, T separator) where T : IEquatable<T>
        {
            var start = 0;
            var length = 0;

            foreach (var line in memory)
            {
                if (separator.Equals(line))
                {
                    if (length > 0)
                    {
                        yield return memory.Slice(start, length);
                        start += length + 1;
                        length = 0;
                        continue;
                    }
                }

                length++;
            }

            if (length > 0)
            {
                yield return memory.Slice(start, length);
            }
        }

        public static Enumerator<T> GetEnumerator<T>(this ReadOnlyMemory<T> memory) => new Enumerator<T>(memory);

        public struct Enumerator<T>
        {
            private readonly ReadOnlyMemory<T> _memory;
            private int _index;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Enumerator(ReadOnlyMemory<T> memory)
            {
                _memory = memory;
                _index = -1;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                var index = _index + 1;
                if (index < _memory.Length)
                {
                    _index = index;
                    return true;
                }

                return false;
            }

            public T Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _memory.Span[_index];
            }
        }
    }
}