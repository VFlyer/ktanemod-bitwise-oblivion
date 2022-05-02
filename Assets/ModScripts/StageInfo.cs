using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using RNG = UnityEngine.Random;

namespace BitwiseOblivion
{
    internal class StageInfo
    {
        static int Reverse(int x, int length)
        {
            var binary = Convert.ToString(x, 2);
            return Convert.ToInt32(
                (new string('0', length - binary.Length) + binary).Replace('1', '2').Replace('0', '1')
                .Replace('2', '0'), 2);
        }

        static int GetMaxLength(byte a, byte b)
        {
            return Mathf.Max(Convert.ToString(a, 2).Length, Convert.ToString(b, 2).Length);
        }

        private static readonly Dictionary<string, Func<byte, byte, int>> BitwiseOperators = new Dictionary<string, Func<byte, byte, int>>
        {
            {"and", (a, b) => a & b},
            {"nand", (a, b) => Reverse(a & b, GetMaxLength(a, b))},
            {"or", (a, b) => a | b},
            {"nor", (a, b) => Reverse(a | b, GetMaxLength(a, b))},
            {"xor", (a, b) => a ^ b},
            {"xnor", (a, b) => Reverse(a ^ b, GetMaxLength(a, b))},
        };

        private static bool RandomBool
        {
            get
            {
                return RNG.Range(0, 3) == 1;
            }
        }
        
        internal readonly int Index;
        internal readonly Func<byte, byte, int> Operator;
        internal readonly string OperatorName;
        internal readonly int Solution;
        internal readonly bool Row1;
        internal readonly bool Col1;
        internal readonly bool RowText1;
        internal readonly bool ColText1;
        internal readonly string RecoverString;

        internal StageInfo(string str1, string str2, Action<string, object> Log)
        {
            Log("String 1: {0}", str1);
            Log("String 2: {0}", str2);
            Index = RNG.Range(0, Mathf.Min(Mathf.Min(str1.Length, str2.Length), 100));
            Log("Character index: {0}", Index+1);
            var pair = BitwiseOperators.ToArray()[RNG.Range(0, BitwiseOperators.Count)];
            Operator = pair.Value;
            OperatorName = pair.Key;
            Log("Selected character in string 1: {0}", str1[Index]);
            Log("Selected character in string 2: {0}", str2[Index]);
            var b1 = Encoding.UTF8.GetBytes(str1)[Index];
            var b2 = Encoding.UTF8.GetBytes(str2)[Index];
            Log("Character 1 UTF-8 byte (decimal): {0}", b1);
            Log("Character 2 UTF-8 byte (decimal): {0}", b2);
            RecoverString = string.Format("{0} {1}", b1, b2);
            Solution = Operator(b1, b2);
            Log("Operation: {0}", OperatorName);
            Log("Solution: {0}", Solution);
            Row1 = RandomBool;
            Col1 = RandomBool;
            RowText1 = RandomBool;
            ColText1 = RandomBool;
        }
    }
}
