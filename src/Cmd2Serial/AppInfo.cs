// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Cmd2Serial
{
    public static class AppInfo
    {
        public static Assembly Assembly => _assembly ??= Assembly.GetExecutingAssembly();
        private static Assembly _assembly = null;

        public static string Name => _name ??= Assembly.GetName().Name;
        private static string _name = null;

        public static string Version
        {
            get
            {
                if (null == _version)
                {
                    Version vers = Assembly.GetName().Version;
                    _version = vers.Build == 0 ? $"{vers.Major}.{vers.Minor}" : $"{vers.Major}.{vers.Minor}.{vers.Build}";
                }
                return _version;
            }
        }
        private static string _version = null;

        public static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static readonly bool IsMacOS = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        public static readonly bool IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }
}
