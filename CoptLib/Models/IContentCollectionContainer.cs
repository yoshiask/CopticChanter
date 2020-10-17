using System.Collections.Generic;

namespace CoptLib.Models
{
	/// <summary>
	/// Interface for any model that contains a collection of <see cref="ContentPart"/>s.
	/// </summary>
	public interface IContentCollectionContainer
	{
		List<ContentPart> Content { get; set; }

		int CountRows();
	}
}
