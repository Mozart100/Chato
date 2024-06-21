using FluentAssertions;

namespace Chato.Automation.Extensions
{
    internal static class ByteArrayExtensions
    {

        public static void IsEqualsShould(this byte[] source, byte[] target)
        {
            source.Length.Should().Be(target.Length);

            for (int i = 0; i < target.Length; i++)
            {
                var result = source[i] == target[i];
                result.Should().BeTrue();
            }
        }
    }
}
