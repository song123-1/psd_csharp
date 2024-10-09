using System.Collections.Generic;
using System.IO;

using common;

using edflib;

using Fft;

using NUnit.Framework;

namespace nunit.test
{
    [TestFixture]
    public class psd
    {
        private readonly string file = Path.Combine(helper.assert_dir, "X.edf");

        private edf_file edf_file;

        // List of lead pairs
        private readonly List<(string, string)> lead_pairs = new List<(string, string)>
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


        [SetUp]
        public void set_up()
        {
            edf_file = new edf_file(file);
        }

        [Test]
        public void mock_data()
        {
            string csv = Path.Combine(helper.assert_dir, "diff.csv");
            double[] data = helper.read_csv<double>(csv);

            var x = fft_psd.get_one_lead_psd(data, 4478, 5);
        }

        [Test]
        public void edf_psd()
        {
            Assert.IsTrue(edf_file.read_data(22));

            foreach (var (lead1, lead2) in lead_pairs)
            {
                var (signal1, signal2) = edf_file.retrieve_leads(lead1, lead2);
                Assert.IsNotNull(signal1);
                Assert.IsNotNull(signal2);
                Assert.IsTrue(signal1.Length == signal2.Length);

                double[] diff = array_op.subtract_arr(signal1, signal2);


                var x = fft_psd.get_one_lead_psd(diff, 4478, 5);
            }
        }

        [TearDown]
        public void close_file()
        {
            edf_file.edf_close();
        }
    }
}
