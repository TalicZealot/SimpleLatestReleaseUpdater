using System.ComponentModel;

namespace SimpleLatestReleaseUpdater.Models
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private double _progress = 0;
        private string _text = "";

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Text"));
                }
            }
        }
        public double Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                _progress = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Progress"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
