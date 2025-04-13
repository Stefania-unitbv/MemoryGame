using MemoryGame.Commands;
using MemoryGame.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MemoryGame.Views
{
    public partial class GameView : Window
    {
        public GameView()
        {
            InitializeComponent();

           
        
        }
        private void CategoryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && DataContext is GameViewModel viewModel)
            {
                string categoryName = menuItem.Header.ToString();
                viewModel.SelectedCategory = categoryName;

              
                if (menuItem.Parent is MenuItem parentItem && parentItem.Parent is MenuItem categoryMenu)
                {
                    foreach (MenuItem item in categoryMenu.Items)
                    {   
                        if (item is MenuItem categoryItem)
                        {
                            categoryItem.IsChecked = (categoryItem.Header.ToString() == categoryName);
                        }
                    }
                }
               

            }
        }

        private void StandardSize_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is GameViewModel viewModel)
            {
                viewModel.IsCustomSize = false;
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            
            string aboutText = "Memory Game\n\n" +
                "Creator: [Ionita Stefania]\n" +
                "Email: [stefania.ionita@student.unitbv.ro]\n" +
                "Group: [10LF333]\n" +
                "Field: [Info Aplicata]";

            MessageBox.Show(aboutText, "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }
     
        private void CustomSize_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is GameViewModel viewModel)
            {
              
                var optionsDialog = new GameOptionsView(
                    viewModel.IsCustomSize,
                    viewModel.Rows,
                    viewModel.Columns,
                    viewModel.TimeInSeconds)
                {
                    Owner = this
                };

               
                if (optionsDialog.ShowDialog() == true)
                {  

                    viewModel.IsCustomSize = optionsDialog.IsCustom;

                    if (optionsDialog.IsCustom)
                    {
                        viewModel.Rows = optionsDialog.Rows;
                        viewModel.Columns = optionsDialog.Columns;
                    }
                    else
                    {
                        
                        viewModel.Rows = 4;
                        viewModel.Columns = 4;
                    }

                    viewModel.TimeInSeconds = optionsDialog.TimeInSeconds;
                }
            }
        }
    }
}