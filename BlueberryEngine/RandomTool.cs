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

        public static double RandDouble()
        {
            return random.NextDouble();
        }

        public static float RandFloat()
        {
            return (float)random.NextDouble();
        }

        /// <summary>Возвращает логическое значение в зависимости от заданного отношения</summary>
        /// <param name="ratio">Вероятность возврата true от 0 до 1</param>
        /// <returns>Логическое значение</returns>
        public static bool RandBool(float ratio)
        {
            return random.NextDouble() <= ratio / 100;
        }

        /// <summary>Возвращает логическое значение в зависимости от заданного отношения</summary>
        /// <param name="ratio">Вероятность возврата true от 0 до 1</param>
        /// <returns>Логическое значение</returns>
        public static bool RandBool(double ratio)
        {
            return random.NextDouble() <= ratio / 100;
        }

        /// <summary>Возвращает логическое значение c вероятностью возврата true - 50%</summary>
        /// <returns>Логическое значение</returns>
        public static bool RandBool()
        {
            return random.NextDouble() <= 0.5;
        }

        public static int RandSign()
        {
            return random.NextDouble() <= 0.5 ? 1 : -1;
        }

        /// <summary>Возвращает число в пределе с более высокой вероятностью в большую или меньшую сторону</summary>
        /// <param name="ratio">Чем выше коэффициент, тем выше шанс получения большего числа и наоборот</param>
        /// <param name="min">Минимальное значение</param>
        /// <param name="max">Максимальное значение</param>
        public static int PreferRandInt(double ratio, double min, double max)
        {
            //Ratio += 50;// для удобства
            double RatioPerc = (max / min * (ratio + (min / max * ratio)));
            // RatioPerc = (RatioPerc * Ratio) / 100;
            int RealAvarege = (int)(min + max) / 2;
            int avarege = (int)(min * RatioPerc / 100);
            if (avarege < min)
                avarege = (int)min;
            else if (avarege > max)
                avarege = (int)max;
            while (true)
            {
                int temp = random.Next((int)min, (int)max);
                //  if(RealAvarege > avarege)

                if (avarege < temp)
                {
                    if (RandBool(ratio))
                        return temp;
                }
                else
                    if (RandBool(100 - ratio))
                        return temp;
            }
        }
    }
}