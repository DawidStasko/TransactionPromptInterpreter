﻿using FinancialTransactionTextInterpreter.ViewModels;
using Microsoft.Win32;
using System.Windows;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace FinancialTransactionTextInterpreter;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : FluentWindow
{

					public MainWindow(MainWindowVM mainWindowVM, ISnackbarService snackbarService)
					{
										InitializeComponent();
										this.DataContext = mainWindowVM;
										snackbarService.SetSnackbarPresenter(SnackbarPresenter);
					}

					private void OpenFileDialog(object sender, RoutedEventArgs e)
					{
										OpenFileDialog fileDialog = new();
										fileDialog.DefaultExt = ".xlsx";
										fileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
										bool? result = fileDialog.ShowDialog();
										if (result ?? false)
										{
															MainWindowVM mainWindowVM = (MainWindowVM)DataContext;
															mainWindowVM.SetNewFileName(fileDialog.FileName);
										}
					}
}