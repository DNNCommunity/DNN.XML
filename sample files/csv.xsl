<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
  <xsl:output method="text" />
  <xsl:template match="/founders">
    <xsl:text>First Name; Last Name&#xD;&#xA;</xsl:text>
    <xsl:for-each select ="person">
      <xsl:value-of select ="@first-name"/>
      <xsl:text>; </xsl:text>
      <xsl:value-of select ="@last-name"/>
      <xsl:text> &#xd;&#xa;</xsl:text>
    </xsl:for-each>
  </xsl:template>
</xsl:stylesheet>
