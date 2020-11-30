﻿
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.DataReader
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Linq

Namespace mzData.mzWebCache

    Public MustInherit Class ScanPopulator(Of Scan)

        Protected ms1 As ScanMS1
        Protected products As New List(Of ScanMS2)
        Protected trim As LowAbundanceTrimming = New RelativeIntensityCutoff(0.03)
        Protected reader As MsDataReader(Of Scan)
        Protected ms1Err As Tolerance

        Sub New(mzErr As String)
            ms1Err = Tolerance.ParseScript(mzErr)
        End Sub

        Protected MustOverride Function dataReader() As MsDataReader(Of Scan)
        Protected MustOverride Function loadScans(rawfile As String) As IEnumerable(Of Scan)

        Public Iterator Function Load(scans As IEnumerable(Of Scan)) As IEnumerable(Of ScanMS1)
            For Each scan As Scan In scans.Where(Function(s) Not reader.IsEmpty(s))
                Dim scan_time As Double = reader.GetScanTime(scan)
                Dim scan_id As String = reader.GetScanId(scan)
                Dim msms As ms2() = reader.GetMsMs(scan).Centroid(ms1Err, trim).ToArray

                If reader.GetMsLevel(scan) = 1 Then
                    If Not ms1 Is Nothing Then
                        ms1.products = products.ToArray
                        products.Clear()

                        Yield ms1
                    End If

                    ms1 = New ScanMS1 With {
                        .BPC = reader.GetBPC(scan),
                        .TIC = reader.GetTIC(scan),
                        .rt = scan_time,
                        .scan_id = scan_id,
                        .mz = msms.Select(Function(a) a.mz).ToArray,
                        .into = msms.Select(Function(a) a.intensity).ToArray
                    }
                Else
                    Call New ScanMS2 With {
                        .rt = scan_time,
                        .parentMz = reader.GetParentMz(scan),
                        .scan_id = scan_id,
                        .intensity = reader.GetBPC(scan),
                        .mz = msms.Select(Function(a) a.mz).ToArray,
                        .into = msms.Select(Function(a) a.intensity).ToArray
                    }.DoCall(AddressOf products.Add)
                End If
            Next

            ms1.products = products.ToArray
            products.Clear()

            Yield ms1
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Load(rawfile As String) As IEnumerable(Of ScanMS1)
            Return Load(loadScans(rawfile))
        End Function

    End Class
End Namespace