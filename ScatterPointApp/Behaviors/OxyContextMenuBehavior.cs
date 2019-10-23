using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Wpf;
using Prism.Events;

namespace ScatterPointApp.Behaviors
{
    [TypeConstraint(typeof(Plot))]
    public class OxyContextMenuBehavior: Behavior<Plot>
    {
        /// <summary>
        /// 要素にアタッチされた時にイベントハンドラを登録
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            menuItemCopy.Header = "Copy";
            menuItemSave.Header = "Save";
            menuListBox.Items.Add(menuItemCopy);
            menuListBox.Items.Add(menuItemSave);

            menuItemCopy.Click += OnCopyClick;
            menuItemSave.Click += OnSaveClick;
            this.AssociatedObject.ContextMenu = menuListBox;

            if(EventA != null) EventA.GetEvent<PubSubEvent<string>>().Subscribe(ChartSave);
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            this.AssociatedObject.SaveBitmap("test.png");

            double width = this.AssociatedObject.ActualWidth;
            double height = this.AssociatedObject.ActualHeight;
            if (ImageWidth > 0) width = ImageWidth;
            if (ImageHeight > 0) height = ImageHeight;
            if (Scale > 0)
            {
                width = width * Scale;
                height = height * Scale;
            }

            if (FileName == "")
            {
                var dlg = new SaveFileDialog
                {
                    Filter = ".png files|*.png|.pdf files|*.pdf",
                    DefaultExt = ".png"
                };
                if (dlg.ShowDialog().Value) FileName = dlg.FileName;
            }
            if (FileName != "")
            {
                var ext = Path.GetExtension(FileName).ToLower();
                switch (ext)
                {
                    case ".png":

                        PngExporter.Export(this.AssociatedObject.ActualModel, FileName, (int)width, (int)height, OxyColors.White);
                        break;
                    case ".pdf":
                        using (var s = File.Create(FileName))
                        {
                            PdfExporter.Export(this.AssociatedObject.ActualModel, s, width, height);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void OnCopyClick(object sender, RoutedEventArgs e)
        {
            double width = this.AssociatedObject.ActualWidth;
            double height = this.AssociatedObject.ActualHeight;
            if (ImageWidth > 0) width = ImageWidth;
            if (ImageHeight > 0) height = ImageHeight;
            if (Scale > 0)
            {
                width = width * Scale;
                height = height * Scale;
            }

            var pngExporter = new PngExporter
            { Width = (int)width, Height = (int)height, Background = OxyColors.White };
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                var bitmap = pngExporter.ExportToBitmap(this.AssociatedObject.ActualModel);
                Clipboard.SetImage(bitmap);
            }));
        }

        /// <summary>
        /// 要素にデタッチされた時にイベントハンドラを解除
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            menuItemSave.Click -= OnSaveClick;
            menuItemCopy.Click -= OnCopyClick;
            this.AssociatedObject.ContextMenu = null;
        }

        ContextMenu menuListBox = new ContextMenu();
        MenuItem menuItemCopy = new MenuItem();
        MenuItem menuItemSave = new MenuItem();

        /// <summary>
        /// 画像幅
        /// </summary>
        public int ImageWidth
        {
            get { return (int)GetValue(MyImageWidth); }
            set { SetValue(MyImageWidth, value); }
        }
        public static readonly DependencyProperty MyImageWidth =
            DependencyProperty.Register("ImageWidth", typeof(int), typeof(OxyContextMenuBehavior), new PropertyMetadata(0));

        /// <summary>
        /// 画像高さ
        /// </summary>
        public int ImageHeight
        {
            get { return (int)GetValue(MyImageHeight); }
            set { SetValue(MyImageHeight, value); }
        }
        public static readonly DependencyProperty MyImageHeight =
            DependencyProperty.Register("ImageHeight", typeof(int), typeof(OxyContextMenuBehavior), new PropertyMetadata(0));

        /// <summary>
        /// 画像倍率
        /// </summary>
        public double Scale
        {
            get { return (double)GetValue(MyScale); }
            set { SetValue(MyScale, value); }
        }
        public static readonly DependencyProperty MyScale =
            DependencyProperty.Register("Scale", typeof(double), typeof(OxyContextMenuBehavior), new PropertyMetadata(1.0));

        /// <summary>
        /// 画像保存時のファイル名
        /// </summary>
        public string FileName
        {
            get { return (string)GetValue(MyFileName); }
            set { SetValue(MyFileName, value); }
        }
        public static readonly DependencyProperty MyFileName =
            DependencyProperty.Register("FileName", typeof(string), typeof(OxyContextMenuBehavior), new PropertyMetadata(""));

        /// <summary>
        /// ViewModelからのイベントを受けるためのEventAggregator
        /// </summary>
        public IEventAggregator EventA
        {
            get { return (IEventAggregator)GetValue(MyEA); }
            set { SetValue(MyEA, value); }
        }
        public static readonly DependencyProperty MyEA =
            DependencyProperty.Register("EventA", typeof(IEventAggregator), typeof(OxyContextMenuBehavior), new PropertyMetadata());

        /// <summary>
        /// 画像保存のイベントを受け取った時の処理
        /// </summary>
        /// <param name="fn"></param>
        private void ChartSave(string fn)
        {
            string filename = FileName;
            if (FileName == "") filename = fn;

            if (filename == "")
            {
                var dlg = new SaveFileDialog
                {
                    Filter = ".png files|*.png",
                    DefaultExt = ".png"
                };
                if (dlg.ShowDialog().Value) filename = dlg.FileName;
            }
            Plot plot = this.AssociatedObject as Plot;

            double width = plot.ActualWidth;
            double height = plot.ActualHeight;
            if (ImageWidth > 0) width = ImageWidth;
            if (ImageHeight > 0) height = ImageHeight;
            if (Scale > 0)
            {
                width = width * Scale;
                height = height * Scale;
            }

            if (filename != "")
            {
                var ext = Path.GetExtension(filename).ToLower();
                if (ext == ".png")
                {
                    PngExporter.Export(plot.ActualModel, filename, (int)width, (int)height, OxyColors.White);
                }
            }

        }
    }
}
