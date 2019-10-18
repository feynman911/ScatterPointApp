using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using OxyPlot.Series;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using ScatterPointApp.Models;
using OxyPlot;
using OxyPlot.Axes;
using System.Collections.Generic;
using System.Diagnostics;

namespace ScatterPointApp.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "ScatterPoint Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private DispatcherTimer timer = new DispatcherTimer();

        public MainWindowViewModel()
        {
            ScatterPoints.Limit = 50;
            ScatterPoints.SizeMax = 5.0;
            ScatterPoints.SizeMin = 1.0;
            timer.Interval = TimeSpan.FromMilliseconds(Interval) ;
            timer.Tick += new EventHandler(MovingPoint);

            watch.Start();
        }

        private ScatterPointCollection scatterPoints = new ScatterPointCollection();
        public ScatterPointCollection ScatterPoints
        {
            get { return scatterPoints; }
            set { SetProperty(ref scatterPoints, value); }
        }

        /// <summary>
        /// UIスレッド以外からポイントを追加する時に使用（Dispatcher.BeginInvokeを使用）
        /// </summary>
        /// <param name="sp"></param>
        public void SetScatterPoint(ScatterPoint sp)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(
                new Action(() => ScatterPoints.Add(sp)));
        }

        private bool startFlag = false;
        /// <summary>
        /// DispatcherTimerのStart/Stop
        /// </summary>
        public bool StartFlag
        {
            get { return startFlag; }
            set {
                if (value == true) timer.Start();
                else timer.Stop();
                SetProperty(ref startFlag, value);
            }
        }

        private double dA1 = 10.0;
        private double dA2 = 0.7 / 180.0 * Math.PI;
        private double ang1 = 0.0;
        private double ang2 = 0.0;

        /// <summary>
        /// DispatcherTimerで実行するポイント追加メソッド
        /// UIスレッドで実行される
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MovingPoint(object sender, EventArgs e)
        {
            double mag = 50.0 + 40.0 * Math.Sin(ang2);
            double ang = ang1;
            var p = new ScatterPoint(mag, ang);
            ScatterPoints.Add(p);
            ang1 += dA1;
            ang2 += dA2;


            frameCount++;
            if (watch.ElapsedMilliseconds > 1000 && frameCount > 1)
            {
                PointAddRate = string.Format("PointAddRate {0:#.}", this.frameCount / (this.watch.ElapsedMilliseconds * 0.001));
                frameCount = 0;
                watch.Reset();
                watch.Start();
            }
        }


        public InteractionRequest<Notification> OxyPlotSaveRequest { get; private set; } = new InteractionRequest<Notification>();

        private DelegateCommand saveChart;
        public DelegateCommand SaveChart =>
            saveChart ?? (saveChart = new DelegateCommand(ExecuteSaveChart));

        void ExecuteSaveChart()
        {
            OxyPlotSaveRequest.Raise(new Notification { Content = "test1.png" });

        }

        private int interval = 25;
        public int Interval
        {
            get { return interval; }
            set { SetProperty(ref interval, value);
                timer.Interval = TimeSpan.FromMilliseconds(Interval);
            }
        }

        /// <summary>
        /// The frame count.
        /// </summary>
        private int frameCount;

        /// <summary>
        /// The watch.
        /// </summary>
        private Stopwatch watch = new Stopwatch();

        private string pointAddRate = "PointAddRate";
        public string PointAddRate
        {
            get { return pointAddRate; }
            set { SetProperty(ref pointAddRate, value); }
        }


    }
}
