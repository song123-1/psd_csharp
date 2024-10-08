using System;

namespace common
{
    public class fft_utils
    {
        /// <summary>
        /// Hann 窗
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static double[] hann_window(int length)
        {
            double[] window = new double[length];
            for (int i = 0; i < length; i++)
            {
                window[i] = 0.5 * (1 - Math.Cos(2.0 * Math.PI * i / (length - 1)));
            }

            return window;
        }

        /// <summary>
        /// Caculate the average of the array.
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static double get_mean(double[] arr)
        {
            double mean = 0;
            mean = array_op.sum_arr(arr, 0, arr.Length);

            return mean / arr.Length;
        }

        /// <summary>
        /// Remove the average from the array。
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static double[] detrend(double[] arr)
        {
            double mean = get_mean(arr);
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] -= mean;
            }

            return arr;
        }
    }
}
