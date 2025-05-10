using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WorkShopPC;
using System.Windows.Forms.DataVisualization.Charting;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.Remoting.Contexts;

namespace WorkShopPC.wndPC
{
    /// <summary>
    /// Логика взаимодействия для Report.xaml
    /// </summary>
    public partial class Report : Window
    {

        private readonly Entities _context;
        public Report()
        {
            InitializeComponent();

            _context = Entities.GetContext();

            ChartPayments.ChartAreas.Add(new ChartArea("Main"));
            var currentSeries = new Series("Платежи")
            {
                IsValueShownAsLabel = true
            };
            ChartPayments.Series.Add(currentSeries);
            CmbUser.ItemsSource = _context.Employees.ToList();
            CmbDiagram.ItemsSource = Enum.GetValues(typeof(SeriesChartType));
        }


        private void UpdateChart(object sender, SelectionChangedEventArgs e)
        {
            if (CmbUser.SelectedItem is Employees currentUser && CmbDiagram.SelectedItem is SeriesChartType currentType)
            {
                Series currentSeries = ChartPayments.Series.FirstOrDefault();
                currentSeries.ChartType = currentType;
                currentSeries.Points.Clear();

                // Группируем завершённые работы по названию работы, выполнявшиеся текущим сотрудником
                var worksGrouped = _context.CompletedWorks
                    .Where(w => w.EmployeeID == currentUser.ID)
                    .GroupBy(w => w.Works.WorkName)
                    .Select(g => new
                    {
                        WorkName = g.Key,
                        TotalPrice = g.Sum(w => w.Works.Price ?? 0) // учитываем, если Price может быть null
                    })
                    .ToList();

                foreach (var item in worksGrouped)
                {
                    currentSeries.Points.AddXY(item.WorkName, item.TotalPrice);
                }
            }

        }

