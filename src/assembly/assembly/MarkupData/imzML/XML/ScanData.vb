﻿Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace MarkupData.imzML

    ''' <summary>
    ''' a single pixel pointer data in the generated image
    ''' </summary>
    Public Class ScanData

        Public Property totalIon As Double
        Public Property x As Integer
        Public Property y As Integer

        Public Property MzPtr As ibdPtr
        Public Property IntPtr As ibdPtr

        Sub New(scan As spectrum)
            If Not scan.cvParams Is Nothing Then
                totalIon = Double.Parse(scan.cvParams.KeyItem("total ion current")?.value)
            End If

            x = Integer.Parse(scan.scanList.scans(Scan0).cvParams.KeyItem("position x")?.value)
            y = Integer.Parse(scan.scanList.scans(Scan0).cvParams.KeyItem("position y")?.value)
            MzPtr = ibdPtr.ParsePtr(scan.binaryDataArrayList(Scan0))
            IntPtr = ibdPtr.ParsePtr(scan.binaryDataArrayList(1))
        End Sub

        Sub New()
        End Sub

        Sub New(copy As ScanData)
            totalIon = copy.totalIon
            x = copy.x
            y = copy.y
            MzPtr = copy.MzPtr
            IntPtr = copy.IntPtr
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{x}, {y}] {totalIon.ToString("F3")}"
        End Function
    End Class
End Namespace