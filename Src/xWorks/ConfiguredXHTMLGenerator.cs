﻿// Copyright (c) 2014 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using SIL.CoreImpl;
using SIL.FieldWorks.Common.COMInterfaces;
using SIL.FieldWorks.Common.Controls;
using SIL.FieldWorks.FDO;
using SIL.FieldWorks.FDO.DomainServices;
using SIL.Utils;
using XCore;

namespace SIL.FieldWorks.XWorks
{
	/// <summary>
	/// This class groups the static methods used for generating XHTML, according to specified configurations, from Fieldworks model objects
	/// </summary>
	public static class ConfiguredXHTMLGenerator
	{
		/// <summary>
		/// The Assembly that the model Types should be loaded from. Allows test code to introduce a test model.
		/// </summary>
		internal static string AssemblyFile { get; set; }

		/// <summary>
		/// Map of the Assembly to the file name, so that different tests can use different models
		/// </summary>
		internal static Dictionary<string, Assembly> AssemblyMap = new Dictionary<string, Assembly>();

		private const string PublicIdentifier = @"-//W3C//DTD XHTML 1.1//EN";

		/// <summary>
		/// Static initializer setting the AssemblyFile to the default Fieldworks model dll.
		/// </summary>
		static ConfiguredXHTMLGenerator()
		{
			AssemblyFile = "FDO";
		}

		/// <summary>
		/// Generates self-contained XHTML for a single entry for, eg, the preview panes in Lexicon Edit and the Dictionary Config dialog
		/// </summary>
		/// <returns>The HTML as a string</returns>
		public static string GenerateEntryHtmlWithStyles(ICmObject entry, DictionaryConfigurationModel configuration,
																		 DictionaryPublicationDecorator pubDecorator, Mediator mediator)
		{
			if(entry == null)
			{
				throw new ArgumentNullException("entry");
			}
			if(pubDecorator == null)
			{
				throw new ArgumentException("pubDecorator");
			}
			var projectPath = Path.Combine(FdoFileHelper.GetConfigSettingsDir(entry.Cache.ProjectId.ProjectFolder),
													 DictionaryConfigurationListener.GetDictionaryConfigurationType(mediator));
			var previewCssPath = Path.Combine(projectPath, "Preview.css");
			var stringBuilder = new StringBuilder();
			using(var writer = XmlWriter.Create(stringBuilder))
			using(var cssWriter = new StreamWriter(previewCssPath, false))
			{
				var exportSettings = new GeneratorSettings((FdoCache)mediator.PropertyTable.GetValue("cache"), writer, false, false, null);
				GenerateOpeningHtml(previewCssPath, exportSettings);
				GenerateXHTMLForEntry(entry, configuration, pubDecorator, exportSettings);
				GenerateClosingHtml(writer);
				writer.Flush();
				cssWriter.Write(CssGenerator.GenerateCssFromConfiguration(configuration, mediator));
				cssWriter.Flush();
			}

			return stringBuilder.ToString();
		}

		private static void GenerateOpeningHtml(string cssPath, GeneratorSettings exportSettings)
		{
			var xhtmlWriter = exportSettings.Writer;

			xhtmlWriter.WriteDocType("html", PublicIdentifier, null, null);
			xhtmlWriter.WriteStartElement("html", "http://www.w3.org/1999/xhtml");
			xhtmlWriter.WriteAttributeString("lang", "utf-8");
			xhtmlWriter.WriteStartElement("head");
			xhtmlWriter.WriteStartElement("link");
			xhtmlWriter.WriteAttributeString("href", "file:///" + cssPath);
			xhtmlWriter.WriteAttributeString("rel", "stylesheet");
			xhtmlWriter.WriteEndElement(); //</link>
			// write out schema links for writing system metadata
			xhtmlWriter.WriteStartElement("link");
			xhtmlWriter.WriteAttributeString("href", "http://purl.org/dc/terms/");
			xhtmlWriter.WriteAttributeString("rel", "schema.DCTERMS");
			xhtmlWriter.WriteEndElement(); //</link>
			xhtmlWriter.WriteStartElement("link");
			xhtmlWriter.WriteAttributeString("href", "http://purl.org/dc/elements/1.1/");
			xhtmlWriter.WriteAttributeString("rel", "schema.DC");
			xhtmlWriter.WriteEndElement(); //</link>
			GenerateWritingSystemsMetadata(exportSettings);
			xhtmlWriter.WriteEndElement(); //</head>
			xhtmlWriter.WriteStartElement("body");
		}

		private static void GenerateWritingSystemsMetadata(GeneratorSettings exportSettings)
		{
			var xhtmlWriter = exportSettings.Writer;
			var lp = exportSettings.Cache.LangProject;
			var wsList = lp.CurrentAnalysisWritingSystems.Union(lp.CurrentVernacularWritingSystems.Union(lp.CurrentPronunciationWritingSystems));
			foreach(var ws in wsList)
			{
				xhtmlWriter.WriteStartElement("meta");
				xhtmlWriter.WriteAttributeString("name", "DC.language");
				xhtmlWriter.WriteAttributeString("content", String.Format("{0}:{1}", ws.RFC5646, ws.LanguageName));
				xhtmlWriter.WriteAttributeString("scheme", "DCTERMS.RFC5646");
				xhtmlWriter.WriteEndElement();
				xhtmlWriter.WriteStartElement("meta");
				xhtmlWriter.WriteAttributeString("name", ws.RFC5646);
				xhtmlWriter.WriteAttributeString("content", ws.DefaultFontName);
				xhtmlWriter.WriteAttributeString("scheme", "language to font");
				xhtmlWriter.WriteEndElement();
			}
		}

		private static void GenerateClosingHtml(XmlWriter xhtmlWriter)
		{
			xhtmlWriter.WriteEndElement(); //</body>
			xhtmlWriter.WriteEndElement(); //</html>
		}

