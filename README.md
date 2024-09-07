# PassXYZLib

**PassXYZLib** is a .NET MAUI class library to extend *KeePassLib* for *KeePass* compatible password manager apps.
**KPCLib** is a .NET Standard build of KeePassLib which is a library of KeePass. With **PassXYZLib** and **KPCLib**, we can build KeePass compatible applications on all major platforms. A command line application [KPCLibPy][1] is built using **KPCLib** and [Python.NET][2].

**KPCLib** 1.x.x includes both KeePassLib and PassXYZLib, but they become separate libraries from version 2.x.x.
- **KeePassLib** - This is the port of the original KeePassLib under project `KPCLib`. It is a .NET standard library.
- **PassXYZLib** - This is the enhancement built on top of KeePassLib, such as localization, OTP etc. Version 1.x.x is built for Xamarin.Forms and 2.0.0 or above is built for .NET MAUI.

### Setup
* Available on NuGet: [![NuGet](https://img.shields.io/nuget/v/Xam.Plugin.Media.svg?label=NuGet)](https://www.nuget.org/packages/passxyzlib)
* Version `1.x.x` is built using Visual Studio 2019.
* Version `2.x.x` or above is built using Visual Studio 2022 and .NET 7.
* Version `3.x.x` or above is built using Visual Studio 2022 and .NET 8.


[1]: https://github.com/passxyz/KPCLibPy
[2]: https://github.com/pythonnet/pythonnet