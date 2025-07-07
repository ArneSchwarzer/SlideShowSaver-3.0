Imports System.Drawing

Public Module LanguageHelper

    Public Structure SprachInformation
        Public Sprache As String              ' z. B. "Deutsch"
        Public ToolTipText As String          ' z. B. "हिन्दी"
        Public btnTag As String               ' z. B. "LangDE"
        Public ISOCode As String              ' z. B. "DE"
        Public Font As Font                   ' z. B. Segoe UI, Nirmala UI, Microsoft YaHei etc.
        Public FlaggeAktiv As Image
        Public FlaggeInaktiv As Image
    End Structure

    Public SprachenListe As New List(Of SprachInformation) From {
        New SprachInformation With {
            .Sprache = "Deutsch",
            .ToolTipText = "Deutsch",
            .btnTag = "LangDE",
            .ISOCode = "DE",
            .Font = New Font("Segoe UI", 10, FontStyle.Regular),
            .FlaggeAktiv = My.Resources.Flagge_Deutschland,
            .FlaggeInaktiv = My.Resources.Flagge_Deutschland_sw
        },
        New SprachInformation With {
            .Sprache = "Englisch",
            .ToolTipText = "English",
            .btnTag = "LangEN",
            .ISOCode = "EN",
            .Font = New Font("Segoe UI", 10, FontStyle.Regular),
            .FlaggeAktiv = My.Resources.Flagge_GB,
            .FlaggeInaktiv = My.Resources.Flagge_GB_sw
        },
        New SprachInformation With {
            .Sprache = "Französisch",
            .ToolTipText = "Français",
            .btnTag = "LangFR",
            .ISOCode = "FR",
            .Font = New Font("Segoe UI", 10, FontStyle.Regular),
            .FlaggeAktiv = My.Resources.flagge_Frankreich,
            .FlaggeInaktiv = My.Resources.flagge_Frankreich_sw
        },
        New SprachInformation With {
            .Sprache = "Hindi",
            .ToolTipText = "हिन्दी",
            .btnTag = "LangHI",
            .ISOCode = "HI",
            .Font = New Font("Nirmala UI", 10, FontStyle.Regular),
            .FlaggeAktiv = My.Resources.Flagge_Indien,
            .FlaggeInaktiv = My.Resources.Flagge_Indien_sw
        },
        New SprachInformation With {
            .Sprache = "Polnisch",
            .ToolTipText = "Polski",
            .btnTag = "LangPL",
            .ISOCode = "PL",
            .Font = New Font("Segoe UI", 10, FontStyle.Regular),
            .FlaggeAktiv = My.Resources.Flagge_Polen,
            .FlaggeInaktiv = My.Resources.Flagge_Polen_sw
        },
        New SprachInformation With {
            .Sprache = "Russisch",
            .ToolTipText = "Русский",
            .btnTag = "LangRU",
            .ISOCode = "RU",
            .Font = New Font("Segoe UI", 10, FontStyle.Regular),
            .FlaggeAktiv = My.Resources.Flagge_Russland,
            .FlaggeInaktiv = My.Resources.Flagge_Russland_sw
        },
        New SprachInformation With {
            .Sprache = "Spanisch",
            .ToolTipText = "Español",
            .btnTag = "LangES",
            .ISOCode = "ES",
            .Font = New Font("Segoe UI", 10, FontStyle.Regular),
            .FlaggeAktiv = My.Resources.Flagge_Spanien,
            .FlaggeInaktiv = My.Resources.Flagge_Spanien_sw
        },
        New SprachInformation With {
            .Sprache = "Klingonisch",
            .ToolTipText = "tlhIngan Hol",
            .btnTag = "LangKL",
            .ISOCode = "TLH",
            .Font = New Font("Segoe UI", 10, FontStyle.Regular), ' Optional alternative: "Code2000"
            .FlaggeAktiv = My.Resources.Flagge_Klingonen,
            .FlaggeInaktiv = My.Resources.Flagge_Klingonen_sw
        },
        New SprachInformation With {
            .Sprache = "Chinesisch",
            .ToolTipText = "中文",
            .btnTag = "LangZH",
            .ISOCode = "ZH",
            .Font = New Font("Microsoft YaHei", 10, FontStyle.Regular),
            .FlaggeAktiv = My.Resources.Flagge_China,
            .FlaggeInaktiv = My.Resources.Flagge_China_sw
        }
    }

End Module