		/// <summary>
		/// Saves the generated content into the given xhtml and css file paths for all the entries in
		/// the given collection.
		/// </summary>
		public static void SavePublishedHtmlWithStyles(IEnumerable<int> entryHvos, DictionaryPublicationDecorator publicationDecorator, DictionaryConfigurationModel configuration, Mediator mediator, string xhtmlPath, string cssPath, IThreadedProgress progress = null)
		{
			var cache = (FdoCache)mediator.PropertyTable.GetValue("cache");
			using(var xhtmlWriter = XmlWriter.Create(xhtmlPath))
			using(var cssWriter = new StreamWriter(cssPath, false))
			{
				var settings = new GeneratorSettings(cache, xhtmlWriter, true, true, Path.GetDirectoryName(xhtmlPath));
				GenerateOpeningHtml(cssPath, settings);
				string lastHeader = null;
				foreach(var hvo in entryHvos)
				{
					var entry = cache.ServiceLocator.GetObject(hvo);
					// TODO pH 2014.08: generate only if entry is published (confignode enabled, pubAsMinor, selected complex- or variant-form type)
					GenerateLetterHeaderIfNeeded(entry, ref lastHeader, xhtmlWriter, cache);
					GenerateXHTMLForEntry(entry, configuration, publicationDecorator, settings);
					if(progress != null)
					{
						progress.Position++;
					}
				}
				GenerateClosingHtml(xhtmlWriter);
				xhtmlWriter.Flush();
				cssWriter.Write(CssGenerator.GenerateLetterHeaderCss(mediator));
				cssWriter.Write(CssGenerator.GenerateCssFromConfiguration(configuration, mediator));
				cssWriter.Flush();
			}
		}

		internal static void GenerateLetterHeaderIfNeeded(ICmObject entry, ref string lastHeader, XmlWriter xhtmlWriter, FdoCache cache)
		{
			var lexEntry = entry as ILexEntry;
			// If performance is an issue these dummy's can be stored between calls
			var dummyOne = new Dictionary<string, Set<string>>();
			var dummyTwo = new Dictionary<string, Dictionary<string, string>>();
			var dummyThree = new Dictionary<string, Set<string>>();
			var wsString = cache.WritingSystemFactory.GetStrFromWs(cache.DefaultVernWs);
			var firstLetter = ConfiguredExport.GetLeadChar(lexEntry.HeadWord.Text, wsString,
																		  dummyOne, dummyTwo, dummyThree, cache);
			if(firstLetter != lastHeader && !String.IsNullOrEmpty(firstLetter))
			{
				var headerTextBuilder = new StringBuilder();
				headerTextBuilder.Append(Icu.ToTitle(firstLetter, wsString));
				headerTextBuilder.Append(' ');
				headerTextBuilder.Append(firstLetter.Normalize());

				xhtmlWriter.WriteStartElement("div");
				xhtmlWriter.WriteAttributeString("class", "letHead");
				xhtmlWriter.WriteStartElement("div");
				xhtmlWriter.WriteAttributeString("class", "letter");
				xhtmlWriter.WriteString(headerTextBuilder.ToString());
				xhtmlWriter.WriteEndElement();
				xhtmlWriter.WriteEndElement();

				lastHeader = firstLetter;
			}
		}

		/// <summary>
		/// Generating the xhtml representation for the given ICmObject using the given configuration to select which data to write out
		/// If it is a Dictionary Main Entry or non-Dictionary entry, uses the first configuration.
		/// If it is a Minor Entry, first checks whether the entry should be published as a Minor Entry; then, generates XHTML for each applicable
		/// Minor Entry configuration.
		/// </summary>
		public static void GenerateXHTMLForEntry(ICmObject entry, DictionaryConfigurationModel configuration,
			DictionaryPublicationDecorator publicationDecorator, GeneratorSettings settings)
		{
				if (IsMinorEntry(entry))
				{
					if(((ILexEntry)entry).PublishAsMinorEntry)
					{
						for(var i = 1; i < configuration.Parts.Count; i++)
						{
							if(IsListItemSelectedForExport(configuration.Parts[i], entry))
							{
								GenerateXHTMLForEntry(entry, configuration.Parts[i], publicationDecorator, settings);
							}
						}
					}
				}
				else
				{
					GenerateXHTMLForEntry(entry, configuration.Parts[0], publicationDecorator, settings);
				}
		}

		internal static bool IsMinorEntry(ICmObject entry)
		{
			// owning an ILexEntryRef denotes a minor entry (Complex* or Variant Form)
			return entry is ILexEntry && ((ILexEntry)entry).EntryRefsOS.Any();
			// TODO pH 2014.08: *Owning a LexEntryRef denotes a minor entry only in those configs that display complex forms as subentries
			// TODO				(Root, Bart?, and their descendants) or if the reftype is Variant Form
		}

		/// <summary>Generates XHTML for an ICmObject for a specific ConfigurableDictionaryNode</summary>
		/// <param name="configuration"><remarks>this configuration node must match the entry type</remarks></param>
		internal static void GenerateXHTMLForEntry(ICmObject entry, ConfigurableDictionaryNode configuration, DictionaryPublicationDecorator publicationDecorator, GeneratorSettings settings)
		{
			if(settings == null || entry == null || configuration == null)
			{
				throw new ArgumentNullException();
			}
			if(String.IsNullOrEmpty(configuration.FieldDescription))
			{
				throw new ArgumentException(@"Invalid configuration: FieldDescription can not be null", @"configuration");
			}
			if(entry.ClassID != settings.Cache.MetaDataCacheAccessor.GetClassId(configuration.FieldDescription))
			{
				throw new ArgumentException(@"The given argument doesn't configure this type", @"configuration");
			}
			if(!configuration.IsEnabled)
			{
				return;
			}

			settings.Writer.WriteStartElement("div");
			WriteClassNameAttribute(settings.Writer, configuration);
			settings.Writer.WriteAttributeString("id", "hvo" + entry.Hvo);
			foreach(var config in configuration.Children)
			{
				GenerateXHTMLForFieldByReflection(entry, config, publicationDecorator, settings);
			}
			settings.Writer.WriteEndElement(); // </div>
		}

