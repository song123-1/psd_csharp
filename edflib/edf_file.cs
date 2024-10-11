using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

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

        /// <summary>
        /// Close the edf.
        /// </summary>
        public bool edf_close()
        {
            int flag = 0;
            if (edflib_func.edflib_is_file_used(_file) == 1)
            {
                flag = edflib_func.edfclose_file(_hdr.handle);
            }

            return flag == 0;
        }

        /// <summary>
        /// Open the edf.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public bool open_edf(string file = null)
        {
            if (file != null) _file = file;

            // Throwing exception if the file does not exist. 
            if (!File.Exists(_file))
            {
                throw new FileNotFoundException("The edf file does not exist!");
            }

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

                _opened = false;
                return false;
            }

            _opened = true;
            return true;
        }

        /// <summary>
        /// Read data.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public bool read_data(int count)
        {
            // Open the file before reading when the file isn`t opened.
            if (!_opened)
            {
                if (!open_edf()) return false;
            }

            _data_dict = new Dictionary<string, double[]>();

            int batch_size = 16384;

            int max_cnt = _hdr.signalparam.Length;
            if (count <= 0)
            {
                throw new ArgumentException("Incoming effective value");
            }

            if (count > max_cnt)
            {
                throw new ArgumentOutOfRangeException($"The count argument is out of range. The maximum value is {max_cnt}");
            }

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

                    //edflib_func.edfseek(_hdr.handle, i, offset, edflib_constants.EDFSEEK_SET);

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

        public Dictionary<string, int[]> read_data_all()
        {
            // Open the file before reading when the file isn`t opened.
            if (!_opened)
            {
                if (!open_edf()) return null;
            }

            var data_dict = new Dictionary<string, int[]>();

            // Allocate memory for the buffer
            IntPtr[] buf = new IntPtr[_hdr.edfsignals];
            for (int i = 0; i < _hdr.edfsignals; i++)
            {
                long smpl_nums = _hdr.signalparam[i].smp_in_datarecord * _hdr.datarecords_in_file;
                buf[i] = Marshal.AllocHGlobal((int)smpl_nums * sizeof(int));
            }

            edflib_func.edfread_all_digital_samples(_hdr.handle, buf);
            
            // Copy the data to the dictionary
            for (int i = 0; i < _hdr.edfsignals; i++)
            {
                string label = _hdr.signalparam[i].label.TrimEnd(' ');
                long smpl_nums = _hdr.signalparam[i].smp_in_datarecord * _hdr.datarecords_in_file;
                int[] bufs = new int[smpl_nums];

                Marshal.Copy(buf[i], bufs, 0, (int)smpl_nums);
                if (data_dict.ContainsKey(label))
                {

                }
                data_dict[label] = bufs;
            }

            // 使用完毕后记得释放分配的内存，防止内存泄漏
            for (int j = 0; j < _hdr.edfsignals; j++)
            {
                Marshal.FreeHGlobal(buf[j]);
            }

            return data_dict;
        }

        public Dictionary<string, int[]> read_raw_data(int count)
        {
            // Open the file before reading when the file isn`t opened.
            if (!_opened)
            {
                if (!open_edf()) return null;
            }

            var data_dict = new Dictionary<string, int[]>();

            int batch_size = 16384;

            int max_cnt = _hdr.edfsignals;
            if (count <= 0)
            {
                throw new ArgumentException("Incoming effective value");
            }

            if (count > max_cnt)
            {
                throw new ArgumentOutOfRangeException($"The count argument is out of range. The maximum value is {max_cnt}");
            }

            // 遍历
            for (int i = 0; i < count; i++)
            {
                string label = _hdr.signalparam[i].label;
                long total_samples = _hdr.signalparam[i].smp_in_datarecord * _hdr.datarecords_in_file;
                int[] bufs = new int[total_samples];

                for (int offset = 0; offset < total_samples; offset += batch_size)
                {
                    long samples_to_read = Math.Min(batch_size, total_samples - offset);
                    int[] buf = new int[(int)samples_to_read];

                    //edflib_func.edfseek(_hdr.handle, i, offset, edflib_constants.EDFSEEK_SET);

                    int x = edflib_func.edfread_digital_samples(_hdr.handle, i, (int)samples_to_read, buf);
                    if (x == -1)
                    {
                        return null;
                    }
                    else
                    {
                        Array.Copy(buf, 0, bufs, offset, samples_to_read);
                    }
                }

                // Remove redundance the spaces
                label = label.TrimEnd(' ');
                data_dict[label] = bufs;
            }

            return data_dict;
        }

        /// <summary>
        /// Retrieve key
        /// </summary>
        /// <param name="lead1"></param>
        /// <param name="lead2"></param>
        /// <returns></returns>
        public (double[] signal1, double[] signal2) retrieve_leads(
            string lead1, string lead2) => (retrieve(lead1), retrieve(lead2));

        /// <summary>
        /// Retrieve the specified key value.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private double[] retrieve(string key)
         => _data_dict.TryGetValue(key, out double[] signal) ? signal : null;
    }
}
