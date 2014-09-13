namespace NextView
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Net.Mime;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Threading;

    using Next;
    using Next.Dtos;

    using NextView.Annotations;

    public class InstrumentListVm : INotifyPropertyChanged
    {
        private readonly NextClient _client;
        private readonly ObservableCollection<InstrumentVm> _instruments = new ObservableCollection<InstrumentVm>();
        private bool _isSelected;
        private bool _hasInstruments = false;

        public InstrumentListVm(InstrumentList instrumentList, NextClient client)
        {
            this._client = client;
            this.InstrumentList = instrumentList;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public InstrumentList InstrumentList { get; private set; }

        public ObservableCollection<InstrumentVm> Instruments
        {
            get
            {
                return _instruments;
            }
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (value.Equals(_isSelected))
                {
                    return;
                }
                _isSelected = value;
                GetInstruments();
                OnPropertyChanged();
            }
        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private async void GetInstruments()
        {
            if (_hasInstruments)
            {
                return;
            }
            _hasInstruments = true;
            var listItems = await _client.ListItems(InstrumentList.Id);
            Application.Current.Dispatcher.Invoke(
                () =>
                {
                    foreach (var instrumentItem in listItems)
                    {
                        Instruments.Add(new InstrumentVm(instrumentItem, _client));
                    }
                });
        }
    }
}
