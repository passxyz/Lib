using Renci.SshNet.Common;
using Renci.SshNet;
using System.Security.Cryptography;
using Microsoft.Maui.Storage;
using System.IO;

namespace PassXYZLib
{
    public class FileSynchronizationService
    {
        private string host;
        private string username;
        private string password;
        private int port;
        private string privateKeyFilePath = default;

        public FileSynchronizationService(string host, string username, string password, int port = 22)
        {
            this.host = host;
            this.username = username;
            this.password = password;
            this.port = port;
        }

        void SetPrivateKeyFilePath(string privateKeyFilePath)
        {
            this.privateKeyFilePath = privateKeyFilePath;
        }

        public void SynchronizeFiles(string localPath, string remotePath)
        {
            using (var client = CreateSftpClient())
            {
                client.Connect();

                SynchronizeRemoteFiles(client, localPath, remotePath);
                SynchronizeLocalFiles(client, localPath, remotePath);

                client.Disconnect();
            }
        }

        private SftpClient CreateSftpClient()
        {
            if (!string.IsNullOrEmpty(privateKeyFilePath))
            {
                var privateKeyFile = new PrivateKeyFile(privateKeyFilePath);
                var privateKeyAuthMethod = new PrivateKeyAuthenticationMethod(username, privateKeyFile);
                // Todo: need to fix this.
                return new SftpClient(host, port, username, privateKeyFilePath);
            }
            else
            {
                return new SftpClient(host, port, username, password);
            }
        }

        private void SynchronizeRemoteFiles(SftpClient client, string localPath, string remotePath)
        {
            var remoteFiles = client.ListDirectory(remotePath);
            foreach (var remoteFile in remoteFiles)
            {
                if (!remoteFile.IsDirectory)
                {
                    var localFilePath = Path.Combine(localPath, remoteFile.Name);
                    if (!File.Exists(localFilePath))
                    {
                        using (var fileStream = File.OpenWrite(localFilePath))
                        {
                            client.DownloadFile(remoteFile.FullName, fileStream);
                        }
                    }
                    else 
                    {
                        var remoteFilePath = remotePath + "/" + remoteFile.Name;
                        if (IsUnchanged(client, localFilePath, remoteFilePath))
                        {
                            continue;
                        }

                        if(IsRemoteFileNewer(localFilePath, remoteFile.LastWriteTime))
                        {
                            using (var fileStream = File.OpenWrite(localFilePath))
                            {
                                client.DownloadFile(remoteFile.FullName, fileStream);
                            }
                        }

                        if (IsLocalFileNewer(localFilePath, remoteFile.LastWriteTime))
                        {
                            using (var fileStream = File.OpenRead(localFilePath))
                            {
                                client.UploadFile(fileStream, remoteFile.FullName);
                            }
                        }
                    }
                }
            }
        }

        private void SynchronizeLocalFiles(SftpClient client, string localPath, string remotePath)
        {
            var localFiles = Directory.GetFiles(localPath);
            foreach (var localFile in localFiles)
            {
                var fileName = Path.GetFileName(localFile);
                var remoteFilePath = remotePath + "/" + fileName;
                if (!IsFileExistsRemotely(client, remoteFilePath))
                {
                    using (var fileStream = new FileStream(localFile, FileMode.Open))
                    {
                        client.UploadFile(fileStream, remoteFilePath);
                    }
                }
                else
                {
                    var remoteFile = client.GetAttributes(remoteFilePath);
                    if (IsUnchanged(client, localFile, remoteFilePath))
                    {
                        continue;
                    }

                    if (IsLocalFileNewer(localFile, remoteFile.LastWriteTime))
                    {
                        using (var fileStream = new FileStream(localFile, FileMode.Open))
                        {
                            client.UploadFile(fileStream, remoteFilePath);
                        }
                    }
                    else if (IsRemoteFileNewer(localFile, remoteFile.LastWriteTime))
                    {
                        using (var fileStream = File.OpenWrite(localFile))
                        {
                            client.DownloadFile(remoteFilePath, fileStream);
                        }
                    }
                }
            }
            
/*            var localDirectories = Directory.GetDirectories(localPath);
            foreach (var localDirectory in localDirectories)
            {
                var directoryName = Path.GetFileName(localDirectory);
                var remoteDirectoryPath = remotePath + "/" + directoryName;
                if (!IsDirectoryExistsRemotely(client, remoteDirectoryPath))
                {
                    client.CreateDirectory(remoteDirectoryPath);
                }
                SynchronizeLocalFiles(client, localDirectory, remoteDirectoryPath);
            }
*/        }

        private bool IsFileExistsRemotely(SftpClient client, string remoteFilePath)
        {
            try
            {
                client.GetAttributes(remoteFilePath);
                return true;
            }
            catch (SftpPathNotFoundException)
            {
                return false;
            }
        }

        private static string CalculateSHA256FromStream(FileStream stream)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                var hash = sha256Hash.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        private static string CalculateSHA256FromArray(byte[] data)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                var hash = sha256Hash.ComputeHash(data);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        private bool IsUnchanged(SftpClient client, string localFilePath, string remoteFilePath)
        {
            string localHash;
            string remoteHash;

            using (FileStream localStream = File.OpenRead(localFilePath)) 
            {
                localHash = CalculateSHA256FromStream(localStream);
            }

            using (var remoteFileStream = new MemoryStream())
            {
                client.DownloadFile(remoteFilePath, remoteFileStream);
                var fileData = remoteFileStream.ToArray();
                remoteHash = CalculateSHA256FromArray(fileData);
            }

            return localHash.Equals(remoteHash);
        }

        private bool IsLocalFileNewer(string localFilePath, DateTime remoteFileModificationTime)
        {
            var localFileModificationTime = File.GetLastWriteTimeUtc(localFilePath);
            return localFileModificationTime > remoteFileModificationTime;
        }

        private bool IsRemoteFileNewer(string localFilePath, DateTime remoteFileModificationTime)
        {
            var localFileModificationTime = File.GetLastWriteTimeUtc(localFilePath);
            return localFileModificationTime < remoteFileModificationTime;
        }

/*        private bool IsDirectoryExistsRemotely(SftpClient client, string remoteDirectoryPath)
        {
            try
            {
                var remoteFiles = client.ListDirectory(remoteDirectoryPath);
                return true;
            }
            catch (SftpPathNotFoundException)
            {
                return false;
            }
        }
*/    }
}
