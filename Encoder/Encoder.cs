using BadgeReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Text;

namespace Encoder
{
    public enum EncodeType
    {
        Broadcast = 0,
        Unicast = 1,
    }

    public enum Protocol
    {
        UNICODE = 0,
        ASCII = 1,
        ALPHABET = 2,
        PSRT = 3,
        IM = 4,
        WORDS = 5
    }

    public class Encoder
    {
        public string Decode(List<Badge> badges)
        {
            var e2HttpProtocol = new E2HttpProtocol();
            var finalText = string.Empty;

            var resultInt = new List<int>();
            for (int i = 0; i < 10; ++i)
            {
                var panelPos = e2HttpProtocol.PanelPositions[i];

                var selectedBadges = new List<Badge>();
                foreach (var badge in badges)
                {
                    if (badge.Position.X >= panelPos.X && badge.Position.X <= panelPos.X + 4 && badge.Position.Y >= panelPos.Y && badge.Position.Y <= panelPos.Y + 10)
                    {
                        var x = badge.Position.X - panelPos.X;
                        var y = badge.Position.Y - panelPos.Y;
                        var selectedBadge = new Badge();
                        selectedBadge.Position = new Position(x, y);
                        selectedBadge.BadgeType = badge.BadgeType;
                        selectedBadges.Add(selectedBadge);
                    }
                }


                BadgeElement selectedBadgeElement = null;
                foreach (var badgeElement in e2HttpProtocol.BadgeElements)
                {
                    if (badgeElement.AllocatedBadges.Count != selectedBadges.Count)
                        continue;

                    bool found = true;
                    foreach (var selectedBadge in selectedBadges)
                    {
                        if (!badgeElement.AllocatedBadges.Any(s =>
                            s.Position.X == selectedBadge.Position.X && s.Position.Y == selectedBadge.Position.Y &&
                            s.BadgeType == selectedBadge.BadgeType))
                        {
                            found = false;
                            break;
                        }
                    }

                    if (found)
                    {
                        selectedBadgeElement = badgeElement;
                        break;
                    }
                }

                var index = e2HttpProtocol.BadgeElements.ToList().IndexOf(selectedBadgeElement);
                resultInt.Add(index);
            }

            BigInteger finalInt = 0;

            for (int i = 0; i < 10; ++i)
            {
                finalInt += resultInt[i] * BigInteger.Pow(54, 10 - i - 1);
            }

            finalText += finalInt.ToBinaryString();
            // bit 22 - 56 -> Body text
            BigInteger contentInt = finalInt & 0x7FFFFFFFF;
            finalInt >>= 35;

            // bit 19 - 21 -> CheckSum
            int checkSum = (int)(finalInt & 0b111);
            finalInt >>= 3;

            // bit 14 - 18 -> Protocol type
            int protocolType = (int)(finalInt & 0b11111);
            finalInt >>= 5;

            // bit 1 - 13 -> ID / Extended body text
            var extendedData = finalInt & 0x1FFF;
            finalInt >>= 13;

            // bit 0 -> type
            int encodeType = (int)finalInt;

            EncodeType encode = (EncodeType)encodeType;
            if (encode == EncodeType.Broadcast)
            {
                extendedData <<= 35;
                contentInt = extendedData | contentInt;
            }
            else if (encode == EncodeType.Unicast)
            {
                // TODO
            }

            if (checkSum != CheckSum(contentInt))
                throw new Exception(@"Check sum failure");


            Protocol protocol = (Protocol)protocolType;
            List<char> chars = new List<char>();
            switch (protocol)
            {
                case Protocol.UNICODE:
                    var byteList = new List<byte>();
                    while (contentInt > 0)
                    {
                        byte code = (byte)(contentInt & 0xFF);
                        byteList.Add(code);
                        contentInt >>= 8;
                    }

                    byteList.Reverse();
                    chars = Encoding.Unicode.GetChars(byteList.ToArray()).ToList();

                    break;
                case Protocol.ASCII:
                    var byteListAscii = new List<byte>();
                    while (contentInt > 0)
                    {
                        byte code = (byte)(contentInt & 0x7F);
                        byteListAscii.Add(code);
                        contentInt >>= 7;
                    }
                    byteListAscii.Reverse();

                    chars = Encoding.ASCII.GetChars(byteListAscii.ToArray()).ToList();

                    break;
                default:
                    break;
            }


            while (finalText.Length < 57)
            {
                finalText = @"0" + finalText;
            }

            finalText = new string(chars.ToArray()) + @" (" + finalText + ")";
            Console.WriteLine(finalInt);

            return finalText;
        }

