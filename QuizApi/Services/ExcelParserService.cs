using OfficeOpenXml;
using QuizApi.Models;

namespace QuizApi.Services
{
    public class ExcelParserService
    {
        public List<Question> ParseExcel(Stream fileStream)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // For EPPlus
            using var package = new ExcelPackage(fileStream);
            var worksheet = package.Workbook.Worksheets[0];
            var questions = new List<Question>();
            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++) // Assuming row 1 is header
            {
                var question = new Question
                {
                    Text = worksheet.Cells[row, 1].Text,
                    Options = new List<string>
                    {
                        worksheet.Cells[row, 2].Text, // Option A
                        worksheet.Cells[row, 3].Text, // Option B
                        worksheet.Cells[row, 4].Text, // Option C
                        worksheet.Cells[row, 5].Text  // Option D
                    },
                    CorrectOptionIndex = int.Parse(worksheet.Cells[row, 6].Text) // Correct option index (0-based)
                };
                questions.Add(question);
            }

            return questions;
        }
    }
}