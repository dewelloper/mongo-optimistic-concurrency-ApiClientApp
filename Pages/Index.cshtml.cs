using ApiClientApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiClientApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        IntegrationController _integrationController;
        public IndexModel(ILogger<IndexModel> logger, IntegrationController httpClientFactory)
        {
            _logger = logger;
            _integrationController = httpClientFactory;
        }

        
        public async Task OnGet()
        {
            var result = await _integrationController.GetStock();
        }
    }
}
