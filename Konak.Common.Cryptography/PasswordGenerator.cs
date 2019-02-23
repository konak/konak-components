using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Konak.Common.Cryptography
{
    public static class PasswordGenerator
    {
        private static int DEFAULT_MIN_PASSWORD_LENGTH = 8;
        private static int DEFAULT_MAX_PASSWORD_LENGTH = 10;

        private static int MIN_PASSWORD_LENGTH = 6;
        private static int MAX_PASSWORD_LENGTH = 20;

        private static char[] PASSWORD_CHARS_LCASE = "abcdefgjkmnpqrstwxy".ToCharArray();
        private static char[] PASSWORD_CHARS_UCASE = "ACDEFGHJKLMNPQRSTWXYZ".ToCharArray();
        private static char[] PASSWORD_CHARS_NUMERIC = "3456789".ToCharArray();
        private static char[] PASSWORD_CHARS_SPECIAL = "6LMjkm789H3RS48abG6C73efg6AZ34pqrP57cd89JKnstwDE45QF56xy345N897TWXY9".ToCharArray();

        public static string Generate()
        {
            return Generate(DEFAULT_MIN_PASSWORD_LENGTH, DEFAULT_MAX_PASSWORD_LENGTH);
        }

        public static string Generate(int length)
        {
            return Generate(length, length);
        }

        public static string Generate(int minLength, int maxLength)
        {
            if (minLength < MIN_PASSWORD_LENGTH) minLength = MIN_PASSWORD_LENGTH;
            if (minLength > MAX_PASSWORD_LENGTH) minLength = MAX_PASSWORD_LENGTH;

            if (maxLength < MIN_PASSWORD_LENGTH) maxLength = MIN_PASSWORD_LENGTH;
            if (maxLength > MAX_PASSWORD_LENGTH) maxLength = MAX_PASSWORD_LENGTH;

            if (minLength > maxLength) maxLength = minLength;

            char[][] charGroups = new char[][] { PASSWORD_CHARS_LCASE, PASSWORD_CHARS_UCASE, PASSWORD_CHARS_NUMERIC, PASSWORD_CHARS_SPECIAL };

            int[] charsLeftInGroup = new int[charGroups.Length];
            int[] leftGroupsOrder = new int[charGroups.Length];

            for (int i = 0; i < charsLeftInGroup.Length; i++)
                charsLeftInGroup[i] = charGroups[i].Length;

            for (int i = 0; i < leftGroupsOrder.Length; i++)
                leftGroupsOrder[i] = i;

            byte[] seedBytes = new byte[4];

            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(seedBytes);

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
