using System.Collections.Generic;
using System.IO;
using System;
using Fft;
using NUnit.Framework;
using System.Linq;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace nunit.test
{
    public class fft
    {
        Complex[] data = null;
        private string file = Path.Combine(helper.assert_dir, "sine_wave.csv");

        [SetUp]
        public void Setup()
        {
            data = helper.read_csv<Complex>(file);
        }

        [Test]
        public void diff_fft_ifft()
        {
            var fd = new fft_psd();
            var x = fd.fft(data);
            var y = fd.ifft(x);

            double tolerance = 1e-10; // …Ë∂®»›≤Ó÷µ
            for (int i = 0; i < data.Length; i++)
            {
                Assert.IsTrue(Math.Abs(data[i].Real - y[i].Real) < tolerance,
                    $"Values at index {i} are not equal: {data[i]} != {y[i]}");
            }
        }

        [TestCaseSource(nameof(InputCases))]
        public void input_fft(Complex[] input)
        {
            var fd = new fft_psd();
            var x = fd.fft(input);
        }


        public static IEnumerable<TestCaseData> InputCases
        {
            get
            {
                yield return new TestCaseData(new Complex[] {
                    new Complex(0, 0), 
                    new Complex(-5.8201672e-16, 0),
                });
            }
        }
    }
}