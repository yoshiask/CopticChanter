using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CoptTest;

public static class Helpers
{
    public static void MemberwiseAssertEqual(object? expected, object? actual)
    {
        if (expected is not null)
        {
            Assert.IsType(expected.GetType(), actual);

            // Check if collections are equal
            if (expected is IEnumerable<object> expectedCollection)
            {
                foreach (var (ex, ac) in expectedCollection.Zip((IEnumerable<object>)actual))
                {
                    Assert.Equal(ex, ac);
                }
            }

            var expectedProps = expected.GetType().GetProperties();
            var actualProps = actual.GetType().GetProperties();
            foreach (var (ex, ac) in expectedProps.Zip(actualProps))
            {
                if (ex.Name == "Item")
                    continue;

                Assert.Equal(ex.GetValue(expected), ac.GetValue(actual));
            }
        }
        else
        {
            Assert.Equal(expected, actual);
        }
    }
}