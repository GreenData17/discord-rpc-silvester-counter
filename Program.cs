using System;
using DiscordRPC;
using DiscordRPC.Logging;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Discord_SilvesterCounter
{
    public class Window : Form
    {
        public Window()
        {
            DoubleBuffered = true;
        }
    }

    class Program
    {
        static string ClientID = "";

        public static  DiscordRpcClient client;
        static bool initalized = false;

        public static Thread Loop = null;
        public static Window win = null;

        public static string countTime;

        static void Main(string[] args)
        {
            ClientID = File.ReadAllText(Assembly.GetExecutingAssembly().Location.Replace("Discord_SilvesterCounter.exe", "id.txt"));

            win = new Window
            {
                Size = new Size(800, 300),
                Text = "Discord Countdown app",
            };
            win.Paint += Win_Paint;
            win.FormClosing += Win_FormClosing;

            Loop = new Thread(WinLoop);
            Loop.Start();

            initialize();

            Application.Run(win);
        }

        private static void Win_FormClosing(object sender, FormClosingEventArgs e)
        {
            Loop.Abort();
        }

        static void WinLoop()
        {
            while (Loop.IsAlive)
            {
                try
                {
                    if (initalized)
                    {
                        win.BeginInvoke((MethodInvoker)delegate { win.Refresh(); });
                        client.ClearPresence();
                        Update();
                        Thread.Sleep(900);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + " | " + e.Source);
                }
            }
        }

        private static void Win_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.White);

            // -- From here on new code starts --
            DateTime now = DateTime.Now;
            DateTime silvester = DateTime.Parse($"01.01.{now.Year + 1} 00:00:00");

            TimeSpan differenze = silvester - now;

            if (now > silvester)
            {
                countTime = "Happy New Year!";
            }
            else countTime = $"-{(silvester - now).Days}d:{differenze.Hours}h:{differenze.Minutes}m:{differenze.Seconds}s";

            g.DrawString(DateTime.Now.ToString(), new Font(FontFamily.GenericMonospace, 20f), new SolidBrush(Color.Black), 100, 50);
            g.DrawString(silvester.ToString(), new Font(FontFamily.GenericMonospace, 20f), new SolidBrush(Color.Black), 100, 100);
            g.DrawString(countTime.ToString(), new Font(FontFamily.GenericMonospace, 20f), new SolidBrush(Color.Black), 500, 100);
            // -- new code ends here :D --

            #region old_code

            //string _time = DateTime.Now.ToString("HH:mm:ss");
            //string[] time = _time.Split(':');

            //int s = (int.Parse(time[0]) * 60) + (int.Parse(time[1]) * 60) + int.Parse(time[2]);
            //int n = s - 86400;

            //int ss = (int.Parse(time[2]) - 59);
            //string _ss = ss.ToString();

            //int mm = (int.Parse(time[1]) - 59);
            //string _mm = mm.ToString();

            //int hh = (int.Parse(time[0]) - 23);
            //string _hh = hh.ToString();


            //if (ss < 0) { _ss = _ss.Remove(0, 1); if (_ss.Length == 1) { _ss = "0" + _ss; } if (_ss.Length == 0) { _ss = "00"; } }
            //if (mm < 0) { _mm = _mm.Remove(0, 1); if (_mm.Length == 1) { _mm = "0" + _mm; } if (_mm.Length == 0) { _mm = "00"; } }
            //if (hh < 0) { _hh = _hh.Remove(0, 1); if (_hh.Length == 1) { _hh = "0" + _hh; } if (_hh.Length == 0) { _hh = "00"; } }

            //if (int.Parse(startHour) > int.Parse(_hh))
            //    countTime = $"-{_hh}:{_mm}:{_ss}";
            //else
            //    countTime = $"Happy New Year!";

            //g.DrawString(s.ToString(), new Font(FontFamily.GenericMonospace, 20f), new SolidBrush(Color.Black), 100, 100);
            //g.DrawString(DateTime.Now.ToString(), new Font(FontFamily.GenericMonospace, 20f), new SolidBrush(Color.Black), 100, 50);
            //g.DrawString(n.ToString(), new Font(FontFamily.GenericMonospace, 20f), new SolidBrush(Color.Black), 300, 100);
            //g.DrawString(countTime.ToString(), new Font(FontFamily.GenericMonospace, 20f), new SolidBrush(Color.Black), 500, 100);

            #endregion
        }

        public static void initialize()
        {
            client = new DiscordRpcClient(ClientID);
            client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };
            client.Initialize();
            initalized = true;
        }

        public static void Update()
        {
            if(initalized == false)
            {
                Console.WriteLine("App is not Initialized!");
            }
            else
            {
                client.SetPresence(new RichPresence()
                {
                    Details = "Waiting for new Year...",
                    State = countTime,
                    Timestamps = null,
                    Assets = new Assets()
                    {
                        LargeImageKey = "fireworks",
                        LargeImageText = "@greendata's RPC App",
                        SmallImageKey = ""
                    }
                });
            }
        }

    }
}
