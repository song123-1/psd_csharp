using System;
using System.IO;

using edflib;

using NUnit.Framework;

namespace nunit.test
{
    [TestFixture]
    public class edf
    {
        private readonly string file = Path.Combine(helper.assert_dir, "X.edf");

        private edf_file edf_file;

        [SetUp]
        public void set_up()
        {
            edf_file = new edf_file(file);
        }

        [TestCase("1")]
        public void with_not_exist(string file)
        {
            Assert.Throws<FileNotFoundException>(() => edf_file.open_edf(file));
        }

        [Test]
        public void open_with_correct_path()
        {
            Assert.IsTrue(edf_file.open_edf(file));
        }

        [TestCase(-1)]
        public void with_is_invalid(int pos_cnt)
        {
            Assert.Throws<ArgumentException>(() => edf_file.read_data(pos_cnt));
        }

        [TestCase(10000000)]
        public void with_out_range(int out_cnt)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => edf_file.read_data(out_cnt));
        }

        [Test]
        public void with_is_valid([Values(22)] int cnt)
        {
            Assert.IsTrue(edf_file.read_data(cnt));
        }


        [TearDown]
        public void close_file()
        {
            edf_file.edf_close();
        }
    }
}
