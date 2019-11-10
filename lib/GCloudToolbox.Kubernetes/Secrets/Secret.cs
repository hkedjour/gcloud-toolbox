#region License
// Copyright (c) 2019 Hichem Kedjour
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GCloudToolbox.Kubernetes.Secrets
{
    /// <summary>
    /// An implementation of <see cref="ISecret"/>
    /// </summary>
    internal class Secret : ISecret
    {
        private readonly JObject _document;

        public Secret(JObject document)
        {
            _document = document;
        }

        /// <inheritdoc />
        public IList<string> Files => _document["data"]?.Children().Select(c => ((JProperty) c).Name).ToList() ??
                                      (IList<string>) Array.Empty<string>();

        /// <inheritdoc />
        public string GetTextFile(string fileName)
        {
            var encodedData = _document["data"]?[fileName]?.Value<string>() ?? string.Empty;

            return DecodeSecretContent(encodedData);
        }

        /// <inheritdoc />
        public void SetTextFile(string fileName, string content)
        {
            var data = _document["data"]?[fileName];
            if (data == null)
            {
                return;
            }

            var encodedContent = EncodeSecretContent(content);

            data.Replace(new JValue(encodedContent));
        }

        /// <inheritdoc />
        public async Task WriteToFileAsync(string filePath, CancellationToken cancellationToken)
        {
            await using var sw = new StreamWriter(filePath);
            await _document.WriteToAsync(new JsonTextWriter(sw), cancellationToken);
        }

        private static string DecodeSecretContent(string encodedData) => 
            Encoding.UTF8.GetString(Convert.FromBase64String(encodedData)).Replace("\n", Environment.NewLine);

        private static string EncodeSecretContent(string content) =>
            Convert.ToBase64String(Encoding.UTF8.GetBytes(content.Replace(Environment.NewLine, "\n")));
    }
}