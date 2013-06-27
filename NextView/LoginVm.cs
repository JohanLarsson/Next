using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NextView.Annotations;

namespace NextView
{
    public class LoginVm : INotifyPropertyChanged
    {
        public LoginVm()
        {
            Username = string.IsNullOrEmpty(Properties.Settings.Default.Username)
                           ? ""
                           : Decrypt(Properties.Settings.Default.Username, DefaultKey);
            Password = string.IsNullOrEmpty(Properties.Settings.Default.Password)
                           ? ""
                           : Decrypt(Properties.Settings.Default.Password, DefaultKey);
        }
        private static byte[] DefaultKey = Encoding.Unicode.GetBytes("NEXTAPI");
        private string _username;
        public string Username
        {
            get { return _username; }
            set
            {
                if (value == _username) return;
                _username = value;
                OnPropertyChanged();
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                if (value == _password) return;
                _password = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Save()
        {
            Properties.Settings.Default.Username = EncryptString(Username, DefaultKey);
            Properties.Settings.Default.Password = EncryptString(Password, DefaultKey);
            Properties.Settings.Default.Save();
        }


        private string EncryptString(string input, byte[] salt)
        {
            byte[] encryptedData = ProtectedData.Protect(Encoding.Unicode.GetBytes(input), salt, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptedData);
        }

        private string Decrypt(string input, byte[] salt)
        {
            byte[] decryptedData = ProtectedData.Unprotect(Convert.FromBase64String(input), salt, DataProtectionScope.CurrentUser);
            return Encoding.Unicode.GetString(decryptedData);
        }
    }
}
