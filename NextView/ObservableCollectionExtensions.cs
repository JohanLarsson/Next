using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NextView
{
    public static class ObservableCollectionExtensions
    {
        public static void UpdateCollection<T>(this ObservableCollection<T> collection, Task<List<T>> task)
        {
            Application.Current.Dispatcher.Invoke(collection.Clear);
            task.ContinueWith(ant => ant.Result.ForEach(x=>Application.Current.Dispatcher.Invoke(()=>collection.Add(x))));
        }
    }
}
