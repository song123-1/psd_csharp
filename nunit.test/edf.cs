using NUnit.Framework;
using edflib;
using System.IO;

namespace nunit.test
{
    [TestFixture]
    public class edf
    {
        private string file = Path.Combine(helper.assert_dir, "X.edf");

        private edf_file edf_file;

        [OneTimeSetUp]
        public void set_up()
        {
            edf_file = new edf_file(file);
        }

        [Test]
        public void open_with_correct_path()
        {
            Assert.IsTrue(edf_file.open_edf(file));
        }

        [Test]
        public void read_cnt([Values(22)] int cnt)
        {
            Assert.IsTrue(edf_file.read_data(cnt));
        }
    }
}
