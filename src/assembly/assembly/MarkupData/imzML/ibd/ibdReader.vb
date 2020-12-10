﻿Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.IO

Namespace MarkupData.imzML

    Public Class ibdReader : Implements IDisposable

        ReadOnly stream As BinaryDataReader

        Dim disposedValue As Boolean

        ''' <summary>
        ''' The first 16 bytes of the binary file are reserved for an Universally Unique Identifier. 
        ''' It is also saved in the imzML file so that a correct assignment of ibd and imzML file 
        ''' is possible even if the names of both files are different.
        ''' </summary>
        Dim magic As Guid
        Dim format As Format

        ''' <summary>
        ''' Universal Unique Identifier
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property UUID As String
            Get
                Return magic.ToString
            End Get
        End Property

        Sub New(file As Stream, layout As Format)
            stream = New BinaryDataReader(file)
            stream.ByteOrder = ByteOrder.LittleEndian
            format = layout
            magic = New Guid(stream.ReadBytes(16))
        End Sub

        Public Function GetMSMS(scan As ScanData) As ms2()
            Dim mz As Double() = ReadArray(scan.MzPtr)
            Dim intensity As Double() = ReadArray(scan.IntPtr)
            Dim data As ms2() = mz _
                .Select(Function(mzi, i)
                            Return New ms2 With {
                                .mz = mzi,
                                .intensity = intensity(i),
                                .quantity = .intensity
                            }
                        End Function) _
                .ToArray

            Return data
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ReadArray(ptr As ibdPtr) As Double()
            Return ReadArray(ptr.offset, ptr.encodedLength, ptr.arrayLength)
        End Function

        Public Function ReadArray(offset As Long, encodedLength As Integer, arrayLength As Integer) As Double()
            Dim externalArray As Byte()
            Dim sizeof As Integer = encodedLength / arrayLength

            stream.Seek(offset, SeekOrigin.Begin)
            externalArray = stream.ReadBytes(encodedLength)

            If sizeof = 4 Then
                Return externalArray.Split(4).Select(Function(bytes) CDbl(BitConverter.ToSingle(bytes, Scan0))).ToArray
            Else
                Return externalArray.Split(8).Select(Function(bytes) BitConverter.ToDouble(bytes, Scan0)).ToArray
            End If
        End Function

        Public Overrides Function ToString() As String
            Return $"[{format.ToString}] " & UUID
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Open(ibd As String) As ibdReader
            Return New ibdReader(ibd.Open(FileMode.Open, doClear:=False, [readOnly]:=True), Format.Processed)
        End Function

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects)
                    Call stream.Dispose()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
                ' TODO: set large fields to null
                disposedValue = True
            End If
        End Sub

        ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
        ' Protected Overrides Sub Finalize()
        '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        '     Dispose(disposing:=False)
        '     MyBase.Finalize()
        ' End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class
End Namespace