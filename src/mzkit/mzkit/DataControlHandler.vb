﻿#Region "Microsoft.VisualBasic::605d40b368e86a7ee569d372d2f2b5f5, src\mzkit\mzkit\DataControlHandler.vb"

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

' Module DataControlHandler
' 
'     Sub: SaveDataGrid
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text

Module DataControlHandler

    ''' <summary>
    ''' save data grid as excel table file
    ''' </summary>
    ''' <param name="table"></param>
    <Extension>
    Public Sub SaveDataGrid(table As DataGridView)
        Using file As New SaveFileDialog With {.Filter = "Excel Table(*.xls)|*.xls"}
            If file.ShowDialog = DialogResult.OK Then
                Using writeTsv As StreamWriter = file.FileName.OpenWriter
                    Dim row As New List(Of String)

                    For i As Integer = 0 To table.Columns.Count - 1
                        row.Add(table.Columns(i).HeaderText)
                    Next

                    writeTsv.WriteLine(row.PopAll.JoinBy(vbTab))

                    For j As Integer = 0 To table.Rows.Count - 1
                        Dim rowObj = table.Rows(j)

                        For i As Integer = 0 To rowObj.Cells.Count - 1
                            row.Add(Scripting.ToString(rowObj.Cells(i).Value))
                        Next

                        writeTsv.WriteLine(row.PopAll.Select(Function(s) $"""{s}""").JoinBy(vbTab))
                    Next
                End Using

                MessageBox.Show($"Exact Mass Search Result table export to [{file.FileName}] successfully!", "Export Table", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End Using
    End Sub

    <Extension>
    Public Sub PasteTextData(table As DataGridView)
        Dim text As String = Strings.Trim(Clipboard.GetText).Trim(ASCII.CR, ASCII.LF, ASCII.TAB)

        If table.SelectedCells.Count = 0 Then
            Return
        End If

        Dim i As Integer = table.SelectedCells.Item(Scan0).RowIndex
        Dim j As Integer = table.SelectedCells.Item(Scan0).ColumnIndex

        If text.Contains(vbCr) OrElse text.Contains(vbLf) Then
            Dim colCells As String() = text.LineTokens

            If i + colCells.Length >= table.Rows.Count Then
                Dim n As Integer = table.Rows.Count

                For rid As Integer = 0 To (colCells.Length + i) - n
                    table.Rows.Add()
                Next
            End If

            For ii As Integer = 0 To colCells.Length - 1
                table.Rows(ii + i).Cells(j).Value = colCells(ii)
            Next
        Else
            Dim rowCells As String() = text.Split(ASCII.TAB)

            For ci As Integer = 0 To rowCells.Length - 1
                table.Rows(i).Cells(j + ci).Value = rowCells(ci)
            Next
        End If
    End Sub
End Module
