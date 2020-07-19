﻿using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using BadgeReader;
using Newtonsoft.Json;

namespace BadgeReaderTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var dir = @"C:\Users\qqytqqyt\source\repos\BadgeReader\BadgeReader.Tests\Resources\";
            var fileInfo = new FileInfo(dir + "test14.png");
            var posRetriever = new PosRetriever() {Debug = true};
            using (var croppedImg = posRetriever.RetrievePanel(fileInfo.FullName))
            {
                var map = new Map();

                var dots = posRetriever.PrintDots(croppedImg, map.MapMatrix);
                var results = posRetriever.ReadDots(dots);

                File.WriteAllText(@"C:\Users\qqytqqyt\OneDrive\Documents\OneDrive\OwnProjects\Combination\freetest\" + fileInfo.Name + ".json", @JsonConvert.SerializeObject(results));
            }

            Console.WriteLine("Hello World!");
        }
    }
}
