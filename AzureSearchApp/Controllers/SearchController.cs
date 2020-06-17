using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureSearchApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Configuration;

namespace AzureSearchApp.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private static SearchServiceClient _serviceClient;
        private static ISearchIndexClient _indexClient;
        private static IConfigurationBuilder _builder;
        private static IConfigurationRoot _configuration;
        //[HttpPost]
        //public ActionResult

        private void InitSearch()
        {
            _builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            _configuration = _builder.Build();

            string searchServiceName = _configuration["SearchServiceName"];
            string queryApiKey = _configuration["SearchServiceQueryApiKey"];

            _serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(queryApiKey));
            _indexClient = _serviceClient.Indexes.GetClient("hotels");
        }

        private async Task<ActionResult> RunQueryAsync(SearchData model, int page, int leftMostPage, string catFilter, string ameFilter)
        {
            InitSearch();

            string facetFilter = "";

            if(catFilter.Length > 0 && ameFilter.Length > 0)
            {
                facetFilter = $"";
            }
            else
            {
                facetFilter = $"";
            }

            var parameters = new SearchParameters
            {
                Filter = facetFilter,
                Facets = new List<string> { "Category, count:20", "Tags, count:20"},
                Select = new[] { "HotelName", "Description", "Category", "Tags" },
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

            return CreatedAtAction("Index", model);
        }

        [Route("api/search")]
        [HttpPost]
        public async Task<ActionResult> Search(SearchData model)
        {
            try
            {
                if (model.SearchText == null)
                {
                    model.SearchText = "";
                }

                await RunQueryAsync(model, 0, 0, "", "");
            }
            catch
            {
                return CreatedAtAction("Error", new ErrorViewModel { RequestId = "1" });

            }
            return CreatedAtAction("search", model);
        }

        [HttpGet]
        [Route("api/suggest")]
        public async Task<ActionResult> Suggest(bool highlights, bool fuzzy, string term)
        {
            InitSearch();

            var parameters = new SuggestParameters()
            {
                UseFuzzyMatching = fuzzy,
                Top = 8
            };

            if (highlights)
            {
                parameters.HighlightPreTag = "<b>";
                parameters.HighlightPostTag = "</b>";
            }

            DocumentSuggestResult<Hotel> suggestResult = await _indexClient.Documents.SuggestAsync<Hotel>(term, "sg", parameters);

            List<string> suggestions = suggestResult.Results.Select(x => x.Text).ToList();

            return new JsonResult(suggestions);
        }
        [HttpPost]
        [Route("api/facet")]
        public async Task<ActionResult> Facet(SearchData model)
        {
            try
            {
                string catFilter;
                string ameFilter;

                if(model.categoryFilter != null)
                {
                    catFilter = model.categoryFilter;
                }
                else
                {
                    catFilter = "";
                }

                if(model.amenityFilter != null)
                {
                    ameFilter = model.amenityFilter;
                }
                else
                {
                    ameFilter = "";
                }

                //model.SearchText = 
                await RunQueryAsync(model, 0, 0, catFilter, ameFilter);
            }
            catch
            {
                return CreatedAtAction("error", new ErrorViewModel());
            }

            return CreatedAtAction("facet", model);
        }
    }
}
