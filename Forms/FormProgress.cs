using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using JobMiner.Helpers;

namespace JobMiner.Forms
{
    public class FormProgress : Form
    {
  private Label       lblEtapa;
        private Panel       pnlTrack;     // trilho da barra
        private Panel       pnlFill;      // preenchimento âmbar
   private Label       lblPercent;
     private Button      btnCancelar;
  private BackgroundWorker _worker;

        public FormProgress(BackgroundWorker worker)
  {
    _worker = worker;
    InitializeComponent();
     }

   private void InitializeComponent()
   {
     SuspendLayout();

  this.FormBorderStyle = FormBorderStyle.None;
      this.StartPosition   = FormStartPosition.CenterParent;
    this.ClientSize      = new Size(360, 150);
          this.BackColor       = ColorHelper.Base;
   this.ShowInTaskbar   = false;
   this.TopMost = true;

            this.Paint += (s, e) =>
  {
        using var p = new Pen(ColorHelper.Amber, 1);
    e.Graphics.DrawRectangle(p, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
 };

   // ?? TitleBar ??????????????????????????????????????????????
   var pnlTitle = new Panel
     {
    Height    = 30,
 Dock    = DockStyle.Top,
  BackColor = ColorHelper.Chrome,
      };
   pnlTitle.Paint += (s, e) =>
     {
 using var p = new Pen(ColorHelper.Amber, 1);
          e.Graphics.DrawLine(p, 0, pnlTitle.Height - 1, pnlTitle.Width, pnlTitle.Height - 1);
    };

    var lGlyph = new Label
  {
  Text      = "?",
    ForeColor = ColorHelper.Amber,
      Font      = UiHelper.FontMonoBold(9),
 Location  = new Point(8, 4),
      AutoSize  = true,
   BackColor = Color.Transparent,
         };
  var lTitle = new Label
       {
    Text      = "PESQUISANDO VAGAS",
   ForeColor = ColorHelper.AmberHi,
   Font      = UiHelper.FontMonoBold(8.5f),
  Location  = new Point(28, 7),
       AutoSize  = true,
  BackColor = Color.Transparent,
  };
pnlTitle.Controls.Add(lGlyph);
   pnlTitle.Controls.Add(lTitle);
   this.Controls.Add(pnlTitle);

     // ?? Body ??????????????????????????????????????????????????
    var pnlBody = new Panel
            {
       Dock      = DockStyle.Fill,
      BackColor = ColorHelper.Surface,
     Padding   = new Padding(14, 10, 14, 10),
  };

          lblEtapa = new Label
            {
      Text      = "Iniciando…",
     ForeColor = ColorHelper.TextPrimary,
 Dock  = DockStyle.Top,
             Height    = 22,
  TextAlign = ContentAlignment.MiddleLeft,
  Font      = UiHelper.FontMono(9),
 BackColor = Color.Transparent,
      };
     pnlBody.Controls.Add(lblEtapa);

    pnlBody.Controls.Add(new Panel
    { Height = 6, Dock = DockStyle.Top, BackColor = Color.Transparent });

  // trilho
   pnlTrack = new Panel
   {
  Height    = 10,
 Dock      = DockStyle.Top,
   BackColor = ColorHelper.Chrome,
   };
     pnlFill = new Panel
    {
  Width     = 0,
     Dock      = DockStyle.Left,
  BackColor = ColorHelper.Amber,
    };
        pnlTrack.Controls.Add(pnlFill);
   pnlBody.Controls.Add(pnlTrack);

      pnlBody.Controls.Add(new Panel
     { Height = 4, Dock = DockStyle.Top, BackColor = Color.Transparent });

  lblPercent = new Label
{
  Text      = "0%",
      ForeColor = ColorHelper.TextSecond,
         Dock      = DockStyle.Top,
   Height    = 16,
     TextAlign = ContentAlignment.MiddleRight,
  Font      = UiHelper.FontMono(7.5f),
    BackColor = Color.Transparent,
  };
    pnlBody.Controls.Add(lblPercent);

    btnCancelar = new Button
            {
 Text      = "Cancelar",
   Dock      = DockStyle.Bottom,
  Height    = 28,
     FlatStyle = FlatStyle.Flat,
  BackColor = ColorHelper.BtnDanger,
  ForeColor = ColorHelper.BtnDangerFg,
      Font    = UiHelper.FontMonoBold(8.5f),
             Cursor    = Cursors.Hand,
    };
     btnCancelar.FlatAppearance.BorderColor = ColorHelper.BtnDangerFg;
            btnCancelar.FlatAppearance.BorderSize  = 1;
    btnCancelar.Click += (s, e) =>
    {
      if (_worker?.WorkerSupportsCancellation == true) _worker.CancelAsync();
     this.Close();
      };
  pnlBody.Controls.Add(btnCancelar);
   this.Controls.Add(pnlBody);

       ResumeLayout(true);
        }

    public void ReportProgress(int percent, string etapa)
        {
     if (InvokeRequired) { Invoke(() => Update(percent, etapa)); return; }
          Update(percent, etapa);
     }

        private void Update(int percent, string etapa)
        {
    percent = Math.Clamp(percent, 0, 100);
   pnlFill.Width     = (int)(pnlTrack.Width * percent / 100.0);
  pnlFill.BackColor = percent >= 100 ? ColorHelper.NovaFg : ColorHelper.Amber;
  lblEtapa.Text     = etapa;
   lblPercent.Text   = $"{percent}%";
   }

  protected override CreateParams CreateParams
        {
  get { var cp = base.CreateParams; cp.ClassStyle |= 0x00020000; return cp; }
        }
 }
}