using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Get_Process_Manager
{
    public static class IconHelper
    {
        [DllImport("Shell32.dll",
            CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.Winapi,
            EntryPoint = "ExtractAssociatedIconW")]
        static extern IntPtr ApiExtractAssociatedIcon(IntPtr hInst, String pszIconPath, ref short piIcon);

        [DllImport("Kernel32.dll")]
        static extern IntPtr GetModuleHandle(String lpModuleName);

        public static Icon ExtractAssociatedIcon(string path)
        {
            IntPtr hMod = GetModuleHandle(null);
            short nIcon = 0;
            IntPtr hIcon = ApiExtractAssociatedIcon(hMod, path, ref nIcon);
            return Icon.FromHandle(hIcon);
        }
    }
}