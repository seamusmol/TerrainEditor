using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Main.Util
{
    public static class MathUtil
    {

        public static Matrix GetRotationfromAngles(float RX,float RY, float RZ)
        {
            Matrix Rotation = new Matrix();
            Rotation += Matrix.CreateRotationZ(MathHelper.ToRadians(RX));
            Rotation *= Matrix.CreateRotationY(MathHelper.ToRadians(RY));
            Rotation *= Matrix.CreateRotationX(MathHelper.ToRadians(RX));

            return Rotation;
        }

        public static float Lerp(float X, float X1, float X2, float Q0, float Q1)
        {
            return ((X2 - X) / (X2 - X1)) * Q0 + ((X - X1) / (X2 - X1)) * Q1;
        }

        
        public static float BiLerp(float PX, float PY, float X1, float X2, float Y1, float Y2, float Q11, float Q21, float Q12, float Q22)
        {
            float V1 = Lerp(PX, X1, X2, Q11, Q21);
            float V2 = Lerp(PX, X1, X2, Q12, Q22);

            return Lerp(PY, Y1, Y2, V1, V2);
        }
        

        public static float RoundToNearest(float Value, float RoundVal)
        {
            return (float)Math.Round(Value / RoundVal) * RoundVal;
        }
        
        public static float Distance(float X1, float X2)
        {
            return (float)Math.Sqrt((X1 - X2) * (X1 - X2));
        }

        public static double Distance(double X1, double X2)
        {
            return Math.Sqrt((X1 - X2) * (X1 - X2));
        }

        public static int Distance(int X1, int X2)
        {
            return (int)Math.Sqrt((X1 - X2) * (X1 - X2));
        }

        public static float Distance(int X1, int X2, int Y1, int Y2)
        {
            return (float)Math.Sqrt(Math.Pow(X1 -X2, 2) + Math.Pow(Y1 - Y2, 2));
        }

        public static float GetAngle(Point Center, Point Point)
        {
            Vector2 Vec = new Vector2(Point.X - Center.X, Point.Y - Center.Y);
            double radian = Math.Atan2(Vec.X, Vec.Y);
            double degrees = radian * (180 / Math.PI);
            return (float)degrees;
        }
        
        public static float GetAngle(Vector2 Center, Vector2 Point)
        {
            Vector2 Vec = new Vector2(Point.X - Center.X, Point.Y - Center.Y);
            double radian = Math.Atan2(Vec.X, Vec.Y);
            double degrees = radian * (180 / Math.PI);
            return (float)degrees;
        }

        public static float GetRadian(double Angle)
        {
            return (float)(Angle * Math.PI / 180);
        }

        public static Vector2 RotatePoint(Point P, double Angle)
        {
            double radians = Angle == 0 ? 0 : Angle * Math.PI / 180;

            double cos = Math.Cos(radians);
            double sin = Math.Sin(radians);

            double px = P.X * cos - P.Y * sin;
            double py = P.X * sin + P.Y * cos;

            return new Vector2((float)px, (float)py);
        }

        public static int CalculateMost(int[] array)
        {
            int most = 9999;
            int count = 0;
            for (int countX = 0; countX < array.Length; countX++)
            {
                if (array[countX] == most)
                {
                    continue;
                }
                int curCount = 0;
                for (int countY = 0; countY < array.Length; countY++)
                {
                    if (array[countX] == array[countY])
                    {
                        curCount++;
                    }
                }
                if (curCount >= count && array[countX] < most && array[countX] != 0)
                {
                    most = array[countX];
                    count = curCount;
                }
            }
            return most;
        }

        public static int[,] scaleArray(int[,] OldArray, int factor)
        {
            int[,] newArray = new int[(int)((OldArray.Length - 1) * factor) + 1, (int)((OldArray.GetLength(1) - 1) * factor) + 1];

            for (int i = 0; i < OldArray.GetLength(0) - 2; i++)
            {
                for (int j = 0; j < OldArray.GetLength(1) - 2; j++)
                {
                    float q11 = OldArray[i,j];
                    float q21 = OldArray[i,j + 1];
                    float q12 = OldArray[i + 1,j];
                    float q22 = OldArray[i + 1,j + 1];

                    for (int countX = 0; countX < factor; countX++)
                    {
                        for (int countY = 0; countY < factor; countY++)
                        {
                            float px = (1.00f / factor * (countX % factor)) % 1.0f;
                            float py = (1.00f / factor * (countY % factor)) % 1.0f;

                            int val = (int)Math.Round( BiLerp(px, py, 0.0f, 1.0f, 0.0f, 1.0f, q11, q21, q12, q22));
                            //int val = (int)MathUtil.bilerp(px, py, 0, 1.0f, 0, 1.0f, q11, q21, q12, q22);

                            newArray[i * factor + countX, j * factor + countY] = val;
                        }
                    }
                }
            }
            return newArray;
        }

        public static int[] GenerateFibonocciNumbers(int SeedX, int SeedY, int X, int DigitLength)
        {
            int[] Values = new int[X];

            long Num = 2 + 10 + SeedX;
            long LastNum = 1 + 10 + SeedY;

            long N1 = Num;
            long N2 = LastNum;

            int Count = 0;

            while (true)
            {
                string numString = Num + "";

                for (int i = 0; i < numString.Length; i += DigitLength)
                {
                    if (i + DigitLength < numString.Length)
                    {
                        if (Count < X)
                        {
                            String tempString = numString.Substring(i, DigitLength);
                            //String tempString = numString.Substring();

                            if (tempString != "")
                            {
                                Values[Count] = Int32.Parse(tempString);
                                Count++;
                            }
                        }
                        else
                        {
                            return Values;
                        }
                    }
                }

                if (Math.Log10(Num) < 17)
                {
                    long tempNum = Num;
                    Num += LastNum;
                    LastNum = tempNum;
                }
                else
                {
                    Num = 2 + 10 + N1;
                    LastNum = 2 + 10 + N2;
                    N1 = Num;
                    N2 = LastNum;
                }
            }
        }

        public static int[,] GenerateFibonocciNumbers(int SeedX, int SeedY, int X, int Y, int DigitLength)
        {
            int[,] values = new int[X,Y];

            long num = 2 + 10 + SeedX;
            long lastNum = 1 + 10 + SeedY;

            long n1 = num;
            long n2 = lastNum;

            int Count = 0;

            while (true)
            {
                string numString = num + "";

                for (int i = 0; i < numString.Length; i += DigitLength)
                {
                    if (i + DigitLength < numString.Length)
                    {
                        if (Count < X * Y)
                        {
                            String tempString = numString.Substring(i, DigitLength);
                            //String tempString = numString.Substring();

                            if (tempString != "")
                            {
                                values[Count / Y, Count % X] = Int32.Parse(tempString);
                                Count++;
                            }
                        }
                        else
                        {
                            return values;
                        }
                    }
                }

                if (Math.Log10(num) < 17)
                {
                    long tempNum = num;
                    num += lastNum;
                    lastNum = tempNum;
                }
                else
                {
                    num = 2 + 10 + n1;
                    lastNum = 2 + 10 + n2;
                    n1 = num;
                    n2 = lastNum;
                }
            }
        }

        

    }
}
