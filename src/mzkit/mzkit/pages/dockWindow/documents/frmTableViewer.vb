﻿#Region "Microsoft.VisualBasic::c9b06c3861861d4c9da1e0e76a73f531, src\mzkit\mzkit\pages\dockWindow\documents\frmTableViewer.vb"

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

    ' Class frmTableViewer
    ' 
    '     Properties: FilePath, MimeType, ViewRow
    ' 
    '     Function: (+2 Overloads) Save
    ' 
    '     Sub: frmTableViewer_Load, SaveDocument, ViewToolStripMenuItem_Click
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Text
Imports mzkit.My

Public Class frmTableViewer : Implements ISaveHandle, IFileReference

    Public Property FilePath As String Implements IFileReference.FilePath
    Public Property ViewRow As Action(Of Dictionary(Of String, Object))

    Public ReadOnly Property MimeType As ContentType() Implements IFileReference.MimeType
        Get
            Return {
                New ContentType With {.Details = "Excel Data Table", .FileExt = ".xls", .MIMEType = "application/xls", .Name = "Excel Data Table"}
            }
        End Get
    End Property

    Protected Overrides Sub SaveDocument()
        Call DataGridView1.SaveDataGrid("Save Table View")
    End Sub

    Private Sub frmTableViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        CopyFullPathToolStripMenuItem.Enabled = False
        OpenContainingFolderToolStripMenuItem.Enabled = False

        TabText = "Table View"

        ApplyVsTheme(ContextMenuStrip1)
    End Sub

    Public Function Save(path As String, encoding As Encoding) As Boolean Implements ISaveHandle.Save
        Call DataGridView1.WriteTableToFile(path)
        Return True
    End Function

    Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
        Return Save(path, encoding.CodePage)
    End Function

    Private Sub ViewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewToolStripMenuItem.Click
        If DataGridView1.SelectedRows.Count <= 0 Then
            Call MyApplication.host.showStatusMessage("Please select a row data for view content!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        ElseIf Not ViewRow Is Nothing Then
            Dim obj As New Dictionary(Of String, Object)
            Dim row As DataGridViewRow = DataGridView1.SelectedRows(0)

            For i As Integer = 0 To DataGridView1.Columns.Count - 1
                obj(DataGridView1.Columns(i).HeaderText) = row.Cells(i).Value
            Next

            Call _ViewRow(obj)
        End If
    End Sub
End Class
