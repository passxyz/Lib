﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using System;
using System.Windows.Input;

namespace UITest.ViewModels
{
    public partial class AboutViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? title = "About";

        [RelayCommand]
        private async Task OpenWeb()
        {
            await Browser.OpenAsync("https://learn.microsoft.com/en-us/dotnet/maui/?view=net-maui-7.0");
        }
    }
}