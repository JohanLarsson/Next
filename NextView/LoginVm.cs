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
        private static byte[] DefaultKey = Encoding.Unicode.GetBytes("NEXTAPI");
        private string _username;
        private string _password;

        private bool rememberMe;

        public LoginVm()
        {
            var settings = Properties.Settings.Default;
            if (settings != null && settings.User != null)
            {
                var userSetting = settings.User;
                Username = string.IsNullOrEmpty(userSetting.UserName)
                               ? ""
                               : Decrypt(userSetting.UserName, DefaultKey);
                Password = string.IsNullOrEmpty(userSetting.Password)
                               ? ""
                               : Decrypt(userSetting.Password, DefaultKey);
                RememberMe = userSetting.RememberMe;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

        public bool RememberMe
        {
            get
            {
                return this.rememberMe;
            }
            set
            {
                if (value.Equals(this.rememberMe))
                {
                    return;
                }
                this.rememberMe = value;
                this.OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Save()
        {
            var userName = EncryptString(Username, DefaultKey);
            var password = EncryptString(Password, DefaultKey);

            Properties.Settings.Default.User = new UserSetting
                                                   {
                                                       UserName = userName,
                                                       Password = password,
                                                       RememberMe = rememberMe
                                                   };
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
