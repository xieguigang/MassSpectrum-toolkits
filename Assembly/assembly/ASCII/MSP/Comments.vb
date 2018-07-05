﻿Imports System.Collections.Specialized
Imports System.Data.Linq.Mapping
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Parsers
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace ASCII.MSP

    Public Module Comments

        ''' <summary>
        ''' 解析放置于注释之中的代谢物注释元数据
        ''' </summary>
        ''' <param name="comments$"></param>
        ''' <returns></returns>
        <Extension> Public Function ToTable(comments$) As NameValueCollection
            Dim tokens$() = CLIParser.GetTokens(comments)
            Dim data = tokens _
                .Select(Function(s)
                            Return s.GetTagValue("=", trim:=True)
                        End Function) _
                .GroupBy(Function(o) o.Name)
            Dim table As New NameValueCollection  ' 为了兼容两个SMILES结构

            For Each g In data
                For Each s As NamedValue(Of String) In g
                    Call table.Add(g.Key, s.Value)
                Next
            Next

            Return table
        End Function

        <Extension>
        Public Iterator Function AsDataFrame(msp As IEnumerable(Of MspData)) As IEnumerable(Of EntityObject)
            For Each mspValue As MspData In msp
                Yield New EntityObject With {
                    .ID = mspValue.DB_id,
                    .Properties = mspValue _
                        .DictionaryTable(primitiveType:=True) _
                        .Join(mspValue.MetaDB.DictionaryTable) _
                        .ToDictionary
                }
            Next
        End Function

        Friend ReadOnly names As Dictionary(Of String, String)
        Friend ReadOnly fields As Dictionary(Of BindProperty(Of ColumnAttribute))

        Sub New()
            names = Mappings.FieldNameMappings(Of MetaData)(explict:=True, reversed:=True)
            fields = Mappings.GetFields(Of MetaData).ToDictionary

            For Each field As BindProperty(Of ColumnAttribute) In fields.Values.ToArray
                If Not fields.ContainsKey(field.member.Name) Then
                    fields.Add(field.member.Name, field)
                End If
            Next
        End Sub

        ''' <summary>
        ''' Annotation comments text parser for lipidBlast database.
        ''' </summary>
        ''' <param name="comments$"></param>
        ''' <returns></returns>
        <Extension>
        Public Function LipidBlastParser(comments$) As MetaData
            Dim meta As MetaData = comments.FillData
            Dim tokens$() = comments.Split(";"c).Skip(1).ToArray

            meta.name = Strings.Trim(tokens(0))
            meta.precursor_type = Strings.Trim(tokens(1))
            meta.scientific_name = Strings.Trim(tokens(2))
            meta.molecular_formula = Strings.Trim(tokens(3))

            Return meta
        End Function

        ''' <summary>
        ''' 从头部的<see cref="MspData.Comments"/>字符串之中解析出具体的物质注释信息
        ''' </summary>
        ''' <param name="comments$"></param>
        ''' <returns></returns>
        <Extension> Public Function FillData(comments$) As MetaData
            Dim table As NameValueCollection = comments.ToTable
            Dim meta As Object = New MetaData
            Dim castValue As Object

            For Each field As BindProperty(Of ColumnAttribute) In fields.Values
                Dim name$ = field.Identity

                If field.Type.IsInheritsFrom(GetType(Array)) Then
                    Dim value As String()

                    value = table.GetValues(name)

                    If value.IsNullOrEmpty Then
                        value = table.GetValues(names(name))
                    End If

                    Call field.SetValue(meta, value)
                Else
                    Dim value$ = table(name)

                    If value.StringEmpty Then
                        value = table(names(name))
                    End If

                    castValue = Scripting.CTypeDynamic(value, field.Type)

                    Call field.SetValue(meta, castValue)
                End If

                'table.Remove(name)
                'table.Remove(names(name))
            Next

            'If table.Count > 0 Then
            '    Throw New NotImplementedException(table.ToDictionary.GetJson)
            'End If

            Return DirectCast(meta, MetaData)
        End Function
    End Module
End Namespace