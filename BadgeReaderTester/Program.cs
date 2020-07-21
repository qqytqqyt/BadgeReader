using System;
using System.Collections.Generic;
using System.IO;
using BadgeReader;
using Encoder;
using Newtonsoft.Json;

namespace BadgeReaderTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var resultInt = new Encoder.Encoder().Encode(EncodeType.Broadcast, Protocol.ASCII, @"abded");

            var badges = new List<Badge>();
            var e2HttpProtocol = new E2HttpProtocol();
            for (int i = 0; i < 10; ++i)
            {
                var panelPos = e2HttpProtocol.PanelPositions[i];
                foreach (var badgeElement in e2HttpProtocol.BadgeElements[resultInt[i]].AllocatedBadges)
                {
                    var badge = new Badge();
                    badge.Position = new Position(panelPos.X + badgeElement.Position.X, panelPos.Y + badgeElement.Position.Y);
                    badge.BadgeType = badgeElement.BadgeType;
                    badges.Add(badge);
                }
            }

            var result = new Encoder.Encoder().Decode(badges);

            using (var img = new ImageProducer().ProduceImage(badges))
                img.Save(PosRetriever.DebugDir + @"output2.jpg");

            var dir = @"C:\Users\qqytqqyt\source\repos\BadgeReader\BadgeReader.Tests\Resources\";
            var fileInfo = new FileInfo(dir + "test16.jpg");
            var posRetriever = new PosRetriever();
            PosRetriever.Debug = true;
            using (var croppedImg = posRetriever.RetrievePanel(fileInfo.FullName))
            {
                var map = new Map();

                var dots = posRetriever.PrintDots(croppedImg, map.MapMatrix);
                var results = PosRetriever.ReadDots(dots);

                using (var img = new ImageProducer().ProduceImage(results))
                        img.Save(PosRetriever.DebugDir + @"output.jpg");

                File.WriteAllText(@"C:\Users\qqytqqyt\OneDrive\Documents\OneDrive\OwnProjects\Combination\freetest\" + fileInfo.Name + ".json", @JsonConvert.SerializeObject(results));
            }

            Console.WriteLine("Hello World!");
        }
    }
}
