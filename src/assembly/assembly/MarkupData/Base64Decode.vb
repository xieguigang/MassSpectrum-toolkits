﻿Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Net.Http

Namespace MarkupData

    <HideModuleName>
    Public Module Base64Decoder

        ''' <summary>
        ''' 对质谱扫描信号结果进行解码操作
        ''' </summary>
        ''' <param name="stream">Container for the binary base64 string data.</param>
        ''' <returns></returns>
        <Extension>
        Public Function Base64Decode(stream As IBase64Container, Optional networkByteOrder As Boolean = False) As Double()
            Dim bytes As Byte() = Convert.FromBase64String(stream.BinaryArray)
            Dim floats#()
            Dim byteStream As MemoryStream

            Select Case stream.GetCompressionType
                Case CompressionMode.zlib
                    ' 2018-11-15 经过测试，与zlib的结果一致
                    byteStream = bytes.UnZipStream
                Case CompressionMode.gzip
                    byteStream = bytes.UnGzipStream
                Case CompressionMode.none
                    byteStream = New MemoryStream(bytes)
                Case Else
                    Throw New NotImplementedException(stream.GetCompressionType)
            End Select

            Using byteStream
                bytes = byteStream.ToArray
            End Using

            If networkByteOrder AndAlso BitConverter.IsLittleEndian Then
                Call Array.Reverse(bytes)
            End If

            Select Case stream.GetPrecision
                Case 64
                    floats = bytes _
                        .Split(8) _
                        .Select(Function(b) BitConverter.ToDouble(b, Scan0)) _
                        .ToArray
                Case 32
                    floats = bytes _
                        .Split(4) _
                        .Select(Function(b) BitConverter.ToSingle(b, Scan0)) _
                        .Select(Function(s) Val(s)) _
                        .ToArray
                Case Else
                    Throw New NotImplementedException
            End Select

            Return floats
        End Function
    End Module
End Namespace