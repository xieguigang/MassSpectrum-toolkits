﻿Imports System.Runtime.InteropServices

Namespace DataObjects

    ''' <summary>
    ''' Container for metadata relating to a single scan
    ''' </summary>
    <CLSCompliant(True)>
    Public Class SingleScanInfo

#Region "Properties"

        ''' <summary>
        ''' UTC Time that this scan info was cached
        ''' </summary>
        ''' <remarks>Used for determining which cached scan info can be discarded if too many scans become cached</remarks>
        Public ReadOnly Property CacheDateUTC As Date

        ''' <summary>
        ''' Scan number
        ''' </summary>
        Public ReadOnly Property ScanNumber As Integer

        ''' <summary>
        ''' MS Level
        ''' </summary>
        ''' <returns>MS acquisition level, where 1 means MS, 2 means MS/MS, 3 means MS^3 aka MS/MS/MS</returns>
        Public Property MSLevel As Integer

        ''' <summary>
        ''' Event Number
        ''' </summary>
        ''' <returns>1 for parent-ion scan; 2 for 1st frag scan, 3 for 2nd frag scan, etc.</returns>
        Public Property EventNumber As Integer

        ''' <summary>
        ''' SIM Scan Flag
        ''' </summary>
        ''' <returns>True if this is a selected ion monitoring (SIM) scan (i.e. a small mass range is being examined)</returns>
        ''' <remarks>If multiple selected ion ranges are examined simultaneously, then this will be false but MRMScanType will be .MRMQMS</remarks>
        Public Property SIMScan As Boolean

        ''' <summary>
        ''' Multiple reaction monitoring mode
        ''' </summary>
        ''' <returns>1 or 2 if this is a multiple reaction monitoring scan (MRMQMS or SRM)</returns>
        Public Property MRMScanType As MRMScanTypeConstants

        ''' <summary>
        ''' Zoom scan flag
        ''' </summary>
        ''' <returns>True when the given scan is a zoomed in mass region</returns>
        ''' <remarks>These spectra are typically skipped when creating SICs</remarks>
        Public Property ZoomScan As Boolean

        ''' <summary>
        ''' Number of mass intensity value pairs
        ''' </summary>
        ''' <returns>Number of points, -1 if unknown</returns>
        Public Property NumPeaks As Integer

        ''' <summary>
        ''' Retention time (in minutes)
        ''' </summary>
        Public Property RetentionTime As Double

        ''' <summary>
        ''' Lowest m/z value
        ''' </summary>
        Public Property LowMass As Double

        ''' <summary>
        ''' Highest m/z value
        ''' </summary>
        Public Property HighMass As Double

        ''' <summary>
        ''' Total ion current
        ''' </summary>
        ''' <returns>Sum of all ion abundances</returns>
        Public Property TotalIonCurrent As Double

        ''' <summary>
        ''' Base peak m/z
        ''' </summary>
        ''' <returns>m/z value of the most abundant ion in the scan</returns>
        Public Property BasePeakMZ As Double

        ''' <summary>
        ''' Base peak intensity
        ''' </summary>
        ''' <returns>Intensity of the most abundant ion in the scan</returns>
        Public Property BasePeakIntensity As Double

        ''' <summary>
        ''' Scan Filter string
        ''' </summary>
        Public Property FilterText As String

        ''' <summary>
        ''' Parent ion m/z
        ''' </summary>
        Public Property ParentIonMZ As Double

        ''' <summary>
        ''' The monoisotopic parent ion m/z, as determined by the Thermo software
        ''' </summary>
        <Obsolete("Unused")>
        Public ReadOnly Property ParentIonMonoisotopicMZ As Double
            Get
                Dim value As String = Nothing

                If TryGetScanEvent("Monoisotopic M/Z:", value) Then
                    Return Convert.ToDouble(value)
                End If

                Return 0
            End Get
        End Property

        ''' <summary>
        ''' The monoisotopic parent ion m/z, as determined by the Thermo software
        ''' </summary>
        <Obsolete("Unused")>
        Public ReadOnly Property IsolationWindowTargetMZ As Double
            Get
                Dim value As String = Nothing

                If TryGetScanEvent("MS2 Isolation Width:", value) Then
                    Return Convert.ToDouble(value)
                End If

                Return 0
            End Get
        End Property

        ''' <summary>
        ''' The parent ion charge state, as determined by the Thermo software
        ''' </summary>
        <Obsolete("Unused")>
        Public ReadOnly Property ChargeState As Integer
            Get
                Dim value As String = Nothing

                If TryGetScanEvent("Charge State:", value) Then
                    Return Convert.ToInt32(value)
                End If

                Return 0
            End Get
        End Property

        ''' <summary>
        ''' Activation type (aka activation method) as reported by the reader
        ''' </summary>
        ''' <remarks>Not applicable for MS1 scans (though will report 0=CID, which should be disregarded)</remarks>
        Public Property ActivationType As ActivationTypeConstants

        ''' <summary>
        ''' Collision mode, determined from the filter string
        ''' </summary>
        ''' <remarks>Typically CID, ETD, HCD, EThcD, or ETciD</remarks>
        Public Property CollisionMode As String

        ''' <summary>
        ''' Ionization polarity
        ''' </summary>
        Public Property IonMode As IonModeConstants

        ''' <summary>
        ''' MRM mode
        ''' </summary>
        Public Property MRMInfo As MRMInfo

        ''' <summary>
        ''' Number of channels
        ''' </summary>
        Public Property NumChannels As Integer

        ''' <summary>
        ''' Indicates whether the sampling time increment for the controller is constant
        ''' </summary>
        Public Property UniformTime As Boolean

        ''' <summary>
        ''' Sampling frequency for the current controller
        ''' </summary>
        Public Property Frequency As Double

        ''' <summary>
        ''' Ion Injection Time (in milliseconds)
        ''' </summary>
        Public IonInjectionTime As Double

        ''' <summary>
        ''' Centroid scan flag
        ''' </summary>
        ''' <returns>True if centroid (sticks) scan; False if profile (continuum) scan</returns>
        Public Property IsCentroided As Boolean

        ''' <summary>
        ''' FTMS flag (or Orbitrap, Q-Exactive, Lumos, or any other high resolution instrument)
        ''' </summary>
        ''' <returns>True if acquired on a high resolution mass analyzer (for example, on an Orbitrap or Q-Exactive)</returns>
        Public Property IsFTMS As Boolean

        ''' <summary>
        ''' Scan event data
        ''' </summary>
        ''' <returns>List of key/value pairs</returns>
        ''' <remarks>Ignores scan events with a blank or null event name</remarks>
        Public ReadOnly Property ScanEvents As List(Of KeyValuePair(Of String, String))

        ''' <summary>
        ''' Status log data
        ''' </summary>
        ''' <returns>List of key/value pairs</returns>
        ''' <remarks>Includes blank events that separate log sections</remarks>
        Public ReadOnly Property StatusLog As List(Of KeyValuePair(Of String, String))

