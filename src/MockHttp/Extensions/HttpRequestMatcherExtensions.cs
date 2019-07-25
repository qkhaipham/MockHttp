﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using MockHttp.Matchers;

namespace MockHttp
{
	internal static class HttpRequestMatcherExtensions
	{
		/// <summary>
		/// Checks if all <paramref name="matchers"/> match the given <paramref name="request"/>.
		/// </summary>
		/// <param name="matchers">The matchers to check against the <paramref name="request"/>.</param>
		/// <param name="request">The request to check.</param>
		/// <param name="notMatchedOn">A list of matches that were not matched by the request.</param>
		/// <returns><see langword="true" /> if all <paramref name="matchers"/> match the <paramref name="request"/>.</returns>
		public static bool All(this IEnumerable<HttpRequestMatcher> matchers, HttpRequestMessage request, out IEnumerable<HttpRequestMatcher> notMatchedOn)
		{
			var noMatchList = new List<HttpRequestMatcher>();
			bool hasMatchedAll = true;
			foreach (HttpRequestMatcher m in matchers)
			{
				if (m.IsMatch(request))
				{
					continue;
				}

				noMatchList.Add(m);
				hasMatchedAll = false;
			}

			notMatchedOn = noMatchList;
			return hasMatchedAll;
		}

		/// <summary>
		/// Checks if any <paramref name="matchers"/> match the given <paramref name="request"/>.
		/// </summary>
		/// <param name="matchers">The matchers to check against the <paramref name="request"/>.</param>
		/// <param name="request">The request to check.</param>
		/// <returns><see langword="true" /> if any <paramref name="matchers"/> match the <paramref name="request"/>.</returns>
		public static bool Any(this IEnumerable<HttpRequestMatcher> matchers, HttpRequestMessage request)
		{
			return matchers.Any(m => m.IsMatch(request));
		}
	}
}