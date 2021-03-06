﻿#Region "Microsoft.VisualBasic::d79e26582dfdff4a71b2eb91f9ede62c, src\assembly\assembly\MarkupData\mzML\XML\ControlVocabulary.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:

    '     Class cvList
    ' 
    '         Properties: list
    ' 
    '     Structure cv
    ' 
    '         Properties: fullName, id, URI, version
    ' 
    '         Function: ToString
    ' 
    '     Class Params
    ' 
    '         Properties: cvParams, userParams
    ' 
    '     Class userParam
    ' 
    '         Properties: name, type, value
    ' 
    '         Function: ToString
    ' 
    '     Class cvParam
    ' 
    '         Properties: accession, cvRef, name, unitAccession, unitCvRef
    '                     unitName, value
    ' 
    '         Function: GetDouble, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Language.Default

Namespace MarkupData.mzML.ControlVocabulary

    Public Class cvList : Inherits List

        <XmlElement(NameOf(cv))>
        Public Property list As cv()

    End Class

    Public Structure cv

        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property fullName As String
        <XmlAttribute> Public Property version As String
        <XmlAttribute> Public Property URI As String

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return fullName
        End Function
    End Structure

    Public Class Params

        <XmlElement(NameOf(cvParam))>
        Public Property cvParams As cvParam()

        <XmlElement(NameOf(userParam))>
        Public Property userParams As userParam()

    End Class

    Public Class userParam : Implements INamedValue

        <XmlAttribute> Public Property name As String Implements IKeyedEntity(Of String).Key
        <XmlAttribute> Public Property value As String
        <XmlAttribute> Public Property type As String

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return $"Dim {name} As {type} = {value}"
        End Function
    End Class

    ''' <summary>
    ''' [<see cref="cvParam.name"/> => <see cref="cvParam"/>]
    ''' </summary>
    Public Class cvParam : Implements INamedValue

        <XmlAttribute> Public Property cvRef As String
        <XmlAttribute> Public Property accession As String
        <XmlAttribute> Public Property name As String Implements IKeyedEntity(Of String).Key
        <XmlAttribute> Public Property value As String
        <XmlAttribute> Public Property unitName As String
        <XmlAttribute> Public Property unitCvRef As String
        <XmlAttribute> Public Property unitAccession As String

        Shared ReadOnly Unknown As [Default](Of String) = NameOf(Unknown)

        ''' <summary>
        ''' returns <see cref="value"/> as <see cref="Double"/>
        ''' </summary>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetDouble() As Double
            Return Val(value)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return $"[{accession}] Dim {name} As <{unitName Or Unknown}> = {value}"
        End Function
    End Class
End Namespace
