﻿#Region "Microsoft.VisualBasic::4f2accbdd8d3fe79effec426634259d4, src\mzkit\Task\BioDeep\KEGGRepo.vb"

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

    ' Module KEGGRepo
    ' 
    '     Function: RequestKEGGcompounds, RequestKEGGReactions
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.My
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Data.KEGG.Metabolism
Imports biodeep

Module KEGGRepo

    Public Function RequestKEGGcompounds(println As Action(Of String)) As Compound()
        Const url As String = "http://query.biodeep.cn/kegg/repository/compounds"

        Call println("request KEGG compounds from BioDeep...")

        Return SingletonHolder(Of BioDeepSession).Instance _
            .RequestStream(url) _
            .DoCall(AddressOf KEGGCompoundPack.ReadKeggDb)
    End Function

    Public Function RequestKEGGReactions(println As Action(Of String)) As ReactionClass()
        Const url As String = "http://query.biodeep.cn/kegg/repository/reactions"

        Call println("request KEGG reaction network from BioDeep...")

        Return SingletonHolder(Of BioDeepSession).Instance _
            .RequestStream(url) _
            .DoCall(AddressOf ReactionClassPack.ReadKeggDb)
    End Function
End Module