		/// <summary>
		/// This method will write out the class name attribute into the xhtml for the given configuration node
		/// taking into account the current information in ClassNameOverrides
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="configNode">used to look up any mapping overrides</param>
		private static void WriteClassNameAttribute(XmlWriter writer, ConfigurableDictionaryNode configNode)
		{
			writer.WriteAttributeString("class", CssGenerator.GetClassAttributeForConfig(configNode));
		}

		/// <summary>
		/// This method will use reflection to pull data out of the given object based on the given configuration and
		/// write out appropriate XHTML.
		/// </summary>
		private static void GenerateXHTMLForFieldByReflection(object field, ConfigurableDictionaryNode config, DictionaryPublicationDecorator publicationDecorator, GeneratorSettings settings)
		{
			if(!config.IsEnabled)
			{
				return;
			}
			var cache = settings.Cache;
			var entryType = field.GetType();
			object propertyValue = null;
			if(config.IsCustomField)
			{
				var customFieldOwnerClassName = GetClassNameForCustomFieldParent(config);
				int customFieldFlid;
				try
				{
					customFieldFlid = cache.MetaDataCacheAccessor.GetFieldId(customFieldOwnerClassName, config.FieldDescription, false);
				}
				catch(FDOInvalidFieldException)
				{
					var usefulMessage = String.Format("The custom field {0} could not be found in the class {1} for the node labelled {2}",
						config.FieldDescription, customFieldOwnerClassName, config.Parent.Label);
					throw new ArgumentException(usefulMessage, "config");
				}
				if(customFieldFlid != 0)
				{
					var customFieldType = cache.MetaDataCacheAccessor.GetFieldType(customFieldFlid);
					switch(customFieldType)
					{
						case (int)CellarPropertyType.ReferenceSequence:
						case (int)CellarPropertyType.OwningSequence:
						{
							var sda = cache.MainCacheAccessor;
							// This method returns the hvo of the object pointed to
							var chvo = sda.get_VecSize(((ICmObject)field).Hvo, customFieldFlid);
							int[] contents;
							using(var arrayPtr = MarshalEx.ArrayToNative<int>(chvo))
							{
								sda.VecProp(((ICmObject)field).Hvo, customFieldFlid, chvo, out chvo, arrayPtr);
								contents = MarshalEx.NativeToArray<int>(arrayPtr, chvo);
							}
							// if the hvo is invalid set propertyValue to null otherwise get the object
							propertyValue = contents.Select(id => cache.LangProject.Services.GetObject(id));
							break;
						}
						case (int)CellarPropertyType.ReferenceAtomic:
						case (int)CellarPropertyType.OwningAtomic:
						{
							// This method returns the hvo of the object pointed to
							propertyValue = cache.MainCacheAccessor.get_ObjectProp(((ICmObject)field).Hvo, customFieldFlid);
							// if the hvo is invalid set propertyValue to null otherwise get the object
							propertyValue = (int)propertyValue > 0 ? cache.LangProject.Services.GetObject((int)propertyValue) : null;
							break;
						}
						case (int)CellarPropertyType.Time:
						{
							propertyValue = SilTime.ConvertFromSilTime(cache.MainCacheAccessor.get_TimeProp(((ICmObject)field).Hvo, customFieldFlid));
							break;
						}
						case (int)CellarPropertyType.MultiUnicode:
						case (int)CellarPropertyType.MultiString:
						{
							propertyValue = cache.MainCacheAccessor.get_MultiStringProp(((ICmObject)field).Hvo, customFieldFlid);
							break;
						}
						case (int)CellarPropertyType.String:
						{
							propertyValue = cache.MainCacheAccessor.get_StringProp(((ICmObject)field).Hvo, customFieldFlid);
							break;
						}
					}
				}
			}
			else
			{
				var property = entryType.GetProperty(config.FieldDescription);
				if(property == null)
				{
					Debug.WriteLine("Issue with finding {0}", (object)config.FieldDescription);
					return;
				}
				propertyValue = property.GetValue(field, new object[] { });
			}
			// If the property value is null there is nothing to generate
			if(propertyValue == null)
			{
				return;
			}
			if(!String.IsNullOrEmpty(config.SubField))
			{
				var subType = propertyValue.GetType();
				var subProp = subType.GetProperty(config.SubField);
				propertyValue = subProp.GetValue(propertyValue, new object[] { });
			}
			var typeForNode = config.IsCustomField
										? GetPropertyTypeFromReflectedTypes(propertyValue.GetType(), null)
										: GetPropertyTypeForConfigurationNode(config);
			switch(typeForNode)
			{
				case(PropertyType.CollectionType):
				{
					if(!IsCollectionEmpty(propertyValue))
					{
						GenerateXHTMLForCollection(propertyValue, config, publicationDecorator, settings);
					}
					return;
				}
				case(PropertyType.MoFormType):
				{
					GenerateXHTMLForMoForm(propertyValue as IMoForm, config, settings);
					return;
				}
				case(PropertyType.CmObjectType):
				{
					GenerateXHTMLForICmObject(propertyValue as ICmObject, config, settings);
					return;
				}
				case (PropertyType.CmPictureType):
				{
					var fileProperty = propertyValue as ICmFile;
					if(fileProperty != null)
					{
						GenerateXHTMLForPicture(fileProperty, config, settings);
					}
					else
					{
						GenerateXHTMLForPictureCaption(propertyValue, config, settings);
					}
					return;
				}
				case(PropertyType.CmPossibility):
				{
					GenerateXHTMLForPossibility(propertyValue, config, publicationDecorator, settings);
					return;
				}
				default:
				{
					GenerateXHTMLForValue(field, propertyValue, config, settings);
					break;
				}
			}

			if(config.Children != null)
			{
				foreach(var child in config.Children)
				{
					GenerateXHTMLForFieldByReflection(propertyValue, child, publicationDecorator, settings);
				}
			}
		}

