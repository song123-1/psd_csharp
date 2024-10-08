using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Fft
{
    public class fft_psd
    {
        private IntPtr in_ptr;
        private IntPtr out_ptr;
        private IntPtr plan_ptr;

        private Complex[] compute(Complex[] input, bool norm = true,
            fftw_direction direction = fftw_direction.FORWARD)
        {
            int n = input.Length;
            double[] temp_arr = c2d(input);

            in_ptr = fftw_func.fftw_malloc(n * 2 * 16);
            out_ptr = fftw_func.fftw_malloc(n * 2 * 16);

            // Copy array to the input pointer.
            Marshal.Copy(temp_arr, 0, in_ptr, n * 2);

            // Create plan_ptr(one-dimension) 
            plan_ptr = fftw_func.fftw_plan_dft_1d(
                n, in_ptr, out_ptr,
                (int)direction, (uint)fftw_planner.ESTIMATE);

            fftw_func.fftw_execute(plan_ptr);

            Complex[] c_arr = retrieve_out(out_ptr, n, norm);

            // Free unmanaged memory.
            fftw_func.fftw_destroy_plan(plan_ptr);
            fftw_func.fftw_free(in_ptr);
            fftw_func.fftw_free(out_ptr);

            return c_arr;
        }

        /// <summary>
        /// FFT
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Complex[] fft(Complex[] input) => compute(input, false, fftw_direction.FORWARD);

        /// <summary>
        /// IFFT
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Complex[] ifft(Complex[] input) => compute(input, true, fftw_direction.BACKWARD);

        /// <summary>
        /// Read data from pointer.
        /// </summary>
        /// <param name="out_ptr"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Complex[] retrieve_out(IntPtr out_ptr, int n, bool norm)
        {
            double[] temp_arr = new double[n * 2];
            // Copy data from the pointer.
            Marshal.Copy(out_ptr, temp_arr, 0, n * 2);

            double scale = norm ? 1.0 / n : 1.0;

            // Shape
            Complex[] c = new Complex[n];
            for (int i = 0; i < n; i++)
            {
                // TODO: pyfftw库具体实现位置
                //if (norm && i == 0)
                //{
                //    c[0] = new Complex(0, 0);
                //    continue;
                //}

                double real = temp_arr[2 * i] * scale;
                double imag = temp_arr[2 * i + 1] * scale;

                c[i] = new Complex(real, imag);
            }

            return c;
        }

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
            mean = sum_arr(arr, 0, arr.Length);

            return mean / arr.Length;
        }

        /// <summary>
        /// The sum of array.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private static double sum_arr(double[] arr, int start, int end)
        {
            double sum = 0;
            for (int i = start; i < end; i++)
            {
                sum += arr[i];
            }

            return sum;
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

        public (double[] freqs, double[] meanResult) welch(
            double[] signal, double fs, int nperseg, int noverlap)
        {
            int step = nperseg - noverlap;
            // TODO: Get_window, in the scipy module implementation, is not symmetric and is incremented by 1
            double[] win_orig = hann_window(nperseg + 1);
            double[] win_truncate = new double[nperseg];
            Array.Copy(win_orig, win_truncate, nperseg);

            double win_sum = 0;
            
            for (int i = 0; i < win_truncate.Length; i++)
            {
                win_sum += win_truncate[i] * win_truncate[i];
            }

            double scale = 1.0 / (win_sum * fs);

            int num_x = signal.Length - nperseg;
            int num_cnt = (int)Math.Ceiling(num_x / step * 1.0) + 1;

            double[][] psd_arr = new double[num_cnt][];
            for (int i = 0; i < num_cnt; i++)
            {
                int start = i * step;
                psd_arr[i] = new double[nperseg];
                Array.Copy(signal, start, psd_arr[i], 0, nperseg);

                psd_arr[i] = detrend(psd_arr[i]); // detrend
                for (int j = 0; j < nperseg; j++)  // window
                {
                    psd_arr[i][j] *= win_truncate[j];
                }
            }

            Complex[][] psd_c = d2c(psd_arr);

            // compute fft
            double[][] fft_results = new double[num_cnt][];
            for (int i = 0; i < num_cnt; i++)
            {
                Complex[] c = fft(psd_c[i]);
                double[] fft_rs = c2d(c);
                fft_results[i] = new double[nperseg / 2 + 1];
                for (int j = 0; j < nperseg / 2 + 1; j++)
                {
                    //Complex.Conjugate(c)*c
                    double magnitude_sqrt = Math.Pow(fft_rs[2 * j], 2) + Math.Pow(fft_rs[2 * j + 1], 2);
                    
                    fft_results[i][j] = magnitude_sqrt * scale;
                    // 共轭性
                    if (nperseg % 2 == 0 && nperseg > 2)
                    {
                        // 除开0和最后一个
                        if (j > 0 && j < nperseg / 2)
                        {
                            fft_results[i][j] *= 2;
                        }
                    }
                    else if (nperseg % 2 == 1 && nperseg > 2)
                    {   //除开0
                        if (j > 0)
                        {
                            fft_results[i][j] *= 2;
                        }
                    }
                }
            }

            double[] mean_fft_result = new double[nperseg / 2 + 1];
            for (int i = 0; i < nperseg / 2 + 1; i++)
            {
                for (int j = 0; j < num_cnt; j++)
                {
                    mean_fft_result[i] += fft_results[j][i];
                }

                mean_fft_result[i] /= num_cnt;
            }

            // Calculate frequency bins
            double[] freqs = new double[nperseg / 2 + 1];
            for (int i = 0; i < freqs.Length; i++)
            {
                freqs[i] = i * (fs / nperseg);
            }


            return (freqs, mean_fft_result);
        }


        public double[] get_psd_welch(double[] signal)
        {
            var (fs, pxx_spec) = welch(signal, 500, 500, 250);

            double out_gamma = sum_arr(pxx_spec, 30, 80);

            double[] out_PSD = new double[31];

            Array.Copy(pxx_spec, out_PSD, 30);

            out_PSD[30] = out_gamma;

            return out_PSD;
        }

    
        public double[][] get_one_lead_psd(double[] in_signal, int signal_second_len, int time_scale)
        {
            if (signal_second_len < time_scale) return null;

            List<double[]> psd = new List<double[]>();

            int out_len = signal_second_len / time_scale;
            int remain_sig_second = signal_second_len % time_scale;

            int p = 1;
            int high_th = 7;

            for (int i = 0; i < out_len * time_scale; i += time_scale)
            {
                int time_begin = i * 500 - p * 250;
                int time_end = time_begin + time_scale * 500 + 250 * p;

                if (time_begin < 0) time_begin = 0;
                if (time_end > signal_second_len * 500) time_end = signal_second_len * 500;

                double[] cur_signal = slice_arr(in_signal, time_begin, time_end);
                double[] one_seg_psd = get_psd_welch(cur_signal);

                double c_psd_max = one_seg_psd.Max();
                if (c_psd_max <= high_th) c_psd_max = high_th;

                for (int j = 0; j < one_seg_psd.Length; j++)
                {
                    if (one_seg_psd[j] > high_th)
                    {
                        one_seg_psd[j] = high_th + 3 * (one_seg_psd[j] - high_th) / c_psd_max;
                    }
                }

                for (int j = 0; j < time_scale; j++)
                    psd.Add(one_seg_psd);
            }

            // Handle remaining seconds
            if (remain_sig_second > 0)
            {
                int time_end = signal_second_len * 500;
                int time_begin = time_end - time_scale * 500;
                var cur_signal = slice_arr(in_signal, time_begin, time_end);
                var oneSegPsd = get_psd_welch(cur_signal);

                for (int i = 0; i < remain_sig_second; i++)
                    psd.Add(oneSegPsd);
            }

            int rows = psd.Count;
            int cols = psd[0].Length;
            double[][] psdArray = new double[cols][];

            for (int i = 0; i < cols; i++)
            {
                psdArray[i] = new double[rows];
                for (int j = 0; j < rows; j++)
                {
                    psdArray[i][j] = psd[j][i];
                }
            }

            return psdArray;
        }

    }
}
