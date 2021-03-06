﻿#Region "Microsoft.VisualBasic::c515ebc6a559501162de6d6f2b4e28ba, metaDNA\Models\UnknownSet.vb"

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

    ' Class UnknownSet
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: CreateTree, EnumerateAllUnknownFeatures, MassQuery, QueryByKey, QueryByParentMz
    ' 
    '     Sub: AddTrace
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.ComponentModel.Collection

''' <summary>
''' 未知Feature列表
''' </summary>
Public Class UnknownSet

    Dim features As AVLTree(Of Double, PeakMs2)
    Dim spectrumIndex As Dictionary(Of String, PeakMs2)
    Dim nodeVisted As New Index(Of String)

    Friend ReadOnly rtmax As Double

    Private Sub New(rtmax As Double)
        Me.rtmax = rtmax
    End Sub

    Public Function QueryByKey(key As String) As PeakMs2
        Return spectrumIndex.TryGetValue(key)
    End Function

    Public Sub AddTrace(libguids As IEnumerable(Of String))
        For Each uid As String In libguids
            Call nodeVisted.Add(uid)
        Next
    End Sub

    ''' <summary>
    ''' query all unvisited unknown features by given m/z
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <returns></returns>
    Public Iterator Function QueryByParentMz(mz As Double) As IEnumerable(Of PeakMs2)
        Dim members As PeakMs2() = features.Find(mz)?.Members

        If Not members Is Nothing Then
            For Each item As PeakMs2 In members
                ' broke the vist loop at here
                If Not item.lib_guid Like nodeVisted Then
                    Yield item
                End If
            Next
        End If
    End Function

    Private Shared Function MassQuery(tolerance As Tolerance) As Comparison(Of Double)
        Return Function(x, y) As Integer
                   If tolerance(x, y) Then
                       Return 0
                   ElseIf x > y Then
                       Return 1
                   Else
                       Return -1
                   End If
               End Function
    End Function

    ''' <summary>
    ''' create tree by precursor m/z
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="tolerance"></param>
    ''' <returns></returns>
    Public Shared Function CreateTree(raw As IEnumerable(Of PeakMs2), tolerance As Tolerance) As UnknownSet
        Dim tree As New AVLTree(Of Double, PeakMs2)(MassQuery(tolerance), Function(mz) mz.ToString("F4"))
        Dim index As New Dictionary(Of String, PeakMs2)

        For Each product As PeakMs2 In raw
            If product.lib_guid Is Nothing Then
                product.lib_guid = product.ToString
            End If

            Call index.Add(product.lib_guid, product)
            Call tree.Add(product.mz, product, valueReplace:=False)
        Next

        Dim rtmax As Double = Aggregate peak As PeakMs2 In index.Values Into Max(peak.rt)

        Return New UnknownSet(rtmax) With {
            .features = tree,
            .spectrumIndex = index
        }
    End Function

    Public Iterator Function EnumerateAllUnknownFeatures() As IEnumerable(Of PeakMs2)
        For Each node As BinaryTree(Of Double, PeakMs2) In features.GetAllNodes
            For Each peak In node.Members
                Yield peak
            Next
        Next
    End Function

End Class