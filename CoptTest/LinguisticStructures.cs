using CoptLib.Writing.Linguistics.XBar;
using Xunit;
using Xunit.Abstractions;

namespace CoptTest;

public class LinguisticStructures(ITestOutputHelper _output)
{

    [Fact]
    public void BinaryTree_GraphViz()
    {
        BinaryNode<string> root = new("XP",
            new("Specifier"),
                new("X'",
                new("X"),
                    new("Complement")
            )
        );

        _output.WriteLine(root.SerializeToDot());
    }
}
