﻿#Region "Microsoft.VisualBasic::3001ad82522f32571d3f8bf321c1b3d1, Rscript\Library\mzkit\math\Formula.vb"

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

    ' Module FormulaTools
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: (+5 Overloads) add, asFormula, CreateGraph, divide, DownloadKCF
    '               EvalFormula, FormulaCompositionString, FormulaFinder, FormulaString, getElementCount
    '               LoadChemicalDescriptorsMatrix, (+5 Overloads) minus, openChemicalDescriptorDatabase, parseSMILES, printFormulas
    '               readKCF, readSDF, removeElement, (+2 Overloads) repeats, ScanFormula
    '               SDF2KCF
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemistry
Imports BioNovoGene.BioDeep.Chemistry.Model.Graph
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.SDF
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime.Internal.ConsolePrinter

''' <summary>
''' The chemical formulae toolkit
''' </summary>
<Package("formula", Category:=APICategories.UtilityTools)>
Module FormulaTools

    Sub New()
        Call REnv.AttachConsoleFormatter(Of FormulaComposition)(AddressOf FormulaCompositionString)
        Call REnv.AttachConsoleFormatter(Of Formula)(AddressOf FormulaString)
        Call REnv.AttachConsoleFormatter(Of FormulaComposition())(AddressOf printFormulas)
    End Sub

    Private Function printFormulas(formulas As FormulaComposition()) As String
        Dim table As New List(Of String())

        table += {"formula", "mass", "da", "ppm", "charge", "m/z"}

        For Each formula As FormulaComposition In formulas
            table += New String() {
                formula.EmpiricalFormula,
                formula.ExactMass,
                formula.ppm,
                formula.ppm,
                formula.charge,
                formula.ExactMass / formula.charge
            }
        Next

        Return table.Print(addBorder:=False)
    End Function

    Private Function FormulaCompositionString(formula As FormulaComposition) As String
        Return formula.EmpiricalFormula & $" ({formula.CountsByElement.GetJson})"
    End Function

    Private Function FormulaString(formula As Formula) As String
        Return formula.ExactMass.ToString("F7") & $" ({formula.CountsByElement.Select(Function(e) $"{e.Key}:{e.Value}").JoinBy(", ")})"
    End Function

    <ExportAPI("find.formula")>
    Public Function FormulaFinder(mass#,
                                  Optional ppm# = 5,
                                  <RRawVectorArgument(GetType(String))>
                                  Optional candidateElements As Object = "C|H|N|O") As FormulaComposition()

        Dim opts As New SearchOption(-9999, 9999, ppm)

        For Each element As String In DirectCast(candidateElements, String())
            Call opts.AddElement(element, 0, 30)
        Next

        Dim oMwtWin As New FormulaSearch(opts)
        Dim results As FormulaComposition() = oMwtWin.SearchByExactMass(mass).ToArray

        Return results
    End Function

    ''' <summary>
    ''' evaluate exact mass for the given formula strings.
    ''' </summary>
    ''' <param name="formula">
    ''' a vector of the character formulas.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("eval")>
    <RApiReturn(GetType(Double))>
    Public Function EvalFormula(<RRawVectorArgument> formula As Object, Optional env As Environment = Nothing) As Object
        If TypeOf formula Is Formula Then
            Return DirectCast(formula, Formula).ExactMass
        End If

        Return env.EvaluateFramework(Of String, Double)(
            x:=formula,
            eval:=Function(str)
                      Return FormulaScanner.ScanFormula(str).ExactMass
                  End Function)
    End Function

    ''' <summary>
    ''' Get atom composition from a formula string
    ''' </summary>
    ''' <param name="formula">The input formula string text.</param>
    ''' <returns></returns>
    <ExportAPI("scan")>
    <RSymbolLanguageMask("[a-zA-Z0-9()]+", True, Test:=GetType(TestFormulaSymbolLang))>
    Public Function ScanFormula(formula$, Optional env As Environment = Nothing) As Formula
        Dim globalEnv As GlobalEnvironment = env.globalEnvironment
        Dim n As Integer = globalEnv.options.getOption("formula.polymers_n", 999)
        Dim formulaObj As Formula = FormulaScanner.ScanFormula(formula, n)

        Return formulaObj
    End Function

#Region "formula operators"

    <ExportAPI("getElementCount")>
    Public Function getElementCount(formula As Formula, element As String) As Integer
        Return formula(element)
    End Function

    <ExportAPI("removeElement")>
    Public Function removeElement(formula As Formula, element As String) As Formula
        formula = New Formula(formula.CountsByElement, formula.EmpiricalFormula)

        If formula.CountsByElement.ContainsKey(element) Then
            formula.CountsByElement.Remove(element)
        End If

        Return formula
    End Function

    <ROperator("+")>
    Public Function add(part1 As Formula, part2 As Formula) As Formula
        Return part1 + part2
    End Function

    <ROperator("-")>
    Public Function minus(total As Formula, part As Formula, Optional env As Environment = Nothing) As Formula
        Dim delta = total - part
        Dim negative = delta.CountsByElement.Where(Function(c) c.Value < 0).ToDictionary

        If Not negative.IsNullOrEmpty Then
            Call env.AddMessage({
                $"formula math results negative composition result: {negative.GetJson}",
                $"total: {total.EmpiricalFormula}",
                $"part: {part.EmpiricalFormula}"
            }, MSG_TYPES.WRN)
        End If

        Return delta
    End Function

    <ROperator("*")>
    Public Function repeats(part As Formula, n As Integer) As Formula
        Return part * n
    End Function

    <ROperator("*")>
    Public Function repeats(n As Integer, part As Formula) As Formula
        Return part * n
    End Function

    <ROperator("/")>
    Public Function divide(total As Formula, n As Integer) As Formula
        Return total / n
    End Function

    <ROperator("-")>
    Public Function minus(f As Formula, mass As Double) As Double
        Return f.ExactMass - mass
    End Function

    <ROperator("-")>
    Public Function minus(mass As Double, f As Formula) As Double
        Return mass - f.ExactMass
    End Function

    <ROperator("+")>
    Public Function add(mass As Double, f As Formula) As Double
        Return mass + f.ExactMass
    End Function

    <ROperator("+")>
    Public Function add(f As Formula, mass As Double) As Double
        Return f.ExactMass + mass
    End Function

    <ROperator("-")>
    Public Function minus(f As Formula, mass As Integer) As Double
        Return f.ExactMass - mass
    End Function

    <ROperator("-")>
    Public Function minus(mass As Integer, f As Formula) As Double
        Return mass - f.ExactMass
    End Function

    <ROperator("+")>
    Public Function add(mass As Integer, f As Formula) As Double
        Return mass + f.ExactMass
    End Function

    <ROperator("+")>
    Public Function add(f As Formula, mass As Integer) As Double
        Return f.ExactMass + mass
    End Function

#End Region

    ''' <summary>
    ''' Read KCF model data
    ''' </summary>
    ''' <param name="data">The text data or file path</param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("read.KCF")>
    Public Function readKCF(data As String) As Model.KCF
        Return Model.IO.LoadKCF(data)
    End Function

    ''' <summary>
    ''' Create molecular network graph model
    ''' </summary>
    ''' <param name="kcf">The KCF molecule model</param>
    ''' <returns></returns>
    <ExportAPI("KCF.graph")>
    Public Function CreateGraph(kcf As Model.KCF) As NetworkGraph
        Return kcf.CreateGraph
    End Function

    ''' <summary>
    ''' parse a single sdf text block
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="parseStruct"></param>
    ''' <returns></returns>
    <ExportAPI("parse.SDF")>
    Public Function readSDF(data As String, Optional parseStruct As Boolean = True) As SDF
        Return SDF.ParseSDF(data.SolveStream, parseStruct)
    End Function

    <ExportAPI("SDF.convertKCF")>
    Public Function SDF2KCF(sdfModel As SDF) As Model.KCF
        Return sdfModel.ToKCF
    End Function

    <ExportAPI("download.kcf")>
    Public Function DownloadKCF(keggcompoundIDs As String(), save$) As Object
        Dim result As New List(Of Object)
        Dim KCF$

        For Each id As String In keggcompoundIDs.SafeQuery
            KCF = MetaboliteWebApi.DownloadKCF(id, saveDIR:=save)

            Call result.Add(KCF)
            Call Thread.Sleep(1000)
        Next

        Return result.ToArray
    End Function

    ''' <summary>
    ''' open the file handles of the chemical descriptor database. 
    ''' </summary>
    ''' <param name="dbFile">
    ''' A directory path which contains the multiple database file of the 
    ''' chemical descriptors.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("open.descriptor.db")>
    Public Function openChemicalDescriptorDatabase(dbFile As String) As PubChemDescriptorRepo
        Return New PubChemDescriptorRepo(dir:=dbFile)
    End Function

    <ExportAPI("descriptor.matrix")>
    <RApiReturn(GetType(DataSet()))>
    Public Function LoadChemicalDescriptorsMatrix(repo As PubChemDescriptorRepo, cid As Long(), Optional env As Environment = Nothing) As Object
        If repo Is Nothing Then
            Return Internal.debug.stop("no chemical descriptor database was provided!", env)
        ElseIf cid.IsNullOrEmpty Then
            Return Nothing
        End If

        Dim matrix As New List(Of DataSet)
        Dim descriptor As ChemicalDescriptor
        Dim row As DataSet

        For Each id As Long In cid
            descriptor = repo.GetDescriptor(cid:=id)
            row = New DataSet With {
                .ID = id,
                .Properties = New Dictionary(Of String, Double) From {
                    {NameOf(descriptor.AtomDefStereoCount), descriptor.AtomDefStereoCount},
                    {NameOf(descriptor.AtomUdefStereoCount), descriptor.AtomUdefStereoCount},
                    {NameOf(descriptor.BondDefStereoCount), descriptor.BondDefStereoCount},
                    {NameOf(descriptor.BondUdefStereoCount), descriptor.BondUdefStereoCount},
                    {NameOf(descriptor.Complexity), descriptor.Complexity},
                    {NameOf(descriptor.ComponentCount), descriptor.ComponentCount},
                    {NameOf(descriptor.ExactMass), descriptor.ExactMass},
                    {NameOf(descriptor.FormalCharge), descriptor.FormalCharge},
                    {NameOf(descriptor.HeavyAtoms), descriptor.HeavyAtoms},
                    {NameOf(descriptor.HydrogenAcceptor), descriptor.HydrogenAcceptor},
                    {NameOf(descriptor.HydrogenDonors), descriptor.HydrogenDonors},
                    {NameOf(descriptor.IsotopicAtomCount), descriptor.IsotopicAtomCount},
                    {NameOf(descriptor.RotatableBonds), descriptor.RotatableBonds},
                    {NameOf(descriptor.TautoCount), descriptor.TautoCount},
                    {NameOf(descriptor.TopologicalPolarSurfaceArea), descriptor.TopologicalPolarSurfaceArea},
                    {NameOf(descriptor.XLogP3), descriptor.XLogP3},
                    {NameOf(descriptor.XLogP3_AA), descriptor.XLogP3_AA}
                }
            }

            If Not row.Properties.Values.All(Function(x) x = 0.0) Then
                ' is not empty
                matrix += row
            End If
        Next

        Return matrix.ToArray
    End Function

    <ExportAPI("parseSMILES")>
    Public Function parseSMILES(SMILES As String) As ChemicalFormula
        Return ParseChain.ParseGraph(SMILES)
    End Function

    <ExportAPI("as.formula")>
    Public Function asFormula(SMILES As ChemicalFormula) As Formula
        Return SMILES.GetFormula
    End Function
End Module
