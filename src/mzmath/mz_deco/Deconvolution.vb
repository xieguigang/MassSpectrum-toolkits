﻿Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports scan = BioNovoGene.Analytical.MassSpectrometry.Math.IMs1Scan

''' <summary>
''' #### 解卷积计算步骤
''' 
''' 1. 首先对每一个原始数据点按照mz进行数据分组
''' 2. 对每一个mz数据分组按照rt进行升序排序
''' 3. 对每一个mz数据分组进行解卷积，得到峰列表
''' 4. 输出peaktable结果，完成解卷积操作
''' </summary>
Public Module Deconvolution

    ''' <summary>
    ''' All of the mz value in <paramref name="mzpoints"/> should be equals
    ''' </summary>
    ''' <param name="mzpoints"></param>
    ''' <returns></returns>
    ''' <remarks>实际的解卷积操作步骤：应用于处理复杂的样本数据</remarks>
    <Extension>
    Public Function GetPeakGroups(mzpoints As MzGroup) As IEnumerable(Of PeakFeature)

    End Function

    ''' <summary>
    ''' Separation of mass signals.
    ''' (进行原始数据的mz分组操作，然后进行rt的升序排序)
    ''' </summary>
    ''' <param name="scans"></param>
    ''' <returns></returns>
    Public Iterator Function GetMzGroup(scans As IEnumerable(Of scan), Optional tolerance As Tolerance = Nothing) As IEnumerable(Of MzGroup)
        For Each group As NamedCollection(Of scan) In scans.GroupBy(Function(t) t.mz, AddressOf (tolerance Or Tolerance.DefaultTolerance).Assert)
            Dim timePoints As scan() = group.ToArray
            Dim xic As ChromatogramTick() = timePoints _
                .Select(Function(t)
                            Return New ChromatogramTick With {
                                .Time = t.rt,
                                .Intensity = t.intensity
                            }
                        End Function) _
                .OrderBy(Function(t) t.Time) _
                .ToArray
            Dim mz As Double = Aggregate t As scan
                               In timePoints
                               Into Average(t.mz)

            Yield New MzGroup With {
                .mz = mz,
                .XIC = xic
            }
        Next
    End Function

    <Extension>
    Public Function DecoMzGroups(mzgroups As IEnumerable(Of MzGroup)) As IEnumerable(Of PeakFeature)
        Return mzgroups.Select(Function(mz) mz.GetPeakGroups()).IteratesALL
    End Function
End Module
