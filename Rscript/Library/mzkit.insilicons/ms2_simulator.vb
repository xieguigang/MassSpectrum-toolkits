﻿Imports BioNovoGene.Analytical.MassSpectrometry.Math.Insilicon
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemistry.Model
Imports BioNovoGene.BioDeep.Chemistry.Model.Graph
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("mzkit.simulator")>
Module ms2_simulator

    <ExportAPI("read.kcf")>
    Public Function loadKCF(file As String) As KCF
        Return file.LoadKCF(False)
    End Function

    <ExportAPI("molecular.graph")>
    Public Function MolecularGraph(mol As KCF) As NetworkGraph
        Return mol _
           .CreateGraph _
           .FillBoundEnergy(New BoundEnergyFinder)
    End Function

    <ExportAPI("fragmentation")>
    Public Function MolecularFragmentation(mol As NetworkGraph, energy As EnergyModel,
                                           Optional step% = 100,
                                           Optional precision% = 4,
                                           Optional intoCutoff# = -1) As LibraryMatrix
        Return mol.MolecularFragment(energy, [step], precision, intoCutoff)
    End Function

    <ExportAPI("energy.normal")>
    Public Function energyModel_normalDist(mu#, delta#) As EnergyModel
        Return New EnergyModel(Function(x, y)
                                   Return pnorm.ProbabilityDensity(x, mu, delta)
                               End Function, 0, 1000)
    End Function
End Module
