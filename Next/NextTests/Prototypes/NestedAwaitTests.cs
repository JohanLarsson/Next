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
        public void RandomTasksTests()
        {
            Assert.AreEqual(1, GetLevel1().Result);
            Assert.AreEqual(1, GetLevel2().Result);
            Assert.AreEqual(1, Prop);
            Assert.AreEqual(1, GetTaskAsync1().Result);
            Assert.AreEqual(1, GetTaskAsync2().Result);
            Assert.AreEqual(1, Hang1().Result);
            Assert.AreEqual(1, Hang2().Result);
        }

        private int Prop
        {
            get { return GetLevel1().Result; }
        }

        private async Task<int> GetLevel1()
        {
            return await GetLevel2();
        }

        private async Task<int> Hang1()
        {
            int result = await GetTaskAsync1();
            return result;
        }

        private async Task<int> Hang2()
        {
            int result = await GetTaskAsync2();
            return result;
        }

        private Task<int> GetTaskAsync1()
        {
            var tcs = new TaskCompletionSource<int>();
            GetAsync(tcs.SetResult);
            return tcs.Task;
        }

        private async Task<int> GetTaskAsync2()
        {
            var tcs = new TaskCompletionSource<int>();
            GetAsync(tcs.SetResult);
            return await tcs.Task;
        }

        private void GetAsync(Action<int> callback)
        {
            Task.Delay(1000);
            callback(1);
        }
        private async Task<int> GetLevel2()
        {
            var tcs = new TaskCompletionSource<int>();
            tcs.SetResult(1);
            return await tcs.Task;
        }
    }
}
