using NUnit.Framework;
using QuickConnectPlugin.PasswordChanger;

namespace QuickConnectPlugin.Tests.PasswordChanger {

    [TestFixture]
    public class PasswordGeneratorTests {

        [Test]
        public void GenerateLowComplexityPassword() {
            var password = PasswordGenerator.Generate(16, PasswordComplexity.Low);

            Assert.AreEqual(16, password.Length);
            Assert.IsTrue(ContainsLowercaseCharacter(password));
            Assert.IsTrue(ContainsUppercaseCharacter(password));
            Assert.IsFalse(ContainsDigitCharacter(password));
            Assert.IsFalse(ContainsSpecialCharacter(password));
        }

        [Test]
        public void GenerateMediumComplexityPassword() {
            var password = PasswordGenerator.Generate(20, PasswordComplexity.Medium);

            Assert.AreEqual(20, password.Length);
            Assert.IsTrue(ContainsLowercaseCharacter(password));
            Assert.IsTrue(ContainsUppercaseCharacter(password));
            Assert.IsTrue(ContainsDigitCharacter(password));
            Assert.IsFalse(ContainsSpecialCharacter(password));
        }

        [Test]
        public void GenerateHighComplexityPassword() {
            var password = PasswordGenerator.Generate(24, PasswordComplexity.High);

            Assert.AreEqual(24, password.Length);
            Assert.IsTrue(ContainsLowercaseCharacter(password));
            Assert.IsTrue(ContainsUppercaseCharacter(password));
            Assert.IsTrue(ContainsDigitCharacter(password));
            Assert.IsTrue(ContainsSpecialCharacter(password));
        }

        [Test]
        public void GenerateThrowsForInvalidLength() {
            Assert.Throws<System.ArgumentOutOfRangeException>(
                delegate {
                    PasswordGenerator.Generate(3, PasswordComplexity.Low);
                }
            );
        }

        private static bool IsSpecialCharacter(char value) {
            return !char.IsLetterOrDigit(value);
        }

        private static bool ContainsLowercaseCharacter(string value) {
            foreach (var character in value) {
                if (char.IsLower(character)) {
                    return true;
                }
            }
            return false;
        }

        private static bool ContainsUppercaseCharacter(string value) {
            foreach (var character in value) {
                if (char.IsUpper(character)) {
                    return true;
                }
            }
            return false;
        }

        private static bool ContainsDigitCharacter(string value) {
            foreach (var character in value) {
                if (char.IsDigit(character)) {
                    return true;
                }
            }
            return false;
        }

        private static bool ContainsSpecialCharacter(string value) {
            foreach (var character in value) {
                if (IsSpecialCharacter(character)) {
                    return true;
                }
            }
            return false;
        }
    }
}