        public List<int> Encode(EncodeType encodeType, Protocol protocol, string content)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            BigInteger bigInt = 0;
            int maxDataBits = 35;

            // bit 0 -> type
            if (encodeType == EncodeType.Unicast)
            {
                bigInt |= 1;
            }
            else
            {
                maxDataBits += 13;
            }

            BigInteger contentInt = 0;
            // encode content 
            switch (protocol)
            {
                case Protocol.UNICODE:
                    byte[] bytesUnicode = Encoding.Unicode.GetBytes(content);
                    if (bytesUnicode.Length * 8 > maxDataBits)
                        throw new Exception(@"Content too long to fit!");

                    for (int i = 0; i < bytesUnicode.Length; ++i)
                    {
                        contentInt <<= 8;
                        contentInt |= bytesUnicode[i];
                    }

                    break;
                case Protocol.ASCII:
                    byte[] bytesAscII = Encoding.ASCII.GetBytes(content);
                    if (bytesAscII.Length * 7 > maxDataBits)
                        throw new Exception(@"Content too long to fit!");

                    for (int i = 0; i < bytesAscII.Length; i++)
                    {
                        contentInt <<= 7;
                        contentInt |= bytesAscII[i] & 0x7F;
                    }

                    break;
                default:
                    break;
            }

            // bit 1 - 13 -> ID / Extended body text
            bigInt <<= 13;

            if (encodeType == EncodeType.Unicast)
            {
                // TODO
            }
            else if (encodeType == EncodeType.Broadcast)
            {
                var extendedContent = contentInt >> 35;
                bigInt |= extendedContent;
            }

            Console.WriteLine(bigInt.ToBinaryString());
            // bit 14 - 18 -> Protocol type
            bigInt <<= 5;
            bigInt |= (int)protocol;

            // bit 19 - 21 -> CheckSum
            bigInt <<= 3;
            bigInt |= CheckSum(contentInt);

            // bit 22 - 56 -> Body text
            bigInt <<= 35;
            switch (protocol)
            {
                case Protocol.UNICODE:
                case Protocol.ASCII:
                case Protocol.ALPHABET:
                    Console.WriteLine(contentInt.ToBinaryString());
                    var contentLeftOver = contentInt & 0x7FFFFFFFF;
                    Console.WriteLine(contentLeftOver.ToBinaryString());
                    Console.WriteLine();
                    bigInt |= contentLeftOver;
                    break;
                default:
                    break;
            }

            var results = bigInt.ToBaseX(54);
            while (results.Count < 10)
            {
                results.Insert(0, 0);
            }

            Console.WriteLine(string.Join(@" ", results));
            Console.WriteLine(bigInt);
            Console.WriteLine(bigInt.ToBinaryString());
            Console.Write(contentInt.ToBinaryString());

            return results;
        }

        private static int CheckSum(BigInteger number)
        {
            var leftOver = number;
            var sum = 0;
            for (int i = 0; i < 64; i += 4)
            {
                sum += (int)(leftOver & 0x0F);
                leftOver >>= 4;
            }

            return sum % 8;
        }

        private static int GetBitcount(BigInteger n)
        {
            var test = n;
            var count = 0;

            while (test != 0)
            {
                if ((test & 1) == 1)
                {
                    count++;
                }
                test >>= 1;
            }

            return count;
        }

    }
}
