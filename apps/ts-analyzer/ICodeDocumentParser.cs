using Zeeko.TsAnalyzer.Model;

namespace Zeeko.TsAnalyzer;

public interface ICodeDocumentParser
{
    public CodeDocument Parse(string fileName, string code);
}
