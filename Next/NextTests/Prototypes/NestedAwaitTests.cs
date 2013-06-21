using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NextTests.Prototypes
{
    class NestedAwaitTests
    {
        [Test]
        public void TestNameTest()
        {
            Assert.AreEqual(1, GetLevel1().Result);
            Assert.AreEqual(1, GetLevel2().Result);
            Assert.AreEqual(1, Prop);
        }

        private int Prop
        {
            get { return GetLevel1().Result; }
        }

        private async Task<int> GetLevel1()
        {
            return await GetLevel2();
        }

        private async Task<int> GetLevel2()
        {
            var tcs = new TaskCompletionSource<int>();
            tcs.SetResult(1);
            return await tcs.Task;
        }
    }
}
