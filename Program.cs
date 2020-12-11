using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

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
            /*var A = Matrix<double>.Build.DenseOfArray(new double[,] {
                { 3, 2, -1 },
                { 2, -2, 4 },
                { -1, 0.5, -1 }
            });
            var b = Vector<double>.Build.Dense(new double[] { 1, -2, 0 });
            var x = A.Solve(b);*/

            double[,] detectedPoints = readPoints("C:/CODE/geomod/export.txt");

            //nurbs_n = detectedPoints.Length / 2;

            knotSetup(nurbs_n);

            matrixNSetup();

            var A = Matrix<double>.Build.DenseOfArray(matrix_N);
            var b = Matrix<double>.Build.DenseOfArray(detectedPoints);
            var x = A.Solve(b);
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
                if (new Random().NextDouble() > 0.9)
                {
                    string[] parts = line.Split(' ');
                    result[currentLineCount, 0] = Int32.Parse(parts[1]);
                    result[currentLineCount, 1] = Int32.Parse(parts[0]);
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
            int uMax = nurbs_n + nurbs_p;
            matrix_N = new double[n, n];
            for(int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    double u = uMax * i / (double) n;
                    matrix_N[i, j] = nurbs_N(j, nurbs_p, u);
                }

            }
        }

        private static double nurbs_N(int i, int p, double u)
        {

            if (p == 0)
            {
                if (knots[i] <= u && u < knots[i + 1])
                    return 1;
                else
                    return 0;
            }
            double ret = (((u - knots[i]) / (knots[i + p] - knots[i])) * nurbs_N(i, p - 1, u))
                        + (((knots[i + p + 1] - u) / (knots[i + p + 1] - knots[i + 1])) * nurbs_N(i + 1, p - 1, u));
            return ret;
        }
    }
}
