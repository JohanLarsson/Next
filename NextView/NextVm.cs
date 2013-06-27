using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Next;
using Next.Dtos;
using NextView.Annotations;

namespace NextView
{
    public class NextVm : INotifyPropertyChanged
    {
        private readonly NextClient _client;
        private InstrumentList _selectedInstrumentList;
        private InstrumentItem _selectedInstrument;
        public NextVm(NextClient client)
        {
            _client = client;
            PublicFeed= new FeedVm(_client.PublicFeed);
            PrivateFeed= new FeedVm(_client.PrivateFeed);
            _client.LoggedInChanged += async (_, e) =>
                {
                    OnPropertyChanged("IsLoggedIn");
                    if (IsLoggedIn)
                    {
                        List<InstrumentList> instrumentLists = await _client.Lists();
                        foreach (var instrumentList in instrumentLists.OrderBy(x=>x.Name))
                        {
                            InstrumentLists.Add(instrumentList);
                        }
                        List<Account> accounts = await _client.Accounts();
                        accounts.ForEach(Accounts.Add);
                    }
                };
            Accounts.CollectionChanged += (o, e) =>
                {
                    if (Account.Account != null || Accounts.Count > 1)
                        return;
                    Account.Account = Accounts.First();
                };
            Account= new AccountVm(_client,null);
            var loginVm = new LoginVm();
            if (loginVm.Username!=null && loginVm.Password!=null)
            {
                _client.Login(loginVm.Username, loginVm.Password);
            }
        }

        public FeedVm PrivateFeed { get; private set; }

        public FeedVm PublicFeed { get; private set; }

        public async Task Login()
        {
            var loginVm = new LoginVm();
            var loginWindow = new LoginWindow(loginVm);
            bool? showDialog = loginWindow.ShowDialog();
            await _client.Login(loginVm.Username, loginVm.Password);
        }

        public bool IsLoggedIn { get { return _client.Session != null; } }

        private readonly ObservableCollection<InstrumentList> _instrumentLists = new ObservableCollection<InstrumentList>();
        public ObservableCollection<InstrumentList> InstrumentLists
        {
            get { return _instrumentLists; }
        }

        public InstrumentList SelectedInstrumentList
        {
            get { return _selectedInstrumentList; }
            set
            {
                if (Equals(value, _selectedInstrumentList)) return;
                _selectedInstrumentList = value;
                OnPropertyChanged();
                if (_selectedInstrumentList == null)
                {
                    Instruments.Clear();
                    return;
                }
                Instruments.UpdateCollection( _client.ListItems(_selectedInstrumentList.Id));
            }
        }

        private readonly ObservableCollection<InstrumentItem> _instruments = new ObservableCollection<InstrumentItem>();
        public ObservableCollection<InstrumentItem> Instruments
        {
            get { return _instruments; }
        }

        public InstrumentItem SelectedInstrument
        {
            get { return _selectedInstrument; }
            set
            {
                if (Equals(value, _selectedInstrument)) return;
                _selectedInstrument = value;
                OnPropertyChanged();
                UpdateInstrument();
                //if (_selectedInstrument == null)
                //    Ticks.Clear();
                //else
                //{

                //}

            }
        }

        public async Task UpdateInstrument()
        {
            Instrument = SelectedInstrument == null 
                ? null 
                : await _client.InstrumentSearch(_selectedInstrument);
        }

        private InstrumentMatch _instrument;

        public InstrumentMatch Instrument
        {
            get { return _instrument; }
            set
            {
                if (Equals(value, _instrument)) return;
                _instrument = value;
                OnPropertyChanged();
            }
        }

        private readonly ObservableCollection<Account> _accounts = new ObservableCollection<Account>();
        public ObservableCollection<Account> Accounts
        {
            get { return _accounts; }
        }

        public AccountVm Account { get; private set; }

        //private readonly ObservableCollection<EodPoint> _ticks = new ObservableCollection<EodPoint>();
        //public ObservableCollection<EodPoint> Ticks
        //{
        //    get { return _ticks; }
        //}

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
