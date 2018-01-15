﻿// Copyright (c) 2015-2018 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;
using SIL.LCModel.Core.Cellar;
using SIL.LCModel;
using SIL.LCModel.Application;
using SIL.LCModel.Infrastructure;

namespace LanguageExplorer.Controls.XMLViews
{
	/// <summary>
	/// This class helps manage "ghost" virtual properties. The characteristic of such a property is that
	/// it contains a mixture of objects of a 'signature' class (or a subclass) and objects of a 'parent'
	/// class, which are put into the list when they have no children of the signature class in a specified
	/// owning property. Bulk edit operations may insert a suitable child if necessary to set a value for
	/// a parent-type object.
	/// </summary>
	public class GhostParentHelper
	{
		internal ILcmServiceLocator m_services; // think of it as protected, but limited to this assembly.

		// The class of objects that are considered parents (they don't have the basic property we
		// try to set).
		// The property of m_parentClsid that owns signature objects.
		private int m_flidOwning;
		// Index at which to insert a new child; hence indicates type of m_flidOwning.
		private int m_indexToCreate;

		/// <summary>
		/// Returns GPHs for the four properties we currently know about.
		/// </summary>
		/// <param name="services"></param>
		/// <param name="classDotMethod"></param>
		/// <returns></returns>
		public static GhostParentHelper Create(ILcmServiceLocator services, string classDotMethod)
		{
			var result = CreateIfPossible(services, classDotMethod);
			if (result == null)
			{
				throw new ArgumentException(@"Unexpected field request to GhostParentHelper.Create", nameof(classDotMethod));
			}
			return result;
		}
		/// <summary>
		/// Returns GPHs for the four properties we currently know about, or null if not a known property that has ghosts.
		/// </summary>
		public static GhostParentHelper CreateIfPossible(ILcmServiceLocator services, string classDotMethod)
		{
			switch (classDotMethod)
			{
				case "LexDb.AllPossiblePronunciations":
					return new GhostParentHelper(services, LexEntryTags.kClassId, LexEntryTags.kflidPronunciations);
				case "LexDb.AllPossibleEtymologies":
					return new GhostParentHelper(services, LexEntryTags.kClassId, LexEntryTags.kflidEtymology);
				case "LexDb.AllPossibleAllomorphs":
					return new GphAllPossibleAllomorphs(services, LexEntryTags.kClassId, LexEntryTags.kflidAlternateForms);
				case "LexDb.AllExampleSentenceTargets":
					return new GhostParentHelper(services, LexSenseTags.kClassId, LexSenseTags.kflidExamples);
				case "LexDb.AllExampleTranslationTargets":
					return new GhostParentHelper(services, LexExampleSentenceTags.kClassId, LexExampleSentenceTags.kflidTranslations);
				case "LexDb.AllComplexEntryRefPropertyTargets":
					return new GphComplexEntries(services);
				case "LexDb.AllVariantEntryRefPropertyTargets":
					return new GphVariants(services);
				case "LexDb.AllExtendedNoteTargets":
					return new GhostParentHelper(services, LexSenseTags.kClassId, LexSenseTags.kflidExtendedNote);
				case "LexDb.AllPossiblePictures":
					return new GhostParentHelper(services, LexSenseTags.kClassId, LexSenseTags.kflidPictures);
				default:
					return null;
			}
		}

		/// <summary>
		/// Get the destination class for the specified flid, as used in bulk edit. For this purpose
		/// we need to override the destination class of the fields that have ghost parent helpers,
		/// since the properties hold a mixture of classes and therefore have CmObject as their
		/// signature, but the bulk edit code needs to treat them as having the class they primarily contain.
		/// </summary>
		public static int GetBulkEditDestinationClass(LcmCache cache, int listFlid)
		{
			var destClass = cache.GetDestinationClass(listFlid);
			if (destClass == 0)
			{
				// May be a special "ghost" property used for bulk edit operations which primarily contains,
				// say, example sentences, but also contains senses and entries so we can bulk edit to senses
				// with no examples and entries with no senses.
				// We don't want to lie to the MDC, but here, we need to treat these properties as having the
				// primary destination class.
				switch (cache.MetaDataCacheAccessor.GetFieldName(listFlid))
				{
					case "AllExampleSentenceTargets":
						return LexExampleSentenceTags.kClassId;
					case "AllPossiblePronunciations":
						return LexPronunciationTags.kClassId;
					case "AllPossibleEtymologies":
						return LexEtymologyTags.kClassId;
					case "AllPossibleAllomorphs":
						return MoFormTags.kClassId;
					case "AllExampleTranslationTargets":
						return CmTranslationTags.kClassId;
					case "AllComplexEntryRefPropertyTargets":
					case "AllVariantEntryRefPropertyTargets":
						return LexEntryRefTags.kClassId;
					case "AllExtendedNoteTargets":
						return LexExtendedNoteTags.kClassId;
					case "AllPossiblePictures":
						return CmPictureTags.kClassId;
				}
			}
			return destClass;
		}

