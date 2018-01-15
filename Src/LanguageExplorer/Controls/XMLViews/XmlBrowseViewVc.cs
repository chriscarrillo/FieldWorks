// Copyright (c) 2003-2018 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System.Xml.Linq;

namespace LanguageExplorer.Controls.XMLViews
{
#if RANDYTODO
	// TODO: Why does this class exist, since it does nothing other than what the base class does.
#endif
	/// <summary>
	/// View constructor for BrowseView. The argument XmlNode represents XML like this:
	/// <browseview>
	///		<columns>
	///			<column width="10%" label="MyColumn">
	///
	///			</column>
	///			...
	///		</columns>
	///		<fragments>
	///			<frag>...</frag>...
	///		</fragments>
	/// </browseview>
	/// The body of each <column></column> node is the same as that of a <frag></frag>
	/// node, and represents what will be shown for each column for each object in the list.
	/// The additional <fragments></fragments> are passed to the base constructor,
	/// for use in interpreting any frag arguments in elements of the columns.
	/// No fragment should be marked as a root.
	/// Fragment 100000 is the root.
	/// </summary>
	internal class XmlBrowseViewVc : XmlBrowseViewBaseVc
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		internal XmlBrowseViewVc(XElement xnSpec, int madeUpFieldIdentifier, XmlBrowseViewBase xbv)
			: base(xnSpec, madeUpFieldIdentifier, xbv)
		{
		}
	}
}