        private void buttonExcel_Click(object sender, RoutedEventArgs e)
        {
            // Получаем список пользователей, отсортированных по ФИО
            var allUsers = _context.Employees.OrderBy(u => u.FirstName).ThenBy(u => u.LastName).ToList();

            // Создаем новый экземпляр Excel приложения
            var application = new Excel.Application();

            // Устанавливаем количество листов, равное количеству пользователей
            application.SheetsInNewWorkbook = allUsers.Count;
            Excel.Workbook workbook = application.Workbooks.Add(Type.Missing);

            // Запускаем цикл по пользователям
            for (int i = 0; i < allUsers.Count; i++)
            {
                // Устанавливаем начальный индекс строки
                int startRowIndex = 1;

                // Получаем текущий рабочий лист
                Excel.Worksheet worksheet = application.Worksheets.Item[i + 1];

                // Формируем имя листа, используя ФИО пользователя
                string sheetName = $"{allUsers[i].FirstName} {allUsers[i].LastName}";
                int counter = 1;
                string originalSheetName = sheetName;

                // Проверяем, существует ли лист с таким именем, если да, добавляем индекс
                while (worksheetExists(workbook, sheetName))
                {
                    sheetName = $"{originalSheetName} ({counter++})";
                }

                // Устанавливаем уникальное имя листа
                worksheet.Name = sheetName;

                // Добавляем названия колонок и форматируем их
                worksheet.Cells[1][startRowIndex] = "Дата платежа";
                worksheet.Cells[2][startRowIndex] = "Название";
                worksheet.Cells[3][startRowIndex] = "Стоимость";
                worksheet.Cells[4][startRowIndex] = "Количество";
                worksheet.Cells[5][startRowIndex] = "Сумма";

                // Форматируем заголовки таблицы
                Excel.Range columnHeaderRange = worksheet.Range[worksheet.Cells[1][1], worksheet.Cells[5][1]];
                columnHeaderRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                columnHeaderRange.Font.Bold = true;
                startRowIndex++;

                // Группируем платежи текущего пользователя по категориям и сортируем их по дате и названию категории
                var userCategories = allUsers[i].CompletedWorks
                    .SelectMany(cw => cw.Orders.Payments) // Корректно обращаемся к заказам через CompletedWorks
                    .OrderBy(p => p.PaymentDate)
                    .GroupBy(p => p.PaymentMethods.PaymentMethodName)
                    .ToList();

                // Вложенный цикл по категориям платежей
                foreach (var groupCategory in userCategories)
                {
                    // Добавляем строку с названием категории
                    Excel.Range headerRange = worksheet.Range[worksheet.Cells[1][startRowIndex], worksheet.Cells[5][startRowIndex]];
                    headerRange.Merge();
                    headerRange.Value = groupCategory.Key;
                    headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    headerRange.Font.Italic = true;
                    startRowIndex++;

                    // Вложенный цикл по платежам, расчет величины платежа по каждой категории
                    foreach (var payment in groupCategory)
                    {
                        worksheet.Cells[1][startRowIndex] = string.Format("{0:dd.MM.yyyy}", payment.PaymentDate);
                        worksheet.Cells[2][startRowIndex] = payment.Orders.Clients.FirstName; // Имя клиента (можно заменить на нужное поле)
                        worksheet.Cells[3][startRowIndex] = payment.Amount;
                        (worksheet.Cells[3][startRowIndex] as Excel.Range).NumberFormat = "0.00";
                        worksheet.Cells[4][startRowIndex] = 1; // Если количество не указано, можно установить по умолчанию как 1
                        worksheet.Cells[5][startRowIndex].Formula = $"=C{startRowIndex}*D{startRowIndex}";
                        (worksheet.Cells[5][startRowIndex] as Excel.Range).NumberFormat = "0.00";
                        startRowIndex++;
                    }

                    // Добавляем строку с итоговой суммой по категории
                    Excel.Range sumRange = worksheet.Range[worksheet.Cells[1][startRowIndex], worksheet.Cells[4][startRowIndex]];
                    sumRange.Merge();
                    sumRange.Value = "ИТОГО: ";
                    sumRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                    // Рассчитываем общую сумму
                    worksheet.Cells[5][startRowIndex].Formula = $"=SUM(E{startRowIndex - groupCategory.Count()}:" +
                        $"E{startRowIndex - 1})";

                    sumRange.Font.Bold = worksheet.Cells[5][startRowIndex].Font.Bold = true;
                    startRowIndex++;

                    // Добавляем границы таблицы
                    Excel.Range rangeBorders = worksheet.Range[worksheet.Cells[1][1], worksheet.Cells[5][startRowIndex - 1]];
                    rangeBorders.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle =
                        rangeBorders.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle =
                        rangeBorders.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle =
                        rangeBorders.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle =
                        rangeBorders.Borders[Excel.XlBordersIndex.xlInsideHorizontal].LineStyle =
                        rangeBorders.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle =
                        Excel.XlLineStyle.xlContinuous;

                    // Устанавливаем автоширину всех столбцов
                    worksheet.Columns.AutoFit();
                }

                // Разрешаем отобразить таблицу по завершении экспорта
                application.Visible = true;
            }

            // Сохраняем файл Excel в нужном месте
            string filePath = @"C:\Users\Ivan\Downloads\PaymentsReport.xlsx";
            workbook.SaveAs(filePath);
        }

        // Метод для проверки, существует ли лист с таким именем
        private bool worksheetExists(Excel.Workbook workbook, string sheetName)
        {
            foreach (Excel.Worksheet sheet in workbook.Sheets)
            {
                if (sheet.Name == sheetName)
                {
                    return true;
                }
            }
            return false;
        }

