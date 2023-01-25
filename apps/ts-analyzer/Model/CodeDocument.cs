namespace Zeeko.TsAnalyzer.Model;

public record CodeLocation(uint Line, uint Column);

/// <summary>
/// 
/// </summary>
/// <param name="Name"></param>
/// <param name="File"></param>
/// <param name="Start">
/// The start location of the identifier in the file,
/// including the first char of the code.
/// </param>
/// <param name="End">
/// The end location of the identifier in the file,
/// excluding the last char of the code.
/// </param>
public record Identifier(
    string Name,
    string File,
    CodeLocation Start,
    CodeLocation End);

/// <summary>
/// The declaration of a identifier.
/// </summary>
public class Declaration
{
// todo:  quick diff declaration
    /// <summary>
    /// The file where the declaration is located.
    /// </summary>
    public string File { get; set; }

    /// <summary>
    /// The identifier that is declared.
    /// </summary>
    public Identifier Identifier { get; set; }
    public IList<Identifier> References { get; set; }
}

public class CodeDocument
{
    public IList<Declaration> Declarations { get; set; }
    public IList<Identifier> Imports { get; set; }
}
