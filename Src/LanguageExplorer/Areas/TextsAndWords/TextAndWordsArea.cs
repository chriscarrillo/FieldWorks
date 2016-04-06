﻿// Copyright (c) 2015 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SIL.CoreImpl;
using SIL.FieldWorks.Common.FwUtils;

namespace LanguageExplorer.Areas.TextsAndWords
{
	/// <summary>
	/// IArea implementation for the area: "textAndWords".
	/// </summary>
	internal sealed class TextAndWordsArea : IArea
	{
		private readonly IToolRepository m_toolRepository;

		/// <summary>
		/// Contructor used by Reflection to feed the tool repository to the area.
		/// </summary>
		/// <param name="toolRepository"></param>
		internal TextAndWordsArea(IToolRepository toolRepository)
		{
			m_toolRepository = toolRepository;
		}

		#region Implementation of IPropertyTableProvider

		/// <summary>
		/// Placement in the IPropertyTableProvider interface lets FwApp call IPropertyTable.DoStuff.
		/// </summary>
		public IPropertyTable PropertyTable { get; private set; }

		#endregion

		#region Implementation of IPublisherProvider

		/// <summary>
		/// Get the IPublisher.
		/// </summary>
		public IPublisher Publisher { get; private set; }

		#endregion

		#region Implementation of ISubscriberProvider

		/// <summary>
		/// Get the ISubscriber.
		/// </summary>
		public ISubscriber Subscriber { get; private set; }

		#endregion

		#region Implementation of IFlexComponent

		/// <summary>
		/// Initialize a FLEx component with the basic interfaces.
		/// </summary>
		/// <param name="flexComponentParameters">Parameter object that contains the required three interfaces.</param>
		public void InitializeFlexComponent(FlexComponentParameters flexComponentParameters)
		{
			FlexComponentCheckingService.CheckInitializationValues(flexComponentParameters, new FlexComponentParameters(PropertyTable, Publisher, Subscriber));

			PropertyTable = flexComponentParameters.PropertyTable;
			Publisher = flexComponentParameters.Publisher;
			Subscriber = flexComponentParameters.Subscriber;

			// Respeller dlg uses these.
			PropertyTable.SetDefault("RemoveAnalyses", true, SettingsGroup.GlobalSettings, true, false);
			PropertyTable.SetDefault("UpdateLexiconIfPossible", true, SettingsGroup.GlobalSettings, true, false);
			PropertyTable.SetDefault("CopyAnalysesToNewSpelling", true, SettingsGroup.GlobalSettings, true, false);
			PropertyTable.SetDefault("MaintainCaseOnChangeSpelling", true, SettingsGroup.GlobalSettings, true, false);

			// <property name="ShowMorphBundles" bool="true" persist="false" settingsGroup="local"/>
			PropertyTable.RemoveProperty("ShowMorphBundles", SettingsGroup.LocalSettings);

			// <property name="ITexts_AddWordsToLexicon" bool="false" persist="true" settingsGroup="local" />
			// <property name="ITexts_ShowAddWordsToLexiconDlg" bool="true" persist="true" settingsGroup="local" />
			// <property name="ITexts-ScriptureIds" value="" persist="true" settingsGroup="local" />
			PropertyTable.SetDefault("ITexts_AddWordsToLexicon", false, SettingsGroup.LocalSettings, true, false);
			PropertyTable.SetDefault("ITexts_ShowAddWordsToLexiconDlg", true, SettingsGroup.LocalSettings, true, false);
			PropertyTable.SetDefault("ITexts-ScriptureIds", string.Empty, SettingsGroup.LocalSettings, true, false);
		}

		#endregion

		#region Implementation of IMajorFlexComponent

		/// <summary>
		/// Deactivate the component.
		/// </summary>
		/// <remarks>
		/// This is called on the outgoing component, when the user switches to a component.
		/// </remarks>
		public void Deactivate(ICollapsingSplitContainer mainCollapsingSplitContainer, MenuStrip menuStrip,
			ToolStripContainer toolStripContainer, StatusBar statusbar)
		{
		}

		#endregion

		#region Implementation of IMajorFlexComponent

		/// <summary>
		/// Activate the component.
		/// </summary>
		/// <remarks>
		/// This is called on the component that is becoming active.
		/// </remarks>
		public void Activate(ICollapsingSplitContainer mainCollapsingSplitContainer, MenuStrip menuStrip,
			ToolStripContainer toolStripContainer, StatusBar statusbar)
		{
		}

		/// <summary>
		/// Do whatever might be needed to get ready for a refresh.
		/// </summary>
		public void PrepareToRefresh()
		{
			m_toolRepository.GetPersistedOrDefaultToolForArea(this).PrepareToRefresh();
		}

		/// <summary>
		/// Finish the refresh.
		/// </summary>
		public void FinishRefresh()
		{
			m_toolRepository.GetPersistedOrDefaultToolForArea(this).FinishRefresh();
		}

		/// <summary>
		/// The properties are about to be saved, so make sure they are all current.
		/// Add new ones, as needed.
		/// </summary>
		public void EnsurePropertiesAreCurrent()
		{
			PropertyTable.SetProperty("InitialArea", MachineName, SettingsGroup.LocalSettings, true, false);

			var myCurrentTool = m_toolRepository.GetPersistedOrDefaultToolForArea(this);
			myCurrentTool.EnsurePropertiesAreCurrent();
		}

		#endregion

		#region Implementation of IMajorFlexUiComponent

		/// <summary>
		/// Get the internal name of the component.
		/// </summary>
		/// <remarks>NB: This is the machine friendly name, not the user friendly name.</remarks>
		public string MachineName
		{
			get { return "textAndWords"; }
		}

		/// <summary>
		/// User-visible localizable component name.
		/// </summary>
		public string UiName
		{
			get { return "Texts & Words"; }
		}

		#endregion

		#region Implementation of IArea

		/// <summary>
		/// Get the most recently persisted tool, or the default tool if
		/// the persisted one is no longer available.
		/// </summary>
		/// <returns>The last persisted tool or the default tool for the area.</returns>
		public ITool GetPersistedOrDefaultToolForArea()
		{
			return m_toolRepository.GetPersistedOrDefaultToolForArea(this);
		}

		/// <summary>
		/// Get the machine name of the area's default tool.
		/// </summary>
		public string DefaultToolMachineName
		{
			get { return "interlinearEdit"; }
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
					"interlinearEdit",
					"concordance",
					"complexConcordance",
					"wordListConcordance",
					"Analyses",
					"bulkEditWordforms",
					"corpusStatistics"
				};
				return m_toolRepository.AllToolsForAreaInOrder(myToolsInOrder, MachineName);
			}
		}

		/// <summary>
		/// Get the image for the area.
		/// </summary>
		public Image Icon
		{
			get { return LanguageExplorerResources.Text_And_Words.ToBitmap(); }
		}

		#endregion
	}
}
