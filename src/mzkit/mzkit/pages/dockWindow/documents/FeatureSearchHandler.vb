﻿#Region "Microsoft.VisualBasic::b4de520e0061ad3008e81bf3cda65bf1, src\mzkit\mzkit\pages\dockWindow\documents\FeatureSearchHandler.vb"

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

    ' Module FeatureSearchHandler
    ' 
    '     Function: MatchByFormula
    ' 
    '     Sub: runFormulaMatch, SearchByMz, searchInFileByMz
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports Microsoft.VisualBasic.Linq
Imports mzkit.My
Imports RibbonLib.Interop
Imports Task

Module FeatureSearchHandler

    Public Sub SearchByMz(text As String, raw As IEnumerable(Of Raw), directRaw As Boolean)
        If text.StringEmpty Then
            Return
        ElseIf text.IsNumeric Then
            Call searchInFileByMz(mz:=Val(text), raw:=raw)
        Else
            Call runFormulaMatch(text, raw, directRaw)

            MyApplication.host.ribbonItems.TabGroupExactMassSearchTools.ContextAvailable = ContextAvailability.Active
        End If
    End Sub

    Private Sub runFormulaMatch(formula As String, files As IEnumerable(Of Raw), directRaw As Boolean)
        Dim ppm As Double = MyApplication.host.GetPPMError()
        Dim display As frmFeatureSearch = VisualStudio.ShowDocument(Of frmFeatureSearch)

        If directRaw Then
            display.directRaw = files.First
            display.AddFileMatch(display.directRaw.source, MatchByFormula(formula, display.directRaw).ToArray)
        Else
            For Each file As Raw In files
                Dim result = MatchByFormula(formula, file).ToArray

                display.AddFileMatch(file.source, result)
            Next
        End If
    End Sub

    Public Iterator Function MatchByFormula(formula As String, raw As Raw) As IEnumerable(Of ParentMatch)
        ' formula
        Dim exact_mass As Double = Math.EvaluateFormula(formula)
        Dim ppm As Double = MyApplication.host.GetPPMError()

        ' MyApplication.host.showStatusMessage($"Search MS ions for [{Text}] exact_mass={exact_mass} with tolerance error {ppm} ppm")

        ' C25H40N4O5
        Dim pos = MzCalculator.EvaluateAll(exact_mass, "+", False).ToArray
        Dim neg = MzCalculator.EvaluateAll(exact_mass, "-", False).ToArray
        Dim info As PrecursorInfo()

        For Each scan As ScanMS2 In raw.GetMs2Scans
            If scan.polarity > 0 Then
                info = pos
            Else
                info = neg
            End If

            For Each mode As PrecursorInfo In info
                If PPMmethod.PPM(scan.parentMz, Val(mode.mz)) <= ppm Then
                    Yield New ParentMatch With {
                        .scan_id = scan.scan_id,
                        .mz = scan.mz,
                        .rt = CInt(scan.rt),
                        .BPC = scan.into.Max,
                        .TIC = scan.into.Sum,
                        .M = mode.M,
                        .adducts = mode.adduct,
                        .charge = mode.charge,
                        .precursor_type = mode.precursor_type,
                        .ppm = PPMmethod.PPM(scan.parentMz, Val(mode.mz)).ToString("F2"),
                        .polarity = scan.polarity,
                        .XIC = scan.intensity,
                        .into = scan.into,
                        .parentMz = scan.parentMz
                    }
                End If
            Next
        Next
    End Function

    Private Sub searchInFileByMz(mz As Double, raw As IEnumerable(Of Raw))
        Dim ppm As Double = MyApplication.host.GetPPMError()
        Dim tolerance As Tolerance = Tolerance.PPM(ppm)
        Dim display As New frmFeatureSearch

        display.Show(MyApplication.host.dockPanel)

        For Each file As Raw In raw
            Dim result = file.LoadMzpack.GetMs2Scans.Where(Function(a) tolerance(a.parentMz, mz)).ToArray

            display.AddFileMatch(file.source, mz, result)
        Next
    End Sub
End Module
