using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetCoreUploadDemo.Models;

namespace NetCoreUploadDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private static IConfiguration configuration;
        private string repo;
        private string ambiente;
        private string correo;
        public HomeController(ILogger<HomeController> logger, IConfiguration iconfiguration)
        {
            _logger = logger;
            configuration = iconfiguration;
            repo = configuration.GetSection("GitHub").Value;
            ambiente = configuration.GetSection("Ambiente").Value;
            correo = configuration.GetSection("correo").Value;
        }

        public IActionResult Index()
        {
            ViewData["GitHub"] = repo;
            ViewData["Ambiente"] = ambiente;
            ViewData["correo"] = correo;
            //var feature = HttpContext.Features.Get<IHttpConnectionFeature>();
            //string LocalIPAddr = feature?.LocalIpAddress?.ToString();
            var HostName = Dns.GetHostName(); // get container id
            var LocalIP = Dns.GetHostEntry(HostName).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
            var currDate = DateTime.Now.ToString("dd/MM/yyyy");
            ViewData["IpLocal"] = LocalIP;
            ViewData["currDate"] = currDate;
            return View();
        }

        public IActionResult Images(string file="")
        {
            ViewData["GitHub"] = repo;

            if (file.Length > 0)
            {
                System.IO.File.Delete(@"wwwroot/images/"+file);
            }

            string[] fileEntries = Directory.GetFiles(@"wwwroot/images").Select(file => Path.GetFileName(file)).ToArray(); ;
            return View(fileEntries);
        }

        public async Task<JsonResult> Upload(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var fileName = Path.GetFileName(formFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/images", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            return new JsonResult(new { count = files.Count, size });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
