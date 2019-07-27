﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MockHttp.Language;
using MockHttp.Language.Flow;
#if !NETSTANDARD1_1
using System.Net.Http.Formatting;
#endif

namespace MockHttp
{
	/// <summary>
	/// Extensions for <see cref="IResponds"/>.
	/// </summary>
	// ReSharper disable once InconsistentNaming
	public static class IRespondsExtensions
	{
		/// <summary>
		/// Specifies a function that returns the response for a request.
		/// </summary>
		/// <param name="responds"></param>
		/// <param name="response">The function that provides the response message to return for a request.</param>
		public static IResponseResult Respond(this IResponds responds, Func<HttpResponseMessage> response)
		{
			return responds.Respond(() => Task.FromResult(response()));
		}

		/// <summary>
		/// Specifies a function that returns the response for a request.
		/// </summary>
		/// <param name="responds"></param>
		/// <param name="response">The function that provides the response message to return for given request.</param>
		public static IResponseResult Respond(this IResponds responds, Func<HttpRequestMessage, HttpResponseMessage> response)
		{
			return responds.Respond(request => Task.FromResult(response(request)));
		}

		/// <summary>
		/// Specifies the <paramref name="statusCode"/> response for a request.
		/// </summary>
		/// <param name="responds"></param>
		/// <param name="statusCode">The status code response for given request.</param>
		public static IResponseResult Respond(this IResponds responds, HttpStatusCode statusCode)
		{
			return responds.Respond(() => new HttpResponseMessage(statusCode));
		}

		/// <summary>
		/// Specifies the <see cref="HttpStatusCode.OK"/> and <paramref name="content"/> to respond with for a request.
		/// </summary>
		/// <param name="responds"></param>
		/// <param name="content">The response content.</param>
		public static IResponseResult Respond(this IResponds responds, string content)
		{
			return responds.Respond(HttpStatusCode.OK, content);
		}

		/// <summary>
		/// Specifies the <paramref name="statusCode"/> and <paramref name="content"/> to respond with for a request.
		/// </summary>
		/// <param name="responds"></param>
		/// <param name="statusCode">The status code response for given request.</param>
		/// <param name="content">The response content.</param>
		public static IResponseResult Respond(this IResponds responds, HttpStatusCode statusCode, string content)
		{
			return responds.Respond(statusCode, content, (string)null);
		}

		/// <summary>
		/// Specifies the <see cref="HttpStatusCode.OK"/>, <paramref name="content"/> and <paramref name="mediaType"/> to respond with for a request.
		/// </summary>
		/// <param name="responds"></param>
		/// <param name="content">The response content.</param>
		/// <param name="mediaType">The media type.</param>
		public static IResponseResult Respond(this IResponds responds, string content, string mediaType)
		{
			return responds.Respond(HttpStatusCode.OK, content, mediaType);
		}

		/// <summary>
		/// Specifies the <paramref name="statusCode"/>, <paramref name="content"/> and <paramref name="mediaType"/> to respond with for a request.
		/// </summary>
		/// <param name="responds"></param>
		/// <param name="statusCode">The status code response for given request.</param>
		/// <param name="content">The response content.</param>
		/// <param name="mediaType">The media type.</param>
		public static IResponseResult Respond(this IResponds responds, HttpStatusCode statusCode, string content, string mediaType)
		{
			return responds.Respond(statusCode, content, null, mediaType);
		}

		/// <summary>
		/// Specifies the <see cref="HttpStatusCode.OK"/>, <paramref name="content"/> and <paramref name="mediaType"/> to respond with for a request.
		/// </summary>
		/// <param name="responds"></param>
		/// <param name="content">The response content.</param>
		/// <param name="mediaType">The media type.</param>
		public static IResponseResult Respond(this IResponds responds, string content, MediaTypeHeaderValue mediaType)
		{
			return responds.Respond(HttpStatusCode.OK, content, mediaType);
		}

		/// <summary>
		/// Specifies the <paramref name="statusCode"/>, <paramref name="content"/> and <paramref name="mediaType"/> to respond with for a request.
		/// </summary>
		/// <param name="responds"></param>
		/// <param name="statusCode">The status code response for given request.</param>
		/// <param name="content">The response content.</param>
		/// <param name="mediaType">The media type.</param>
		public static IResponseResult Respond(this IResponds responds, HttpStatusCode statusCode, string content, MediaTypeHeaderValue mediaType)
		{
			if (content == null)
			{
				throw new ArgumentNullException(nameof(content));
			}

			return responds.Respond(() => new HttpResponseMessage(statusCode)
			{
				Content = new StringContent(content)
				{
					Headers =
					{
						ContentType = mediaType
					}
				}
			});
		}