        private void buttonWord_Click(object sender, RoutedEventArgs e)
        {
            // Получаем всех сотрудников
            var allEmployees = _context.Employees.ToList();

            // Создаем новый документ Word
            var application = new Word.Application();
            Word.Document document = application.Documents.Add();

            // Цикл по каждому сотруднику
            foreach (var employee in allEmployees)
            {
                // Создаем параграф с именем сотрудника
                Word.Paragraph employeeParagraph = document.Paragraphs.Add();
                Word.Range employeeRange = employeeParagraph.Range;
                employeeRange.Text = $"{employee.FirstName} {employee.LastName}";
                employeeParagraph.set_Style("Заголовок");
                employeeRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                employeeRange.InsertParagraphAfter();

                // Создаем таблицу для платежей
                Word.Paragraph tableParagraph = document.Paragraphs.Add();
                Word.Range tableRange = tableParagraph.Range;
                Word.Table paymentsTable = document.Tables.Add(tableRange, 1, 2);
                paymentsTable.Borders.InsideLineStyle = paymentsTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                paymentsTable.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                // Добавляем заголовки колонок
                paymentsTable.Cell(1, 1).Range.Text = "Категория";
                paymentsTable.Cell(1, 2).Range.Text = "Сумма расходов";
                paymentsTable.Rows[1].Range.Font.Name = "Times New Roman";
                paymentsTable.Rows[1].Range.Font.Size = 14;
                paymentsTable.Rows[1].Range.Bold = 1;
                paymentsTable.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                // Цикл по всем заказам сотрудника
                foreach (var completedWork in employee.CompletedWorks)
                {
                    var order = completedWork.Orders;

                    // Для каждого заказа находим связанные платежи
                    foreach (var payment in order.Payments)
                    {
                        // Добавляем строку в таблицу
                        var row = paymentsTable.Rows.Add();
                        row.Cells[1].Range.Text = $"Заказ {order.ID}";
                        row.Cells[2].Range.Text = $"{payment.Amount:N2} руб.";
                        row.Cells[1].Range.Font.Name = "Times New Roman";
                        row.Cells[1].Range.Font.Size = 12;
                        row.Cells[2].Range.Font.Name = "Times New Roman";
                        row.Cells[2].Range.Font.Size = 12;
                    }
                }

                // Добавляем максимальный платеж
                var maxPayment = employee.CompletedWorks
                    .SelectMany(cw => cw.Orders.Payments)
                    .OrderByDescending(p => p.Amount)
                    .FirstOrDefault();

                if (maxPayment != null)
                {
                    Word.Paragraph maxPaymentParagraph = document.Paragraphs.Add();
                    Word.Range maxPaymentRange = maxPaymentParagraph.Range;
                    maxPaymentRange.Text = $"Самый дорогостоящий платеж - {maxPayment.Amount:N2} руб. от {maxPayment.PaymentDate:dd.MM.yyyy}";
                    maxPaymentParagraph.set_Style("Подзаголовок");
                    maxPaymentRange.Font.Color = Word.WdColor.wdColorDarkRed;
                    maxPaymentRange.InsertParagraphAfter();
                }

                // Добавляем минимальный платеж
                var minPayment = employee.CompletedWorks
                    .SelectMany(cw => cw.Orders.Payments)
                    .OrderBy(p => p.Amount)
                    .FirstOrDefault();

                if (minPayment != null)
                {
                    Word.Paragraph minPaymentParagraph = document.Paragraphs.Add();
                    Word.Range minPaymentRange = minPaymentParagraph.Range;
                    minPaymentRange.Text = $"Самый дешевый платеж - {minPayment.Amount:N2} руб. от {minPayment.PaymentDate:dd.MM.yyyy}";
                    minPaymentParagraph.set_Style("Подзаголовок");
                    minPaymentRange.Font.Color = Word.WdColor.wdColorDarkGreen;
                    minPaymentRange.InsertParagraphAfter();
                }

                // Добавляем разрыв страницы после каждого сотрудника
                if (employee != allEmployees.LastOrDefault())
                {
                    document.Words.Last.InsertBreak(Word.WdBreakType.wdPageBreak);
                }
            }

            // Разрешаем отображение таблицы по завершении экспорта
            application.Visible = true;

            // Сохраняем документ в формате .docx и .pdf
            document.SaveAs2(@"C:\Users\Ivan\Downloads\Payments.docx");
            document.SaveAs2(@"C:\Users\Ivan\Downloads\Payments.pdf", Word.WdExportFormat.wdExportFormatPDF);
        }
    }
}
