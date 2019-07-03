﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.MockHttp.Http;
using System.Net.Http.MockHttp.Matchers;
using System.Text;

namespace System.Net.Http.MockHttp
{
	/// <summary>
	/// Extensions for <see cref="RequestMatching"/>.
	/// </summary>
	public static class RequestMatchingExtensions
	{
		/// <summary>
		/// Matches a request by specified <paramref name="requestUri"/>.
		/// </summary>
		/// <param name="builder">The request matching builder instance.</param>
		/// <param name="requestUri">The request URI or a URI wildcard (based on glob).</param>
		/// <returns>The request matching builder instance.</returns>
		public static RequestMatching Url(this RequestMatching builder, string requestUri)
		{
			return builder.With(new UrlMatcher(requestUri));
		}

		/// <summary>
		/// Matches a request by query string.
		/// </summary>
		/// <param name="builder">The request matching builder instance.</param>
		/// <param name="key">The query string parameter key.</param>
		/// <param name="value">The query string value.</param>
		/// <returns>The request matching builder instance.</returns>
		public static RequestMatching QueryString(this RequestMatching builder, string key, string value)
		{
			return builder.QueryString(key, new[] { value });
		}

		/// <summary>
		/// Matches a request by query string.
		/// </summary>
		/// <param name="builder">The request matching builder instance.</param>
		/// <param name="key">The query string parameter key.</param>
		/// <param name="values">The query string values.</param>
		/// <returns>The request matching builder instance.</returns>
		public static RequestMatching QueryString(this RequestMatching builder, string key, IEnumerable<string> values)
		{
			return builder.QueryString(new Dictionary<string, IEnumerable<string>>
			{
				{ key, values }
			});
		}

		/// <summary>
		/// Matches a request by query string.
		/// </summary>
		/// <param name="builder">The request matching builder instance.</param>
		/// <param name="key">The query string parameter key.</param>
		/// <param name="values">The query string value.</param>
		/// <returns>The request matching builder instance.</returns>
		public static RequestMatching QueryString(this RequestMatching builder, string key, params string[] values)
		{
			return builder.QueryString(key, values?.AsEnumerable());
		}

		/// <summary>
		/// Matches a request by query string.
		/// </summary>
		/// <param name="builder">The request matching builder instance.</param>
		/// <param name="parameters">The query string parameters.</param>
		/// <returns>The request matching builder instance.</returns>
		public static RequestMatching QueryString(this RequestMatching builder, IEnumerable<KeyValuePair<string, IEnumerable<string>>> parameters)
		{
			return builder.With(new QueryStringMatcher(parameters));
		}

		/// <summary>
		/// Matches a request by query string.
		/// </summary>
		/// <param name="builder">The request matching builder instance.</param>
		/// <param name="queryString">The query string.</param>
		/// <returns>The request matching builder instance.</returns>
		public static RequestMatching QueryString(this RequestMatching builder, string queryString)
		{
			return builder.With(new QueryStringMatcher(queryString));
		}

		/// <summary>
		/// Matches a request by HTTP method.
		/// </summary>
		/// <param name="builder">The request matching builder instance.</param>
		/// <param name="httpMethod">The HTTP method.</param>
		/// <returns>The request matching builder instance.</returns>
		public static RequestMatching Method(this RequestMatching builder, string httpMethod)
		{
			return builder.With(new HttpMethodMatcher(httpMethod));
		}

		/// <summary>
		/// Matches a request by HTTP method.
		/// </summary>
		/// <param name="builder">The request matching builder instance.</param>
		/// <param name="httpMethod">The HTTP method.</param>
		/// <returns>The request matching builder instance.</returns>
		public static RequestMatching Method(this RequestMatching builder, HttpMethod httpMethod)
		{
			return builder.With(new HttpMethodMatcher(httpMethod));
		}

		/// <summary>
		/// Matches a request by HTTP header.
		/// </summary>
		/// <param name="builder">The request matching builder instance.</param>
		/// <param name="name">The header name.</param>
		/// <param name="value">The header value.</param>
		/// <returns>The request matching builder instance.</returns>
		public static RequestMatching Headers(this RequestMatching builder, string name, string value)
		{
			return builder.With(new HttpHeadersMatcher(name, value));
		}

