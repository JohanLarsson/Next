using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Next;
using Next.Dtos;
using NextView.Annotations;

namespace NextView
{
    public class NextVm : INotifyPropertyChanged
    {
        private readonly NextClient _client;
        private InstrumentList _selectedInstrumentList;
        private Account _selectedAccount;
        private AccountSummary _selectedAccountSummary;

        public NextVm(NextClient client)
        {
            _client = client;

            InstrumentLists = new ObservableCollection<InstrumentList>();
            Instruments = new ObservableCollection<InstrumentItem>();
            Accounts = new ObservableCollection<Account>();
            SelectedAccountLedgers = new ObservableCollection<Ledger>();
            SelectedAccountOrders = new ObservableCollection<OrderStatus>();
            SelectedAccountPositions = new ObservableCollection<Position>();
            SelectedAccountTrades = new ObservableCollection<Trade>();
        }

        public async Task Login()
        {
            var loginVm = new LoginVm();
            var loginWindow = new LoginWindow(loginVm);
            bool? showDialog = loginWindow.ShowDialog();
            await _client.Login(loginVm.Username, loginVm.Password);
            OnPropertyChanged("IsLoggedIn");
            List<InstrumentList> instrumentLists = await _client.Lists();
            instrumentLists.ForEach(InstrumentLists.Add);
            List<Account> accounts = await _client.Accounts();
            accounts.ForEach(Accounts.Add);
        }

        public bool IsLoggedIn { get { return _client.Session != null; } }

        public ObservableCollection<InstrumentList> InstrumentLists { get; private set; }

        public InstrumentList SelectedInstrumentList
        {
            get { return _selectedInstrumentList; }
            set
            {
                if (Equals(value, _selectedInstrumentList)) return;
                _selectedInstrumentList = value;
                OnPropertyChanged();
                if (_selectedInstrumentList == null)
                    return;
                UpdateCollection(Instruments, _client.ListItems(_selectedInstrumentList.Id));
                //Instruments.Clear();
                //Task<List<InstrumentItem>> listItems = _client.ListItems(_selectedInstrumentList.Id);
                //listItems.ContinueWith(items => items.Result.ForEach(Instruments.Add), TaskScheduler.FromCurrentSynchronizationContext());

            }
        }

        public ObservableCollection<InstrumentItem> Instruments { get; set; }

        public ObservableCollection<Account> Accounts { get; private set; }

        public Account SelectedAccount
        {
            get { return _selectedAccount; }
            set
            {
                if (Equals(value, _selectedAccount)) return;
                _selectedAccount = value;
                OnPropertyChanged();
                if (_selectedAccount == null)
                    return;


                Task<AccountSummary> accountSummary = _client.AccountSummary(_selectedAccount);
                accountSummary.ContinueWith(ant => SelectedAccountSummary = ant.Result);
                UpdateCollection(SelectedAccountLedgers, _client.AccountLedgers(_selectedAccount));
                UpdateCollection(SelectedAccountTrades, _client.AccountTrades(_selectedAccount));
                UpdateCollection(SelectedAccountPositions, _client.AccountPositions(_selectedAccount));
                UpdateCollection(SelectedAccountOrders, _client.AccountOrders(_selectedAccount));


            }
        }

        private void UpdateCollection<T>(ObservableCollection<T> collection, Task<List<T>> task)
        {
            collection.Clear();
            task.ContinueWith(ant => ant.Result.ForEach(collection.Add), TaskScheduler.FromCurrentSynchronizationContext());
        }

        public AccountSummary SelectedAccountSummary
        {
            get { return _selectedAccountSummary; }
            set
            {
                if (Equals(value, _selectedAccountSummary)) return;
                _selectedAccountSummary = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Ledger> SelectedAccountLedgers { get; private set; }
        public ObservableCollection<Trade> SelectedAccountTrades { get; private set; }
        public ObservableCollection<Position> SelectedAccountPositions { get; private set; }
        public ObservableCollection<OrderStatus> SelectedAccountOrders { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
