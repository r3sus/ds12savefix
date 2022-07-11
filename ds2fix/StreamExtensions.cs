﻿using System;
using System.IO;
using System.Text;

namespace DeadSpace2SaveEditor.Code
{
    public static class StreamExtensions
    {
        #region Read extensions

        public static int ReadInt32(this Stream stream, bool BE)
        {
            var buffer = new byte[4]; 
            stream.Read(buffer, 0, buffer.Length); if (BE) Array.Reverse(buffer);
            return BitConverter.ToInt32(buffer, 0); 
        }

        public static uint ReadUInt32(this Stream stream, bool BE)
        {
            var buffer = new byte[4]; 
            stream.Read(buffer, 0, buffer.Length); if (BE) Array.Reverse(buffer);
            return BitConverter.ToUInt32(buffer, 0);
        }

        public static short ReadInt16(this Stream stream)
        {
            var buffer = new byte[2];
            stream.Read(buffer, 0, buffer.Length);
            return BitConverter.ToInt16(buffer, 0);
        }

        public static ushort ReadUInt16(this Stream stream)
        {
            var buffer = new byte[2];
            stream.Read(buffer, 0, buffer.Length);
            return BitConverter.ToUInt16(buffer, 0);
        }

        public static float ReadFloat(this Stream stream)
        {
            var buffer = new byte[4];
            stream.Read(buffer, 0, buffer.Length);
            return BitConverter.ToSingle(buffer, 0);
        }

        public static bool ReadBool(this Stream stream)
        {
            return Convert.ToBoolean(stream.ReadByte());
        }

        public static Guid ReadGuid(this Stream stream)
        {
            var buffer = new byte[16];
            stream.Read(buffer, 0, buffer.Length);
            return new Guid(buffer);
        }

        public static string ReadASCIIString(this Stream stream)
        {
            var buffer = new byte[1];
            var str = string.Empty;
            stream.Read(buffer, 0, buffer.Length);
            while (buffer[0] > 20)
            {
                str += Encoding.ASCII.GetString(buffer);
                stream.Read(buffer, 0, buffer.Length);
            }
            return str;
        }

        public static string ReadUnicodeString(this Stream stream)
        {
            var buffer = new byte[2];
            var str = string.Empty;
            stream.Read(buffer, 0, buffer.Length);
            while (buffer[0] != 0 || buffer[1] != 0)
            {
                str += Encoding.Unicode.GetString(buffer);
                stream.Read(buffer, 0, buffer.Length);
            }
            return str;
        }
        #endregion

        #region Write extensions
        public static void WriteInt32(this Stream stream, int value, bool BE)
        {
            var out1 = BitConverter.GetBytes(value);
            if (BE) Array.Reverse(out1);
            stream.Write(out1, 0, 4);
        }
        public static void WriteUInt32(this Stream stream, uint value, bool BE)
        {
            var out1 = BitConverter.GetBytes(value);
            if (BE) Array.Reverse(out1);
            stream.Write(out1, 0, 4);
        }

        public static void WriteInt16(this Stream stream, short value)
        {
            stream.Write(BitConverter.GetBytes(value), 0, 2);
        }

        public static void WriteUInt16(this Stream stream, ushort value)
        {
            stream.Write(BitConverter.GetBytes(value), 0, 2);
        }

        public static void WriteBool(this Stream stream, bool value)
        {
            stream.Write(BitConverter.GetBytes(value), 0, 1);
        }

        public static void WriteFloat(this Stream stream, float value)
        {
            stream.Write(BitConverter.GetBytes(value), 0, 4);
        }

        public static void WriteGuid(this Stream stream, Guid value)
        {
            stream.Write(value.ToByteArray(), 0, 16);
        }
        #endregion

        public enum SearchOrigin
        {
            Begin,
            Current
        }

        public static long SearchForBytePattern(this Stream stream, byte[] pattern, SearchOrigin searchOrigin = SearchOrigin.Begin)
        {
            var initPos = stream.Position;
            if (searchOrigin == SearchOrigin.Begin)
            {
                stream.Position = 0;
            }

            int patternLength = pattern.Length;
            byte[] currentByte = new byte[1];
            long startPos = 0;
            int matchPos = 0;

            while (stream.Read(currentByte, 0, 1) != 0)
            {
                if (currentByte[0] == pattern[matchPos])
                {
                    if (matchPos == 0)
                    {
                        startPos = stream.Position;
                    }
                    if (matchPos >= patternLength - 1)
                    {
                        var result = stream.Position - patternLength;
                        stream.Position = initPos;
                        return result;
                    }
                    matchPos++;
                }
                else
                {
                    if (matchPos > 0)
                    {
                        stream.Position = startPos;
                    }
                    matchPos = 0;
                }
            }

            stream.Position = initPos;
            return -1;
        }

    }
}
