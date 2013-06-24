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
            InstrumentLists= new ObservableCollection<InstrumentList>();
            Instruments= new ObservableCollection<InstrumentItem>();
            Accounts= new ObservableCollection<Account>();
            Account= new AccountVm(_client,null);
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
                Instruments.UpdateCollection( _client.ListItems(_selectedInstrumentList.Id));
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
                Account.Account = _selectedAccount;
            }
        }

        public AccountVm Account { get; private set; }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
