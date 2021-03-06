﻿#Region "Microsoft.VisualBasic::8b3e8583d4e7ffcd08d15e46f70e2617, src\metadb\SMILES\ParseChain.vb"

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

    ' Class ParseChain
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: CreateGraph, ParseGraph, ToString
    ' 
    '     Sub: WalkElement, WalkKey, WalkToken
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Linq

Public Class ParseChain

    ReadOnly graph As New ChemicalFormula
    ReadOnly chainStack As New Stack(Of ChemicalElement)
    ReadOnly stackSize As New Stack(Of Counter)
    ReadOnly SMILES As String
    ReadOnly tokens As Token()

    Dim lastKey As Bonds?

    Sub New(tokens As IEnumerable(Of Token))
        Me.tokens = tokens.ToArray
        Me.SMILES = Me.tokens _
            .Select(Function(t) t.text) _
            .JoinBy("")
    End Sub

    Public Shared Function ParseGraph(SMILES As String) As ChemicalFormula
        Dim tokens As Token() = New Scanner(SMILES).GetTokens().ToArray
        Dim graph As ChemicalFormula = New ParseChain(tokens).CreateGraph
        Dim degree = graph.AllBonds.DoCall(AddressOf Network.ComputeDegreeData(Of ChemicalElement, ChemicalKey))

        For Each element As ChemicalElement In graph.AllElements
            element.degree = (degree.in.TryGetValue(element.label), degree.out.TryGetValue(element.label))
        Next

        Return graph
    End Function

    Public Function CreateGraph() As ChemicalFormula
        For Each t As Token In tokens
            Call WalkToken(t)
        Next

        Return graph
    End Function

    Private Sub WalkToken(t As Token)
        Select Case t.name
            Case ElementTypes.Element : Call WalkElement(t)
            Case ElementTypes.Key : Call WalkKey(t)
            Case ElementTypes.Open
                ' do nothing
                stackSize.Push(0)
            Case ElementTypes.Close
                Call chainStack.Pop(stackSize.Pop())
            Case Else
                Throw New NotImplementedException(t.ToString)
        End Select
    End Sub

    Private Sub WalkKey(t As Token)
        lastKey = CType(CByte(ChemicalBonds.IndexOf(t.text)), Bonds)
    End Sub

    Private Sub WalkElement(t As Token)
        Dim element As New ChemicalElement(t.text)

        element.ID = graph.vertex.Count
        graph.AddVertex(element)

        If chainStack.Count > 0 Then
            Dim lastElement As ChemicalElement = chainStack.Peek
            Dim bondType As Bonds = If(lastKey Is Nothing, Bonds.single, lastKey.Value)
            ' 默认为单键
            Dim bond As New ChemicalKey With {
                .U = lastElement,
                .V = element,
                .weight = 1,
                .bond = bondType
            }

            Call graph.AddBond(bond)
        End If
        If stackSize.Count > 0 Then
            Call stackSize.Peek.Hit()
        End If

        lastKey = Nothing
        chainStack.Push(element)
    End Sub

    Public Overrides Function ToString() As String
        Return SMILES
    End Function

End Class

