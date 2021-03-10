﻿#Region "Microsoft.VisualBasic::7ef2a4ad6ddb43a9d515d69fb35516ba, Library\mzkit.plot\ChromatogramOverlap.vb"

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

' Class ChromatogramOverlap
' 
' 
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.DataReader
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components.Interface
Imports SMRUCC.Rsharp.Runtime.Internal.Object

''' <summary>
''' a collection of <see cref="Chromatogram"/>
''' </summary>
Public Class ChromatogramOverlap : Implements RNames, RNameIndex

    Public Property overlaps As New Dictionary(Of String, Chromatogram)

    Default Public Overloads Property TIC(refName As String) As Chromatogram
        Get
            Return _overlaps(refName)
        End Get
        Set
            _overlaps(refName) = Value
        End Set
    End Property

    Default Public Overloads ReadOnly Property TIC(refNames As String()) As ChromatogramOverlap
        Get
            Return New ChromatogramOverlap With {
                .overlaps = refNames _
                    .ToDictionary(Function(name) name,
                                  Function(name)
                                      Return _overlaps(name)
                                  End Function)
            }
        End Get
    End Property

    Public ReadOnly Property length As Integer
        Get
            Return overlaps.Count
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return overlaps.Keys.GetJson
    End Function

    Public Function setNames(names() As String, envir As Environment) As Object Implements RNames.setNames
        Throw New NotImplementedException()
    End Function

    Public Function hasName(name As String) As Boolean Implements RNames.hasName
        Return overlaps.ContainsKey(name)
    End Function

    Public Function getNames() As String() Implements IReflector.getNames
        Return overlaps.Keys.ToArray
    End Function

    Public Function getByName(name As String) As Object Implements RNameIndex.getByName
        Return overlaps.TryGetValue(name)
    End Function

    Public Function getByName(names() As String) As Object Implements RNameIndex.getByName
        Return New list With {
            .slots = names _
                .ToDictionary(Function(name) name,
                              Function(name)
                                  Return getByName(name)
                              End Function)
        }
    End Function

    Public Function setByName(name As String, value As Object, envir As Environment) As Object Implements RNameIndex.setByName
        Throw New NotImplementedException()
    End Function

    Public Function setByName(names() As String, value As Array, envir As Environment) As Object Implements RNameIndex.setByName
        Throw New NotImplementedException()
    End Function
End Class
