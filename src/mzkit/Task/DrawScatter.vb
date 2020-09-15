﻿Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Windows.Forms
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Public Module DrawScatter

    <Extension>
    Public Function DrawScatter(raw As Raw) As Image
        Dim ms1 As New List(Of ms1_scan)

        Using cache As New netCDFReader(raw.ms1_cache)
            For Each scan In raw.scans.Where(Function(a) a.mz = 0.0)
                Dim data As CDFData = cache.getDataVariable(cache.getDataVariableEntry(scan.id))
                Dim rawData As ms2() = data.numerics.AsMs2.ToArray

                ms1.AddRange(rawData.Centroid(Tolerance.DeltaMass(0.3), New RelativeIntensityCutoff(0.01)).Select(Function(a) New ms1_scan With {.intensity = a.intensity, .mz = a.mz, .scan_time = scan.rt}))
                Application.DoEvents()
            Next
        End Using

        Return RawScatterPlot.Plot(ms1, margin:="padding:200px 600px 200px 200px;", rawfile:=raw.source.FileName).AsGDIImage
    End Function
End Module