		/// <summary>
		/// Return a ghost parent helper based on a flid, or null if this flid does not need one.
		/// </summary>
		public static GhostParentHelper CreateIfPossible(ILcmServiceLocator services, int flid)
		{
			var mdc = services.MetaDataCache;
			return CreateIfPossible(services, mdc.GetOwnClsName(flid) + "." + mdc.GetFieldName(flid));
		}

		internal GhostParentHelper(ILcmServiceLocator services, int parentClsid, int flidOwning)
		{
			m_services = services;
			GhostOwnerClass = parentClsid;
			m_flidOwning = flidOwning;
			var mdc = m_services.GetInstance<IFwMetaDataCacheManaged>();
			TargetClass = mdc.GetDstClsId(flidOwning);
			switch ((CellarPropertyType)mdc.GetFieldType(flidOwning))
			{
				case CellarPropertyType.OwningAtomic:
					m_indexToCreate = -2;
					break;
				case CellarPropertyType.OwningCollection:
					m_indexToCreate = -1;
					break;
				case CellarPropertyType.OwningSequence:
					m_indexToCreate = 0;
					break;
				default:
					throw new InvalidOperationException("can only create objects in owning properties");
			}
		}

		/// <summary>
		/// The class of objects we expect to be the children; the destination class of FlidOwning.
		/// </summary>
		public int TargetClass { get; }

		/// <summary>
		/// Get the object related to hvo that has the basic properties of interest: that is, a child object.
		/// hvo is assumed to be the desired object unless it is of the parent class, in which case,
		/// we return its first child if any, or zero if it has no relevant children.
		/// </summary>
		public int GetOwnerOfTargetProperty(int hvo)
		{
			var hvoTargetOwner = hvo; // by default, we assume hvo is the owner
			if (IsGhostOwnerClass(hvo))
			{
				if (IsGhostOwnerChildless(hvo))
				{
					return 0;
				}
				// this owning object has a child, so get the property from it
				hvoTargetOwner = GetFirstChildFromParent(hvo);
			}
			return hvoTargetOwner;
		}

		/// <summary>
		/// Override if you should always create a particular class.
		/// </summary>
		internal virtual int ClassToCreate(int hvoItem, int flidBasicProp)
		{
			var mdc = m_services.GetInstance<IFwMetaDataCacheManaged>();
			return mdc.GetOwnClsId((int)flidBasicProp);
		}

		/// <summary>
		/// Like GetOwnerOfTargetProperty, but will create a child if necessary so as never to return zero.
		/// Caller must ensure we are in a UOW.
		/// </summary>
		public int FindOrCreateOwnerOfTargetProp(int hvoItem, int flidBasicProp)
		{
			var hvoOwnerOfTargetProp = GetOwnerOfTargetProperty(hvoItem);
			if (hvoOwnerOfTargetProp == 0)
			{
				hvoOwnerOfTargetProp = CreateOwnerOfTargetProp(hvoItem, flidBasicProp);
			}
			return hvoOwnerOfTargetProp;
		}

		/// <summary>
		/// create the first child for this ghost owner
		/// </summary>
		internal virtual int CreateOwnerOfTargetProp(int hvoItem, int flidBasicProp)
		{
			return GetSda().MakeNewObject(ClassToCreate(hvoItem, flidBasicProp), hvoItem, m_flidOwning, m_indexToCreate);
		}

		internal ISilDataAccessManaged GetSda()
		{
			return m_services.GetInstance<ISilDataAccessManaged>();
		}

		/// <summary>
		/// Return true if the target object (which must be of the owner class) has no children in the relevant property.
		/// </summary>
		public virtual bool IsGhostOwnerChildless(int hvoItem)
		{
			return IsOwningPropVector() ? GetSda().get_VecSize(hvoItem, m_flidOwning) == 0 : GetSda().get_ObjectProp(hvoItem, m_flidOwning) == 0;
		}

		private bool IsOwningPropVector()
		{
			return m_indexToCreate != -2;
		}

		internal virtual int GetFirstChildFromParent(int hvoParent)
		{
			return IsOwningPropVector() ? GetSda().get_VecItem(hvoParent, m_flidOwning, 0) : GetSda().get_ObjectProp(hvoParent, m_flidOwning);
		}

		/// <summary>
		/// Return true if the object represented by the HVO is of the parent object class.
		/// Enhance JohnT: improve name!
		/// </summary>
		public bool IsGhostOwnerClass(int hvo)
		{
			return GhostOwnerClass == m_services.GetObject(hvo).ClassID;
		}

		/// <summary>
		/// Answer the class of parent objects.
		/// </summary>
		public int GhostOwnerClass { get; }
	}
}
