﻿using FinancialTransactionTextInterpreter.ViewModels;
using Microsoft.Win32;
using System.Diagnostics;
using System.Windows;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace FinancialTransactionTextInterpreter;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : FluentWindow
{
					public static readonly DependencyProperty FilePathProperty =
									DependencyProperty.Register("FilePath", typeof(string), typeof(MainWindow));

					public string FilePath
					{
										get { return (string)GetValue(FilePathProperty); }
										set { SetValue(FilePathProperty, value); }
					}

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
															FilePath = fileDialog.FileName;
										}
					}

					private void OpenExcelFile(object sender, RoutedEventArgs e)
					{
										string filePath = ((MainWindowVM)DataContext).GetFilePath();
										if (string.IsNullOrEmpty(filePath))
										{
															return;
										}

										Process excelFile = new()
										{
															StartInfo = new ProcessStartInfo()
															{
																				FileName = "powershell.exe",
																				Arguments = $"-Command \"& {{Start-Process {filePath} }}\"",
																				UseShellExecute = true
															}
										};

										excelFile.Start();
					}

}
