﻿Imports WeifenLuo.WinFormsUI.Docking

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMsImagingViewer
    Inherits DocumentWindow

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMsImagingViewer))
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.SaveImageToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ExportMatrixToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PixelSelector1 = New ControlLibrary.PixelSelector()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SaveImageToolStripMenuItem, Me.ToolStripMenuItem1, Me.ExportMatrixToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(146, 54)
        '
        'SaveImageToolStripMenuItem
        '
        Me.SaveImageToolStripMenuItem.Image = CType(resources.GetObject("SaveImageToolStripMenuItem.Image"), System.Drawing.Image)
        Me.SaveImageToolStripMenuItem.Name = "SaveImageToolStripMenuItem"
        Me.SaveImageToolStripMenuItem.Size = New System.Drawing.Size(145, 22)
        Me.SaveImageToolStripMenuItem.Text = "Save Image"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(142, 6)
        '
        'ExportMatrixToolStripMenuItem
        '
        Me.ExportMatrixToolStripMenuItem.Name = "ExportMatrixToolStripMenuItem"
        Me.ExportMatrixToolStripMenuItem.Size = New System.Drawing.Size(145, 22)
        Me.ExportMatrixToolStripMenuItem.Text = "Export Matrix"
        '
        'PixelSelector1
        '
        Me.PixelSelector1.ContextMenuStrip = Me.ContextMenuStrip1
        Me.PixelSelector1.Cursor = System.Windows.Forms.Cursors.Cross
        Me.PixelSelector1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PixelSelector1.Location = New System.Drawing.Point(0, 0)
        Me.PixelSelector1.MSImage = Nothing
        Me.PixelSelector1.Name = "PixelSelector1"
        Me.PixelSelector1.Size = New System.Drawing.Size(1004, 653)
        Me.PixelSelector1.TabIndex = 1
        '
        'frmMsImagingViewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1004, 653)
        Me.Controls.Add(Me.PixelSelector1)
        Me.DoubleBuffered = True
        Me.Name = "frmMsImagingViewer"
        Me.Text = "MS-Imaging Viewer"
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents SaveImageToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ExportMatrixToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
    Friend WithEvents PixelSelector1 As ControlLibrary.PixelSelector
End Class
