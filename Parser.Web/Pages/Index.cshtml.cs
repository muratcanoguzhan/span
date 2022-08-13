using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Parser.FileParser;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Parser.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IEnumerable<IFileParser> _fileParsers;
        private static CancellationTokenSource _cts;

        [ViewData]
        public string DefaultParser { get; set; }
        [ViewData]
        public string MemoryParser { get; set; }

        public string CountSort { get; set; }
        public static Dictionary<string, int> WordsWithCount { get; set; }

        public IndexModel(IWebHostEnvironment environment,
                         IEnumerable<IFileParser> fileParsers)
        {
            _environment = environment;
            _fileParsers = fileParsers;
        }

        public async Task OnGetAsync(string sortOrder)
        {
            if (WordsWithCount == null)
            {
                WordsWithCount = new Dictionary<string, int>();
            }

            CountSort = sortOrder == "Count" ? "count_desc" : "Count";

            switch (CountSort)
            {
                case "count_desc":
                    WordsWithCount = WordsWithCount.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                    break;
                case "Count":
                    WordsWithCount = WordsWithCount.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                    break;
                default:
                    break;
            }


        }

        [BindProperty]
        public IFormFile Upload { get; set; }
        public async Task OnPostUploadAsync()
        {
            if (Upload == null) return;

            _cts = new CancellationTokenSource();

            var file = Path.Combine(_environment.WebRootPath, "uploads", Upload.FileName);
            using (var fileStream = new FileStream(file, FileMode.Create))
            {
                await Upload.CopyToAsync(fileStream, _cts.Token);
            }

            foreach (var _fileParser in _fileParsers)
            {
                Stopwatch sw = Stopwatch.StartNew();
                WordsWithCount = await _fileParser.Parse(file, _cts.Token);
                sw.Stop();
                if (_fileParser is TextFileParser)
                {
                    DefaultParser = sw.Elapsed.TotalSeconds.ToString();
                }
                else if (_fileParser is TextFileMemoryParser)
                {
                    MemoryParser = sw.Elapsed.TotalSeconds.ToString();
                }
            }
        }

        public async Task OnPostCancelAsync()
        {
            if (_cts != null)
            {
                _cts.Cancel();
            }
        }
    }
}