#End Region

#Region "Constructor and public methods"

        ''' <summary>
        ''' Constructor with only scan number
        ''' </summary>
        Public Sub New(scan As Integer)
            NumPeaks = -1
            ScanNumber = scan
            CacheDateUTC = Date.UtcNow
            FilterText = String.Empty
            CollisionMode = String.Empty
            ActivationType = ActivationTypeConstants.Unknown
            ScanEvents = New List(Of KeyValuePair(Of String, String))()
            StatusLog = New List(Of KeyValuePair(Of String, String))()
        End Sub

        ''' <summary>
        ''' Store this scan's scan events using parallel string arrays
        ''' </summary>
        ''' <param name="eventNames"></param>
        ''' <param name="eventValues"></param>
        Public Sub StoreScanEvents(eventNames As String(), eventValues As String())
            StoreParallelStrings(ScanEvents, eventNames, eventValues, True, True)
        End Sub

        ''' <summary>
        ''' Store this scan's log messages using parallel string arrays
        ''' </summary>
        ''' <param name="logNames"></param>
        ''' <param name="logValues"></param>
        Public Sub StoreStatusLog(logNames As String(), logValues As String())
            StoreParallelStrings(StatusLog, logNames, logValues)
        End Sub

        ''' <summary>
        ''' Get the event value associated with the given scan event name
        ''' </summary>
        ''' <param name="eventName">Event name to find</param>
        ''' <param name="eventValue">Event value</param>
        ''' <param name="partialMatchToStart">Set to true to match the start of an event name, and not require a full match</param>
        ''' <returns>True if found a match for the event name, otherwise false</returns>
        ''' <remarks>Event names nearly always end in a colon, e.g. "Monoisotopic M/Z:" or "Charge State:"</remarks>
        Public Function TryGetScanEvent(eventName As String, <Out> ByRef eventValue As String, Optional partialMatchToStart As Boolean = False) As Boolean
            Dim results As IEnumerable(Of KeyValuePair(Of String, String))

            If partialMatchToStart Then
                ' Partial match
                results = From item In ScanEvents Where item.Key.StartsWith(eventName, StringComparison.OrdinalIgnoreCase) Select item
            Else
                results = From item In ScanEvents Where String.Equals(item.Key, eventName, StringComparison.OrdinalIgnoreCase) Select item
            End If

            For Each item In results
                eventValue = item.Value
                Return True
            Next

            eventValue = String.Empty
            Return False
        End Function

        ''' <summary>
        ''' Overridden ToString(): Displays a short summary of this object
        ''' </summary>
        Public Overrides Function ToString() As String
            If String.IsNullOrWhiteSpace(FilterText) Then
                Return "Scan " & ScanNumber & ": Generic ScanHeaderInfo"
            End If

            Return "Scan " & ScanNumber & ": " & FilterText
        End Function

#End Region

#Region "Private methods"

        Private Sub StoreParallelStrings(targetList As ICollection(Of KeyValuePair(Of String, String)), names As IList(Of String), values As IList(Of String), Optional skipEmptyNames As Boolean = False, Optional replaceTabsInValues As Boolean = False)
            targetList.Clear()

            For i = 0 To names.Count - 1

                If skipEmptyNames AndAlso (String.IsNullOrWhiteSpace(names(i)) OrElse Equals(names(i), CStr(Microsoft.VisualBasic.Strings.ChrW(1)))) Then
                    ' Name is empty or null
                    Continue For
                End If

                If replaceTabsInValues AndAlso values(CInt(i)).Contains(Microsoft.VisualBasic.Constants.vbTab) Then
                    targetList.Add(New KeyValuePair(Of String, String)(names(i), values(CInt(i)).Replace(CStr(Microsoft.VisualBasic.Constants.vbTab), CStr(" ")).TrimEnd(" "c)))
                Else
                    targetList.Add(New KeyValuePair(Of String, String)(names(i), values(i).TrimEnd(" "c)))
                End If
            Next
        End Sub
#End Region
    End Class
End Namespace