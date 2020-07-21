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
                    for (int i = 0; i < length; ++i)
                    {
                        text += (char)ran.Next(33, 127);
                    }
                    Console.WriteLine(text);
                    var badges = new Encoder.Encoder().Encode(EncodeType.Broadcast, Protocol.ASCII, text);
                    var result = new Encoder.Encoder().Decode(badges);

                    result = result.Substring(0, length);
                    Assert.AreEqual(text, result);
                }
            }
        }

        [TestMethod]
        public void TestAsciiUnicast()
        {
            var ran = new Random();
            for (int length = 1; length <= 6; ++length)
            {
                for (int attempts = 0; attempts < 50000; ++attempts)
                {
                    var text = string.Empty;
                    for (int i = 0; i < length; ++i)
                    {
                        text += (char)ran.Next(33, 127);
                    }
                    Console.WriteLine(text);
                    var badges = new Encoder.Encoder().Encode(EncodeType.Unicast, Protocol.ASCII, text, "Aurehen#1115");
                    var result = new Encoder.Encoder().Decode(badges, "Aurehen#1115");

                    result = result.Substring(0, length);
                    Assert.AreEqual(text, result);
                }
            }
        }

        [TestMethod]
        public void TestAsciiUnicast_MismatchId_ShouldFail()
        {
            var ran = new Random();
            for (int length = 1; length <= 5; ++length)
            {
                for (int attempts = 0; attempts < 50000; ++attempts)
                {
                    var text = string.Empty;
                    for (int i = 0; i < length; ++i)
                    {
                        text += (char)ran.Next(33, 127);
                    }
                    Console.WriteLine(text);
                    var badges = new Encoder.Encoder().Encode(EncodeType.Unicast, Protocol.ASCII, text, "Aurehen#1115");
                    try
                    {
                        var result = new Encoder.Encoder().Decode(badges, "Aurehen#1116");
                    }
                    catch (Exception e)
                    {
                        Assert.IsTrue(e.Message.Contains(@"mismatch"));
                        return;
                    }

                    Assert.Fail(@"User should not receive this cast.");
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

        [TestMethod]
        public void TestUnicodeUniCast()
        {
            var ranges = new[]
            {
                new Range(0x4e00, 0x4f80),
                new Range(0x5000, 0x9fa0),
                new Range(0x3400, 0x4db0),
                new Range(0x30a0, 0x30f0)
            };

            for (int length = 1; length <= 2; ++length)
            {
                for (int attempts = 0; attempts < 50000; ++attempts)
                {
                    var text = GenerateString(length, ranges);
                    var receiver = GenerateString(8, ranges);
                    var badges = new Encoder.Encoder().Encode(EncodeType.Unicast, Protocol.UNICODE, text, receiver);
                    var result = new Encoder.Encoder().Decode(badges, receiver);

                    result = result.Substring(0, length);
                    Assert.AreEqual(text, result);
                }
            }
        }

        [TestMethod]
        public void TestUnicodeUniCast_MismatchId_ShouldFail()
        {
            var ranges = new[]
            {
                new Range(0x4e00, 0x4f80),
                new Range(0x5000, 0x9fa0),
                new Range(0x3400, 0x4db0),
                new Range(0x30a0, 0x30f0)
            };

            var ran = new Random();
            for (int length = 1; length <= 2; ++length)
            {
                for (int attempts = 0; attempts < 50000; ++attempts)
                {
                    var text = GenerateString(length, ranges);
                    var badges = new Encoder.Encoder().Encode(EncodeType.Unicast, Protocol.UNICODE, text, "Aurehen#1115");
                    try
                    {
                        var result = new Encoder.Encoder().Decode(badges, "Aurehen#1116");
                    }
                    catch (Exception e)
                    {
                        Assert.IsTrue(e.Message.Contains(@"mismatch"));
                        return;
                    }

                    Assert.Fail(@"User should not receive this cast.");
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
