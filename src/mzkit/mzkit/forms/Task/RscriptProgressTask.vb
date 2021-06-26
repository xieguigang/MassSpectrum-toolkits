﻿Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Task

Public Class RscriptProgressTask

    Public Shared Function CreateMSIIndex(imzML As String) As String
        Dim Rscript As String = RscriptPipelineTask.GetRScript("buildMSIIndex.R")
        Dim ibd As ibdReader = ibdReader.Open(imzML.ChangeSuffix("ibd"))
        Dim uid As String = ibd.UUID
        Dim cachefile As String = App.AppSystemTemp & "/MSI_imzML/" & uid
        Dim cli As String = $"""{Rscript}"" --imzML ""{imzML}"" --cache ""{cachefile}"""
        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Rscript.Path, cli)

        If cachefile.FileLength > 1024 Then
            Return cachefile
        End If

        Dim progress As New frmTaskProgress

        progress.ShowProgressTitle("Open imzML...", directAccess:=True)
        progress.ShowProgressDetails("Loading MSI raw data file into viewer workspace...", directAccess:=True)
        progress.SetProgressMode()

        Call pipeline.CommandLine.__DEBUG_ECHO

        AddHandler pipeline.SetProgress, AddressOf progress.SetProgress
        AddHandler pipeline.Finish, Sub() progress.Invoke(Sub() progress.Close())

        Call New Thread(AddressOf pipeline.Run).Start()
        Call progress.ShowDialog()

        Return cachefile
    End Function

End Class
