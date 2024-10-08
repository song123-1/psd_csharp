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
        public void cal_welch()
        {
            var fd = new fft_psd();
            int seg_len = (int)Math.Floor(data.Length / 8.0);
            int noverlap = (int)Math.Floor(seg_len / 2.0);
            fd.welch(data, 20, seg_len, noverlap);
        }
    }
}
