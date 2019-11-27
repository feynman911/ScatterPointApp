using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Wpf;

namespace ScatterPointApp.Behaviors
{
    [TypeConstraint(typeof(Plot))]
    public class OxyContextMenuBehavior : Behavior<Plot>
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
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
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

            var dlg = new SaveFileDialog
            {
                Filter = ".png files|*.png|.pdf files|*.pdf",
                DefaultExt = ".png"
            };
            if (dlg.ShowDialog().Value) FileName = dlg.FileName;

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

        /// <summary>
        /// 画像保存（右クリック用）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        #region ******************************* ImageWidth
        public int ImageWidth
        {
            get { return (int)this.GetValue(ImageWidthProperty); }
            set { this.SetValue(ImageWidthProperty, value); }
        }

        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof(int),
                typeof(OxyContextMenuBehavior),
                new FrameworkPropertyMetadata(0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    ImageWidthChangeFunc,
                    ImageWidthCoerceFunc));

        static void ImageWidthChangeFunc(DependencyObject target,
            DependencyPropertyChangedEventArgs e)
        {
            var of = (int)e.OldValue;
            var nf = (int)e.NewValue;
            var obj = (OxyContextMenuBehavior)target;
        }

        static object ImageWidthCoerceFunc(DependencyObject target, object baseValue)
        {
            var obj = (OxyContextMenuBehavior)target;
            var val = (int)baseValue;
            if (val < 0) val = 0;
            return val;
        }
        #endregion

        #region ******************************* ImageHeight
        public int ImageHeight
        {
            get { return (int)this.GetValue(ImageHeightProperty); }
            set { this.SetValue(ImageHeightProperty, value); }
        }

        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof(int),
                typeof(OxyContextMenuBehavior),
                new FrameworkPropertyMetadata(0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    ImageHeightChangeFunc,
                    ImageHeightCoerceFunc));

        static void ImageHeightChangeFunc(DependencyObject target,
            DependencyPropertyChangedEventArgs e)
        {
            var of = (int)e.OldValue;
            var nf = (int)e.NewValue;
            var obj = (OxyContextMenuBehavior)target;
        }

        static object ImageHeightCoerceFunc(DependencyObject target, object baseValue)
        {
            var obj = (OxyContextMenuBehavior)target;
            var val = (int)baseValue;
            if (val < 0) val = 0;
            return val;
        }
        #endregion

        #region ******************************* Scale
        public double Scale
        {
            get { return (double)this.GetValue(ScaleProperty); }
            set { this.SetValue(ScaleProperty, value); }
        }

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double),
                typeof(OxyContextMenuBehavior),
                new FrameworkPropertyMetadata(1.0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    ScaleChangeFunc,
                    ScaleCoerceFunc));

        static void ScaleChangeFunc(DependencyObject target,
            DependencyPropertyChangedEventArgs e)
        {
            var of = (double)e.OldValue;
            var nf = (double)e.NewValue;
            var obj = (OxyContextMenuBehavior)target;
        }

        static object ScaleCoerceFunc(DependencyObject target, object baseValue)
        {
            var obj = (OxyContextMenuBehavior)target;
            var val = (double)baseValue;
            if (val < 0.5) val = 0.5;
            if (val > 5.0) val = 5.0;
            return val;
        }
        #endregion

        #region ******************************* FileName
        public string FileName
        {
            get { return (string)this.GetValue(FileNameProperty); }
            set { this.SetValue(FileNameProperty, value); }
        }

        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register("FileName", typeof(string),
                typeof(OxyContextMenuBehavior),
                new FrameworkPropertyMetadata("",
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        #endregion


        #region ******************************* SaveChart
        public bool SaveChart
        {
            get { return (bool)this.GetValue(SaveChartProperty); }
            set { this.SetValue(SaveChartProperty, value); }
        }

        public static readonly DependencyProperty SaveChartProperty =
            DependencyProperty.Register("SaveChart", typeof(bool),
                typeof(OxyContextMenuBehavior),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    SaveChartChangeFunc,
                    SaveChartCoerceFunc));

        static void SaveChartChangeFunc(DependencyObject target,
            DependencyPropertyChangedEventArgs e)
        {
            var of = (bool)e.OldValue;
            var nf = (bool)e.NewValue;
            var obj = (OxyContextMenuBehavior)target;

            obj.SaveChart = false;
        }

        static object SaveChartCoerceFunc(DependencyObject target, object baseValue)
        {
            var obj = (OxyContextMenuBehavior)target;
            var val = (bool)baseValue;

            if (val == true)
            {
                obj.Save();
            }

            return val;
        }
        #endregion


        /// <summary>
        /// 画像保存
        /// </summary>
        private void Save()
        {
            string filename = FileName;

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
