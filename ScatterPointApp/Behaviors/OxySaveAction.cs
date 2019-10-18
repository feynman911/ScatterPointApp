using System;
using System.IO;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media;
using OxyPlot;
using OxyPlot.Wpf;
using Prism.Interactivity.InteractionRequest;

namespace ScatterPointApp.Behaviors
{
    [TypeConstraint(typeof(Plot))]
    public class OxySaveAction : TriggerAction<Plot>
    {
        protected override void Invoke(object parameter)
        {
            var args = parameter as InteractionRequestedEventArgs;
            FileName = (string)args.Context.Content;
            Plot plot = this.AssociatedObject as Plot;

            double width = this.AssociatedObject.ActualWidth;
            double height = this.AssociatedObject.ActualHeight;
            if (ImageWidth > 0) width = ImageWidth;
            if (ImageHeight > 0) height = ImageHeight;
            if (Scale > 0)
            {
                width = width * Scale;
                height = height * Scale;
            }

            var tempMajorGridlineStyle0 = plot.Axes[0].MajorGridlineStyle;
            var tempMinorGridlineStyle0 = plot.Axes[0].MinorGridlineStyle;
            var tempMajorGridlineStyle1 = plot.Axes[1].MajorGridlineStyle;
            var tempMinorGridlineStyle1 = plot.Axes[1].MinorGridlineStyle;

            plot.Axes[0].MajorGridlineStyle = LineStyle.Solid;
            plot.Axes[0].MinorGridlineStyle = LineStyle.Dash;
            plot.Axes[1].MajorGridlineStyle = LineStyle.Solid;
            plot.Axes[1].MinorGridlineStyle = LineStyle.Dash;

            if (FileName != "")
            {
                var ext = Path.GetExtension(FileName).ToLower();
                if(ext == ".png")
                {
                    PngExporter.Export(plot.ActualModel, FileName, (int)width, (int)height, OxyColors.White);
                }
            }
            plot.Axes[0].MajorGridlineStyle = tempMajorGridlineStyle0;
            plot.Axes[0].MinorGridlineStyle = tempMinorGridlineStyle0;
            plot.Axes[1].MajorGridlineStyle = tempMajorGridlineStyle1;
            plot.Axes[1].MinorGridlineStyle = tempMinorGridlineStyle1;

        }

        /// <summary>
        /// 画像保存時のファイル名
        /// </summary>
        public string FileName
        {
            get { return (string)GetValue(MyFileName); }
            set { SetValue(MyFileName, value); }
        }

        public static readonly DependencyProperty MyFileName =
            DependencyProperty.Register("FileName", typeof(string), typeof(OxySaveAction), new PropertyMetadata(""));

        /// <summary>
        /// 画像幅
        /// </summary>
        public int ImageWidth
        {
            get { return (int)GetValue(MyImageWidth); }
            set { SetValue(MyImageWidth, value); }
        }

        public static readonly DependencyProperty MyImageWidth =
            DependencyProperty.Register("ImageWidth", typeof(int), typeof(OxySaveAction), new PropertyMetadata(0));


        /// <summary>
        /// 画像高さ
        /// </summary>
        public int ImageHeight
        {
            get { return (int)GetValue(MyImageHeight); }
            set { SetValue(MyImageHeight, value); }
        }

        public static readonly DependencyProperty MyImageHeight =
            DependencyProperty.Register("ImageHeight", typeof(int), typeof(OxySaveAction), new PropertyMetadata(0));


        /// <summary>
        /// 画像倍率
        /// </summary>
        public double Scale
        {
            get { return (double)GetValue(MyScale); }
            set { SetValue(MyScale, value); }
        }

        public static readonly DependencyProperty MyScale =
            DependencyProperty.Register("Scale", typeof(double), typeof(OxySaveAction), new PropertyMetadata(1.0));


    }
}
