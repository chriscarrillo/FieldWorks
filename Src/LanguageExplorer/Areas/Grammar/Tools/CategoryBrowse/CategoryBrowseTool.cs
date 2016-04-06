﻿// Copyright (c) 2015 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System.Drawing;
using System.Windows.Forms;
using System.Xml.Linq;
using LanguageExplorer.Controls;
using SIL.CoreImpl;
using SIL.FieldWorks.Common.FwUtils;
using SIL.FieldWorks.Resources;
using SIL.FieldWorks.XWorks;
using SIL.Utils;

namespace LanguageExplorer.Areas.Grammar.Tools.CategoryBrowse
{
	/// <summary>
	/// ITool implementation for the "categoryBrowse" tool in the "grammar" area.
	/// </summary>
	internal sealed class CategoryBrowseTool : ITool
	{
		private XDocument _configurationDocument;
		private PaneBarContainer _paneBarContainer;
		private RecordClerk _recordClerk;

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
		}

		#endregion

		#region Implementation of IMajorFlexComponent

		/// <summary>
		/// Deactivate the component.
		/// </summary>
		/// <remarks>
		/// This is called on the outgoing component, when the user switches to a component.
		/// </remarks>
		public void Deactivate(ICollapsingSplitContainer mainCollapsingSplitContainer, MenuStrip menuStrip, ToolStripContainer toolStripContainer,
			StatusBar statusbar)
		{
			PaneBarContainerFactory.RemoveFromParentAndDispose(ref _paneBarContainer, ref _recordClerk);
		}

		/// <summary>
		/// Activate the component.
		/// </summary>
		/// <remarks>
		/// This is called on the component that is becoming active.
		/// </remarks>
		public void Activate(ICollapsingSplitContainer mainCollapsingSplitContainer, MenuStrip menuStrip, ToolStripContainer toolStripContainer,
			StatusBar statusbar)
		{
			_configurationDocument = XDocument.Parse(GrammarResources.GrammarCategoryBrowserParameters);
			_recordClerk = GrammarArea.CreateBrowseClerkForGrammarArea(PropertyTable, false);
			var flexComponentParameterObject = new FlexComponentParameters(PropertyTable, Publisher, Subscriber);
			_recordClerk.InitializeFlexComponent(flexComponentParameterObject);
			_paneBarContainer = PaneBarContainerFactory.Create(
				flexComponentParameterObject,
				mainCollapsingSplitContainer.SecondControl,
				new RecordBrowseView(_configurationDocument.Root, _recordClerk));
		}

		/// <summary>
		/// Do whatever might be needed to get ready for a refresh.
		/// </summary>
		public void PrepareToRefresh()
		{
		}

		/// <summary>
		/// Finish the refresh.
		/// </summary>
		public void FinishRefresh()
		{
#if RANDYTODO
			// TODO: If tool uses a SDA decorator (IRefreshable), then call its "Refresh" method.
#endif
			_recordClerk.ReloadIfNeeded();
			((IRefreshable)_recordClerk.VirtualListPublisher).Refresh();
		}

		/// <summary>
		/// The properties are about to be saved, so make sure they are all current.
		/// Add new ones, as needed.
		/// </summary>
		public void EnsurePropertiesAreCurrent()
		{
		}

		#endregion

		#region Implementation of IMajorFlexUiComponent

		/// <summary>
		/// Get the internal name of the component.
		/// </summary>
		/// <remarks>NB: This is the machine friendly name, not the user friendly name.</remarks>
		public string MachineName
		{
			get { return "categoryBrowse"; }
		}

		/// <summary>
		/// User-visible localizable component name.
		/// </summary>
		public string UiName
		{
			get { return "Categories Browse"; }
		}

		#endregion

		#region Implementation of ITool

		/// <summary>
		/// Get the area machine name the tool is for.
		/// </summary>
		public string AreaMachineName
		{
			get { return "grammar"; }
		}

		/// <summary>
		/// Get the image for the area.
		/// </summary>
		public Image Icon
		{
			get
			{
				var image = Images.BrowseView;
				image.MakeTransparent(Color.Magenta);
				return image;
			}
		}

		#endregion
	}
}
