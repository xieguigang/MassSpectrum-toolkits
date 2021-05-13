﻿
Imports System.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Unit

<Convertor(GetType(Concentration))>
Public Enum Units As Integer

    NA = 0

    ''' <summary>
    ''' 百分之一``[%]``
    ''' </summary>
    <Description("%")> percent = 1
    ''' <summary>
    ''' 千分之一``[‰]``
    ''' </summary>
    <Description("‰")> permil
    ''' <summary>
    ''' ``液量盎司/加仑``(UK)
    ''' </summary>
    <Description("fl.oz/gallon(UK)")> floz_gallon_UK
    ''' <summary>
    ''' ``液量盎司/加仑``(US)
    ''' </summary>
    <Description("fl.oz/gallon(US)")> floz_gallon_US
    ''' <summary>
    ''' ``立方英寸/立方英尺``
    ''' </summary>
    <Description("cu.in/cu.ft")> cuin_cuft
    ''' <summary>
    ''' ``立方英寸/立方码``
    ''' </summary>
    <Description("cu.in/cu.yard")> cuin_cuyard
    ''' <summary>
    ''' ``滴/加仑``(UK)
    ''' </summary>
    <Description("drops/gallon(UK)")> drops_gallon_UK
    ''' <summary>
    ''' ``滴/加仑``(US)
    ''' </summary>
    <Description("drops/gallon(US)")> drops_gallon_US
    ''' <summary>
    ''' ``滴/立方英尺``
    ''' </summary>
    <Description("drops/cu.ft")> drops_cuft
    ''' <summary>
    ''' ``盎司/镑``
    ''' </summary>
    <Description("oz/pound")> oz_pound
    ''' <summary>
    ''' ``盎司/吨``(UK)
    ''' </summary>
    <Description("oz/ton(UK)")> oz_ton_UK
    ''' <summary>
    ''' ``盎司/吨``(US)
    ''' </summary>
    <Description("oz/ton(US)")> oz_ton_US
    ''' <summary>
    ''' Parts per million
    ''' </summary>
    <Description("ppm")> ppm
    ''' <summary>
    ''' 十亿分之一
    ''' </summary>
    <Description("parts/billion")> parts_billion
    ''' <summary>
    ''' ``毫升/升``
    ''' </summary>
    <Description("mL/litre")> mL_litre
    ''' <summary>
    ''' ``毫升/兆升``
    ''' </summary>
    <Description("mL/megalitre")> mL_megalitre
    ''' <summary>
    ''' ``毫升/立方米``
    ''' </summary>
    <Description("mL/cu.metre")> mL_cumetre
    ''' <summary>
    ''' ``滴/毫升``
    ''' </summary>
    <Description("drops/mL")> drops_mL
    ''' <summary>
    ''' ``滴/升``
    ''' </summary>
    <Description("drops/litre")> drops_litre
    ''' <summary>
    ''' ``滴/立方米``
    ''' </summary>
    <Description("drops/cu.metre")> drops_cumetre
    ''' <summary>
    ''' ``毫克/公斤`` 
    ''' </summary>
    <Description("milligrams/kg")> milligrams_kg
    ''' <summary>
    ''' ``克/公斤``
    ''' </summary>
    <Description("grams/kg")> grams_kg
    ''' <summary>
    ''' ``克/吨`` 
    ''' </summary>
    <Description("grams/tonne")> grams_tonne

End Enum
