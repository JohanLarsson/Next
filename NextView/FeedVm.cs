using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

using Next;
using NextView.Annotations;

namespace NextView
{
    using System;

    public class FeedVm : INotifyPropertyChanged
    {
        private readonly ObservableCollection<FeedEvent> _messages = new ObservableCollection<FeedEvent>();

        public FeedVm(NextFeed feed)
        {
            Feed = feed;
            Feed.ReceivedSomething += (o, s) => Application.Current.Dispatcher.Invoke(() => Messages.Insert(0, FeedEvent.Read(s)));
            Feed.WroteSomething += (o, s) => Application.Current.Dispatcher.Invoke(() => Messages.Insert(0, FeedEvent.Wrote(s)));
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

    public class FeedEvent
    {
        private FeedEvent(string readOrWrote, string message)
        {
            TimeStamp = DateTime.UtcNow;
            Action = readOrWrote;
            Message = message;
        }
        public static FeedEvent Wrote(string message)
        {
            return new FeedEvent("Wrote", message);
        }
        public static FeedEvent Read(string message)
        {
            return new FeedEvent("Read", message);
        }
        public DateTime TimeStamp { get; private set; }

        public string Action { get; private set; }

        public string Message { get; set; }
    }
}
