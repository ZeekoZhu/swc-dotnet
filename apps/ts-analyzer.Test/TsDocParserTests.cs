using FluentAssertions;

namespace Zeeko.TsAnalyzer.Test;

[UsesVerify]
public class TsDocParserTests
{
    [Fact]
    public Task TypeAlias()
    {
        var src = @"
type Bar = string;
";
        var parser = new TsDocParser();
        var doc = parser.Parse("type-alias.ts", src);
        doc.Declarations.Should().NotBeEmpty();
        return Verify(doc);
    }

    [Fact]
    public Task TypeAliasWithComment()
    {
        var src = @"
// foo bar
type Foo = string;
// baz
";
        var parser = new TsDocParser();
        var doc = parser.Parse("type-alias-with-comments.ts", src);
        doc.Declarations.Should().NotBeEmpty();
        return Verify(doc);
    }
}