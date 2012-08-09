//
// MediaWikiApi.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace MediaWikiCrawl
{
	/// <summary>
	/// Media wiki API.
	/// </summary>
	public class MediaWikiApi
	{
		/// <summary>
		/// The seed.
		/// </summary>
		private static int seed = 42;

		/// <summary>
		/// The _base URI.
		/// </summary>
		private Uri _baseUri;

		/// <summary>
		/// Initializes a new instance of the <see cref="MediaWikiCrawl.MediaWikiApi"/> class.
		/// </summary>
		/// <param name='baseUri'>
		/// Base URI.
		/// </param>
		public MediaWikiApi(string baseUri = "/api.php")
		{
			if (!baseUri.ToLower().Contains("/api.php"))
			{
				baseUri += @"/api.php";
			}

			this._baseUri = new Uri(baseUri);
		}

		/// <summary>
		/// Alls the images.
		/// </summary>
		/// <returns>
		/// The images.
		/// </returns>
		/// <param name='startsWith'>
		/// Starts with.
		/// </param>
		public IEnumerable<WikiImage> AllImages(string startsWith = "0")
		{
			// Formulate the URI.
			var uri = new Uri(string.Format(
				"{0}?format={1}&action={2}&list={3}&aifrom={4}",
				this._baseUri.ToString(),
				Webbie.Sanitize("xml"),
				Webbie.Sanitize("query"),
				Webbie.Sanitize("allimages"),
				Webbie.Sanitize(startsWith)));

			// Find the root of the API (it is re-used so we cache it).
			var xmlApi = XDocument.Parse(Webbie.Get(uri)).Descendants("api");

			// Navigate the XML to formulate some images!
			var images = xmlApi
					.Descendants("query")
					.Descendants("allimages")
			        .Descendants("img")
					.Select(x => new WikiImage {
						Name = x.Attribute("name").Value,
						Url = new Uri(x.Attribute("url").Value),
						DescriptionUrl = new Uri(x.Attribute("descriptionurl").Value)
					})
					.ToList();

			// Return what we can when we can.
			foreach (var img in images)
			{
				yield return img;
			}

			// Determine what to continue with.
			var continueWith = xmlApi.Descendants("query-continue").Count() == 0
				? string.Empty
				: xmlApi
					.Descendants("query-continue")
					.Descendants("allimages")
					.Select(x => x.Attribute("aifrom").Value)
					.First();

			// If there is something left then recursively grab it.
			if (!string.IsNullOrWhiteSpace(continueWith))
			{
				// Give a moment to reflect.
				seed = new Random(seed).Next() % 100;
				new AutoResetEvent(true).WaitOne(new TimeSpan(0, 0, seed));

				foreach (var img in AllImages(continueWith))
				{
					yield return img;
				}
			}
		}

		/// <summary>
		/// Page the specified title.
		/// </summary>
		/// <param name='title'>
		/// Title.
		/// </param>
		public WikiPage Page(string title)
		{
			if (string.IsNullOrWhiteSpace(title))
			{
				throw new ArgumentNullException("title");
			}

			return new WikiPage {
				Title = title,
				ImageTitles = this.PageImages(title)
			};
		}

		/// <summary>
		/// Pages the image.
		/// </summary>
		/// <returns>
		/// The image.
		/// </returns>
		/// <param name='title'>
		/// Title.
		/// </param>
		public WikiImage PageImage(string title)
		{
			if (string.IsNullOrWhiteSpace(title))
			{
				throw new ArgumentNullException("title");
			}

			var uri = new Uri(string.Format(
				"{0}?format={1}&action={2}&titles={3}&prop={4}&iiprop={5}",
				this._baseUri.ToString(),
				Webbie.Sanitize("xml"),
				Webbie.Sanitize("query"),
				Webbie.Sanitize(title),
				Webbie.Sanitize("imageinfo"),
				Webbie.Sanitize("url")));

			return (XDocument.Parse(Webbie.Get(uri))
			        .Descendants("api")
			        .Descendants("query")
			        .Descendants("pages")
			        .Descendants("page")
			        .Descendants("imageinfo")
		        	.Descendants("ii")
			        .Select(x => new WikiImage { 
						Url = new Uri(x.Attribute("url").Value ?? string.Empty), 
						DescriptionUrl = new Uri(x.Attribute("descriptionurl").Value ?? string.Empty) 
					}))
					.First();
		}

		/// <summary>
		/// Pages the images.
		/// </summary>
		/// <returns>
		/// The images.
		/// </returns>
		/// <param name='title'>
		/// Title.
		/// </param>
		public ICollection<string> PageImages(string title)
		{
			if (string.IsNullOrWhiteSpace(title))
			{
				throw new ArgumentNullException("title");
			}

			var uri = new Uri(string.Format(
				"{0}?format={1}&action={2}&titles={3}&prop={4}",
				this._baseUri.ToString(),
				Webbie.Sanitize("xml"),
				Webbie.Sanitize("query"),
				Webbie.Sanitize(title),
				Webbie.Sanitize("images")));

			return XDocument.Parse(Webbie.Get(uri))
					.Descendants("api")
			        .Descendants("query")
			        .Descendants("pages")
			        .Descendants("page")
			        .Descendants("images")
		        	.Descendants("im")
			        .Select(x => x.Attribute("title").Value)
					.ToList();
		}
	}
}