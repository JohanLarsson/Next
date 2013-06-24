using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextView
{
    public static class ObservableCollectionExtensions
    {
        public static void UpdateCollection<T>(this ObservableCollection<T> collection, Task<List<T>> task)
        {
            collection.Clear();
            task.ContinueWith(ant => ant.Result.ForEach(collection.Add), TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
