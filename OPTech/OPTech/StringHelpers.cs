using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace OPTech
{
    static class StringHelpers
    {
        private static readonly char[] faceSeparators = new char[] { ':', ' ' };

        private static readonly char[] vertexSeparators = new char[] { ':', ' ', '(', ')' };

        private static readonly char[] hardpointSeparators = new char[] { ':', ' ' };

        private static readonly char[] engineGlowSeparators = new char[] { ':', ' ' };

        public static bool SplitFace(string line, out int thisMesh, out int thisFace)
        {
            string[] parts = line.Split(faceSeparators, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 4)
            {
                thisMesh = -1;
                thisFace = -1;
                return false;
            }

            thisMesh = int.Parse(parts[1], CultureInfo.InvariantCulture) - 1;
            thisFace = int.Parse(parts[3], CultureInfo.InvariantCulture) - 1;
            return true;
        }

        public static bool SplitFace(string line, out int thisMesh, out int thisFace, out int textureCount, out string texture)
        {
            string[] parts = line.Split(faceSeparators, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 6)
            {
                thisMesh = -1;
                thisFace = -1;
                texture = null;
                textureCount = 0;
                return false;
            }

            thisMesh = int.Parse(parts[1], CultureInfo.InvariantCulture) - 1;
            thisFace = int.Parse(parts[3], CultureInfo.InvariantCulture) - 1;
            textureCount = int.Parse(parts[5].TrimEnd(','), CultureInfo.InvariantCulture);
            texture = string.Join(" ", parts.Skip(6));
            return true;
        }

        public static bool SplitVertex(string line, out int thisMesh, out int thisFace, out int thisVertex)
        {
            string[] parts = line.Split(vertexSeparators, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 6)
            {
                thisMesh = -1;
                thisFace = -1;
                thisVertex = -1;
                return false;
            }

            thisMesh = int.Parse(parts[1], CultureInfo.InvariantCulture) - 1;
            thisFace = int.Parse(parts[3], CultureInfo.InvariantCulture) - 1;
            thisVertex = int.Parse(parts[5], CultureInfo.InvariantCulture) - 1;
            return true;
        }

        public static bool SplitHardpoint(string line, out int thisMesh, out int thisHardpoint)
        {
            string[] parts = line.Split(hardpointSeparators, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 4)
            {
                thisMesh = -1;
                thisHardpoint = -1;
                return false;
            }

            thisMesh = int.Parse(parts[1], CultureInfo.InvariantCulture) - 1;
            thisHardpoint = int.Parse(parts[3], CultureInfo.InvariantCulture) - 1;
            return true;
        }

        public static bool SplitEngineGlow(string line, out int thisMesh, out int thisEngineGlow)
        {
            string[] parts = line.Split(engineGlowSeparators, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 4)
            {
                thisMesh = -1;
                thisEngineGlow = -1;
                return false;
            }

            thisMesh = int.Parse(parts[1], CultureInfo.InvariantCulture) - 1;
            thisEngineGlow = int.Parse(parts[3], CultureInfo.InvariantCulture) - 1;
            return true;
        }
    }
}
