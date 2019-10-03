using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using OxyPlot.Series;

namespace ScatterPointApp.Models
{
    public class ScatterPointCollection : ObservableCollection<ScatterPoint>
    {
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName]string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
            return true;
        }

        protected virtual void Shrink(int index)
        {
            while (Limit < Count) RemoveAt(0);
            for(int i = 0;i < Count; i++)
            {
                var item = Items[i];
                item.Size = (SizeMax - SizeMin) * (i + 1) / Count + SizeMin;
                item.Value = (i + 1) / (double)Count;
                Items[i] = item;
            }
        }

        private double sizeMax = 1.0;
        public double SizeMax
        {
            get { return sizeMax; }
            set { SetProperty(ref sizeMax, value); }
        }

        private double sizeMin = 1.0;
        public double SizeMin
        {
            get { return sizeMin; }
            set { SetProperty(ref sizeMin, value); }
        }


        private int limit = 50;
        public int Limit
        {
            get { return limit; }
            set
            {
                if (value < 1) value = 1;
                if (SetProperty(ref limit, value))
                {
                    Shrink(Count);
                }
            }
        }

        protected override void InsertItem(int index, ScatterPoint item)
        {
            base.InsertItem(index, item);
            Shrink(index);
        }
    }
}
