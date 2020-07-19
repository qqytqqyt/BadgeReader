using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                {
                    fileInfo = new FileInfo(dir + $"test{i}.png");
                    if (!fileInfo.Exists)
                        continue;
                }
                
                var posRetriever = new PosRetriever();
                using (var croppedImg = posRetriever.RetrievePanel(fileInfo.FullName))
                {
                    var map = new Map();

                    var dots = posRetriever.PrintDots(croppedImg, map.MapMatrix);
                    var results = posRetriever.ReadDots(dots);

                    results = results.OrderBy(r => r.Position.X).ThenBy(r => r.Position.Y).ToList();
                    var expectedResults = JsonConvert.DeserializeObject<List<Badge>>(File.ReadAllText(dir + fileInfo.Name + ".json")).OrderBy(r => r.Position.X).ThenBy(r => r.Position.Y).ToList();

                    Assert.AreEqual(JsonConvert.SerializeObject(expectedResults), JsonConvert.SerializeObject(results), fileInfo.Name);
                }
            }
        }
    }
}
