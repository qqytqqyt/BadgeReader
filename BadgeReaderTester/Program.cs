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
            var badges = new Encoder.Encoder().Encode(EncodeType.Broadcast, Protocol.ASCII, @"abded");
            
            var result = new Encoder.Encoder().Decode(badges);

            var map = new Map();
            using (var img = new ImageProducer().ProduceImage(badges, map.MapMatrix))
                img.Save(PosRetriever.DebugDir + @"output2.jpg");

            var dir = @"C:\Users\qqytqqyt\source\repos\BadgeReader\BadgeReader.Tests\Resources\";
            var fileInfo = new FileInfo(dir + "test16.jpg");
            var posRetriever = new PosRetriever();
            PosRetriever.Debug = true;
            using (var croppedImg = posRetriever.RetrievePanel(fileInfo.FullName))
            {

                var dots = posRetriever.PrintDots(croppedImg, map.MapMatrix);
                var results = PosRetriever.ReadDots(dots);

                using (var img = new ImageProducer().ProduceImage(results, map.MapMatrix))
                        img.Save(PosRetriever.DebugDir + @"output.jpg");

                File.WriteAllText(@"C:\Users\qqytqqyt\OneDrive\Documents\OneDrive\OwnProjects\Combination\freetest\" + fileInfo.Name + ".json", @JsonConvert.SerializeObject(results));
            }

            Console.WriteLine("Hello World!");
        }
    }
}
