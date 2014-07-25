using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using DunGen.Engine.Models;

namespace DunGen.Visualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewModel mViewModel;
        public MainWindow()
        {
            InitializeComponent();
            mViewModel = (ViewModel) DataContext;
            mViewModel.Dispatcher = Dispatcher;
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Console.WriteLine("Width = {0}, Height = {1}",ActualWidth, ActualHeight);

        }
    }
}
