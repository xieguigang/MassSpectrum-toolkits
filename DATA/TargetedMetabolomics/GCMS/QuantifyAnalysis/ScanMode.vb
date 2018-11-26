﻿#Region "Microsoft.VisualBasic::b11c1deec689c8403a107f6412a184a9, GCMS_quantify\QuantifyAnalysis.vb"

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

' Module QuantifyAnalysis
' 
'     Function: ExportReferenceROITable, ExportROI, ReadData
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.MassSpectrum.Math
Imports SMRUCC.MassSpectrum.Math.Chromatogram
Imports SMRUCC.MassSpectrum.Math.Spectra

Namespace GCMS.QuantifyAnalysis

    ''' <summary>
    ''' GCMS自动化定量分析模块
    ''' 
    ''' https://github.com/cheminfo-js/netcdf-gcms
    ''' </summary>
    Public Module ScanModeWorker

        ''' <summary>
        ''' 利用标准品的信息从GCMS的实验数据之中找出对应的检测物质的检测结果
        ''' </summary>
        ''' <param name="standards">标准品数据</param>
        ''' <param name="data">实验数据</param>
        ''' <param name="sn#">信噪比阈值，低于这个阈值的信号都将会被抛弃</param>
        ''' <param name="winSize">
        ''' 进行查找的时间窗大小
        ''' </param>
        ''' <returns>
        ''' 这个函数所返回来的结果之中已经包含有必须的峰面积等信息了
        ''' </returns>
        <Extension>
        Public Iterator Function ScanIons(standards As IEnumerable(Of ROITable), data As Raw,
                                          Optional sn# = 3,
                                          Optional winSize! = 3,
                                          Optional scoreCutoff# = 0.85,
                                          Optional angleCutoff# = 5,
                                          Optional all As Boolean = False) As IEnumerable(Of (ROITable, query As LibraryMatrix, ref As LibraryMatrix))

            Dim ROIlist As ROI() = data _
                .ExportROI(angleCutoff) _
                .Where(Function(ROI) ROI.snRatio >= sn) _
                .ToArray
            Dim resultTable As ROITable

            ' 先用时间窗，找出和参考相近的实验数据
            ' 然后做质谱图的比对操作
            For Each ref As ROITable In standards
                Dim timeRange As DoubleRange = {ref.rtmin - winSize, ref.rtmax + winSize}
                Dim refSpectrum As LibraryMatrix = ref.CreateMatrix

                refSpectrum.Name = ref.ID

                Dim candidates = ROIlist _
                    .SkipWhile(Function(c) c.Time.Max < timeRange.Min) _
                    .TakeWhile(Function(c) c.Time.Min < timeRange.Max) _
                    .Select(Function(region As ROI)
                                ' 在这个循环之中的都是rt符合条件要求的
                                Dim matrixName$ = $"rt={region.rt}, [{Fix(region.Time.Min)},{Fix(region.Time.Max)}]"
                                Dim query = data.GetMsScan(region.Time) _
                                    .GroupByMz() _
                                    .CreateLibraryMatrix(matrixName)
                                Dim score = GlobalAlignment.TwoDirectionSSM(
                                    x:=query.ms2,
                                    y:=refSpectrum.ms2,
                                    method:=Tolerance.DefaultTolerance
                                )
                                Dim minScore# = {score.forward, score.reverse}.Min

                                Return (
                                    score:=score,
                                    minScore:=minScore,
                                    query:=query,
                                    region:=region
                                )
                            End Function) _
                    .Where(Function(candidate)
                               Return candidate.minScore >= scoreCutoff
                           End Function) _
                    .OrderByDescending(Function(candidate) candidate.minScore) _
                    .ToArray

                For Each candidate In candidates
                    ' 计算出峰面积
                    Dim TPA = candidate.region _
                        .Ticks _
                        .Shadows _
                        .TPAIntegrator(candidate.region.Time, 0.65, Methods.Integrator)

                    resultTable = candidate.region.convert(
                        raw:=data,
                        ri:=0,
                        title:=ref.ID
                    )
                    ' 将获取得到原始的峰面积信息
                    ' 在下一个步骤函数之中将会除以内标的峰面积得到X坐标轴的数据
                    resultTable.integration = TPA.area
                    resultTable.IS = ref.IS

                    Yield (resultTable, candidate.query, refSpectrum)

                    If Not all Then
                        Exit For
                    End If
                Next
            Next
        End Function

        <Extension>
        Private Function convert(ROI As ROI, raw As Raw, ri#, title$) As ROITable
            Dim spectra = raw.GetMsScan(ROI.Time).GroupByMz
            Dim base64 As String = spectra _
                .Select(Function(mz) $"{mz.mz} {mz.intensity}") _
                .JoinBy(ASCII.TAB) _
                .Base64String

            Return New ROITable With {
                .sn = ROI.snRatio,
                .baseline = ROI.Baseline,
                .ID = title,
                .integration = ROI.Integration,
                .maxInto = ROI.MaxInto,
                .ri = ri,
                .rt = ROI.rt,
                .rtmax = ROI.Time.Max,
                .rtmin = ROI.Time.Min,
                .mass_spectra = base64
            }
        End Function

        ''' <summary>
        ''' 导出标准品参考的ROI区间列表，用于``GC/MS``自动化定性分析
        ''' </summary>
        ''' <param name="regions"></param>
        ''' <param name="sn">
        ''' 信噪比筛选阈值
        ''' </param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 保留指数的计算：在标准化流程之中，GCMS的出峰顺序保持不变，但是保留时间可能会在不同批次实验间有变化
        ''' 这个时候如果定量用的标准品混合物和样本之中的所检测物质的出峰顺序一致，则可以将标准品混合物之中的
        ''' 第一个出峰的物质和最后一个出峰的物质作为保留指数的参考，在这里假设第一个出峰的物质的保留指数为零，
        ''' 最后一个出峰的物质的保留指数为1000，则可以根据这个区间和rt之间的线性关系计算出保留指数
        ''' </remarks>
        <Extension> Public Function ExportReferenceROITable(regions As ROI(), raw As Raw,
                                                            Optional sn# = 5,
                                                            Optional names$() = Nothing,
                                                            Optional RImax# = 1000) As ROITable()

            With regions.Where(Function(ROI) ROI.snRatio >= sn).ToArray
                Dim refA = .First, refB = .Last
                Dim A = (refA.rt, 0)
                Dim B = (refB.rt, RImax)
                Dim getTitle As Func(Of ROI, Integer, String)

                If names.IsNullOrEmpty Then
                    getTitle = Function(ROI, i) $"#{i + 1}={Fix(ROI.rt)}s"
                Else
                    getTitle = Function(ROI, i)
                                   Return names.ElementAtOrDefault(i, $"#{i + 1}={Fix(ROI.rt)}s")
                               End Function
                End If

                Return .Select(Function(ROI, i)
                                   Return ROI.convert(raw, ROI.RetentionIndex(A, B), getTitle(ROI, i))
                               End Function) _
                       .ToArray
            End With
        End Function
    End Module
End Namespace