using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Parser.FileParser
{
    public interface IFileParser
    {
        Task<Dictionary<string, int>> Parse(string filePath, CancellationToken cancellationToken = default);
    }
}