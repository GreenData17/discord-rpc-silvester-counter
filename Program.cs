﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordRPC;
using DiscordRPC.Logging;
using System.Windows.Forms;
using System.Drawing;
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
        static string ClientID = "censored";

        public static  DiscordRpcClient client;
        static bool initalized = false;

        public static Thread Loop = null;
        public static Window win = null;

        public static string startHour;
        public static string countTime;

        static void Main(string[] args)
        {
            startHour = DateTime.Now.ToString("HH").ToString();
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

            string _time = DateTime.Now.ToString("HH:mm:ss");
            string[] time = _time.Split(':');

            int s = (int.Parse(time[0]) * 60) + (int.Parse(time[1]) * 60) + int.Parse(time[2]);
            int n = s - 86400;

            int ss = (int.Parse(time[2]) - 59);
            string _ss = ss.ToString();

            int mm = (int.Parse(time[1]) - 59);
            string _mm = mm.ToString();

            int hh = (int.Parse(time[0]) - 23);
            string _hh = hh.ToString();


            if (ss < 0) { _ss = _ss.Remove(0, 1); if (_ss.Length == 1) { _ss = "0" + _ss; } if (_ss.Length == 0) { _ss = "00"; } }
            if (mm < 0) { _mm = _mm.Remove(0, 1); if (_mm.Length == 1) { _mm = "0" + _mm; } if (_mm.Length == 0) { _mm = "00"; } }
            if (hh < 0) { _hh = _hh.Remove(0, 1); if (_hh.Length == 1) { _hh = "0" + _hh; } if (_hh.Length == 0) { _hh = "00"; } }

            //if (int.Parse(startHour) > int.Parse(_hh))
            //    countTime = $"-{_hh}:{_mm}:{_ss}";
            //else
            //    countTime = $"Happy New Year!";

            DateTime now = DateTime.Now;
            DateTime silvester = DateTime.Parse("01.01.2024 00:00:01Z");

            if (now > silvester)
            {
                countTime = "Happy New Year!";
            }
            else countTime = $"-{(silvester - now).Days}d:{_hh}H:{_mm}m:{_ss}s";


            g.DrawString(s.ToString(), new Font(FontFamily.GenericMonospace, 20f), new SolidBrush(Color.Black), 100, 100);
            g.DrawString(DateTime.Now.ToString(), new Font(FontFamily.GenericMonospace, 20f), new SolidBrush(Color.Black), 100, 50);
            g.DrawString(n.ToString(), new Font(FontFamily.GenericMonospace, 20f), new SolidBrush(Color.Black), 300, 100);
            g.DrawString(countTime.ToString(), new Font(FontFamily.GenericMonospace, 20f), new SolidBrush(Color.Black), 500, 100);
        }

        public static void initialize()
        {
            initalized = true;
            client = new DiscordRpcClient(ClientID);
            client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };
            client.Initialize();
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
                        LargeImageText = "01000111's RPC App",
                        SmallImageKey = ""
                    }
                });
            }
        }

    }
}