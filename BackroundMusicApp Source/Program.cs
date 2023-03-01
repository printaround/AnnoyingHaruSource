using System;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading;

namespace BackroundMusicApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var handle = NativeMethods.GetConsoleWindow();

            NativeMethods.ShowWindow(handle, NativeMethods.SW_HIDE);

            string currentMus = ReadFromFile(@"DataBackgroundMusic.txt").Trim();
            string checker;

            BackPlay(currentMus);

            while (true)
            {
                checker = ReadFromFile(@"DataBackgroundMusic.txt").Trim();

                if (checker == "off") break;
                else if (currentMus != checker) { currentMus = checker; BackPlay(currentMus); }

                Thread.Sleep(100);
            }

            return;
        }
        private static string ReadFromFile(string gettingFileName)
        {
            string file_data = "none";
            try
            {
                StreamReader sr = new StreamReader(gettingFileName);
                file_data = sr.ReadToEnd();
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            return file_data;
        }
        private static void BackPlay(string soundName)
        {
            try
            {
                SoundPlayer sp = new SoundPlayer();
                sp.SoundLocation = @"sounds/" + soundName + ".wav";
                sp.Load();
                sp.PlayLooping();
            }
            catch (Exception er)
            {
                Console.WriteLine("Exception: " + er.Message);
            }
        }
    }
    static class NativeMethods
    {
        [DllImport("kernel32.dll")]
        static public extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static public extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;
    }
}
