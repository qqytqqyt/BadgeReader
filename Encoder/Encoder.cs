using System;
using System.Collections.Generic;
using System.Numerics;
using BadgeReader;

namespace Encoder
{
    public enum EncodeType
    {
        Broadcast = 0,
        Unicast = 1
    }

    public class Encoder
    {
        /// <summary>
        ///     Decode badges to content
        /// </summary>
        /// <param name="badges"></param>
        /// <param name="receiver"></param>
        /// <returns></returns>
        public string Decode(List<Badge> badges, string receiver = "")
        {
            var finalText = string.Empty;

            // physical
            var finalInt = E2HttpProtocol.DecodeBadges(badges);

            // visualization purpose only
            finalText += finalInt.ToBinaryString();

            // transaction
            var contentInt = ReadTransactionCode(finalInt, receiver, out var protocolType);

            // application
            var chars = ContentEncoder.DecodeContent(protocolType, contentInt);

            // visualization purpose only
            while (finalText.Length < 57) finalText = @"0" + finalText;

            finalText = new string(chars.ToArray()) + @" (" + finalText + ")";

            return finalText;
        }

        private static BigInteger ReadTransactionCode(BigInteger finalInt, string receiver, out int protocolType)
        {
            // bit 22 - 56 -> Body text
            var contentInt = finalInt & 0x7FFFFFFFF;
            finalInt >>= 35;

            // bit 19 - 21 -> CheckSum
            var checkSum = (int) (finalInt & 0b111);
            finalInt >>= 3;

            // bit 14 - 18 -> Protocol type
            protocolType = (int) (finalInt & 0b11111);
            finalInt >>= 5;

            // bit 1 - 13 -> ID / Extended body text
            var extendedData = finalInt & 0x1FFF;
            
            finalInt >>= 13;

            // bit 0 -> type
            var encodeType = (int) finalInt;

            var encode = (EncodeType) encodeType;
            if (encode == EncodeType.Broadcast)
            {
                extendedData <<= 35;
                contentInt = extendedData | contentInt;
            }
            else if (encode == EncodeType.Unicast)
            {
                var receiverCode = ContentEncoder.EncodeContent(Protocol.UNICODE, receiver, int.MaxValue) % 0x1FFF;
                if (receiverCode != extendedData || string.IsNullOrEmpty(receiver))
                    throw new Exception(@"Receiver mismatched");
            }

            if (checkSum != CheckSum(contentInt))
                throw new Exception(@"Check sum failure");

            return contentInt;
        }

        /// <summary>
        ///     Encode content to badges
        /// </summary>
        /// <param name="encodeType"></param>
        /// <param name="protocol"></param>
        /// <param name="content"></param>
        /// <param name="receiver"></param>
        /// <returns></returns>
        public List<Badge> Encode(EncodeType encodeType, Protocol protocol, string content, string receiver = "")
        {
            BigInteger bigInt = 0;
            var maxDataBits = 35;

            // bit 0 -> type
            if (encodeType == EncodeType.Unicast)
                bigInt |= 1;
            else
                maxDataBits += 13;

            // encode content (application layer)
            var contentInt = ContentEncoder.EncodeContent(protocol, content, maxDataBits);

            // transaction
            var transactionInt = GenerateTransactionCode(encodeType, protocol, bigInt, contentInt, receiver);

            // phyiscal
            var badges = E2HttpProtocol.GenerateBadges(transactionInt);

            return badges;
        }

        private static BigInteger GenerateTransactionCode(EncodeType encodeType, Protocol protocol, BigInteger bigInt,
            BigInteger contentInt, string receiver)
        {
            // bit 1 - 13 -> ID / Extended body text
            bigInt <<= 13;

            if (encodeType == EncodeType.Unicast)
            {
                var receiverInt = ContentEncoder.EncodeContent(Protocol.UNICODE, receiver, int.MaxValue);
                bigInt |= receiverInt % 0x1FFF;
            }
            else if (encodeType == EncodeType.Broadcast)
            {
                var extendedContent = contentInt >> 35;
                bigInt |= extendedContent;
            }

            Console.WriteLine(bigInt.ToBinaryString());
            // bit 14 - 18 -> Protocol type
            bigInt <<= 5;
            bigInt |= (int) protocol;

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
            }

            var transactionInt = bigInt;
            return transactionInt;
        }

        private static int CheckSum(BigInteger number)
        {
            var leftOver = number;
            var sum = 0;
            for (var i = 0; i < 64; i += 4)
            {
                sum += (int) (leftOver & 0x0F);
                leftOver >>= 4;
            }

            return sum % 8;
        }
    }
}