		/// <summary>
		/// Specifies the <see cref="HttpStatusCode.OK"/>, <paramref name="content"/>, <paramref name="encoding"/> and <paramref name="mediaType"/> to respond with for a request.
		/// </summary>
		/// <param name="responds"></param>
		/// <param name="content">The response content.</param>
		/// <param name="encoding">The encoding.</param>
		/// <param name="mediaType">The media type.</param>
		public static IResponseResult Respond(this IResponds responds, string content, Encoding encoding, string mediaType)
		{
			return responds.Respond(HttpStatusCode.OK, content, encoding, mediaType);
		}

		/// <summary>
		/// Specifies the <paramref name="statusCode"/>, <paramref name="content"/>, <paramref name="encoding"/> and <paramref name="mediaType"/> to respond with for a request.
		/// </summary>
		/// <param name="responds"></param>
		/// <param name="statusCode">The status code response for given request.</param>
		/// <param name="content">The response content.</param>
		/// <param name="encoding">The encoding.</param>
		/// <param name="mediaType">The media type.</param>
		public static IResponseResult Respond(this IResponds responds, HttpStatusCode statusCode, string content, Encoding encoding, string mediaType)
		{
			return responds.Respond(statusCode, content, new MediaTypeHeaderValue(mediaType ?? "text/plain")
			{
				CharSet = (encoding ?? Encoding.UTF8).WebName
			});
		}

		/// <summary>
		/// Specifies the <see cref="HttpStatusCode.OK"/> and <paramref name="content"/> to respond with for a request.
		/// </summary>
		/// <param name="responds"></param>
		/// <param name="content">The response content.</param>
		public static IResponseResult Respond(this IResponds responds, Stream content)
		{
			return responds.Respond(HttpStatusCode.OK, content);
		}

		/// <summary>
		/// Specifies the <see cref="HttpStatusCode.OK"/> and <paramref name="content"/> to respond with for a request.
		/// </summary>
		/// <param name="responds"></param>
		/// <param name="content">The response content.</param>
		/// <param name="mediaType">The media type.</param>
		public static IResponseResult Respond(this IResponds responds, Stream content, string mediaType)
		{
			return responds.Respond(HttpStatusCode.OK, content, mediaType);
		}

		/// <summary>
		/// Specifies the <paramref name="statusCode"/> and <paramref name="content"/> to respond with for a request.
		/// </summary>
		/// <param name="responds"></param>
		/// <param name="statusCode">The status code response for given request.</param>
		/// <param name="content">The response content.</param>
		public static IResponseResult Respond(this IResponds responds, HttpStatusCode statusCode, Stream content)
		{
			return responds.Respond(statusCode, content, "application/octet-stream");
		}

		/// <summary>
		/// Specifies the <paramref name="statusCode"/> and <paramref name="content"/> to respond with for a request.
		/// </summary>
		/// <param name="responds"></param>
		/// <param name="statusCode">The status code response for given request.</param>
		/// <param name="content">The response content.</param>
		/// <param name="mediaType">The media type.</param>
		public static IResponseResult Respond(this IResponds responds, HttpStatusCode statusCode, Stream content, string mediaType)
		{
			return responds.Respond(statusCode, content, mediaType == null ? null : MediaTypeHeaderValue.Parse(mediaType));
		}

		/// <summary>
		/// Specifies the <see cref="HttpStatusCode.OK"/> and <paramref name="content"/> to respond with for a request.
		/// </summary>
		/// <param name="responds"></param>
		/// <param name="content">The response content.</param>
		/// <param name="mediaType">The media type.</param>
		public static IResponseResult Respond(this IResponds responds, Stream content, MediaTypeHeaderValue mediaType)
		{
			return responds.Respond(HttpStatusCode.OK, content, mediaType);
		}

		/// <summary>
		/// Specifies the <paramref name="statusCode"/> and <paramref name="content"/> to respond with for a request.
		/// </summary>
		/// <param name="responds"></param>
		/// <param name="statusCode">The status code response for given request.</param>
		/// <param name="content">The response content.</param>
		/// <param name="mediaType">The media type.</param>
		public static IResponseResult Respond(this IResponds responds, HttpStatusCode statusCode, Stream content, MediaTypeHeaderValue mediaType)
		{
			if (content == null)
			{
				throw new ArgumentNullException(nameof(content));
			}

			if (!content.CanRead)
			{
				throw new ArgumentException("Cannot read from stream.", nameof(content));
			}

			long originalStreamPos = content.Position;
			var lockObject = new object();

			byte[] buffer = null;
			return responds.Respond(() =>
			{
				if (content.CanSeek)
				{
					content.Position = originalStreamPos;
					return new HttpResponseMessage(statusCode)
					{
						Content = new StreamContent(content)
						{
							Headers =
							{
								ContentType = mediaType
							}
						}
					};
				}

				// Stream not seekable, so we have to use internal buffer for repeated responses.
				if (buffer == null)
				{
					lock (lockObject)
					{
						// Since acquired lock, check if buffer is not set by other thread.
						if (buffer == null)
						{
							using (var ms = new MemoryStream())
							{
								content.CopyTo(ms);
								buffer = ms.ToArray();
							}
						}
					}
				}

				return new HttpResponseMessage(statusCode)
				{
					Content = new ByteArrayContent(buffer)
					{
						Headers =
						{
							ContentType = mediaType
						}
					}
				};
			});
		}

