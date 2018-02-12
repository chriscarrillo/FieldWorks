﻿// Copyright (c) 2015-2018 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System.Linq;
using System.Windows.Forms;
using LanguageExplorer.Controls.DetailControls.Resources;
using LanguageExplorer.Controls.XMLViews;
using SIL.FieldWorks.Common.FwUtils;
using SIL.LCModel;
using SIL.LCModel.DomainServices;
using SIL.LCModel.Infrastructure;

namespace LanguageExplorer.Controls.DetailControls
{
	internal class GhostReferenceVectorLauncher: ButtonLauncher
	{
		// We want to emulate what ReferenceLauncher does, but without the object being created
		// until the user clicks OK in the simple list chooser.
		protected override void HandleChooser()
		{
			// YAGNI: may eventually need to make configurable how it comes up with the list of candidates.
			// Currently this is used only for properties of a ghost notebook record.
			var candidateList = (ICmPossibilityList) ReferenceTargetServices.RnGenericRecReferenceTargetOwner(m_cache, m_flid);
			var candidates = candidateList?.PossibilitiesOS;
			// YAGNI: see ReferenceLauncher implementation of this method for a possible approach to
			// making the choice of writing system configurable.
			var labels = ObjectLabel.CreateObjectLabels(m_cache, candidates, m_displayNameProperty, "analysis vernacular");
			var chooser = new SimpleListChooser(m_persistProvider,
				labels,
				m_fieldName,
				m_cache,
				new ICmObject[0],
				PropertyTable.GetValue<IHelpTopicProvider>("HelpTopicProvider"));
			chooser.SetHelpTopic(Slice.GetChooserHelpTopicID());

			chooser.SetObjectAndFlid(0, m_flid);
			if (Slice.ConfigurationNode != null)
			{
				// Review JohnT: can any of this be made relevant without an object?
				//    // Handle the default case ("owner") for text parameters.

				//    // This (old approach) works only if
				//    // all of the list items are owned by the same object as the first one in the
				//    // list.  (Later elements can be owned by elements owned by that first owner,
				//    // if you know what I mean.)
				//    //if (candidates.Count != 0)
				//    //    chooser.TextParamHvo = m_cache.GetOwnerOfObject((int)candidates[0]);
				//    // JohnT: this approach depends on a new LCM method.
				//    ICmObject referenceTargetOwner = m_obj.ReferenceTargetOwner(m_flid);
				//    if (referenceTargetOwner != null)
				//        chooser.TextParamHvo = referenceTargetOwner.Hvo;
				//    chooser.SetHelpTopic(Slice.GetChooserHelpTopicID());
				chooser.InitializeExtras(Slice.ConfigurationNode, PropertyTable);
			}
			var res = chooser.ShowDialog(FindForm());
			if (DialogResult.Cancel == res)
			{
				return;
			}

			if (chooser.HandleAnyJump())
			{
				return;
			}

			if (chooser.ChosenObjects != null && chooser.ChosenObjects.Any())
			{
				UndoableUnitOfWorkHelper.Do(string.Format(DetailControlsStrings.ksUndoSet, m_fieldName),
					string.Format(DetailControlsStrings.ksRedoSet, m_fieldName), m_obj,
					() =>
					{
						// YAGNI: creating the real object may eventually need to be configurable,
						// perhaps by indicating in the configuration node what class of object to create
						// and so forth, or perhaps by just putting a "doWhat" attribute on the configuration node
						// and making a switch here to control what is done. For now this slice is only used
						// in one situation, where we need to create a notebook record, associate the current object
						// with it, and add the values to it.
						((IText)m_obj).AssociateWithNotebook(false);
						IRnGenericRec notebookRec;
						DataTree.NotebookRecordRefersToThisText(m_obj as IText, out notebookRec);
						var recHvo = notebookRec.Hvo;
						var values = (from obj in chooser.ChosenObjects select obj.Hvo).ToArray();
						var listFlid = m_flid;
						if (m_flid == RnGenericRecTags.kflidParticipants)
						{
							var defaultRoledParticipant = notebookRec.MakeDefaultRoledParticipant();
							recHvo = defaultRoledParticipant.Hvo;
							listFlid = RnRoledParticTags.kflidParticipants;
						}
						m_cache.DomainDataByFlid.Replace(recHvo, listFlid, 0, 0, values, values.Length);
						// We don't do anything about updating the display because creating the real object
						// will typically destroy this slice altogether and replace it with a real one.
					});
				// Structure has changed drastically, start over.
				var index = Slice.IndexInContainer;
				var dataTree = Slice.ContainingDataTree;
				dataTree.RefreshList(false); // Slice will be destroyed!!
				if (index <= dataTree.Slices.Count - 1)
				{
					dataTree.CurrentSlice = dataTree.FieldAt(index);
				}
			}
		}
	}
}