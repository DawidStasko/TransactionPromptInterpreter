using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialTransactionTextInterpreter.Logic.Interfaces;
using FinancialTransactionTextInterpreter.Logic.Services.Interfaces;
using FinancialTransactionTextInterpreter.Model;
using System.Collections.ObjectModel;
using Wpf.Ui;

namespace WpfFinancialTransactionPromptInterpreter.ViewModels;

public partial class InscribedTransactionsListVM : ObservableObject
{
					private readonly INewTransactionCreatedService _newTransactionCreatedService;
					private readonly ITransactionsSelectionService? _selectionService;
					private readonly ITransactionsTextProcessor? _transactionsTextProcessor;
					private readonly ISnackbarService _snackbarService;

					private ObservableCollection<InscribedTransaction> _inscribedTransactions = new();
					private InscribedTransaction _selectedItem;

					public ObservableCollection<InscribedTransaction> InscribedTransactions
					{
										get { return _inscribedTransactions; }
										set
										{
															_inscribedTransactions = value;
															OnPropertyChanged();
										}
					}

					public InscribedTransaction SelectedItem
					{

										get { return _selectedItem; }
										set
										{
															_selectedItem = value;
															OnPropertyChanged();
										}
					}

					public InscribedTransactionsListVM(ITransactionsSelectionService selectionService,
										ITransactionsTextProcessor? transactionsTextProcessor,
										INewTransactionCreatedService newTransactionCreatedService,
										ISnackbarService snackbarService)
					{
										_selectionService = selectionService;
										_transactionsTextProcessor = transactionsTextProcessor;
										_newTransactionCreatedService = newTransactionCreatedService;
										_newTransactionCreatedService.NewTransactionCreated += (transaction) =>
										{
															InscribedTransactions.Insert(0, transaction);
										};
										_snackbarService = snackbarService;
										InscribedTransactions = new ObservableCollection<InscribedTransaction>();

#if DEBUG
										InscribedTransactions = new ObservableCollection<InscribedTransaction>()
										{
														new($"&{DateTime.Now.AddDays(-2).ToString("dd-MM-yyyy")} $PKO #Jedzenie ToPowinnoMiecWartoscMinus11 -6 -5 TenutajMinus8 -8 #Transport Paliwo -302,02 @StacjaDokowania"),
														new($"&{DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy")} $PKO > $Santander 30 40,1"),
										};
#endif
					}

					[RelayCommand]
					private void Edit(InscribedTransaction selectedTransaction)
					{
										if (_selectionService != null)
															_selectionService.SelectedTransaction = selectedTransaction;
					}

					[RelayCommand]
					private void Delete()
					{
										InscribedTransactions.Remove(SelectedItem);
					}

					[RelayCommand]
					private void ProcessTransactions()
					{
										(IList<InscribedTransaction> successfullyProcessed, IList<InscribedTransaction> unsuccessfullyProcessed)? processingResult = _transactionsTextProcessor?.ProcessMultipleTransactions(InscribedTransactions);
										_snackbarService.Show("Transactions saved", $"Successfully processed {processingResult?.successfullyProcessed.Count ?? 0} out of {InscribedTransactions.Count}.", Wpf.Ui.Controls.ControlAppearance.Primary, null, TimeSpan.FromSeconds(20));
										InscribedTransactions = new ObservableCollection<InscribedTransaction>(processingResult?.unsuccessfullyProcessed ?? []);
					}
}