using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureSearchApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpPost]
        public ActionResult<String> Search(SearchItem searchText)
        {
            return CreatedAtAction("search", searchText.Term);
        }

        [HttpGet]
        public ActionResult<String> Test()
        {
            return CreatedAtAction("test", "hey~");
        }
    }

    public class SearchItem
    {
        public string Term { get; set; }
    }
}
