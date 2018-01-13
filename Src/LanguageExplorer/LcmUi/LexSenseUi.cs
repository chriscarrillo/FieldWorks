// Copyright (c) 2015-2018 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LanguageExplorer.Controls.LexText;
using SIL.LCModel;
using SIL.LCModel.Core.KernelInterfaces;
using SIL.LCModel.Core.Text;
using SIL.LCModel.Infrastructure;

namespace LanguageExplorer.LcmUi
{
	/// <summary>
	/// UI functions for MoMorphSynAnalysis.
	/// </summary>
	public class LexSenseUi : CmObjectUi
	{
		/// <summary>
		/// Create one.
		/// </summary>
		/// <param name="obj"></param>
		public LexSenseUi(ICmObject obj)
			: base(obj)
		{
			Debug.Assert(obj is ILexSense);
		}

		internal LexSenseUi() { }

		/// <summary>
		/// gives the hvo of the object to use in the URL we construct when doing a jump
		/// </summary>
		/// <param name="commandObject"></param>
		/// <returns></returns>
		public override Guid GuidForJumping(object commandObject)
		{
#if RANDYTODO
			var command = (Command) commandObject;
			string className = XmlUtils.GetMandatoryAttributeValue(command.Parameters[0], "className");
			if (className == "LexSense")
				return Object.Guid;
#endif
			ICmObject cmo = GetSelfOrParentOfClass(Object, LexEntryTags.kClassId);
			return (cmo == null) ? Guid.Empty : cmo.Guid;
		}

#if RANDYTODO
/// <summary>
/// disable/hide delete selected item for LexSenses (eg. since we don't want them to delete all senses
/// from its owning entry.)
/// </summary>
/// <param name="commandObject"></param>
/// <param name="display"></param>
/// <returns></returns>
		public override bool OnDisplayDeleteSelectedItem(object commandObject, ref UIItemDisplayProperties display)
		{
			display.Visible = false;
			display.Enabled = false;
			display.Text = String.Format(display.Text, DisplayNameOfClass);
			return true;
		}
#endif

		protected override bool ShouldDisplayMenuForClass(int specifiedClsid)
		{
			return LexEntryTags.kClassId == specifiedClsid || LexSenseTags.kClassId == specifiedClsid;
		}

		protected override DummyCmObject GetMergeinfo(WindowParams wp, List<DummyCmObject> mergeCandidates, out string guiControl, out string helpTopic)
		{
			wp.m_title = LcmUiStrings.ksMergeSense;
			wp.m_label = LcmUiStrings.ksSenses;
			int defAnalWs = m_cache.ServiceLocator.WritingSystems.DefaultAnalysisWritingSystem.Handle;

			var sense = (ILexSense) Object;
			var le = sense.Entry;
			// Exclude subsenses of the chosen sense.  See LT-6107.
			var rghvoExclude = new List<int>();
			foreach (var ls in sense.AllSenses)
				rghvoExclude.Add(ls.Hvo);
			foreach (var senseInner in le.AllSenses)
			{
				if (senseInner != Object && !rghvoExclude.Contains(senseInner.Hvo))
				{
					// Make sure we get the actual WS used (best analysis would be the
					// descriptive term) for the ShortName.  See FWR-2812.
					ITsString tssName = senseInner.ShortNameTSS;
					mergeCandidates.Add(
						new DummyCmObject(
							senseInner.Hvo,
							tssName.Text,
							TsStringUtils.GetWsAtOffset(tssName, 0)));
				}
			}
			guiControl = "MergeSenseList";
			helpTopic = "khtpMergeSense";
			ITsString tss = Object.ShortNameTSS;
			return new DummyCmObject(m_hvo, tss.Text, TsStringUtils.GetWsAtOffset(tss, 0));
		}

		public override void MoveUnderlyingObjectToCopyOfOwner()
		{
			CheckDisposed();

			var obj = Object.Owner;
			int clid = obj.ClassID;
			while (clid != LexEntryTags.kClassId)
			{
				obj = obj.Owner;
				clid = obj.ClassID;
			}
			var le = (ILexEntry) obj;
			le.MoveSenseToCopy((ILexSense) Object);
		}

