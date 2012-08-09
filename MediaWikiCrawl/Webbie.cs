//
// Webbie.cs
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
using System.Diagnostics;
using System.IO;
using System.Net;

namespace MediaWikiCrawl
{
	/// <summary>
	/// Webbie.
	/// </summary>
	public class Webbie
	{
		/// <summary>
		/// Download the specified uri.
		/// </summary>
		/// <param name='uri'>
		/// URI.
		/// </param>
		public static byte[] Download(Uri uri)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}

			try
			{
				// Configure the request.
				var request = WebRequest.Create(uri) as HttpWebRequest;
				request.UserAgent = @"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/22.0.1207.1 Safari/537.1";
				request.KeepAlive = false;
				request.Timeout = 15 * 1000;

				using (var response = request.GetResponse().GetResponseStream())
				{
					if (request.HaveResponse && response != null)
					{
						var memoryStream = new MemoryStream();
						byte[] buffer = new byte[0x1000];
						int bytes;

						while ((bytes = response.Read(buffer, 0, buffer.Length)) > 0)
						{
							memoryStream.Write(buffer, 0, bytes);
						}

						return memoryStream.ToArray();
					}
				}
			}
			catch (WebException wex)
			{
				if (wex.Response != null)
				{
					using (var errorResponse = wex.Response as HttpWebResponse)
					{
						Debug.WriteLine(
							"[{1}] {0}",
							errorResponse.StatusDescription,
							errorResponse.StatusCode);
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Get the specified uri.
		/// </summary>
		/// <param name='uri'>
		/// URI.
		/// </param>
		public static string Get(Uri uri)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}

			var html = string.Empty;

			try
			{
				// Configure the request.
				var request = WebRequest.Create(uri) as HttpWebRequest;
				request.UserAgent = @"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/22.0.1207.1 Safari/537.1";
				request.KeepAlive = false;
				request.Timeout = 15 * 1000;

				using (var response = request.GetResponse() as HttpWebResponse)
				{
					if (request.HaveResponse && response != null)
					{
						using (var stream = new StreamReader(response.GetResponseStream()))
						{
							html = stream.ReadToEnd();
						}
					}
				}
			}
			catch (WebException wex)
			{
				if (wex.Response != null)
				{
					using (var errorResponse = wex.Response as HttpWebResponse)
					{
						Debug.WriteLine(
							"[{1}] {0}",
							errorResponse.StatusDescription,
							errorResponse.StatusCode);
					}
				}
			}

			return html;
		}

		/// <summary>
		/// Sanitize the specified input.
		/// </summary>
		/// <param name='input'>
		/// Input.
		/// </param>
		public static string Sanitize(string input)
		{
			// TODO: Provide proper sanitization!!!!
			return input;
		}
	}
}