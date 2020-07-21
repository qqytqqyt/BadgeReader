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
        public void TestUnicode()
        {
            for (int i = 33; i < 128; ++i)
            {
                for (int j = 33; j < 128; ++j)
                {
                    for (int k = 33; k < 128; ++k)
                    {
                        var text = @"" + (char) i + (char) j + (char) k;
                        Console.WriteLine(text);
                        var badges = new Encoder.Encoder().Encode(EncodeType.Broadcast, Protocol.ASCII, text);
                        var result = new Encoder.Encoder().Decode(badges);

                        result = result.Substring(0, 3);
                        Assert.AreEqual(text, result);
                    }
                }
            }


        }
    }
}
