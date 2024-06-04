﻿using ClosedXML.Excel;
using WpfFinancialTransactionPromptInterpreter.Logic.ExternalInterfaces;
using WpfFinancialTransactionPromptInterpreter.Model;
using WpfFinancialTransactionPromptInterpreter.Model.Interfaces;

namespace WpfFinancialTransactionPromptInterpreter.Infrastructure;
public class TransactionToXLSXSaver : ITransactionsRepository
{
					private readonly IConfig _config;

					public TransactionToXLSXSaver(IConfig config)
					{
										_config = config;
					}

					public void Save(Transaction transaction)
					{
										string worksheetName = GetWorksheetName(transaction.Date);
										using XLWorkbook workbook = new(_config.FinancialDataFullyQualifiedFileName);
										IXLWorksheets worksheetsList = workbook.Worksheets;
										IXLWorksheet worksheet = workbook.Worksheets.FirstOrDefault(w => w.Name == worksheetName) ?? workbook.Worksheets.Add(worksheetName);
										IXLRow firstRow = worksheet.LastRowUsed()?.RowBelow() ?? worksheet.FirstRow();
										IXLRow row = firstRow;
										foreach (Item item in transaction.Items)
										{
															row.Cell(1).Value = transaction.Date;
															row.Cell(2).Value = item.Name;
															row.Cell(3).Value = item.Price;
															row.Cell(4).Value = item.Category;
															row.Cell(5).Value = transaction.Account;
															row.Cell(6).Value = transaction.Contractor;
															row.Cell(7).Value = string.Join(", ", item.Tags);
															row = row.RowBelow();
										}

										worksheet.Range(firstRow.Cell(8), row.RowAbove().Cell(8)).Merge();

										IXLCell sumCell = firstRow.Cell(8);
										sumCell.Value = transaction.Items.Select(x => x.Price).Sum();
										sumCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
										sumCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
										sumCell.Style.Font.Bold = true;

										XLColor newBackgroundColor = XLColor.DarkElectricBlue;

										if (firstRow.RowNumber() == 1)
										{
															newBackgroundColor = XLColor.DarkGreen;
										}
										else
										{
															XLColor? rowAboveBackgroundColor = firstRow.RowAbove().Cell(1).Style.Fill.BackgroundColor;
															newBackgroundColor = (rowAboveBackgroundColor == XLColor.LightGreen ?
																																													XLColor.DarkGreen : XLColor.LightGreen);
										}

										IXLRange transactionRange = worksheet.Range(firstRow.Cell(1), row.RowAbove().Cell(8));
										transactionRange.Style.Fill.SetBackgroundColor(newBackgroundColor);
										transactionRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thick);
										transactionRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);

										workbook.Save();
					}

					private static string GetWorksheetName(DateTime date)
					{
										int month = date.Month;
										int year = date.Year;

										return $"{month}-{year}";
					}
}
