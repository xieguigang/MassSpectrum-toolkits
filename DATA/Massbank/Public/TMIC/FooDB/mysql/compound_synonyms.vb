REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @2018/2/8 13:27:33


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace TMIC.FooDB.mysql

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `compound_synonyms`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `compound_synonyms` (
'''   `id` int(11) NOT NULL AUTO_INCREMENT,
'''   `synonym` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
'''   `synonym_source` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
'''   `created_at` datetime DEFAULT NULL,
'''   `updated_at` datetime DEFAULT NULL,
'''   `source_id` int(11) DEFAULT NULL,
'''   `source_type` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   PRIMARY KEY (`id`),
'''   UNIQUE KEY `index_compound_synonyms_on_synonym` (`synonym`),
'''   KEY `index_compound_synonyms_on_source_id_and_source_type` (`source_id`,`source_type`)
''' ) ENGINE=InnoDB AUTO_INCREMENT=251049 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("compound_synonyms", Database:="foodb", SchemaSQL:="
CREATE TABLE `compound_synonyms` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `synonym` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `synonym_source` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `created_at` datetime DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL,
  `source_id` int(11) DEFAULT NULL,
  `source_type` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `index_compound_synonyms_on_synonym` (`synonym`),
  KEY `index_compound_synonyms_on_source_id_and_source_type` (`source_id`,`source_type`)
) ENGINE=InnoDB AUTO_INCREMENT=251049 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;")>
Public Class compound_synonyms: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="id"), XmlAttribute> Public Property id As Long
    <DatabaseField("synonym"), NotNull, DataType(MySqlDbType.VarChar, "255"), Column(Name:="synonym")> Public Property synonym As String
    <DatabaseField("synonym_source"), NotNull, DataType(MySqlDbType.VarChar, "255"), Column(Name:="synonym_source")> Public Property synonym_source As String
    <DatabaseField("created_at"), DataType(MySqlDbType.DateTime), Column(Name:="created_at")> Public Property created_at As Date
    <DatabaseField("updated_at"), DataType(MySqlDbType.DateTime), Column(Name:="updated_at")> Public Property updated_at As Date
    <DatabaseField("source_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="source_id")> Public Property source_id As Long
    <DatabaseField("source_type"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="source_type")> Public Property source_type As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `compound_synonyms` (`id`, `synonym`, `synonym_source`, `created_at`, `updated_at`, `source_id`, `source_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `compound_synonyms` (`id`, `synonym`, `synonym_source`, `created_at`, `updated_at`, `source_id`, `source_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `compound_synonyms` WHERE `id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `compound_synonyms` SET `id`='{0}', `synonym`='{1}', `synonym_source`='{2}', `created_at`='{3}', `updated_at`='{4}', `source_id`='{5}', `source_type`='{6}' WHERE `id` = '{7}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `compound_synonyms` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `compound_synonyms` (`id`, `synonym`, `synonym_source`, `created_at`, `updated_at`, `source_id`, `source_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, id, synonym, synonym_source, DataType.ToMySqlDateTimeString(created_at), DataType.ToMySqlDateTimeString(updated_at), source_id, source_type)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{id}', '{synonym}', '{synonym_source}', '{created_at}', '{updated_at}', '{source_id}', '{source_type}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `compound_synonyms` (`id`, `synonym`, `synonym_source`, `created_at`, `updated_at`, `source_id`, `source_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, id, synonym, synonym_source, DataType.ToMySqlDateTimeString(created_at), DataType.ToMySqlDateTimeString(updated_at), source_id, source_type)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `compound_synonyms` SET `id`='{0}', `synonym`='{1}', `synonym_source`='{2}', `created_at`='{3}', `updated_at`='{4}', `source_id`='{5}', `source_type`='{6}' WHERE `id` = '{7}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, synonym, synonym_source, DataType.ToMySqlDateTimeString(created_at), DataType.ToMySqlDateTimeString(updated_at), source_id, source_type, id)
    End Function
#End Region
Public Function Clone() As compound_synonyms
                  Return DirectCast(MyClass.MemberwiseClone, compound_synonyms)
              End Function
End Class


End Namespace
