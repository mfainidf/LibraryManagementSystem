using System;
using Xunit;
using FluentAssertions;
using Library.Infrastructure.Services;
using System.Reflection;

namespace Library.UnitTests.Services
{
    public class EmailMaskingTests
    {
        private readonly MethodInfo _maskEmailMethod;

        public EmailMaskingTests()
        {
            _maskEmailMethod = typeof(AuthenticationService)
                .GetMethod("MaskEmail", BindingFlags.NonPublic | BindingFlags.Static) 
                ?? throw new InvalidOperationException("MaskEmail method not found");
        }

        private string InvokeMaskEmail(string? email)
        {
            var result = _maskEmailMethod.Invoke(null, new object?[] { email });
            return result?.ToString() ?? email ?? string.Empty;
        }

        [Theory]
        [InlineData("mario.rossi@domain.com", "m***i@domain.com")]
        [InlineData("test@example.com", "t***t@example.com")]
        [InlineData("ab@domain.com", "ab@domain.com")]
        [InlineData("a@domain.com", "a@domain.com")]
        public void MaskEmail_WithValidEmails_ShouldMaskCorrectly(string input, string expected)
        {
            // Act
            var result = InvokeMaskEmail(input);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void MaskEmail_WithEmptyOrNullInput_ShouldReturnSameValue(string? input)
        {
            // Act
            var result = InvokeMaskEmail(input);

            // Assert
            result.Should().Be(input ?? string.Empty);
        }

        [Theory]
        [InlineData("invalidemail")]
        [InlineData("@domain.com")]
        [InlineData("test@")]
        [InlineData("test@@domain.com")]
        public void MaskEmail_WithInvalidEmails_ShouldReturnOriginalValue(string input)
        {
            // Act
            var result = InvokeMaskEmail(input);

            // Assert
            result.Should().Be(input);
        }
    }
}
