using edflib;
using System.IO;

using NUnit.Framework;
using System.Collections.Generic;
using Fft;
using System;

namespace nunit.test
{
    [TestFixture]
    public class psd
    {
        private string file = Path.Combine(helper.assert_dir, "X.edf");

        private edf_file edf_file;

        // List of lead pairs
        private List<(string, string)> leadPairs = new List<(string, string)>
            {
                ("EEG Fp1-REF", "EEG F3-REF"),
                ("EEG Fp2-REF", "EEG F4-REF"),
                ("EEG F3-REF", "EEG C3-REF"),
                ("EEG F4-REF", "EEG C4-REF"),
                ("EEG C3-REF", "EEG P3-REF"),
                ("EEG C4-REF", "EEG P4-REF"),
                ("EEG P3-REF", "EEG O1-REF"),
                ("EEG P4-REF", "EEG O2-REF"),
                ("EEG Fp1-REF", "EEG F7-REF"),
                ("EEG Fp2-REF", "EEG F8-REF"),
                ("EEG F7-REF", "EEG T3-REF"),
                ("EEG F8-REF", "EEG T4-REF"),
                ("EEG T3-REF", "EEG T5-REF"),
                ("EEG T4-REF", "EEG T6-REF"),
                ("EEG T5-REF", "EEG O1-REF"),
                ("EEG T6-REF", "EEG O2-REF"),
                ("EEG F3-REF", "EEG P3-REF"),
                ("EEG F4-REF", "EEG P4-REF")
            };


        [OneTimeSetUp]
        public void set_up()
        {
            edf_file = new edf_file(file);
        }

        [Test]
        public void cal_psd()
        {
            Assert.IsTrue(edf_file.read_data(22));
            CalculatePsdForLeads(edf_file);
        }


        public void CalculatePsdForLeads(edf_file psd)
        {
            string csv = "C:\\Users\\admin\\source\\For_test_PSD\\For_test_PSD\\diff.csv";
            List<double> data = new List<double>();

            using (var reader = new StreamReader(csv))
            {
                string line;
                // Skip the header line
                reader.ReadLine();

                while ((line = reader.ReadLine()) != null)
                {
                    double value = 0;

                    var values = line.Split(',');
                    if (values.Length == 2)
                    {
                        value = Convert.ToDouble(values[0]);

                    }
                    else
                    {
                        value = Convert.ToDouble(line);
                    }

                    data.Add(value);
                }
            }

            fft_psd fft_Psd = new fft_psd();
            var x = fft_Psd.get_one_lead_psd(data.ToArray(), 4478, 5);
            //foreach (var (lead1, lead2) in leadPairs)
            //{
            //    var difference = psd.get_lead_diff(lead1, lead2);
            //    difference = new double[] { 1.0, 1.0, 1.0, 1.0 };
            //    var x = fft_Psd.get_one_lead_psd(difference, 4478, 5);
            //}
        }
    }
}
