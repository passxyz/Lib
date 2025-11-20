using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Microsoft.Maui.Devices;

using HtmlAgilityPack;
using SkiaSharp;

using KPCLib;
using KeePassLib;
using KeePassLib.Security;
using KeePassLib.Utility;

using PassXYZLib.Resources;

namespace PassXYZLib
{
    /// <summary>
    /// ItemExtensions is a static class which defines a set of extension methods for Item.
    /// </summary>
    public static class PxItem
    {
        #region ItemIcon
        private static bool UrlExists(string url)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(url))
                {
                    // Check URL format
                    Uri uri = new Uri(url);
                    if (uri.Scheme.Contains("http") || uri.Scheme.Contains("https"))
                    {
                        var webRequest = WebRequest.Create(url);
                        webRequest.Method = "HEAD";
                        var webResponse = (HttpWebResponse)webRequest.GetResponse();
                        return webResponse.StatusCode == HttpStatusCode.OK;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex}");
            }
            return false;
        }

        private static string FormatUrl(string url, string baseUrl)
        {
            if (url.StartsWith("//")) { return ("http:" + url); }
            else if (url.StartsWith("/")) { return (baseUrl + url); }

            return url;
        }

        public static string? RetrieveFavicon(string url)
        {
            string? returnFavicon = null;

            // declare htmlweb and load html document
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(url);

            //1. 处理 apple-touch-icon 的情况
            var elementsAppleTouchIcon = htmlDoc.DocumentNode.SelectNodes("//link[contains(@rel, 'apple-touch-icon')]");
            if (elementsAppleTouchIcon != null && elementsAppleTouchIcon.Any())
            {
                var favicon = elementsAppleTouchIcon.First();
                var faviconUrl = FormatUrl(favicon.GetAttributeValue("href", null), url);
                if (UrlExists(faviconUrl))
                {
                    return faviconUrl;
                }
            }

            // 2. Try to get svg version
            var el = htmlDoc.DocumentNode.SelectSingleNode("/html/head/link[@rel='icon' and @href]");
            if (el != null)
            {
                try
                {
                    var faviconUrl = FormatUrl(el.Attributes["href"].Value, url);

                    if (UrlExists(faviconUrl))
                    {
                        return faviconUrl;
                    }
                }
                catch (WebException ex)
                {
                    Debug.WriteLine($"{ex}");
                }
            }

            // 3. 从页面的 HTML 中抓取
            var elements = htmlDoc.DocumentNode.SelectNodes("//link[contains(@rel, 'icon')]");
            if (elements != null && elements.Any())
            {
                var favicon = elements.First();
                var faviconUrl = FormatUrl(favicon.GetAttributeValue("href", null), url);
                if (UrlExists(faviconUrl))
                {
                    return faviconUrl;
                }
            }

            // 4. 直接获取站点的根目录图标
            try
            {
                var uri = new Uri(url);
                if (uri.HostNameType == UriHostNameType.Dns)
                {
                    var faviconUrl = string.Format("{0}://{1}/favicon.ico", uri.Scheme == "https" ? "https" : "http", uri.Host);
                    if (UrlExists(faviconUrl))
                    {
                        return faviconUrl;
                    }
                }
            }
            catch (UriFormatException ex)
            {
                Debug.WriteLine($"{ex}");
                return returnFavicon;
            }

            return returnFavicon;
        }

        /// <summary>
        /// Create a SKBitmap instance from a byte arrary
        /// </summary>
		/// <param name="pb">byte arraty</param>
		/// <param name="url">This is the url using to retrieve icon.</param>
        public static SKBitmap? LoadImage(byte[] pb, string? faviconUrl = null)
        {
            int w = 96, h = 96;
            if (DeviceInfo.Platform.Equals(DevicePlatform.Android))
            {
                w = 96; h = 96;
            }
            else if (DeviceInfo.Platform.Equals(DevicePlatform.iOS))
            {
                w = 64; h = 64;
            }
            else if (DeviceInfo.Platform.Equals(DevicePlatform.UWP))
            {
                w = 32; h = 32;
            }

            if (faviconUrl != null) 
            {
                if (faviconUrl.EndsWith(".ico") || faviconUrl.EndsWith(".png"))
                {
                    return GfxUtil.ScaleImage(GfxUtil.LoadImage(pb), w, h);
                }
                else if (faviconUrl.EndsWith(".svg"))
                {
                    return GfxUtil.LoadSvgImage(pb, w, h);
                }
                else { return null; }
            }
            else 
            {
                return GfxUtil.ScaleImage(GfxUtil.LoadImage(pb), w, h);
            }

        }

        public static ImageSource? GetImageSource(SKBitmap bitmap)
        {
            if (bitmap != null)
            {
                //SKImage image = SKImage.FromPixels(bitmap.PeekPixels());
                //SKData encoded = image.Encode();
                //Stream stream = encoded.AsStream();
                // There is a bug so we cannot use stream here. Please refer to the below link about the issue.
                // https://github.cohttps://github.com/xamarin/Xamarin.Forms/issues/11495m/xamarin/Xamarin.Forms/issues/11495
                return ImageSource.FromStream(() => SKImage.FromPixels(bitmap.PeekPixels()).Encode().AsStream());
            }
            else { return null; }
        }

        public static SKBitmap? GetBitmapByUrl(string url) 
        {
            try
            {
                string? faviconUrl = RetrieveFavicon(url);
                if (faviconUrl != null) 
                {
                    Uri uri = new Uri(faviconUrl);
                    WebClient myWebClient = new WebClient();
                    byte[] pb = myWebClient.DownloadData(faviconUrl);

                    return LoadImage(pb, faviconUrl);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex}");
            }
            return null;
        }

        public static ImageSource? GetImageByUrl(string url) 
        {
            SKBitmap? bitmap = GetBitmapByUrl(url);

            try
            {
                if (bitmap != null) { return GetImageSource(bitmap); }
            }
            catch (WebException ex)
            {
                Debug.WriteLine($"{ex}");
            }
            return null;
        }
        #endregion

        #region Item extensions

        public static Guid GetUuid(this Item item) 
        {
            PwUuid uuid = GetPwUuid(item);

            return (uuid != PwUuid.Zero) ? uuid.GetGuid() : default;
        }

        /// <summary>
        /// Get the PwUuid from the Item instance.
        /// Since we cannot define PwUuid in KPCLib.Item, we add extension methods here.
        /// </summary>
		/// <returns>Uuid</returns>
        public static PwUuid GetPwUuid(this Item item)
        {
            if (item is PwGroup group)
            {
                return group.Uuid;
            }
            else if (item is PwEntry entry)
            {
                return entry.Uuid;
            }
            else
            {
                return PwUuid.Zero;
            }
        }

        /// <summary>
        /// Set the Uuid from the Item instance.
        /// Since we cannot define Uuid in KPCLib.Item, we add extension methods here.
        /// </summary>
        /// <param name="uuid">Uuid of the item</param>
        public static void SetPwUuid(this Item item, PwUuid uuid)
        {
            if (item is PwGroup group)
            {
                group.Uuid = uuid;
            }
            else if (item is PwEntry entry)
            {
                entry.Uuid = uuid;
            }
        }

        /// <summary>
        /// Get the CustomIconUuid from the Item instance.
        /// Since we cannot define CustomIconUuid in KPCLib.Item, we add extension methods here.
        /// </summary>
		/// <returns>CustomIconUuid</returns>
        public static PwUuid GetCustomIconUuid(this Item item)
        {
            if (item is PwGroup group)
            {
                return group.CustomIconUuid;
            }
            else if(item is PwEntry entry)
            {
                return entry.CustomIconUuid;
            } 
            else 
            {
                return PwUuid.Zero;
            }
        }

        /// <summary>
        /// Set the CustomIconUuid from the Item instance.
        /// Since we cannot define CustomIconUuid in KPCLib.Item, we add extension methods here.
        /// </summary>
        /// <param name="uuid">PwUuid of the item icon</param>
        public static void SetCustomIconUuid(this Item item, PwUuid uuid)
        {
            if (item is PwGroup group)
            {
                group.CustomIconUuid = uuid;
            }
            else if (item is PwEntry entry)
            {
                entry.CustomIconUuid = uuid;
            }
        }

        /// <summary>
        /// Extension method of KPCLib.Item
        /// This method can be used to retrieve icon from a url.
        /// </summary>
        /// <param name="item">Instance of Item</param>
        /// <param name="url">This is the url using to retrieve icon.</param>
        public static void UpdateIcon(this Item item, string url)
        {
            var faviconUrl = RetrieveFavicon(url);

            try
            {
                var uri = new Uri(faviconUrl);
                WebClient myWebClient = new WebClient();
                byte[] pb = myWebClient.DownloadData(faviconUrl);

                SKBitmap bitmap = LoadImage(pb, faviconUrl);
                item.ImgSource = GetImageSource(bitmap);
            }
            catch (WebException ex)
            {
                Debug.WriteLine($"{ex}");
            }
        }

        /// <summary>
        /// This is an extension method of <c>Item</c> to set default icon
        /// </summary>
        // TODO: Retrieve color from static resource
        public static void SetDefaultIcon(this Item item)
        {
            item.ImgSource = new FontImageSource
            {
                FontFamily = "FontAwesomeRegular",
                Glyph = item.IsGroup ? FontAwesomeRegular.Folder : FontAwesomeRegular.File,
                Color = Microsoft.Maui.Graphics.Colors.Black
            };
        }

        public static async Task SetCustomIconByUrl(this Item item, string url)
        {
            if (string.IsNullOrWhiteSpace(url)) { return; }

            PasswordDb db = PasswordDb.Instance;
            try 
            {
                Uri uri = new Uri(url);
                PwCustomIcon old = db.GetCustomIcon(uri.Host);
                if (old == null)
                {
                    // If this is a new one, try to load it.
                    await Task.Run(() => AddNewIcon(item, url));
                }
                else
                {
                    // If an icon can be found, then use it.
                    item.SetCustomIconUuid(old.Uuid);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex}");
            }
        }

        public static PxIconType GetIconType(this Item item) 
        {
            // 1. Get built-in icon
            if (item.IsGroup)
            {
                // Group
                if (item is PwGroup group)
                {
                    if (group.CustomData.Exists(PxDefs.PxCustomDataIconName))
                    {
                        return PxIconType.PxBuiltInIcon;
                    }
                }
            }
            else
            {
                // Entry
                if (item is PwEntry entry)
                {
                    if (entry.CustomData.Exists(PxDefs.PxCustomDataIconName))
                    {
                        return PxIconType.PxBuiltInIcon;
                    }
                }
            }

            // 2. Get custom icon
            if (item.GetCustomIconUuid() != PwUuid.Zero) { return PxIconType.PxEmbeddedIcon; }
            return PxIconType.None;
        }

        /// <summary>
        /// Set the ImgSource of an item.
        /// </summary>
        public static void SetIcon(this Item item)
        {
            // 1. Get built-in icon
            if (item.IsGroup)
            {
                // Group
                if (item is PwGroup group)
                {
                    if (group.CustomData.Exists(PxDefs.PxCustomDataIconName))
                    {
                        string iconPath = System.IO.Path.Combine(PxDataFile.IconFilePath, group.CustomData.Get(PxDefs.PxCustomDataIconName));
                        item.ImgSource = ImageSource.FromFile(iconPath);
                        return;
                    }
                }
            }
            else
            {
                // Entry
                if (item is PwEntry entry)
                {
                    if (entry.CustomData.Exists(PxDefs.PxCustomDataIconName))
                    {
                        string iconPath = System.IO.Path.Combine(PxDataFile.IconFilePath, entry.CustomData.Get(PxDefs.PxCustomDataIconName));
                        item.ImgSource = ImageSource.FromFile(iconPath);
                        return;
                    }
                }
            }

            // 2. Get custom icon
            if (item.GetCustomIconUuid() != PwUuid.Zero)
            {
                PasswordDb db = PasswordDb.Instance;
                if(db != null)
                {
                    if(db.IsOpen)
                    {
                        PwCustomIcon customIcon = db.GetPwCustomIcon(item.GetCustomIconUuid());
                        if(customIcon != null) 
                        {
                            var pb = customIcon.ImageDataPng;
                            SKBitmap bitmap = LoadImage(pb);
                            item.ImgSource = GetImageSource(bitmap);
                            return;
                        }
                    }
                    else
                    {
                        Debug.WriteLine("SetIcon: PasswordDb is closed");
                    }
                }
                else
                {
                    Debug.WriteLine("SetIcon: No PasswordDb instance");
                }
            }

            // 3. Get font icon
            var icon = item.GetFontIcon();
            if(icon != null) 
            {
                item.ImgSource = new FontImageSource
                {
                    FontFamily = icon.FontFamily,
                    Glyph = icon.Glyph,
                    Color = Microsoft.Maui.Graphics.Colors.Black
                };
            }

            if (item.ImgSource == null)
            {
                SetDefaultIcon(item);
            }
        }

        /// <summary>
        /// Set the font icon of an item.
        /// <param name="item">Instance of Item</param>
        /// <param name="fontIcon">Instance of Item</param>
        /// </summary>
        public static void SetFontIcon(this Item item, PxFontIcon fontIcon) 
        {
            if (fontIcon == null)
            {
                ArgumentNullException.ThrowIfNull(fontIcon);
            }

            string jsonString = JsonSerializer.Serialize(fontIcon);
            if (!string.IsNullOrWhiteSpace(jsonString)) 
            {
                // Group
                if (item is PwGroup group)
                {
                    if (group.CustomData.Exists(PxDefs.PxCustomDataIconName)) 
                    {
                        // if there is an embedded icon, remove it
                        group.CustomData.Remove(PxDefs.PxCustomDataIconName);
                    }
                    group.CustomData.Set(PxDefs.PxCustomDataFontIcon, jsonString);
                    return;
                }

                if (item is PwEntry entry)
                {
                    if (entry.CustomData.Exists(PxDefs.PxCustomDataIconName)) 
                    {
                        // if there is an embedded icon, remove it
                        entry.CustomData.Remove(PxDefs.PxCustomDataIconName);
                    }
                    entry.CustomData.Set(PxDefs.PxCustomDataFontIcon, jsonString);
                    return;
                }
            }
            throw new NullReferenceException();
        }

        /// <summary>
        /// Get the font Icon of an item.
        /// </summary>
        /// <returns>an instance of PxFontIcon.</returns>
        public static PxFontIcon? GetFontIcon(this Item item)
        {
            PxFontIcon? fontIcon = default;

            if (item is PwGroup group)
            {
                if (group.CustomData.Exists(PxDefs.PxCustomDataFontIcon))
                {
                    // if there is an embedded icon, remove it
                    string jsonString = group.CustomData.Get(PxDefs.PxCustomDataFontIcon);
                    fontIcon = JsonSerializer.Deserialize<PxFontIcon?>(jsonString);
                }
            }

            if (item is PwEntry entry)
            {
                if (entry.CustomData.Exists(PxDefs.PxCustomDataFontIcon))
                {
                    // if there is an embedded icon, remove it
                    string jsonString = entry.CustomData.Get(PxDefs.PxCustomDataFontIcon);
                    fontIcon = JsonSerializer.Deserialize<PxFontIcon?>(jsonString);
                }
            }
            return fontIcon;
        }

        /// <summary>
        /// Get the Custom Icon in embedded data in HTML.
        /// </summary>
        /// <returns>Encoded data in base64 format.</returns>
        public static string GetCustomIcon(this Item item)
        {
            if (item.GetCustomIconUuid() != PwUuid.Zero)
            {
                PasswordDb db = PasswordDb.Instance;
                if (db != null)
                {
                    if (db.IsOpen)
                    {
                        PwCustomIcon customIcon = db.GetPwCustomIcon(item.GetCustomIconUuid());
                        if (customIcon != null)
                        {
                            var pb = customIcon.ImageDataPng;
                            return "data:image/png;base64," +
                                    Convert.ToBase64String(pb, Base64FormattingOptions.None);
                        }
                    }
                    else
                    {
                        Debug.WriteLine("GetCustomIcon: PasswordDb is closed");
                    }
                }
                else
                {
                    Debug.WriteLine("GetCustomIcon: No PasswordDb instance");
                }
            }
            else
            {
                if (item.IsGroup)
                {
                    return "folder.svg";
                }
                else
                {
                    return "file.svg";
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Add a new custom icon to the database and set the new icon as the icon for this item.
        /// If the url is null, try to get the url from the URL field in the item.
        /// </summary>
        /// <param name="item">an instance of Item. Must not be <c>null</c>.</param>	
        /// <param name="url">Url used to retrieve the new icon.</param>	
		/// <returns>an instance of PxIcon</returns>
        public static PxIcon? AddNewIcon(this Item item, string? url = null)
        {
            PasswordDb db = PasswordDb.Instance;
            if (url == null && !item.IsGroup)
            {
                // If the url is null, we try to get the url from the URL field in the item.
                if (item is PwEntry entry)
                {
                    url = entry.GetUrlField();
                }
            }

            if (db != null && !string.IsNullOrEmpty(url))
            {
                if (db.IsOpen)
                {
                    try
                    {
                        Uri uri = new Uri(url);
                        PwCustomIcon old = db.GetCustomIcon(uri.Host);
                        if (old == null)
                        {
                            SKBitmap? bitmap = GetBitmapByUrl(url);
                            if (bitmap != null)
                            {
                                PwUuid uuid = db.SaveCustomIcon(bitmap, uri.Host);
                                if (!uuid.Equals(PwUuid.Zero))
                                {
                                    item.SetCustomIconUuid(uuid);

                                    PxIcon icon = new PxIcon
                                    {
                                        IconType = PxIconType.PxEmbeddedIcon,
                                        Uuid = uuid,
                                        Name = uri.Host
                                    };

                                    Debug.WriteLine($"AddNewIcon: hostname={uri.Host}");
                                    return icon;
                                }
                            }
                        }
                        else
                        {
                            Debug.WriteLine($"AddNewIcon: Found an existing icon as {uri.Host}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"{ex}");
                    }
                }
            }
            Debug.WriteLine("AddNewIcon: cannot add the new icon.");
            return null;
        }

        public static PxPlainFields GetPlainFields(this Item item)
        {
            return item.IsGroup ? ((PwGroup)item).GetPlainFields() : ((PwEntry)item).GetPlainFields();
        }

        public static bool IsNotes(this Item item)
        {
            return !item.IsGroup && ((PwEntry)item).IsNotes();
        }

        public static Field AddField(this Item item, string key, string value, bool isProtected)
        {
            Field field = null;
            if (item is PwEntry entry)
            {
                if (key == PxDefs.PxCustomDataOtpUrl)
                {
                    // Add or update OTP URL
                    entry.SetOtpUrl(value);
                }
                else
                {
                    string k = entry.IsPxEntry() ? entry.EncodeKey(key) : key;

                    entry.Strings.Set(k, new ProtectedString(isProtected, value));
                    if (key.EndsWith(PwDefs.UrlField) && entry.CustomIconUuid.Equals(PwUuid.Zero))
                    {
                        // If this is a URL field and there is no custom icon, we can try to add a custom icon by URL.
                        entry.SetCustomIconByUrl(value);
                    }

                    if (entry.IsPxEntry())
                    {
                        field = new Field(key, value, isProtected, FieldIcons.GetImage, entry.EncodeKey(k));
                    }
                    else
                    {
                        field = new Field(k, value, isProtected);
                    }
                }
            }
            else
            {
                throw new ArgumentException("AddField: item type is not an entry!");
            }
            return field;
        }

        public static Field AddBinaryField(this Item item, string key, byte[] binaryArrage, string label = null)
        {
            Field field = default;
            if (item is PwEntry entry) 
            {
                if (binaryArrage != null)
                {
                    ProtectedBinary pb = new ProtectedBinary(false, binaryArrage);
                    entry.Binaries.Set(key, pb);

                    field = new Field(key, $"{label} {entry.Binaries.UCount}", false)
                    {
                        IsBinaries = true,
                        Binary = entry.Binaries.Get(key),
                        ImgSource = FieldIcons.GetImage(key)
                    };
                }
            }
            else
            {
                throw new ArgumentException("AddBinaryField: item type is not an entry!");
            }
            return field;
        }

        public static void UpdateField(this Item item, string key, string value, bool isProtected)
        {
            if (item is PwEntry entry)
            {
                string k = entry.IsPxEntry() ? entry.FindEncodeKey(key) : key;
                if (entry.Strings.Exists(k))
                {
                    entry.Strings.Set(k, new ProtectedString(isProtected, value));
                    if (key.EndsWith(PwDefs.UrlField) && entry.CustomIconUuid.Equals(PwUuid.Zero))
                    {
                        // If this is a URL field and there is no custom icon, we can try to add a custom icon by URL.
                        entry.SetCustomIconByUrl(value);
                    }
                }
                else
                {
                    throw new ArgumentException("UpdateField: field.Key does not exist!");
                }
            }
            else
            {
                throw new ArgumentException("UpdateField: item type is not an entry!");
            }
        }

        public static void DeleteField(this Item item, Field field) 
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            if (item is PwEntry entry) 
            {
                if (field.IsBinaries)
                {
                    if (entry.Binaries.Exists(field.Key))
                    {
                        if (entry.Binaries.Remove(field.Key))
                        {

                            Debug.WriteLine($"DeleteField: Attachment {field.Key} deleted.");
                        }
                        else
                        {
                            Debug.WriteLine($"DeleteField: Cannot delete Attachment {field.Key}.");
                        }
                    }
                }
                else
                {
                    string key = field.IsEncoded ? field.EncodedKey : field.Key;
                    if (entry.Strings.Exists(key))
                    {
                        if (entry.Strings.Remove(key))
                        {

                            Debug.WriteLine($"DeleteField: Field {field.Key} deleted.");
                        }
                        else
                        {
                            Debug.WriteLine($"DeleteField: Cannot delete field {field.Key}.");
                        }
                    }
                }
            }

        }

        public static List<Field> GetFields(this Item item) 
        {
            if(item is PwEntry entry)
            {
                return entry.GetFields(GetImage: FieldIcons.GetImage);
            }
            else { return null; }
        }

        public static string GetNotesInHtml(this Item item) 
        {
            if (item is PwEntry entry)
            {
                return entry.GetNotesInHtml();
            }
            else { return item.Notes; }
        }
        #endregion
    }

    /// <summary>
    /// PxField is a static class which defines a set of extension methods for Field.
    /// </summary>
    public static class PxField 
    {
        public static byte[] GetBinaryData(this Field field)
        {
            if(field.Binary is ProtectedBinary binary)
            {
                return binary.ReadData();
            }
            else { return null; }
        }
    }
}
