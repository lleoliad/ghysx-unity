using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GhysX.Framework.Utilities
{
    public static class Unit
    {
        public static IEnumerable<bool> GetBitList(int value)
        {
            var list = new List<bool>(32);
            for (var i = 0; i <= 31; i++)
            {
                var val = 1 << i;
                list.Add((value & val) == val);
            }
            return list;
        }

        public static bool GetBitValue(int value, ushort index)
        {
            if (index > 31) throw new ArgumentOutOfRangeException("index");
            var val = 1 << index;
            return (value & val) == val;
        }

        public static int SetBitValue(int value, ushort index, bool bitValue)
        {
            if (index > 31) throw new ArgumentOutOfRangeException("index");
            var val = 1 << index;
            return bitValue ? (value | val) : (value & ~val);
        }
    }
}