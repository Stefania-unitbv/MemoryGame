using System.ComponentModel;
using System.Windows;

namespace MemoryGame.Views
{
    public partial class GameOptionsView : Window, INotifyPropertyChanged
    {
        private bool _isStandard;
        private bool _isCustom;
        private int _rows;
        private int _columns;
        private int _timeInSeconds;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsStandard
        {
            get => _isStandard;
            set
            {
                _isStandard = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsStandard)));
            }
        }

        public bool IsCustom
        {
            get => _isCustom;
            set
            {
                _isCustom = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCustom)));

              
                if (RowsComboBox != null) RowsComboBox.IsEnabled = value;
                if (ColumnsComboBox != null) ColumnsComboBox.IsEnabled = value;
            }
        }

        public int Rows
        {
            get => _rows;
            set
            {
                _rows = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Rows)));
            }
        }

        public int Columns
        {
            get => _columns;
            set
            {
                _columns = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Columns)));
            }
        }

        public int TimeInSeconds
        {
            get => _timeInSeconds;
            set
            {
                _timeInSeconds = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TimeInSeconds)));
            }
        }

        public GameOptionsView(bool isCustomSize, int rows, int columns, int timeInSeconds)
        {
            InitializeComponent();

        
            IsStandard = !isCustomSize;
            IsCustom = isCustomSize;
            Rows = rows;
            Columns = columns;
            TimeInSeconds = timeInSeconds;

          
            DataContext = this;
        }

        private void StandardRadio_Checked(object sender, RoutedEventArgs e)
        {
            IsStandard = true;
            IsCustom = false;

          
            Rows = 4;
            Columns = 4;
        }

        private void CustomRadio_Checked(object sender, RoutedEventArgs e)
        {
            IsStandard = false;
            IsCustom = true;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}