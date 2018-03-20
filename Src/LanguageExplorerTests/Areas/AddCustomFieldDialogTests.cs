﻿// Copyright (c) 2015-2018 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System.Linq;
using LanguageExplorer.Areas;
using NUnit.Framework;
using SIL.LCModel;
using SIL.WritingSystems;

namespace LanguageExplorerTests.Areas
{
	/// <summary>
	/// Tests for (what small part has automated tests of) AddCustomFieldDialog.
	/// </summary>
	[TestFixture]
	public class AddCustomFieldDialogTests : MemoryOnlyBackendProviderRestoredForEachTestTestBase
	{
		#region Overrides of LcmTestBase
		public override void FixtureSetup()
		{
			if (!Sldr.IsInitialized)
			{
				// initialize the SLDR
				Sldr.Initialize();
			}

			base.FixtureSetup();
		}

		public override void FixtureTeardown()
		{
			base.FixtureTeardown();

			if (Sldr.IsInitialized)
			{
				Sldr.Cleanup();
			}
		}
		#endregion

		/// <summary>
		/// Test the code that populates the Lists combo.
		/// </summary>
		[Test]
		public void PopulateListsCombo()
		{
			const string customListName = "Custom 1";
			var cmPossibilityListFactory = Cache.ServiceLocator.GetInstance<ICmPossibilityListFactory>();
			cmPossibilityListFactory.CreateUnowned(customListName, Cache.WritingSystemFactory.GetWsFromStr("en"));
			if (Cache.LangProject.SemanticDomainListOA == null)
			{
				Cache.LangProject.SemanticDomainListOA = cmPossibilityListFactory.Create();
			}
			if (Cache.LangProject.GenreListOA == null)
			{
				Cache.LangProject.GenreListOA = cmPossibilityListFactory.Create();
			}
			var possListRepository = Cache.ServiceLocator.GetInstance<ICmPossibilityListRepository>();
			var items = AddCustomFieldDlg.GetListsComboItems(possListRepository);
			Assert.That(items, Has.Length.EqualTo(possListRepository.Count));
			Assert.That(items.First(id => id.Name == customListName), Is.Not.Null);
		}
	}
}