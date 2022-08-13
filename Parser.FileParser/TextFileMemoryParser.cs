using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Parser.FileParser
{
    public class TextFileMemoryParser : IFileParser
    {
        public async Task<Dictionary<string, int>> Parse(string filePath, CancellationToken cancellationToken = default)
        {
            var dic = new Dictionary<string, int>();
            bool goon = true;
            string line;
            var chars = new List<char>();
            using (var file = new StreamReader(filePath))
            {
                Memory<char> memory = new Memory<char>(new char[1]);

                while (goon)
                {
                    await file.ReadAsync(memory, cancellationToken);

                    goon = !file.EndOfStream;

                    if (file.EndOfStream) { chars.Add(memory.Span.ToString()[0]); }

                    if (file.EndOfStream || memory.Span.Contains('\n') || memory.Span.Contains('\r'))
                    {
                        line = string.Create(chars.Count, chars, (x, y) =>
                        {
                            for (int i = 0; i < x.Length; i++)
                            {
                                x[i] = y[i];
                            }
                        });
                        foreach (var word in line.Split(" ").Where(x => !string.IsNullOrWhiteSpace(x)))
                        {
                            if (dic.ContainsKey(word))
                            {
                                dic[word] = dic[word] + 1;
                            }
                            else { dic[word] = 1; }
                        }
                        chars.Clear();
                    }
                    else
                    {
                        chars.Add(memory.Span.ToString()[0]);
                    }
                }
            }
            return dic;
        }
    }
}
