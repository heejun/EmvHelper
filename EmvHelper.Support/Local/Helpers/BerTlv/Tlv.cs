﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// https://github.com/kspearrin/BerTlv.NET
namespace EmvHelper.Support.Local.Helpers.BerTlv
{
    /// <summary>
    /// TLV data.
    /// </summary>
    public class Tlv
    {
        private readonly int _valueOffset;

        private Tlv(int tag, int length, int valueOffset, byte[] data)
        {
            Tag = tag;
            Length = length;
            Data = data;
            Children = new List<Tlv>();

            _valueOffset = valueOffset;
        }

        /// <summary>
        /// The raw TLV data.
        /// </summary>
        public byte[] Data { get; private set; }

        /// <summary>
        /// The raw TLV data.
        /// </summary>
        public string HexData { get { return GetHexString(Data); } }

        /// <summary>
        /// The TLV tag.
        /// </summary>
        public int Tag { get; private set; }

        /// <summary>
        /// The TLV tag.
        /// </summary>
        public string HexTag { get { return Tag.ToString("X"); } }

        /// <summary>
        /// The length of the TLV value.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// The length of the TLV value.
        /// </summary>
        public string HexLength { get { return Length.ToString("X"); } }

        /// <summary>
        /// The TLV value.
        /// </summary>
        public byte[] Value
        {
            get
            {
                byte[] result = new byte[Length];
                Array.Copy(Data, _valueOffset, result, 0, Length);
                return result;
            }
        }

        /// <summary>
        /// The TLV value.
        /// </summary>
        public string HexValue { get { return GetHexString(Value); } }

        /// <summary>
        /// TLV children.
        /// </summary>
        public ICollection<Tlv> Children { get; set; }

        /// <summary>
        /// Parse TLV data.
        /// </summary>
        /// <param name="tlv">The hex TLV blob.</param>
        /// <returns>A collection of TLVs.</returns>
        public static ICollection<Tlv> Parse(string tlv)
        {
            return ParseTlv(tlv);
        }

        /// <summary>
        /// Parse TLV data.
        /// </summary>
        /// <param name="tlv">The byte array TLV blob.</param>
        /// <returns>A collection of TLVs.</returns>
        public static ICollection<Tlv> Parse(byte[] rawTlv)
        {
            return ParseTlv(rawTlv);
        }

        /// <summary>
        /// Parse TLV data.
        /// </summary>
        /// <param name="tlv">The hex TLV blob.</param>
        /// <returns>A collection of TLVs.</returns>
        public static ICollection<Tlv> ParseTlv(string tlv)
        {
            if (string.IsNullOrWhiteSpace(tlv))
            {
                throw new ArgumentException("tlv");
            }

            return ParseTlv(GetBytes(tlv));
        }

        /// <summary>
        /// Parse TLV data.
        /// </summary>
        /// <param name="tlv">The byte array TLV blob.</param>
        /// <returns>A collection of TLVs.</returns>
        public static ICollection<Tlv> ParseTlv(byte[] tlv)
        {
            if (tlv == null || tlv.Length == 0)
            {
                throw new ArgumentException("tlv");
            }

            var result = new List<Tlv>();
            ParseTlv(tlv, result);

            return result;
        }

        private static void ParseTlv(byte[] rawTlv, ICollection<Tlv> result)
        {
            try
            {
                for (int i = 0, start = 0; i < rawTlv.Length; start = i)
                {
                    // 0x00 and 0xFF can be used as padding before, between, and after tags
                    if (rawTlv[i] == 0x00)
                    {
                        i++;
                        continue;
                    }

                    // bit6 being 1 indicates that the tag is 'constructed' (contains more tags within it)
                    bool constructedTlv = (rawTlv[i] & 0x20) != 0;

                    // bit1 through 5 being 1 indicates that the tag is more than one byte long
                    // recognize this and keep looking until we find the last byte of the tag, indicated by bit8 being 0
                    bool multiByteTag = (rawTlv[i] & 0x1F) == 0x1F;
                    while (multiByteTag && (rawTlv[++i] & 0x80) != 0) ;

                    // i is on the last byte of the tag, so move it forward one and 
                    // retreve the tag value from the raw tlv
                    i++;
                    int tag = GetInt(rawTlv, start, i - start);

                    // bit8 being 1 indicates that the length is multiple bytes long
                    bool multiByteLength = (rawTlv[i] & 0x80) != 0;

                    int length = multiByteLength ? GetInt(rawTlv, i + 1, rawTlv[i] & 0x1F) : rawTlv[i];
                    i = multiByteLength ? i + (rawTlv[i] & 0x1F) + 1 : i + 1;

                    // i is on the last byte of the length, so move it forward by the length we found
                    i += length;

                    // now that we know the start position and length of the data, retrieve it
                    // then create the Tlv object and add it to the list
                    byte[] rawData = new byte[i - start];
                    Array.Copy(rawTlv, start, rawData, 0, i - start);
                    var tlv = new Tlv(tag, length, rawData.Length - length, rawData);
                    result.Add(tlv);

                    // if this was a constructed tag, parse its value into individual Tlv children as well
                    if (constructedTlv)
                    {
                        ParseTlv(tlv.Value, tlv.Children);
                    }
                }
            }
            catch 
            {
                // exception 발생전까지 parsing 성공한 Tag들은 result에 포함되어 리턴되도록 함.
            }
        }

        private static string GetHexString(byte[] arr)
        {
            var sb = new StringBuilder(arr.Length * 2);
            foreach (byte b in arr)
            {
                sb.AppendFormat("{0:X2}", b);
            }

            return sb.ToString();
        }

        private static byte[] GetBytes(string hexString)
        {
            return Enumerable
                .Range(0, hexString.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hexString.Substring(x, 2), 16))
                .ToArray();
        }

        private static int GetInt(byte[] data, int offset, int length)
        {
            var result = 0;
            for (var i = 0; i < length; i++)
            {
                result = result << 8 | data[offset + i];
            }

            return result;
        }
    }
}