using System;
using System.Collections.Generic;
using CoptLib.Scripting.Patterns;
using Xunit;
using Xunit.Abstractions;

namespace CoptTest;

public class Patterns(ITestOutputHelper output)
{
    [Theory]
    [InlineData("int, int > int")]
    [InlineData("int, set > string")]
    [InlineData("int, set{string} > string")]
    public void Specification_FromString(string specification)
    {
        var actual = StringPatternSpecification.Parse(specification);
    }
    
    [Fact]
    public void Lambda_FromString()
    {
        const string specText = "int, set{string} > string";
        const string exprText = "!_1.Contains(\"ar\") " +
                                    "? ((_0 < 12 && _0 > 2) " +
                                        "? (_0 % 2 == 0 ? \"en\" : \"cop\") " +
                                        ": \"cop\") " +
                                    ": \"ar\"";
        
        var spec = StringPatternSpecification.Parse(specText);
        var lambda = spec.ToLambda(exprText);

        var result = lambda.Invoke(10, new HashSet<string>(["ar"]));
        Assert.Equal("ar", result);

        result = lambda.Invoke(10, new HashSet<string>());
        Assert.Equal("en", result);

        result = lambda.Invoke(11, new HashSet<string>());
        Assert.Equal("cop", result);

        result = lambda.Invoke(16, new HashSet<string>());
        Assert.Equal("cop", result);
    }

    [Fact]
    public void Pattern_DynamicExpresso()
    {
        const string exprStr = "!c.Contains(\"ar\") " +
                                    "? ((r < 12 && r > 2) " +
                                        "? (r % 2 == 0 ? \"en\" : \"cop\") " +
                                        ": \"cop\") " +
                                    ": \"ar\"";
        
        DynamicExpressoPattern<Func<int, HashSet<string>, string>> pattern = new(exprStr, ["r", "c"]);
        var func = pattern.GetDelegate();

        var result = func(10, ["ar"]);
        Assert.Equal("ar", result);

        result = func(10, []);
        Assert.Equal("en", result);

        result = func(11, []);
        Assert.Equal("cop", result);

        result = func(16, []);
        Assert.Equal("cop", result);
    }
}