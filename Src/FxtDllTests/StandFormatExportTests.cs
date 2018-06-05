// Copyright (c) 2003-2013 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)
//-------------------------------------------------------------------------------
using System;
using System.IO;
using SIL.FieldWorks.Common.FwUtils;
using NUnit.Framework;
using SIL.LCModel.Utils;

namespace SIL.FieldWorks.Common.FXT
{
	/// <summary>
	/// Test SFM export
	/// </summary>
	[TestFixture]
	public class StandardFormat : FxtTestBase
	{
		/// <summary>
		/// Location of simple test FXT files
		/// </summary>
		protected string m_testDir;

		public override void FixtureSetup()
		{
			base.FixtureSetup();

			m_testDir = Path.Combine(FwDirectoryFinder.FlexFolder, "Export Templates");
		}

		[Test]
		[Ignore("TestLangProj export tests need upgrading.")]
		public void MDF()
		{
			string sFxtPath = Path.Combine(m_testDir, "mdf.xml");
			string sAnswerFile = Path.Combine(m_sExpectedResultsPath, "TLPStandardFormatMDF.sfm");
			DoDump("TestLangProj", "MDF", sFxtPath, sAnswerFile);
		}
		[Test]
		[Ignore("TestLangProj export tests need upgrading.")]
		public void RootBasedMDF()
		{
			string sFxtPath = Path.Combine(m_testDir, "RootBasedMDF.xml");
			string sAnswerFile = Path.Combine(m_sExpectedResultsPath, "TLPRootBasedMDF.sfm");
			DoDump("TestLangProj", "RootBasedMDF", sFxtPath, sAnswerFile);
		}
		[Test]
		[Ignore("TestLangProj export tests need upgrading.")]
		public void TwoTimesSpeedTest()
		{
			string sFxtPath = Path.Combine(m_testDir, "mdf.xml");
			XDumper dumper = PrepareDumper("TestLangProj",sFxtPath, false);
			PerformDump(dumper, @"C:\first.txt", "TestLangProj", "first");
			PerformDump(dumper, @"C:\second.txt", "TestLangProj", "second");
			string sAnswerFile = Path.Combine(m_sExpectedResultsPath, "TLPStandardFormatMDF.sfm");
			CheckFilesEqual(sAnswerFile, @"C:\first.txt");
			CheckFilesEqual(@"C:\first.txt", @"C:\second.txt");
		}

		/// <summary>
		/// The expected results were generated by applying an Anywhere filter for "bo" on Headword in Sena3/Lexicon Edit, and
		/// exporting the default Configured Dictionary from the Dictionary tool. The final sfm (.db) file was hand edited, so that
		/// the abandoned fs feature information appears after the \ps [PartOfSpeech] on the preceeding line
		/// (e.g. "\ps N 5/6" for "bokho").
		///
		/// The test transforms the second phase of the configured dictionary export from .xml to .sfm,
		/// and checks the resulting transform against the hand edited sfm.
		///
		/// The expected results may need to be updated whenever the ConfiguredSfm.xsl is modified.
		/// And they HAVE been updated to include a couple of Custom fields in the bodzi-bodzi entry.
		/// </summary>
		[Test]
		public void ConfiguredDictionary_FsFeatStruc_LT5655()
		{
			string sXmlPhase2 = Path.Combine(m_sExpectedResultsPath, "Phase2-Sena3-bo-ConfiguredDictionary.xml");
			string sXsltSfm = Path.Combine(m_testDir, "ConfiguredSfm.xsl");
			string sAnswerFile = Path.Combine(m_sExpectedResultsPath, "Sena3-bo-ConfiguredDictionary.sfm");
			string sOutputFile = FileUtils.GetTempFile("sfm");
			PerformTransform(sXsltSfm, sXmlPhase2, sOutputFile);
			CheckFilesEqual(sAnswerFile, sOutputFile);
		}

		public void CheckFilesEqual(string sAnswerPath, string outputPath)
		{
			using (StreamReader test = new StreamReader(outputPath))
			using (StreamReader control = new StreamReader(sAnswerPath))
			{
				string testResult = test.ReadToEnd();
				string expected = control.ReadToEnd();
				if (Environment.OSVersion.Platform == PlatformID.Unix)
				{
					// The xslt processor on linux inserts a BOM at the beginning, and writes \r\n for newlines.
					int iBegin = testResult.IndexOf("\\lx ");
					if (iBegin > 0 && iBegin < 6)
						testResult = testResult.Substring(iBegin);
					testResult = testResult.Replace("\r\n", "\n");
				}
				Assert.AreEqual(expected, testResult,
					"FXT Output Differs. If you have done a model change, you can update the 'correct answer' xml files by runing fw\\bin\\FxtAnswersUpdate.bat.");
			}
		}

	}
}
