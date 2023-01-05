using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Backend
{
    public static class Utilities
    {
        public static int HexToDec(string hex)
        {
            int dec = System.Convert.ToInt32(hex, 16);
            return dec;
        }

        public static string DecToHex(int value)
        {
            return value.ToString("X2");
        }

        public static string FloatToNormalizedToHex(float value)
        {
            return DecToHex(Mathf.RoundToInt(value * 255f));
        }

        public static float HexToFloatNormalized(string hex)
        {
            return HexToDec(hex) / 255f;
        }

        public static Color StringToColor(string hexString)
        {
            float red = HexToFloatNormalized(hexString.Substring(0, 2));
            float green = HexToFloatNormalized(hexString.Substring(2, 2));
            float blue = HexToFloatNormalized(hexString.Substring(4, 2));
            float alpha = 1f;
            if (hexString.Length >= 8)
            {
                alpha = HexToFloatNormalized(hexString.Substring(6, 2));
            }

            return new Color(red, green, blue, alpha);
        }
        public static string ColorToString(Color color, bool useAlpha = false)
        {
            string red = FloatToNormalizedToHex(color.r);
            string green = FloatToNormalizedToHex(color.g);
            string blue = FloatToNormalizedToHex(color.b);
            if (!useAlpha)
            {
                return red + green + blue;
            }
            else
            {
                string alpha = FloatToNormalizedToHex(color.a);
                return red + green + blue + alpha;
            }
        }
    }
}