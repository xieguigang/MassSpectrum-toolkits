﻿Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.MetaDNA.Infer
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

Public Module ResultHandler

    <Extension>
    Public Iterator Function ExportTable(candidates As IEnumerable(Of CandidateInfer), kegg As KEGGHandler) As IEnumerable(Of MetaDNAResult)
        For Each infer As CandidateInfer In candidates
            Dim compound As Compound = kegg.GetCompound(infer.kegg_id)

            For Each type As Candidate In infer.infers
                Yield New MetaDNAResult With {
                    .exactMass = compound.exactMass,
                    .formula = compound.formula,
                    .id = type.infer.query.id,
                    .forward = type.infer.forward,
                    .reverse = type.infer.reverse,
                    .inferLevel = type.infer.level,
                    .KEGGId = infer.kegg_id,
                    .name = If(compound.commonNames.FirstOrDefault, compound.formula),
                    .ppm = type.ppm,
                    .precursorType = type.precursorType,
                    .pvalue = type.pvalue,
                    .partnerKEGGId = type.infer.reference.id,
                    .seed = type.infer.reference.id,
                    .mz = type.infer.query.mz,
                    .rt = type.infer.query.rt
                }
            Next
        Next
    End Function
End Module
