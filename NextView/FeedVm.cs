using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

using Next;
using NextView.Annotations;

namespace NextView
{
    public class FeedVm : INotifyPropertyChanged
    {
        private readonly ObservableCollection<FeedEvent> _messages = new ObservableCollection<FeedEvent>();
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public FeedVm(NextFeed feed)
        {
            Feed = feed;
            Feed.ReceivedSomething += (o, s) => Application.Current.Dispatcher.Invoke(() => Messages.Insert(0, FeedEvent.Read(s)));
            Feed.WroteSomething += (o, s) => Application.Current.Dispatcher.Invoke(() => Messages.Insert(0, FeedEvent.Wrote(s)));
            Feed.ReceivedUnknownMessage += (sender, s) => Log.DebugFormat("feed:{0} unknown: {1}", sender.GetType().Name, s);
            Feed.ReceivedError += (sender, s) => Log.DebugFormat("feed:{0} error: {1}", sender.GetType().Name, s);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public NextFeed Feed { get; private set; }

        public ObservableCollection<FeedEvent> Messages
        {
            get { return _messages; }
        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
