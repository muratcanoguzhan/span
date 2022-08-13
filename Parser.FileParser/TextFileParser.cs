using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Parser.FileParser
{
    public class TextFileParser : IFileParser
    {
        public async Task<Dictionary<string, int>> Parse(string filePath, CancellationToken cancellationToken = default)
        {
            var dic = new Dictionary<string, int>();
            string line;
            using (var file = new StreamReader(filePath))
            {
                while ((line = await file.ReadLineAsync()) != null)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    var words = line.Split(" ").Where(x => !string.IsNullOrWhiteSpace(x));
                    foreach (var word in words)
                    {
                        if (dic.ContainsKey(word))
                        {
                            dic[word] = dic[word] + 1;
                        }
                        else { dic[word] = 1; }
                    }

                }
            }
            return dic;
        }
    }
}
