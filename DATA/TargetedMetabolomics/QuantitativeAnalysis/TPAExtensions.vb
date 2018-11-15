﻿Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Scripting
Imports SMRUCC.MassSpectrum.Math.Chromatogram
Imports SMRUCC.MassSpectrum.Math.MRM.Models

Public Module TPAExtensions

    ''' <summary>
    ''' 对某一个色谱区域进行峰面积的积分计算
    ''' </summary>
    ''' <param name="ion"></param>
    ''' <param name="baselineQuantile#"></param>
    ''' <param name="peakAreaMethod"></param>
    ''' <param name="integratorTicks%"></param>
    ''' <param name="TPAFactor">
    ''' ``{<see cref="Standards.HMDB"/>, <see cref="Standards.Factor"/>}``，这个是为了计算亮氨酸和异亮氨酸这类无法被区分的物质的峰面积所需要的
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function ionTPA(ion As NamedCollection(Of ChromatogramTick),
                           baselineQuantile#,
                           peakAreaMethod As PeakArea.Methods,
                           Optional integratorTicks% = 5000,
                           Optional TPAFactor# = 1) As NamedValue(Of (DoubleRange, Double, Double, Double))

        Dim vector As IVector(Of ChromatogramTick) = ion.Value.Shadows
        Dim ROIData = vector _
            .PopulateROI _
            .OrderByDescending(Function(ROI)
                                   ' 这个积分值只是用来查找最大的峰面积的ROI区域
                                   ' 并不是最后的峰面积结果
                                   ' 还需要在下面的代码之中做峰面积积分才可以得到最终的结果
                                   Return ROI.Integration
                               End Function) _
            .ToArray


        If ROIData.Length = 0 Then
            Return New NamedValue(Of (DoubleRange, Double, Double, Double)) With {
                .Name = ion.Name,
                .Value = (New DoubleRange(0, 0), 0, 0, 0)
            }
        Else
            Dim peak As DoubleRange = ROIData _
                .First _
                .Time ' .MRMPeak(baselineQuantile:=baselineQuantile)
            Dim data As (peak As DoubleRange, area#, baseline#, maxPeakHeight#)

            With vector.TPAIntegrator(peak, baselineQuantile, peakAreaMethod, integratorTicks, TPAFactor)
                data = (peak, .Item1, .Item2, .Item3)
            End With

            Return New NamedValue(Of (DoubleRange, Double, Double, Double)) With {
                .Name = ion.Name,
                .Value = data
            }
        End If
    End Function

    ''' <summary>
    ''' 对某一个色谱区域进行峰面积的积分计算
    ''' </summary>
    ''' <param name="vector">完整的色谱图，计算基线的时候会需要使用到的</param>
    ''' <param name="peak">
    ''' 目标峰的时间范围
    ''' </param>
    ''' <param name="baselineQuantile">推荐0.65</param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function TPAIntegrator(vector As IVector(Of ChromatogramTick), peak As DoubleRange,
                                  baselineQuantile#,
                                  Optional peakAreaMethod As PeakArea.Methods = PeakArea.Methods.Integrator,
                                  Optional resolution% = 5000,
                                  Optional TPAFactor# = 1) As (area#, baseline#, maxPeakHeight#)
        Dim area#
        Dim baseline# = vector.Baseline(quantile:=baselineQuantile)

        Select Case peakAreaMethod
            Case Methods.NetPeakSum
                area = vector.PeakArea(peak, baseline:=baselineQuantile)
            Case Methods.SumAll
                area = vector.SumAll
            Case Methods.MaxPeakHeight
                area = vector.MaxPeakHeight
            Case Else
                ' 默认是使用积分器方法
                area = vector.PeakAreaIntegrator(
                    peak:=peak,
                    baselineQuantile:=baselineQuantile,
                    resolution:=resolution
                )
        End Select

        area *= TPAFactor

        Return (area, baseline, vector.PickArea(range:=peak).MaxPeakHeight)
    End Function
End Module
