﻿Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' The massbank data records
''' </summary>
Public Class Record : Implements INamedValue

    Public Property ACCESSION As String Implements INamedValue.Key
    Public Property RECORD_TITLE As String
    Public Property [DATE] As String
    Public Property AUTHORS As String
    Public Property LICENSE As String
    Public Property COPYRIGHT As String
    Public Property PUBLICATION As String
    Public Property COMMENT As String()
    Public Property CH As CH
    Public Property AC As AC
    Public Property MS As MS
    Public Property PK As PK

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

End Class

Public Structure CH

    Public Property NAME As String()
    Public Property COMPOUND_CLASS As String
    Public Property FORMULA As String
    Public Property EXACT_MASS As String
    Public Property SMILES As String
    Public Property IUPAC As String
    Public Property LINK As String()

End Structure

Public Structure AC

    Public Property INSTRUMENT As String
    Public Property INSTRUMENT_TYPE As String
    Public Property MASS_SPECTROMETRY As String()
    Public Property CHROMATOGRAPHY As String()

End Structure

Public Structure MS

    Public Property FOCUSED_ION As String()
    Public Property DATA_PROCESSING As String()

End Structure

Public Structure PK

    Public Property SPLASH As String
    Public Property ANNOTATION As AnnotationData()
    Public Property NUM_PEAK As String
    Public Property PEAK As PeakData()

End Structure

Public Structure AnnotationData

    <Column("m/z")>
    Public Property mz As Double
    Public Property tentative_formula As String
    Public Property formula_count As String
    Public Property mass As String
    <Column("error(ppm)")>
    Public Property delta_ppm As String

End Structure

Public Structure PeakData

    <Column("m/z")> Public Property mz As Double
    <Column("int.")> Public Property int As Double
    <Column("rel.int.")>
    Public Property relint As Double

End Structure