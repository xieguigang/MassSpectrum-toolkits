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
''' DROP TABLE IF EXISTS `health_effects`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `health_effects` (
'''   `id` int(11) NOT NULL AUTO_INCREMENT,
'''   `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
'''   `description` mediumtext COLLATE utf8_unicode_ci,
'''   `chebi_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `chebi_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `created_at` datetime DEFAULT NULL,
'''   `updated_at` datetime DEFAULT NULL,
'''   `creator_id` int(11) DEFAULT NULL,
'''   `updater_id` int(11) DEFAULT NULL,
'''   `chebi_definition` text COLLATE utf8_unicode_ci,
'''   PRIMARY KEY (`id`),
'''   UNIQUE KEY `index_health_effects_on_name` (`name`),
'''   KEY `index_health_effects_on_chebi_name` (`chebi_name`),
'''   KEY `index_health_effects_on_chebi_id` (`chebi_id`)
''' ) ENGINE=InnoDB AUTO_INCREMENT=1436 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("health_effects", Database:="foodb", SchemaSQL:="
CREATE TABLE `health_effects` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `description` mediumtext COLLATE utf8_unicode_ci,
  `chebi_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `chebi_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `created_at` datetime DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL,
  `creator_id` int(11) DEFAULT NULL,
  `updater_id` int(11) DEFAULT NULL,
  `chebi_definition` text COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`),
  UNIQUE KEY `index_health_effects_on_name` (`name`),
  KEY `index_health_effects_on_chebi_name` (`chebi_name`),
  KEY `index_health_effects_on_chebi_id` (`chebi_id`)
) ENGINE=InnoDB AUTO_INCREMENT=1436 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;")>
Public Class health_effects: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="id"), XmlAttribute> Public Property id As Long
    <DatabaseField("name"), NotNull, DataType(MySqlDbType.VarChar, "255"), Column(Name:="name")> Public Property name As String
    <DatabaseField("description"), DataType(MySqlDbType.Text), Column(Name:="description")> Public Property description As String
    <DatabaseField("chebi_name"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="chebi_name")> Public Property chebi_name As String
    <DatabaseField("chebi_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="chebi_id")> Public Property chebi_id As String
    <DatabaseField("created_at"), DataType(MySqlDbType.DateTime), Column(Name:="created_at")> Public Property created_at As Date
    <DatabaseField("updated_at"), DataType(MySqlDbType.DateTime), Column(Name:="updated_at")> Public Property updated_at As Date
    <DatabaseField("creator_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="creator_id")> Public Property creator_id As Long
    <DatabaseField("updater_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="updater_id")> Public Property updater_id As Long
    <DatabaseField("chebi_definition"), DataType(MySqlDbType.Text), Column(Name:="chebi_definition")> Public Property chebi_definition As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `health_effects` (`id`, `name`, `description`, `chebi_name`, `chebi_id`, `created_at`, `updated_at`, `creator_id`, `updater_id`, `chebi_definition`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `health_effects` (`id`, `name`, `description`, `chebi_name`, `chebi_id`, `created_at`, `updated_at`, `creator_id`, `updater_id`, `chebi_definition`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `health_effects` WHERE `id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `health_effects` SET `id`='{0}', `name`='{1}', `description`='{2}', `chebi_name`='{3}', `chebi_id`='{4}', `created_at`='{5}', `updated_at`='{6}', `creator_id`='{7}', `updater_id`='{8}', `chebi_definition`='{9}' WHERE `id` = '{10}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `health_effects` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `health_effects` (`id`, `name`, `description`, `chebi_name`, `chebi_id`, `created_at`, `updated_at`, `creator_id`, `updater_id`, `chebi_definition`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, id, name, description, chebi_name, chebi_id, DataType.ToMySqlDateTimeString(created_at), DataType.ToMySqlDateTimeString(updated_at), creator_id, updater_id, chebi_definition)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{id}', '{name}', '{description}', '{chebi_name}', '{chebi_id}', '{created_at}', '{updated_at}', '{creator_id}', '{updater_id}', '{chebi_definition}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `health_effects` (`id`, `name`, `description`, `chebi_name`, `chebi_id`, `created_at`, `updated_at`, `creator_id`, `updater_id`, `chebi_definition`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, id, name, description, chebi_name, chebi_id, DataType.ToMySqlDateTimeString(created_at), DataType.ToMySqlDateTimeString(updated_at), creator_id, updater_id, chebi_definition)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `health_effects` SET `id`='{0}', `name`='{1}', `description`='{2}', `chebi_name`='{3}', `chebi_id`='{4}', `created_at`='{5}', `updated_at`='{6}', `creator_id`='{7}', `updater_id`='{8}', `chebi_definition`='{9}' WHERE `id` = '{10}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, name, description, chebi_name, chebi_id, DataType.ToMySqlDateTimeString(created_at), DataType.ToMySqlDateTimeString(updated_at), creator_id, updater_id, chebi_definition, id)
    End Function
#End Region
Public Function Clone() As health_effects
                  Return DirectCast(MyClass.MemberwiseClone, health_effects)
              End Function
End Class


End Namespace