		/// <summary>
		/// Specifies the <see cref="HttpStatusCode.OK"/> and <paramref name="content"/> to respond with for a request.
		/// </summary>
		/// <param name="responds"></param>
		/// <param name="content">The response content.</param>
		public static IResponseResult Respond(this IResponds responds, HttpContent content)
		{
			return responds.Respond(HttpStatusCode.OK, content);
		}

		/// <summary>
		/// Specifies the <paramref name="statusCode"/> and <paramref name="content"/> to respond with for a request.
		/// </summary>
		/// <param name="responds"></param>
		/// <param name="statusCode">The status code response for given request.</param>
		/// <param name="content">The response content.</param>
		public static IResponseResult Respond(this IResponds responds, HttpStatusCode statusCode, HttpContent content)
		{
			if (content == null)
			{
				throw new ArgumentNullException(nameof(content));
			}

			return responds.Respond(async () => new HttpResponseMessage(statusCode)
			{
				Content = await content.CloneAsByteArrayContentAsync().ConfigureAwait(false)
			});
		}

#if !NETSTANDARD1_1
		private static readonly JsonMediaTypeFormatter JsonFormatter = new JsonMediaTypeFormatter();
#endif
		/// <summary>
		/// Specifies the <see cref="HttpStatusCode.OK"/> and <paramref name="content"/> to respond with for a request.
		/// </summary>
		/// <param name="responds"></param>
		/// <param name="content">The response content.</param>
		public static IResponseResult RespondJson<T>(this IResponds responds, T content)
		{
			return responds.RespondJson(content, (MediaTypeHeaderValue)null);
		}

		/// <summary>
		/// Specifies the <paramref name="statusCode"/> and <paramref name="content"/> to respond with for a request.
		/// </summary>
		/// <param name="responds"></param>
		/// <param name="statusCode">The status code response for given request.</param>
		/// <param name="content">The response content.</param>
		public static IResponseResult RespondJson<T>(this IResponds responds, HttpStatusCode statusCode, T content)
		{
			return responds.RespondJson(statusCode, content, (MediaTypeHeaderValue)null);
		}

		/// <summary>
		/// Specifies the <see cref="HttpStatusCode.OK"/> and <paramref name="content"/> to respond with for a request.
		/// </summary>
		/// <param name="responds"></param>
		/// <param name="content">The response content.</param>
		/// <param name="mediaType">The media type. Can be null, in which case the default JSON content type will be used.</param>
		public static IResponseResult RespondJson<T>(this IResponds responds, T content, MediaTypeHeaderValue mediaType)
		{
			return responds.RespondJson(HttpStatusCode.OK, content, mediaType);
		}

		/// <summary>
		/// Specifies the <paramref name="statusCode"/> and <paramref name="content"/> to respond with for a request.
		/// </summary>
		/// <param name="responds"></param>
		/// <param name="statusCode">The status code response for given request.</param>
		/// <param name="content">The response content.</param>
		/// <param name="mediaType">The media type. Can be null, in which case the default JSON content type will be used.</param>
		public static IResponseResult RespondJson<T>(this IResponds responds, HttpStatusCode statusCode, T content, MediaTypeHeaderValue mediaType)
		{
			MediaTypeHeaderValue mt = mediaType ?? MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

#if !NETSTANDARD1_1
			return responds.RespondObject(statusCode, content, JsonFormatter, mt);
#else
			var jsonContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(content))
			{
				Headers =
				{
					ContentType = mt
				}
			};
			return responds.Respond(() => new HttpResponseMessage(statusCode)
			{
				Content = jsonContent
			});
#endif
		}

		/// <summary>
		/// Specifies the <see cref="HttpStatusCode.OK"/> and <paramref name="content"/> to respond with for a request.
		/// </summary>
		/// <param name="responds"></param>
		/// <param name="content">The response content.</param>
		/// <param name="mediaType">The media type. Can be null, in which case the default JSON content type will be used.</param>
		public static IResponseResult RespondJson<T>(this IResponds responds, T content, string mediaType)
		{
			return responds.RespondJson(HttpStatusCode.OK, content, mediaType);
		}

