﻿#Region "Microsoft.VisualBasic::9894fe382d50eb5a740e0d2c23f393ac, pages\dockWindow\documents\frmTargetedQuantification.vb"

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

' Class frmTargetedQuantification
' 
'     Function: createLinear, GetContentTable, GetScans, getStandards, GetTableLevelKeys
'               isValidLinearRow, linearProfileNames
' 
'     Sub: DataGridView1_CellDoubleClick, DataGridView1_CellEndEdit, DataGridView1_DragDrop, DataGridView1_DragEnter, DataGridView1_DragOver
'          DataGridView1_KeyDown, DeleteIonFeatureToolStripMenuItem_Click, deleteProfiles, ExportImageToolStripMenuItem_Click, ExportLinearTableToolStripMenuItem_Click
'          ExportTableToolStripMenuItem_Click, frmTargetedQuantification_FormClosing, frmTargetedQuantification_Load, ImportsLinearReferenceToolStripMenuItem_Click, loadLinearRaw
'          (+2 Overloads) loadLinears, LoadSamplesToolStripMenuItem_Click, reload, reloadProfileNames, SaveAsToolStripMenuItem_Click
'          saveLinearPack, saveLinearsTable, SaveToolStripMenuItem_Click, showIonPeaksTable, showQuanifyTable
'          showRawXTable, ToolStripComboBox2_SelectedIndexChanged, ViewLinearReportToolStripMenuItem_Click
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Content
Imports BioNovoGene.Analytical.MassSpectrometry.Math.GCMS
Imports BioNovoGene.Analytical.MassSpectrometry.Math.GCMS.QuantifyAnalysis
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Data
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Data
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports mzkit.My
Imports RibbonLib.Controls.Events
Imports RibbonLib.Interop
Imports Task
Imports any = Microsoft.VisualBasic.Scripting
Imports Rlist = SMRUCC.Rsharp.Runtime.Internal.Object.list
Imports stdNum = System.Math

