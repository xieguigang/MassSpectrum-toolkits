﻿Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Linq

Public Class frmCalculator

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim exact_mass As Double = Val(TextBox1.Text)

        Call Update(exact_mass, Provider.GetCalculator("+").Values, DataGridView1)
        Call Update(exact_mass, Provider.GetCalculator("-").Values, DataGridView2)
    End Sub

    Overloads Sub Update(exact_mass As Double, mode As IEnumerable(Of MzCalculator), show As DataGridView)
        Dim mzText As String

        Call show.Rows.Clear()

        For Each type As MzCalculator In mode
            mzText = type.CalcMZ(exact_mass)
            mzText = If(Val(mzText) < 0, "n/a", mzText)

            Call show.Rows.Add(type.ToString, type.adducts, type.M, type.charge, mzText)
        Next
    End Sub

    Private Sub ExportToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportToolStripMenuItem.Click
        Using file As New SaveFileDialog With {.Filter = "Excel Table|*.csv"}
            If file.ShowDialog = DialogResult.OK Then
                Dim currentTab As DataGridView = (From ctrl As Control In TabControl1.SelectedTab.Controls Where TypeOf ctrl Is DataGridView).First
                Dim table As New List(Of PrecursorInfo)

                For Each row As DataGridViewRow In currentTab.Rows
                    If row.Cells(0).Value Is Nothing Then
                        Exit For
                    End If

                    Call New PrecursorInfo With {
                        .precursor_type = row.Cells(0).Value.ToString,
                        .adduct = CDbl(row.Cells(1).Value.ToString),
                        .M = CInt(row.Cells(2).Value.ToString),
                        .charge = CDbl(row.Cells(3).Value),
                        .mz = row.Cells(4).Value.ToString
                    }.DoCall(AddressOf table.Add)
                Next

                Call table.SaveTo(file.FileName)
                Call MessageBox.Show($"Export table to [{file.FileName}] success!", "Table Export", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End Using
    End Sub
End Class