namespace UkrGuru.Sql.Tests;

public class Extens_IsName_Tests
{

    // -----------------------------
    // 1-part names (simple)
    // -----------------------------
    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData(".", false)]
    [InlineData("SELECT 1", false)]
    [InlineData("1", false)]
    [InlineData("_", true)]
    [InlineData("a", true)]
    [InlineData("A", true)]
    [InlineData("_1", true)]
    [InlineData("a1", true)]
    [InlineData("A1", true)]
    [InlineData("A 1", false)]
    [InlineData("[ ]", true)]
    [InlineData("[1]", true)]
    [InlineData("[A 1]", true)]
    public void OnePart_ShouldValidate(string? text, bool expected)
    {
        Assert.Equal(expected, Extens.IsName(text));
    }

    // -----------------------------
    // 2-part names (schema.name)
    // -----------------------------
    [Theory]
    [InlineData(" .A", false)]
    [InlineData("_.A", true)]
    [InlineData("a.A", true)]
    [InlineData("A.A", true)]
    [InlineData("1.A", false)]
    [InlineData("_1.A", true)]
    [InlineData("a1.A", true)]
    [InlineData("A1.A", true)]
    [InlineData("[ ].A", true)]
    [InlineData("[1].A", true)]
    [InlineData("dbo. ", false)]
    [InlineData("dbo._", true)]
    [InlineData("dbo.a", true)]
    [InlineData("dbo.A", true)]
    [InlineData("dbo.1", false)]
    [InlineData("dbo._1", true)]
    [InlineData("dbo.a1", true)]
    [InlineData("dbo.A1", true)]
    [InlineData("dbo.[ ]", true)]
    [InlineData("dbo.[1]", true)]
    public void TwoPart_ShouldValidate(string text, bool expected)
    {
        Assert.Equal(expected, Extens.IsName(text));
    }

    // -----------------------------
    // 3-part names (server.schema.name)
    // -----------------------------
    [Theory]
    [InlineData("srv.dbo.Tbl", true)]
    [InlineData("SRV.DB.TBL", true)]
    [InlineData("1srv.dbo.Tbl", false)]  // invalid first part
    [InlineData("srv.1db.Tbl", false)]  // invalid second part
    [InlineData("srv.db.1Tbl", false)]  // invalid third part
    [InlineData("[srv].[dbo].[tbl]", true)]
    [InlineData("[ ].[1].[A 1]", true)]
    [InlineData("srv.db.", false)]
    [InlineData(".db.tbl", false)]
    [InlineData("srv..tbl", false)]
    [InlineData("srv.db.tbl.more", false)] // 4-part → must be rejected
    public void ThreePart_ShouldValidate(string text, bool expected)
    {
        Assert.Equal(expected, Extens.IsName(text));
    }

    // -----------------------------
    // Leading/trailing space rules
    // -----------------------------
    [Theory]
    [InlineData(" A", false)]
    [InlineData("A ", false)]
    [InlineData("dbo .tbl", false)]
    [InlineData("dbo. tbl", false)]
    [InlineData("dbo.tbl ", false)]
    public void Whitespace_ShouldFail(string text, bool expected)
    {
        Assert.Equal(expected, Extens.IsName(text));
    }
}