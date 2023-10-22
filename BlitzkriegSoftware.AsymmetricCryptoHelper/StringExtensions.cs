using System;
using System.Collections.Generic;
using System.Text;

namespace BlitzkriegSoftware.AsymmetricCryptoHelper
{
    /// <summary>
    /// String Extentions
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Throw if a string is null
        /// </summary>
        /// <param name="text">Text To Check</param>
        /// <param name="fieldName">Name of Field</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void ThrowIfNullOrEmpty(this string text, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName)) fieldName = nameof(text);
            if(string.IsNullOrWhiteSpace(text)) throw new ArgumentNullException(fieldName);
        }
    }
}