		/// <summary>
		/// This method will return the string representing the class name for the parent
		/// node of a configuration item representing a custom field.
		/// </summary>
		private static string GetClassNameForCustomFieldParent(ConfigurableDictionaryNode customFieldNode)
		{
			Type unneeded;
			// If the parent node of the custom field represents a collection, calling GetTypeForConfigurationNode
			// with the parent node returns the collection type. We want the type of the elements in the collection.
			var parentNodeType = GetTypeForConfigurationNode(customFieldNode.Parent, out unneeded);
			if(IsCollectionType(parentNodeType))
			{
				parentNodeType = parentNodeType.GetGenericArguments()[0];
			}
			if(parentNodeType.IsInterface)
			{
				// Strip off the interface designation since custom fields are added to concrete classes
				return parentNodeType.Name.Substring(1);
			}
			return parentNodeType.Name;
		}

		private static void GenerateXHTMLForPossibility(object propertyValue,ConfigurableDictionaryNode config,
			DictionaryPublicationDecorator publicationDecorator, GeneratorSettings settings)
		{
			var writer = settings.Writer;
			if(config.Children.Any(node => node.IsEnabled))
			{
				writer.WriteStartElement("span");
				writer.WriteAttributeString("class", CssGenerator.GetClassAttributeForConfig(config));
				if(config.Children != null)
				{
					foreach(var child in config.Children)
					{
						GenerateXHTMLForFieldByReflection(propertyValue, child, publicationDecorator, settings);
					}
				}
				writer.WriteEndElement();
			}
		}

		private static void GenerateXHTMLForPictureCaption(object propertyValue, ConfigurableDictionaryNode config, GeneratorSettings settings)
		{
			var writer = settings.Writer;
			var cache = settings.Cache;
			writer.WriteStartElement("div");
			writer.WriteAttributeString("class", CssGenerator.GetClassAttributeForConfig(config));
			// todo: get sense numbers and captions into the same div and get rid of this if else
			if(config.DictionaryNodeOptions != null)
			{
				GenerateXHTMLForStrings(propertyValue as IMultiString, config, settings);
			}
			else
			{
				GenerateXHTMLForString(propertyValue as ITsString, config, settings);
			}
			writer.WriteEndElement();
		}

		private static void GenerateXHTMLForPicture(ICmFile pictureFile, ConfigurableDictionaryNode config, GeneratorSettings settings)
		{
			var writer = settings.Writer;
			writer.WriteStartElement("img");
			writer.WriteAttributeString("class", CssGenerator.GetClassAttributeForConfig(config));
			var srcAttribute = GenerateSrcAttributeFromFilePath(pictureFile, settings.UseRelativePaths ? "pictures" : null, settings);
			writer.WriteAttributeString("src", srcAttribute);
			writer.WriteAttributeString("id", "hvo" + pictureFile.Hvo);
			writer.WriteEndElement();
		}

		/// <summary>
		/// This method will generate a src attribute which will point to the given file from the xhtml.
		/// </summary>
		/// <para name="subfolder">If not null the path generated will be a relative path with the file in subfolder</para>
		private static string GenerateSrcAttributeFromFilePath(ICmFile file, string subFolder, GeneratorSettings settings)
		{
			string filePath;
			if(settings.UseRelativePaths && subFolder != null)
			{
				filePath = Path.Combine(subFolder, Path.GetFileName(MakeSafeFilePath(file.InternalPath)));
				if(settings.CopyFiles)
				{
					FileUtils.EnsureDirectoryExists(Path.Combine(settings.ExportPath, subFolder));
					var destination = Path.Combine(settings.ExportPath, filePath);
					var source = MakeSafeFilePath(file.AbsoluteInternalPath);
					if(!File.Exists(destination))
					{
						if(File.Exists(source))
						{
							FileUtils.Copy(source, destination);
						}
					}
					else if(!FileUtils.AreFilesIdentical(source, destination))
					{
						var fileWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
						var fileExtension = Path.GetExtension(filePath);
						var copyNumber = 0;
						do
						{
							++copyNumber;
							destination = Path.Combine(settings.ExportPath, subFolder, String.Format("{0}{1}{2}", fileWithoutExtension, copyNumber, fileExtension));
						}
						while(File.Exists(destination));
						if(File.Exists(source))
						{
							FileUtils.Copy(source, destination);
						}
						// Change the filepath to point to the copied file
						filePath = Path.Combine(subFolder, String.Format("{0}{1}{2}", fileWithoutExtension, copyNumber, fileExtension));
					}
				}
			}
			else
			{
				filePath = MakeSafeFilePath(file.AbsoluteInternalPath);
			}
			return settings.UseRelativePaths ? filePath : new Uri(filePath).ToString();
		}

		private static string MakeSafeFilePath(string filePath)
		{
			if(Unicode.CheckForNonAsciiCharacters(filePath))
			{
				// Flex keeps the filename as NFD in memory because it is unicode. We need NFC to actually link to the file
				filePath = Icu.Normalize(filePath, Icu.UNormalizationMode.UNORM_NFC);
			}
			return filePath;
		}

		internal enum PropertyType
		{
			CollectionType,
			MoFormType,
			CmObjectType,
			CmPictureType,
			CmFileType,
			CmPossibility,
			PrimitiveType,
			InvalidProperty
		}

		private static Dictionary<ConfigurableDictionaryNode, PropertyType> _configNodeToTypeMap = new Dictionary<ConfigurableDictionaryNode, PropertyType>();

		/// <summary>
		/// This method will reflectively return the type that represents the given configuration node as
		/// described by the ancestry and FieldDescription and SubField properties of each node in it.
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		internal static PropertyType GetPropertyTypeForConfigurationNode(ConfigurableDictionaryNode config)
		{
			Type parentType;
			var fieldType = GetTypeForConfigurationNode(config, out parentType);
			return GetPropertyTypeFromReflectedTypes(fieldType, parentType);
		}

		private static PropertyType GetPropertyTypeFromReflectedTypes(Type fieldType, Type parentType)
		{
			if(fieldType == null)
			{
				return PropertyType.InvalidProperty;
			}
			if(IsCollectionType(fieldType))
			{
				return PropertyType.CollectionType;
			}
			if(typeof(ICmPicture).IsAssignableFrom(parentType))
			{
				return PropertyType.CmPictureType;
			}
			if(typeof(ICmFile).IsAssignableFrom(fieldType))
			{
				return PropertyType.CmFileType;
			}
			if(typeof(IMoForm).IsAssignableFrom(fieldType))
			{
				return PropertyType.MoFormType;
			}
			if(typeof(ICmPossibility).IsAssignableFrom(fieldType))
			{
				return PropertyType.CmPossibility;
			}
			if(typeof(ICmObject).IsAssignableFrom(fieldType))
			{
				return PropertyType.CmObjectType;
			}
			return PropertyType.PrimitiveType;
		}

