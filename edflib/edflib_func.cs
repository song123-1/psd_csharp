using System;
using System.Runtime.InteropServices;

namespace edflib
{
    public static class edflib_func
    {
        // edflib_is_file_used
        [DllImport("edf.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int edflib_is_file_used(
            [MarshalAs(UnmanagedType.LPStr)] string path);

        // edfclose_file
        [DllImport("edf.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int edfclose_file(int handle);

        // Import the edfopen_file_readonly function
        [DllImport("edf.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int edfopen_file_readonly(
            [MarshalAs(UnmanagedType.LPStr)] string path,
            IntPtr edfhdr,
            int read_annotations);

        // Import the edfread_physical_samples function
        [DllImport("edf.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int edfread_physical_samples(
            int handle,
            int edfsignal,
            int n,
            [Out] double[] buf);
    }

    public static class edflib_constants
    {
        public const long EDFLIB_TIME_DIMENSION = 10000000L;
        public const int EDFLIB_MAXSIGNALS = 4096;
        public const int EDFLIB_MAX_ANNOTATION_LEN = 512;

        public const int EDFLIB_DO_NOT_READ_ANNOTATIONS = 0;
        public const int EDFLIB_READ_ANNOTATIONS = 1;
        public const int EDFLIB_READ_ALL_ANNOTATIONS = 2;

        public const int EDFSEEK_SET = 0;
        public const int EDFSEEK_CUR = 1;
        public const int EDFSEEK_END = 2;
    }
    // edflib_para_t
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct edflib_param_t
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 17)]
        public string label;             // Label of the signal
        public long smp_in_file;    // Number of samples in the file
        public double phys_max;          // Physical maximum
        public double phys_min;          // Physical minimum
        public int dig_max;              // Digital maximum
        public int dig_min;              // Digital minimum
        public int smp_in_datarecord;    // Number of samples in a datarecord
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string physdimension;      // Physical dimension (unit)
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
        public string prefilter;         // Prefilter settings
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
        public string transducer;        // Transducer (sensor)
    }

    // Define a struct for edf_annotation_struct
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct edflib_annotation_t
    {
        public long onset;                              // Onset time of the event
        public long duration_l;                         // Duration
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string duration;                              // Duration in seconds
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)] // Assuming EDFLIB_MAX_ANNOTATION_LEN is 255
        public string annotation;                            // Description of the annotation
    }

    // Define a struct for edf_hdr_struct
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct edflib_hdr_t
    {
        public int handle;                                   // A handle used to distinguish the different files
        public int filetype;                                 // File type (0: EDF, 1: EDF+, 2: BDF, 3: BDF+)
        public int edfsignals;                               // Number of signals in the file
        public long file_duration;                      // Duration of the file
        public int startdate_day;                            // Startdate: day
        public int startdate_month;                          // Startdate: month
        public int startdate_year;                           // Startdate: year
        public long starttime_subsecond;                // Starttime subsecond
        public int starttime_second;                         // Starttime: second
        public int starttime_minute;                         // Starttime: minute
        public int starttime_hour;                           // Starttime: hour
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
        public string patient;                               // Patient field of header
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
        public string recording;                             // Recording field of header
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
        public string patientcode;                           // Patient code
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string sex;                                   // Patient sex
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string gender;                                // Deprecated, use sex
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string birthdate;                             // Birthdate
        public int birthdate_day;                            // Birthdate: day
        public int birthdate_month;                          // Birthdate: month
        public int birthdate_year;                           // Birthdate: year
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
        public string patient_name;                          // Patient name
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
        public string patient_additional;                    // Additional patient info
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
        public string admincode;                             // Admin code
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
        public string technician;                            // Technician info
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
        public string equipment;                             // Equipment used
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
        public string recording_additional;                  // Additional recording info
        public long datarecord_duration;                // Duration of a datarecord
        public long datarecords_in_file;                // Number of datarecords in the file
        public long annotations_in_file;                // Number of annotations/events/triggers
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = edflib_constants.EDFLIB_MAXSIGNALS)]
        public edflib_param_t[] signalparam;                // Array of signal parameters
    }
}
