﻿#Region "Microsoft.VisualBasic::0305cb816036e58e33a99a51620f3e33, src\mzkit\mzkit\pages\Settings\AppConfig.vb"

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

    ' Class AppConfig
    ' 
    '     Sub: AppConfig_Load, CheckBox1_CheckedChanged, CheckBox2_CheckedChanged, LoadSettings, SaveSettings
    '          ShowPage
    ' 
    ' /********************************************************************************/

#End Region

Imports mzkit.Configuration

Public Class AppConfig : Implements ISaveSettings, IPageSettings

    '  Dim WithEvents colorPicker As New ThemeColorPicker

    Public Sub LoadSettings() Implements ISaveSettings.LoadSettings
        If Globals.Settings.ui Is Nothing Then
            Globals.Settings.ui = New UISettings With {.rememberWindowsLocation = True}
        End If

        CheckBox1.Checked = Globals.Settings.ui.rememberWindowsLocation
        CheckBox2.Checked = Globals.Settings.ui.rememberLayouts

        'If Globals.Settings.ui.background.IsNullOrEmpty Then
        '    Dim colors As RibbonColors = MyApplication.host.Ribbon1.GetColors

        '    PictureBox1.BackColor = colors.BackgroundColor
        '    PictureBox2.BackColor = colors.HighlightColor
        '    PictureBox3.BackColor = colors.TextColor

        '    Globals.Settings.ui.background = {colors.BackgroundColor.R, colors.BackgroundColor.G, colors.BackgroundColor.B}
        '    Globals.Settings.ui.highlight = {colors.HighlightColor.R, colors.HighlightColor.G, colors.HighlightColor.B}
        '    Globals.Settings.ui.text = {colors.TextColor.R, colors.TextColor.G, colors.TextColor.B}
        'End If
    End Sub

    Public Sub SaveSettings() Implements ISaveSettings.SaveSettings
        Globals.Settings.Save()
    End Sub

    Public Sub ShowPage() Implements IPageSettings.ShowPage

    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        Globals.Settings.ui.rememberWindowsLocation = CheckBox1.Checked
    End Sub

    Private Sub AppConfig_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Controls.Add(colorPicker)

        'colorPicker.Location = New Point(50, 50)
        '' PictureBox1.BorderStyle = BorderStyle.FixedSingle

        'AddHandler colorPicker.ColorSelected, AddressOf selectColor
    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        Globals.Settings.ui.rememberLayouts = CheckBox2.Checked
    End Sub

    ' Private Sub selectColor(sender As Object, e As ColorSelectedArg) Handles colorPicker.ColorSelected
    'Dim color As Integer() = {e.R, e.G, e.B}

    'If PictureBox1.BorderStyle = BorderStyle.FixedSingle Then
    '    Globals.Settings.ui.background = color
    '    PictureBox1.BackColor = e.Color
    'ElseIf PictureBox2.BorderStyle = BorderStyle.FixedSingle Then
    '    Globals.Settings.ui.highlight = color
    '    PictureBox2.BackColor = e.Color
    'ElseIf PictureBox3.BorderStyle = BorderStyle.FixedSingle Then
    '    Globals.Settings.ui.text = color
    '    PictureBox3.BackColor = e.Color
    'End If

    'Globals.Settings.ui.setColors(MyApplication.host.Ribbon1)
    ' End Sub

    'Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs)
    '    PictureBox1.BorderStyle = BorderStyle.FixedSingle
    '    PictureBox2.BorderStyle = BorderStyle.None
    '    PictureBox3.BorderStyle = BorderStyle.None
    'End Sub

    'Private Sub LinkLabel2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs)
    '    PictureBox1.BorderStyle = BorderStyle.None
    '    PictureBox2.BorderStyle = BorderStyle.FixedSingle
    '    PictureBox3.BorderStyle = BorderStyle.None
    'End Sub

    'Private Sub LinkLabel3_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs)
    '    PictureBox1.BorderStyle = BorderStyle.None
    '    PictureBox2.BorderStyle = BorderStyle.None
    '    PictureBox3.BorderStyle = BorderStyle.FixedSingle
    'End Sub
End Class
