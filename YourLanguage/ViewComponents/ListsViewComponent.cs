using AppCore.Business.Models.Results;
using Business.Services.Bases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace YourLanguage.ViewComponents
{
    public class ListsViewComponent: ViewComponent
    {
        private readonly IUserWordService _userWordService;
        public ListsViewComponent(IUserWordService userWordService)
        {
            _userWordService = userWordService;
        }
        public ViewViewComponentResult Invoke()
        {
            var task = _userWordService.GetListsAsync(User.Identity.Name);
            var result = task.Result;
            if (result.Status == ResultStatus.Exception)
                throw new Exception();
            return View(result.Data);
        }
    }
}
