﻿Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports mzkit.My
Imports mzkit.UnFound.Controls

Public Class PlotConfig : Implements ISaveSettings, IPageSettings

    Dim WithEvents colorPicker As New ThemeColorPicker
    Dim WithEvents deleteColorButton As New DropDownButton With {
        .Text = "Delete Color",
        .Enabled = True,
        .BackColor = Color.Silver
    }

    Public Sub LoadSettings() Implements ISaveSettings.LoadSettings
        If Globals.Settings.viewer Is Nothing Then
            Globals.Settings.viewer = New RawFileViewerSettings
        End If
        If Globals.Settings.viewer.colorSet.IsNullOrEmpty Then
            Globals.Settings.viewer.colorSet = Designer.GetColors("scibasic.category31()").Select(Function(a) a.ToHtmlColor).ToArray
        End If

        ListBox1.Items.Clear()

        For Each color As String In Globals.Settings.viewer.colorSet
            ListBox1.Items.Add(color)
        Next
    End Sub

    Public Sub SaveSettings() Implements ISaveSettings.SaveSettings
        Dim colorSet As New List(Of String)

        For i As Integer = 0 To ListBox1.Items.Count - 1
            colorSet.Add(ListBox1.Items(i).ToString)
        Next

        Globals.Settings.viewer.colorSet = colorSet.ToArray
    End Sub

    Public Sub ShowPage() Implements IPageSettings.ShowPage
        Call MyApplication.host.ShowPage(MyApplication.host.mzkitTool)
        Call MyApplication.host.ShowMzkitToolkit()
    End Sub

    Private Sub PlotConfig_Load(sender As Object, e As EventArgs) Handles Me.Load
        Controls.Add(colorPicker)
        Controls.Add(deleteColorButton)

        Dim dropDowns As New DropDownMenu
        Dim deleteAll As New ToolStripMenuItem() With {.Text = "Clear"}

        dropDowns.Items.Add(deleteAll)

        deleteColorButton.Location = New Point(Button1.Location.X, Button1.Location.Y + Button1.Height + 15)
        deleteColorButton.Renderer = DropDownButton.Renderers.Native
        deleteColorButton.DropDownMenu = dropDowns
        dropDowns.DropDownButton = deleteColorButton

        colorPicker.Location = New Point(50, 50)
        ' PictureBox1.BorderStyle = BorderStyle.FixedSingle

        AddHandler colorPicker.ColorSelected, AddressOf selectColor

        AddHandler deleteColorButton.ClickButton, AddressOf Button2_Click
        AddHandler deleteColorButton.DropDownItemClicked, AddressOf clearAllColors
    End Sub

    Private Sub selectColor(sender As Object, e As ColorSelectedArg) Handles colorPicker.ColorSelected
        ' Dim color As Integer() = {e.R, e.G, e.B}

        PictureBox1.BackColor = e.Color

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
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        selected = ListBox1.SelectedItem
        selectedIndex = ListBox1.SelectedIndex
    End Sub

    Dim selected As Object
    Dim selectedIndex As Integer

    Private Sub ListBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles ListBox1.MouseDown
        If Not (ListBox1.SelectedItem Is Nothing) Then
            selected = ListBox1.SelectedItem
            selectedIndex = ListBox1.SelectedIndex
            ListBox1.DoDragDrop(ListBox1.SelectedItem, DragDropEffects.Move)
        End If
    End Sub

    Private Sub ListBox1_DragEnter(sender As Object, e As DragEventArgs) Handles ListBox1.DragEnter
        e.Effect = DragDropEffects.Move
    End Sub

    Private Sub ListBox1_DragDrop(sender As Object, e As DragEventArgs) Handles ListBox1.DragDrop
        Dim Point = ListBox1.PointToClient(New Point(e.X, e.Y))
        Dim Index = ListBox1.IndexFromPoint(Point)
        If (Index < 0) Then Index = ListBox1.Items.Count - 1

        If Index <> selectedIndex Then
            Dim Data = e.Data.GetData(GetType(String))
            ListBox1.Items.Remove(Data)
            ListBox1.Items.Insert(Index, Data)
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Not PictureBox1.BackColor.IsEmpty Then
            ListBox1.Items.Add(PictureBox1.BackColor.ToHtmlColor)
        End If
    End Sub

    Const WidthOfColorBar As Integer = 32

    Private Sub ListBox1_DrawItem(sender As Object, e As DrawItemEventArgs) Handles ListBox1.DrawItem
        If e.Index = -1 Then
            Return
        End If

        Dim senderList As ListBox = sender
        Dim mycolor As String = ListBox1.Items(e.Index).ToString
        Dim brush1 As Brush = New SolidBrush(mycolor.TranslateColor)
        Dim rect As Rectangle = e.Bounds
        e.Graphics.FillRectangle(New SolidBrush(senderList.BackColor), rect)
        Dim highlightcolor As Color = SystemColors.Highlight
        If (e.State = DrawItemState.Focus) _
            Or (e.State = DrawItemState.Selected) _
            Or (e.State = DrawItemState.Selected + DrawItemState.Focus) Then
            e.Graphics.FillRectangle(New SolidBrush(highlightcolor), e.Bounds)
        Else
            e.Graphics.FillRectangle(New SolidBrush(senderList.BackColor), e.Bounds)
        End If
        rect.Width = WidthOfColorBar
        rect.Height -= 4
        rect.X += 2
        rect.Y += 2
        e.Graphics.FillRectangle(brush1, rect)
        e.Graphics.DrawRectangle(Pens.Black, rect)
        e.Graphics.DrawString(mycolor, senderList.Font,
            New SolidBrush(senderList.ForeColor), WidthOfColorBar + 5, rect.Y - 2)
    End Sub

    Private Sub Button2_Click()
        If Not selected Is Nothing Then
            ListBox1.Items.Remove(selected)

            If ListBox1.Items.Count > 0 Then
                ListBox1.SelectedIndex = 0
            End If
        End If
    End Sub

    Private Sub clearAllColors()
        ListBox1.Items.Clear()
    End Sub

    Private Sub ListBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles ListBox1.MouseMove

    End Sub

    Private Sub deleteColorButton_MouseEnter(sender As Object, e As EventArgs) Handles deleteColorButton.MouseEnter
        deleteColorButton.BackColor = Color.White
    End Sub

    Private Sub deleteColorButton_MouseDown(sender As Object, e As MouseEventArgs) Handles deleteColorButton.MouseDown
        deleteColorButton.BackColor = Color.Black
    End Sub

    Private Sub deleteColorButton_MouseLeave(sender As Object, e As EventArgs) Handles deleteColorButton.MouseLeave
        deleteColorButton.BackColor = Color.Silver
    End Sub
End Class
