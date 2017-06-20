// Copyright (c) 2015 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System.Windows.Forms;
using SIL.FieldWorks.FDO;
using SIL.FieldWorks.LexText.Controls;
using SIL.FieldWorks.Common.Framework.DetailControls;
using SIL.FieldWorks.FDO.Infrastructure;

namespace LanguageExplorer.Areas.Lexicon.Tools.ReversalIndexes
{
	/// <summary />
	internal sealed class RevEntrySensesCollectionReferenceLauncher : VectorReferenceLauncher
	{
		private System.ComponentModel.IContainer components = null;

		/// <summary />
		public RevEntrySensesCollectionReferenceLauncher()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			//Debug.WriteLineIf(!disposing, "****************** " + GetType().Name + " 'disposing' is false. ******************");
			// Must not be run more than once.
			if (IsDisposed)
				return;

			if( disposing )
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		/// <summary />
		protected override VectorReferenceView CreateVectorReferenceView()
		{
			return new RevEntrySensesCollectionReferenceView();
		}

		/// <summary>
		/// Override method to handle launching of a chooser for selecting lexical entries or senses.
		/// </summary>
		protected override void HandleChooser()
		{
			using (var dlg = new LinkEntryOrSenseDlg())
			{
				var wp = new WindowParams
				{
					m_title = LanguageExplorerResources.ksIdentifySense,
					m_btnText = LanguageExplorerResources.ksSetReversal
				};
				dlg.SetDlgInfo(m_cache, wp, PropertyTable, Publisher, Subscriber);
				dlg.SelectSensesOnly = true;
				if (dlg.ShowDialog(FindForm()) == DialogResult.OK && dlg.SelectedObject != null)
					AddItem(dlg.SelectedObject);
			}
		}

		/// <summary />
		public override void AddItem(ICmObject obj)
		{
			CheckDisposed();

			ILexSense selectedSense = obj as ILexSense;
			IFdoReferenceCollection<IReversalIndexEntry> col = selectedSense.ReversalEntriesRC;
			if (!col.Contains(m_obj as IReversalIndexEntry))
			{
				int h1 = m_vectorRefView.RootBox.Height;
				using (UndoableUnitOfWorkHelper helper = new UndoableUnitOfWorkHelper(
					m_cache.ActionHandlerAccessor, LanguageExplorerResources.ksUndoAddRevToSense,
					LanguageExplorerResources.ksRedoAddRevToSense))
				{
					col.Add(m_obj as IReversalIndexEntry);
					helper.RollBack = false;
				}
				int h2 = m_vectorRefView.RootBox.Height;
				CheckViewSizeChanged(h1, h2);
			}
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion
	}
}