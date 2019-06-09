﻿using System.ComponentModel;

namespace HttpClientMock.Language
{
	/// <summary>
	/// Defines the <c>Responds</c> and <c>Throws</c> verb.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IRespondsThrows : IResponds, IThrows, IFluentInterface
	{
	}
}