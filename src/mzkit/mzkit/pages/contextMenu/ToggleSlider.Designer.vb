﻿Imports System.ComponentModel
Imports System.Drawing


Partial Class ToggleSlider
        ''' <summary> 
        ''' Required designer variable.
        ''' </summary>
        Private components As IContainer = Nothing

        ''' <summary> 
        ''' Clean up any resources being used.
        ''' </summary>
        ''' <paramname="disposing">true if managed resources should be disposed; otherwise, false.</param>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If

            MyBase.Dispose(disposing)
        End Sub

#Region "Component Designer generated code"

        ''' <summary> 
        ''' Required method for Designer support - do not modify 
        ''' the contents of this method with the code editor.
        ''' </summary>
        Private Sub InitializeComponent()
            SuspendLayout()
            ' 
            ' ToggleSliderComponent
            ' 
            AutoScaleDimensions = New SizeF(6.0F, 13.0F)
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Name = "ToggleSliderComponent"
            Size = New Size(231, 42)
            ResumeLayout(False)
        End Sub

#End Region
    End Class

