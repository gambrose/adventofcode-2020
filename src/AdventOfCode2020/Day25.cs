#nullable  enable
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xunit;

namespace AdventOfCode2020
{
    public class Day25
    {
        [Fact]
        public void Part_1_example() => Assert.Equal(14897079, Part1(Example));

        [Fact]
        public void Part_1() => Assert.Equal(3803729, Part1(Input));

        [Fact]
        public void Part_2_example() => Assert.Equal(default, Part2(Example));

        [Fact]
        public void Part_2() => Assert.Equal(default, Part2(Input));

        private static long Part1(ReadOnlyMemory<string> input)
        {
            static int LoopSize(long publicKey)
            {
                long value = 1;
                const long subject = 7;
                var loopSize = 0;

                while (value != publicKey)
                {
                    loopSize++;

                    value *= subject;
                    value = value % 20201227;
                }

                return loopSize;
            }

            static long EncryptionKey(long subject, int loopSize)
            {
                long value = 1;

                for (int i = 0; i < loopSize; i++)
                {
                    value *= subject;
                    value = value % 20201227;
                }

                return value;
            }

            var cardPublicKey = long.Parse(input.Span[0]);
            var doorPublicKey = long.Parse(input.Span[1]);

            var cardLoopSize = LoopSize(cardPublicKey);
            var doorLoopSize = LoopSize(doorPublicKey);

            Debug.Assert(cardLoopSize != doorLoopSize);

            var encryptionKey = EncryptionKey(cardPublicKey, doorLoopSize);

            Debug.Assert(encryptionKey == EncryptionKey(doorPublicKey, cardLoopSize));

            return encryptionKey;
        }

        private static long Part2(ReadOnlyMemory<string> input)
        {
            return default;
        }

        private static ReadOnlyMemory<string> Example { get; } = @"5764801
17807724".SplitLines();

        private static ReadOnlyMemory<string> Input { get; } = File.ReadLines("Day25.input.txt").ToArray();
    }
}