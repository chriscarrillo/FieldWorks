<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
   <xsl:output method="xml" encoding="UTF-8" indent="yes"/>
   <!-- This stylesheet transforms an interlinear text as output by FieldWorks into a form which conforms to the XLingPap DTD and can then be edited with the XMLmind XML Editor usingthe XLingPap configuration.  -->
   <!--
================================================================
FieldWorks Language Explorer interlinear XML to XLingPap mapper for Stage 1.
  Input:    XML output of FLEx interlinear, where result concatenates morphemes within a word
  Output: XLingPap XML

================================================================
Revision History is at the end of this file.

- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
Preamble
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
-->
   <xsl:key name="Language" match="language" use="@lang"/>
   <xsl:param name="sHyphen" select="'-'"/>
   <!--
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
Main template
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
-->
   <xsl:template match="document">
	  <!-- output dtd path -->
	  <xsl:text disable-output-escaping="yes">&#xa;&#x3c;!DOCTYPE lingPaper PUBLIC   "-//XMLmind//DTD XLingPap//EN" "XLingPap.dtd"&#x3e;&#xa;</xsl:text>
	  <lingPaper automaticallywrapinterlinears="yes">
		 <frontMatter>
			<title>
			   <xsl:choose>
				  <xsl:when test="//interlinear-text/item[@type='title']">
					 <xsl:value-of select="//interlinear-text/item[@type='title']"/>
				  </xsl:when>
				  <xsl:otherwise>
					 <xsl:text>FieldWorks Language Explorer Interlinear Export</xsl:text>
				  </xsl:otherwise>
			   </xsl:choose>
			</title>
			<author>
			   <xsl:choose>
				  <xsl:when test="//interlinear-text/item[@type='source']">
					 <xsl:value-of select="//interlinear-text/item[@type='source']"/>
				  </xsl:when>
				  <xsl:otherwise>
					 <xsl:text>[Insert author's name here]</xsl:text>
				  </xsl:otherwise>
			   </xsl:choose>
			</author>
		 </frontMatter>
		 <section1 id="s1">
			<secTitle>Interlinear text</secTitle>
			<xsl:apply-templates/>
		 </section1>
		 <backMatter>
			<endnotes/>
			<references/>
		 </backMatter>
		 <languages>
			<xsl:call-template name="OutputLanguageElements"/>
		 </languages>
		 <types>
			 <xsl:call-template name="CommonTypes"/>
			 <type id="tGrammaticalGloss" font-variant="small-caps"/>
			<type id="tWordPos"/>
			<type id="tLiteralTranslation"/>
		 </types>
	  </lingPaper>
   </xsl:template>
   <!--
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
paragraph[1]
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
-->
   <xsl:template match="paragraphs" priority="100">
	  <xsl:param name="sScriptureType"/>
	  <xsl:variable name="iCount" select="count(../preceding-sibling::interlinear-text)+1"/>
	  <p>
		 <xsl:text>paragraph </xsl:text>
		 <xsl:value-of select="$iCount"/>
	  </p>
	  <example num="x{$iCount}">
		 <xsl:apply-templates select="descendant::phrase">
			<xsl:with-param name="sScriptureType" select="$sScriptureType"/>
		 </xsl:apply-templates>
	  </example>
   </xsl:template>
   <!--
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
phrase
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
-->
   <xsl:template match="phrase">
	  <xsl:param name="sScriptureType"/>
	  <xsl:variable name="sLevel">
		 <xsl:call-template name="OutputLevelContent">
			<xsl:with-param name="sScriptureType" select="$sScriptureType"/>
		 </xsl:call-template>
	  </xsl:variable>
	  <xsl:variable name="sThisTextId">
		 <!-- we really need something from the DB or some such.  Am trying this in hopes it will be unique in most cases -->
		 <xsl:for-each select="../phrase[position()=last()]">
			<!--                <xsl:if test="position()=last()">-->
			<xsl:value-of select="generate-id()"/>
			<!--                </xsl:if>-->
		 </xsl:for-each>
	  </xsl:variable>
	  <listInterlinear letter="x{$sThisTextId}-{translate($sLevel,':','.')}">
<!--         <xsl:attribute name="letter">
			<xsl:text>x</xsl:text>
			<xsl:value-of select="$sLevel"/>
		 </xsl:attribute>
-->         <xsl:call-template name="OutputInterlinearContent"/>

		 <!--<lineGroup>
			<xsl:if test="words/word/item[@type='txt']">
			   <!-\- word -\->
			   <line>
				  <!-\- word -\->
				  <xsl:for-each select="words/word[item[@type='txt']]">
					 <wrd>
						<langData lang="{item/@lang}">
						   <!-\- prepend any initial punctuation -\->
						   <xsl:for-each select="preceding-sibling::word[1][item/@type='punct']">
							  <xsl:choose>
								 <xsl:when test="count(preceding-sibling::word)=0">
									<!-\- it's the first word item -\->
									<xsl:value-of select="normalize-space(item)"/>
								 </xsl:when>
								 <xsl:when test="preceding-sibling::word[1][item/@type='punct']">
									<!-\- there are other punct items before it; assume only the last one is preceding punct -\->
									<xsl:value-of select="normalize-space(item)"/>
								 </xsl:when>
								 <xsl:when test="contains(item,'(') or contains(item,'[') or contains(item,'{')">
									<!-\- there are other preceding word items; look for preceding punctuation N.B. may well need to look for characters, too -\->
									<xsl:value-of select="normalize-space(item)"/>
								 </xsl:when>
							  </xsl:choose>
						   </xsl:for-each>
						   <!-\- output the word -\->
						   <xsl:value-of select="normalize-space(item)"/>
						   <!-\- append any following punctuation -\->
						   <xsl:for-each select="following-sibling::word[1]/item[@type='punct']">
							  <xsl:if test="not(contains(.,'(') or contains(.,'[') or contains(.,'{'))">
								 <!-\- skip any preceding punctuation N.B. may well need to look for characters, too -\->
								 <xsl:value-of select="normalize-space(.)"/>
							  </xsl:if>
						   </xsl:for-each>
						</langData>
					 </wrd>
				  </xsl:for-each>
			   </line>
			</xsl:if>
			<!-\- morphemes -\->
			<xsl:call-template name="OutputLineOfWrdElementsFromMorphs">
			   <xsl:with-param name="sType" select="'txt'"/>
			</xsl:call-template>
			<!-\- Lex Entries -\->
			<xsl:call-template name="OutputLineOfWrdElementsFromMorphs">
			   <xsl:with-param name="sType" select="'cf'"/>
			</xsl:call-template>
			<!-\- Gloss -\->
			<xsl:call-template name="OutputLineOfWrdElementsFromMorphs">
			   <xsl:with-param name="sType" select="'gls'"/>
			   <xsl:with-param name="bAddHyphen" select="'Y'"/>
			</xsl:call-template>
			<!-\- msa -\->
			<xsl:call-template name="OutputLineOfWrdElementsFromMorphs">
			   <xsl:with-param name="sType" select="'msa'"/>
			   <xsl:with-param name="bAddHyphen" select="'Y'"/>
			</xsl:call-template>
			<!-\- word gloss -\->
			<xsl:call-template name="OutputLineOfWrdElementsFromWord">
			   <xsl:with-param name="sType" select="'gls'"/>
			</xsl:call-template>
			<!-\- word cat -\->
			<xsl:call-template name="OutputLineOfWrdElementsFromWord">
			   <xsl:with-param name="sType" select="'pos'"/>
			</xsl:call-template>
		 </lineGroup>
		 <xsl:for-each select="item">
			<xsl:choose>
			   <xsl:when test="@type='txt'">
				  <!-\- what is this for?   -\->
			   </xsl:when>
			   <xsl:when test="@type='gls'">
				  <xsl:call-template name="OutputFreeWithAnyNotes"/>
			   </xsl:when>
			   <xsl:when test="@type='lit' ">
				  <!-\-  someday we'll have a literal translation element in XLingPaper -\->
				  <free>
					 <object type="tLiteralTranslation">
						<xsl:apply-templates/>
					 </object>
				  </free>
			   </xsl:when>
			</xsl:choose>
		 </xsl:for-each>-->
	  </listInterlinear>
   </xsl:template>
   <xsl:include href="xml2XLingPapCommonConcatMorphemes.xsl"/>
</xsl:stylesheet>
<!--
================================================================
Revision History
- - - - - - - - - - - - - - - - - - -
28-Jul-2006  Andy Black  Initial Draft
================================================================
-->
