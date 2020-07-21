using System;
using System.Collections.Generic;
using System.Text;
using Encoder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BadgeReader.Tests
{
    [TestClass]
    public class EncoderTest
    {
        [TestMethod]
        public void TestAsciiBroadcast()
        {
            var ran = new Random();
            for (int length = 1; length <= 6; ++length)
            {
                for (int attempts = 0; attempts < 50000; ++attempts)
                {
                    var text = string.Empty;
                    for (int i = 0; i < 3; ++i)
                    {
                        text += (char)ran.Next(33, 127);
                    }
                    Console.WriteLine(text);
                    var badges = new Encoder.Encoder().Encode(EncodeType.Broadcast, Protocol.ASCII, text);
                    var result = new Encoder.Encoder().Decode(badges);

                    result = result.Substring(0, 3);
                    Assert.AreEqual(text, result);
                }
            }
        }


        [TestMethod]
        public void TestUnicodeBroadCast()
        {
            var ranges = new[]
            {
                new Range(0x4e00, 0x4f80),
                new Range(0x5000, 0x9fa0),
                new Range(0x3400, 0x4db0),
                new Range(0x30a0, 0x30f0)
            };

            var ran = new Random();
            for (int length = 1; length <= 3; ++length)
            {
                for (int attempts = 0; attempts < 50000; ++attempts)
                {
                    var text = GenerateString(length, ranges);
                    var badges = new Encoder.Encoder().Encode(EncodeType.Broadcast, Protocol.UNICODE, text);
                    var result = new Encoder.Encoder().Decode(badges);

                    result = result.Substring(0, length);
                    Assert.AreEqual(text, result);
                }
            }
        }

        private static string GenerateString(int length, IList<Range> ranges)
        {
            var builder = new StringBuilder(length);
            var random = new Random();
            for (var i = 0; i < length; i++)
            {
                var rangeIndex = random.Next(ranges.Count);
                var range = ranges[rangeIndex];
                builder.Append((char)random.Next(range.Begin, range.End));
            }
            return builder.ToString();
        }

        private class Range
        {

            public int Begin { get; set; }

            public int End { get; set; }

            public Range(int begin, int end)
            {
                Begin = begin;
                End = end;
            }

        }
    }
}
