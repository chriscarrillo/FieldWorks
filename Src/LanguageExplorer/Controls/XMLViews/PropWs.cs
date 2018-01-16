// Copyright (c) 2003-2018 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

namespace LanguageExplorer.Controls.XMLViews
{
	/// <summary/>
	public class PropWs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PropWs"/> class.
		/// </summary>
		public PropWs(int xflid, int xws)
		{
			flid = xflid;
			ws = xws;
		}
		/// <summary></summary>
		public int flid;
		/// <summary>0 if not applicable</summary>
		public int ws;

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
		/// <returns>
		/// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
		/// </returns>
		/// <exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
		public override bool Equals(object obj)
		{
			var other = obj as PropWs;
			if (other == null)
			{
				return false;
			}
			return other.flid == this.flid && other.ws == this.ws;
		}

		/// <summary>
		/// Probably not used but should have some overide when Equals overridden.
		/// </summary>
		public override int GetHashCode()
		{
			return flid * (ws + 11);
		}
	}
}