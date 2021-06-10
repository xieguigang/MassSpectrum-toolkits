﻿Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace Pixel

    Public MustInherit Class PixelScan

        Public MustOverride ReadOnly Property X As Integer
        Public MustOverride ReadOnly Property Y As Integer

        Public MustOverride Function GetMs() As ms2()

        Public MustOverride Function HasAnyMzIon(mz As Double(), tolerance As Tolerance) As Boolean

        Public Overrides Function ToString() As String
            Return $"[{X},{Y}]"
        End Function

    End Class
End Namespace