		/// <summary>
		/// When inserting a LexSense, copy the MSA from the one we are inserting after, or the
		/// first one.  If this is the first one, we may need to create an MSA if the owning entry
		/// does not have an appropriate one.
		/// </summary>
		public static LexSenseUi CreateNewUiObject(LcmCache cache, int hvoOwner, int insertionPosition = int.MaxValue)
		{
			if (cache == null)
				throw new ArgumentNullException(nameof(cache));

			var owner = cache.ServiceLocator.GetInstance<ICmObjectRepository>().GetObject(hvoOwner);
			if (owner is ILexEntry)
			{
				return CreateNewLexSenseUiObject(cache, owner as ILexEntry, insertionPosition);
			}
			if (owner is ILexSense)
			{
				return CreateNewLexSenseUiObject(cache, owner as ILexSense, insertionPosition);
			}
			throw new ArgumentOutOfRangeException(nameof(hvoOwner), $"Owner must be an ILexEntry or an ILexSense, but it was: '{owner.ClassName}'.");
		}

		/// <summary>
		/// Create a new LexSenseUi in the given entry.
		/// </summary>
		internal static LexSenseUi CreateNewLexSenseUiObject(LcmCache cache, ILexEntry ownerEntry, int insertionPosition)
		{
			return new LexSenseUi(CreateNewLexSense(cache, ownerEntry, insertionPosition));
		}

		/// <summary>
		/// Create a new LexSenseUi in the given sense.
		/// </summary>
		internal static LexSenseUi CreateNewLexSenseUiObject(LcmCache cache, ILexSense ownerSense, int insertionPosition = int.MaxValue)
		{
			return new LexSenseUi(CreateNewLexSense(cache, ownerSense, insertionPosition));
		}

		/// <summary>
		/// Create a new LexSense in the given entry.
		/// </summary>
		internal static ILexSense CreateNewLexSense(LcmCache cache, ILexEntry ownerEntry, int insertionPosition = int.MaxValue)
		{
			ILexSense newSense = null;
			UndoableUnitOfWorkHelper.Do(LcmUiStrings.ksUndoInsertSense, LcmUiStrings.ksRedoInsertSense, cache.ServiceLocator.GetInstance<IActionHandler>(), () =>
			{
				IMoMorphSynAnalysis msa;
				var entrySenseCount = ownerEntry.SensesOS.Count;
				var appendNewSense = insertionPosition >= entrySenseCount;
				if (entrySenseCount == 0)
				{
					// No senses at all.
					// If we don't get the MSA here, trouble ensues.  See LT-5411.
					msa = ownerEntry.FindOrCreateDefaultMsa();
				}
				else
				{
					// Use the MSA from the sense right before the location we want the new one to go into.
					ILexSense senseToGetMsaFrom;
					if (insertionPosition == 0)
					{
						// Use first sense.
						senseToGetMsaFrom = ownerEntry.SensesOS.First();
					}
					else if (appendNewSense)
					{
						// Use last sense.
						senseToGetMsaFrom = ownerEntry.SensesOS.Last();
					}
					else
					{
						// Use the one before the insertion point.
						senseToGetMsaFrom = ownerEntry.SensesOS[insertionPosition - 1];
					}
					msa = GetSafeMsa(cache, senseToGetMsaFrom);
				}
				newSense = cache.ServiceLocator.GetInstance<ILexSenseFactory>().Create();
				if (appendNewSense)
				{
					ownerEntry.SensesOS.Add(newSense);
				}
				else
				{
					ownerEntry.SensesOS.Insert(insertionPosition, newSense);
				}
				newSense.MorphoSyntaxAnalysisRA = msa;
			});
			return newSense;
		}

