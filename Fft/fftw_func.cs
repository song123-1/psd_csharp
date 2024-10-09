using System;
using System.Runtime.InteropServices;

namespace Fft
{
    public enum fftw_direction : int
    {
        FORWARD = -1,
        BACKWARD = 1,
    }

    [Flags]
    public enum fftw_planner : uint
    {
        DEFAULT = MEASURE,

        MEASURE = (0U),
        EXHAUSTIVE = (1U << 3),
        PATIENT = (1U << 5),
        ESTIMATE = (1U << 6),
        WISDOM_ONLY = (1U << 21),
    }

    public static class fftw_func
    {
        public const string dll_name = "libfftw3-3.dll";

        [DllImport(dll_name,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr fftw_plan_guru_dft(int rank, IntPtr dims, int howmany_rank, IntPtr howmany_dims, IntPtr inArray, IntPtr outArray, int sign, uint flags);

        [DllImport(dll_name,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr fftw_execute_dft(IntPtr plan, IntPtr inArray, IntPtr outArray);

        [DllImport(dll_name,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr fftw_plan_dft_r2c_1d(int n, IntPtr inArray, IntPtr outArray, uint flags);

        [DllImport(dll_name,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr fftw_plan_dft_1d(int n, IntPtr inArray, IntPtr outArray, int sign, uint flags);

        [DllImport(dll_name,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern void fftw_execute(IntPtr plan);

        [DllImport(dll_name,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern void fftw_destroy_plan(IntPtr plan);

        [DllImport(dll_name,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr fftw_malloc(int size);

        [DllImport(dll_name,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern void fftw_free(IntPtr ptr);
    }
}
