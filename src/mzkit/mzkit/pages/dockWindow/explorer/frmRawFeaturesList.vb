﻿Imports System.ComponentModel
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports mzkit.Kesoft.Windows.Forms.Win7StyleTreeView
Imports mzkit.My
Imports RibbonLib.Interop
Imports Task

Public Class frmRawFeaturesList

    Friend WithEvents treeView1 As New Win7StyleTreeView

    Public ReadOnly Property CurrentRawFile As Raw

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        DoubleBuffered = True
    End Sub

    Private Sub frmFileExplorer_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        MyApplication.host.ribbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.Active
    End Sub

    Private Sub frmFileExplorer_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
        Me.Hide()
    End Sub

    Private Sub frmFileExplorer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Controls.Add(treeView1)

        ContextMenuStrip1.RenderMode = ToolStripRenderMode.System

        treeView1.Location = New Point(1, TextBox2.Height + 5)
        treeView1.Size = New Size(Width - 2, Me.Height - TextBox2.Height - 25)
        treeView1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        treeView1.HotTracking = True
        treeView1.CheckBoxes = True
        treeView1.BringToFront()
        treeView1.ContextMenuStrip = ContextMenuStrip1
        treeView1.ShowLines = True
        treeView1.ShowRootLines = True
        treeView1.Dock = DockStyle.Fill

        Me.TabText = "Features Explorer"
    End Sub

    Public Sub LoadRaw(raw As Raw)
        _CurrentRawFile = raw
        treeView1.loadRawFile(raw)
    End Sub

    Public Iterator Function GetXICCollection(ppm As Double) As IEnumerable(Of NamedCollection(Of ChromatogramTick))
        Dim scans = GetSelectedNodes _
            .Where(Function(t) TypeOf t.Tag Is ScanEntry) _
            .Select(Function(a) DirectCast(a.Tag, ScanEntry)) _
            .GroupBy(Function(a) a.mz, Tolerance.DeltaMass(0.3)) _
            .ToArray
        Dim raw As Raw = CurrentRawFile

        For Each scanId In scans.Select(Function(a) a.First.id)
            Yield MyApplication.mzkitRawViewer.getXICMatrix(raw, scanId, ppm, relativeInto:=False)
        Next
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    Dim checked As New List(Of TreeNode)

    ''' <summary>
    ''' 不包含 root node
    ''' </summary>
    ''' <returns></returns>
    Public Function GetSelectedNodes() As IEnumerable(Of TreeNode)
        Return checked.AsEnumerable
    End Function

    Dim lockCheckList As Boolean

    Public Sub Clear()
        lockCheckList = True

        For i As Integer = 0 To checked.Count - 1
            checked(i).Checked = False
        Next

        checked.Clear()
        lockCheckList = False
    End Sub

    Private Sub TreeView1_AfterCheck(sender As Object, e As TreeViewEventArgs) Handles treeView1.AfterCheck
        If lockCheckList Then
            Return
        End If

        If TypeOf e.Node.Tag Is ScanEntry Then
            If e.Node.Checked Then
                checked.Add(e.Node)
            Else
                checked.Remove(e.Node)
            End If
        Else
            Dim checked As Boolean = e.Node.Checked
            Dim node As TreeNode

            For i As Integer = 0 To e.Node.Nodes.Count - 1
                node = e.Node.Nodes(i)
                node.Checked = checked

                If checked Then
                    Me.checked.Add(node)
                Else
                    Me.checked.Remove(node)
                End If
            Next
        End If

        ClearToolStripMenuItem.Text = $"Clear [{checked.Count} XIC Ions]"
    End Sub

    Private Sub ClearSelectionsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearToolStripMenuItem.Click
        lockCheckList = True
        checked.Clear()

        For i As Integer = 0 To treeView1.Nodes.Count - 1
            treeView1.Nodes(i).Checked = False

            If Not treeView1.Nodes(i).Nodes Is Nothing Then
                For j As Integer = 0 To treeView1.Nodes(i).Nodes.Count - 1
                    treeView1.Nodes(i).Nodes(j).Checked = False
                Next
            End If
        Next

        lockCheckList = False
    End Sub

    Private Sub DeleteFileToolStripMenuItem_Click(sender As Object, e As EventArgs) 'Handles DeleteFileToolStripMenuItem.Click
        Dim current = treeView1.CurrentRawFile

        If Not current.raw Is Nothing Then
            If MessageBox.Show($"Going to remove the raw data file [{current.raw.source.FileName}]?", "Delete File", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.OK Then
                treeView1.Nodes.Remove(current.tree)
                treeView1.SaveRawFileCache(
                    Sub()
                        ' do nothing
                    End Sub)

                Call MyApplication.host.mzkitTool.setCurrentFile()
            End If
        End If

        If treeView1.Nodes.Count = 0 Then
            MyApplication.host.showStatusMessage("No raw file for removes!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            Do While treeView1.Nodes.Count > 0
                Dim clearOne As Boolean = False

                For i As Integer = 0 To treeView1.Nodes.Count - 1
                    If treeView1.Nodes(i).Checked Then
                        treeView1.Nodes.Remove(treeView1.Nodes(i))
                        clearOne = True
                        Exit For
                    End If
                Next

                If Not clearOne Then
                    Exit Do
                End If
            Loop
        End If

        ' MyApplication.host.ToolStripStatusLabel2.Text =  treeView1.GetTotalCacheSize
    End Sub

    Private Sub SelectAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SelectAllToolStripMenuItem.Click
        lockCheckList = True
        checked.Clear()

        For i As Integer = 0 To treeView1.Nodes.Count - 1
            treeView1.Nodes(i).Checked = True

            If Not treeView1.Nodes(i).Nodes Is Nothing Then
                For j As Integer = 0 To treeView1.Nodes(i).Nodes.Count - 1
                    treeView1.Nodes(i).Nodes(j).Checked = True
                    checked.Add(treeView1.Nodes(i).Nodes(j))
                Next
            End If
        Next

        lockCheckList = False
    End Sub

    Private Sub treeView1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles treeView1.AfterSelect
        MyApplication.host.ribbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.Active

        ' scan节点
        Dim raw As Task.Raw = CurrentRawFile
        Dim scanId As String = e.Node.Text

        Call MyApplication.host.mzkitTool.showSpectrum(scanId, raw)
        Call MyApplication.host.mzkitTool.ShowPage()
    End Sub

    Private Sub CollapseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CollapseToolStripMenuItem.Click
        Dim currentNode = treeView1.SelectedNode

        If currentNode Is Nothing Then
            Return
        End If

        If TypeOf currentNode.Tag Is ScanEntry Then
            currentNode.Parent.Collapse()
        Else
            currentNode.Collapse()
        End If
    End Sub

    Private Sub TextBox2_Click(sender As Object, e As EventArgs) Handles TextBox2.Click
        MyApplication.host.showStatusMessage("Input a number for m/z search, or input formula text for precursor ion match!")
    End Sub

    Private Sub ShowTICToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowTICToolStripMenuItem.Click
        Call MyApplication.host.mzkitTool.TIC({CurrentRawFile}, isBPC:=False)
    End Sub

    Private Sub ShowBPCToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowBPCToolStripMenuItem.Click
        Call MyApplication.host.mzkitTool.TIC({CurrentRawFile}, isBPC:=True)
    End Sub

    Friend Sub ShowXICToolStripMenuItem_Click() Handles ShowXICToolStripMenuItem.Click
        Dim ions = GetSelectedNodes.ToArray

        If ions.Length >= 500 Then
            If MessageBox.Show("There are too many ions for create XIC plot, probably you should uncheck some ions for reduce data, continute to procedure?", "Too much data!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = DialogResult.Cancel Then
                MyApplication.host.showStatusMessage("Show XIC plot for too many ions has been cancel!")
                Return
            End If
        End If

        ' scan节点
        Dim raw As Task.Raw = CurrentRawFile
        Dim ppm As Double = MyApplication.host.GetPPMError()
        Dim plotTIC As NamedCollection(Of ChromatogramTick) = MyApplication.mzkitRawViewer.getXICMatrix(raw, treeView1.SelectedNode.Text, ppm, relativeInto:=False)

        If plotTIC.value.IsNullOrEmpty Then
            ' 当前没有选中MS2，但是可以显示选中的XIC
            If ions.Length > 0 Then
            Else
                MyApplication.host.showStatusMessage("No ion was selected for XIC plot...", My.Resources.StatusAnnotations_Warning_32xLG_color)
                Return
            End If
        Else
            Call MyApplication.mzkitRawViewer.showMatrix(plotTIC.value, Name)
        End If

        Call MyApplication.mzkitRawViewer.ShowXIC(ppm, plotTIC, AddressOf GetXICCollection, raw.GetXICMaxYAxis)
    End Sub
End Class