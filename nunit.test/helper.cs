using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace nunit.test
{
    public static class helper
    {
        public static string assert_dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "asserts");

        public static T[] read_csv<T>(string csv_file) where T : struct
        {
            List<T> data = new List<T>();

            using (var reader = new StreamReader(csv_file))
            {
                string line;
                // Skip the header line
                reader.ReadLine();

                while ((line = reader.ReadLine()) != null)
                {
                    var values = line.Split(',');
                    double value = Convert.ToDouble(values[1]);

                    data.Add(ConvertValue<T>(value));
                }
            }

            return data.ToArray();
        }

        private static T ConvertValue<T>(double value)
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Double:
                    return (T)(object)Convert.ToDouble(value);
                case TypeCode.Int32:
                    return (T)(object)Convert.ToInt32(value);
                case TypeCode.Single:
                    return (T)(object)Convert.ToSingle(value);
                case TypeCode.Object when typeof(T) == typeof(Complex):
                    return (T)(object)new Complex(Convert.ToDouble(value), 0);
                // Add more cases as needed
                default:
                    throw new InvalidOperationException($"Unsupported type: {typeof(T)}");
            }
        }
    }
}
