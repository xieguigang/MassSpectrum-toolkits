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
''' DROP TABLE IF EXISTS `compounds_pathways`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `compounds_pathways` (
'''   `id` int(11) NOT NULL AUTO_INCREMENT,
'''   `compound_id` int(11) DEFAULT NULL,
'''   `pathway_id` int(11) DEFAULT NULL,
'''   `creator_id` int(11) DEFAULT NULL,
'''   `updater_id` int(11) DEFAULT NULL,
'''   `created_at` datetime NOT NULL,
'''   `updated_at` datetime NOT NULL,
'''   PRIMARY KEY (`id`),
'''   KEY `index_compounds_pathways_on_compound_id` (`compound_id`),
'''   KEY `index_compounds_pathways_on_pathway_id` (`pathway_id`),
'''   CONSTRAINT `fk_rails_14c02acb79` FOREIGN KEY (`pathway_id`) REFERENCES `pathways` (`id`),
'''   CONSTRAINT `fk_rails_34b0bf14de` FOREIGN KEY (`compound_id`) REFERENCES `compounds` (`id`)
''' ) ENGINE=InnoDB AUTO_INCREMENT=1605 DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("compounds_pathways", Database:="foodb", SchemaSQL:="
CREATE TABLE `compounds_pathways` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `compound_id` int(11) DEFAULT NULL,
  `pathway_id` int(11) DEFAULT NULL,
  `creator_id` int(11) DEFAULT NULL,
  `updater_id` int(11) DEFAULT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `index_compounds_pathways_on_compound_id` (`compound_id`),
  KEY `index_compounds_pathways_on_pathway_id` (`pathway_id`),
  CONSTRAINT `fk_rails_14c02acb79` FOREIGN KEY (`pathway_id`) REFERENCES `pathways` (`id`),
  CONSTRAINT `fk_rails_34b0bf14de` FOREIGN KEY (`compound_id`) REFERENCES `compounds` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=1605 DEFAULT CHARSET=utf8;")>
Public Class compounds_pathways: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="id"), XmlAttribute> Public Property id As Long
    <DatabaseField("compound_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="compound_id")> Public Property compound_id As Long
    <DatabaseField("pathway_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="pathway_id")> Public Property pathway_id As Long
    <DatabaseField("creator_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="creator_id")> Public Property creator_id As Long
    <DatabaseField("updater_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="updater_id")> Public Property updater_id As Long
    <DatabaseField("created_at"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="created_at")> Public Property created_at As Date
    <DatabaseField("updated_at"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="updated_at")> Public Property updated_at As Date
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `compounds_pathways` (`id`, `compound_id`, `pathway_id`, `creator_id`, `updater_id`, `created_at`, `updated_at`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `compounds_pathways` (`id`, `compound_id`, `pathway_id`, `creator_id`, `updater_id`, `created_at`, `updated_at`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `compounds_pathways` WHERE `id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `compounds_pathways` SET `id`='{0}', `compound_id`='{1}', `pathway_id`='{2}', `creator_id`='{3}', `updater_id`='{4}', `created_at`='{5}', `updated_at`='{6}' WHERE `id` = '{7}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `compounds_pathways` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `compounds_pathways` (`id`, `compound_id`, `pathway_id`, `creator_id`, `updater_id`, `created_at`, `updated_at`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, id, compound_id, pathway_id, creator_id, updater_id, DataType.ToMySqlDateTimeString(created_at), DataType.ToMySqlDateTimeString(updated_at))
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{id}', '{compound_id}', '{pathway_id}', '{creator_id}', '{updater_id}', '{created_at}', '{updated_at}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `compounds_pathways` (`id`, `compound_id`, `pathway_id`, `creator_id`, `updater_id`, `created_at`, `updated_at`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, id, compound_id, pathway_id, creator_id, updater_id, DataType.ToMySqlDateTimeString(created_at), DataType.ToMySqlDateTimeString(updated_at))
    End Function
''' <summary>
''' ```SQL
''' UPDATE `compounds_pathways` SET `id`='{0}', `compound_id`='{1}', `pathway_id`='{2}', `creator_id`='{3}', `updater_id`='{4}', `created_at`='{5}', `updated_at`='{6}' WHERE `id` = '{7}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, compound_id, pathway_id, creator_id, updater_id, DataType.ToMySqlDateTimeString(created_at), DataType.ToMySqlDateTimeString(updated_at), id)
    End Function
#End Region
Public Function Clone() As compounds_pathways
                  Return DirectCast(MyClass.MemberwiseClone, compounds_pathways)
              End Function
End Class


End Namespace
