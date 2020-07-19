using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace BadgeReader.Tests
{
    [TestClass]
    public class BadgeReaderTest
    {
        [TestMethod]
        public void TestResources()
        {

            var dir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\Resources\"));
            for (int i = 0; i < 99; ++i)
            {
                var fileInfo = new FileInfo(dir + $"test{i}.jpg");
                if (!fileInfo.Exists)
                    continue;
                
                var posRetriever = new PosRetriever();
                using (var croppedImg = posRetriever.RetrieveFreePositions(fileInfo.FullName))
                {
                    var map = new Map();

                    var dots = posRetriever.PrintDots(croppedImg, map.MapMatrix);
                    var results = posRetriever.ReadDots(dots);

                    Assert.AreEqual(JsonConvert.SerializeObject(results), File.ReadAllText(dir + $"test{i}.jpg.json"));
                }
            }
        }
    }
}
