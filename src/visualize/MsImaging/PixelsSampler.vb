﻿Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

Public Class PixelsSampler

    ''' <summary>
    ''' pixels[x][y]
    ''' </summary>
    ReadOnly col_scans As PixelScan()()
    ReadOnly dims As Size

    Sub New(reader As PixelReader)
        dims = reader.dimension
        col_scans = CreateColumns(reader, dims).ToArray
    End Sub

    Private Shared Iterator Function CreateColumns(reader As PixelReader, dims As Size) As IEnumerable(Of PixelScan())
        For x As Integer = 1 To dims.Width
            Dim column As PixelScan() = New PixelScan(dims.Height - 1) {}

            For y As Integer = 1 To dims.Height
                column(y - 1) = reader.GetPixel(x, y)
            Next

            Yield column
        Next
    End Function

    Public Function GetBlock(o As Point, width As Integer, height As Integer) As IEnumerable(Of PixelScan)
        Return GetBlock(o.X, o.Y, width, height)
    End Function

    Public Iterator Function GetBlock(px As Integer, py As Integer, width As Integer, height As Integer) As IEnumerable(Of PixelScan)
        For x As Integer = px To px + width
            For y As Integer = py To py + height
                If Not col_scans(x)(y) Is Nothing Then
                    Yield col_scans(x)(y)
                End If
            Next
        Next
    End Function

    ''' <summary>
    ''' scale to new size for reduce data size for downstream analysis by
    ''' pixels sampling
    ''' </summary>
    ''' <param name="samplingSize"></param>
    ''' <returns></returns>
    Public Iterator Function Sampling(samplingSize As Size, tolerance As Tolerance) As IEnumerable(Of InMemoryPixel)
        Dim dw As Integer = samplingSize.Width
        Dim dh As Integer = samplingSize.Height

        For x As Integer = 1 To dims.Width Step dw
            For y As Integer = 1 To dims.Height Step dh
                Dim block = GetBlock(x, y, dw, dh) _
                    .Select(Function(p) p.GetMsPipe) _
                    .IteratesALL _
                    .GroupBy(tolerance) _
                    .ToArray
                Dim mz As Double() = block.Select(Function(d) Aggregate p In d Into Average(p.mz)).ToArray
                Dim into As Double() = block.Select(Function(d) Aggregate p In d Into Max(p.intensity)).ToArray
                Dim matrix As ms2() = mz _
                    .Select(Function(mzi, i)
                                Return New ms2 With {
                                    .mz = mzi,
                                    .intensity = into(i)
                                }
                            End Function) _
                    .ToArray

                Yield New InMemoryPixel(x, y, matrix)
            Next
        Next
    End Function
End Class
