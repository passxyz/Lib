using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;

using PassXYZLib.Resources;

namespace PassXYZLib
{
#if PASSXYZ_CLOUD_SERVICE
    public enum PxCloudType
    {
        OneDrive,
        WebDav,
        SFTP,
        FTP,
        SMB
    }

    public class PxCloudConfigData : INotifyPropertyChanged
    {
        private string _username = PxCloudConfig.Username;
        public string Username
        {
            get
            {
                return _username;
            }

            set
            {
                _username = value;
                OnPropertyChanged("Username");
            }
        }
        private string _password = PxCloudConfig.Password;
        public string Password
        {
            get
            {
                return _password;
            }

            set
            {
                _password = value;
                OnPropertyChanged("Password");
            }
        }
        private string _hostname = PxCloudConfig.Hostname;
        public string Hostname 
        {
            get
            {
                return _hostname;
            }

            set
            {
                _hostname = value;
                OnPropertyChanged("Hostname");
            }
        }
        private int _port = PxCloudConfig.Port;
        /// <summary>
        /// Gets connection port.
        /// </summary>
        /// <value>
        /// The connection port. The default value is 22.
        /// </value>
        public int Port
        {
            get
            {
                return _port;
            }

            set
            {
                _port = value;
                OnPropertyChanged("Port");
            }
        }
        private string _remoteHomePath = PxCloudConfig.RemoteHomePath;
        public string RemoteHomePath 
        {
            get
            {
                return _remoteHomePath;
            }

            set
            {
                _remoteHomePath = value;
                OnPropertyChanged("RemoteHomePath");
            }
        }
        private bool _isEnabled = PxCloudConfig.IsEnabled;
        public bool IsEnabled 
        {
            get
            {
                return _isEnabled;
            }

            set
            {
                _isEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }
        private string _configMessage = PxRes.message_id_cloud_config;
        public string ConfigMessage 
        {
            get => _configMessage;
            set 
            {
                _configMessage = value;
                OnPropertyChanged("ConfigMessage");
            }
        }

        public bool IsConfigured
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Username)
                    && !string.IsNullOrWhiteSpace(Password)
                    && !string.IsNullOrWhiteSpace(Hostname)
                    && !string.IsNullOrWhiteSpace(RemoteHomePath);
            }
        }

        #region INotifyPropertyChanged
        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion    
    }

    /// <summary>
    /// PxCloudConfig implements cloud configuration for synchronization
    /// </summary>
    public static class PxCloudConfig
    {
        public const string DEFAULT_USERNAME = "pxtestuser";
        public const string DEFAULT_PASSWORD = "pxtestpw";
        public const string DEFAULT_HOSTNAME = "";
        public const int DEFAULT_PORT = 22;
        public const string DEFAULT_REMOTEHOMEPATH = "data/";
        public static PxCloudType CurrentServiceType { get => PxCloudType.SFTP; }

        /// <summary>
        /// Gets username from <c>Preference</c>.
        /// </summary>
        /// <value>
        /// The username in <c>Preference</c> or the default value.
        /// </value>
        public static string Username
        {
            get 
            { 
                try { return Preferences.Get(nameof(PxCloudConfig) + nameof(Username), DEFAULT_USERNAME); }
                catch (NotImplementedException) { return DEFAULT_USERNAME; }
            }
            set
            {
                try { Preferences.Set(nameof(PxCloudConfig) + nameof(Username), value); }
                catch (NotImplementedException) { Debug.WriteLine($"Set to {DEFAULT_USERNAME}"); }
            }
        }

        /// <summary>
        /// Gets password from <c>Preference</c>.
        /// </summary>
        /// <value>
        /// The password in <c>Preference</c> or the default value.
        /// </value>
        public static string Password 
        {
            get
            {
                try { return Preferences.Get(nameof(PxCloudConfig) + nameof(Password), DEFAULT_PASSWORD); }
                catch (NotImplementedException) { return DEFAULT_PASSWORD; }
            }
            set
            {
                try { Preferences.Set(nameof(PxCloudConfig) + nameof(Password), value); }
                catch (NotImplementedException) { Debug.WriteLine($"Set to {DEFAULT_PASSWORD}"); }
            }
        }

        private static string _hostname = DEFAULT_HOSTNAME;
        /// <summary>
        /// Gets hostname from <c>Preference</c>.
        /// </summary>
        /// <value>
        /// The hostname in <c>Preference</c> or the default value.
        /// </value>
        public static string Hostname 
        {
            get
            {
                try { return Preferences.Get(nameof(PxCloudConfig) + nameof(Hostname), DEFAULT_HOSTNAME); }
                catch (NotImplementedException) { return _hostname; }
            }
            set 
            {
                try { Preferences.Set(nameof(PxCloudConfig) + nameof(Hostname), value); }
                catch (NotImplementedException) { _hostname = value; }
            }
        }

        /// <summary>
        /// Gets connection port from <c>Preference</c>.
        /// </summary>
        /// <value>
        /// The connection port from <c>Preference</c> or the default port.
        /// </value>
        public static int Port
        {
            get
            {
                try { return Preferences.Get(nameof(PxCloudConfig) + nameof(Port), DEFAULT_PORT); }
                catch (NotImplementedException) { return DEFAULT_PORT; }
            }
            set
            {
                try { Preferences.Set(nameof(PxCloudConfig) + nameof(Port), value); }
                catch (NotImplementedException) { Debug.WriteLine($"Set to {DEFAULT_PORT}"); }
            }
        }

        /// <summary>
        /// Gets remote home path from <c>Preference</c>.
        /// </summary>
        /// <value>
        /// The remote home path from <c>Preference</c> or return the default value.
        /// </value>
        public static string RemoteHomePath 
        {
            get
            {
                try { return Preferences.Get(nameof(PxCloudConfig) + nameof(RemoteHomePath), DEFAULT_REMOTEHOMEPATH); }
                catch (NotImplementedException) { return DEFAULT_REMOTEHOMEPATH; }
            }
            set
            {
                try { Preferences.Set(nameof(PxCloudConfig) + nameof(RemoteHomePath), value); }
                catch (NotImplementedException) { Debug.WriteLine($"Set to {DEFAULT_REMOTEHOMEPATH}"); }
            }
        }

        private static bool _isEnabled = false;
        /// <summary>
        /// Whether synchronization is enabled
        /// </summary>
        /// <value>
        /// Gets the value from <c>Preference</c> or return the default value.
        /// </value>
        /// <remarks>
        /// The <see cref="IsEnabled"/> is a <see langword="bool"/>
        /// that you check whether the synchronization is enabled.
        /// <para>
        /// Note that <c>_isEnabled</c> is used when the implementation is not available.
        /// </para>
        /// </remarks>
        public static bool IsEnabled
        {
            get
            {
                try { return Preferences.Get(nameof(PxCloudConfig) + nameof(IsEnabled), false); }
                catch (NotImplementedException) { return _isEnabled; }
            }
            set
            {
                try { Preferences.Set(nameof(PxCloudConfig) + nameof(IsEnabled), value); }
                catch (NotImplementedException) { _isEnabled = value; }
            }
        }

        public static bool IsConfigured => !string.IsNullOrWhiteSpace(Username)
                    && !string.IsNullOrWhiteSpace(Password)
                    && !string.IsNullOrWhiteSpace(Hostname)
                    && !string.IsNullOrWhiteSpace(RemoteHomePath);

        public static void SetConfig(PxCloudConfigData configData)
        {
            Username = configData.Username;
            Password = configData.Password;
            Hostname = configData.Hostname;
            Port = configData.Port;
            RemoteHomePath = configData.RemoteHomePath;
            IsEnabled = configData.IsEnabled;
        }

        private static PxSFtp pxSFtp = null;
        public static ICloudServices<PxUser> GetCloudServices()
        {
            if (pxSFtp == null)
            {
                pxSFtp = new PxSFtp();
            }
            return pxSFtp;
        }
    }

    public class PxFileInfo : INotifyPropertyChanged
    {
        public string IconPath { get; set; }
        public PxSyncFileType FileType { get; set; } = PxSyncFileType.Local;
        public string FileTypeComments { get; set; }
        private DateTime _lastWriteTime;
        public DateTime LastWriteTime
        {
            get => _lastWriteTime;
            set
            {
                _lastWriteTime = value;
                OnPropertyChanged("LastWriteTime");
            }
        }
        private long _length;
        public long Length
        {
            get => _length;
            set
            {
                _length = value;
                OnPropertyChanged("Length");
            }
        }

        #region INotifyPropertyChanged
        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    public interface ICloudServices<T>
    {
        Task LoginAsync();
        Task<string> DownloadFileAsync(string filename, bool isMerge = false);
        Task UploadFileAsync(string filename);
        Task<bool> DeleteFileAsync(string filename);
        Task<IEnumerable<T>> LoadRemoteUsersAsync();
        Task<IEnumerable<T>> SynchronizeUsersAsync();
        Task<IEnumerable<PxUser>> SynchronizeAsync();
        void Logout();
        bool IsConnected { get; }
        bool IsSynchronized { get; }
        bool IsSshOperationTimeout { get; set; }
        bool IsBusyToLoadUsers { get; set; }
    }
#endif // PASSXYZ_CLOUD_SERVICE
}
