Imports System.Drawing

Public Class LanguageHelper
    Public Structure SprachInformation
        Public Sprache As String
        Public btnTag As String
        Public ISOCode As String
        Public Font As Font
        Public FlaggeAktiv As Image
        Public FlaggeInaktiv As Image
    End Structure

    Public Shared SprachenListe As New List(Of SprachInformation) From {
        New SprachInformation With {
        .Sprache = "Chinesisch",
        .btnTag = "LangZH",
        .ISOCode = "ZH",
        .Font = New Font("Microsoft YaHei", 10, FontStyle.Regular),
        .FlaggeAktiv = My.Resources.Flagge_China,
        .FlaggeInaktiv = My.Resources.Flagge_China_sw},
        New SprachInformation With {
        .Sprache = "Deutsch",
        .btnTag = "LangDE",
        .ISOCode = "DE",
        .Font = New Font("Segoe UI", 10, FontStyle.Regular),
        .FlaggeAktiv = My.Resources.Flagge_Deutschland,
        .FlaggeInaktiv = My.Resources.Flagge_Deutschland_sw},
        New SprachInformation With {
        .Sprache = "Englisch",
        .btnTag = "LangEN",
        .ISOCode = "EN",
        .Font = New Font("Segoe UI", 10, FontStyle.Regular),
        .FlaggeAktiv = My.Resources.Flagge_GB,
        .FlaggeInaktiv = My.Resources.Flagge_GB_sw},
        New SprachInformation With {
        .Sprache = "Französich",
        .btnTag = "LangFR",
        .ISOCode = "FR",
        .Font = New Font("Segoe UI", 10, FontStyle.Regular),
        .FlaggeAktiv = My.Resources.flagge_Frankreich,
        .FlaggeInaktiv = My.Resources.flagge_Frankreich_sw},
        New SprachInformation With {
        .Sprache = "Klingonisch",
        .btnTag = "LangTLH",
        .ISOCode = "TLH",
        .Font = New Font("Segoe UI", 10, FontStyle.Regular),
        .FlaggeAktiv = My.Resources.Flagge_Klingonen,
        .FlaggeInaktiv = My.Resources.Flagge_Klingonen_sw},
        New SprachInformation With {
        .Sprache = "Hindi",
        .btnTag = "LangHI",
        .ISOCode = "HI",
        .Font = New Font("Nirmala UI", 10, FontStyle.Regular),
        .FlaggeAktiv = My.Resources.Flagge_Indien,
        .FlaggeInaktiv = My.Resources.Flagge_Indien_sw},
        New SprachInformation With {
        .Sprache = "Polnisch",
        .btnTag = "LangPL",
        .ISOCode = "PL",
        .Font = New Font("Segoe UI", 10, FontStyle.Regular),
        .FlaggeAktiv = My.Resources.Flagge_Polen,
        .FlaggeInaktiv = My.Resources.Flagge_Polen_sw},
        New SprachInformation With {
        .Sprache = "Russisch",
        .btnTag = "LangRU",
        .ISOCode = "RU",
        .Font = New Font("Segoe UI", 10, FontStyle.Regular),
        .FlaggeAktiv = My.Resources.Flagge_Russland,
        .FlaggeInaktiv = My.Resources.Flagge_Russland_sw},
        New SprachInformation With {
        .Sprache = "Spanisch",
        .btnTag = "LangES",
        .ISOCode = "ES",
        .Font = New Font("Segoe UI", 10, FontStyle.Regular),
        .FlaggeAktiv = My.Resources.Flagge_Spanien,
        .FlaggeInaktiv = My.Resources.Flagge_Spanien_sw}
    }
End Class
