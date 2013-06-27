using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Next;
using NextView.Annotations;

namespace NextView
{
    public class FeedVm :INotifyPropertyChanged
    {
        public NextFeed Feed { get; set; }
        public FeedVm(NextFeed feed)
        {
            Feed = feed;
            Feed.ReceivedSomething += (o, s) => Application.Current.Dispatcher.Invoke(() => Messages.Add(s));
        }

        private readonly ObservableCollection<string> _messages = new ObservableCollection<string>();
        public ObservableCollection<string> Messages
        {
            get { return _messages; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