Public Class frmTargetedQuantification

    Private Sub frmTargetedQuantification_Load(sender As Object, e As EventArgs) Handles Me.Load
        MyApplication.ribbon.TargetedContex.ContextAvailable = ContextAvailability.Active

        AddHandler MyApplication.ribbon.ImportsLinear.ExecuteEvent, AddressOf loadLinearRaw
        AddHandler MyApplication.ribbon.SaveLinears.ExecuteEvent, AddressOf saveLinearsTable

        TabText = "Targeted Quantification"

        Call reloadProfileNames()
        Call ApplyVsTheme(ToolStrip1, ToolStrip2, ContextMenuStrip1, ContextMenuStrip2, ContextMenuStrip3)
    End Sub

    Private Sub reloadProfileNames()
        cbProfileNameSelector.Items.Clear()

        For Each key As String In linearProfileNames()
            cbProfileNameSelector.Items.Add(key)
        Next
    End Sub

    Private Sub frmTargetedQuantification_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        RemoveHandler MyApplication.ribbon.ImportsLinear.ExecuteEvent, AddressOf loadLinearRaw
        RemoveHandler MyApplication.ribbon.SaveLinears.ExecuteEvent, AddressOf saveLinearsTable
    End Sub

    Sub loadLinearRaw(sender As Object, e As ExecuteEventArgs)
        Call ImportsLinearReferenceToolStripMenuItem_Click(Nothing, Nothing)
    End Sub

    Dim linearPack As LinearPack
    Dim linearFiles As NamedValue(Of String)()
    Dim allFeatures As String()
    Dim isGCMS As Boolean = False

    Sub saveLinearsTable(sender As Object, e As ExecuteEventArgs)
        If linearPack Is Nothing OrElse linearPack.linears.IsNullOrEmpty Then
            Call MyApplication.host.showStatusMessage("No linears for save!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else

        End If
    End Sub

    Private Sub ImportsLinearReferenceToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ImportsLinearReferenceToolStripMenuItem.Click
        Using importsFile As New OpenFileDialog With {
            .Filter = "LC-MSMS / GC-MS Targeted(*.mzML)|*.mzML|GC-MS Targeted(*.cdf)|*.cdf",
            .Multiselect = True,
            .Title = "Select linears"
        }

            If importsFile.ShowDialog = DialogResult.OK Then
                Dim files As NamedValue(Of String)() = ContentTable.StripMaxCommonNames(importsFile.FileNames)
                Dim fakeLevels As Dictionary(Of String, Double)
                Dim directMapName As Boolean = False

                If files.All(Function(name) name.Value.BaseName.IsContentPattern) Then
                    files = files _
                        .Select(Function(file)
                                    Return New NamedValue(Of String) With {
                                        .Name = file.Value.BaseName,
                                        .Value = file.Value,
                                        .Description = file.Description
                                    }
                                End Function) _
                        .ToArray
                    fakeLevels = files _
                        .ToDictionary(Function(file) file.Value.BaseName,
                                      Function(file)
                                          Return file.Value _
                                              .BaseName _
                                              .ParseContent _
                                              .ScaleTo(ContentUnits.ppb) _
                                              .Value
                                      End Function)
                    directMapName = True
                Else
                    fakeLevels = files _
                        .ToDictionary(Function(file) file.Name,
                                      Function()
                                          Return 0.0
                                      End Function)
                End If

                DataGridView1.Rows.Clear()
                DataGridView1.Columns.Clear()

                DataGridView1.Columns.Add(New DataGridViewLinkColumn With {.HeaderText = "Features"})
                DataGridView1.Columns.Add(New DataGridViewComboBoxColumn With {.HeaderText = "IS"})

                For Each file As NamedValue(Of String) In files
                    Call DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = file.Name})

                    If file.Value.ExtensionSuffix("CDF") OrElse RawScanParser.IsSIMData(file.Value) Then
                        isGCMS = True
                        Call MyApplication.host.ShowGCMSSIM(file.Value, isBackground:=False, showExplorer:=False)
                    Else
                        isGCMS = False
                        Call MyApplication.host.ShowMRMIons(file.Value)
                    End If
                Next

                Me.linearFiles = files
                Me.linearPack = New LinearPack With {
                    .reference = New Dictionary(Of String, SampleContentLevels) From {
                        {"n/a", New SampleContentLevels(fakeLevels, directMapName)}
                    }
                }

                If isGCMS Then
                    Call loadGCMSReference(files, directMapName)
                Else
                    Call loadMRMReference(files, directMapName)
                End If
            End If
        End Using
    End Sub

    Private Function LoadGCMSIonLibrary() As QuantifyIon()
        Dim filePath = Globals.Settings.QuantifyIonLibfile

        If filePath.FileLength > 0 Then
            Try
                Using file As Stream = filePath.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                    Return MsgPackSerializer.Deserialize(Of QuantifyIon())(file)
                End Using
            Catch ex As Exception
                Call App.LogException(ex)
                Call MyApplication.host.showStatusMessage("Error while load GCMS reference: " & ex.Message, My.Resources.StatusAnnotations_Warning_32xLG_color)

                Return {}
            End Try
        Else
            Return {}
        End If
    End Function

    Private Sub loadGCMSReference(files As NamedValue(Of String)(), directMapName As Boolean)
        Dim ions As QuantifyIon() = LoadGCMSIonLibrary()
        Dim extract As New SIMIonExtract(ions, {5, 15}, Tolerance.DeltaMass(0.3), 20, 0.65)
        Dim allFeatures = files _
            .Select(Function(file) GetGCMSFeatures(file, extract)) _
            .IteratesALL _
            .GroupBy(Function(p) p.rt, Function(x, y) stdNum.Abs(x - y) <= 15) _
            .ToArray
        Dim contentLevels = linearPack.reference("n/a")

        Me.allFeatures = allFeatures.Select(Function(p) $"{p.value.First.time.Min}/{p.value.First.time.Max}").ToArray

        For Each group As NamedCollection(Of ROI) In allFeatures
            Dim ion As QuantifyIon = extract.FindIon(group.First)
            Dim i As Integer = DataGridView1.Rows.Add(ion.name)
            Dim comboxBox As DataGridViewComboBoxCell = DataGridView1.Rows(i).Cells(1)

            comboxBox.Items.Add("")

            For Each IS_candidate In allFeatures
                comboxBox.Items.Add(extract.FindIon(IS_candidate.First).name)
            Next

            If directMapName Then
                Dim row As DataGridViewRow = DataGridView1.Rows(i)

                For index As Integer = 2 To DataGridView1.Columns.Count - 1
                    row.Cells(index).Value = contentLevels.Content(DataGridView1.Columns(index).HeaderText)
                Next
            End If
        Next
    End Sub

    Private Function GetGCMSFeatures(file As String, extract As SIMIonExtract) As IEnumerable(Of ROI)
        Dim gcms As GCMS.Raw

        If file.ExtensionSuffix("cdf") Then
            gcms = netCDFReader.Open(file).ReadData(showSummary:=False)
        Else
            gcms = mzMLReader.LoadFile(file)
        End If

        Return extract.GetAllFeatures(gcms)
    End Function

    Private Sub loadMRMReference(files As NamedValue(Of String)(), directMapName As Boolean)
        Dim ionsLib As IonLibrary = Globals.LoadIonLibrary
        Dim allFeatures As IonPair() = files _
            .Select(Function(file) file.Value) _
            .GetAllFeatures
        Dim contentLevels = linearPack.reference("n/a")

        Me.allFeatures = allFeatures.Select(AddressOf ionsLib.GetDisplay).ToArray

        For Each ion As IonPair In allFeatures
            Dim refId As String = ionsLib.GetDisplay(ion)
            Dim i As Integer = DataGridView1.Rows.Add(refId)
            Dim comboxBox As DataGridViewComboBoxCell = DataGridView1.Rows(i).Cells(1)

            comboxBox.Items.Add("")

            For Each IS_candidate As IonPair In allFeatures
                comboxBox.Items.Add(ionsLib.GetDisplay(IS_candidate))
            Next

            If directMapName Then
                Dim row As DataGridViewRow = DataGridView1.Rows(i)

                For index As Integer = 2 To DataGridView1.Columns.Count - 1
                    row.Cells(index).Value = contentLevels.Content(DataGridView1.Columns(index).HeaderText)
                Next
            End If
        Next
    End Sub

    Private Function isValidLinearRow(r As DataGridViewRow) As Boolean
        Dim allKeys = linearPack.GetLevelKeys

        For i As Integer = 2 To allKeys.Length - 1 + 2
            If (Not any.ToString(r.Cells(i).Value).IsNumeric) OrElse (any.ToString(r.Cells(i).Value) = "0") Then
                Return False
            End If
        Next

        Return True
    End Function

    Private Sub DeleteIonFeatureToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteIonFeatureToolStripMenuItem.Click

    End Sub

    Private Iterator Function GetTableLevelKeys() As IEnumerable(Of String)
        For i As Integer = 2 To DataGridView1.Columns.Count - 1
            Yield DataGridView1.Columns(i).HeaderText
        Next
    End Function

    Private Iterator Function unifyGetStandards() As IEnumerable(Of Standards)
        Dim levelKeys As String() = GetTableLevelKeys.ToArray
        Dim ionLib As IonLibrary = Globals.LoadIonLibrary
        Dim GCMSIons = LoadGCMSIonLibrary.ToDictionary(Function(i) i.name)

        For Each row As DataGridViewRow In DataGridView1.Rows
            Dim rid As String = any.ToString(row.Cells(0).Value)
            Dim IS_id As String = any.ToString(row.Cells(1).Value)
            Dim levels As New Dictionary(Of String, Double)

            If rid.StringEmpty AndAlso IS_id.StringEmpty Then
                Continue For
            End If

            If isGCMS Then
                Dim ion As QuantifyIon = GCMSIons.GetIon(rid)

                If Not ion Is Nothing Then
                    rid = $"{ion.rt.Min}/{ion.rt.Max}"
                End If

                ion = GCMSIons.GetIon(IS_id)

                If Not ion Is Nothing Then
                    IS_id = $"{ion.rt.Min}/{ion.rt.Max}"
                End If

            Else
                Dim ion As IonPair = ionLib.GetIonByKey(rid)

                If Not ion Is Nothing Then
                    rid = $"{ion.precursor}/{ion.product}"
                ElseIf rid.IsPattern("Ion \[.+?\]") Then
                    rid = rid.GetStackValue("[", "]")
                End If

                ion = ionLib.GetIonByKey(IS_id)

                If Not ion Is Nothing Then
                    IS_id = $"{ion.precursor}/{ion.product}"
                ElseIf IS_id.IsPattern("Ion \[.+?\]") Then
                    IS_id = IS_id.GetStackValue("[", "]")
                End If
            End If

            For i As Integer = 2 To DataGridView1.Columns.Count - 1
                levels(levelKeys(i - 2)) = any.ToString(row.Cells(i).Value).ParseDouble
            Next

            If levels.Values.All(Function(x) x = 0.0) Then
                Continue For
            End If

            Yield New Standards() With {
                .ID = rid,
                .Name = rid,
                .[IS] = IS_id,
                .ISTD = IS_id,
                .Factor = 1,
                .C = levels
            }
        Next
    End Function

    Private Sub SaveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveToolStripMenuItem.Click, ToolStripButton1.Click
        ' Dim ref As New List(Of Standards)(getStandards)
        Dim profileName As String = cbProfileNameSelector.Text

        If profileName.StringEmpty Then
            Call MessageBox.Show("Empty profile name!", "Targeted Quantification Linear", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Dim file As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & $"/mzkit/linears/{profileName}.linearPack"

        Call frmTaskProgress.RunAction(
            Sub()
                Call Me.Invoke(Sub() Call saveLinearPack(profileName, file))
                Call Me.Invoke(Sub() Call reloadProfileNames())
            End Sub, "Save Linear Reference Models", "...")

        Call MyApplication.host.showStatusMessage($"linear model profile '{profileName}' saved!")
    End Sub

    Private Sub SaveAsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveAsToolStripMenuItem.Click
        Using savefile As New SaveFileDialog With {.Title = "Select location for save linear pack data.", .Filter = "Mzkit Linear Models(*.linearPack)|*.linearPack"}
            If savefile.ShowDialog = DialogResult.OK Then
                Call frmTaskProgress.RunAction(
                    Sub()
                        Call Me.Invoke(Sub() saveLinearPack(savefile.FileName.BaseName, savefile.FileName))
                    End Sub, "Save Linear Reference Models", "...")
            End If
        End Using
    End Sub

    Private Function linearProfileNames() As String()
        Return (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & $"/mzkit/linears/") _
            .ListFiles("*.linearPack") _
            .Select(AddressOf BaseName) _
            .ToArray
    End Function

    Private Sub DataGridView1_KeyDown(sender As Object, e As KeyEventArgs) Handles DataGridView1.KeyDown
        If e.KeyCode = Keys.V AndAlso e.Control AndAlso Clipboard.ContainsText Then
            Call DataGridView1.PasteTextData()
        End If
    End Sub

    Private Sub loadMRMLinears()
        Dim ionLib As IonLibrary = Globals.LoadIonLibrary

        DataGridView1.Rows.Clear()
        DataGridView1.Columns.Clear()

        DataGridView1.Columns.Add(New DataGridViewLinkColumn With {.HeaderText = "Features"})
        DataGridView1.Columns.Add(New DataGridViewComboBoxColumn With {.HeaderText = "IS"})

        Dim levelKeys As String() = linearPack.GetLevelKeys

        For Each level As String In levelKeys
            DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = level})
        Next

        Dim islist As String() = linearPack.IS _
            .Select(Function(i)
                        Dim ionpairtext = i.ID.Split("/"c)
                        Dim ionpair As New IonPair With {.precursor = ionpairtext(0), .product = ionpairtext(1)}

                        Return ionLib.GetDisplay(ionpair)
                    End Function) _
            .ToArray

        allFeatures = islist

        For Each linear As KeyValuePair(Of String, SampleContentLevels) In linearPack.reference
            Dim ionpairtext = linear.Key.Split("/"c)
            Dim ionpair As New IonPair With {.precursor = ionpairtext(0), .product = ionpairtext(1)}
            Dim ionID As String = ionLib.GetDisplay(ionpair)
            Dim [is] As [IS] = linearPack.GetLinear(linear.Key).IS

            If Not [is].ID.StringEmpty Then
                ionpairtext = [is].ID.Split("/"c)
                ionpair = New IonPair With {.precursor = ionpairtext(0), .product = ionpairtext(1)}
                [is].name = ionLib.GetDisplay(ionpair)
            End If

            Dim i As Integer = DataGridView1.Rows.Add(ionID)
            Dim IScandidate As DataGridViewComboBoxCell = DataGridView1.Rows(i).Cells(1)

            For Each id As String In islist
                IScandidate.Items.Add(id)
            Next

            IScandidate.Value = [is].name

            For j As Integer = 0 To levelKeys.Length - 1
                DataGridView1.Rows(i).Cells(j + 2).Value = CStr(linear.Value(levelKeys(j)))
            Next
        Next
    End Sub

    Dim linearEdit As Boolean = False

    Private Sub loadLinears(sender As Object, e As EventArgs) Handles cbProfileNameSelector.SelectedIndexChanged
        If linearEdit AndAlso MessageBox.Show("Current linear profiles has been edited, do you want continute to load new linear profiles data?", "Linear Profile Unsaved", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
            Return
        End If

        If cbProfileNameSelector.SelectedIndex = -1 Then
            Return
        End If

        Dim profileName As String = any.ToString(cbProfileNameSelector.Items(cbProfileNameSelector.SelectedIndex))
        Dim file As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & $"/mzkit/linears/{profileName}.linearPack"

        linearPack = LinearPack.OpenFile(file)
        loadMRMLinears()
    End Sub

    Private Sub reload(sender As Object, e As EventArgs) Handles ToolStripButton3.Click
        Call reloadProfileNames()
        Call loadLinears(Nothing, Nothing)
    End Sub

    Private Function GetContentTable(row As DataGridViewRow) As ContentTable
        Dim ionId As String = any.ToString(row.Cells(0).Value)
        Dim isId As String = any.ToString(row.Cells(1).Value)
        Dim contentLevel As New Dictionary(Of String, Double)

        For Each id As SeqValue(Of String) In GetTableLevelKeys().SeqIterator
            contentLevel(id.value) = any.ToString(row.Cells(id + 2).Value).ParseDouble
        Next

        Dim directMap As Boolean = Me.linearFiles.All(Function(name) name.Value.BaseName.IsContentPattern)
        Dim contentSampleLevel As New SampleContentLevels(contentLevel, directMap)
        Dim ref As New Standards With {
            .C = New Dictionary(Of String, Double),
            .ID = ionId,
            .[IS] = isId,
            .ISTD = isId,
            .Name = ionId
        }
        Dim levels As New Dictionary(Of String, SampleContentLevels) From {{ionId, contentSampleLevel}}
        Dim refs As New Dictionary(Of String, Standards) From {{ionId, ref}}
        Dim ISlist As New Dictionary(Of String, [IS]) From {{isId, New [IS] With {.ID = isId, .name = isId, .CIS = 5}}}

        Return New ContentTable(levels, refs, ISlist)
    End Function

    Dim standardCurve As StandardCurve

    Private Sub DataGridView1_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellDoubleClick
        If e.ColumnIndex <> 0 OrElse e.RowIndex < 0 Then
            Return
        End If

        standardCurve = createLinear(DataGridView1.Rows(e.RowIndex))

        If standardCurve Is Nothing Then
            Return
        End If

        PictureBox1.BackgroundImage = standardCurve _
            .StandardCurves(
                size:="1920,1200",
                name:=$"Linear of {standardCurve.name}",
                margin:="padding: 100px 100px 200px 200px;",
                gridFill:="white"
            ) _
            .AsGDIImage

        Call DataGridView2.Rows.Clear()

        For Each point As ReferencePoint In standardCurve.points
            Call DataGridView2.Rows.Add(point.ID, point.Name, point.AIS, point.Ati, point.cIS, point.Cti, point.Px, point.yfit, point.error, point.variant, point.valid, point.level)
        Next
    End Sub

    ''' <summary>
    ''' unify save linear pack data
    ''' </summary>
    ''' <param name="title"></param>
    ''' <param name="file"></param>
    Private Sub saveLinearPack(title As String, file As String)
        Dim ref As Standards() = unifyGetStandards.ToArray
        Dim linears As New List(Of StandardCurve)
        Dim points As TargetPeakPoint() = Nothing
        Dim refPoints As New List(Of TargetPeakPoint)
        Dim refLevels As New Dictionary(Of String, SampleContentLevels)
        Dim ionLib As IonLibrary = Globals.LoadIonLibrary
        Dim GCMSIons = LoadGCMSIonLibrary.ToDictionary(Function(i) i.name)
        Dim directMap As Boolean = ref(Scan0).C.Keys.All(Function(name) name.IsContentPattern)

        For Each i As Standards In ref
            refLevels(i.ID) = New SampleContentLevels(i.C, directMap:=directMap)
        Next

        For Each row As DataGridViewRow In DataGridView1.Rows
            If isValidLinearRow(row) Then
                Dim line = createLinear(row, points)

                If Not line Is Nothing Then
                    linears.Add(line)
                    refPoints.AddRange(points)
                End If
            End If
        Next

        refPoints = refPoints _
            .GroupBy(Function(p) $"{p.SampleName}\{p.Name}") _
            .Select(Function(pg) pg.First) _
            .AsList

        If isGCMS Then
            Dim ion As QuantifyIon

            For Each point As TargetPeakPoint In refPoints
                ion = GCMSIons.GetIon(point.Name)

                If Not ion Is Nothing Then
                    point.Name = $"{ion.rt.Min}/{ion.rt.Max}"
                End If
            Next

            For Each line As StandardCurve In linears
                ion = GCMSIons.GetIon(line.name)

                If Not ion Is Nothing Then
                    line.name = $"{ion.rt.Min}/{ion.rt.Max}"
                End If

                If Not line.IS Is Nothing AndAlso Not line.IS.ID.StringEmpty Then
                    ion = GCMSIons.GetIon(line.IS.ID)

                    If Not ion Is Nothing Then
                        line.IS.ID = $"{ion.rt.Min}/{ion.rt.Max}"
                    End If

                    line.IS.name = line.IS.ID
                End If
            Next
        Else
            Dim ion As IonPair

            For Each point As TargetPeakPoint In refPoints
                ion = ionLib.GetIonByKey(point.Name)
                point.Name = $"{ion.precursor}/{ion.product}"
            Next

            For Each line As StandardCurve In linears
                ion = ionLib.GetIonByKey(line.name)
                line.name = $"{ion.precursor}/{ion.product}"

                If Not line.IS Is Nothing AndAlso Not line.IS.ID.StringEmpty Then
                    ion = ionLib.GetIonByKey(line.IS.ID)
                    line.IS.ID = $"{ion.precursor}/{ion.product}"
                    line.IS.name = line.IS.ID
                End If
            Next
        End If

        Dim linearPack As New LinearPack With {
            .linears = linears.ToArray,
            .peakSamples = refPoints.ToArray,
            .time = Now,
            .title = title,
            .reference = refLevels,
            .[IS] = allFeatures _
                .Select(Function(name)
                            If isGCMS Then
                                ' do nothing
                            Else
                                Dim nameIon As IonPair = ionLib.GetIonByKey(name)
                                name = $"{nameIon.precursor}/{nameIon.product}"
                            End If

                            Return New [IS] With {
                                .ID = name,
                                .name = name,
                                .CIS = 5
                            }
                        End Function) _
                .ToArray,
            .targetted = If(isGCMS, TargettedData.SIM, TargettedData.MRM)
        }

        Call linearPack.Write(file)
    End Sub

    ''' <summary>
    ''' unify create linear reference
    ''' </summary>
    ''' <param name="refRow"></param>
    ''' <param name="refPoints"></param>
    ''' <returns></returns>
    Private Function createLinear(refRow As DataGridViewRow, Optional ByRef refPoints As TargetPeakPoint() = Nothing) As StandardCurve
        Dim id As String = any.ToString(refRow.Cells(0).Value)
        Dim isid As String = any.ToString(refRow.Cells(1).Value)
        Dim chr As New List(Of TargetPeakPoint)

        If isGCMS Then
            Dim ionLib = LoadGCMSIonLibrary.ToDictionary(Function(a) a.name)
            Dim quantifyIon = ionLib.GetIon(id)
            Dim quantifyIS = ionLib.GetIon(isid)
            Dim SIMIonExtract As New SIMIonExtract(ionLib.Values, {5, 15}, Tolerance.DeltaMass(0.3), 20, 0.65)

            If linearFiles.IsNullOrEmpty Then
                Call linearPack.peakSamples _
                    .Select(Function(p)
                                Dim t = p.Name.Split("/"c).Select(AddressOf Val).ToArray

                                If stdNum.Abs(t(0) - quantifyIon.rt.Min) <= 10 AndAlso stdNum.Abs(t(1) - quantifyIon.rt.Max) <= 10 Then
                                    Return New TargetPeakPoint With {
                                        .Name = quantifyIon.name,
                                        .ChromatogramSummary = p.ChromatogramSummary,
                                        .Peak = p.Peak,
                                        .SampleName = p.SampleName
                                    }
                                ElseIf stdNum.Abs(t(0) - quantifyIS.rt.Min) <= 10 AndAlso stdNum.Abs(t(1) - quantifyIS.rt.Max) <= 10 Then
                                    Return New TargetPeakPoint With {
                                        .Name = quantifyIS.name,
                                        .ChromatogramSummary = p.ChromatogramSummary,
                                        .Peak = p.Peak,
                                        .SampleName = p.SampleName
                                    }
                                Else
                                    Return Nothing
                                End If
                            End Function) _
                    .Where(Function(p) Not p Is Nothing) _
                    .DoCall(AddressOf chr.AddRange)
            Else
                Call SIMIonExtract.LoadSamples(linearFiles, quantifyIon, keyByName:=True).DoCall(AddressOf chr.AddRange)

                If Not isid.StringEmpty Then
                    Call SIMIonExtract.LoadSamples(linearFiles, quantifyIS, keyByName:=True).DoCall(AddressOf chr.AddRange)
                End If
            End If
        Else
            Dim ionLib As IonLibrary = Globals.LoadIonLibrary
            Dim quantifyIon = ionLib.GetIonByKey(id)
            Dim quantifyIS = ionLib.GetIonByKey(isid)
            Dim dadot3 As Tolerance = Tolerance.DeltaMass(0.3)

            If linearFiles.IsNullOrEmpty Then
                Call linearPack.peakSamples _
                    .Select(Function(p)
                                Dim t = p.Name.Split("/"c).Select(AddressOf Val).ToArray

                                If dadot3(t(0), quantifyIon.precursor) AndAlso dadot3(t(1), quantifyIon.product) Then
                                    Return New TargetPeakPoint With {
                                        .Name = quantifyIon.name,
                                        .ChromatogramSummary = p.ChromatogramSummary,
                                        .Peak = p.Peak,
                                        .SampleName = p.SampleName
                                    }
                                ElseIf dadot3(t(0), quantifyIS.precursor) AndAlso dadot3(t(1), quantifyIS.product) Then
                                    Return New TargetPeakPoint With {
                                        .Name = quantifyIS.name,
                                        .ChromatogramSummary = p.ChromatogramSummary,
                                        .Peak = p.Peak,
                                        .SampleName = p.SampleName
                                    }
                                Else
                                    Return Nothing
                                End If
                            End Function) _
                    .Where(Function(p) Not p Is Nothing) _
                    .DoCall(AddressOf chr.AddRange)
            Else
                Call MRMIonExtract.LoadSamples(linearFiles, quantifyIon).DoCall(AddressOf chr.AddRange)

                If Not isid.StringEmpty Then
                    Call MRMIonExtract.LoadSamples(linearFiles, quantifyIS).DoCall(AddressOf chr.AddRange)
                End If
            End If
        End If

        Dim algorithm As New InternalStandardMethod(GetContentTable(refRow), PeakAreaMethods.NetPeakSum)

        refPoints = chr.ToArray

        If chr = 0 Then
            Call MyApplication.host.showStatusMessage($"No sample data was found of ion '{id}'!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return Nothing
        Else
            Return algorithm.ToLinears(chr).First
        End If
    End Function

    Private Sub ExportImageToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportImageToolStripMenuItem.Click
        Using file As New SaveFileDialog With {
            .Title = "Export Standard Curve Image",
            .Filter = "Plot Image(*.png)|*.png"
        }
            If file.ShowDialog = DialogResult.OK Then
                Call PictureBox1.BackgroundImage.SaveAs(file.FileName)
            End If
        End Using
    End Sub

    Private Sub ExportLinearTableToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportLinearTableToolStripMenuItem.Click
        Using file As New SaveFileDialog With {
            .Title = "Export Reference Points",
            .Filter = "Reference Point Table(*.csv)|*.csv"
        }
            If file.ShowDialog = DialogResult.OK Then
                Call standardCurve.points.SaveTo(file.FileName)
            End If
        End Using
    End Sub

    Dim scans As New List(Of QuantifyScan)

    Private Sub LoadSamplesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LoadSamplesToolStripMenuItem.Click
        Using importsFile As New OpenFileDialog With {
            .Filter = "LC-MSMS / GC-MS Targeted(*.mzML)|*.mzML|GC-MS Targeted(*.cdf)|*.cdf",
            .Multiselect = True,
            .Title = "Select linears"
        }
            If importsFile.ShowDialog = DialogResult.OK Then
                Dim files As NamedValue(Of String)() = importsFile.FileNames _
                    .Select(Function(file)
                                Return New NamedValue(Of String) With {
                                    .Name = file.BaseName,
                                    .Value = file
                                }
                            End Function) _
                    .ToArray

                ' add files to viewer
                For Each file As NamedValue(Of String) In files
                    Call MyApplication.host.showStatusMessage($"open raw data file '{file.Value}'...")
                    Call MyApplication.host.OpenFile(file.Value, showDocument:=False)
                    Call Application.DoEvents()
                Next

                ' and then do quantify if the linear is exists
                If Not linearPack Is Nothing Then
                    Call scans.Clear()

                    Dim points As New List(Of TargetPeakPoint)
                    Dim linears As New List(Of StandardCurve)

                    For Each row As DataGridViewRow In DataGridView1.Rows
                        If isValidLinearRow(row) Then
                            Dim ion As IonPair = Nothing
                            Dim ISion As IonPair = Nothing

                            linears.Add(createLinear(row))
                            points.AddRange(MRMIonExtract.LoadSamples(files, ion))

                            If Not ISion Is Nothing Then
                                points.AddRange(MRMIonExtract.LoadSamples(files, ISion))
                            End If
                        End If
                    Next

                    With linears.Where(Function(l) Not l Is Nothing).ToArray
                        For Each file In points.GroupBy(Function(p) p.SampleName)
                            scans.Add(.SampleQuantify(file.ToArray, PeakAreaMethods.SumAll, fileName:=file.Key))
                        Next
                    End With
                Else
                    Call MyApplication.host.showStatusMessage("no linear model for run quantification, just open raw files viewer...", My.Resources.StatusAnnotations_Warning_32xLG_color)
                End If

                ToolStripComboBox2.SelectedIndex = 1

                Call showQuanifyTable()
            End If
        End Using
    End Sub

    Private Sub showQuanifyTable()
        DataGridView3.Rows.Clear()
        DataGridView3.Columns.Clear()

        Dim quantify = scans.Select(Function(q) q.quantify).ToArray
        Dim metaboliteNames = quantify.PropertyNames

        DataGridView3.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Sample Name"})

        For Each col As String In metaboliteNames
            DataGridView3.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = col})
        Next

        For Each sample In quantify
            Dim vec As Object() = New Object() {sample.ID} _
                .JoinIterates(metaboliteNames.Select(Function(name) CObj(sample(name)))) _
                .ToArray

            DataGridView3.Rows.Add(vec)
        Next
    End Sub

    Private Sub showRawXTable()
        DataGridView3.Rows.Clear()
        DataGridView3.Columns.Clear()

        Dim quantify = scans.Select(Function(q) q.rawX).ToArray
        Dim metaboliteNames = quantify.PropertyNames

        DataGridView3.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Sample Name"})

        For Each col As String In metaboliteNames
            DataGridView3.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = col})
        Next

        For Each sample In quantify
            Dim vec As Object() = New Object() {sample.ID} _
                .JoinIterates(metaboliteNames.Select(Function(name) CObj(sample(name)))) _
                .ToArray

            DataGridView3.Rows.Add(vec)
        Next
    End Sub

    Private Sub showIonPeaksTable()
        DataGridView3.Rows.Clear()
        DataGridView3.Columns.Clear()

        Dim quantify As EntityObject() = scans.Select(Function(q) q.ionPeaks).IteratesALL.DataFrame.ToArray
        Dim metaboliteNames = quantify.PropertyNames

        DataGridView3.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Sample Name"})

        For Each col As String In metaboliteNames
            DataGridView3.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = col})
        Next

        For Each sample In quantify
            Dim vec As Object() = New Object() {sample.ID} _
                .JoinIterates(metaboliteNames.Select(Function(name) CObj(sample(name)))) _
                .ToArray

            DataGridView3.Rows.Add(vec)
        Next
    End Sub

    Private Sub DataGridView1_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellEndEdit
        Me.linearEdit = True
    End Sub

    Private Sub deleteProfiles(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        Dim profileName As String = cbProfileNameSelector.Text

        If MessageBox.Show($"Going to delete current linear profile '{cbProfileNameSelector.Text}'?", "Delete current profiles", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
            Return
        Else
            Call (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & $"/mzkit/linears/{profileName}.linearPack").DeleteFile
        End If

        linearEdit = False
        linearFiles = Nothing
        linearPack = Nothing

        DataGridView1.Rows.Clear()
        DataGridView1.Columns.Clear()

        Call reloadProfileNames()
    End Sub

    Private Sub ToolStripComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ToolStripComboBox2.SelectedIndexChanged
        Select Case ToolStripComboBox2.SelectedIndex
            Case 0 : Call showIonPeaksTable()
            Case 1 : Call showQuanifyTable()
            Case 2 : Call showRawXTable()

        End Select
    End Sub

    Private Sub ExportTableToolStripMenuItem_Click() Handles ExportTableToolStripMenuItem.Click, ToolStripButton4.Click
        Call DataGridView3.SaveDataGrid("Export sample result table [%s] success!")
    End Sub

    Private Sub DataGridView1_DragDrop(sender As Object, e As DragEventArgs) Handles DataGridView1.DragDrop
        Dim path As String = CType(e.Data.GetData(DataFormats.FileDrop), String())(Scan0)

        If Not path.ExtensionSuffix("linearpack") Then
            MessageBox.Show($"[{path}] is not a mzkit linear model file...", "Not a linearPack file", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        Else
            cbProfileNameSelector.Text = path.BaseName
            linearPack = LinearPack.OpenFile(path)

            Call loadMRMLinears()
        End If
    End Sub

    Private Sub DataGridView1_DragEnter(sender As Object, e As DragEventArgs) Handles DataGridView1.DragEnter
        e.Effect = DragDropEffects.Copy
    End Sub

    Private Sub DataGridView1_DragOver(sender As Object, e As DragEventArgs) Handles DataGridView1.DragOver
        e.Effect = DragDropEffects.Copy
    End Sub

    Private Iterator Function GetScans() As IEnumerable(Of QuantifyScan)

    End Function

    Private Sub ViewLinearReportToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewLinearReportToolStripMenuItem.Click
        If linearPack Is Nothing OrElse linearPack.linears.IsNullOrEmpty Then
            Call MyApplication.host.showStatusMessage("no linear model was loaded!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        End If

        Dim tempfile As String = App.GetAppSysTempFile(".html", sessionID:=App.PID.ToHexString, "linear_report")
        Dim samples As QuantifyScan() = {}
        Dim ionsRaw As New Rlist With {
            .slots = linearPack.peakSamples _
                .GroupBy(Function(sample) sample.Name) _
                .ToDictionary(Function(ion) ion.Key,
                              Function(ionGroup)
                                  Dim innerList As New Rlist With {
                                      .slots = ionGroup _
                                          .ToDictionary(Function(ion) ion.SampleName,
                                                        Function(ion)
                                                            Return CObj(ion.Peak.ticks)
                                                        End Function)
                                  }

                                  Return CObj(innerList)
                              End Function)
        }

        For Each line In linearPack.linears
            line.linear.ErrorTest = line.points _
                .Select(Function(p)
                            Return CType(New TestPoint With {.X = p.Px, .Y = p.Cti, .Yfit = p.yfit}, IFitError)
                        End Function) _
                .ToArray
        Next

        Call MyApplication.REngine.LoadLibrary("mzkit")
        Call MyApplication.REngine.Evaluate("imports 'Linears' from 'mzkit.quantify';")
        Call MyApplication.REngine.Set("$temp_report", MyApplication.REngine.Invoke("report.dataset", linearPack.linears, samples, Nothing, ionsRaw))
        Call MyApplication.REngine.Invoke("html", MyApplication.REngine("$temp_report"), MyApplication.REngine.globalEnvir).ToString.SaveTo(tempfile)

        Call VisualStudio.ShowDocument(Of frmHtmlViewer)().LoadHtml(tempfile)
    End Sub
End Class
