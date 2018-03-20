// Copyright (c) 2007-2013 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using LanguageExplorer.LcmUi;
using NUnit.Framework;
using SIL.LCModel.Core.Text;
using SIL.LCModel;
using SIL.LCModel.DomainServices;
using SIL.WritingSystems;

namespace LanguageExplorerTests.LcmUi
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	///
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	[TestFixture]
	public class LcmUiTests : MemoryOnlyBackendProviderRestoredForEachTestTestBase
	{
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

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests FindEntryForWordform with empty string (related to TE-5916)
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void FindEntryForWordform_EmptyString()
		{
			using (var lexEntryUi = LexEntryUi.FindEntryForWordform(Cache, TsStringUtils.EmptyString(Cache.DefaultVernWs)))
			{
				Assert.IsNull(lexEntryUi);
			}
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests FindEntryForWordform to make sure it finds matches regardless of case.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void FindEntryNotMatchingCase()
		{
			// Setup
			var servLoc = Cache.ServiceLocator;
			var langProj = Cache.LangProject;
			var lexDb = langProj.LexDbOA;
			// Create a WfiWordform with some string.
			// We need this wordform to get a real hvo, flid, and ws.
			// Make Spanish be the vern ws.
			var spanish = servLoc.WritingSystemManager.Get("es");
			langProj.AddToCurrentVernacularWritingSystems(spanish);
			langProj.CurAnalysisWss = "en";
			langProj.DefaultVernacularWritingSystem = spanish;
			var defVernWs = spanish.Handle;
			var entry1 = servLoc.GetInstance<ILexEntryFactory>().Create(
				"Uppercaseword", "Uppercasegloss", new SandboxGenericMSA());
			var entry2 = servLoc.GetInstance<ILexEntryFactory>().Create(
				"lowercaseword", "lowercasegloss", new SandboxGenericMSA());

			// SUT
			// First make sure it works with the same case
			using (var lexEntryUi = LexEntryUi.FindEntryForWordform(Cache,
				TsStringUtils.MakeString("Uppercaseword", Cache.DefaultVernWs)))
			{
				Assert.IsNotNull(lexEntryUi);
				Assert.AreEqual(entry1.Hvo, lexEntryUi.Object.Hvo, "Found wrong object");
			}
			using (var lexEntryUi = LexEntryUi.FindEntryForWordform(Cache,
				TsStringUtils.MakeString("lowercaseword", Cache.DefaultVernWs)))
			{
				Assert.IsNotNull(lexEntryUi);
				Assert.AreEqual(entry2.Hvo, lexEntryUi.Object.Hvo, "Found wrong object");
			}
			// Now make sure it works with the wrong case
			using (var lexEntryUi = LexEntryUi.FindEntryForWordform(Cache,
				TsStringUtils.MakeString("uppercaseword", Cache.DefaultVernWs)))
			{
				Assert.IsNotNull(lexEntryUi);
				Assert.AreEqual(entry1.Hvo, lexEntryUi.Object.Hvo, "Found wrong object");
			}
			using (var lexEntryUi = LexEntryUi.FindEntryForWordform(Cache,
				TsStringUtils.MakeString("LowerCASEword", Cache.DefaultVernWs)))
			{
				Assert.IsNotNull(lexEntryUi);
				Assert.AreEqual(entry2.Hvo, lexEntryUi.Object.Hvo, "Found wrong object");
			}
		}
	}
}