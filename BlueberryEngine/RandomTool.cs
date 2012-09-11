using System;

namespace Blueberry
{
    public static class RandomTool
    {
        public static Random random = new Random();

        public static int RandInt(int min, int max)
        {
            return random.Next(min, max);
        }

        public static int RandInt(int max)
        {
            if (max >= 0)
                return random.Next(max);
            throw new ArgumentOutOfRangeException("max", "must be greater or equal than 0");
        }

        public static int RandInt()
        {
            return random.Next();
        }

        public static byte RandByte()
        {
            return (byte)random.Next();
        }
        public static byte RandByte(byte max)
        {
            return (byte)random.Next(max);
        }
        public static byte RandByte(byte min, byte max)
        {
            return (byte)random.Next(min, max);
        }

        public static double RandDouble()
        {
            return random.NextDouble();
        }

        public static float RandFloat()
        {
            return (float)random.NextDouble();
        }

        public static bool RandBool(float ratio)
        {
            return random.NextDouble() <= ratio / 100;
        }

        public static bool RandBool(double ratio)
        {
            return random.NextDouble() <= ratio / 100;
        }

        public static bool RandBool()
        {
            return random.NextDouble() <= 0.5;
        }

        public static sbyte RandSign()
        {
            return random.NextDouble() <= 0.5 ? (sbyte)1 : (sbyte)-1;
        }
    }
}