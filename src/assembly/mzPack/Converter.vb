﻿#Region "Microsoft.VisualBasic::ba17996f9f7b02287916da7cbbbe0d48, src\assembly\mzPack\Converter.vb"

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

' Module Converter
' 
'     Function: GetUVScans, LoadMzML, LoadRawFileAuto
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.SignalProcessing

Public Module Converter

    ''' <summary>
    ''' A unify method for load mzpack data from mzXML/mzML raw data file
    ''' </summary>
    ''' <param name="xml">the file path of the raw mzXML/mzML data file.</param>
    ''' <returns></returns>
    Public Function LoadRawFileAuto(xml As String, Optional progress As Action(Of String) = Nothing) As mzPack
        If xml.ExtensionSuffix("mzXML") Then
            Return New mzPack With {
                .MS = New mzXMLScans().Load(xml, progress).ToArray
            }
        ElseIf xml.ExtensionSuffix("mzML") Then
            Return LoadMzML(xml, progress)
        ElseIf xml.ExtensionSuffix("imzML") Then
            Return LoadimzML(xml, progress)
        Else
            Throw New NotImplementedException(xml.ExtensionSuffix)
        End If
    End Function

    Public Function LoadimzML(xml As String, Optional progress As Action(Of String) = Nothing) As mzPack
        Dim scans As New List(Of ScanMS1)
        Dim ibd As New ibdReader(xml.ChangeSuffix("ibd").Open(FileMode.Open, doClear:=False, [readOnly]:=True), Format.Continuous)
        Dim pixel As ScanMS1
        Dim ms As ms2()

        For Each scan As ScanData In imzML.XML.LoadScans(xml)
            ms = ibd.GetMSMS(scan)
            pixel = New ScanMS1 With {
                .meta = New Dictionary(Of String, String) From {
                    {"x", scan.x},
                    {"y", scan.y}
                },
                .TIC = scan.totalIon,
                .scan_id = $"[{scan.x},{scan.y}] totalIon: {scan.totalIon}",
                .mz = ms.Select(Function(m) m.mz).ToArray,
                .into = ms.Select(Function(m) m.intensity).ToArray
            }
            scans.Add(pixel)

            If Not progress Is Nothing Then
                Call progress(pixel.scan_id)
            End If
        Next

        Return New mzPack With {
            .MS = scans.ToArray,
            .source = xml.FileName
        }
    End Function

    Public Function LoadMzML(xml As String, Optional progress As Action(Of String) = Nothing) As mzPack
        Dim UVdetecor As String = ExtractUVData.GetPhotodiodeArrayDetectorInstrumentConfigurationId(xml)
        Dim scanLoader As New mzMLScans
        Dim MS As ScanMS1() = scanLoader.Load(xml, progress).ToArray
        Dim UV As New ChromatogramOverlap
        Dim PDA As New List(Of ChromatogramTick)

        For Each time_scan As GeneralSignal In scanLoader.GetUVScans(UVdetecor)
            Dim scan_time As Double = time_scan.meta!scan_time
            Dim TIC As Double = time_scan.meta!total_ion_current
            Dim scanId As String = $"[{time_scan.meta!scan}] {ExtractUVData.UVScanType} {TIC.ToString("G3")}@{scan_time.ToString("F3")}s"

            PDA += New ChromatogramTick With {
                .Time = scan_time,
                .Intensity = TIC
            }
            UV(scanId) = New DataReader.Chromatogram With {
                .TIC = time_scan.Strength,
                .scan_time = time_scan.Measures,
                .BPC = .TIC
            }

            If Not progress Is Nothing Then
                Call progress(scanId)
            End If
        Next

        Dim PDAPlot As New ChromatogramOverlap

        PDAPlot("PDA") = New DataReader.Chromatogram With {
            .scan_time = PDA.Select(Function(t) t.Time).ToArray,
            .TIC = PDA.Select(Function(t) t.Intensity).ToArray,
            .BPC = .TIC
        }

        Dim otherScanner As New Dictionary(Of String, ChromatogramOverlap)

        If UV.length > 0 Then
            otherScanner(ExtractUVData.UVScanType) = UV
            otherScanner("PDA") = PDAPlot
        End If

        Return New mzPack With {
            .MS = MS,
            .Scanners = otherScanner
        }
    End Function

    <Extension>
    Public Iterator Function GetUVScans(mzpack As mzPack) As IEnumerable(Of UVScan)
        If Not mzpack.Scanners.ContainsKey(ExtractUVData.UVScanType) Then
            Return
        End If

        Dim time_scans As ChromatogramOverlap = mzpack.Scanners(ExtractUVData.UVScanType)
        Dim PDA As DataReader.Chromatogram = mzpack.Scanners!PDA!PDA
        Dim scan As DataReader.Chromatogram
        Dim UV As UVScan

        For Each timeId As SeqValue(Of String) In time_scans.overlaps.Keys.SeqIterator
            scan = time_scans(timeId.value)
            UV = New UVScan With {
                .scan_time = PDA.scan_time(timeId),
                .total_ion_current = PDA.TIC(timeId),
                .wavelength = scan.scan_time,
                .intensity = scan.TIC
            }

            Yield UV
        Next
    End Function
End Module