		/// <summary>
		/// Matches a request by HTTP header.
		/// </summary>
		/// <param name="builder">The request matching builder instance.</param>
		/// <param name="name">The header name.</param>
		/// <param name="value">The header value.</param>
		/// <returns>The request matching builder instance.</returns>
		public static RequestMatching Headers<T>(this RequestMatching builder, string name, T value)
			where T : struct
		{
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
			return builder.Headers(name, converter.ConvertToString(value));
		}
		
		/// <summary>
		/// Matches a request by HTTP header.
		/// </summary>
		/// <param name="builder">The request matching builder instance.</param>
		/// <param name="name">The header name.</param>
		/// <param name="date">The header value.</param>
		/// <returns>The request matching builder instance.</returns>
		public static RequestMatching Headers(this RequestMatching builder, string name, DateTime date)
		{
			return builder.Headers(name, (DateTimeOffset)date.ToUniversalTime());
		}

		/// <summary>
		/// Matches a request by HTTP header.
		/// </summary>
		/// <param name="builder">The request matching builder instance.</param>
		/// <param name="name">The header name.</param>
		/// <param name="date">The header value.</param>
		/// <returns>The request matching builder instance.</returns>
		public static RequestMatching Headers(this RequestMatching builder, string name, DateTimeOffset date)
		{
			// https://tools.ietf.org/html/rfc2616#section-3.3.1
			CultureInfo ci = CultureInfo.InvariantCulture;
			return builder.Any(any => any
				.Headers(name, date.ToString("R", ci))
				.Headers(name, date.ToString("dddd, dd-MMM-yy HH:mm:ss 'GMT'", ci))
				.Headers(name, date.ToString("ddd MMM  d  H:mm:ss yyyy", ci))
			);
		}

		/// <summary>
		/// Matches a request by HTTP headers.
		/// </summary>
		/// <param name="builder">The request matching builder instance.</param>
		/// <param name="headers">The headers.</param>
		/// <returns>The request matching builder instance.</returns>
		public static RequestMatching Headers(this RequestMatching builder, string headers)
		{
			return builder.Headers(HttpHeadersCollection.Parse(headers));
		}

		/// <summary>
		/// Matches a request by HTTP header.
		/// </summary>
		/// <param name="builder">The request matching builder instance.</param>
		/// <param name="name">The header name.</param>
		/// <param name="values">The header values.</param>
		/// <returns>The request matching builder instance.</returns>
		public static RequestMatching Headers(this RequestMatching builder, string name, IEnumerable<string> values)
		{
			return builder.With(new HttpHeadersMatcher(name, values));
		}

		/// <summary>
		/// Matches a request by HTTP header.
		/// </summary>
		/// <param name="builder">The request matching builder instance.</param>
		/// <param name="name">The header name.</param>
		/// <param name="values">The header values.</param>
		/// <returns>The request matching builder instance.</returns>
		public static RequestMatching Headers(this RequestMatching builder, string name, params string[] values)
		{
			return builder.Headers(name, values.AsEnumerable());
		}

		/// <summary>
		/// Matches a request by HTTP header.
		/// </summary>
		/// <param name="builder">The request matching builder instance.</param>
		/// <param name="headers">The headers.</param>
		/// <returns>The request matching builder instance.</returns>
		public static RequestMatching Headers(this RequestMatching builder, IEnumerable<KeyValuePair<string, string>> headers)
		{
			return builder.With(new HttpHeadersMatcher(headers));
		}

		/// <summary>
		/// Matches a request by HTTP header.
		/// </summary>
		/// <param name="builder">The request matching builder instance.</param>
		/// <param name="headers">The headers.</param>
		/// <returns>The request matching builder instance.</returns>
		public static RequestMatching Headers(this RequestMatching builder, IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers)
		{
			return builder.With(new HttpHeadersMatcher(headers));
		}

		/// <summary>
		/// Matches a request by content type.
		/// </summary>
		/// <param name="builder">The request matching builder instance.</param>
		/// <param name="contentType">The content type.</param>
		/// <returns>The request matching builder instance.</returns>
		public static RequestMatching ContentType(this RequestMatching builder, string contentType)
		{
			return builder.ContentType(MediaTypeHeaderValue.Parse(contentType));
		}

