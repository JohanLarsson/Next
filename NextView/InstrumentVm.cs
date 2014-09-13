namespace NextView
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using Next;
    using Next.Dtos;

    using NextView.Annotations;

    public class InstrumentVm : INotifyPropertyChanged
    {
        private readonly NextClient _client;

        private bool _isSelected;

        private InstrumentMatch _info;

        public InstrumentVm(InstrumentItem instrument, NextClient client)
        {
            _client = client;
            Instrument = instrument;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public InstrumentItem Instrument { get; private set; }

        public InstrumentMatch Info
        {
            get
            {
                return _info;
            }
            private set
            {
                if (Equals(value, _info))
                {
                    return;
                }
                _info = value;
                OnPropertyChanged();
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
                UpdateInstrument();
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

        private async void UpdateInstrument()
        {
            if (Info != null)
            {
                return;
            }
            Info = await _client.InstrumentSearch(new InstrumentDescriptor(Instrument.MarketID, Instrument.Identifier));
        }
    }
}