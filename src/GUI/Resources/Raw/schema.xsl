<?xml version="1.0" encoding="iso-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" />
	<xsl:template match="/">
		<html>
			<body>
				<h2>Salaries</h2>
				<table border="2">
					<tr bgcolor="#fff">
						<th>Name</th>
						<th>Faculty</th>
						<th>Department</th>
						<th>Role</th>
						<th>Salary</th>
						<th>Time Tenure (Days)</th>
					</tr>
					<xsl:for-each select="//Person">
						<tr>
							<td>
								<xsl:value-of select="Name"/>
							</td>
							<td>
								<xsl:value-of select="Faculty"/>
							</td>
							<td>
								<xsl:value-of select="Department"/>
							</td>
							<td>
								<xsl:value-of select="Role"/>
							</td>
							<td>
								<xsl:value-of select="Salary"/>
							</td>
							<td>
								<xsl:value-of select="TimeTenue"/>
							</td>
						</tr>
					</xsl:for-each>
				</table>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>