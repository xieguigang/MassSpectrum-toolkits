﻿#Region "Microsoft.VisualBasic::13636b710d15c7add5bb883831e486eb, src\mzkit\mzkit\pages\PageMzkitTools.vb"

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

' Class PageMzkitTools
' 
'     Sub: applyLevelFilter, Button1_Click, DeleteFileToolStripMenuItem_Click, ExportExactMassSearchTable, ImportsRaw
'          InitializeFileTree, ListBox1_SelectedIndexChanged, missingCacheFile, MS1ToolStripMenuItem_Click, MS2ToolStripMenuItem_Click
'          PageMzkitTools_Load, runMzSearch, SaveFileCache, SaveImageToolStripMenuItem_Click, SaveMatrixToolStripMenuItem_Click
'          SearchFormulaToolStripMenuItem_Click, searchInFileByMz, SearchInFileToolStripMenuItem_Click, setCurrentFile, showSpectrum
'          showStatusMessage, ShowTICToolStripMenuItem_Click, TreeView1_AfterSelect
' 
' /********************************************************************************/

#End Region

Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports RibbonLib.Interop
Imports Task

Public Class PageMzkitTools

    Dim host As frmMain
    Dim status As ToolStripStatusLabel
    Dim RibbonItems As RibbonItems
    Dim matrix As [Variant](Of ms2(), ChromatogramTick())
    Dim matrixName As String

    Sub showStatusMessage(message As String, Optional icon As Image = Nothing)
        host.Invoke(Sub()
                        status.Text = message
                        status.Image = icon
                    End Sub)
    End Sub

    Sub InitializeFileTree()
        If TreeView1.LoadRawFileCache = 0 Then
            showStatusMessage($"It seems that you don't have any raw file opended. You could open raw file through [File] -> [Open Raw File].", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            TreeView1.SelectedNode = TreeView1.Nodes.Item(Scan0)
            setCurrentFile()
        End If
    End Sub

    Private Sub missingCacheFile(raw As Raw)
        MessageBox.Show($"The specific raw data cache is missing!{vbCrLf}{raw.cache.GetFullPath}", "Cache Not Found!", MessageBoxButtons.OK, MessageBoxIcon.Stop)
    End Sub

    Public Sub SaveFileCache()
        Call TreeView1.SaveRawFileCache
    End Sub

    Public Sub ImportsRaw()
        Using file As New OpenFileDialog With {.Filter = "Raw Data|*.mzXML;*.mzML"}
            If file.ShowDialog = DialogResult.OK Then
                Dim progress As New frmTaskProgress() With {.Text = $"Imports raw data [{file.FileName}]"}
                Dim showProgress As Action(Of String) = Sub(text) progress.Invoke(Sub() progress.Label1.Text = text)
                Dim task As New Task.ImportsRawData(file.FileName, showProgress, Sub() Call progress.Invoke(Sub() progress.Close()))
                Dim runTask As New Thread(AddressOf task.RunImports)

                ParentForm.Invoke(Sub() status.Text = "Run Raw Data Imports")
                progress.Label2.Text = progress.Text

                Call runTask.Start()
                Call progress.ShowDialog()

                'Call New frmRawViewer() With {
                '    .MdiParent = Me,
                '    .Text = file.FileName,
                '    .rawFile = task.raw
                '}.Show()
                Call TreeView1.addRawFile(task.raw)
                Call ParentForm.Invoke(Sub() status.Text = "Ready!")
            End If
        End Using
    End Sub

    Private Sub TreeView1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles TreeView1.AfterSelect
        If TypeOf e.Node.Tag Is Task.Raw Then
            ' 原始文件节点
        ElseIf TypeOf e.Node.Tag Is ms2() Then
            ' TIC 图绘制
            Dim raw = DirectCast(e.Node.Tag, ms2())
            Dim selects = TreeView1.CurrentRawFile.raw
            Dim TIC As New NamedCollection(Of ChromatogramTick) With {
                .name = $"m/z {raw.Select(Function(m) m.mz).Min.ToString("F3")} - {raw.Select(Function(m) m.mz).Max.ToString("F3")}",
                .value = raw _
                    .Select(Function(a)
                                Return New ChromatogramTick With {
                                    .Time = Val(a.Annotation),
                                    .Intensity = a.intensity
                                }
                            End Function) _
                    .ToArray
            }
            Dim maxY As Double = selects.scans.Select(Function(a) a.intensity).Max

            TIC.value = {
                New ChromatogramTick With {.Time = selects.rtmin},
                New ChromatogramTick With {.Time = selects.rtmax}
            }.JoinIterates(TIC.value) _
             .OrderBy(Function(c) c.Time) _
             .ToArray
            showMatrix(TIC.value, TIC.name)
            PictureBox1.BackgroundImage = TIC.TICplot(intensityMax:=maxY).AsGDIImage
        ElseIf e.Node.Tag Is Nothing AndAlso e.Node.Text = "TIC" Then
            Dim raw = TreeView1.CurrentRawFile.raw
            Dim TIC As New NamedCollection(Of ChromatogramTick) With {
                .name = "TIC",
                .value = raw.scans _
                    .Where(Function(a) a.mz = 0R) _
                    .Select(Function(m)
                                Return New ChromatogramTick With {.Time = m.rt, .Intensity = m.intensity}
                            End Function) _
                    .ToArray
            }

            TIC.value = {
                New ChromatogramTick With {.Time = raw.rtmin},
                New ChromatogramTick With {.Time = raw.rtmax}
            }.JoinIterates(TIC.value) _
             .OrderBy(Function(c) c.Time) _
             .ToArray
            showMatrix(TIC.value, TIC.name)
            PictureBox1.BackgroundImage = TIC.TICplot.AsGDIImage
        Else
            ' scan节点
            Dim raw As Task.Raw = e.Node.Parent.Tag
            Dim scanId As String = e.Node.Text

            Call showSpectrum(scanId, raw)
        End If

        Call setCurrentFile()
    End Sub

    Private Sub PageMzkitTools_Load(sender As Object, e As EventArgs) Handles Me.Load
        host = DirectCast(ParentForm, frmMain)
        status = host.ToolStripStatusLabel1
        RibbonItems = host.ribbonItems

        Call InitializeFileTree()
    End Sub

    Dim currentMatrix As [Variant](Of ms2(), ChromatogramTick())

    Private Sub showSpectrum(scanId As String, raw As Raw)
        Dim scanData As LibraryMatrix

        If raw.cache.FileExists Then
            Using cache As New netCDFReader(raw.cache)
                Dim data As CDFData = cache.getDataVariable(cache.getDataVariableEntry(scanId))
                Dim attrs = cache.getDataVariableEntry(scanId).attributes
                Dim rawData As ms2() = data.numerics.AsMs2.ToArray

                scanData = New LibraryMatrix With {
                    .name = scanId,
                    .centroid = False,
                    .ms2 = rawData.Centroid(Tolerance.DeltaMass(0.1), 0.01).ToArray
                }
                showMatrix(rawData, scanId)

                Dim draw As Image = scanData.MirrorPlot.AsGDIImage

                PropertyGrid1.SelectedObject = New SpectrumProperty(scanId, attrs)
                PropertyGrid1.Refresh()

                PictureBox1.BackgroundImage = draw
            End Using

            TabControl1.SelectedTab = TabPage1
        Else
            Call missingCacheFile(raw)
        End If
    End Sub

    Private Sub setCurrentFile()
        If TreeView1.Nodes.Count = 0 Then
            showStatusMessage("No raw file opened.")
            Return
        End If

        With TreeView1.CurrentRawFile.raw
            Static selectedFile As String

            If selectedFile <> status.Text Then
                selectedFile = $"{ .source.FileName} [{ .numOfScans} scans]"
                showStatusMessage(selectedFile)
                ListBox1.Items.Clear()
            End If

            host.Text = $"M/z Toolkit [ { .source.GetFullPath} ]"
        End With

        If Not TreeView1.CurrentRawFile.raw.cache.FileExists Then
            TreeView1.SelectedNode.ImageIndex = 1
            TreeView1.SelectedNode.SelectedImageIndex = 1
        End If
    End Sub

    Private Sub ShowTICToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowTICToolStripMenuItem.Click
        Dim raw = TreeView1.CurrentRawFile

        ShowTICToolStripMenuItem.Checked = Not ShowTICToolStripMenuItem.Checked

        If raw.raw Is Nothing Then
            Return
        End If

        If ShowTICToolStripMenuItem.Checked Then
            If Not raw.raw.cache.FileExists Then
                Call missingCacheFile(raw.raw)
                Return
            End If

            raw.tree.Nodes.Clear()
            raw.tree.Nodes.Add(New TreeNode("TIC"))

            Using cache As New netCDFReader(raw.raw.cache)
                Dim progress As New frmTaskProgress() With {.Text = $"Reading TIC raw data [{raw.raw.source}]"}
                Dim showProgress As Action(Of String) = Sub(text) progress.Invoke(Sub() progress.Label1.Text = text)
                Dim mzgroups As NamedCollection(Of ms2)() = {}
                Dim runTask As New Thread(
                        Sub()
                            Dim ms1n = raw.raw.scans.Where(Function(a) a.mz = 0R).Count
                            Dim i As i32 = 1
                            Dim allMz As New List(Of ms2)
                            Dim mztemp As ms2()

                            For Each scan In raw.raw.scans
                                If scan.mz = 0 Then
                                    Dim entry = cache.getDataVariableEntry(scan.id)
                                    Dim rt As String = entry.attributes.Where(Function(a) a.name = "retentionTime").FirstOrDefault?.value

                                    mztemp = cache.getDataVariable(entry).numerics.AsMs2.ToArray

                                    For i2 As Integer = 0 To mztemp.Length - 1
                                        mztemp(i2).Annotation = rt
                                    Next

                                    allMz.AddRange(mztemp)
                                    showProgress($"[{++i}/{ms1n}] {scan.id}")
                                End If
                            Next

                            showProgress("Run m/z group....")
                            mzgroups = allMz _
                                .GroupBy(Function(mz) mz.mz, Tolerance.DeltaMass(5)) _
                                .Select(Function(a)
                                            Dim max = a.Select(Function(m) m.intensity).Max

                                            Return New NamedCollection(Of ms2) With {.value = a.value.Where(Function(m) m.intensity / max >= 0.05).OrderBy(Function(m) Val(m.Annotation)).ToArray}
                                        End Function) _
                                .ToArray
                            progress.Invoke(Sub() progress.Close())
                        End Sub)

                showStatusMessage("Run Raw Data Imports")
                progress.Label2.Text = progress.Text

                Call runTask.Start()
                Call progress.ShowDialog()

                For Each mzblock In mzgroups
                    Dim range As New DoubleRange(mzblock.Select(Function(m) m.mz))

                    raw.tree.Nodes.Add(New TreeNode($"m/z {range.Min.ToString("F3")} - {range.Max.ToString("F3")}") With {.Tag = mzblock.ToArray})
                Next

                showStatusMessage("Ready!")
            End Using

            host.Invoke(Sub() RibbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.NotAvailable)
        Else
            Call applyLevelFilter()

            host.Invoke(Sub() RibbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.Active)
        End If
    End Sub

    Private Sub SaveImageToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveImageToolStripMenuItem.Click
        If Not PictureBox1.BackgroundImage Is Nothing Then
            Dim preFileName As String = matrixName.NormalizePathString(alphabetOnly:=False)

            Using file As New SaveFileDialog With {.Filter = "image(*.png)|*.png", .FileName = preFileName & ".png"}
                If file.ShowDialog = DialogResult.OK Then
                    Call PictureBox1.BackgroundImage.SaveAs(file.FileName)
                    Call Process.Start(file.FileName)
                End If
            End Using
        End If
    End Sub

    Private Sub applyLevelFilter()
        Dim raw = TreeView1.CurrentRawFile

        If Not raw.raw Is Nothing Then
            raw.tree.Nodes.Clear()
            raw.tree.addRawFile(raw.raw, MS1ToolStripMenuItem.Checked, MS2ToolStripMenuItem.Checked)
        End If
    End Sub

    Private Sub MS1ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MS1ToolStripMenuItem.Click
        MS1ToolStripMenuItem.Checked = Not MS1ToolStripMenuItem.Checked
        applyLevelFilter()
    End Sub

    Private Sub MS2ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MS2ToolStripMenuItem.Click
        MS2ToolStripMenuItem.Checked = Not MS2ToolStripMenuItem.Checked
        applyLevelFilter()
    End Sub

    Private Sub DeleteFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteFileToolStripMenuItem.Click
        Dim current = TreeView1.CurrentRawFile

        TreeView1.Nodes.Remove(current.tree)
        TreeView1.SaveRawFileCache

        Call setCurrentFile()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TextBox1.Text.StringEmpty Then
            Return
        ElseIf TextBox1.Text.IsNumeric Then
            Call searchInFileByMz(mz:=Val(TextBox1.Text))
        Else
            ' formula
            Dim exact_mass As Double = Math.EvaluateFormula(TextBox1.Text)
            Dim ppm As Double = Val(RibbonItems.Spinner.DecimalValue)
            Dim raw As Raw = TreeView1.CurrentRawFile.raw

            DataGridView1.Rows.Clear()
            DataGridView1.Columns.Clear()

            showStatusMessage($"Search MS ions for [{TextBox1.Text}] exact_mass={exact_mass} with tolerance error {ppm} ppm")

            DataGridView1.Columns.Add(New DataGridViewLinkColumn With {
                  .AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                  .ValueType = GetType(String),
                  .HeaderText = "scan Id"})

            DataGridView1.Columns.Add(New DataGridViewTextBoxColumn() With {
                  .AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                  .ValueType = GetType(String),
                  .HeaderText = "m/z"})

            DataGridView1.Columns.Add(New DataGridViewTextBoxColumn() With {
                  .AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                  .ValueType = GetType(String),
                  .HeaderText = "rt"})

            DataGridView1.Columns.Add(New DataGridViewTextBoxColumn() With {
                  .AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                  .ValueType = GetType(String),
                  .HeaderText = "rt(min)"})

            DataGridView1.Columns.Add(New DataGridViewTextBoxColumn() With {
                  .AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                  .ValueType = GetType(String),
                  .HeaderText = "intensity"})

            DataGridView1.Columns.Add(New DataGridViewTextBoxColumn() With {
                  .AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                  .ValueType = GetType(String),
                  .HeaderText = "M"})

            DataGridView1.Columns.Add(New DataGridViewTextBoxColumn() With {
                  .AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                  .ValueType = GetType(String),
                  .HeaderText = "adducts"})

            DataGridView1.Columns.Add(New DataGridViewTextBoxColumn() With {
                  .AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                  .ValueType = GetType(String),
                  .HeaderText = "charge"})

            DataGridView1.Columns.Add(New DataGridViewTextBoxColumn() With {
                  .AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                  .ValueType = GetType(String),
                  .HeaderText = "precursor_type"})

            DataGridView1.Columns.Add(New DataGridViewTextBoxColumn() With {
                  .AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                  .ValueType = GetType(String),
                  .HeaderText = "ppm"})

            ' C25H40N4O5
            Dim pos = MzCalculator.EvaluateAll(exact_mass, "+", False).ToArray
            Dim neg = MzCalculator.EvaluateAll(exact_mass, "-", False).ToArray

            For Each scan As ScanEntry In raw.scans
                If scan.polarity > 0 Then
                    For Each mode In pos
                        If PPMmethod.PPM(scan.mz, Val(mode.mz)) <= ppm Then
                            DataGridView1.Rows.Add(
                                scan.id,
                                scan.mz.ToString("F4"),
                                CInt(scan.rt),
                                (scan.rt / 60).ToString("F2"),
                                scan.intensity.ToString("G3"),
                                mode.M,
                                mode.adduct,
                                mode.charge,
                                mode.precursor_type,
                                PPMmethod.PPM(scan.mz, Val(mode.mz)).ToString("F2"))
                        End If
                    Next
                ElseIf scan.polarity < 0 Then
                    For Each mode In neg
                        If PPMmethod.PPM(scan.mz, Val(mode.mz)) <= ppm Then
                            DataGridView1.Rows.Add(
                                scan.id,
                                scan.mz.ToString("F4"),
                                CInt(scan.rt),
                                (scan.rt / 60).ToString("F2"),
                                scan.intensity.ToString("G3"),
                                mode.M,
                                mode.adduct,
                                mode.charge,
                                mode.precursor_type,
                                PPMmethod.PPM(scan.mz, Val(mode.mz)).ToString("F2"))
                        End If
                    Next
                End If
            Next

            TabControl1.SelectedTab = TabPage2

            host.ribbonItems.TabGroupExactMassSearchTools.ContextAvailable = ContextAvailability.Active
        End If
    End Sub

    Public Sub ExportExactMassSearchTable()
        Call DataGridView1.SaveDataGrid
    End Sub

    Private Sub searchInFileByMz(mz As Double)
        Dim ppm As Double = Val(RibbonItems.Spinner.DecimalValue)
        Dim raw = TreeView1.CurrentRawFile.raw
        Dim ms2Hits = raw.scans.Where(Function(m) PPMmethod.PPM(m.mz, mz) <= ppm).ToArray

        ListBox1.Items.Clear()

        For Each hit As ScanEntry In ms2Hits
            ListBox1.Items.Add(hit)
        Next

        If ms2Hits.Length = 0 Then
            Label2.Text = "no hits!"
            MessageBox.Show($"Sorry, no hits was found for m/z={mz} with tolerance {ppm}ppm...", "No hits found!", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            Label2.Text = $"{ms2Hits.Length} ms2 hits for m/z={mz} with tolerance {ppm}ppm"
        End If

        TabControl2.SelectedIndex = 1
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        Dim scanId As ScanEntry = ListBox1.SelectedItem
        Dim raw = TreeView1.CurrentRawFile.raw

        Call showSpectrum(scanId.id, raw)
    End Sub

    Private Sub SearchInFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SearchInFileToolStripMenuItem.Click
        Call runMzSearch(Sub(mz) Call searchInFileByMz(mz))
    End Sub

    Private Sub SaveMatrixToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveMatrixToolStripMenuItem.Click
        If matrix Is Nothing Then
            Return
        End If
        If matrix Like GetType(ms2()) Then
            Using file As New SaveFileDialog() With {.Filter = "Excel Table(*.xls)|*.xls", .FileName = matrixName.NormalizePathString(False)}
                If file.ShowDialog = DialogResult.OK Then
                    Call matrix.TryCast(Of ms2()).SaveTo(file.FileName)
                End If
            End Using
        ElseIf matrix Like GetType(ChromatogramTick()) Then
            Using file As New SaveFileDialog() With {.Filter = "Excel Table(*.xls)|*.xls", .FileName = matrixName.NormalizePathString(False)}
                If file.ShowDialog = DialogResult.OK Then
                    Call matrix.TryCast(Of ChromatogramTick()).SaveTo(file.FileName)
                End If
            End Using
        End If
    End Sub

    Private Sub runMzSearch(searchAction As Action(Of Double))
        If Not ShowTICToolStripMenuItem.Checked Then
            Dim current = TreeView1.CurrentRawFile
            Dim node = TreeView1.SelectedNode

            If Not node Is Nothing AndAlso current.raw.cache.FileExists Then
                Dim mz = current.raw.scans.Where(Function(scan) scan.id = node.Text).FirstOrDefault

                If Not mz Is Nothing AndAlso mz.mz > 0 Then
                    Call searchAction(mz.mz)
                End If
            End If
        End If
    End Sub

    Private Sub SearchFormulaToolStripMenuItem_Click(sender As Object, e As EventArgs) ' Handles SearchFormulaToolStripMenuItem.Click
        If Not ShowTICToolStripMenuItem.Checked Then
            Dim current = TreeView1.CurrentRawFile
            Dim node = TreeView1.SelectedNode

            If Not node Is Nothing AndAlso current.raw.cache.FileExists Then
                Dim mz = current.raw.scans.Where(Function(scan) scan.id = node.Text).FirstOrDefault

                If Not mz Is Nothing AndAlso mz.mz > 0 Then
                    Dim charge As Double = mz.charge
                    Dim ionMode As Integer = mz.polarity

                    If charge = 0 Then
                        charge = 1
                    End If

                    host.mzkitSearch.doMzSearch(mz.mz, charge, ionMode)
                    host.ShowPage(host.mzkitSearch)
                End If
            End If
        End If
    End Sub

    Private Sub MolecularNetworkingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MolecularNetworkingToolStripMenuItem.Click
        If TreeView1.CurrentRawFile.raw Is Nothing Then
            Return
        End If

        Dim raw As Raw = TreeView1.CurrentRawFile.raw
        Dim progress As New frmTaskProgress
        Dim runTask As New Thread(
            Sub()
                ' Dim tree As New SpectrumTreeCluster(SpectrumTreeCluster.SSMCompares(Tolerance.DeltaMass(0.3), 0.75, 0.4), showReport:=False)
                Dim run As New List(Of PeakMs2)

                progress.Invoke(Sub() progress.Label1.Text = "loading cache ms2 scan data...")

                Using cache As New netCDFReader(raw.cache)
                    For Each scan In raw.scans.Where(Function(s) s.mz > 0)
                        run += New PeakMs2 With {
                            .rt = scan.rt,
                            .mz = scan.mz,
                            .lib_guid = $"M{CInt(.mz)}T{CInt(.rt)}",
                            .mzInto = cache.getDataVariable(scan.id).numerics.AsMs2.ToArray.Centroid(Tolerance.DeltaMass(0.3)).ToArray
                        }

                        progress.Invoke(Sub() progress.Label2.Text = scan.id)
                    Next
                End Using

                progress.Invoke(Sub() progress.Label2.Text = "run molecular networking....")

                ' Call tree.doCluster(run)
                Dim net = MoleculeNetworking.CreateMatrix(run, 0.6, Tolerance.DeltaMass(0.3), Sub(msg) progress.Invoke(Sub() progress.Label1.Text = msg)).ToArray

                progress.Invoke(Sub() progress.Label1.Text = "run family clustering....")

                Dim clusters = net.ToKMeansModels.Kmeans(expected:=10, debug:=False)

                progress.Invoke(Sub() progress.Label1.Text = "initialize result output...")

                host.Invoke(Sub()
                                Call host.mzkitMNtools.loadNetwork(clusters)
                                Call host.ShowPage(host.mzkitMNtools)
                            End Sub)

                progress.Invoke(Sub() progress.Close())
            End Sub)
        runTask.Start()
        progress.ShowDialog()
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        If e.ColumnIndex = Scan0 AndAlso e.RowIndex >= 0 Then
            Dim scanId As String = DataGridView1.Rows(e.RowIndex).Cells(0).Value?.ToString

            If Not scanId.StringEmpty Then
                Call showSpectrum(scanId, TreeView1.CurrentRawFile.raw)
            End If
        End If
    End Sub

    Private Sub CustomToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CustomToolStripMenuItem.Click
        host.mzkitSearch.ComboBox1.SelectedIndex = 0
        SearchFormulaToolStripMenuItem_Click(sender, e)
    End Sub

    Private Sub DefaultToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DefaultToolStripMenuItem.Click
        host.mzkitSearch.ComboBox1.SelectedIndex = 1
        SearchFormulaToolStripMenuItem_Click(sender, e)
    End Sub

    Private Sub SmallMoleculeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SmallMoleculeToolStripMenuItem.Click
        host.mzkitSearch.ComboBox1.SelectedIndex = 2
        SearchFormulaToolStripMenuItem_Click(sender, e)
    End Sub

    Private Sub NatureProductToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NatureProductToolStripMenuItem.Click
        host.mzkitSearch.ComboBox1.SelectedIndex = 3
        SearchFormulaToolStripMenuItem_Click(sender, e)
    End Sub

    Private Sub GeneralFlavoneToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GeneralFlavoneToolStripMenuItem.Click
        host.mzkitSearch.ComboBox1.SelectedIndex = 4
        SearchFormulaToolStripMenuItem_Click(sender, e)
    End Sub

    Sub showMatrix(matrix As ms2(), name As String)
        Me.matrix = matrix
        matrixName = name

        DataGridView1.Rows.Clear()
        DataGridView1.Columns.Clear()

        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .HeaderText = "m/z"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .HeaderText = "intensity"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .HeaderText = "relative"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .HeaderText = "annotation"})

        Dim max As Double = matrix.Select(Function(a) a.intensity).Max

        For Each tick In matrix
            tick.quantity = CInt(tick.intensity / max * 100)
            DataGridView1.Rows.Add({tick.mz, tick.intensity, tick.quantity, tick.Annotation})
        Next
    End Sub

    Sub showMatrix(matrix As ChromatogramTick(), name As String)
        Me.matrix = matrix
        matrixName = name

        DataGridView1.Rows.Clear()
        DataGridView1.Columns.Clear()

        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .HeaderText = "time"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .HeaderText = "intensity"})

        For Each tick In matrix
            DataGridView1.Rows.Add({tick.Time, tick.Intensity})
        Next
    End Sub

    Private Sub ShowXICToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowXICToolStripMenuItem.Click
        ' scan节点
        Dim raw As Task.Raw = TreeView1.CurrentRawFile.raw
        Dim scanId As String = TreeView1.SelectedNode.Text
        Dim ms2 As ScanEntry = raw.scans.Where(Function(a) a.id = scanId).FirstOrDefault
        Dim ppm As Double = Val(RibbonItems.Spinner.DecimalValue)
        Dim name As String = $"XIC [m/z={ms2.mz.ToString("F4")}, {ppm}ppm]"

        If ms2 Is Nothing OrElse ms2.mz = 0.0 Then
            host.ToolStripStatusLabel1.Image = My.Resources.StatusAnnotations_Warning_32xLG_color
            host.ToolStripStatusLabel1.Text = "XIC plot is not avaliable for MS1 parent!"
            Return
        Else
            host.ToolStripStatusLabel1.Image = Nothing
            host.ToolStripStatusLabel1.Text = name
        End If

        Dim XIC As ChromatogramTick() = raw.scans _
            .Where(Function(a) PPMmethod.PPM(a.mz, ms2.mz) <= ppm) _
            .Select(Function(a)
                        Return New ChromatogramTick With {
                            .Time = a.rt,
                            .Intensity = a.intensity
                        }
                    End Function) _
            .ToArray
        Dim plotTIC As New NamedCollection(Of ChromatogramTick) With {
            .name = name,
            .value = {
                  New ChromatogramTick With {.Time = raw.rtmin},
                  New ChromatogramTick With {.Time = raw.rtmax}
              }.JoinIterates(XIC) _
               .OrderBy(Function(c) c.Time) _
               .ToArray
        }
        Dim maxY As Double = raw.scans _
            .Where(Function(a) a.mz > 0) _
            .Select(Function(a) a.intensity) _
            .Max

        showMatrix(plotTIC.value, name)

        PictureBox1.BackgroundImage = plotTIC.TICplot(intensityMax:=maxY).AsGDIImage
    End Sub
End Class

