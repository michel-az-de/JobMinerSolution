using System.Drawing;
using System.Windows.Forms;
using JobMiner.Helpers;

namespace JobMiner.Helpers
{
    /// <summary>Factory de controles com estilo visual padronizado do JobMiner.</summary>
    internal static class UiHelper
    {
        // ?? Fontes ?????????????????????????????????????????????????????
        public static Font FontMono(float size, FontStyle style = FontStyle.Regular)
   => new Font("Courier New", size, style);

        public static Font FontMonoBold(float size) => FontMono(size, FontStyle.Bold);

  // ?? Botão de ação (painel de detalhe / sidebar) ????????????????
        public static Button ActionButton(string text, Color bg, Color fg, Size size)
        {
            var btn = new Button
  {
      Text         = text,
   Size    = size,
              BackColor    = bg,
           ForeColor = fg,
     FlatStyle    = FlatStyle.Flat,
                Font         = FontMono(8.5f, FontStyle.Bold),
   Cursor       = Cursors.Hand,
     TextAlign  = ContentAlignment.MiddleCenter,
           UseVisualStyleBackColor = false,
            };
            btn.FlatAppearance.BorderColor = Blend(bg, fg, 0.25f);
     btn.FlatAppearance.BorderSize  = 1;
            btn.FlatAppearance.MouseOverBackColor  = Lighten(bg, 20);
         btn.FlatAppearance.MouseDownBackColor  = Darken(bg, 10);
      return btn;
    }

        /// <summary>Botão de sucesso (marcar enviado).</summary>
        public static Button SuccessButton(string text, Size size)
            => ActionButton(text, ColorHelper.BtnSuccessBg, ColorHelper.BtnSuccessFg, size);

   /// <summary>Botão de perigo (excluir).</summary>
        public static Button DangerButton(string text, Size size)
     => ActionButton(text, ColorHelper.BtnDangerBg, ColorHelper.BtnDangerFg, size);

        /// <summary>Botão neutro (abrir URL, etc.).</summary>
        public static Button NeutralButton(string text, Size size)
  => ActionButton(text, ColorHelper.BtnNeutralBg, ColorHelper.BtnNeutralFg, size);

        // ?? Label de campo (rótulo âmbar) ??????????????????????????????
        public static Label FieldLabel(string text)
    => new Label
       {
 Text      = text.ToUpper(),
       ForeColor = ColorHelper.AmberDim,
        Font      = FontMono(7.5f, FontStyle.Bold),
        AutoSize  = true,
            };

        // ?? Label de valor ?????????????????????????????????????????????
   public static Label ValueLabel(string text, float fontSize = 9.5f)
            => new Label
            {
          Text      = text,
         ForeColor = ColorHelper.BodyText,
          Font      = FontMono(fontSize),
            AutoSize  = true,
     };

// ?? Label de badge colorido por status ?????????????????????????
        public static Label StatusBadge(string text, Color bg, Color fg)
    {
            var lbl = new Label
            {
    Text      = $" {text} ",
 ForeColor = fg,
 BackColor = bg,
 Font      = FontMonoBold(8),
           AutoSize  = true,
                Padding   = new Padding(3, 1, 3, 1),
   };
            return lbl;
  }

        // ?? Separador horizontal ???????????????????????????????????????
        public static Panel HRule(Color color, int height = 1)
            => new Panel { Height = height, Dock = DockStyle.Top, BackColor = color };

        // ?? Utilitários de cor ?????????????????????????????????????????
        public static Color Lighten(Color c, int amount)
            => Color.FromArgb(
     c.A,
       Math.Min(255, c.R + amount),
      Math.Min(255, c.G + amount),
    Math.Min(255, c.B + amount));

        public static Color Darken(Color c, int amount)
   => Color.FromArgb(
      c.A,
                Math.Max(0, c.R - amount),
                Math.Max(0, c.G - amount),
    Math.Max(0, c.B - amount));

        public static Color Blend(Color a, Color b, float t)
 => Color.FromArgb(
         a.A,
     (int)(a.R * (1 - t) + b.R * t),
  (int)(a.G * (1 - t) + b.G * t),
      (int)(a.B * (1 - t) + b.B * t));
    }
}
