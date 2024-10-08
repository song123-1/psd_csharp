
using System;
using System.Numerics;

namespace common
{
    public class array_op
    {
        /// <summary>
        /// Converts a complex array to a double precision array
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static double[] c2d(Complex[] input)
        {
            int len = input.Length;
            if (len == 0) return null;

            double[] temp_array = new double[len * 2];
            for (int i = 0; i < len; i++)
            {
                temp_array[2 * i] = input[i].Real;
                temp_array[2 * i + 1] = input[i].Imaginary;
            }

            return temp_array;
        }

        /// <summary>
        /// Convert a double array to a complex array.
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static Complex[][] d2c(double[][] arr)
        {
            int out_len = arr.Length;
            Complex[][] c = new Complex[out_len][];

            for (int i = 0; i < out_len; i++)
            {
                int arr_len = arr[i].Length;
                c[i] = new Complex[arr_len];
                for (int j = 0; j < arr_len; j++)
                {
                    c[i][j] = new Complex(arr[i][j], 0);
                }
            }

            return c;
        }

        /// <summary>
        /// The sum of array.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static double sum_arr(double[] arr, int start, int end)
        {
            double sum = 0;
            for (int i = start; i < end; i++)
            {
                sum += arr[i];
            }

            return sum;
        }

        /// <summary>
        /// Slice the array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static double[] slice_arr(double[] array, int start, int end)
        {
            double[] result = new double[end - start];
            Array.Copy(array, start, result, 0, end - start);

            return result;
        }

        /// <summary>
        /// Subtract the values of the two arrays.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static double[] subtract_arr(double[] first, double[] second)
        {
            if (first.Length != second.Length)
            {
                throw new ArgumentException("Arrays must be of the same length.");
            }

            double[] result = new double[first.Length];
            for (int i = 0; i < first.Length; i++)
            {
                result[i] = first[i] - second[i];
            }

            return result;
        }
    }
}
