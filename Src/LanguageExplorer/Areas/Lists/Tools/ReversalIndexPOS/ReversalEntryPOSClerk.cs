// Copyright (c) 2015-2018 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using LanguageExplorer.Areas.Lexicon;
using SIL.FieldWorks.FDO;
using SIL.FieldWorks.FDO.Application;
using SIL.FieldWorks.Filters;

namespace LanguageExplorer.Areas.Lists.Tools.ReversalIndexPOS
{
	/// <summary>
	/// This clerk is used to deal with the POSes of a IReversalIndex.
	/// </summary>
	internal sealed class ReversalEntryPOSClerk : ReversalClerk
	{
		/// <summary />
		internal ReversalEntryPOSClerk(IFdoServiceLocator serviceLocator, ISilDataAccessManaged decorator, IReversalIndex reversalIndex)
			: base("ReversalEntriesPOS", new ReversalIndexPOSRecordList(serviceLocator, decorator, reversalIndex), new PropertyRecordSorter("ShortName"), "Default", null, true, true)
		{
		}

		/// <summary />
		protected override ICmObject NewOwningObject(IReversalIndex ri)
		{
			return ri.PartsOfSpeechOA;
		}
	}
}
