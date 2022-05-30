# PassXYZLib

**PassXYZLib** is a .NET MAUI class library to extend *KeePassLib* for *KeePass* compatible password manager apps.
**KPCLib** is a .NET Standard build of KeePassLib which is a library of KeePass. With **PassXYZLib** and **KPCLib**, we can build KeePass compatible applications on all major platforms. A command line application [KPCLibPy][1] is built using **KPCLib** and [Python.NET][2].

**KPCLib** 1.x.x includes both KeePassLib and PassXYZLib, but they become separate libraries from version 2.x.x.
- **KeePassLib** - This is the port of the original KeePassLib under project `KPCLib`. It is a .NET standard library.
- **PassXYZLib** - This is the enhancement built on top of KeePassLib, such as localization, OTP etc. Version 1.x.x is built for Xamarin.Forms and 2.0.0 or above is built for .NET MAUI.

### Setup
* Available on NuGet: [![NuGet](https://img.shields.io/nuget/v/Xam.Plugin.Media.svg?label=NuGet)](https://www.nuget.org/packages/KPCLib)
* Build status: [![Build status](https://ci.appveyor.com/api/projects/status/4py18evnh0xxxvi1?svg=true)](https://ci.appveyor.com/project/shugaoye/kpclib-bccwi)
* [Branch strategy](https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow)
* Version `1.x.x` is built using Visual Studio 2019 and version `2.0.0` or above is built using Visual Studio 2022.


[1]: https://github.com/passxyz/KPCLibPy
[2]: https://github.com/pythonnet/pythonnet