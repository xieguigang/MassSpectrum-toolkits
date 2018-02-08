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
''' DROP TABLE IF EXISTS `compounds_health_effects`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `compounds_health_effects` (
'''   `id` int(11) NOT NULL AUTO_INCREMENT,
'''   `compound_id` int(11) NOT NULL,
'''   `health_effect_id` int(11) NOT NULL,
'''   `orig_health_effect_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `orig_compound_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `orig_citation` mediumtext COLLATE utf8_unicode_ci,
'''   `citation` mediumtext COLLATE utf8_unicode_ci NOT NULL,
'''   `citation_type` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
'''   `created_at` datetime DEFAULT NULL,
'''   `updated_at` datetime DEFAULT NULL,
'''   `creator_id` int(11) DEFAULT NULL,
'''   `updater_id` int(11) DEFAULT NULL,
'''   `source_id` int(11) DEFAULT NULL,
'''   `source_type` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   PRIMARY KEY (`id`),
'''   KEY `index_compounds_health_effects_on_source_id_and_source_type` (`source_id`,`source_type`)
''' ) ENGINE=InnoDB AUTO_INCREMENT=11093 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("compounds_health_effects", Database:="foodb", SchemaSQL:="
CREATE TABLE `compounds_health_effects` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `compound_id` int(11) NOT NULL,
  `health_effect_id` int(11) NOT NULL,
  `orig_health_effect_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `orig_compound_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `orig_citation` mediumtext COLLATE utf8_unicode_ci,
  `citation` mediumtext COLLATE utf8_unicode_ci NOT NULL,
  `citation_type` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `created_at` datetime DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL,
  `creator_id` int(11) DEFAULT NULL,
  `updater_id` int(11) DEFAULT NULL,
  `source_id` int(11) DEFAULT NULL,
  `source_type` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `index_compounds_health_effects_on_source_id_and_source_type` (`source_id`,`source_type`)
) ENGINE=InnoDB AUTO_INCREMENT=11093 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;")>
Public Class compounds_health_effects: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="id"), XmlAttribute> Public Property id As Long
    <DatabaseField("compound_id"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="compound_id")> Public Property compound_id As Long
    <DatabaseField("health_effect_id"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="health_effect_id")> Public Property health_effect_id As Long
    <DatabaseField("orig_health_effect_name"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="orig_health_effect_name")> Public Property orig_health_effect_name As String
    <DatabaseField("orig_compound_name"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="orig_compound_name")> Public Property orig_compound_name As String
    <DatabaseField("orig_citation"), DataType(MySqlDbType.Text), Column(Name:="orig_citation")> Public Property orig_citation As String
    <DatabaseField("citation"), NotNull, DataType(MySqlDbType.Text), Column(Name:="citation")> Public Property citation As String
    <DatabaseField("citation_type"), NotNull, DataType(MySqlDbType.VarChar, "255"), Column(Name:="citation_type")> Public Property citation_type As String
    <DatabaseField("created_at"), DataType(MySqlDbType.DateTime), Column(Name:="created_at")> Public Property created_at As Date
    <DatabaseField("updated_at"), DataType(MySqlDbType.DateTime), Column(Name:="updated_at")> Public Property updated_at As Date
    <DatabaseField("creator_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="creator_id")> Public Property creator_id As Long
    <DatabaseField("updater_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="updater_id")> Public Property updater_id As Long
    <DatabaseField("source_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="source_id")> Public Property source_id As Long
    <DatabaseField("source_type"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="source_type")> Public Property source_type As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `compounds_health_effects` (`id`, `compound_id`, `health_effect_id`, `orig_health_effect_name`, `orig_compound_name`, `orig_citation`, `citation`, `citation_type`, `created_at`, `updated_at`, `creator_id`, `updater_id`, `source_id`, `source_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `compounds_health_effects` (`id`, `compound_id`, `health_effect_id`, `orig_health_effect_name`, `orig_compound_name`, `orig_citation`, `citation`, `citation_type`, `created_at`, `updated_at`, `creator_id`, `updater_id`, `source_id`, `source_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `compounds_health_effects` WHERE `id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `compounds_health_effects` SET `id`='{0}', `compound_id`='{1}', `health_effect_id`='{2}', `orig_health_effect_name`='{3}', `orig_compound_name`='{4}', `orig_citation`='{5}', `citation`='{6}', `citation_type`='{7}', `created_at`='{8}', `updated_at`='{9}', `creator_id`='{10}', `updater_id`='{11}', `source_id`='{12}', `source_type`='{13}' WHERE `id` = '{14}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `compounds_health_effects` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `compounds_health_effects` (`id`, `compound_id`, `health_effect_id`, `orig_health_effect_name`, `orig_compound_name`, `orig_citation`, `citation`, `citation_type`, `created_at`, `updated_at`, `creator_id`, `updater_id`, `source_id`, `source_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, id, compound_id, health_effect_id, orig_health_effect_name, orig_compound_name, orig_citation, citation, citation_type, DataType.ToMySqlDateTimeString(created_at), DataType.ToMySqlDateTimeString(updated_at), creator_id, updater_id, source_id, source_type)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{id}', '{compound_id}', '{health_effect_id}', '{orig_health_effect_name}', '{orig_compound_name}', '{orig_citation}', '{citation}', '{citation_type}', '{created_at}', '{updated_at}', '{creator_id}', '{updater_id}', '{source_id}', '{source_type}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `compounds_health_effects` (`id`, `compound_id`, `health_effect_id`, `orig_health_effect_name`, `orig_compound_name`, `orig_citation`, `citation`, `citation_type`, `created_at`, `updated_at`, `creator_id`, `updater_id`, `source_id`, `source_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, id, compound_id, health_effect_id, orig_health_effect_name, orig_compound_name, orig_citation, citation, citation_type, DataType.ToMySqlDateTimeString(created_at), DataType.ToMySqlDateTimeString(updated_at), creator_id, updater_id, source_id, source_type)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `compounds_health_effects` SET `id`='{0}', `compound_id`='{1}', `health_effect_id`='{2}', `orig_health_effect_name`='{3}', `orig_compound_name`='{4}', `orig_citation`='{5}', `citation`='{6}', `citation_type`='{7}', `created_at`='{8}', `updated_at`='{9}', `creator_id`='{10}', `updater_id`='{11}', `source_id`='{12}', `source_type`='{13}' WHERE `id` = '{14}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, compound_id, health_effect_id, orig_health_effect_name, orig_compound_name, orig_citation, citation, citation_type, DataType.ToMySqlDateTimeString(created_at), DataType.ToMySqlDateTimeString(updated_at), creator_id, updater_id, source_id, source_type, id)
    End Function
#End Region
Public Function Clone() As compounds_health_effects
                  Return DirectCast(MyClass.MemberwiseClone, compounds_health_effects)
              End Function
End Class


End Namespace
