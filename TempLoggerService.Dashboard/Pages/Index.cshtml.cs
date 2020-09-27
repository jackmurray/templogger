using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace TempLoggerService.Dashboard.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private IConfiguration _configuration;
        public string ApiServer {get; private set;}

        [BindProperty(SupportsGet = true)]
        public string Device {get;set;}

        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public void OnGet()
        {
            ApiServer = _configuration.GetSection("ApiServer").Value;
            if (String.IsNullOrEmpty(Device)) { Device = "rpi1"; } // Set the default device to be 'rpi1'
        }
    }
}
