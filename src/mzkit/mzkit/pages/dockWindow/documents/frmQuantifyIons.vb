﻿#Region "Microsoft.VisualBasic::37bd38180675b27452b435d36e0e2035, pages\dockWindow\documents\frmQuantifyIons.vb"

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

' Class frmQuantifyIons
' 
'     Sub: frmQuantifyIons_Load
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Math.GCMS
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Text
Imports any = Microsoft.VisualBasic.Scripting

Public Class frmQuantifyIons
    Implements ISaveHandle
    Implements IFileReference

    Public Property FilePath As String Implements IFileReference.FilePath

    Public ReadOnly Property MimeType As ContentType() Implements IFileReference.MimeType
        Get
            Return {
                New ContentType With {
                    .Details = "GCMS Quantify Ions",
                    .FileExt = ".ionPack",
                    .MIMEType = "application/msl",
                    .Name = "GCMS Quantify Ions"
                }
            }
        End Get
    End Property

    Private Sub frmQuantifyIons_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = "GC-MS Quantify Ions Library"
        FilePath = Globals.Settings.QuantifyIonLibfile

        If FilePath.FileLength > 0 Then
            Try
                Using file As Stream = FilePath.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                    For Each ion As QuantifyIon In MsgPackSerializer.Deserialize(Of QuantifyIon())(file)
                        DataGridView1.Rows.Add(ion.id, ion.name, ion.rt.Min, ion.rt.Max, ion.ms(Scan0).mz, ion.ms.ElementAtOrDefault(1)?.mz)
                    Next
                End Using
            Catch ex As Exception

            End Try
        End If
    End Sub

    Private Sub DataGridView1_KeyDown(sender As Object, e As KeyEventArgs) Handles DataGridView1.KeyDown, Me.KeyDown
        If e.KeyCode = Keys.V AndAlso e.Control AndAlso Clipboard.ContainsText Then
            Call DataGridView1.PasteTextData()
        End If
    End Sub

    Public Function Save(path As String, encoding As Encoding) As Boolean Implements ISaveHandle.Save
        Dim ions As New List(Of QuantifyIon)
        Dim row As DataGridViewRow
        Dim ion1, ion2 As ms2

        For i As Integer = 0 To DataGridView1.Rows.Count - 1
            row = DataGridView1.Rows.Item(i)
            ion1 = New ms2 With {.mz = any.ToString(row.Cells(4).Value).ParseDouble, .intensity = 1}
            ion2 = New ms2 With {.mz = any.ToString(row.Cells(5).Value).ParseDouble, .intensity = 0.65}
            ions += New QuantifyIon With {
                .id = any.ToString(row.Cells(0).Value),
                .name = any.ToString(row.Cells(1).Value),
                .rt = {
                    any.ToString(row.Cells(2).Value).ParseDouble,
                    any.ToString(row.Cells(3).Value).ParseDouble
                },
                .ms = {ion1, ion2}
            }
        Next

        FilePath = path
        Globals.Settings.QuantifyIonLibfile = path.GetFullPath

        Using file As Stream = path.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
            Call MsgPackSerializer.SerializeObject(ions.ToArray, file)
        End Using

        Return True
    End Function

    Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
        Return Save(path, encoding.CodePage)
    End Function
End Class
