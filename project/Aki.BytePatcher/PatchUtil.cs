/* BB.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Basuro
 */


using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace Aki.ByteBanger
{
    public static class PatchUtil
    {
        public static PatchInfo Diff(byte[] original, byte[] patched)
        {
            PatchInfo pi = new PatchInfo
            {
                OriginalLength = original.Length,
                PatchedLength = patched.Length
            };

            using (SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider())
            {
                pi.OriginalChecksum = sha256.ComputeHash(original);
                pi.PatchedChecksum = sha256.ComputeHash(patched);
            }

            long minLength = Math.Min(pi.OriginalLength, pi.PatchedLength);

            List<PatchItem> items = new List<PatchItem>();
            List<byte> currentData = null;
            int diffOffsetStart = 0;

            for (int i = 0; i < minLength; i++)
            {
                if (original[i] != patched[i])
                {
                    if (currentData == null)
                    {
                        diffOffsetStart = i;
                        currentData = new List<byte>();
                    }

                    currentData.Add(patched[i]);
                }
                else
                {
                    if (currentData != null)
                        items.Add(new PatchItem { Offset = diffOffsetStart, Data = currentData.ToArray() });

                    currentData = null;
                    diffOffsetStart = 0;
                }
            }

            if (currentData != null)
                items.Add(new PatchItem { Offset = diffOffsetStart, Data = currentData.ToArray() });

            if (pi.PatchedLength > pi.OriginalLength)
            {
                byte[] buf = new byte[pi.PatchedLength - pi.OriginalLength];
                Array.Copy(patched, pi.OriginalLength, buf, 0, buf.Length);
                items.Add(new PatchItem { Offset = pi.OriginalLength, Data = buf });
            }

            pi.Items = items.ToArray();

            return pi;
        }

        public static PatchInfo Diff(string originalFile, string patchedFile)
        {
            if (string.IsNullOrWhiteSpace(originalFile)) throw new ArgumentException("originalFile must not be null or empty");
            if (string.IsNullOrWhiteSpace(patchedFile)) throw new ArgumentException("patchedFile must not be null or empty");
            if (!File.Exists(originalFile)) throw new FileNotFoundException($"File '{originalFile}' not found");
            if (!File.Exists(patchedFile)) throw new FileNotFoundException($"File '{patchedFile}' not found");

            return Diff(File.ReadAllBytes(originalFile), File.ReadAllBytes(patchedFile));
        }

        public static byte[] Patch(byte[] input, PatchInfo pi)
        {
            byte[] inputHash;
            using (SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider())
            {
                inputHash = sha256.ComputeHash(input);
            }

            if (!ArraysMatch(inputHash, pi.OriginalChecksum)) throw new Exception("Invalid input file");

            byte[] patchedData = new byte[pi.PatchedLength];
            long minLen = Math.Min(pi.OriginalLength, pi.PatchedLength);
            Array.Copy(input, patchedData, minLen);

            foreach (PatchItem itm in pi.Items)
                Array.Copy(itm.Data, 0, patchedData, itm.Offset, itm.Data.Length);

            byte[] patchedHash;
            using (SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider())
            {
                patchedHash = sha256.ComputeHash(patchedData);
            }

            if (!ArraysMatch(patchedHash, pi.PatchedChecksum)) throw new Exception("Output hash mismatch");

            return patchedData;
        }

        private static bool ArraysMatch(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;

            for (int i = 0; i < a.Length; i++)
                if (a[i] != b[i]) return false;

            return true;
        }
    }
}
