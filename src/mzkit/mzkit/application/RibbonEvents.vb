﻿Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports mzkit.My
Imports RibbonLib
Imports RibbonLib.Controls.Events
Imports RibbonLib.Interop
Imports Task
Imports WeifenLuo.WinFormsUI.Docking

Module RibbonEvents

    Public ReadOnly Property ribbonItems As RibbonItems

    <Extension>
    Public Sub AddHandlers(ribbonItems As RibbonItems)
        _ribbonItems = ribbonItems

        AddHandler ribbonItems.ButtonExit.ExecuteEvent, AddressOf ExitToolsStripMenuItem_Click
        AddHandler ribbonItems.ButtonOpenRaw.ExecuteEvent, AddressOf WindowModules.OpenFile
        AddHandler ribbonItems.ButtonImportsRawFiles.ExecuteEvent, AddressOf MyApplication.host.ImportsFiles
        AddHandler ribbonItems.ButtonAbout.ExecuteEvent, AddressOf About_Click
        AddHandler ribbonItems.ButtonPageNavBack.ExecuteEvent, AddressOf NavBack_Click
        AddHandler ribbonItems.ButtonNew.ExecuteEvent, AddressOf CreateNewScript

        AddHandler ribbonItems.TweaksImage.ExecuteEvent, AddressOf MyApplication.host.mzkitTool.ShowPlotTweaks
        AddHandler ribbonItems.ShowProperty.ExecuteEvent, AddressOf ShowProperties

        AddHandler ribbonItems.ButtonMzCalculator.ExecuteEvent, Sub(sender, e) Call MyApplication.host.ShowPage(MyApplication.host.mzkitCalculator)
        AddHandler ribbonItems.ButtonSettings.ExecuteEvent, AddressOf ShowSettings
        AddHandler ribbonItems.ButtonMzSearch.ExecuteEvent, Sub(sender, e) Call MyApplication.host.ShowPage(MyApplication.host.mzkitSearch)
        AddHandler ribbonItems.ButtonRsharp.ExecuteEvent, AddressOf showRTerm

        AddHandler ribbonItems.ButtonDropA.ExecuteEvent, Sub(sender, e) MyApplication.host.ShowPage(MyApplication.host.mzkitTool)
        AddHandler ribbonItems.ButtonDropB.ExecuteEvent, Sub(sender, e) MyApplication.host.ShowPage(MyApplication.host.mzkitCalculator)
        AddHandler ribbonItems.ButtonFormulaSearch.ExecuteEvent, Sub(sender, e) MyApplication.host.ShowPage(MyApplication.host.mzkitSearch)
        AddHandler ribbonItems.ButtonDropD.ExecuteEvent, Sub(sender, e) MyApplication.host.ShowPage(MyApplication.host.mzkitMNtools)
        AddHandler ribbonItems.ButtonShowSpectrumSearchPage.ExecuteEvent, Sub(sender, e) Call New frmSpectrumSearch().Show(MyApplication.host.dockPanel)

        AddHandler ribbonItems.ButtonCalculatorExport.ExecuteEvent, Sub(sender, e) Call MyApplication.host.mzkitCalculator.ExportToolStripMenuItem_Click()
        AddHandler ribbonItems.ButtonExactMassSearchExport.ExecuteEvent, Sub(sender, e) Call MyApplication.host.mzkitTool.ExportExactMassSearchTable()
        AddHandler ribbonItems.ButtonSave.ExecuteEvent, Sub(sender, e) Call MyApplication.host.saveCurrentFile()
        AddHandler ribbonItems.ButtonNetworkExport.ExecuteEvent, Sub(sender, e) Call MyApplication.host.mzkitMNtools.saveNetwork()
        AddHandler ribbonItems.ButtonFormulaSearchExport.ExecuteEvent, Sub(sender, e) Call MyApplication.host.mzkitSearch.SaveSearchResultTable()

        AddHandler ribbonItems.ButtonBioDeep.ExecuteEvent, Sub(sender, e) Call Process.Start("http://www.biodeep.cn/")
        AddHandler ribbonItems.ButtonLicense.ExecuteEvent, Sub(sender, e) Call New frmLicense().ShowDialog()

        AddHandler ribbonItems.ButtonExportImage.ExecuteEvent, Sub(sender, e) Call MyApplication.host.mzkitTool.SaveImageToolStripMenuItem_Click()
        AddHandler ribbonItems.ButtonExportMatrix.ExecuteEvent, Sub(sender, e) Call MyApplication.host.mzkitTool.SaveMatrixToolStripMenuItem_Click()

        AddHandler ribbonItems.ButtonLayout1.ExecuteEvent, Sub(sender, e) Call MyApplication.host.mzkitTool.SaveImageToolStripMenuItem_Click()
        AddHandler ribbonItems.ButtonLayout2.ExecuteEvent, Sub(sender, e) Call MyApplication.host.mzkitTool.SaveMatrixToolStripMenuItem_Click()

        AddHandler ribbonItems.ButtonShowStartPage.ExecuteEvent, AddressOf showStartPage
        AddHandler ribbonItems.ButtonShowLogWindow.ExecuteEvent, AddressOf showLoggingWindow

        AddHandler ribbonItems.ButtonShowExplorer.ExecuteEvent, AddressOf ShowExplorer
        AddHandler ribbonItems.ButtonShowSearchList.ExecuteEvent, AddressOf ShowSearchList
        AddHandler ribbonItems.ButtonShowProperties.ExecuteEvent, AddressOf ShowProperties

        AddHandler ribbonItems.ButtonShowPlotViewer.ExecuteEvent, Sub(sender, e) Call MyApplication.host.mzkitTool.ShowTabPage(MyApplication.host.mzkitTool.TabPage5)
        AddHandler ribbonItems.ButtonShowMatrixViewer.ExecuteEvent, Sub(sender, e) Call MyApplication.host.mzkitTool.ShowTabPage(MyApplication.host.mzkitTool.TabPage6)
        AddHandler ribbonItems.ButtonNetworkRender.ExecuteEvent, Sub(sender, e) Call MyApplication.host.mzkitMNtools.RenderNetwork()
        AddHandler ribbonItems.ButtonRefreshNetwork.ExecuteEvent, Sub(sender, e) Call MyApplication.host.mzkitMNtools.RefreshNetwork()

        AddHandler ribbonItems.ButtonRunScript.ExecuteEvent, AddressOf RunCurrentScript
        AddHandler ribbonItems.ButtonSaveScript.ExecuteEvent, AddressOf MyApplication.host.saveCurrentScript

        AddHandler ribbonItems.HelpButton.ExecuteEvent, AddressOf showHelp

        AddHandler ribbonItems.ButtonTIC.ExecuteEvent, Sub(sender, e) Call MyApplication.host.mzkitTool.TIC(isBPC:=False)
        AddHandler ribbonItems.ButtonBPC.ExecuteEvent, Sub(sender, e) Call MyApplication.host.mzkitTool.TIC(isBPC:=True)
        AddHandler ribbonItems.ButtonXIC.ExecuteEvent, AddressOf WindowModules.rawFeaturesList.ShowXICToolStripMenuItem_Click

        AddHandler ribbonItems.ButtonResetLayout.ExecuteEvent, AddressOf resetLayout

        AddHandler ribbonItems.RecentItems.ExecuteEvent, AddressOf _recentItems_ExecuteEvent
        AddHandler ribbonItems.ButtonMsImaging.ExecuteEvent, AddressOf showMsImaging
        AddHandler ribbonItems.ButtonOpenMSIRaw.ExecuteEvent, AddressOf OpenMSIRaw
        AddHandler ribbonItems.ButtonMsDemo.ExecuteEvent, Sub() WindowModules.msDemo.ShowPage()
        AddHandler ribbonItems.Targeted.ExecuteEvent, Sub() Call ConnectToBioDeep.OpenAdvancedFunction(AddressOf VisualStudio.ShowSingleDocument(Of frmTargetedQuantification))

        AddHandler ribbonItems.MRMLibrary.ExecuteEvent, Sub() Call VisualStudio.ShowSingleDocument(Of frmMRMLibrary)(Nothing)
        AddHandler ribbonItems.QuantifyIons.ExecuteEvent, Sub() Call VisualStudio.ShowSingleDocument(Of frmQuantifyIons)(Nothing)

        AddHandler ribbonItems.LogInBioDeep.ExecuteEvent, Sub() Call New frmLogin().ShowDialog()

        AddHandler ribbonItems.ButtonInstallMzkitPackage.ExecuteEvent, AddressOf MyApplication.InstallPackageRelease
        AddHandler ribbonItems.ShowGCMSExplorer.ExecuteEvent, Sub() Call VisualStudio.Dock(WindowModules.GCMSPeaks, DockState.DockLeft)
        AddHandler ribbonItems.ShowMRMExplorer.ExecuteEvent, Sub() Call VisualStudio.Dock(WindowModules.MRMIons, DockState.DockLeft)

        AddHandler ribbonItems.Tutorials.ExecuteEvent, Sub() Call VisualStudio.ShowSingleDocument(Of frmVideoList)()

        AddHandler ribbonItems.AdjustParameters.ExecuteEvent, Sub() Call VisualStudio.Dock(WindowModules.parametersTool, DockState.DockRight)
    End Sub

    Public Sub OpenMSIRaw()
        Using file As New OpenFileDialog() With {
            .Filter = "Thermo Raw(*.raw)|*.raw|Imaging mzML(*.imzML)|*.imzML",
            .Title = "Open MS-imaging Raw Data File"
        }
            If file.ShowDialog = DialogResult.OK Then
                Call showMsImaging()

                Select Case file.FileName.ExtensionSuffix.ToLower
                    Case "raw" : Call WindowModules.viewer.loadRaw(file.FileName)
                    Case "mzml" : Call WindowModules.viewer.loadmzML(file.FileName)
                    Case "imzml" : Call WindowModules.viewer.loadimzML(file.FileName)
                End Select
            End If
        End Using
    End Sub

    Public Sub showMsImaging()
        Dim dockPanel = VisualStudio.DockPanel

        Call WindowModules.viewer.Show(dockPanel)
        Call WindowModules.msImageParameters.Show(dockPanel)

        WindowModules.msImageParameters.DockState = DockState.DockLeft
    End Sub

    Private Sub resetLayout()
        WindowModules.fileExplorer.DockState = DockState.DockLeft
        WindowModules.rawFeaturesList.DockState = DockState.DockLeftAutoHide
        WindowModules.output.DockState = DockState.DockBottomAutoHide
        WindowModules.propertyWin.DockState = DockState.DockRightAutoHide
    End Sub

    Private Sub _recentItems_ExecuteEvent(sender As Object, e As ExecuteEventArgs)
        If e.Key.PropertyKey = RibbonProperties.RecentItems Then
            ' go over recent items
            Dim objectArray() As Object = CType(e.CurrentValue.PropVariant.Value, Object())
            For i As Integer = 0 To objectArray.Length - 1
                Dim propertySet As IUISimplePropertySet = TryCast(objectArray(i), IUISimplePropertySet)

                If propertySet IsNot Nothing Then
                    Dim propLabel As PropVariant
                    propertySet.GetValue(RibbonProperties.Label, propLabel)
                    Dim label As String = CStr(propLabel.Value)

                    Dim propLabelDescription As PropVariant
                    propertySet.GetValue(RibbonProperties.LabelDescription, propLabelDescription)
                    Dim labelDescription As String = CStr(propLabelDescription.Value)

                    Dim propPinned As PropVariant
                    propertySet.GetValue(RibbonProperties.Pinned, propPinned)
                    Dim pinned As Boolean = CBool(propPinned.Value)

                    ' update pinned value
                    MyApplication.host.recentItems(i).Pinned = pinned
                End If
            Next i
        ElseIf e.Key.PropertyKey = RibbonProperties.SelectedItem Then
            ' get selected item index
            Dim selectedItem As UInteger = CUInt(e.CurrentValue.PropVariant.Value)

            ' get selected item label
            Dim propLabel As PropVariant
            e.CommandExecutionProperties.GetValue(RibbonProperties.Label, propLabel)
            Dim label As String = CStr(propLabel.Value)
            Dim sourceFile As String = Nothing

            For Each file As String In Globals.Settings.recentFiles.SafeQuery
                If label = file.FileName Then
                    sourceFile = file
                    Exit For
                End If
            Next

            ' get selected item label description
            Dim propLabelDescription As PropVariant
            e.CommandExecutionProperties.GetValue(RibbonProperties.LabelDescription, propLabelDescription)
            Dim labelDescription As String = CStr(propLabelDescription.Value)

            ' get selected item pinned value
            Dim propPinned As PropVariant
            e.CommandExecutionProperties.GetValue(RibbonProperties.Pinned, propPinned)
            Dim pinned As Boolean = CBool(propPinned.Value)

            If label.ExtensionSuffix("R") Then
                If Not sourceFile.FileExists Then
                    MessageBox.Show($"The given R# script file [{label}] is not exists on your file system!", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    Call MyApplication.host.openRscript(sourceFile)
                End If
            Else
                Dim raw As Raw() = Globals.FindRaws(WindowModules.fileExplorer.treeView1, label).ToArray

                If raw Is Nothing Then
                    MessageBox.Show($"The given raw data file [{label}] is not exists on your file system!", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    Call MyApplication.host.mzkitTool.TIC(raw)
                End If
            End If
        End If
    End Sub

    Private Sub _uiCollectionChangedEvent_ChangedEvent(sender As Object, e As UICollectionChangedEventArgs)
        MessageBox.Show("Got ChangedEvent. Action = " & e.Action.ToString())
    End Sub

    Friend nav As New Stack(Of Control)

    Private Sub NavBack_Click(sender As Object, e As ExecuteEventArgs)
        If nav.Count > 0 Then
            Call MyApplication.host.ShowPage(nav.Pop, pushStack:=False)
        End If
    End Sub

    Private Sub About_Click(sender As Object, e As ExecuteEventArgs)
        Call New frmSplashScreen() With {.isAboutScreen = True, .TopMost = True}.Show()
    End Sub

    Friend Sub RunCurrentScript(sender As Object, e As ExecuteEventArgs)
        Dim active = MyApplication.host.dockPanel.ActiveDocument

        If Not active Is Nothing AndAlso TypeOf CObj(active) Is frmRScriptEdit Then
            Dim editor = DirectCast(CObj(active), frmRScriptEdit)
            Dim script As String = editor.ScriptText

            If Not editor.scriptFile.StringEmpty Then
                script.SaveTo(editor.scriptFile)
                script = editor.scriptFile
            End If

            Call MyApplication.ExecuteRScript(script, isFile:=Not editor.scriptFile.StringEmpty, AddressOf WindowModules.output.AppendRoutput)
            Call VisualStudio.Dock(WindowModules.output, DockState.DockBottom)
        End If
    End Sub

    Friend ReadOnly scriptFiles As New List(Of frmRScriptEdit)

    Public Sub CreateNewScript(sender As Object, e As ExecuteEventArgs)
        Dim newScript As New frmRScriptEdit

        newScript.Show(MyApplication.host.dockPanel)
        newScript.DockState = DockState.Document
        newScript.Text = "New R# Script"

        scriptFiles.Add(newScript)

        MyApplication.host.Text = $"BioNovoGene Mzkit [{newScript.Text}]"
    End Sub

    Private Sub showRTerm(sender As Object, e As ExecuteEventArgs)
        WindowModules.RtermPage.Show(MyApplication.host.dockPanel)
        WindowModules.RtermPage.DockState = DockState.Document

        MyApplication.host.Text = $"BioNovoGene Mzkit [{  WindowModules.RtermPage.Text}]"
    End Sub

    Private Sub ShowSettings(sender As Object, e As ExecuteEventArgs)
        WindowModules.settingsPage.Show(MyApplication.host.dockPanel)
        WindowModules.settingsPage.DockState = DockState.Document

        MyApplication.host.Text = $"BioNovoGene Mzkit [{WindowModules.settingsPage.Text}]"
    End Sub

    Private Sub ShowExplorer(sender As Object, e As ExecuteEventArgs)
        WindowModules.fileExplorer.Show(MyApplication.host.dockPanel)
        WindowModules.fileExplorer.DockState = DockState.DockLeft
    End Sub

    Private Sub ShowSearchList(sender As Object, e As ExecuteEventArgs)
        WindowModules.rawFeaturesList.Show(MyApplication.host.dockPanel)
        WindowModules.rawFeaturesList.DockState = DockState.DockLeft
    End Sub

    Private Sub ShowProperties(sender As Object, e As ExecuteEventArgs)
        MyApplication.host.ShowPropertyWindow()
    End Sub

    Private Sub showLoggingWindow(sender As Object, e As ExecuteEventArgs)
        WindowModules.output.Show(MyApplication.host.dockPanel)
        WindowModules.output.DockState = DockState.DockBottom
    End Sub

    Private Sub showStartPage(sender As Object, e As ExecuteEventArgs)
        If Not Globals.CheckFormOpened(WindowModules.startPage) Then
            WindowModules.startPage = New frmStartPage
        End If

        WindowModules.startPage.Show(MyApplication.host.dockPanel)
        WindowModules.startPage.DockState = DockState.Document

        MyApplication.host.Text = $"BioNovoGene Mzkit [{WindowModules.startPage.Text}]"
    End Sub

    Private Sub ExitToolsStripMenuItem_Click(sender As Object, e As ExecuteEventArgs)
        Call MyApplication.host.Close()
    End Sub

    Private Sub showHelp(sender As Object, e As ExecuteEventArgs)
        For Each dir As String In {App.HOME, $"{App.HOME}/docs", $"{App.HOME}/../", $"{App.HOME}/../docs/"}
            If $"{dir}/readme.pdf".FileExists Then
                Call Process.Start($"{dir}/readme.pdf")
                Return
            End If
        Next

        ' try open online page
        Call Process.Start("https://mzkit.org/dist/README.pdf")

        ' Me.showStatusMessage("Manul pdf file is missing...", My.Resources.StatusAnnotations_Warning_32xLG_color)
    End Sub
End Module
