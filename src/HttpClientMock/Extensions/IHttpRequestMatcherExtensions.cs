﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace HttpClientMock
{
	internal static class IHttpRequestMatcherExtensions
	{
		/// <summary>
		/// Checks if all <paramref name="matchers"/> match the given <paramref name="request"/>.
		/// </summary>
		/// <param name="matchers">The matchers to check against the <paramref name="request"/>.</param>
		/// <param name="request">The request to check.</param>
		/// <returns><see langword="true" /> if all <paramref name="matchers"/> match the <paramref name="request"/>.</returns>
		public static bool AreAllMatching(this IEnumerable<IHttpRequestMatcher> matchers, HttpRequestMessage request)
		{
			return matchers.All(m => m.IsMatch(request));
		}
	}
}