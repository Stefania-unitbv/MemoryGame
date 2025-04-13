using MemoryGame.Commands;
using MemoryGame.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;

namespace MemoryGame.Views
{
    public partial class StatisticsView : Window
    {
        public StatisticsView()
        {
            InitializeComponent();

            Loaded += StatisticsView_Loaded;
        }

        private void StatisticsView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is StatisticsViewModel viewModel)
            {
              
                RelayCommand closeCommand = viewModel.CloseCommand as RelayCommand;
                if (closeCommand != null)
                {
                   
                    viewModel.CloseRequested += (s, args) => Close();
                }
            }
        }
    }
}