using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

using Microsoft.Maui;
using Microsoft.Maui.Controls;

using KeePassLib;
using KPCLib;
using PassXYZLib.Resources;

namespace PassXYZLib
{
    public enum PxIconType
    {
        None = 0,
        PxBuiltInIcon,
        PxEmbeddedIcon,
        PxFontIcon
    }

    public record PxFontIcon
    {
        public required string FontFamily { get; init; }
        public required string Glyph { get; init; }
    }

    public class PxIcon : INotifyPropertyChanged
    {
        public PxIconType IconType = PxIconType.None;

        public string Type => IconType.ToString();

        // 1. Built-in icon
        private string? _filename = null;
        public string? FileName
        {
            get => _filename;
            set => _ = SetProperty(ref _filename, value);
        }

        public PwUuid Uuid = PwUuid.Zero;

        // 2. Custom icon
        private string? _name = null;
        public string? Name
        {
            get => _name;
            set => _ = SetProperty(ref _name, value);
        }

        // 3. Font icon
        private PxFontIcon? _fontIcon = default;
        public PxFontIcon? FontIcon 
        {
            get => _fontIcon;
            set => _ = SetProperty(ref _fontIcon, value);
        }

        private ImageSource? _imgSource = null;
        public ImageSource? ImgSource
        {
            get => _imgSource;
            set => SetProperty(ref _imgSource, value);
        }
        public PxIcon() 
        {
            _fontIcon = new() { 
                FontFamily = "FontAwesomeRegular", 
                Glyph = FontAwesomeRegular.File };

            _imgSource = new FontImageSource
            {
                FontFamily = FontIcon.FontFamily,
                Glyph = FontIcon.Glyph,
                Color = Microsoft.Maui.Graphics.Colors.Black
            };
        }

        #region INotifyPropertyChanged
        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action? onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler? PropertyChanged = null;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
