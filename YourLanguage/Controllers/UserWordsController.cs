using AppCore.Business.Models.Ordering;
using AppCore.Business.Models.Paging;
using AppCore.Business.Models.Results;
using Business.Models.Filters;
using Business.Models.Reports;
using Business.Services.Bases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YourLanguage.Models;
using YourLanguage.Settings;

namespace YourLanguage.Controllers
{
    public class UserWordsController : Controller
    {
        private readonly IUserWordService _userWordService;
        private readonly IDomainService _domainService;
        public UserWordsController(IUserWordService userWordService, IDomainService domainService)
        {
            _userWordService = userWordService;
            _domainService = domainService;
        }
        public IActionResult Index()
        {
            var filter = new WordFilterModel()
            {
                IsActive = null
            };
            var page = new PageModel()
            {
                PageNumber = 1,
                RecordsCountPerPage = AppSettings.RecordsCountPerPage
            };
            var order = new OrderModel();
            var result = _userWordService.GetReport(filter, page, order);
            if (result.Status == ResultStatus.Exception)
                throw new Exception(result.Message);

            double recordsCount = page.RecordsCount;
            double recordsCountPerPage = page.RecordsCountPerPage;
            double pageCount = Math.Ceiling(recordsCount / recordsCountPerPage);
            if (pageCount == 0)
                pageCount = 1;
            List<SelectListItem> pageNumberSelectList = new List<SelectListItem>();
            for (int pageNumber = 1; pageNumber <= pageCount; pageNumber++)
            {
                pageNumberSelectList.Add(new SelectListItem()
                {
                    Value = pageNumber.ToString(),
                    Text = pageNumber.ToString()
                });
            }

            var viewModel = new WordReportViewModel()
            {
                Words = result.Data,
                Filter = filter,
                PageNumbers = new SelectList(pageNumberSelectList, "Value", "Text")
            };
            var domains = new List<SelectListItem>();
            var domainResult = _domainService.Query();
            foreach (var item in domainResult)
            {
                domains.Add(new SelectListItem()
                {
                    Value = item.Id.ToString(),
                    Text = item.Name
                });
            }
            var selectedDomain = viewModel.Filter.DomainId;
            ViewBag.Domains = new SelectList(domains, "Value", "Text");
            ViewBag.RecordCount = recordsCount;
            var sessionPage = new PageModel()
            {
                RecordsCountPerPage = 1000
            };
            var query = _userWordService.GetReport(filter, sessionPage, order).Data;
            WordReportViewModel sessionViewModel = new WordReportViewModel();
            sessionViewModel.Words = query;
            sessionViewModel.OrderByDirectionAscending = true;
            sessionViewModel.OrderByExpression = null;
            sessionViewModel.Filter = filter;
            var quizJson = JsonConvert.SerializeObject(sessionViewModel);
            HttpContext.Session.SetString("userwordreport", quizJson);
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult Index(WordReportViewModel viewModel)
        {

            if (ModelState.IsValid)
            {
                HttpContext.Session.GetString("userwordreport");
                var session = JsonConvert.DeserializeObject<WordReportViewModel>(HttpContext.Session.GetString("userwordreport"));
                #region deneme 1
                //var page = new PageModel();
                //if (session.OrderByExpression == viewModel.OrderByExpression || session.OrderByDirectionAscending == viewModel.OrderByDirectionAscending)
                //{
                //    var tempPage = new PageModel()
                //    {
                //        PageNumber = viewModel.PageNumber,
                //        RecordsCountPerPage = AppSettings.RecordsCountPerPage
                //    };
                //    page = tempPage;
                //}
                //else
                //{
                //    var templePage = new PageModel()
                //    {
                //        PageNumber = 1,
                //        RecordsCountPerPage = AppSettings.RecordsCountPerPage
                //    };
                //    page = templePage;
                //}
                #endregion
                var page = new PageModel()
                {
                    PageNumber = viewModel.PageNumber,
                    RecordsCountPerPage = AppSettings.RecordsCountPerPage
                };
                var order = new OrderModel()
                {
                    Expression = viewModel.OrderByExpression,
                    DirectionAscending = viewModel.OrderByDirectionAscending
                };
                var result = _userWordService.GetReport(viewModel.Filter, page, order);
                if (result.Status == ResultStatus.Exception)
                    throw new Exception(result.Message);
                viewModel.Words = result.Data;

                double recordsCount = page.RecordsCount;
                double recordsCountPerPage = AppSettings.RecordsCountPerPage;
                double pageCount = Math.Ceiling(recordsCount / recordsCountPerPage);
                if (pageCount == 0)
                    pageCount = 1;
                List<SelectListItem> pageNumberSelectList = new List<SelectListItem>();
                for (int pageNumber = 1; pageNumber <= pageCount; pageNumber++)
                {
                    pageNumberSelectList.Add(new SelectListItem()
                    {
                        Value = pageNumber.ToString(),
                        Text = pageNumber.ToString()
                    });
                }
                var domains = new List<SelectListItem>();
                var domainResult = _domainService.Query();
                foreach (var item in domainResult)
                {
                    domains.Add(new SelectListItem()
                    {
                        Value = item.Id.ToString(),
                        Text = item.Name
                    });
                }
                var selectedDomain = viewModel.Filter.DomainId;
                ViewBag.Domains = new SelectList(domains, "Value", "Text", selectedDomain);
                viewModel.PageNumbers = new SelectList(pageNumberSelectList, "Value", "Text");
                ViewBag.RecordCount = recordsCount;
                var sessionPage = new PageModel()
                {
                    RecordsCountPerPage = 1000
                };
                var query = _userWordService.GetReport(viewModel.Filter, sessionPage, order).Data;
                WordReportViewModel sessionViewModel = new WordReportViewModel();
                sessionViewModel.Words = query;
                sessionViewModel.OrderByExpression = viewModel.OrderByExpression;
                sessionViewModel.OrderByDirectionAscending = viewModel.OrderByDirectionAscending;
                sessionViewModel.Filter = viewModel.Filter;
                var quizJson = JsonConvert.SerializeObject(sessionViewModel);
                HttpContext.Session.SetString("userwordreport", quizJson);
            }
            return PartialView("_Words", viewModel);
        }
        public IActionResult Export()
        {
            #region deneme 1
            //WordReportViewModel viewModel = new WordReportViewModel();
            //WordReportViewModel sessionModel;
            //if (HttpContext.Session.GetString("userwordreport") != null)
            //{
            //    var quizJson = HttpContext.Session.GetString("userwordreport");
            //    sessionModel = JsonConvert.DeserializeObject<WordReportViewModel>(quizJson);
            //    viewModel.Words = sessionModel.Words;
            //}
            //return View(viewModel);
            #endregion


            List<UserWordReportExcelModel> viewModel = new List<UserWordReportExcelModel>();
            IEnumerable<UserWordReportExcelModel> sessionModel;
            if (HttpContext.Session.GetString("userwordreport") != null)
            {
                var quizJson = HttpContext.Session.GetString("userwordreport");
                var tempModel = JsonConvert.DeserializeObject<WordReportViewModel>(quizJson);
                var order = new OrderModel()
                {
                    Expression = tempModel.OrderByExpression,
                    DirectionAscending = tempModel.OrderByDirectionAscending
                };

                sessionModel = _userWordService.GetReport(tempModel.Filter, null, order).Data.Select(uw => new UserWordReportExcelModel()
                {
                    Vocable = uw.Vocable,
                    Mean = uw.Mean,
                    Domain = uw.Domain,
                    Active = uw.Active == true ? "Aktif" : "Aktif Değil",
                    Count = uw.Count
                });
                viewModel = sessionModel.ToList();
                var stream = new MemoryStream();
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(stream))
                {
                    var excelWorksheet = package.Workbook.Worksheets.Add("UserWordReport");
                    #region deneme 4 deneme 1
                    //excelWorksheet.Cells["A1"].Value = "Vocable";
                    //excelWorksheet.Cells["B1"].Value = "Mean";
                    //excelWorksheet.Cells["C1"].Value = "Domain";
                    //excelWorksheet.Cells["D1"].Value = "Count";
                    //excelWorksheet.Cells["E1"].Value = "Active";
                    //int row = 2;
                    //foreach (var item in viewModel.Words)
                    //{
                    //    excelWorksheet.Cells[string.Format("A{0}", row)].Value = item.Vocable;
                    //    excelWorksheet.Cells[string.Format("A{0}", row)].Value = item.Mean;
                    //    excelWorksheet.Cells[string.Format("A{0}", row)].Value = item.Domain;
                    //    excelWorksheet.Cells[string.Format("A{0}", row)].Value = item.Count;
                    //    excelWorksheet.Cells[string.Format("A{0}", row)].Value = item.Active;
                    //    row++;
                    //}
                    //excelWorksheet.Cells["A:AZ"].AutoFitColumns();
                    #endregion
                    excelWorksheet.Cells.LoadFromCollection(viewModel, true);
                    package.Save();
                }
                stream.Position = 0;
                string excelname = $"UserWord-{DateTime.Now.ToString("yyyy/MM/dd-HH:ss:ffff")}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelname);
            }
            return RedirectToAction(nameof(Index));
            #region deneme 3
            //WordReportViewModel viewModel = new WordReportViewModel();
            //WordReportViewModel sessionModel;
            //List<byte> fileContentsG = new List<byte>();
            //byte[] fileContents;
            //if (HttpContext.Session.GetString("userwordreport") != null)
            //{
            //    var quizJson = HttpContext.Session.GetString("userwordreport");
            //    sessionModel = JsonConvert.DeserializeObject<WordReportViewModel>(quizJson);
            //    viewModel.Words = sessionModel.Words;
            //    if (viewModel.Words != null && viewModel.Words.ToList().Count() > 0)
            //    {
            //        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //        ExcelPackage excelPackage = new ExcelPackage();
            //        ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add("User/Word Report");
            //        excelWorksheet.Cells["A1"].Value = "Vocable";
            //        excelWorksheet.Cells["B1"].Value = "Mean";
            //        excelWorksheet.Cells["C1"].Value = "Domain";
            //        excelWorksheet.Cells["D1"].Value = "Count";
            //        excelWorksheet.Cells["E1"].Value = "Active";
            //        int row = 2;
            //        foreach (var item in viewModel.Words)
            //        {
            //            excelWorksheet.Cells[string.Format("A{0}", row)].Value = item.Vocable;
            //            excelWorksheet.Cells[string.Format("A{0}", row)].Value = item.Mean;
            //            excelWorksheet.Cells[string.Format("A{0}", row)].Value = item.Domain;
            //            excelWorksheet.Cells[string.Format("A{0}", row)].Value = item.Count;
            //            excelWorksheet.Cells[string.Format("A{0}", row)].Value = item.Active;
            //            row++;
            //        }
            //        excelWorksheet.Cells["A:AZ"].AutoFitColumns();
            //        Response.Clear();
            //        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //        Response.WriteAsJsonAsync(excelPackage.GetAsByteArray());
            //        Response.AppendTrailer("content-disposition", "attachment: filename=MoviesReport.xlsx");
            //        Response.CompleteAsync();
            //        fileContents = excelPackage.GetAsByteArray();
            //        fileContentsG = fileContents.ToList();
            //    }

            //}
            //if (fileContentsG == null || fileContentsG.Count == 0)
            //{
            //    return View("NotFound");
            //}
            //return File(
            //    fileContents: fileContentsG.ToArray(),
            //    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            //    fileDownloadName: "test.xlsx"
            //);
            #endregion
        }
        #region deneme 2
        //public void UserWordExcel()
        //{
        //    WordReportViewModel viewModel = new WordReportViewModel();
        //    WordReportViewModel sessionModel;
        //    if (HttpContext.Session.GetString("userwordreport") != null)
        //    {
        //        var quizJson = HttpContext.Session.GetString("userwordreport");
        //        sessionModel = JsonConvert.DeserializeObject<WordReportViewModel>(quizJson);
        //        viewModel.Words = sessionModel.Words;
        //        if (viewModel.Words != null && viewModel.Words.ToList().Count() > 0)
        //        {
        //            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //            ExcelPackage excelPackage = new ExcelPackage();
        //            ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add("User/Word Report");
        //            excelWorksheet.Cells["A1"].Value = "Vocable";
        //            excelWorksheet.Cells["B1"].Value = "Mean";
        //            excelWorksheet.Cells["C1"].Value = "Domain";
        //            excelWorksheet.Cells["D1"].Value = "Count";
        //            excelWorksheet.Cells["E1"].Value = "Active";
        //            int row = 2;
        //            foreach (var item in viewModel.Words)
        //            {
        //                excelWorksheet.Cells[string.Format("A{0}", row)].Value = item.Vocable;
        //                excelWorksheet.Cells[string.Format("A{0}", row)].Value = item.Mean;
        //                excelWorksheet.Cells[string.Format("A{0}", row)].Value = item.Domain;
        //                excelWorksheet.Cells[string.Format("A{0}", row)].Value = item.Count;
        //                excelWorksheet.Cells[string.Format("A{0}", row)].Value = item.Active;
        //                row++;
        //            }
        //            excelWorksheet.Cells["A:AZ"].AutoFitColumns();
        //            Response.Clear();
        //            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //            Response.WriteAsJsonAsync(excelPackage.GetAsByteArray());
        //            Response.AppendTrailer("content-disposition", "fileDownloadName: test.xlsx");
        //            Response.CompleteAsync();
        //        }
        //    }
        //}
        #endregion
    }
}
