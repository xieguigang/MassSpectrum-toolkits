﻿#Region "Microsoft.VisualBasic::ec6827293e3b2f707991c9ea95eb865b, src\mzkit\mzkit\forms\frmTweaks\frmMsImagingTweaks.vb"

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

' Class frmMsImagingTweaks
' 
'     Function: GetSelectedIons
' 
'     Sub: AddIonMzLayer, ClearSelectionToolStripMenuItem_Click, frmMsImagingTweaks_Load, loadRenderFromCDF, PropertyGrid1_DragDrop
'          PropertyGrid1_DragEnter, RGBLayers, ToolStripButton1_Click, ToolStripButton2_Click, Win7StyleTreeView1_AfterCheck
' 
' /********************************************************************************/

#End Region

Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Math
Imports mzkit.My
Imports RibbonLib.Interop

Public Class frmMsImagingTweaks

    Friend checkedMz As New List(Of TreeNode)
    Friend viewer As frmMsImagingViewer

    Public Iterator Function GetSelectedIons() As IEnumerable(Of Double)
        If checkedMz.Count > 0 Then
            For Each node In checkedMz
                Yield DirectCast(node.Tag, Double)
            Next
        Else
            If Not Win7StyleTreeView1.SelectedNode Is Nothing Then
                If Win7StyleTreeView1.SelectedNode.Tag Is Nothing Then
                    For Each node As TreeNode In Win7StyleTreeView1.SelectedNode.Nodes
                        Yield DirectCast(node.Tag, Double)
                    Next
                Else
                    Yield DirectCast(Win7StyleTreeView1.SelectedNode.Tag, Double)
                End If
            End If
        End If
    End Function

    Private Sub frmMsImagingTweaks_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.TabText = "MsImage Parameters"

        Call ApplyVsTheme(ContextMenuStrip1, ToolStrip1)

        Win7StyleTreeView1.Nodes.Add("Ion Layers")
        RibbonEvents.ribbonItems.TabGroupMSI.ContextAvailable = ContextAvailability.Active
    End Sub

    Private Sub ClearSelectionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearSelectionToolStripMenuItem.Click, ToolStripButton3.Click
        Win7StyleTreeView1.Nodes.Clear()
        checkedMz.Clear()
        Win7StyleTreeView1.Nodes.Add("Ion Layers")
    End Sub

    Private Sub Win7StyleTreeView1_AfterCheck(sender As Object, e As TreeViewEventArgs) Handles Win7StyleTreeView1.AfterCheck
        If e.Node.Checked Then
            If e.Node.Tag Is Nothing Then
                For Each mz As TreeNode In e.Node.Nodes
                    mz.Checked = True
                    checkedMz.Add(mz)
                Next
            Else
                checkedMz.Add(e.Node)
            End If
        Else
            If e.Node.Tag Is Nothing Then
                For Each mz As TreeNode In e.Node.Nodes
                    mz.Checked = False
                    checkedMz.Remove(mz)
                Next
            Else
                checkedMz.Remove(e.Node)
            End If
        End If
    End Sub

    ''' <summary>
    ''' load m/z layer
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        If ToolStripSpringTextBox1.Text.StringEmpty Then
            Call MyApplication.host.showStatusMessage("no ions data...", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            Dim mz As Double = Val(ToolStripSpringTextBox1.Text)
            Dim viewer = WindowModules.viewer

            If TypeOf viewer Is frmMsImagingViewer Then
                Call DirectCast(viewer, frmMsImagingViewer).renderByMzList({mz})
            End If
        End If
    End Sub

    ''' <summary>
    ''' add ion layer by m/z
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        If ToolStripSpringTextBox1.Text.StringEmpty Then
            Call MyApplication.host.showStatusMessage("no ions data...", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            Call AddIonMzLayer(mz:=Val(ToolStripSpringTextBox1.Text))
        End If

        ToolStripSpringTextBox1.Text = ""
    End Sub

    Private Sub AddIonMzLayer(mz As Double)
        If Win7StyleTreeView1.Nodes.Count = 0 Then
            Win7StyleTreeView1.Nodes.Add("Ion Layers")
        End If

        Dim node As TreeNode = Win7StyleTreeView1.Nodes.Item(0).Nodes.Add(mz)

        node.Tag = mz
    End Sub

    ''' <summary>
    ''' 只渲染前三个选中的m/z
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub RGBLayers(sender As Object, e As EventArgs) Handles RenderLayerCompositionModeToolStripMenuItem.Click
        Dim mz3 As Double() = GetSelectedIons.Take(3).ToArray
        Dim r As Double = mz3.ElementAtOrDefault(0, [default]:=-1)
        Dim g As Double = mz3.ElementAtOrDefault(1, [default]:=-1)
        Dim b As Double = mz3.ElementAtOrDefault(2, [default]:=-1)
        Dim viewer = WindowModules.viewer

        Call viewer.renderRGB(r, g, b)
    End Sub

    Private Sub PropertyGrid1_DragDrop(sender As Object, e As DragEventArgs) Handles PropertyGrid1.DragDrop
        Dim files() As String = e.Data.GetData(DataFormats.FileDrop)
        Dim firstFile As String = files.ElementAtOrDefault(Scan0)

        If Not firstFile Is Nothing Then
            If firstFile.ExtensionSuffix("imzML") Then
                Call MyApplication.host.OpenFile(firstFile, showDocument:=True)
            ElseIf firstFile.ExtensionSuffix("CDF") Then
                Call loadRenderFromCDF(firstFile)
            Else
                Call MyApplication.host.showStatusMessage("invalid file type!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            End If
        End If
    End Sub

    Public Sub loadRenderFromCDF(firstFile As String)
        Using cdf As New netCDFReader(firstFile)
            Dim size As Size = cdf.GetMsiDimension
            Dim pixels As PixelData() = cdf.LoadPixelsData.ToArray
            Dim viewer = WindowModules.viewer
            Dim mzpack As ReadRawPack = cdf.CreatePixelReader
            Dim render As New Drawer(mzpack)

            If TypeOf viewer Is frmMsImagingViewer Then
                Call DirectCast(viewer, frmMsImagingViewer).LoadRender(render, firstFile)
            End If

            If TypeOf viewer Is frmMsImagingViewer Then
                Call DirectCast(viewer, frmMsImagingViewer).renderByPixelsData(pixels, size)
            End If

            Win7StyleTreeView1.Nodes.Clear()

            For Each mz As Double In pixels _
                .GroupBy(Function(p) p.mz, cdf.GetMzTolerance) _
                .Select(Function(a)
                            Return Val(a.name)
                        End Function)

                Call AddIonMzLayer(mz)
            Next
        End Using
    End Sub

    Private Sub PropertyGrid1_DragEnter(sender As Object, e As DragEventArgs) Handles PropertyGrid1.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub LoadAllIonsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LoadAllIonsToolStripMenuItem.Click
        Win7StyleTreeView1.Nodes.Clear()

        If Not viewer.render Is Nothing Then
            Dim progress As New frmProgressSpinner
            Dim layers = Win7StyleTreeView1.Nodes.Add("Ion Layers")

            Call New Thread(Sub()
                                Call Me.Invoke(Sub()
                                                   For Each mz As Double In viewer.render.pixelReader.LoadMzArray(30)
                                                       layers.Nodes.Add(mz.ToString("F4")).Tag = mz
                                                       Application.DoEvents()
                                                   Next
                                               End Sub)
                                Call progress.Invoke(Sub() progress.Close())
                            End Sub).Start()

            Call progress.ShowDialog()
        End If
    End Sub
End Class
