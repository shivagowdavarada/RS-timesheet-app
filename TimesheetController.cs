using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;

namespace TimesheetApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TimesheetController : ControllerBase
    {
        private static readonly string excelFilePath = "timesheet.xlsx";

        [HttpPost("submit")]
        public IActionResult SubmitTimesheet([FromBody] TimesheetEntry timesheetEntry)
        {
            try
            {
                using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
                {
                    var worksheet = package.Workbook.Worksheets["Timesheet"];
                    var rowCount = worksheet.Dimension?.Rows ?? 1;

                    worksheet.Cells[rowCount + 1, 1].Value = timesheetEntry.ProjectName;
                    worksheet.Cells[rowCount + 1, 2].Value = timesheetEntry.Date.ToString("yyyy-MM-dd");
                    worksheet.Cells[rowCount + 1, 3].Value = timesheetEntry.AssignedDate.ToString("yyyy-MM-dd");
                    worksheet.Cells[rowCount + 1, 4].Value = timesheetEntry.AssignPoc;

                    package.Save();
                }

                return Ok("Timesheet submitted successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to submit timesheet: {ex.Message}");
            }
        }

        [HttpGet("entries")]
        public IActionResult GetTimesheetEntries()
        {
            var entries = new List<TimesheetEntry>();

            try
            {
                using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
                {
                    var worksheet = package.Workbook.Worksheets["Timesheet"];
                    var rowCount = worksheet.Dimension?.Rows ?? 0;

                    for (int i = 2; i <= rowCount; i++)
                    {
                        entries.Add(new TimesheetEntry
                        {
                            ProjectName = worksheet.Cells[i, 1].Value?.ToString(),
                            Date = DateTime.Parse(worksheet.Cells[i, 2].Value?.ToString()),
                            AssignedDate = DateTime.Parse(worksheet.Cells[i, 3].Value?.ToString()),
                            AssignPoc = worksheet.Cells[i, 4].Value?.ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to get timesheet entries: {ex.Message}");
            }

            return Ok(entries);
        }
    }

    public class TimesheetEntry
    {
        public string ProjectName { get; set; }
        public DateTime Date { get; set; }
        public DateTime AssignedDate { get; set; }
        public string AssignPoc { get; set; }
    }
}
