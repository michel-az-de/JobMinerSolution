using System.Drawing;

namespace JobMiner.Helpers
{
    /// <summary>
    /// Paleta JobMiner — Dark Warm Terminal.
    /// Uma única família de tons carvão-quente + acento âmbar.
    /// Sem mistura claro/escuro: tudo vive na mesma curva de valor.
    /// </summary>
    public static class ColorHelper
    {
        // ?? Superfícies (do mais escuro ao mais claro) ?????????????????
        /// <summary>Fundo do chrome / TitleBar / StatusStrip.</summary>
        public static Color Chrome => Color.FromArgb(18, 16, 12);   // #12100C
        /// <summary>Fundo do form e painéis principais.</summary>
        public static Color Base => Color.FromArgb(26, 24, 18);   // #1A1812
        /// <summary>Fundo de painéis elevados (TopRow, Detail).</summary>
        public static Color Surface => Color.FromArgb(36, 33, 26);   // #24211A
        /// <summary>Fundo do grid / células.</summary>
        public static Color GridBg => Color.FromArgb(28, 26, 20);   // #1C1A14
        /// <summary>Linhas alternadas do grid (zebra).</summary>
        public static Color GridAlt => Color.FromArgb(32, 30, 23);   // #201E17
        /// <summary>Fundo de campos de input / ToolStrip.</summary>
        public static Color InputBg => Color.FromArgb(42, 39, 30);   // #2A271E
        /// <summary>Borda sutil entre seções.</summary>
        public static Color Divider => Color.FromArgb(52, 48, 38);   // #343026

        // ?? Texto ??????????????????????????????????????????????????????
        /// <summary>Texto principal — creme quente.</summary>
        public static Color TextPrimary => Color.FromArgb(212, 205, 185);   // #D4CDB9
        /// <summary>Texto secundário / labels.</summary>
        public static Color TextSecond => Color.FromArgb(140, 132, 110);   // #8C846E
        /// <summary>Texto desabilitado / riscado.</summary>
        public static Color TextMuted => Color.FromArgb(80, 76, 62);   // #504C3E

        // ?? Âmbar (acento único) ???????????????????????????????????????
        /// <summary>Acento principal — bordas, títulos, ícones ativos.</summary>
        public static Color Amber => Color.FromArgb(200, 136, 10);   // #C8880A
        /// <summary>Âmbar brilhante — texto de destaque, valores.</summary>
        public static Color AmberHi => Color.FromArgb(240, 170, 40);   // #F0AA28
        /// <summary>Âmbar escurecido — títulos de seção, subtextos.</summary>
        public static Color AmberLo => Color.FromArgb(110, 74, 10);   // #6E4A0A
        /// <summary>Fundo para badges/botões âmbar.</summary>
        public static Color AmberBg => Color.FromArgb(44, 30, 4);   // #2C1E04

        // ?? Seleção ????????????????????????????????????????????????????
        public static Color SelectBg => Color.FromArgb(60, 44, 10);   // #3C2C0A
        public static Color SelectFg => Color.FromArgb(240, 216, 130);   // #F0D882

        // ?? Cabeçalho do grid ??????????????????????????????????????????
        public static Color HeaderBg => Color.FromArgb(22, 20, 15);   // #16140F
        public static Color HeaderFg => Color.FromArgb(180, 170, 140);// #B4AA8C
        public static Color GridLine => Color.FromArgb(44, 42, 32);   // #2C2A20

        // ?? Status semânticos (todos vivem no mesmo valor escuro) ??????
        public static Color NovaFg => Color.FromArgb(90, 190, 120);   // #5ABE78 verde musgo
        public static Color PendFg => Color.FromArgb(210, 150, 30);   // #D29620 âmbar
        public static Color SendFg => Color.FromArgb(100, 130, 180);   // #6482B4 azul-acinzentado
        public static Color DelFg => Color.FromArgb(90, 84, 68);   // #5A5444 cinza apagado

        // ?? Botões de ação ?????????????????????????????????????????????
        public static Color BtnPrimary => Color.FromArgb(50, 36, 4); // #322404 âmbar escuro
        public static Color BtnPrimaryFg => Color.FromArgb(240, 170, 40);   // #F0AA28
        public static Color BtnSuccess => Color.FromArgb(18, 44, 28);   // #122C1C verde escuro
        public static Color BtnSuccessFg => Color.FromArgb(80, 190, 120);   // #50BE78
        public static Color BtnDanger => Color.FromArgb(50, 18, 18);   // #321212 vermelho escuro
        public static Color BtnDangerFg => Color.FromArgb(210, 90, 90);   // #D25A5A
        public static Color BtnNeutral => Color.FromArgb(40, 38, 28);   // #28261C neutro
        public static Color BtnNeutralFg => Color.FromArgb(160, 152, 128);   // #A09880

        // ?? Aliases de compatibilidade (usados no código existente) ????
        public static Color FormBackground => Base;
        public static Color WindowBackground => GridBg;
        public static Color ChromeBackground => Chrome;
        public static Color ToolbarBackground => Surface;
        public static Color PanelDetail => Surface;
        public static Color BodyText => TextPrimary;
        public static Color DimText => TextSecond;
        public static Color MutedText => TextMuted;
        public static Color AmberAccent => Amber;
        public static Color AmberBright => AmberHi;
        public static Color AmberDim => AmberLo;
        public static Color GridHeaderBg => HeaderBg;
        public static Color GridHeaderFg => HeaderFg;
        public static Color GridAltRow => GridAlt;
        public static Color SelectionBg => SelectBg;
        public static Color SelectionFg => SelectFg;
        public static Color ChromeText => TextSecond;
        public static Color BorderLo => Divider;
        public static Color BorderHi => Color.FromArgb(64, 60, 46);
        public static Color BorderMid => Color.FromArgb(56, 52, 40);
        public static Color BtnDangerBg => BtnDanger;
        public static Color BtnDangerHover => UiHelper.Lighten(BtnDanger, 14);
        public static Color BtnSuccessBg => BtnSuccess;
        public static Color BtnSuccessHover => UiHelper.Lighten(BtnSuccess, 14);
        public static Color BtnNeutralBg => BtnNeutral;
        public static Color BtnNeutralHover => UiHelper.Lighten(BtnNeutral, 14);
        public static Color BadgeNovaFg => NovaFg;
        public static Color BadgePendenteFg => PendFg;
    }
}