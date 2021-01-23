﻿Imports System.ComponentModel
Imports Vip.Notification

Public Class TaskListWindow

    Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        DoubleBuffered = True
        TabText = "Task List"
    End Sub

    Private Sub TaskListWindow_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
        Me.Hide()
    End Sub

    Public Function Add(task As String, content$) As TaskUI
        Return New TaskUI(task, content, Me)
    End Function
End Class

Public Class TaskUI

    Dim window As TaskListWindow
    Dim row As TreeListViewItem
    Dim status As ListViewItem.ListViewSubItem
    Dim progress As ListViewItem.ListViewSubItem

    Dim taskTitle, taskContent As String

    Sub New(task$, content$, list As TaskListWindow)
        row = New TreeListViewItem With {.Text = task, .ImageIndex = 0}
        status = New ListViewItem.ListViewSubItem With {.Text = "Pending", .BackColor = Color.Yellow}
        progress = New ListViewItem.ListViewSubItem With {.Text = "..."}

        row.SubItems.Add(New ListViewItem.ListViewSubItem With {.Text = content})
        row.SubItems.Add(New ListViewItem.ListViewSubItem With {.Text = Now.ToString})
        row.SubItems.Add(status)
        row.SubItems.Add(progress)

        window = list
        window.TreeListView1.Items.Add(row)

        taskTitle = task
        taskContent = content
    End Sub

    Public Sub Running()
        window.Invoke(Sub()
                          status.Text = "Running..."
                          status.BackColor = Color.Green
                      End Sub)
    End Sub

    Public Sub ProgressMessage(message As String)
        window.Invoke(Sub()
                          progress.Text = message
                      End Sub)
    End Sub

    Public Sub Finish()
        Dim message As String = $"{taskTitle} Job Done!{vbCrLf}{taskContent}"

        window.Invoke(Sub()
                          status.Text = "Finished"
                          progress.Text = ""
                          status.BackColor = Color.SkyBlue
                      End Sub)

        Call Alert.ShowSucess(message)
    End Sub
End Class