		/// <summary>
		/// Create a new LexSense in the given sense.
		/// </summary>
		internal static ILexSense CreateNewLexSense(LcmCache cache, ILexSense ownerSense, int insertionPosition = int.MaxValue)
		{
			ILexSense newSense = null;
			UndoableUnitOfWorkHelper.Do(LcmUiStrings.ksUndoInsertSense, LcmUiStrings.ksRedoInsertSense, cache.ServiceLocator.GetInstance<IActionHandler>(), () =>
			{
				IMoMorphSynAnalysis msa;
				var senseSubsenseCount = ownerSense.SensesOS.Count;
				var appendNewSense = (insertionPosition == int.MaxValue) || (insertionPosition >= senseSubsenseCount);
				if (senseSubsenseCount == 0)
				{
					// No senses at all.
					// If we don't get the MSA here, trouble ensues.  See LT-5411.
					msa = ownerSense.Entry.FindOrCreateDefaultMsa();
				}
				else
				{
					// Use the MSA from the sense right before the location we want the new one to go into.
					ILexSense senseToGetMsaFrom;
					if (insertionPosition == 0)
					{
						// Use first sense.
						senseToGetMsaFrom = ownerSense.SensesOS.First();
					}
					else if (appendNewSense)
					{
						// Use last sense.
						senseToGetMsaFrom = ownerSense.SensesOS.Last();
					}
					else
					{
						// Use the one before the insertion point.
						senseToGetMsaFrom = ownerSense.SensesOS[insertionPosition - 1];
					}
					msa = GetSafeMsa(cache, senseToGetMsaFrom);
				}
				newSense = cache.ServiceLocator.GetInstance<ILexSenseFactory>().Create();
				if (appendNewSense)
				{
					ownerSense.SensesOS.Add(newSense);
				}
				else
				{
					ownerSense.SensesOS.Insert(insertionPosition, newSense);
				}
				newSense.MorphoSyntaxAnalysisRA = msa;
			});
			return newSense;
		}

		/// <summary>
		/// This method will get an MSA which the senses MorphoSyntaxAnalysisRA points to.
		/// If it is null it will try and find an appropriate one in the owning Entries list, if that fails it will make one and put it there.
		/// </summary>
		/// <param name="cache"></param>
		/// <param name="sense">LexSense whose MSA we will use/change</param>
		/// <returns></returns>
		private static IMoMorphSynAnalysis GetSafeMsa(LcmCache cache, ILexSense sense)
		{
			if (sense.MorphoSyntaxAnalysisRA != null)
			{
				//situation normal, return
				return sense.MorphoSyntaxAnalysisRA;
			}

			//Situation not normal.
			var entryPrimaryMorphType = sense.Entry.PrimaryMorphType; // Guard against corrupted data. Every entry should have a PrimaryMorphType
			var isAffixType = entryPrimaryMorphType?.IsAffixType ?? false;
			foreach (var msa in sense.Entry.MorphoSyntaxAnalysesOC) //go through each MSA in the Entry list looking for one with an unknown category
			{
				if (!isAffixType && msa is IMoStemMsa && (msa as IMoStemMsa).PartOfSpeechRA == null)
				{
					sense.MorphoSyntaxAnalysisRA = msa;
					return msa;
				}
				if (msa is IMoUnclassifiedAffixMsa && (msa as IMoUnclassifiedAffixMsa).PartOfSpeechRA == null)
				{
					sense.MorphoSyntaxAnalysisRA = msa;
					return msa;
				}
			}
			if (sense.MorphoSyntaxAnalysisRA != null)
			{
				return sense.MorphoSyntaxAnalysisRA;
			}
			var safeMsa = isAffixType
				? (IMoMorphSynAnalysis)cache.ServiceLocator.GetInstance<IMoUnclassifiedAffixMsaFactory>().Create()
				: (IMoMorphSynAnalysis)cache.ServiceLocator.GetInstance<IMoStemMsaFactory>().Create();
			sense.Entry.MorphoSyntaxAnalysesOC.Add(safeMsa);
			sense.MorphoSyntaxAnalysisRA = safeMsa;
			return sense.MorphoSyntaxAnalysisRA;
		}
	}
}