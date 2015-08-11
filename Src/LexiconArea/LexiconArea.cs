﻿// Copyright (c) 2015 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SIL.CoreImpl;

namespace LexiconAreaPlugin
{
	/// <summary>
	/// IArea implementation for the main, and thus only required, Area: "lexicon".
	/// </summary>
	internal sealed class LexiconArea : IArea
	{
		private readonly IToolRepository m_toolRepository;

		/// <summary>
		/// Contructor used by Reflection to feed the tool repository to the area.
		/// </summary>
		/// <param name="toolRepository"></param>
		internal LexiconArea(IToolRepository toolRepository)
		{
			m_toolRepository = toolRepository;
		}

		#region Implementation of IMajorFlexComponent

		/// <summary>
		/// Get the internal name of the component.
		/// </summary>
		/// <remarks>NB: This is the machine friendly name, not the user friendly name.</remarks>
		public string MachineName { get { return "lexicon"; } }

		/// <summary>
		/// User-visible localizable component name.
		/// </summary>
		public string UiName
		{
			get { return "Lexical Tools"; }
		}

		/// <summary>
		/// Deactivate the component.
		/// </summary>
		/// <remarks>
		/// This is called on the outgoing component, when the user switches to a component.
		/// </remarks>
		public void Deactivate(IPropertyTable propertyTable,
			IPublisher publisher, ISubscriber subscriber,
			ICollapsingSplitContainer mainCollapsingSplitContainer,
			MenuStrip menuStrip, ToolStripContainer toolStripContainer, StatusBar statusbar)
		{
#if RANDYTODO
			// Implement and call Deactivate(parameters) on current tool in area.
			//MessageBoxUtils.Show(Form.ActiveForm, "Implement lexicon area Deactivate method.", "Need to implement", MessageBoxButtons.OK);
#endif
		}

		/// <summary>
		/// Activate the component.
		/// </summary>
		/// <remarks>
		/// This is called on the component that is becoming active.
		/// </remarks>
		public void Activate(IPropertyTable propertyTable,
			IPublisher publisher, ISubscriber subscriber,
			ICollapsingSplitContainer mainCollapsingSplitContainer,
			MenuStrip menuStrip, ToolStripContainer toolStripContainer, StatusBar statusbar)
		{
#if RANDYTODO
			// Implement and call Activate(parameters) on current/default tool in area.
			//MessageBoxUtils.Show(Form.ActiveForm, "Implement lexicon area Activate method.", "Need to implement", MessageBoxButtons.OK);
#endif
		}

		/// <summary>
		/// Do whatever might be needed to get ready for a refresh.
		/// </summary>
		/// <remarks>
		/// One might expect this method to pass this call into the area's current tool.
		/// </remarks>
		public void PrepareToRefresh()
		{
#if RANDYTODO
			// Implement and call PrepareToRefresh() on current tool in area.
			//MessageBoxUtils.Show(Form.ActiveForm, "Implement lexicon area PrepareToRefresh method.", "Need to implement", MessageBoxButtons.OK);
#endif
		}

		/// <summary>
		/// Finish the refresh.
		/// </summary>
		/// <remarks>
		/// One might expect this method to pass this call into the area's current tool.
		/// </remarks>
		public void FinishRefresh()
		{
#if RANDYTODO
			// Implement and call FinishRefresh() on current tool in area.
			//MessageBoxUtils.Show(Form.ActiveForm, "Implement lexicon area FinishRefresh method.", "Need to implement", MessageBoxButtons.OK);
#endif
		}

		/// <summary>
		/// The properties are about to be saved, so make sure they are all current.
		/// Add new ones, as needed.
		/// </summary>
		/// <param name="propertyTable">The table that is about to be persisted.</param>
		public void EnsurePropertiesAreCurrent(IPropertyTable propertyTable)
		{
#if RANDYTODO
			// Implement and call EnsurePropertiesAreCurrent() on current tool in area.
			//MessageBoxUtils.Show(Form.ActiveForm, "Implement lexicon area EnsurePropertiesAreCurrent method.", "Need to implement", MessageBoxButtons.OK);
#endif
		}

		#endregion

		#region Implementation of IArea

		/// <summary>
		/// Get the most recently persisted tool, or the default tool if
		/// the persisted one is no longer available.
		/// </summary>
		/// <returns>The last persisted tool or the default tool for the area.</returns>
		public ITool GetPersistedOrDefaultToolForArea(IPropertyTable propertyTable)
		{
			return m_toolRepository.GetPersistedOrDefaultToolForArea(propertyTable, this);
		}

		/// <summary>
		/// Get the machine name of the area's default tool.
		/// </summary>
		public string DefaultToolMachineName
		{
			get { return "lexiconEdit"; }
		}

		/// <summary>
		/// Get all installed tools for the area.
		/// </summary>
		public IList<ITool> AllToolsInOrder
		{
			get
			{
				var myToolsInOrder = new List<string>
				{
					"lexiconEdit",
					"lexiconBrowse",
					"lexiconDictionary",
					"rapidDataEntry",
					"lexiconClassifiedDictionary",
					"bulkEditEntriesOrSenses",
					"reversalEditComplete",
					"reversalBulkEditReversalEntries"
				};
				return m_toolRepository.AllToolsForAreaInOrder(myToolsInOrder, MachineName);
			}
		}

		/// <summary>
		/// Get the image for the area.
		/// </summary>
		public Image Icon
		{
			get { return Resources.Lexicon_32.ToBitmap(); }
		}

		#endregion
	}
}