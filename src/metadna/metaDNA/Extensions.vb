﻿#Region "Microsoft.VisualBasic::8152f7f27dbc37312d64ddc1aae9981a, src\metadna\metaDNA\Extensions.vb"

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

    ' Module Extensions
    ' 
    '     Function: MgfSeed, MgfSeeds
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

<HideModuleName> Public Module Extensions

    <Extension>
    Public Iterator Function MgfSeeds(mgf As IEnumerable(Of PeakMs2)) As IEnumerable(Of AnnotatedSeed)
        For Each ion As PeakMs2 In mgf
            Yield ion.MgfSeed
        Next
    End Function

    <Extension>
    Private Function MgfSeed(ion As PeakMs2) As AnnotatedSeed
        Dim ms1 As New ms1_scan With {
            .mz = ion.mz,
            .scan_time = ion.rt,
            .intensity = ion.intensity
        }
        Dim ms2 As New LibraryMatrix With {
            .name = ion.lib_guid,
            .ms2 = ion.mzInto
        }

        Return New AnnotatedSeed With {
            .inferSize = 1,
            .kegg_id = ion.meta!KEGG,
            .id = ion.lib_guid,
            .parent = ms1,
            .parentTrace = 100,
            .products = New Dictionary(Of String, LibraryMatrix) From {
                {ion.lib_guid, ms2}
            }
        }
    End Function
End Module

