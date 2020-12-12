using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace SplineInterpolator
{
    class Program
    {

        const int nurbs_p = 3;

        static int[] knots;

        static int nurbs_n;

        static double[,] matrix_N;
        static void Main(string[] args)
        {

            double[,] detectedPoints = readPoints("C:/CODE/geomod/export.txt");

            knotSetup(nurbs_n);

            matrixNSetup();

            var A = Matrix<double>.Build.DenseOfArray(matrix_N);
            var b = Matrix<double>.Build.DenseOfArray(detectedPoints);
            var x = A.Solve(b);

            using (StreamWriter writer = new StreamWriter("C:/CODE/geomod/export2.txt"))
            {
                for (int i = 0; i < nurbs_n; i++)
                {
                    writer.Write(x[i, 0]);
                    writer.Write(' ');
                    writer.WriteLine(x[i, 1]);
                }
            }
            Console.WriteLine(A);
            Console.WriteLine(b);
            Console.WriteLine(x);
        }

        private static double[,] readPoints(string filePath)
        {
            IEnumerable<string> text = System.IO.File.ReadLines(filePath);
            int linesCount = text.Count();
            double[,] result = new double[linesCount, 2];

            int currentLineCount = 0;
            foreach (string line in text)
            {
                if (new Random().NextDouble() > 0)
                {
                    string[] parts = line.Split(' ');
                    result[currentLineCount, 0] = Int32.Parse(parts[0]);
                    result[currentLineCount, 1] = Int32.Parse(parts[1]);
                    currentLineCount++;
                }
            }
            nurbs_n = currentLineCount;


            double[,] result2 = new double[currentLineCount, 2];
            for (int i = 0; i < currentLineCount; i++)
            {
                result2[i, 0] = result[i, 0];
                result2[i, 1] = result[i, 1];
            }
            return result2;
        }

        private static void knotSetup(int pointsCount)
        {
            int nurbs_n = pointsCount / 2 - 1;
            int knot_len = nurbs_n + nurbs_p + 2 + 1000;
            knots = new int[knot_len];
            for (int k = 0; k < knot_len; k++)
            {
                knots[k] = k;
            }
        }

        private static void matrixNSetup()
        {
            int n = nurbs_n;
            matrix_N = new double[n, n];
            for(int i = 0; i < nurbs_n; i++)
            {
                for (int j = 0; j < nurbs_n; j++)
                {
                    if (i == j)
                    {
                        matrix_N[i, j] = (i == 0 || i == nurbs_n - 1) ? 2 : 4;
                    }
                    else if (i - j == 1 || j - i == 1)
                    {
                        matrix_N[i, j] = 1;
                    }
                    else
                    {
                        matrix_N[i, j] = 0;
                    }
                }
            }
        }
    }
}
