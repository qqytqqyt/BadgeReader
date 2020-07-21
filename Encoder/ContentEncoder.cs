using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Linq;

namespace Encoder
{
    public enum Protocol
    {
        UNICODE = 0,
        ASCII = 1,
        ALPHABET = 2,
        PSRT = 3,
        IM = 4,
        WORDS = 5
    }

    public class ContentEncoder
    {
        public static List<char> DecodeContent(int protocolType, BigInteger contentInt)
        {
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

                    if (byteList.Count % 2 == 1)
                        byteList.Add(0);

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

            return chars;
        }

        public static BigInteger EncodeContent(Protocol protocol, string content, int maxDataBits)
        {
            BigInteger contentInt = 0;
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

            return contentInt;
        }
    }
}
