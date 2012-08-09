//
// MediaWikiApiTests.cs
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
using NUnit.Framework;
using MediaWikiCrawl;

namespace MediaWikiCrawl.Tests
{
	[TestFixture]
	public class MediaWikiApiTests
	{
		private string _baseUri;

		[TestFixtureSetUp]
		public void SetUp()
		{
			this._baseUri = @"http://wiki.guildwars2.com/api.php";
		}

		[Test]
		public void CanGetAllImages()
		{
			var count = 0;
			var wikiApi = new MediaWikiApi(this._baseUri);

			foreach (var img in wikiApi.AllImages("z"))
			{
				count++;
				Debug.WriteLine(
					"{0}{1}{2}  {3}",
					count.ToString().PadRight(20),
					img.Name,
					Environment.NewLine,
					img.Url);
			}

			Assert.That(count, Is.GreaterThan(0));
		}

		[Test]
		public void CanGetWikiImage()
		{
			var wikiApi = new MediaWikiApi(this._baseUri);
			var result = wikiApi.PageImage("File:Hall of Monuments background.jpg");
			Assert.That(result, Is.InstanceOf<WikiImage>());
		}

		[Test]
		public void CanGetWikiPage()
		{
			var wikiApi = new MediaWikiApi(this._baseUri);
			var result = wikiApi.Page("Main Page");
			Assert.That(result, Is.InstanceOf<WikiPage>());
			Assert.That(result.ImageTitles, Is.Not.Null);
			Assert.That(result.ImageTitles, Has.None.Null.Or.Empty);
		}
	}
}