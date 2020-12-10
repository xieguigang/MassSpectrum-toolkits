﻿Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.DataReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML

Namespace mzData.mzWebCache

    Public Class mzXMLScans : Inherits ScanPopulator(Of scan)

        Public Sub New(Optional mzErr$ = "da:0.1")
            MyBase.New(mzErr)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function loadScans(rawfile As String) As IEnumerable(Of scan)
            Return XML.LoadScans(rawfile)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function dataReader() As MsDataReader(Of scan)
            Return New mzXMLScan()
        End Function
    End Class
End Namespace