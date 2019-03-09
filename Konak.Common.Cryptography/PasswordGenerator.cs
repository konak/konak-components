using Konak.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Konak.Common.Cryptography
{
    public static class PasswordGenerator
    {
        private const int DEFAULT_MIN_PASSWORD_LENGTH = 8;
        private const int DEFAULT_MAX_PASSWORD_LENGTH = 10;

        private static int MIN_PASSWORD_LENGTH = 4;
        private static int MAX_PASSWORD_LENGTH = 4096;

        private static readonly char[] PASSWORD_CHARS_SAFE_LCASE = "abcdefgjkmnpqrstwxy".ToCharArray();
        private static readonly char[] PASSWORD_CHARS_SAFE_UCASE = "ACDEFGHJKLMNPQRSTWXYZ".ToCharArray();
        private static readonly char[] PASSWORD_CHARS_SAFE_NUMERIC = "3456789".ToCharArray();
        private static readonly char[] PASSWORD_CHARS_SAFE_SPECIAL = "~!@#$%&-=+<>".ToCharArray();
        private static readonly char[] PASSWORD_CHARS_SAFE_MIX = "6LMjkm789H3RS48abG6C73efg6AZ34pqrP57cd89JKnstwDE45QF56xy345N897TWXY9".ToCharArray();
                       
        private static readonly char[] PASSWORD_CHARS_LCASE = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        private static readonly char[] PASSWORD_CHARS_UCASE = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private static readonly char[] PASSWORD_CHARS_NUMERIC = "1234567890".ToCharArray();
        private static readonly char[] PASSWORD_CHARS_SPECIAL = "~!@#$%^&*()-_=+|{}<>?.,".ToCharArray();
        private static readonly char[] PASSWORD_CHARS_MIX = "6LMjkm7891UOH3RS48abG6C273efg6A1Z3B4UpqrP57cd829JKnsOhItwVD0E45QF56xy345iNI89VB7TWX1Y9".ToCharArray();

        /// <summary>
        /// Generate password with specified length
        /// </summary>
        /// <param name="length">Length of the password to generate</param>
        /// <returns></returns>
        public static string Generate(int length)
        {
            return Generate(length, length);
        }

        /// <summary>
        /// Generate password
        /// </summary>
        /// <param name="minLength">Minimum length of the password</param>
        /// <param name="maxLength">Maximum length of the password</param>
        /// <param name="useSafeChars">Use only safe characters. Unsafe characters are the characters that can be visually interpreted as others depending on font used in UI. for example: 1, I and l characters.</param>
        /// <param name="useSpecialSymbols">Use special symbols like: !@#$% etc.</param>
        /// <returns></returns>
        public static string Generate(int minLength = DEFAULT_MIN_PASSWORD_LENGTH, int maxLength = DEFAULT_MAX_PASSWORD_LENGTH, bool useSafeChars = true, bool useSpecialSymbols = false)
        {
            if (minLength < MIN_PASSWORD_LENGTH) minLength = MIN_PASSWORD_LENGTH;
            if (minLength > MAX_PASSWORD_LENGTH) minLength = MAX_PASSWORD_LENGTH;

            if (maxLength < MIN_PASSWORD_LENGTH) maxLength = MIN_PASSWORD_LENGTH;
            if (maxLength > MAX_PASSWORD_LENGTH) maxLength = MAX_PASSWORD_LENGTH;

            if (minLength > maxLength) maxLength = minLength;

            char[][] charGroups = useSafeChars ?
                new char[][] { PASSWORD_CHARS_SAFE_LCASE, PASSWORD_CHARS_SAFE_UCASE, PASSWORD_CHARS_SAFE_NUMERIC, useSpecialSymbols ? PASSWORD_CHARS_SAFE_SPECIAL : PASSWORD_CHARS_SAFE_MIX, PASSWORD_CHARS_SAFE_MIX }
                :
                new char[][] { PASSWORD_CHARS_LCASE, PASSWORD_CHARS_UCASE, PASSWORD_CHARS_NUMERIC, useSpecialSymbols ? PASSWORD_CHARS_SPECIAL : PASSWORD_CHARS_MIX, PASSWORD_CHARS_MIX };

            int[] charsLeftInGroup = new int[charGroups.Length];
            int[] leftGroupsOrder = new int[charGroups.Length];

            for (int i = 0; i < charsLeftInGroup.Length; i++)
                charsLeftInGroup[i] = charGroups[i].Length;

            for (int i = 0; i < leftGroupsOrder.Length; i++)
                leftGroupsOrder[i] = i;

            byte[] seedBytes = new byte[4];

            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            RandomKeyHelpers.Rng.GetBytes(seedBytes);

            int seed = (seedBytes[0] & 0x7f) << 24 | seedBytes[1] << 16 | seedBytes[2] << 8 | seedBytes[3];

            Random random = new Random(seed);
            char[] password = new char[minLength < maxLength ? random.Next(minLength, maxLength + 1) : minLength];

            int nextCharIdx;
            int nextGroupIdx;
            int nextLeftGroupsOrderIdx;
            int lastCharIdx;

            int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

            for (int i = 0; i < password.Length; i++)
            {
                nextLeftGroupsOrderIdx = lastLeftGroupsOrderIdx == 0 ? 0 : random.Next(0, lastLeftGroupsOrderIdx);

                nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

                nextCharIdx = lastCharIdx == 0 ? 0 : random.Next(0, lastCharIdx + 1);

                password[i] = charGroups[nextGroupIdx][nextCharIdx];

                if (lastCharIdx == 0)
                    charsLeftInGroup[nextGroupIdx] = charGroups[nextGroupIdx].Length;
                else
                {
                    if (lastCharIdx != nextCharIdx)
                    {
                        char temp = charGroups[nextGroupIdx][lastCharIdx];
                        charGroups[nextGroupIdx][lastCharIdx] = charGroups[nextGroupIdx][nextCharIdx];
                        charGroups[nextGroupIdx][nextCharIdx] = temp;
                    }

                    charsLeftInGroup[nextGroupIdx]--;
                }

                if (lastLeftGroupsOrderIdx == 0)
                    lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                else
                {
                    if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                    {
                        int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                        leftGroupsOrder[lastLeftGroupsOrderIdx] = leftGroupsOrder[nextLeftGroupsOrderIdx];
                        leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                    }

                    lastLeftGroupsOrderIdx--;
                }
            }

            return new string(password);
        }

    }
}
