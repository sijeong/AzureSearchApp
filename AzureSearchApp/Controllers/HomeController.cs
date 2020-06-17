using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AzureSearchApp.Models;


using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace AzureSearchApp.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private static SearchServiceClient _serviceClient;
        private static ISearchIndexClient _indexClient;
        private static IConfigurationBuilder _builder;
        private static IConfigurationRoot _configuration;

        public HomeController(ILogger<HomeController> logger)
        {
            //_logger = logger;
        }

        private void InitSearch()
        {
            _builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            _configuration = _builder.Build();

            string searchServiceName = _configuration["SearchServiceName"];
            string queryApiKey = _configuration["SearchServiceQueryApiKey"];

            _serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(queryApiKey));
            _indexClient = _serviceClient.Indexes.GetClient("hotels");
        }

        private async Task<ActionResult> RunQueryAsync(SearchData model, int page, int leftMostPage)
        {
            InitSearch();

            var parameters = new SearchParameters
            {
                Select = new[] { "HotelName", "Description" },
                SearchMode = SearchMode.All,
                Skip = page * GlobalVariables.ResultsPerPage,
                Top = GlobalVariables.ResultsPerPage,
                IncludeTotalResultCount = true,
            };

            model.ResultList = await _indexClient.Documents.SearchAsync<Hotel>(model.SearchText, parameters);
            model.PageCount = ((int)model.ResultList.Count + GlobalVariables.ResultsPerPage - 1) / GlobalVariables.ResultsPerPage;
            model.CurrentPage = page;

            if (page == 0)
            {
                leftMostPage = 0;
            }
            else if (page <= leftMostPage)
            {
                leftMostPage = Math.Max(page - GlobalVariables.PageRangeDelta, 0);
            }
            else if (page >= leftMostPage + GlobalVariables.MaxPageRange - 1)
            {
                leftMostPage = Math.Min(page - GlobalVariables.PageRangeDelta, model.PageCount - GlobalVariables.MaxPageRange);
            }
            model.LeftMostPage = leftMostPage;

            model.PageRange = Math.Min(model.PageCount - leftMostPage, GlobalVariables.MaxPageRange);

            return View("Index", model);
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index([FromForm] SearchData model)
        {
            try
            {
                if (model.SearchText == null)
                {
                    model.SearchText = "";
                }

                await RunQueryAsync(model, 0, 0);

                TempData["page"] = 0;
                TempData["leftMostPage"] = 0;
                TempData["searchfor"] = model.SearchText;
            }
            catch
            {
                return View("Error", new ErrorViewModel { RequestId = "1" });

            }
            return View(model);
        }

        public async Task<ActionResult> Page(SearchData model)
        {
            try
            {
                int page;

                switch (model.Paging)
                {
                    case "prev":
                        page = (int)TempData["page"] - 1;
                        break;
                    case "next":
                        page = (int)TempData["page"] + 1;
                        break;
                    default:
                        page = int.Parse(model.Paging);
                        break;
                }

                int leftMostPage = (int)TempData["leftMostPage"];
                model.SearchText = TempData["searchfor"].ToString();

                await RunQueryAsync(model, page, leftMostPage);

                TempData["page"] = (object)page;
                TempData["searchfor"] = model.SearchText;
                TempData["leftMostPage"] = model.LeftMostPage;
            }
            catch
            {
                return View("Error", new ErrorViewModel { RequestId = "2" });
            }

            return View("Index", model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
