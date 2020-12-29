﻿Imports Microsoft.VisualBasic.Data.IO

Namespace mzData.mzWebCache

    Public Class BinaryStreamWriter : Implements IDisposable

        Dim file As BinaryDataWriter
        Dim disposedValue As Boolean
        Dim scanIndex As New Dictionary(Of String, Long)

        Public Const Magic As String = "BioNovoGene/mzWebStream"

        Sub New(file As String)
            Me.file = New BinaryDataWriter(file.Open(IO.FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False))
            Me.file.Write(Magic)
            Me.file.ByteOrder = ByteOrder.LittleEndian
        End Sub

        Public Sub Write(scan As ScanMS1)
            Call file.Write(scan.scan_id, BinaryStringFormat.ZeroTerminated)
            Call file.Write(scan.rt)
            Call file.Write(scan.BPC)
            Call file.Write(scan.TIC)
            Call file.Write(scan.mz.Length)
            Call file.Write(scan.mz)
            Call file.Write(scan.into)
            Call file.Write(scan.products.Length)

            For Each product As ScanMS2 In scan.products
                Call Write(product)
            Next

            Call file.Flush()
            Call scanIndex.Add(scan.scan_id, file.Position)
        End Sub

        Private Sub Write(scan As ScanMS2)
            Call file.Write(scan.scan_id, BinaryStringFormat.ZeroTerminated)
            Call file.Write(scan.parentMz)
            Call file.Write(scan.rt)
            Call file.Write(scan.intensity)
            Call file.Write(scan.polarity)
            Call file.Write(scan.mz.Length)
            Call file.Write(scan.mz)
            Call file.Write(scan.into)
        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: 释放托管状态(托管对象)
                    Call file.Flush()
                    Call file.Dispose()
                End If

                ' TODO: 释放未托管的资源(未托管的对象)并替代终结器
                ' TODO: 将大型字段设置为 null
                disposedValue = True
            End If
        End Sub

        ' ' TODO: 仅当“Dispose(disposing As Boolean)”拥有用于释放未托管资源的代码时才替代终结器
        ' Protected Overrides Sub Finalize()
        '     ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
        '     Dispose(disposing:=False)
        '     MyBase.Finalize()
        ' End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class
End Namespace