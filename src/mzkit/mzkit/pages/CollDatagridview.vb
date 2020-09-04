﻿Imports System.Windows.Forms
Imports System.Runtime.CompilerServices

'MIT License

'Copyright(c) 2016 Mike Rodrigues De Lima

'Permission Is hereby granted, free Of charge, to any person obtaining a copy
'of this software And associated documentation files (the "Software"), to deal
'in the Software without restriction, including without limitation the rights
'to use, copy, modify, merge, publish, distribute, sublicense, And/Or sell
'copies of the Software, And to permit persons to whom the Software Is
'furnished to do so, subject to the following conditions:

'The above copyright notice And this permission notice shall be included In all
'copies Or substantial portions of the Software.

'THE SOFTWARE Is PROVIDED "AS IS", WITHOUT WARRANTY Of ANY KIND, EXPRESS Or
'IMPLIED, INCLUDING BUT Not LIMITED To THE WARRANTIES Of MERCHANTABILITY,
'FITNESS FOR A PARTICULAR PURPOSE And NONINFRINGEMENT. IN NO EVENT SHALL THE
'AUTHORS Or COPYRIGHT HOLDERS BE LIABLE For ANY CLAIM, DAMAGES Or OTHER
'LIABILITY, WHETHER In AN ACTION Of CONTRACT, TORT Or OTHERWISE, ARISING FROM,
'OUT OF Or IN CONNECTION WITH THE SOFTWARE Or THE USE Or OTHER DEALINGS IN THE
'SOFTWARE.


Namespace cooldatagridview
    Public Module CollDatagridview
        <Extension()>
        Public Sub CoolGrid(ByVal dgv As DataGridView)
            Dim cellStyle As DataGridViewCellStyle = New DataGridViewCellStyle()
            Dim headerStyle As DataGridViewCellStyle = New DataGridViewCellStyle()
            dgv.AllowDrop = True
            dgv.AllowUserToAddRows = False
            dgv.AllowUserToDeleteRows = False
            dgv.AllowUserToResizeColumns = False
            dgv.AllowUserToResizeRows = False
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
            dgv.BackgroundColor = Drawing.Color.White
            cellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            cellStyle.BackColor = Drawing.Color.White
            cellStyle.Font = New Drawing.Font("Microsoft Sans Serif", 10.0F, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Point, 0)
            cellStyle.ForeColor = Drawing.Color.Black
            cellStyle.SelectionBackColor = Drawing.Color.Yellow
            cellStyle.SelectionForeColor = Drawing.Color.Black
            cellStyle.WrapMode = DataGridViewTriState.True
            dgv.DefaultCellStyle = cellStyle
            dgv.ColumnHeadersHeight = 40
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            headerStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            headerStyle.BackColor = Drawing.SystemColors.Window
            headerStyle.Font = New Drawing.Font("Microsoft Sans Serif", 10.0F, Drawing.FontStyle.Bold, Drawing.GraphicsUnit.Point, 0)
            headerStyle.ForeColor = Drawing.Color.Black
            headerStyle.SelectionBackColor = Drawing.Color.White
            headerStyle.SelectionForeColor = Drawing.Color.Black
            headerStyle.WrapMode = DataGridViewTriState.False
            dgv.ColumnHeadersDefaultCellStyle = headerStyle
            dgv.MultiSelect = False
            dgv.ReadOnly = True
            dgv.RowHeadersVisible = False
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            dgv.TabIndex = 0
            dgv.TabStop = False
        End Sub

        <Extension()>
        Public Sub MoveToUp(ByVal dgv As DataGridView)
            If dgv.Rows.Count > 0 Then
                Dim indexUp = dgv.SelectedRows(0).Index

                If indexUp > 0 Then
                    dgv.Rows(indexUp).Selected = False
                    dgv.Rows(indexUp - 1).Selected = True
                End If
            End If
        End Sub

        <Extension()>
        Public Sub MoveToDown(ByVal dgv As DataGridView)
            If dgv.Rows.Count > 0 Then
                Dim indexDown = dgv.SelectedRows(0).Index

                If indexDown >= 0 AndAlso indexDown < dgv.Rows.Count - 1 Then
                    dgv.Rows(indexDown).Selected = False
                    dgv.Rows(indexDown + 1).Selected = True
                End If
            End If
        End Sub

        <Extension()>
        Public Sub HideColumn(ByVal dgv As DataGridView, ByVal columnName As String)
            dgv.Columns(columnName).Visible = False
        End Sub
    End Module
End Namespace