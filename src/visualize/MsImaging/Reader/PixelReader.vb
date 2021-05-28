﻿Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1

Public MustInherit Class PixelReader : Implements IDisposable

    Private disposedValue As Boolean

    Public MustOverride ReadOnly Property dimension As Size

    Protected MustOverride Sub release()

    Public MustOverride Iterator Function LoadPixels(mz As Double(), tolerance As Tolerance, Optional skipZero As Boolean = True) As IEnumerable(Of PixelData)
    Public MustOverride Function LoadMzArray(ppm As Double) As Double()

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: 释放托管状态(托管对象)
                Call release()
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
