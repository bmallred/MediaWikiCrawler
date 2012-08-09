//
// Main.cs
//
// Author:
//       Bryan Allred <bryan.allred@gmail.com>
//
// Copyright (c) 2012 Bryan Allred
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.IO;

namespace MediaWikiCrawl
{
	/// <summary>
	/// Main class.
	/// </summary>
	public class MainClass
	{
		/// <summary>
		/// The _base URI.
		/// </summary>
		private static string _baseUri = string.Empty;

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main (string[] args)
		{
			// Go through any arguments.
			for (var idx = 0; idx < args.Length; idx++)
			{
				switch (args[idx].ToLower())
				{
					default:
						_baseUri = args[idx];
						break;
				}
			}

			// Validate inputs.
			if (string.IsNullOrWhiteSpace(_baseUri))
			{
				throw new ArgumentNullException("baseUri", "Missing the base URI!");
			}

			// Create a new API handler.
			var api = new MediaWikiApi(_baseUri);

			// Iterate through captured images and commit them to permanent storage.
			foreach (var img in api.AllImages())
			{
				var file = new FileInfo(img.Name);
				var bytes = img.Download();

				if (bytes != null && bytes.Length > 0)
				{
					using (var stream = file.OpenWrite())
					{
						// Attempt to write if we can.
						if (stream.CanWrite)
						{
							stream.Write(bytes, 0, bytes.Length);

							Console.WriteLine(
								"{0}, {1} bytes",
								file.Length,
								file.FullName);
						}

						// Always close the file.
						stream.Close();
					}
             	}
			}
		}
	}
}