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
    public class AccountVm : INotifyPropertyChanged
    {
        private readonly NextClient _client;
        private Account _account;
        private AccountSummary _summary;

        public AccountVm(NextClient client, Account account)
        {
            _client = client;
            Account = account;
            Ledgers = new ObservableCollection<Ledger>();
            Trades = new ObservableCollection<Trade>();
            Positions = new ObservableCollection<Position>();
            Orders = new ObservableCollection<OrderStatus>();
        }

        public Account Account
        {
            get { return _account; }
            set
            {
                if (Equals(value, _account)) return;
                _account = value;
                OnPropertyChanged();
                UpdateSummary();
                UpdateLedgers();
                UpdateOrders();
                UpdatePositions();
                UpdateTrades();
            }
        }

        public async Task UpdateSummary()
        {
            Summary = Account == null 
                ? null 
                : await _client.AccountSummary(Account);
        }

        public AccountSummary Summary
        {
            get { return _summary; }
            set
            {
                if (Equals(value, _summary)) return;
                _summary = value;
                OnPropertyChanged();
            }
        }

        public void UpdateLedgers()
        {
            if (Account == null)
                Ledgers.Clear();
            else
                Ledgers.UpdateCollection(_client.AccountLedgers(Account));
        }

        public ObservableCollection<Ledger> Ledgers { get; private set; }

        public void UpdateTrades()
        {
            if (Account == null)
                Trades.Clear();
            else
                Trades.UpdateCollection(_client.AccountTrades(Account));
        }

        public ObservableCollection<Trade> Trades { get; private set; }

        public void UpdatePositions()
        {
            if (Account == null)
                Positions.Clear();
            else
                Positions.UpdateCollection(_client.AccountPositions(Account));
        }

        public ObservableCollection<Position> Positions { get; private set; }

        public void UpdateOrders()
        {
            if (Account == null)
                Orders.Clear();
            else
                Orders.UpdateCollection(_client.AccountOrders(Account));
        }

        public ObservableCollection<OrderStatus> Orders { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
