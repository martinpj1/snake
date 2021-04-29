using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace ConsoleExtender
{

    [StructLayout(LayoutKind.Sequential)]
    public struct COORD
    {
        public short X;
        public short Y;

        public COORD(short X, short Y)
        {
            this.X = X;
            this.Y = Y;
        }
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct DEVMODE1
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmDeviceName;
        public short dmSpecVersion;
        public short dmDriverVersion;
        public short dmSize;
        public short dmDriverExtra;
        public int dmFields;

        public short dmOrientation;
        public short dmPaperSize;
        public short dmPaperLength;
        public short dmPaperWidth;

        public short dmScale;
        public short dmCopies;
        public short dmDefaultSource;
        public short dmPrintQuality;
        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmFormName;
        public short dmLogPixels;
        public short dmBitsPerPel;
        public int dmPelsWidth;
        public int dmPelsHeight;

        public int dmDisplayFlags;
        public int dmDisplayFrequency;

        public int dmICMMethod;
        public int dmICMIntent;
        public int dmMediaType;
        public int dmDitherType;
        public int dmReserved1;
        public int dmReserved2;

        public int dmPanningWidth;
        public int dmPanningHeight;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct CONSOLE_FONT_INFO
    {
        public int nFont;
        public COORD dwFontSize;
    }

    [System.Flags]
    enum LoadLibraryFlags : uint
    {
        DONT_RESOLVE_DLL_REFERENCES = 0x00000001,
        LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x00000010,
        LOAD_LIBRARY_AS_DATAFILE = 0x00000002,
        LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE = 0x00000040,
        LOAD_LIBRARY_AS_IMAGE_RESOURCE = 0x00000020,
        LOAD_LIBRARY_SEARCH_APPLICATION_DIR = 0x00000200,
        LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x00001000,
        LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR = 0x00000100,
        LOAD_LIBRARY_SEARCH_SYSTEM32 = 0x00000800,
        LOAD_LIBRARY_SEARCH_USER_DIRS = 0x00000400,
        LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008
    }

    public static class ConsoleHelper
    {

        static ConsoleHelper()
        {
            //var temp = Assembly.CodeBase;
            //var myPath = new Uri(typeof(MyDll).Assembly.CodeBase).LocalPath;
            //var myFolder = Path.GetDirectoryName(myPath);

            //var is64 = IntPtr.Size == 8;
            //var subfolder = is64 ? "\\win64\\" : "\\win32\\";

            IntPtr zero = IntPtr.Zero;
            var ret = LoadLibraryEx(@"C:\Windows\System32\kernel32.dll", zero, LoadLibraryFlags.LOAD_LIBRARY_SEARCH_SYSTEM32);
            if (ret == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hReservedNull, LoadLibraryFlags dwFlags);


        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("MyDll.dll")]
        public static extern int MyFunction(int var1, int var2);


        #region StdHandle

        [DllImport("kernel32")]
        private static extern IntPtr GetStdHandle(StdHandle index);

        private enum StdHandle
        {
            InputHandle = -10,
            OutputHandle = -11,
            ErrorHandle = -12
        }

        #endregion

        #region ICON
        //[DllImport("kernel32")]
        //public static extern bool SetConsoleIcon(IntPtr hIcon);

        //public static bool SetConsoleIcon(Icon icon) {
        //	return SetConsoleIcon(icon.Handle);
        //}
        #endregion

        public static void AdjustBuffer()
        {
            //This is to hide the scroll bar
            Console.BufferWidth = Console.WindowWidth = Console.LargestWindowWidth;
            Console.BufferHeight = Console.WindowHeight = Console.LargestWindowHeight;
        }

        #region FULLSCREEN
        public static void SetFullScreen(bool showCursor = false)
        {
            var screenBuffer = GetStdHandle(StdHandle.OutputHandle);
            COORD fullscreenDimension;// = new COORD(100, 100);
            int ans = Marshal.GetLastWin32Error();
            bool ret = SetConsoleDisplayMode(screenBuffer, (uint)1, out fullscreenDimension);
            ans = Marshal.GetLastWin32Error();
            //throw new Win32Exception(ans);
            AdjustBuffer();

            Console.CursorVisible = showCursor;
        }


        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetConsoleDisplayMode(IntPtr ConsoleOutput, uint Flags, out COORD NewScreenBufferDimensions);

        #endregion

        #region RESOLUTION
        //Change it back to original on application exit
        //Handle multiple screens

        public static int retainWidth;
        public static int retainHeight;

        static COORD CurrentFontSize;

        static bool retainResolution = true;
        public static void ChangeResolution(int width, int height)
        {
            DEVMODE1 dm = new DEVMODE1();
            dm.dmDeviceName = new String(new char[32]);
            dm.dmFormName = new String(new char[32]);
            dm.dmSize = (short)Marshal.SizeOf(dm);


            if (0 != EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref dm))
            {
                //only grab it the first time
                if (retainResolution)
                {
                    retainWidth = dm.dmPelsWidth;
                    retainHeight = dm.dmPelsHeight;
                    retainResolution = false;
                }

                dm.dmPelsWidth = width;
                dm.dmPelsHeight = height;

                int iRet = ChangeDisplaySettings(ref dm, CDS_TEST);

                if (iRet == DISP_CHANGE_FAILED)
                {
                    Console.WriteLine("Unable to process your request");
                    Console.WriteLine("Description: Unable To Process Your Request. Sorry For This Inconvenience.");
                }
                else
                {
                    iRet = ChangeDisplaySettings(ref dm, CDS_UPDATEREGISTRY);
                    //what if this is too fast for slower computers? 
                    //how would you check that the Console.WindowWidth had actually changed? 
                    //you can't loop forever until it does change, because there's the chance
                    //that the default resolution is the same as the one you're changing it to

                    switch (iRet)
                    {
                        case DISP_CHANGE_SUCCESSFUL:
                            {
                                while (Math.Abs((width / CurrentFontSize.X) - Console.WindowWidth) > CurrentFontSize.X)
                                {
                                    Thread.Sleep(50);
                                }
                                //wait for the screen size to actually change
                                //when the font and the screen size are right... 
                                //the console window width will be 126

                                AdjustBuffer();
                                break;
                            }
                        case DISP_CHANGE_RESTART:
                            {
                                Console.WriteLine("Description: You Need To Reboot For The Change To Happen.\n If You Feel Any Problem After Rebooting Your Machine\nThen Try To Change Resolution In Safe Mode.");
                                break;
                                //windows 9x series you have to restart
                            }
                        default:
                            {
                                //failed to change
                                Console.WriteLine("Description: Failed To Change The Resolution.");
                                break;
                            }
                    }
                }

            }
        }

        [DllImport("user32.dll")]
        public static extern int EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE1 devMode);
        [DllImport("user32.dll")]
        public static extern int ChangeDisplaySettings(ref DEVMODE1 devMode, int flags);

        public const int ENUM_CURRENT_SETTINGS = -1;
        public const int CDS_UPDATEREGISTRY = 0x01;
        public const int CDS_TEST = 0x02;
        public const int DISP_CHANGE_SUCCESSFUL = 0;
        public const int DISP_CHANGE_RESTART = 1;
        public const int DISP_CHANGE_FAILED = -1;
        #endregion


        const uint ENABLE_MOUSE_INPUT = 0x0010;
        const uint ENABLE_QUICK_EDIT = 0x0040;
        /// <summary>
        /// Disable mouse selection, copy/paste, etc.
        /// </summary>
        public static void DisableQuickEdit()
        {
            UInt32 consoleMode;
            IntPtr consoleHandle = GetStdHandle(StdHandle.InputHandle);
            var r = GetConsoleMode(consoleHandle, out consoleMode);

            // Clear the quick edit bit in the mode flags
            consoleMode &= ~ENABLE_QUICK_EDIT;
            consoleMode &= ~ENABLE_MOUSE_INPUT;

            r = SetConsoleMode(consoleHandle, consoleMode);
        }

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);
        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        #region FONT
        private const int TMPF_FIXED_PITCH = 1;
        private const int TMPF_VECTOR = 2;
        private const int TMPF_TRUETYPE = 4;
        private const int TMPF_DEVICE = 8;
        private const int LF_FACESIZE = 32;
        private static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        public static void SetConsoleFont(short fontHeight, string fontName = Wyse700b)
        {
            unsafe
            {
                IntPtr hnd = GetStdHandle(StdHandle.OutputHandle);
                if (hnd != INVALID_HANDLE_VALUE)
                {
                    // Set console font to Lucida Console.
                    CONSOLE_FONT_INFO_EX newInfo = new CONSOLE_FONT_INFO_EX();
                    newInfo.cbSize = (uint)Marshal.SizeOf(newInfo);
                    newInfo.FontFamily = 32;
                    newInfo.nFont = 0;

                    IntPtr ptr = new IntPtr(newInfo.FaceName);
                    Marshal.Copy(fontName.ToCharArray(), 0, ptr, fontName.Length);

                    CurrentFontSize = new COORD((short)(fontHeight / 2), fontHeight);
                    // Get some settings from current font.
                    newInfo.dwFontSize = CurrentFontSize;
                    newInfo.FontWeight = 400;

                    SetCurrentConsoleFontEx(hnd, false, ref newInfo);
                }
            }
            AdjustBuffer();
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        unsafe public struct CONSOLE_FONT_INFO_EX
        {
            public uint cbSize;
            public uint nFont;
            public COORD dwFontSize;
            public int FontFamily;
            public int FontWeight;

            public fixed char FaceName[32]; // this will require the assembly to be unsafe 
        }

        //https://int10h.org/oldschool-pc-fonts/
        public const string Wyse700b = "Bm437 Wyse700b";

        public static unsafe COORD GetCurrentFontSize()
        {
            var outputHandle = GetStdHandle(StdHandle.OutputHandle);

            //Obtain the current console font index for a maximized window
            CONSOLE_FONT_INFO_EX currentFont = new CONSOLE_FONT_INFO_EX();
            currentFont.cbSize = (uint)Marshal.SizeOf(currentFont);

            bool success = GetCurrentConsoleFontEx(outputHandle, true, ref currentFont);



            char[] fontName = new char[32];
            for (int i = 0; i < 32; i++)
                fontName[i] = currentFont.FaceName[i];

            string fName = string.Join(string.Empty, fontName);

            var temp = currentFont.FaceName;
            IntPtr ptr = new IntPtr(currentFont.FaceName);
            //Marshal.Copy(ptr, 0, fontName, 32);


            //Use that index to obtain font size    
            return GetConsoleFontSize(outputHandle, (int)currentFont.nFont);
        }

        [DllImport("kernel32.dll")]
        static extern bool GetCurrentConsoleFont(IntPtr hConsoleOutput, bool bMaximumWindow, out CONSOLE_FONT_INFO lpConsoleCurrentFont);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern COORD GetConsoleFontSize(IntPtr hConsoleOutput, Int32 nFont);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetCurrentConsoleFontEx(IntPtr ConsoleOutput, bool MaximumWindow, ref CONSOLE_FONT_INFO_EX ConsoleCurrentFontEx);


        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        extern static bool GetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool bMaximumWindow, [In, Out] ref CONSOLE_FONT_INFO_EX lpConsoleCurrentFont);
        #endregion

    }
}
