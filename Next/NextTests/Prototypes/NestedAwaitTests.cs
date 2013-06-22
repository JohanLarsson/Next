using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NextTests.Prototypes
{
    /// <summary>
    /// This class was just for playing around with tasks trying to figure out deadlock problem
    /// http://stackoverflow.com/questions/17248680/await-works-but-calling-task-result-hangs
    /// </summary>
    [Explicit]
    class NestedAwaitTests
    {
        [Test]
        public void RandomTasksTests()
        {
            //Assert.AreEqual(1, GetLevel1().Result);
            //Assert.AreEqual(1, Prop);
            Assert.AreEqual(1, GetTaskAsync1().Result);
            Assert.AreEqual(1, GetTaskAsync2().Result);
            Assert.AreEqual(1, Hang1().Result);
            Assert.AreEqual(1, Hang2().Result);
        }

        //private int Prop
        //{
        //    get { return GetLevel1().Result; }
        //}

        //private async Task<int> GetLevel1()
        //{
        //    return await GetAsyncTaskCompletionSource();
        //}



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


    }

    public class TaskCompletionSourceVanillaTests
    {
        [Test]
        public void AsyncTaskCompletionSource_Result_Test()
        {
            Task<int> task = GetAsyncTaskCompletionSource();
            int result = task.Result;
            Assert.AreEqual(1, result);
        }

        [Test]
        public async void AwaitAsyncTaskCompletionSource_Result_Test()
        {
            int result = await GetAsyncTaskCompletionSource();
            Assert.AreEqual(1, result);
        }

        private async Task<int> GetAsyncTaskCompletionSource()
        {
            var tcs = new TaskCompletionSource<int>();
            tcs.SetResult(1);
            return await tcs.Task;
        }

        [Test]
        public void TaskCompletionSource_Result_Test()
        {
            int result = GetTaskCompletionSource().Result;
            Assert.AreEqual(1, result);
        }

        [Test]
        public async void AwaitTaskCompletionSource_Result_Test()
        {
            int result = await GetTaskCompletionSource();
            Assert.AreEqual(1, result);
        }

        private Task<int> GetTaskCompletionSource()
        {
            var tcs = new TaskCompletionSource<int>();
            tcs.SetResult(1);
            return tcs.Task;
        }

        [Test]
        public void DelayedTaskTest()
        {
            Task<int> task = GetTaskCompletionSourceDelayed();
            Assert.AreEqual(1, task.Result);
        }

        private Task<int> GetTaskCompletionSourceDelayed()
        {
            var tcs = new TaskCompletionSource<int>();
            Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    tcs.SetResult(1);
                });
            return tcs.Task;
        }
    }

    public class TaskVanillaTests
    {
        [Test]
        public void GetTaskResultTest()
        {
            Task<int> task = GetTask();
            Assert.AreEqual(1, task.Result);
        }

        [Test]
        public async void GetTaskAwaitTest()
        {
            Task<int> task = GetTask();
            Assert.AreEqual(1, await task);
        }

        private Task<int> GetTask()
        {
            return new Task<int>(() => 1);
        }
    }
}
