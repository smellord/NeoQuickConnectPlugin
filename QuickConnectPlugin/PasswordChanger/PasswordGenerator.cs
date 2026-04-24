using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace QuickConnectPlugin.PasswordChanger {

    public static class PasswordGenerator {

        private const string LowercaseCharacters = "abcdefghijkmnopqrstuvwxyz";
        private const string UppercaseCharacters = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        private const string DigitCharacters = "23456789";
        private const string SpecialCharacters = "!@#$%^&*()-_=+[]{}?";

        public static string Generate(int length, PasswordComplexity complexity) {
            if (length < 4) {
                throw new ArgumentOutOfRangeException("length", "Password length must be at least 4 characters.");
            }

            var requiredSets = new List<string>();
            requiredSets.Add(LowercaseCharacters);
            requiredSets.Add(UppercaseCharacters);

            if (complexity == PasswordComplexity.Medium || complexity == PasswordComplexity.High) {
                requiredSets.Add(DigitCharacters);
            }

            if (complexity == PasswordComplexity.High) {
                requiredSets.Add(SpecialCharacters);
            }

            if (length < requiredSets.Count) {
                throw new ArgumentOutOfRangeException("length", "Password length is too short for the selected complexity.");
            }

            var passwordCharacters = new List<char>();
            var allowedCharactersBuilder = new StringBuilder();

            foreach (var characterSet in requiredSets) {
                allowedCharactersBuilder.Append(characterSet);
                passwordCharacters.Add(GetRandomCharacter(characterSet));
            }

            var allowedCharacters = allowedCharactersBuilder.ToString();

            while (passwordCharacters.Count < length) {
                passwordCharacters.Add(GetRandomCharacter(allowedCharacters));
            }

            Shuffle(passwordCharacters);

            return new string(passwordCharacters.ToArray());
        }

        private static char GetRandomCharacter(string characters) {
            var index = GetRandomNumber(characters.Length);
            return characters[index];
        }

        private static int GetRandomNumber(int exclusiveUpperBound) {
            var bytes = new byte[4];
            using (var randomNumberGenerator = RandomNumberGenerator.Create()) {
                randomNumberGenerator.GetBytes(bytes);
            }

            var value = BitConverter.ToUInt32(bytes, 0);
            return (int)(value % exclusiveUpperBound);
        }

        private static void Shuffle(IList<char> value) {
            for (int i = value.Count - 1; i > 0; i--) {
                var swapIndex = GetRandomNumber(i + 1);
                var current = value[i];
                value[i] = value[swapIndex];
                value[swapIndex] = current;
            }
        }
    }
}