		/// <summary>
		/// Specifies the <paramref name="statusCode"/> and <paramref name="content"/> to respond with for a request.
		/// </summary>
		/// <param name="responds"></param>
		/// <param name="statusCode">The status code response for given request.</param>
		/// <param name="content">The response content.</param>
		/// <param name="mediaType">The media type. Can be null, in which case the default JSON content type will be used.</param>
		public static IResponseResult RespondJson<T>(this IResponds responds, HttpStatusCode statusCode, T content, string mediaType)
		{
#if !NETSTANDARD1_1
			return responds.RespondObject(statusCode, content, JsonFormatter, mediaType);
#else
			return responds.RespondJson(statusCode, content, mediaType == null ? null : MediaTypeHeaderValue.Parse(mediaType));
#endif
		}

#if !NETSTANDARD1_1
		/// <summary>
		/// Specifies the <see cref="HttpStatusCode.OK"/> and <paramref name="value"/> to respond with for a request.
		/// </summary>
		/// <param name="responds"></param>
		/// <param name="value">The response value.</param>
		/// <param name="formatter">The media type formatter</param>
		public static IResponseResult RespondObject<T>(this IResponds responds, T value, MediaTypeFormatter formatter)
		{
			return responds.RespondObject(HttpStatusCode.OK, value, formatter);
		}

		/// <summary>
		/// Specifies the <paramref name="statusCode"/> and <paramref name="value"/> to respond with for a request.
		/// </summary>
		/// <param name="responds"></param>
		/// <param name="statusCode">The status code response for given request.</param>
		/// <param name="value">The response value.</param>
		/// <param name="formatter">The media type formatter</param>
		public static IResponseResult RespondObject<T>(this IResponds responds, HttpStatusCode statusCode, T value, MediaTypeFormatter formatter)
		{
			return responds.RespondObject(statusCode, value, formatter, (MediaTypeHeaderValue)null);
		}

		/// <summary>
		/// Specifies the <see cref="HttpStatusCode.OK"/> and <paramref name="value"/> to respond with for a request.
		/// </summary>
		/// <param name="responds"></param>
		/// <param name="value">The response value.</param>
		/// <param name="formatter">The media type formatter</param>
		/// <param name="mediaType">The media type. Can be null, in which case the <paramref name="formatter"/> default content type will be used.</param>
		public static IResponseResult RespondObject<T>(this IResponds responds, T value, MediaTypeFormatter formatter, string mediaType)
		{
			return responds.RespondObject(HttpStatusCode.OK, value, formatter, mediaType);
		}

		/// <summary>
		/// Specifies the <paramref name="statusCode"/> and <paramref name="value"/> to respond with for a request.
		/// </summary>
		/// <param name="responds"></param>
		/// <param name="statusCode">The status code response for given request.</param>
		/// <param name="value">The response value.</param>
		/// <param name="formatter">The media type formatter</param>
		/// <param name="mediaType">The media type. Can be null, in which case the <paramref name="formatter"/> default content type will be used.</param>
		public static IResponseResult RespondObject<T>(this IResponds responds, HttpStatusCode statusCode, T value, MediaTypeFormatter formatter, string mediaType)
		{
			return responds.RespondObject(statusCode, value, formatter, mediaType == null ? null : MediaTypeHeaderValue.Parse(mediaType));
		}

		/// <summary>
		/// Specifies the <see cref="HttpStatusCode.OK"/> and <paramref name="value"/> to respond with for a request.
		/// </summary>
		/// <param name="responds"></param>
		/// <param name="value">The response value.</param>
		/// <param name="formatter">The media type formatter</param>
		/// <param name="mediaType">The media type. Can be null, in which case the <paramref name="formatter"/> default content type will be used.</param>
		public static IResponseResult RespondObject<T>(this IResponds responds, T value, MediaTypeFormatter formatter, MediaTypeHeaderValue mediaType)
		{
			return responds.RespondObject(HttpStatusCode.OK, value, formatter, mediaType);
		}

		/// <summary>
		/// Specifies the <paramref name="statusCode"/> and <paramref name="value"/> to respond with for a request.
		/// </summary>
		/// <param name="responds"></param>
		/// <param name="statusCode">The status code response for given request.</param>
		/// <param name="value">The response value.</param>
		/// <param name="formatter">The media type formatter</param>
		/// <param name="mediaType">The media type. Can be null, in which case the <paramref name="formatter"/> default content type will be used.</param>
		public static IResponseResult RespondObject<T>(this IResponds responds, HttpStatusCode statusCode, T value, MediaTypeFormatter formatter, MediaTypeHeaderValue mediaType)
		{
			return responds.Respond(() => new HttpResponseMessage(statusCode)
			{
				Content = new ObjectContent<T>(value, formatter, mediaType)
			});
		}
#endif
	}
}
