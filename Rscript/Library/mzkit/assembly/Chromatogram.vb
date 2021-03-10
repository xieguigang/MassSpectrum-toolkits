﻿
Imports System.IO
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.DataReader
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("chromatogram")>
<RTypeExport("overlaps", GetType(ChromatogramOverlap))>
Module ChromatogramTools

    Sub New()
        Call ConsolePrinter.AttachConsoleFormatter(Of ChromatogramOverlap)(AddressOf overlapsSummary)
    End Sub

    Private Function overlapsSummary(data As ChromatogramOverlap) As String
        Dim text As New StringBuilder

        Call text.AppendLine($"Chromatogram Overlaps Of {data.length} files:")

        For Each file As String In data.overlaps.Keys
            Call text.AppendLine($"  {file} {data(file).scan_time.Length} scans")
        Next

        Return text.ToString
    End Function

    <ExportAPI("add")>
    Public Function addOverlaps(overlaps As ChromatogramOverlap, name$, data As Chromatogram) As ChromatogramOverlap
        Call overlaps.overlaps.Add(name, data)
        Return overlaps
    End Function

    <ExportAPI("subset")>
    Public Function subset(overlaps As ChromatogramOverlap, names As String()) As ChromatogramOverlap
        Return overlaps(names)
    End Function

    <ExportAPI("labels")>
    Public Function setLabels(overlaps As ChromatogramOverlap, names As String(), Optional env As Environment = Nothing) As ChromatogramOverlap
        overlaps.setNames(names, env)
        Return overlaps
    End Function

    <ExportAPI("overlaps")>
    <RApiReturn(GetType(ChromatogramOverlap))>
    Public Function overlaps(<RRawVectorArgument> Optional TIC As Object = Nothing, Optional env As Environment = Nothing) As Object
        If TIC Is Nothing Then
            Return New ChromatogramOverlap
        End If

        If TypeOf TIC Is ChromatogramOverlap Then
            Return TIC
        End If

        If TypeOf TIC Is list Then
            Dim result As New ChromatogramOverlap

            For Each item In DirectCast(TIC, list).namedValues
                If Not TypeOf item.Value Is Chromatogram Then
                    Return Message.InCompatibleType(GetType(Chromatogram), item.Value.GetType, env, $"item '{item.Name}' is not a chromatogram value.")
                Else
                    result(item.Name) = item.Value
                End If
            Next

            Return result
        Else
            Dim overlapsData As pipeline = pipeline.TryCreatePipeline(Of Chromatogram)(TIC, env)
            Dim result As New ChromatogramOverlap

            If overlapsData.isError Then
                Return overlapsData.getError
            Else
                For Each item As SeqValue(Of Chromatogram) In overlapsData.populates(Of Chromatogram)(env).SeqIterator
                    result(item.i) = item
                Next
            End If

            Return result
        End If
    End Function

    <ExportAPI("write.pack")>
    Public Sub PackData(overlaps As ChromatogramOverlap, cdf As String)
        Using file As Stream = cdf.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
            Call overlaps.SavePackData(file)
        End Using
    End Sub

    <ExportAPI("read.pack")>
    Public Function ReadData(cdf As String) As ChromatogramOverlap
        Using file As Stream = cdf.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Return file.ReadPackData
        End Using
    End Function
End Module
