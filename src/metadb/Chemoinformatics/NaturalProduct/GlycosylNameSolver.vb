﻿Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq

Namespace NaturalProduct

    Public Class GlycosylNameSolver

        ReadOnly steric As Index(Of String) = {
            "alpha", "beta", "trans",
            "cis", "red", "acid",
            "bis", "ester", "cyclic",
            "bata", "hydroxy",
            "aero", "pro",
            "-", ","  ' delimiter symbols
        }
        ReadOnly rules As Dictionary(Of String, String())

        Friend Shared ReadOnly qprefix As Dictionary(Of String, Integer) = Enums(Of QuantityPrefix) _
            .ToDictionary(Function(a) a.Description,
                          Function(a)
                              Return CInt(a)
                          End Function)

        Sub New(Optional customRules As Dictionary(Of String, String()) = Nothing)
            Me.rules = customRules

            If rules.IsNullOrEmpty Then
                rules = New Dictionary(Of String, String())
            End If
        End Sub

        Private Iterator Function HandleComponents(token As String) As IEnumerable(Of String)
            Dim hitPrefix As Boolean = False

            If token.Length < 3 Then
                Return
            ElseIf token Like steric Then
                Return
            End If

            For Each q As KeyValuePair(Of String, Integer) In qprefix
                If token.StartsWith(q.Key) Then
                    hitPrefix = True
                    token = token.Substring(q.Key.Length)

                    If rules.ContainsKey(token) Then
                        For Each item As String In rules(token)
                            For i As Integer = 1 To q.Value
                                Yield item
                            Next
                        Next
                    Else
                        For i As Integer = 1 To q.Value
                            Yield token
                        Next
                    End If

                    Exit For
                End If
            Next

            If Not hitPrefix Then
                If rules.ContainsKey(token) Then
                    For Each item As String In rules(token)
                        Yield item
                    Next
                Else
                    Yield token
                End If
            End If
        End Function

        Private Shared Function Trim(glycosyl As String) As String
            glycosyl = glycosyl.StringReplace("\d+", " ")
            ' glycosyl = glycosyl.StringReplace("[()]", " ")
            glycosyl = glycosyl.Replace("'", "").Replace("[", " ").Replace("]", " ")
            glycosyl = glycosyl.StringReplace("[-]{2,}", "-")
            glycosyl = glycosyl.Trim(" "c, "-"c, ","c, "{"c, "}"c, "["c, "]"c)

            Return glycosyl
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="glycosyl"></param>
        ''' <returns></returns>
        Public Iterator Function GlycosylNameParser(glycosyl As String) As IEnumerable(Of String)
            Dim n As Integer = 1
            Dim tokens As Token() = Trim(glycosyl) _
                .ToLower _
                .StringSplit("([-])|\s+") _
                .Where(Function(t) Not t.StringEmpty AndAlso Not t Like steric) _
                .JoinBy("") _
                .DoCall(AddressOf TokenScanner.ScanTokens) _
                .ToArray

            For Each token As Token In tokens
                If token.name = NameTokens.number Then
                    n = CInt(token.name)
                    Continue For
                End If

                Dim all As String() = HandleComponents(token.text) _
                    .Where(Function(part)
                               Return Not part.StringEmpty AndAlso Not part Like steric
                           End Function) _
                    .ToArray

                For i As Integer = 1 To n
                    For Each item As String In all
                        Yield item
                    Next
                Next

                n = 1
            Next
        End Function
    End Class
End Namespace