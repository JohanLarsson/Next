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
        private readonly ObservableCollection<InstrumentListVm> _instrumentLists = new ObservableCollection<InstrumentListVm>();

        private readonly ObservableCollection<Account> _accounts = new ObservableCollection<Account>();

        private InstrumentListVm _selectedInstrumentList;
        private InstrumentVm _selectedInstrument;

        public NextVm(NextClient client)
        {
            _client = client;
            PublicFeed = new FeedVm(_client.PublicFeed);
            PrivateFeed = new FeedVm(_client.PrivateFeed);
            _client.LoggedInChanged += async (_, e) =>
                {
                    OnPropertyChanged("IsLoggedIn");
                    if (IsLoggedIn)
                    {
                        List<InstrumentList> instrumentLists = await _client.Lists();
                        foreach (var instrumentList in instrumentLists.OrderBy(x => x.Name))
                        {
                            InstrumentLists.Add(new InstrumentListVm(instrumentList, _client));
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
            Account = new AccountVm(_client, null);
            var loginVm = new LoginVm();
            if (loginVm.Username != null && loginVm.Password != null)
            {
                _client.Login(loginVm.Username, loginVm.Password);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public FeedVm PrivateFeed { get; private set; }

        public FeedVm PublicFeed { get; private set; }

        public bool IsLoggedIn { get { return _client.Session != null; } }

        public ObservableCollection<InstrumentListVm> InstrumentLists
        {
            get { return _instrumentLists; }
        }

        public InstrumentListVm SelectedInstrumentList
        {
            get { return _selectedInstrumentList; }
            set
            {
                if (Equals(value, _selectedInstrumentList))
                {
                    return;
                }
                if (_selectedInstrumentList != null)
                {
                    _selectedInstrumentList.IsSelected = false;
                }
                _selectedInstrumentList = value;
                _selectedInstrumentList.IsSelected = true;
                OnPropertyChanged();
            }
        }

        public InstrumentVm SelectedInstrument
        {
            get { return _selectedInstrument; }
            set
            {
                if (Equals(value, _selectedInstrument)) return;
                _selectedInstrument = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Account> Accounts
        {
            get { return _accounts; }
        }

        public AccountVm Account { get; private set; }

        public async Task Login()
        {
            var loginVm = new LoginVm();
            var loginWindow = new LoginWindow(loginVm);
            loginWindow.ShowDialog();
            await _client.Login(loginVm.Username, loginVm.Password);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
