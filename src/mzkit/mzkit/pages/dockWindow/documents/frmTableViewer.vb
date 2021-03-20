﻿Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Text

Public Class frmTableViewer : Implements ISaveHandle, IFileReference

    Public Property FilePath As String Implements IFileReference.FilePath

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
    End Sub

    Public Function Save(path As String, encoding As Encoding) As Boolean Implements ISaveHandle.Save
        Call DataGridView1.WriteTableToFile(path)
        Return True
    End Function

    Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
        Return Save(path, encoding.CodePage)
    End Function
End Class