		/// <summary>
		/// Matches a request by media type.
		/// </summary>
		/// <param name="builder">The request matching builder instance.</param>
		/// <param name="mediaType">The media type.</param>
		/// <returns>The request matching builder instance.</returns>
		public static RequestMatching ContentType(this RequestMatching builder, MediaTypeHeaderValue mediaType)
		{
			if (mediaType == null)
			{
				throw new ArgumentNullException(nameof(mediaType));
			}

			return builder.Headers("Content-Type", mediaType.ToString());
		}

		/// <summary>
		/// Matches a request by request content.
		/// </summary>
		/// <param name="builder">The request matching builder instance.</param>
		/// <param name="content">The request content.</param>
		/// <returns>The request matching builder instance.</returns>
		public static RequestMatching Content(this RequestMatching builder, string content)
		{
			return builder.Replace(new ContentMatcher(content, Encoding.UTF8));
		}

		/// <summary>
		/// Matches a request by request content.
		/// </summary>
		/// <param name="builder">The request matching builder instance.</param>
		/// <param name="content">The request content.</param>
		/// <param name="encoding">The request content encoding.</param>
		/// <returns>The request matching builder instance.</returns>
		public static RequestMatching Content(this RequestMatching builder, string content, Encoding encoding)
		{
			return builder.Replace(new ContentMatcher(content, encoding));
		}

		/// <summary>
		/// Matches a request by request content.
		/// </summary>
		/// <param name="builder">The request matching builder instance.</param>
		/// <param name="content">The request content.</param>
		/// <returns>The request matching builder instance.</returns>
		public static RequestMatching Content(this RequestMatching builder, byte[] content)
		{
			return builder.Replace(new ContentMatcher(content));
		}

		/// <summary>
		/// Matches a request by request content.
		/// </summary>
		/// <param name="builder">The request matching builder instance.</param>
		/// <param name="content">The request content.</param>
		/// <returns>The request matching builder instance.</returns>
		public static RequestMatching Content(this RequestMatching builder, Stream content)
		{
			using (var ms = new MemoryStream())
			{
				content.CopyTo(ms);
				return builder.Replace(new ContentMatcher(ms.ToArray()));
			}
		}

		/// <summary>
		/// Matches a request explicitly that has no request content.
		/// </summary>
		/// <param name="builder">The request matching builder instance.</param>
		/// <returns>The request matching builder instance.</returns>
		public static RequestMatching WithoutContent(this RequestMatching builder)
		{
			return builder.Replace(new ContentMatcher());
		}

		/// <summary>
		/// Matches a request by partially matching the request content.
		/// </summary>
		/// <param name="builder">The request matching builder instance.</param>
		/// <param name="partialContent">The request content.</param>
		/// <returns>The request matching builder instance.</returns>
		public static RequestMatching PartialContent(this RequestMatching builder, string partialContent)
		{
			return builder.Replace(new PartialContentMatcher(partialContent));
		}

		/// <summary>
		/// Matches a request by partially matching the request content.
		/// </summary>
		/// <param name="builder">The request matching builder instance.</param>
		/// <param name="partialContent">The request content.</param>
		/// <param name="encoding">The request content encoding.</param>
		/// <returns>The request matching builder instance.</returns>
		public static RequestMatching PartialContent(this RequestMatching builder, string partialContent, Encoding encoding)
		{
			return builder.Replace(new PartialContentMatcher(partialContent, encoding));
		}

		/// <summary>
		/// Matches a request by partially matching the request content.
		/// </summary>
		/// <param name="builder">The request matching builder instance.</param>
		/// <param name="partialContent">The request content.</param>
		/// <returns>The request matching builder instance.</returns>
		public static RequestMatching PartialContent(this RequestMatching builder, byte[] partialContent)
		{
			return builder.Replace(new PartialContentMatcher(partialContent));
		}

		internal static RequestMatching Any(this RequestMatching builder)
		{
			return builder;
		}

		/// <summary>
		/// Matches a request by verifying it against a list of constraints, for which at least one has to match the request.
		/// </summary>
		/// <param name="builder">The request matching builder instance.</param>
		/// <param name="anyBuilder">An action to configure an inner request matching builder.</param>
		/// <returns>The request matching builder instance.</returns>
		public static RequestMatching Any(this RequestMatching builder, Action<RequestMatching> anyBuilder)
		{
			var anyRequestMatching = new RequestMatching();
			anyBuilder(anyRequestMatching);
			return builder.With(new AnyMatcher(anyRequestMatching.Build()));
		}
	}
}