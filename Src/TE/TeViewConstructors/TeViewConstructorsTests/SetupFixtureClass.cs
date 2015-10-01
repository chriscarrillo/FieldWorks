// Copyright (c) 2010-2013 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using Microsoft.Win32;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using SIL.FieldWorks.Common.Framework;
using SIL.FieldWorks.Common.FwUtils;
using SIL.Utils;

namespace SIL.FieldWorks.TE
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Helper class that contains methods that will be run before/after all other tests and
	/// fixture setups/teardowns are run.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	[SetUpFixture]
	[SuppressMessage("Gendarme.Rules.Design", "TypesWithDisposableFieldsShouldBeDisposableRule",
		Justification="Unit test - m_RegistryKey gets disposed in TearDown() method")]
	public class SetupFixtureClass
	{
		private RegistryKey m_RegistryKey;

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// SetUp method that will be run once before any tests or setup methods
		/// </summary>
		///--------------------------------------------------------------------------------------
		[SetUp]
		public void SetUp()
		{
			RegistryHelper.CompanyName = "SIL";
			RegistryHelper.ProductName = "FieldWorks";

			FwRegistrySettings.Init();
			m_RegistryKey = RegistryHelper.SettingsKey(FwSubKey.TE, "Dummy");
			TeProjectSettings.InitSettings(m_RegistryKey);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Teardown method that will be run once after any tests or setup methods
		/// </summary>
		///--------------------------------------------------------------------------------------
		[TearDown]
		public void TearDown()
		{
			TeProjectSettings.Release();
			FwRegistrySettings.Release();
			ReflectionHelper.CallStaticMethod("FwResources.dll",
				"SIL.FieldWorks.Resources.ResourceHelper", "ShutdownHelper");
			if (m_RegistryKey != null)
				m_RegistryKey.Dispose();
			m_RegistryKey = null;
		}
	}
}