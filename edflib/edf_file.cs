using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

namespace edflib
{
    public class edf_file
    {
        private string _file;
        private edflib_hdr_t _hdr;
        private Dictionary<string, double[]> _data_dict;
        private bool _opened = false;

        public edf_file(string file)
        {
            _file = file;
        }

        ~edf_file()
        {
            if (edflib_func.edflib_is_file_used(_file) == 1)
            {
                edflib_func.edfclose_file(_hdr.handle);
            }
        }

        public bool open_edf(string file = null)
        {
            if (file != null) _file = file;

            _hdr = new edflib_hdr_t();
            IntPtr hdr_ptr = IntPtr.Zero;

            hdr_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(_hdr));
            Marshal.StructureToPtr(_hdr, hdr_ptr, true);

            int tags = edflib_func.edfopen_file_readonly(_file, hdr_ptr, edflib_constants.EDFLIB_DO_NOT_READ_ANNOTATIONS);
            _hdr = Marshal.PtrToStructure<edflib_hdr_t>(hdr_ptr);

            // Free the pointer
            if (hdr_ptr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(hdr_ptr);
            }

            // 打卡失败
            if (tags < 0)
            {
                switch (_hdr.filetype)
                {
                    //
                }

                _opened = true;
                return false;
            }

            return true;
        }

        public bool read_data(int count)
        {
            // Open the file before reading when the file isn`t opened.
            if (!_opened)
            {
                if (!open_edf()) return false;
            }

            _data_dict = new Dictionary<string, double[]>();


            int batch_size = 16384;

            // 遍历
            for (int i = 0; i < count; i++)
            {
                string label = _hdr.signalparam[i].label;
                long total_samples = _hdr.signalparam[i].smp_in_datarecord * _hdr.datarecords_in_file;
                double[] bufs = new double[total_samples];

                for (int offset = 0; offset < total_samples; offset += batch_size)
                {
                    long samples_to_read = Math.Min(batch_size, total_samples - offset);
                    double[] buf = new double[(int)samples_to_read];
                    int x = edflib_func.edfread_physical_samples(_hdr.handle, i, (int)samples_to_read, buf);
                    if (x == -1)
                    {
                        return false;
                    }
                    else
                    {
                        Array.Copy(buf, 0, bufs, offset, samples_to_read);
                    }
                }

                // Remove redundance the spaces
                label = label.TrimEnd(' '); 
                _data_dict[label] = bufs;
            }

            return true;
        }

        public double[] get_lead_diff(string lead1, string lead2)
        {
            bool x = true;
            x = _data_dict.TryGetValue(lead1, out double[] signal1);
            if (!x) return null;
            x = _data_dict.TryGetValue(lead2, out double[] signal2);
            if (!x) return null;

            return subtract_arrays(signal1, signal2);
        }

        public static double[] subtract_arrays(double[] first, double[] second)
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