		/// <summary>
		/// This method will return the Type that represents the data in the given configuration node.
		/// </summary>
		/// <param name="config">This node and it's lineage will be used to find the type</param>
		/// <param name="parentType">This will be set to the type of the parent of config which is sometimes useful to the callers</param>
		/// <returns></returns>
		internal static Type GetTypeForConfigurationNode(ConfigurableDictionaryNode config, out Type parentType)
		{
			if(config == null)
			{
				throw new ArgumentNullException("config", "The configuration node must not be null.");
			}
			parentType = null;
			var lineage = new Stack<ConfigurableDictionaryNode>();
			// Build a list of the direct line up to the top of the configuration
			lineage.Push(config);
			var next = config;
			while(next.Parent != null)
			{
				next = next.Parent;
				lineage.Push(next);
			}
			// pop off the root configuration and read the FieldDescription property to get our starting point
			var assembly = GetAssemblyForFile(AssemblyFile);
			var rootNode = lineage.Pop();
			var lookupType = assembly.GetType(rootNode.FieldDescription);
			if(lookupType == null) // If the FieldDescription didn't load prepend the default model namespace and try again
			{
				lookupType = assembly.GetType("SIL.FieldWorks.FDO.DomainImpl." + rootNode.FieldDescription);
			}
			if(lookupType == null)
			{
				throw new ArgumentException(String.Format(xWorksStrings.InvalidRootConfigurationNode, rootNode.FieldDescription));
			}
			var fieldType = lookupType;

			// Traverse the configuration reflectively inspecting the types in parent to child order
			foreach(var node in lineage)
			{
				var property = GetProperty(lookupType, node);
				if(property != null)
				{
					fieldType = property.PropertyType;
				}
				else
				{
					return null;
				}
				if(IsCollectionType(fieldType))
				{
					// When a node points to a collection all the child nodes operate on individual items in the
					// collection, so look them up in the type that the collection contains. e.g. IEnumerable<ILexEntry>
					// gives ILexEntry and IFdoVector<ICmObject> gives ICmObject
					lookupType = fieldType.GetGenericArguments()[0];
				}
				else
				{
					parentType = lookupType;
					lookupType = fieldType;
				}
			}
			return fieldType;
		}

		/// <summary>
		/// Loading an assembly is expensive so we cache the assembly once it has been loaded
		/// for enahanced performance.
		/// </summary>
		private static Assembly GetAssemblyForFile(string assemblyFile)
		{
			if(!AssemblyMap.ContainsKey(assemblyFile))
			{
				AssemblyMap[assemblyFile] = Assembly.Load(AssemblyFile);
			}
			return AssemblyMap[assemblyFile];
		}

