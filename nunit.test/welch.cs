using System;
using System.IO;
using System.Numerics;

using Fft;

using NUnit.Framework;

namespace nunit.test
{
    [TestFixture]
    class welch
    {
        double[] data = null;
        private string file = Path.Combine(helper.assert_dir, "sine_wave.csv");

        [SetUp]
        public void set_up()
        {
            data = helper.read_csv<double>(file);
        }

        [Test]
        public void mock_data()
        {
            int seg_len = (int)Math.Floor(data.Length / 8.0);
            int noverlap = (int)Math.Floor(seg_len / 2.0);
            fft_psd.welch(data, 20, seg_len, noverlap);
        }
    }
}
