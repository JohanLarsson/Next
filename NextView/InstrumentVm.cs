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

        private bool _isSubscribing;

        private DepthTick _depth;

        private PriceTick _price;

        private TradeTick _trade;

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

        public bool IsSubscribing
        {
            get
            {
                return _isSubscribing;
            }
            set
            {
                if (value.Equals(_isSubscribing))
                {
                    return;
                }
                _isSubscribing = value;
                IsSelected = true;
                OnPropertyChanged();
                Subscribe();
            }
        }

        public DepthTick Depth
        {
            get
            {
                return _depth;
            }
            set
            {
                if (Equals(value, _depth))
                {
                    return;
                }
                _depth = value;
                OnPropertyChanged();
            }
        }

        public PriceTick Price
        {
            get
            {
                return _price;
            }
            set
            {
                if (Equals(value, _price))
                {
                    return;
                }
                _price = value;
                OnPropertyChanged();
            }
        }

        public TradeTick Trade
        {
            get
            {
                return _trade;
            }
            set
            {
                if (Equals(value, _trade))
                {
                    return;
                }
                _trade = value;
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

        private async void Subscribe()
        {
            if (_isSubscribing)
            {
                await _client.PublicFeed.Subscribe(new InstrumentDescriptor(Instrument.MarketID, Instrument.Identifier));
                _client.PublicFeed.ReceivedSomething += (sender, s) =>
                    {
                        // Check that it matches the current instrument etc.
                    };
            }
            else
            {

            }
        }
    }
}