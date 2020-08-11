﻿Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components
Imports Microsoft.VisualBasic.Text

Public Class ImportsRawData

    ReadOnly source As String
    ReadOnly temp As String
    ReadOnly showProgress As Action(Of String)
    ReadOnly success As Action

    Public ReadOnly Property raw As Raw

    Sub New(file As String, progress As Action(Of String), finished As Action)
        source = file
        temp = App.AppSystemTemp & "/" & file.GetFullPath.MD5 & ".cdf"
        showProgress = progress
        success = finished
        raw = New Raw With {.cache = temp, .source = source}
    End Sub

    Public Sub RunImports()
        If source.ExtensionSuffix("mzxml") Then
            Call importsMzXML()
        Else
            Call importsMzML()
        End If

        Call showProgress("Job Done!")
        Call Thread.Sleep(1500)
        Call success()
    End Sub

    Private Sub importsMzXML()
        Using cache As New CDFWriter(temp, Encodings.UTF8WithoutBOM)
            Dim attrs As attribute()
            Dim data As Double()
            Dim name As String
            Dim nscans As Integer = 0

            For Each scan As mzXML.scan In mzXML.XML.LoadScans(source)
                attrs = {
                    New attribute With {.name = NameOf(scan.msLevel), .type = CDFDataTypes.INT, .value = scan.msLevel}
                }
                data = scan.peaks.Base64Decode(True)
                name = scan.getName
                cache.AddVariable(name, New CDFData With {.numerics = data}, New Dimension With {.name = "m/z-int,scan_" & scan.num, .size = data.Length}, attrs)
                nscans += 1

                Call showProgress(name)
            Next

            cache.GlobalAttributes(New attribute With {.name = NameOf(nscans), .type = CDFDataTypes.INT, .value = nscans})
            raw.scans = nscans

            Call showProgress("Write cache data...")
        End Using
    End Sub

    Private Sub importsMzML()

    End Sub
End Class