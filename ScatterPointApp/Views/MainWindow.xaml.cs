using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Wpf;
using Prism.Events;

namespace ScatterPointApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CompositionTarget.Rendering += this.CompositionTargetRendering;
            this.watch.Start();
        }

        /// <summary>
        /// The frame count.
        /// </summary>
        private int frameCount;

        /// <summary>
        /// The watch.
        /// </summary>
        private Stopwatch watch = new Stopwatch();

        /// <summary>
        /// Handles the Rendering event of the CompositionTarget control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void CompositionTargetRendering(object sender, EventArgs e)
        {
            this.frameCount++;
            if (this.watch.ElapsedMilliseconds > 1000 && this.frameCount > 1)
            {
                this.FrameRate.Text = string.Format("FrameRate {0:#.}",this.frameCount / (this.watch.ElapsedMilliseconds * 0.001));
                this.frameCount = 0;
                this.watch.Reset();
                this.watch.Start();
            }

            //if (this.vm.MeasureFrameRate)
            //{
            //    this.Plot1.InvalidatePlot(true);
            //}
        }

        /// <summary>
        /// PlotのGridLineOn/Off
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_GridLine_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBox_GridLine.IsChecked != false)
            {
                Plot_Live.Axes[0].MajorGridlineStyle = OxyPlot.LineStyle.Solid;
                Plot_Live.Axes[0].MinorGridlineStyle = OxyPlot.LineStyle.Dash;
                Plot_Live.Axes[1].MajorGridlineStyle = OxyPlot.LineStyle.Solid;
                Plot_Live.Axes[1].MinorGridlineStyle = OxyPlot.LineStyle.Dash;
            }
            else
            {
                Plot_Live.Axes[0].MajorGridlineStyle = OxyPlot.LineStyle.None;
                Plot_Live.Axes[0].MinorGridlineStyle = OxyPlot.LineStyle.None;
                Plot_Live.Axes[1].MajorGridlineStyle = OxyPlot.LineStyle.None;
                Plot_Live.Axes[1].MinorGridlineStyle = OxyPlot.LineStyle.None;
            }
        }

    }
}