		/// <summary>
		/// Return the property info from a given class and node. Will check interface heirarchy for the property
		/// if <code>lookupType</code> is an interface.
		/// </summary>
		/// <param name="lookupType"></param>
		/// <param name="node"></param>
		/// <returns></returns>
		private static PropertyInfo GetProperty(Type lookupType, ConfigurableDictionaryNode node)
		{
			string propertyOfInterest;
			PropertyInfo propInfo;
			var typesToCheck = new Stack<Type>();
			typesToCheck.Push(lookupType);
			do
			{
				var current = typesToCheck.Pop();
				propertyOfInterest = node.FieldDescription;
				// if there is a SubField we need to use the type of the FieldDescription
				// for the rest of this method so set current to the FieldDescription type.
				if(node.SubField != null)
				{
					var property = current.GetProperty(node.FieldDescription);
					propertyOfInterest = node.SubField;
					if(property != null)
					{
						current = property.PropertyType;
					}
				}
				propInfo = current.GetProperty(propertyOfInterest, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if(propInfo == null)
				{
					foreach(var i in current.GetInterfaces())
					{
						typesToCheck.Push(i);
					}
				}
			} while(propInfo == null && typesToCheck.Count > 0);
			return propInfo;
		}

		private static void GenerateXHTMLForMoForm(IMoForm moForm, ConfigurableDictionaryNode config, GeneratorSettings settings)
		{
			// Don't export if there is no such data
			if(moForm == null)
				return;
			GenerateXHTMLForStrings(moForm.Form, config, settings);
			if(config.Children != null && config.Children.Any())
			{
				throw new NotImplementedException("Children for MoForm types not yet supported.");
			}
		}

		/// <summary>
		/// This method will generate the XHTML that represents a collection and its contents
		/// </summary>
		/// <param name="collectionField"></param>
		/// <param name="config"></param>
		/// <param name="publicationDecorator"></param>
		/// <param name="writer"></param>
		/// <param name="cache"></param>
		private static void GenerateXHTMLForCollection(object collectionField, ConfigurableDictionaryNode config, DictionaryPublicationDecorator publicationDecorator, GeneratorSettings settings)
		{
			var writer = settings.Writer;
			writer.WriteStartElement("span");
			WriteClassNameAttribute(writer, config);
			IEnumerable collection;
			if(collectionField is IEnumerable)
			{
				collection = collectionField as IEnumerable;
			}
			else if(collectionField is IFdoVector)
			{
				collection = (collectionField as IFdoVector).Objects;
			}
			else
			{
				throw new ArgumentException("The given field is not a recognized collection");
			}
			var isSingle = collection.Cast<object>().Count() == 1;
			foreach(var item in collection)
			{
				GenerateCollectionItemContent(config, publicationDecorator, item, isSingle, settings);
			}
			writer.WriteEndElement();
		}

		private static void GenerateCollectionItemContent(ConfigurableDictionaryNode config, DictionaryPublicationDecorator publicationDecorator,
			object item, bool isSingle, GeneratorSettings settings)
		{
			var writer = settings.Writer;
			if (config.DictionaryNodeOptions is DictionaryNodeListOptions && !IsListItemSelectedForExport(config, item))
			{
				return;
			}
			// if we are working with senses start wrapping element and write out the sense number sibling item if necessary
			if(config.DictionaryNodeOptions is DictionaryNodeSenseOptions)
			{
				// Wrap the number and sense combination in a sensecontent span so that can both be affected by DisplayEachSenseInParagraph
				writer.WriteStartElement("span");
				writer.WriteAttributeString("class", "sensecontent");
				GenerateSenseNumberSpanIfNeeded(config.DictionaryNodeOptions as DictionaryNodeSenseOptions, writer, item, settings.Cache,
														  publicationDecorator, isSingle);
			}

			writer.WriteStartElement(GetElementNameForProperty(config));
			WriteCollectionItemClassAttribute(config, writer);
			if(config.Children != null)
			{
				foreach(var child in config.Children)
					GenerateXHTMLForFieldByReflection(item, child, publicationDecorator, settings);
			}
			writer.WriteEndElement();

			// close out the sense wrapping
			if(config.DictionaryNodeOptions is DictionaryNodeSenseOptions)
			{
				writer.WriteEndElement();
			}
		}

		private static void GenerateSenseNumberSpanIfNeeded(DictionaryNodeSenseOptions senseOptions, XmlWriter writer,
																			 object sense, FdoCache cache,
																			 DictionaryPublicationDecorator publicationDecorator, bool isSingle)
		{
			if(senseOptions == null || (isSingle && !senseOptions.NumberEvenASingleSense))
				return;
			writer.WriteStartElement("span");
			writer.WriteAttributeString("class", "sensenumber");
			writer.WriteString(cache.GetOutlineNumber((ICmObject)sense, LexSenseTags.kflidSenses, false, true, publicationDecorator));
			writer.WriteEndElement();
		}

		private static void GenerateXHTMLForICmObject(ICmObject propertyValue, ConfigurableDictionaryNode config, GeneratorSettings settings)
		{
			var writer = settings.Writer;
			// Don't export if there is no such data
			if (propertyValue == null)
				return;
			writer.WriteStartElement("span");
			// Rely on configuration to handle adjusting the classname for "RA" or "OA" model properties
			var fieldDescription = CssGenerator.GetClassAttributeForConfig(config);
			writer.WriteAttributeString("class", fieldDescription);
			if (config.Children != null)
			{
				foreach (var child in config.Children)
				{
					if (child.IsEnabled)
					{
						GenerateXHTMLForFieldByReflection(propertyValue, child, null, settings);
					}
				}
			}

			writer.WriteEndElement();
		}

		/// <summary>
		///  Write out the class element to use in the span for the individual items in the collection
		/// </summary>
		/// <param name="config"></param>
		/// <param name="writer"></param>
		private static void WriteCollectionItemClassAttribute(ConfigurableDictionaryNode config, XmlWriter writer)
		{
			var collectionName = CssGenerator.GetClassAttributeForConfig(config);
			// chop the pluralization off the parent class
			writer.WriteAttributeString("class", collectionName.Substring(0, collectionName.Length - 1).ToLower());
		}

		/// <summary>
		/// This method is used to determine if we need to iterate through a property and generate xhtml for each item
		/// </summary>
		internal static bool IsCollectionType(Type entryType)
		{
			// The collections we test here are generic collection types (e.g. IEnumerable<T>). Note: This (and other code) does not work for arrays.
			// We do have at least one collection type with at least two generic arguments; hence `> 0` instead of `== 1`
			return (entryType.GetGenericArguments().Length > 0);
		}

		/// <summary>
		/// Determines if the user has specified that this item should generate content.
		/// <returns><c>true</c> if the user has ticked the list item that applies to this object</returns>
		/// </summary>
		internal static bool IsListItemSelectedForExport(ConfigurableDictionaryNode config, object listItem)
		{
			var listOptions = (DictionaryNodeListOptions)config.DictionaryNodeOptions;
			var selectedListOptions = (from option in listOptions.Options where option.IsEnabled select new Guid(option.Id)).ToList();
			switch(listOptions.ListId)
			{
				case DictionaryNodeListOptions.ListIds.Variant:
				{
					var entryRef = (ILexEntryRef)listItem;
					var entryTypeGuids = entryRef.VariantEntryTypesRS.Select(guid => guid.Guid);
					return entryTypeGuids.Intersect (selectedListOptions).Any();
				}
				case DictionaryNodeListOptions.ListIds.Minor:
				{
					// minor entry list options are a combination of both the variant and complex options
					var entry = (ILexEntry)listItem;
					var variantTypeGuids = new Set<Guid>();
					var complexTypeGuids = new Set<Guid>();
					foreach(var variantRef in entry.VariantEntryRefs)
					{
						variantTypeGuids.AddRange(variantRef.VariantEntryTypesRS.Select(guid => guid.Guid));
					}
					foreach(var complexRef in entry.EntryRefsOS)
					{
						complexTypeGuids.AddRange(complexRef.ComplexEntryTypesRS.Select(guid => guid.Guid));
					}
					var entryTypeGuids = complexTypeGuids.Union(variantTypeGuids);
					return entryTypeGuids.Intersect(selectedListOptions).Any();
				}
				case DictionaryNodeListOptions.ListIds.Complex:
				{
					if(listItem is ILexEntryRef)
					{
						var entryRef = (ILexEntryRef)listItem;
						var entryTypeGuids = entryRef.ComplexEntryTypesRS.Select(guid => guid.Guid);
						if(entryTypeGuids.Intersect(selectedListOptions).Any())
							return true;
					}
					else if(listItem is ILexEntry)
					{
						var entry = (ILexEntry)listItem;
						var entryTypeGuids = entry.EntryRefsOS.Select(guid => guid.Guid);
						if(entryTypeGuids.Intersect(selectedListOptions).Any())
							return true;
					}
					return false;
				}
				case DictionaryNodeListOptions.ListIds.Entry:
				case DictionaryNodeListOptions.ListIds.Sense:
				{
					var entryRef = (ILexReference)listItem;
					var entryTypeGuid = entryRef.OwnerType.Guid;
					return selectedListOptions.Contains(entryTypeGuid);
				}
				default:
				{
					Debug.WriteLine("Unhandled list ID encountered: " + listOptions.ListId);
					return true;
				}
			}
		}

		/// <summary>
		/// Returns true if the given collection is empty (type determined at runtime)
		/// </summary>
		/// <param name="collection"></param>
		/// <exception cref="ArgumentException">if the object given is null, or not a handled collection</exception>
		/// <returns></returns>
		private static bool IsCollectionEmpty(object collection)
		{
			if(collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			if(collection is IEnumerable)
			{
				return !(((IEnumerable)collection).Cast<object>().Any());
			}
			if(collection is IFdoVector)
			{
				return ((IFdoVector)collection).ToHvoArray().Length == 0;
			}
			throw new ArgumentException(@"Cannot test something that isn't a collection", "collection");
		}

		/// <summary>
		/// This method generates XHTML content for a given object
		/// </summary>
		/// <param name="field">This is the object that owns the property, needed to look up writing system info for virtual string fields</param>
		/// <param name="propertyValue">data to generate xhtml for</param>
		/// <param name="config"></param>
		/// <param name="writer"></param>
		/// <param name="cache"></param>
		private static void GenerateXHTMLForValue(object field, object propertyValue, ConfigurableDictionaryNode config, GeneratorSettings settings)
		{
			if(propertyValue is ITsString)
			{
				if(!TsStringUtils.IsNullOrEmpty((ITsString)propertyValue))
				{
					settings.Writer.WriteStartElement("span");
					WriteClassNameAttribute(settings.Writer, config);
					GenerateXHTMLForString((ITsString)propertyValue, config, settings);
					settings.Writer.WriteEndElement();
				}
			}
			else if(propertyValue is IMultiStringAccessor)
			{
				GenerateXHTMLForStrings((IMultiStringAccessor)propertyValue, config, settings);
			}
			else if(propertyValue is int)
			{
				WriteElementContents(propertyValue, config, settings.Writer);
			}
			else if(propertyValue is DateTime)
			{
				WriteElementContents(((DateTime)propertyValue).ToLongDateString(), config, settings.Writer);
			}
			else if(propertyValue is IMultiAccessorBase)
			{
				GenerateXHTMLForVirtualStrings((ICmObject)field, (IMultiAccessorBase)propertyValue, config, settings);
			}
			else if(propertyValue is String)
			{
				var propValueString = (String)propertyValue;
				if(!String.IsNullOrEmpty(propValueString))
				{
					// write out Strings something like: <span class="foo">Bar</span>
					settings.Writer.WriteStartElement("span");
					WriteClassNameAttribute(settings.Writer, config);
					settings.Writer.WriteString(propValueString);
					settings.Writer.WriteEndElement();
				}
			}
			else
			{
				Debug.WriteLine("What do I do with {0}?", (object)propertyValue.GetType().Name);
			}
		}

		private static void WriteElementContents(object propertyValue, ConfigurableDictionaryNode config,
															  XmlWriter writer)
		{
			writer.WriteStartElement(GetElementNameForProperty(config));
			WriteClassNameAttribute(writer, config);
			writer.WriteString(propertyValue.ToString());
			writer.WriteEndElement();
		}

		/// <summary>
		/// This method will generate an XHTML span with a string for each selected writing system in the
		/// DictionaryWritingSystemOptions of the configuration that also has data in the given IMultiStringAccessor
		/// </summary>
		/// <param name="multiStringAccessor"></param>
		/// <param name="config"></param>
		/// <param name="writer"></param>
		/// <param name="cache"></param>
		private static void GenerateXHTMLForStrings(IMultiStringAccessor multiStringAccessor, ConfigurableDictionaryNode config, GeneratorSettings settings)
		{
			var wsOptions = config.DictionaryNodeOptions as DictionaryNodeWritingSystemOptions;
			if(wsOptions == null)
			{
				throw new ArgumentException(@"Configuration nodes for MultiString fields should have WritingSystemOptions", "config");
			}
			foreach(var option in wsOptions.Options)
			{
				if(!option.IsEnabled)
				{
					continue;
				}
				var wsId = WritingSystemServices.GetMagicWsIdFromName(option.Id);
				// The string for the specific wsId in the option, or the best string option in the accessor if the wsId is magic
				ITsString bestString;
				if(wsId == 0)
				{
					// This is not a magic writing system, so grab the user requested string
					wsId = settings.Cache.WritingSystemFactory.GetWsFromStr(option.Id);
					bestString = multiStringAccessor.get_String(wsId);
				}
				else
				{
					// Writing system is magic i.e. 'best vernacular' or 'first pronunciation'
					// use the method in the multi-string to get the right string and set wsId to the used one
					bestString = multiStringAccessor.GetAlternativeOrBestTss(wsId, out wsId);
				}
				GenerateWsPrefixAndString(config, settings, wsOptions, wsId, bestString);
			}
		}

		/// <summary>
		/// This method will generate an XHTML span with a string for each selected writing system in the
		/// DictionaryWritingSystemOptions of the configuration that also has data in the given IMultiAccessorBase
		/// </summary>
		/// <param name="owningObject">The object used to access the virtual property</param>
		/// <param name="multiStringAccessor">Virtual Property Accessor</param>
		/// <param name="config"></param>
		/// <param name="writer"></param>
		/// <param name="cache"></param>
		private static void GenerateXHTMLForVirtualStrings(ICmObject owningObject, IMultiAccessorBase multiStringAccessor,
																			ConfigurableDictionaryNode config, GeneratorSettings settings)
		{
			var wsOptions = config.DictionaryNodeOptions as DictionaryNodeWritingSystemOptions;
			if(wsOptions == null)
			{
				throw new ArgumentException(@"Configuration nodes for MultiString fields should have WritingSystemOptions", "config");
			}
			foreach(var option in wsOptions.Options)
			{
				if(!option.IsEnabled)
				{
					continue;
				}
				var wsId = WritingSystemServices.GetMagicWsIdFromName(option.Id);
				// The string for the specific wsId in the option, or the best string option in the accessor if the wsId is magic
				if(wsId == 0)
				{
					// This is not a magic writing system, so grab the user requested string
					wsId = settings.Cache.WritingSystemFactory.GetWsFromStr(option.Id);
				}
				else
				{
					var defaultWs = owningObject.Cache.WritingSystemFactory.get_EngineOrNull(owningObject.Cache.DefaultUserWs);
					wsId = WritingSystemServices.InterpretWsLabel(owningObject.Cache, option.Id, (IWritingSystem)defaultWs,
																					owningObject.Hvo, multiStringAccessor.Flid, (IWritingSystem)defaultWs);
				}
				var requestedString = multiStringAccessor.get_String(wsId);
				GenerateWsPrefixAndString(config, settings, wsOptions, wsId, requestedString);
			}
		}

		private static void GenerateWsPrefixAndString(ConfigurableDictionaryNode config, GeneratorSettings settings,
			DictionaryNodeWritingSystemOptions wsOptions, int wsId, ITsString requestedString)
		{
			var writer = settings.Writer;
			var cache = settings.Cache;
			if(String.IsNullOrEmpty(requestedString.Text))
			{
				return;
			}
			if(wsOptions.DisplayWritingSystemAbbreviations)
			{
				writer.WriteStartElement("span");
				writer.WriteStartAttribute("class", "writingsystemprefix");
				var prefix = ((IWritingSystem)cache.WritingSystemFactory.get_EngineOrNull(wsId)).Abbreviation;
				writer.WriteString(prefix);
				writer.WriteEndElement();
			}
			writer.WriteStartElement("span");
			WriteClassNameAttribute(writer, config);
			var wsName = cache.WritingSystemFactory.get_EngineOrNull(wsId).Id;
			GenerateXHTMLForString(requestedString, config, settings, wsName);
			writer.WriteEndElement();
		}

		private static void GenerateXHTMLForString(ITsString fieldValue,
																 ConfigurableDictionaryNode config,
																 GeneratorSettings settings, string writingSystem = null)
		{
			var writer = settings.Writer;
			//use the passed in writing system unless null
			//otherwise use the first option from the DictionaryNodeWritingSystemOptions or english if the options are null
			writingSystem = writingSystem ?? GetLanguageFromFirstOption(config.DictionaryNodeOptions as DictionaryNodeWritingSystemOptions, settings.Cache);
			writer.WriteAttributeString("lang", writingSystem);
			writer.WriteString(fieldValue.Text);
		}

		/// <summary>
		/// This method is intended to produce the xhtml element that we want for given configuration objects.
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		private static string GetElementNameForProperty(ConfigurableDictionaryNode config)
		{
			//TODO: Improve this logic to deal with subentries if necessary
			if(config.FieldDescription.Equals("LexEntry") || config.DictionaryNodeOptions is DictionaryNodePictureOptions)
			{
				return "div";
			}
			return "span";
		}

		/// <summary>
		/// This method returns the lang attribute value from the first selected writing system in the given options.
		/// </summary>
		/// <param name="wsOptions"></param>
		/// <param name="cache"></param>
		/// <returns></returns>
		private static string GetLanguageFromFirstOption(DictionaryNodeWritingSystemOptions wsOptions, FdoCache cache)
		{
			const string defaultLang = "en";
			if(wsOptions == null)
				return defaultLang;
			foreach(var option in wsOptions.Options)
			{
				if(option.IsEnabled)
				{
					var wsId = WritingSystemServices.GetMagicWsIdFromName(option.Id);
					// if the writing system isn't a magic name just use it
					if( wsId == 0)
					{
						return option.Id;
					}
					// otherwise get a list of the writing systems for the magic name, and use the first one
					return WritingSystemServices.GetWritingSystemList(cache, wsId, true).First().Id;
				}
			}
			// paranoid fallback to first option of the list in case there are no enabled options
			return wsOptions.Options[0].Id;
		}

		public static DictionaryPublicationDecorator GetPublicationDecoratorAndEntries(Mediator mediator, out int[] entriesToSave)
		{
			var cache = mediator.PropertyTable.GetValue("cache") as FdoCache;
			if(cache == null)
			{
				throw new ArgumentException(@"Mediator had no cache", "mediator");
			}
			var clerk = mediator.PropertyTable.GetValue("ActiveClerk", null) as RecordClerk;
			if(clerk == null)
			{
				throw new ArgumentException(@"Mediator had no clerk", "mediator");
			}

			ICmPossibility currentPublication;
			var currentPublicationString = mediator.PropertyTable.GetStringProperty("SelectedPublication", xWorksStrings.AllEntriesPublication);
			if(currentPublicationString == xWorksStrings.AllEntriesPublication)
			{
				currentPublication = null;
			}
			else
			{
				currentPublication =
					(from item in cache.LangProject.LexDbOA.PublicationTypesOA.PossibilitiesOS
					 where item.Name.UserDefaultWritingSystem.Text == currentPublicationString
					 select item).FirstOrDefault();
			}
			var decorator = new DictionaryPublicationDecorator(cache, clerk.VirtualListPublisher, clerk.VirtualFlid, currentPublication);
			entriesToSave = decorator.VecProp(cache.LangProject.LexDbOA.Hvo, clerk.VirtualFlid);
			return decorator;
		}

		[SuppressMessage("Gendarme.Rules.Design", "TypesWithDisposableFieldsShouldBeDisposableRule",
			Justification="Cache and Mediator are a references")]
		public class GeneratorSettings
		{
			public FdoCache Cache { get; private set; }
			public XmlWriter Writer { get; private set; }
			public bool UseRelativePaths { get; private set; }
			public bool CopyFiles { get; private set; }
			public string ExportPath { get; private set; }
			public GeneratorSettings(FdoCache cache, XmlWriter writer, bool relativePaths, bool copyFiles, string exportPath)
			{
				if(cache == null || writer == null)
				{
					throw new ArgumentNullException();
				}
				Cache = cache;
				Writer = writer;
				UseRelativePaths = relativePaths;
				CopyFiles = copyFiles;
				ExportPath = exportPath;
			}
		}
	}
}
