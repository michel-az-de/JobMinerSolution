using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using JobMiner.Data;
using JobMiner.Helpers;
using JobMiner.Models;

namespace JobMiner.Forms
{
    public class FormMain : Form
    {
        // ?? controls ??????????????????????????????????????????????????
      private Panel pnlTitleBar;
     private Button btnMinimize, btnMaximize, btnClose;
        private Panel pnlToolbar;        // substitui ToolStrip — controle total
        private Button tsbPesquisar, tsbLimpar, tsbExport;
      private TextBox txtFiltro;
        private DataGridView dgvVagas;
        private Panel pnlTopRow;
   private Panel pnlDetail;
        private Label lblDetailPlaceholder;
  private Panel pnlStatusBar;
        private Label lblStatusSistema, lblStatusWorker, lblContagem, lblStatusTexto;
  private RadioButton radTodos, radNova, radPendente, radEnviado, radExcluida;
     private CheckBox chkApinfo, chkLinkedIn, chkInfoJobs;
        private Button btnPesquisarSidebar;
  private Label lblTotal, lblNovas, lblPendentes, lblEnviados, lblSync;
        private ToolTip _tip;

        // ?? data ??????????????????????????????????????????????????????
        private List<Vaga> _vagasAtivas    = new();
 private List<Vaga> _vagasFiltradas = new();
        private BackgroundWorker _worker;
   private FormProgress _progressForm;
        private string _sortColumn = "";
        private bool   _sortAsc= true;

        // ?? drag ??????????????????????????????????????????????????????
  private bool  _dragging;
private Point _dragStart;

      // ?????????????????????????????????????????????????????????????
        public FormMain()
   {
          InitializeComponent();
            this.DoubleBuffered = true;
        InitData();
        }

     // ?????????????????????????????????????????????????????????????
        #region InitializeComponent
        private void InitializeComponent()
    {
            SuspendLayout();

            this.Text   = "JobMiner";
  this.ClientSize = new Size(1080, 720);
      this.MinimumSize   = new Size(860, 560);
            this.FormBorderStyle = FormBorderStyle.None;
       this.StartPosition   = FormStartPosition.CenterScreen;
 this.BackColor       = ColorHelper.Base;
        this.KeyPreview      = true;
     this.Font= UiHelper.FontMono(9.5f);

            _tip = new ToolTip { AutoPopDelay = 5000, InitialDelay = 500 };

            // ordem: Bottom ? Fill ? Top (último Top fica mais alto)
          BuildWorker();
            BuildStatusBar();
        BuildDetailPanel();
            BuildGrid();
        BuildTopRow();
            BuildToolbar();
  BuildTitleBar();

 this.KeyDown += FormMain_KeyDown;
            this.Paint   += (s, e) =>
     {
       // borda âmbar fina ao redor do form
     using var p = new Pen(ColorHelper.AmberLo, 1);
        e.Graphics.DrawRectangle(p, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
            };

            ResumeLayout(true);
  }
      #endregion

        // ?????????????????????????????????????????????????????????????
 #region TitleBar
        private void BuildTitleBar()
        {
            pnlTitleBar = new Panel
       {
     Height    = 34,
Dock      = DockStyle.Top,
    BackColor = ColorHelper.Chrome,
            };
         pnlTitleBar.Paint       += TitleBar_Paint;
          pnlTitleBar.MouseDown   += TitleBar_MouseDown;
         pnlTitleBar.MouseMove   += TitleBar_MouseMove;
    pnlTitleBar.MouseUp += (s, e) => _dragging = false;
 pnlTitleBar.DoubleClick += (s, e) => ToggleMaximize();

            // badge JM
            var badge = new Label
  {
     Text      = "JM",
        Size    = new Size(24, 16),
            Location  = new Point(8, 9),
                BackColor = ColorHelper.Amber,
 ForeColor = ColorHelper.Chrome,
         Font      = UiHelper.FontMonoBold(7),
    TextAlign = ContentAlignment.MiddleCenter,
     };
            pnlTitleBar.Controls.Add(badge);

            // título
      var lTitle = new Label
       {
    Text     = "JOBMINER",
       Location = new Point(38, 6),
          AutoSize = true,
  ForeColor = ColorHelper.AmberHi,
                Font     = UiHelper.FontMonoBold(10.5f),
        Cursor   = Cursors.SizeAll,
        BackColor = Color.Transparent,
   };
            pnlTitleBar.Controls.Add(lTitle);

          var lSub = new Label
   {
              Text      = "— Garimpo de Vagas",
           Location  = new Point(112, 10),
    AutoSize  = true,
                ForeColor = ColorHelper.TextSecond,
                Font      = UiHelper.FontMono(8),
         Cursor    = Cursors.SizeAll,
 BackColor = Color.Transparent,
   };
            pnlTitleBar.Controls.Add(lSub);

       // botões de chrome
          btnClose    = ChromeBtn("?", Color.FromArgb(140, 38, 28));
   btnMaximize = ChromeBtn("?", Color.FromArgb(36, 72, 36));
  btnMinimize = ChromeBtn("?", Color.FromArgb(54, 50, 36));

            btnClose.Click    += (s, e) => this.Close();
          btnMaximize.Click += (s, e) => ToggleMaximize();
            btnMinimize.Click += (s, e) => this.WindowState = FormWindowState.Minimized;

 _tip.SetToolTip(btnClose,    "Fechar (Alt+F4)");
            _tip.SetToolTip(btnMaximize, "Maximizar / Restaurar");
            _tip.SetToolTip(btnMinimize, "Minimizar");

            pnlTitleBar.Controls.AddRange(new Control[] { btnClose, btnMaximize, btnMinimize });
    pnlTitleBar.SizeChanged += (s, e) => LayoutChromeButtons();
        LayoutChromeButtons();

     this.Controls.Add(pnlTitleBar);
    }

        private void LayoutChromeButtons()
   {
       btnClose.Location    = new Point(pnlTitleBar.Width - 28,  0);
btnMaximize.Location = new Point(pnlTitleBar.Width - 56,  0);
    btnMinimize.Location = new Point(pnlTitleBar.Width - 84,  0);
        }

        private Button ChromeBtn(string glyph, Color hoverBg)
        {
  var b = new Button
            {
    Text      = glyph,
 Size  = new Size(28, 34),
     BackColor = Color.Transparent,
     ForeColor = ColorHelper.TextSecond,
  Font      = UiHelper.FontMono(8.5f),
          FlatStyle = FlatStyle.Flat,
Cursor    = Cursors.Hand,
    TabStop   = false,
          };
   b.FlatAppearance.BorderSize       = 0;
            b.FlatAppearance.MouseOverBackColor = hoverBg;
  b.FlatAppearance.MouseDownBackColor = UiHelper.Darken(hoverBg, 20);
    b.MouseEnter += (s, e) => b.ForeColor = Color.White;
            b.MouseLeave += (s, e) => b.ForeColor = ColorHelper.TextSecond;
  return b;
     }

        private void TitleBar_Paint(object sender, PaintEventArgs e)
        {
   // linha âmbar no fundo da TitleBar
            using var p = new Pen(ColorHelper.Amber, 1);
            e.Graphics.DrawLine(p, 0, pnlTitleBar.Height - 1, pnlTitleBar.Width, pnlTitleBar.Height - 1);
     }

        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || WindowState == FormWindowState.Maximized) return;
  _dragging  = true;
            _dragStart = new Point(e.X, e.Y);
        }
        private void TitleBar_MouseMove(object sender, MouseEventArgs e)
        {
   if (!_dragging) return;
   this.Location = Point.Subtract(Cursor.Position, (Size)_dragStart);
     }
private void ToggleMaximize() =>
       this.WindowState = WindowState == FormWindowState.Maximized
           ? FormWindowState.Normal : FormWindowState.Maximized;
        #endregion

        // ?????????????????????????????????????????????????????????????
        #region Toolbar (Panel customizado — sem ToolStrip)
 private void BuildToolbar()
        {
            pnlToolbar = new Panel
  {
          Height    = 36,
       Dock      = DockStyle.Top,
       BackColor = ColorHelper.Surface,
    Padding   = new Padding(6, 0, 6, 0),
            };
     pnlToolbar.Paint += (s, e) =>
         {
            // linha separadora inferior
     using var p = new Pen(ColorHelper.Divider, 1);
      e.Graphics.DrawLine(p, 0, pnlToolbar.Height - 1, pnlToolbar.Width, pnlToolbar.Height - 1);
   };

        int x = 8;

 // ?? Botão Pesquisar (acento âmbar) ???????????????????????
      tsbPesquisar = ToolbarButton("?  Pesquisar", ColorHelper.AmberBg, ColorHelper.AmberHi,
 ColorHelper.Amber);
    tsbPesquisar.Location = new Point(x, 4);
            tsbPesquisar.Click   += (s, e) => ExecutarPesquisa();
          _tip.SetToolTip(tsbPesquisar, "Pesquisar vagas (F5)");
            pnlToolbar.Controls.Add(tsbPesquisar);
       x += tsbPesquisar.Width + 6;

          // separador
     pnlToolbar.Controls.Add(VSep(x)); x += 9;

       // ?? Botão Limpar ?????????????????????????????????????????
tsbLimpar = ToolbarButton("?  Limpar", ColorHelper.Base, ColorHelper.TextSecond,
    ColorHelper.Divider);
   tsbLimpar.Location = new Point(x, 4);
      tsbLimpar.Click   += (s, e) => LimparResultados();
      pnlToolbar.Controls.Add(tsbLimpar);
            x += tsbLimpar.Width + 6;

            pnlToolbar.Controls.Add(VSep(x)); x += 9;

       // ?? Label Filtrar ????????????????????????????????????????
var lblF = new Label
      {
   Text      = "Filtrar:",
AutoSize  = true,
          ForeColor = ColorHelper.TextSecond,
      Font      = UiHelper.FontMono(8.5f),
           BackColor = Color.Transparent,
       };
  lblF.Location = new Point(x, 10);
       pnlToolbar.Controls.Add(lblF);
            x += lblF.PreferredWidth + 6;

  // ?? Caixa de filtro ??????????????????????????????????????
            txtFiltro = new TextBox
            {
         Location    = new Point(x, 6),
      Size = new Size(260, 22),
    BackColor   = ColorHelper.InputBg,
        ForeColor   = ColorHelper.TextSecond,
      Font        = UiHelper.FontMono(9),
              BorderStyle = BorderStyle.FixedSingle,
                Text        = "buscar vaga, site ou responsável...",
            };
 txtFiltro.GotFocus  += (s, e) =>
         {
 if (txtFiltro.Text == "buscar vaga, site ou responsável...")
            { txtFiltro.Text = ""; txtFiltro.ForeColor = ColorHelper.TextPrimary; }
        };
            txtFiltro.LostFocus += (s, e) =>
            {
  if (string.IsNullOrWhiteSpace(txtFiltro.Text))
{ txtFiltro.Text = "buscar vaga, site ou responsável..."; txtFiltro.ForeColor = ColorHelper.TextSecond; }
            };
            txtFiltro.TextChanged += (s, e) =>
            {
  if (txtFiltro.Text != "buscar vaga, site ou responsável...") AplicarFiltroTexto();
            };
  pnlToolbar.Controls.Add(txtFiltro);
        x += txtFiltro.Width + 6;

        pnlToolbar.Controls.Add(VSep(x)); x += 9;

        // ?? CSV ??????????????????????????????????????????????????
            tsbExport = ToolbarButton("?  CSV", ColorHelper.Base, ColorHelper.TextSecond,
    ColorHelper.Divider);
      tsbExport.Location = new Point(x, 4);
    tsbExport.Click   += (s, e) => ExportarCSV();
            _tip.SetToolTip(tsbExport, "Exportar CSV (Ctrl+E)");
      pnlToolbar.Controls.Add(tsbExport);

       this.Controls.Add(pnlToolbar);
  }

        private static Button ToolbarButton(string text, Color bg, Color fg, Color border)
        {
   var b = new Button
            {
     Text   = text,
          Size  = new Size(0, 28),
                AutoSize  = false,
                BackColor = bg,
         ForeColor = fg,
 Font      = UiHelper.FontMono(8.5f),
            FlatStyle = FlatStyle.Flat,
       Cursor    = Cursors.Hand,
            TabStop   = false,
            };
            // ajusta largura ao texto
   using var g = Graphics.FromHwnd(IntPtr.Zero);
       b.Width = (int)g.MeasureString(text, b.Font).Width + 20;

   b.FlatAppearance.BorderColor     = border;
b.FlatAppearance.BorderSize   = 1;
          b.FlatAppearance.MouseOverBackColor = UiHelper.Lighten(bg, 14);
          b.FlatAppearance.MouseDownBackColor = UiHelper.Darken(bg, 10);
            return b;
        }

  private static Label VSep(int x) => new Label
        {
            Text      = "",
            Location  = new Point(x, 6),
            Size      = new Size(1, 22),
            BackColor = ColorHelper.Divider,
   };
        #endregion

   // ?????????????????????????????????????????????????????????????
        #region TopRow
  private void BuildTopRow()
        {
            pnlTopRow = new Panel
  {
       Height    = 96,
  Dock    = DockStyle.Top,
     BackColor = ColorHelper.Surface,
       Padding   = new Padding(6, 6, 6, 6),
    };
            pnlTopRow.Paint += (s, e) =>
            {
     using var p = new Pen(ColorHelper.Divider, 1);
    e.Graphics.DrawLine(p, 0, pnlTopRow.Height - 1, pnlTopRow.Width, pnlTopRow.Height - 1);
    };

            var tl = new TableLayoutPanel
            {
      Dock        = DockStyle.Fill,
ColumnCount = 3,
           RowCount    = 1,
        BackColor   = Color.Transparent,
            };
  tl.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tl.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tl.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            tl.Controls.Add(BuildSites(),  0, 0);
    tl.Controls.Add(BuildStatus(), 1, 0);
       tl.Controls.Add(BuildResumo(), 2, 0);

            pnlTopRow.Controls.Add(tl);
            this.Controls.Add(pnlTopRow);
     }

        private Panel BuildSites()
  {
            var pnl = SectionPanel(200);
            pnl.Margin = new Padding(0, 0, 6, 0);
            DrawSectionTitle(pnl, "Sites de busca");

      chkApinfo   = DarkCheck("Apinfo",   new Point(8, 22), true);
 chkLinkedIn = DarkCheck("LinkedIn", new Point(8, 42), false);
    chkInfoJobs = DarkCheck("InfoJobs", new Point(8, 62), false);

     btnPesquisarSidebar = new Button
 {
       Text      = "? Pesquisar",
                Location  = new Point(100, 58),
                Size      = new Size(90, 24),
       BackColor = ColorHelper.AmberBg,
              ForeColor = ColorHelper.AmberHi,
     FlatStyle = FlatStyle.Flat,
                Font = UiHelper.FontMonoBold(8),
     Cursor    = Cursors.Hand,
      };
          btnPesquisarSidebar.FlatAppearance.BorderColor     = ColorHelper.Amber;
            btnPesquisarSidebar.FlatAppearance.BorderSize         = 1;
       btnPesquisarSidebar.FlatAppearance.MouseOverBackColor = UiHelper.Lighten(ColorHelper.AmberBg, 14);
 btnPesquisarSidebar.Click += (s, e) => ExecutarPesquisa();

 pnl.Controls.AddRange(new Control[] { chkApinfo, chkLinkedIn, chkInfoJobs, btnPesquisarSidebar });
            return pnl;
        }

        private Panel BuildStatus()
        {
        var pnl = SectionPanel(180);
            pnl.Margin = new Padding(0, 0, 6, 0);
            DrawSectionTitle(pnl, "Status");

    radTodos    = DarkRadio("Todos",     new Point(8,  22), true);
            radNova     = DarkRadio("Novas", new Point(8,  40), false);
        radPendente = DarkRadio("Pendentes", new Point(8,  58), false);
      radEnviado  = DarkRadio("Enviados",  new Point(95, 22), false);
            radExcluida = DarkRadio("Excluídas", new Point(95, 40), false);

          foreach (var r in new[] { radTodos, radNova, radPendente, radEnviado, radExcluida })
                r.CheckedChanged += (s, e) => AplicarFiltroStatus();

        pnl.Controls.AddRange(new Control[] { radTodos, radNova, radPendente, radEnviado, radExcluida });
   return pnl;
        }

        private Panel BuildResumo()
        {
            var pnl = new Panel
            {
  Dock    = DockStyle.Fill,
  BackColor = ColorHelper.Surface,
                Padding   = new Padding(8, 20, 8, 4),
  };
   DrawSectionTitle(pnl, "Resumo");

            var tl = new TableLayoutPanel
            {
                Dock        = DockStyle.Fill,
    ColumnCount = 4,
  RowCount    = 2,
     BackColor   = Color.Transparent,
  Padding     = new Padding(0),
       };
            for (int i = 0; i < 4; i++)
      tl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
     tl.RowStyles.Add(new RowStyle(SizeType.Absolute, 16));
    tl.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

tl.Controls.Add(ResCaption("Total"),    0, 0);
            tl.Controls.Add(ResCaption("Novas"),    1, 0);
    tl.Controls.Add(ResCaption("Pend."),    2, 0);
            tl.Controls.Add(ResCaption("Enviados"), 3, 0);

       lblTotal     = ResValue("—"); tl.Controls.Add(lblTotal,     0, 1);
         lblNovas     = ResValue("—"); tl.Controls.Add(lblNovas,     1, 1);
            lblPendentes = ResValue("—"); tl.Controls.Add(lblPendentes, 2, 1);
       lblEnviados  = ResValue("—"); tl.Controls.Add(lblEnviados,  3, 1);

       lblSync = new Label
            {
        Text      = "sync —",
     ForeColor = ColorHelper.TextSecond,
          Font      = UiHelper.FontMono(7.5f),
    Dock      = DockStyle.Bottom,
       Height    = 16,
                TextAlign = ContentAlignment.MiddleRight,
      BackColor = Color.Transparent,
  Padding   = new Padding(0, 0, 2, 0),
         };

            pnl.Controls.Add(tl);
 pnl.Controls.Add(lblSync);
         return pnl;
        }

        // helpers de seção
        private static Panel SectionPanel(int w) => new Panel
        {
            Width     = w,
       Dock      = DockStyle.Fill,
         BackColor = ColorHelper.Surface,
  Padding   = new Padding(8, 20, 8, 4),
    };

        private static void DrawSectionTitle(Panel pnl, string title)
        {
            pnl.Paint += (s, e) =>
      {
                using var f = UiHelper.FontMonoBold(7.5f);
         using var br  = new SolidBrush(ColorHelper.AmberLo);
         using var pen = new Pen(ColorHelper.Divider, 1);
    e.Graphics.DrawString(title.ToUpper(), f, br, new PointF(8, 4));
                e.Graphics.DrawRectangle(pen, 0, 0, pnl.Width - 1, pnl.Height - 1);
         };
 }

        private static CheckBox DarkCheck(string text, Point loc, bool chk) => new CheckBox
        {
        Text  = text,
      Location  = loc,
            Checked   = chk,
     ForeColor = ColorHelper.TextPrimary,
            BackColor = Color.Transparent,
            AutoSize  = true,
        };
        private static RadioButton DarkRadio(string text, Point loc, bool chk) => new RadioButton
     {
       Text   = text,
    Location  = loc,
         Checked   = chk,
         ForeColor = ColorHelper.TextPrimary,
    BackColor = Color.Transparent,
       AutoSize  = true,
     };
        private static Label ResCaption(string t) => new Label
        {
         Text      = t.ToUpper(),
        ForeColor = ColorHelper.TextSecond,
    Font      = UiHelper.FontMono(7.5f),
      Dock      = DockStyle.Fill,
    TextAlign = ContentAlignment.BottomLeft,
            BackColor = Color.Transparent,
        };
        private static Label ResValue(string t) => new Label
  {
  Text      = t,
   ForeColor = ColorHelper.AmberHi,
            Font    = UiHelper.FontMonoBold(15),
    Dock      = DockStyle.Fill,
    TextAlign = ContentAlignment.TopLeft,
 BackColor = Color.Transparent,
      };
        #endregion

      // ?????????????????????????????????????????????????????????????
        #region Grid
        private void BuildGrid()
        {
            dgvVagas = new DataGridView
            {
    Dock       = DockStyle.Fill,
      BackgroundColor         = ColorHelper.GridBg,
BorderStyle  = BorderStyle.None,
                GridColor          = ColorHelper.GridLine,
                SelectionMode   = DataGridViewSelectionMode.FullRowSelect,
     MultiSelect    = false,
         ReadOnly       = true,
      AllowUserToAddRows      = false,
        AllowUserToDeleteRows   = false,
    RowHeadersVisible       = false,
     AutoSizeColumnsMode     = DataGridViewAutoSizeColumnsMode.Fill,
      Font            = UiHelper.FontMono(9.5f),
                CellBorderStyle         = DataGridViewCellBorderStyle.SingleHorizontal,
   EnableHeadersVisualStyles = false,
       ColumnHeadersBorderStyle  = DataGridViewHeaderBorderStyle.None,
      Cursor           = Cursors.Hand,
         ShowCellToolTips        = true,
 };
        dgvVagas.RowTemplate.Height  = 24;
            dgvVagas.ColumnHeadersHeight = 28;
 dgvVagas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            var h = dgvVagas.ColumnHeadersDefaultCellStyle;
    h.BackColor     = ColorHelper.HeaderBg;
        h.ForeColor      = ColorHelper.HeaderFg;
 h.Font            = UiHelper.FontMonoBold(8.5f);
       h.Alignment          = DataGridViewContentAlignment.MiddleLeft;
            h.Padding            = new Padding(6, 0, 0, 0);
   h.SelectionBackColor = ColorHelper.HeaderBg;
      h.SelectionForeColor = ColorHelper.AmberHi;

            var d = dgvVagas.DefaultCellStyle;
         d.Font            = UiHelper.FontMono(9.5f);
d.ForeColor          = ColorHelper.TextPrimary;
            d.BackColor          = ColorHelper.GridBg;
  d.SelectionBackColor = ColorHelper.SelectBg;
       d.SelectionForeColor = ColorHelper.SelectFg;
     d.Padding            = new Padding(6, 0, 0, 0);

      dgvVagas.Columns.Add(GridCol("Num",        "#",            38,  4,  DataGridViewContentAlignment.MiddleCenter));
            dgvVagas.Columns.Add(GridCol("Site",       "Site",  82, 12,  DataGridViewContentAlignment.MiddleLeft));
        dgvVagas.Columns.Add(GridCol("Titulo",     "Título da Vaga",   0, 55,  DataGridViewContentAlignment.MiddleLeft));
  dgvVagas.Columns.Add(GridCol("Responsavel","Responsável",  150, 22, DataGridViewContentAlignment.MiddleLeft));
            dgvVagas.Columns.Add(GridCol("Status",     "Status",     94, 18, DataGridViewContentAlignment.MiddleCenter));
 dgvVagas.Columns.Add(GridCol("Data",  "Data",   76, 10, DataGridViewContentAlignment.MiddleCenter));

         dgvVagas.SelectionChanged      += DgvVagas_SelectionChanged;
     dgvVagas.CellDoubleClick       += DgvVagas_CellDoubleClick;
            dgvVagas.ColumnHeaderMouseClick += DgvVagas_ColumnHeaderMouseClick;
            dgvVagas.CellFormatting   += DgvVagas_CellFormatting;
       dgvVagas.CellMouseEnter += (s, e) =>
  {
      if (e.RowIndex < 0 || e.RowIndex >= _vagasFiltradas.Count) return;
           bool sel = dgvVagas.SelectedRows.Count > 0 && dgvVagas.SelectedRows[0].Index == e.RowIndex;
           if (!sel) dgvVagas.Rows[e.RowIndex].DefaultCellStyle.BackColor =
      UiHelper.Lighten(ColorHelper.GridBg, 12);
     };
  dgvVagas.CellMouseLeave += (s, e) =>
        {
    if (e.RowIndex >= 0 && e.RowIndex < _vagasFiltradas.Count)
 RowBg(dgvVagas.Rows[e.RowIndex], e.RowIndex);
 };

    this.Controls.Add(dgvVagas);
        }

    private static DataGridViewTextBoxColumn GridCol(string name, string header,
            int w, float fw, DataGridViewContentAlignment align)
    {
          var c = new DataGridViewTextBoxColumn
     {
        Name  = name,
  HeaderText       = header,
                FillWeight       = fw,
           DefaultCellStyle = { Alignment = align },
 };
   if (w > 0) c.Width = w;
       return c;
        }
      #endregion

        // ?????????????????????????????????????????????????????????????
        #region Detail Panel
      private void BuildDetailPanel()
   {
     pnlDetail = new Panel
      {
    Height    = 128,
    Dock   = DockStyle.Bottom,
     BackColor = ColorHelper.Surface,
           Padding   = new Padding(10, 8, 10, 6),
       };
    pnlDetail.Paint += (s, e) =>
            {
     using var p = new Pen(ColorHelper.Amber, 1);
           e.Graphics.DrawLine(p, 0, 0, pnlDetail.Width, 0);
            };

            lblDetailPlaceholder = new Label
            {
       Text      = "Selecione uma vaga para ver os detalhes   ·   duplo-clique abre no navegador",
     Dock= DockStyle.Fill,
                ForeColor = ColorHelper.TextSecond,
       Font      = UiHelper.FontMono(9, FontStyle.Italic),
            TextAlign = ContentAlignment.MiddleCenter,
     BackColor = Color.Transparent,
          };
    pnlDetail.Controls.Add(lblDetailPlaceholder);
       this.Controls.Add(pnlDetail);
        }
        #endregion

        // ?????????????????????????????????????????????????????????????
        #region Status Bar
     private void BuildStatusBar()
        {
            pnlStatusBar = new Panel
   {
        Height= 24,
      Dock      = DockStyle.Bottom,
                BackColor = ColorHelper.Chrome,
     Padding   = new Padding(8, 0, 8, 0),
            };
pnlStatusBar.Paint += (s, e) =>
 {
        using var p = new Pen(ColorHelper.Amber, 1);
    e.Graphics.DrawLine(p, 0, 0, pnlStatusBar.Width, 0);
   };

      lblStatusSistema = StatusLbl("? Sistema OK",     ColorHelper.TextSecond, false);
      lblStatusWorker  = StatusLbl("? Worker: idle",   ColorHelper.TextSecond, false);
  lblContagem      = StatusLbl("0 vagas",     ColorHelper.TextSecond, false);
         lblStatusTexto   = StatusLbl("Pronto.",    ColorHelper.TextSecond, true);

      var sep1 = StatusLbl("?", ColorHelper.Divider, false);
        var sep2 = StatusLbl("?", ColorHelper.Divider, false);

            // layout manual esquerda ? direita
            int lx = 6;
            foreach (var lbl in new[] { lblStatusSistema, sep1, lblStatusWorker, sep2, lblContagem })
    {
              lbl.Location = new Point(lx, 4);
       lbl.AutoSize = true;
      pnlStatusBar.Controls.Add(lbl);
                lx += lbl.PreferredWidth + 8;
            }
       lblStatusTexto.Dock      = DockStyle.Right;
      lblStatusTexto.Width     = 320;
    lblStatusTexto.TextAlign = ContentAlignment.MiddleRight;
      pnlStatusBar.Controls.Add(lblStatusTexto);

      this.Controls.Add(pnlStatusBar);
      }

        private static Label StatusLbl(string text, Color fg, bool spring) => new Label
        {
       Text      = text,
         ForeColor = fg,
    Font      = UiHelper.FontMono(8),
            BackColor = Color.Transparent,
    AutoSize  = !spring,
};
        #endregion

        // ?????????????????????????????????????????????????????????????
    #region Worker
        private void BuildWorker()
    {
        _worker = new BackgroundWorker
            {
       WorkerReportsProgress    = true,
                WorkerSupportsCancellation = true,
  };
      _worker.DoWork             += Worker_DoWork;
  _worker.ProgressChanged    += Worker_ProgressChanged;
  _worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        private void ExecutarPesquisa()
     {
            if (_worker.IsBusy) return;
            if (!chkApinfo.Checked && !chkLinkedIn.Checked && !chkInfoJobs.Checked)
  {
       MessageBox.Show("Selecione ao menos um site.", "Atenção",
   MessageBoxButtons.OK, MessageBoxIcon.Warning);
      return;
            }
         tsbPesquisar.Enabled        = false;
       btnPesquisarSidebar.Enabled = false;
 lblStatusWorker.Text     = "? Worker: executando";
     lblStatusWorker.ForeColor   = ColorHelper.Amber;
        lblStatusTexto.Text= "Pesquisando...";

     _progressForm = new FormProgress(_worker);
          _progressForm.Show(this);
  _worker.RunWorkerAsync(new List<string?>
            {
   chkApinfo.Checked   ? "Apinfo"   : null,
  chkLinkedIn.Checked ? "LinkedIn"  : null,
       chkInfoJobs.Checked ? "InfoJobs"  : null,
            });
  }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
      var w = sender as BackgroundWorker;
      var etapas = new[]
          {
            "Conectando aos sites…", "Raspando Apinfo…", "Raspando LinkedIn…",
      "Raspando InfoJobs…", "Filtrando por perfil…", "Removendo duplicatas…", "Concluído.",
            };
        for (int i = 0; i < etapas.Length; i++)
         {
   if (w!.CancellationPending) { e.Cancel = true; return; }
   Thread.Sleep(400);
    w.ReportProgress((int)((i + 1) / (double)etapas.Length * 100), etapas[i]);
 }
    var sel = new List<Vaga>();
            if (chkApinfo.Checked)   sel.AddRange(MockData._vagasMock.Where(v => v.Site == "Apinfo"));
          if (chkLinkedIn.Checked) sel.AddRange(MockData._vagasMock.Where(v => v.Site == "LinkedIn"));
    if (chkInfoJobs.Checked) sel.AddRange(MockData._vagasMock.Where(v => v.Site == "InfoJobs"));
      _vagasAtivas = sel.OrderByDescending(v => v.Match).ToList();
      e.Result = _vagasAtivas;
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
            => _progressForm?.ReportProgress(e.ProgressPercentage, e.UserState as string ?? "");

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            tsbPesquisar.Enabled = true;
            btnPesquisarSidebar.Enabled = true;
       lblStatusWorker.Text        = "? Worker: idle";
            lblStatusWorker.ForeColor   = ColorHelper.TextSecond;
            _progressForm?.Close();
            if (e.Cancelled) { lblStatusTexto.Text = "Cancelado."; return; }
        if (e.Error != null) { lblStatusTexto.Text = $"Erro: {e.Error.Message}"; return; }
        _vagasAtivas = e.Result as List<Vaga> ?? new();
         lblSync.Text = $"sync {DateTime.Now:HH:mm}";
      UpdateResumo();
            AplicarFiltroTexto();
        lblStatusTexto.Text = $"Concluído — {_vagasAtivas.Count} vaga(s).";
        }
     #endregion

        // ?????????????????????????????????????????????????????????????
        #region Grid population
 private void PopularGrid()
   {
            dgvVagas.SuspendLayout();
        dgvVagas.Rows.Clear();
            for (int i = 0; i < _vagasFiltradas.Count; i++)
   {
    var v   = _vagasFiltradas[i];
      int idx = dgvVagas.Rows.Add(
     (i + 1).ToString(), v.Site, v.Titulo,
            v.Responsavel, StatusLabel(v.Status), v.Data);
          RowBg(dgvVagas.Rows[idx], idx);
            }
      lblContagem.Text    = $"{_vagasFiltradas.Count} vaga(s)";
   lblStatusTexto.Text = "Pronto.";
          dgvVagas.ResumeLayout();
        }

     private static string StatusLabel(StatusVaga s) => s switch
{
       StatusVaga.Nova     => "? Nova",
          StatusVaga.Pendente => "? Pendente",
            StatusVaga.Enviado  => "? Enviado",
  StatusVaga.Excluida => "? Excluída",
 _ => s.ToString(),
        };

 private void RowBg(DataGridViewRow row, int idx)
   => row.DefaultCellStyle.BackColor = idx % 2 == 0
       ? ColorHelper.GridBg : ColorHelper.GridAlt;

        private void DgvVagas_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
       if (e.RowIndex < 0 || e.RowIndex >= _vagasFiltradas.Count) return;
  var v = _vagasFiltradas[e.RowIndex];

   if (dgvVagas.Columns[e.ColumnIndex].Name == "Titulo" && !string.IsNullOrEmpty(v.Url))
     dgvVagas.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = v.Url;

            switch (v.Status)
     {
       case StatusVaga.Enviado:
       e.CellStyle.ForeColor = ColorHelper.SendFg; break;
         case StatusVaga.Excluida:
         e.CellStyle.ForeColor = ColorHelper.DelFg;
         e.CellStyle.Font = new Font(dgvVagas.Font, FontStyle.Strikeout); break;
         case StatusVaga.Nova:
             if (dgvVagas.Columns[e.ColumnIndex].Name == "Status")
      e.CellStyle.ForeColor = ColorHelper.NovaFg; break;
   case StatusVaga.Pendente:
            if (dgvVagas.Columns[e.ColumnIndex].Name == "Status")
  e.CellStyle.ForeColor = ColorHelper.PendFg; break;
            }
        }
   #endregion

    // ?????????????????????????????????????????????????????????????
      #region Filters
        private void AplicarFiltroTexto()
        {
var txt  = txtFiltro.Text == "buscar vaga, site ou responsável..." ? "" : txtFiltro.Text.Trim();
     var list = _vagasAtivas.AsEnumerable();
            if (!string.IsNullOrEmpty(txt))
                list = list.Where(v =>
        v.Titulo.IndexOf(txt,       StringComparison.OrdinalIgnoreCase) >= 0 ||
            v.Site.IndexOf(txt,  StringComparison.OrdinalIgnoreCase) >= 0 ||
      v.Responsavel.IndexOf(txt,  StringComparison.OrdinalIgnoreCase) >= 0);
            list = ApplyStatusFilter(list);
      _vagasFiltradas = list.ToList();
         PopularGrid();
      }

        private IEnumerable<Vaga> ApplyStatusFilter(IEnumerable<Vaga> list)
    {
         if (radNova.Checked)     return list.Where(v => v.Status == StatusVaga.Nova);
  if (radPendente.Checked) return list.Where(v => v.Status == StatusVaga.Pendente);
            if (radEnviado.Checked)  return list.Where(v => v.Status == StatusVaga.Enviado);
            if (radExcluida.Checked) return list.Where(v => v.Status == StatusVaga.Excluida);
            return list;
 }

        private void AplicarFiltroStatus() => AplicarFiltroTexto();
     #endregion

        // ?????????????????????????????????????????????????????????????
        #region Actions
   private void LimparResultados()
   {
            _vagasAtivas.Clear();
     _vagasFiltradas.Clear();
       dgvVagas.Rows.Clear();
          lblTotal.Text = lblNovas.Text = lblPendentes.Text = lblEnviados.Text = "—";
          lblSync.Text        = "sync —";
      lblContagem.Text    = "0 vagas";
            lblStatusTexto.Text = "Pronto.";
   ShowDetailPlaceholder();
     }

        private void ExportarCSV()
  {
     if (_vagasFiltradas == null || _vagasFiltradas.Count == 0)
            {
 MessageBox.Show("Não há vagas para exportar.", "Atenção",
        MessageBoxButtons.OK, MessageBoxIcon.Information);
 return;
            }
 using var sfd = new SaveFileDialog
            {
     Filter   = "CSV (*.csv)|*.csv",
                FileName = $"jobminer-{DateTime.Now:yyyyMMdd}.csv",
      };
            if (sfd.ShowDialog() != DialogResult.OK) return;
      var lines = new List<string> { "#,Site,Titulo,Responsavel,Email,Telefone,URL,Status,Data" };
       for (int i = 0; i < _vagasFiltradas.Count; i++)
         {
       var v = _vagasFiltradas[i];
         lines.Add($"{i + 1},\"{v.Site}\",\"{v.Titulo}\",\"{v.Responsavel}\"," +
 $"\"{v.Email}\",\"{v.Telefone}\",\"{v.Url}\",{v.Status},{v.Data}");
    }
         System.IO.File.WriteAllText(sfd.FileName,
     string.Join(Environment.NewLine, lines),
          System.Text.Encoding.UTF8);
       Process.Start(new ProcessStartInfo(sfd.FileName) { UseShellExecute = true });
     }

      private void SetarStatusVaga(int id, StatusVaga novoStatus)
     {
            var v = _vagasAtivas.FirstOrDefault(x => x.Id == id);
            if (v == null) return;
            v.Status = novoStatus;
    int cur = dgvVagas.SelectedRows.Count > 0 ? dgvVagas.SelectedRows[0].Index : -1;
       AplicarFiltroTexto();
            UpdateResumo();
     if (cur >= 0 && cur < dgvVagas.Rows.Count)
                dgvVagas.Rows[cur].Selected = true;
        }

        private void UpdateResumo()
        {
lblTotal.Text     = _vagasAtivas.Count.ToString();
        lblNovas.Text     = _vagasAtivas.Count(x => x.Status == StatusVaga.Nova).ToString();
  lblPendentes.Text = _vagasAtivas.Count(x => x.Status == StatusVaga.Pendente).ToString();
        lblEnviados.Text  = _vagasAtivas.Count(x => x.Status == StatusVaga.Enviado).ToString();
        }

      private void InitData() => UpdateResumo();
        #endregion

        // ?????????????????????????????????????????????????????????????
        #region Selection / Detail
 private void DgvVagas_SelectionChanged(object sender, EventArgs e)
        {
    if (dgvVagas.SelectedRows.Count == 0) { ShowDetailPlaceholder(); return; }
       int idx = dgvVagas.SelectedRows[0].Index;
            if (idx < 0 || idx >= _vagasFiltradas.Count) { ShowDetailPlaceholder(); return; }
          ShowDetail(_vagasFiltradas[idx]);
        }

private void ShowDetailPlaceholder()
        {
            pnlDetail.Controls.Clear();
      pnlDetail.Controls.Add(lblDetailPlaceholder);
   }

    private void ShowDetail(Vaga v)
{
            pnlDetail.SuspendLayout();
      pnlDetail.Controls.Clear();

  var tl = new TableLayoutPanel
   {
      Dock        = DockStyle.Fill,
             ColumnCount = 2,
  RowCount    = 1,
       BackColor   = Color.Transparent,
    };
   tl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68));
          tl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32));
  tl.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // ?? campos ????????????????????????????????????????????????
        var left = new FlowLayoutPanel
            {
              Dock      = DockStyle.Fill,
           FlowDirection = FlowDirection.LeftToRight,
          WrapContents  = false,
         BackColor     = Color.Transparent,
       Padding  = new Padding(0),
            };
         left.Controls.Add(DField("Título",      Trunc(v.Titulo, 60),   320));
            left.Controls.Add(DField("Responsável", v.Responsavel,    190));

            if (!string.IsNullOrEmpty(v.Email))
        {
        var ef = DLinkField("E-mail", v.Email, 220);
         ef.Click += (s, _) =>
           Process.Start(new ProcessStartInfo($"mailto:{v.Email}") { UseShellExecute = true });
         left.Controls.Add(ef);
      }

            var uf = DLinkField("URL", Trunc(v.Url, 48), 270);
  uf.Click += (s, _) =>
    Process.Start(new ProcessStartInfo(v.Url) { UseShellExecute = true });
      _tip.SetToolTip(uf, v.Url);
 left.Controls.Add(uf);

     // ?? botões ????????????????????????????????????????????????
      var right = new FlowLayoutPanel
      {
      Dock          = DockStyle.Fill,
      FlowDirection = FlowDirection.TopDown,
     WrapContents  = false,
 BackColor     = Color.Transparent,
              Padding       = new Padding(12, 10, 8, 8),
            };

            var bEnv = UiHelper.SuccessButton("?  Marcar Enviado", new Size(162, 30));
   bEnv.Margin  = new Padding(0, 0, 0, 4);
 bEnv.Click  += (s, _) => SetarStatusVaga(v.Id, StatusVaga.Enviado);

var bAb = UiHelper.NeutralButton("?  Abrir no Site", new Size(162, 30));
         bAb.Margin  = new Padding(0, 0, 0, 4);
    bAb.Click  += (s, _) =>
     Process.Start(new ProcessStartInfo(v.Url) { UseShellExecute = true });

            var bEx = UiHelper.DangerButton("?  Excluir Vaga", new Size(162, 30));
            bEx.Click  += (s, _) => SetarStatusVaga(v.Id, StatusVaga.Excluida);

    right.Controls.AddRange(new Control[] { bEnv, bAb, bEx });

   tl.Controls.Add(left,  0, 0);
   tl.Controls.Add(right, 1, 0);
   pnlDetail.Controls.Add(tl);
 pnlDetail.ResumeLayout(true);
    }

      private static Panel DField(string label, string value, int width)
    {
    var pnl = new Panel { Width = width, Height = 120, BackColor = Color.Transparent,
           Padding = new Padding(0, 2, 10, 0) };
      var lbl = new Label
   {
         Text      = label.ToUpper(),
   ForeColor = ColorHelper.AmberLo,
            Font      = UiHelper.FontMonoBold(7.5f),
                Dock      = DockStyle.Top,
          Height = 15,
   BackColor = Color.Transparent,
            };
            var val = new Label
         {
     Text      = value,
       ForeColor = ColorHelper.TextPrimary,
       Font      = UiHelper.FontMono(9),
    Dock      = DockStyle.Fill,
                AutoSize  = false,
    BackColor = Color.Transparent,
            };
          pnl.Controls.Add(val);
         pnl.Controls.Add(lbl);
    return pnl;
    }

        private static Panel DLinkField(string label, string value, int width)
{
            var pnl = DField(label, value, width);
   var v   = pnl.Controls[0] as Label;
  if (v != null)
            {
        v.ForeColor = ColorHelper.Amber;
  v.Cursor    = Cursors.Hand;
           v.Font      = new Font(v.Font, FontStyle.Underline);
            }
          pnl.Cursor = Cursors.Hand;
            return pnl;
        }
    #endregion

        // ?????????????????????????????????????????????????????????????
        #region Grid events
 private void DgvVagas_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
  if (e.RowIndex < 0 || e.RowIndex >= _vagasFiltradas.Count) return;
      var v = _vagasFiltradas[e.RowIndex];
    if (!string.IsNullOrEmpty(v.Url))
          Process.Start(new ProcessStartInfo(v.Url) { UseShellExecute = true });
        }

        private void DgvVagas_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
    var col = dgvVagas.Columns[e.ColumnIndex].Name;
            if (_sortColumn == col) _sortAsc = !_sortAsc;
            else { _sortColumn = col; _sortAsc = true; }

  foreach (DataGridViewColumn c in dgvVagas.Columns)
             c.HeaderText = c.HeaderText.TrimEnd(' ', '?', '?');
 dgvVagas.Columns[col].HeaderText += _sortAsc ? "  ?" : "  ?";

          Comparison<Vaga>? cmp = col switch
 {
     "Num"         => (a, b) => a.Id.CompareTo(b.Id),
     "Site" => (a, b) => string.Compare(a.Site, b.Site, StringComparison.OrdinalIgnoreCase),
     "Titulo"      => (a, b) => string.Compare(a.Titulo, b.Titulo, StringComparison.OrdinalIgnoreCase),
      "Responsavel" => (a, b) => string.Compare(a.Responsavel, b.Responsavel, StringComparison.OrdinalIgnoreCase),
      "Status"      => (a, b) => a.Status.CompareTo(b.Status),
     "Data"  => (a, b) => string.Compare(a.Data, b.Data, StringComparison.OrdinalIgnoreCase),
           _ => null,
         };
 if (cmp != null)
         {
          if (_sortAsc) _vagasFiltradas.Sort(cmp);
    else          _vagasFiltradas.Sort((a, b) => -cmp(a, b));
    PopularGrid();
            }
        }
        #endregion

        // ?????????????????????????????????????????????????????????????
     #region Keyboard
    private void FormMain_KeyDown(object sender, KeyEventArgs e)
 {
    if (e.KeyCode == Keys.F5) { ExecutarPesquisa(); e.Handled = true; return; }
if (e.KeyCode == Keys.Delete && dgvVagas.SelectedRows.Count > 0)
{
          SetarStatusVaga(_vagasFiltradas[dgvVagas.SelectedRows[0].Index].Id, StatusVaga.Excluida);
        return;
      }
        if (e.KeyCode == Keys.Return && dgvVagas.SelectedRows.Count > 0)
          {
     var v = _vagasFiltradas[dgvVagas.SelectedRows[0].Index];
    if (!string.IsNullOrEmpty(v.Url))
   Process.Start(new ProcessStartInfo(v.Url) { UseShellExecute = true });
         e.Handled = true; return;
 }
  if (e.KeyCode == Keys.Escape)
         {
         txtFiltro.Text      = "buscar vaga, site ou responsável...";
        txtFiltro.ForeColor = ColorHelper.TextSecond;
      AplicarFiltroTexto(); return;
  }
    if (e.Control && e.KeyCode == Keys.E) ExportarCSV();
        }
        #endregion

        // ?????????????????????????????????????????????????????????????
    #region Utils
 private static string Trunc(string s, int max)
        {
  if (string.IsNullOrEmpty(s) || s.Length <= max) return s;
      return s[..(max - 1)] + "…";
        }
        #endregion
    }

    // ?????????????????????????????????????????????????????????????????
    #region Custom Renderers (mantidos por FormProgress / StatusStrip)
    internal class FlatToolStripRenderer : ToolStripProfessionalRenderer
    {
        public FlatToolStripRenderer(Color bg) : base(new FlatColorTable(bg)) { }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
   => e.Graphics.Clear(((FlatColorTable)ColorTable).Bg);

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
      {
    var bg = ((FlatColorTable)ColorTable).Bg;
        if (e.Item.Selected || e.Item.Pressed)
       {
     using var b = new SolidBrush(UiHelper.Darken(bg, 18));
         e.Graphics.FillRectangle(b, new Rectangle(Point.Empty, e.Item.Size));
    }
            else if (e.Item.BackColor != bg && e.Item.BackColor != SystemColors.Control)
    {
     using var b = new SolidBrush(e.Item.BackColor);
       e.Graphics.FillRectangle(b, new Rectangle(Point.Empty, e.Item.Size));
        }
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
    using var p = new Pen(ColorHelper.Divider, 1);
            int x = e.Item.Width / 2;
            e.Graphics.DrawLine(p, x, 3, x, e.Item.Height - 3);
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = e.Item.ForeColor != SystemColors.ControlText
        ? e.Item.ForeColor : ColorHelper.TextPrimary;
         base.OnRenderItemText(e);
   }

   protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
 {
      if (e.Item.Selected)
  {
          using var b = new SolidBrush(ColorHelper.AmberBg);
     e.Graphics.FillRectangle(b, new Rectangle(Point.Empty, e.Item.Size));
          using var p = new Pen(ColorHelper.AmberLo, 1);
  e.Graphics.DrawRectangle(p, 0, 0, e.Item.Width - 1, e.Item.Height - 1);
     }
        else
      {
         using var b = new SolidBrush(((FlatColorTable)ColorTable).Bg);
              e.Graphics.FillRectangle(b, new Rectangle(Point.Empty, e.Item.Size));
     }
        }
    }

    internal sealed class FlatColorTable : ProfessionalColorTable
    {
        public Color Bg { get; }
        public FlatColorTable(Color bg) { Bg = bg; UseSystemColors = false; }
        public override Color ToolStripGradientBegin     => Bg;
   public override Color ToolStripGradientMiddle           => Bg;
        public override Color ToolStripGradientEnd   => Bg;
        public override Color MenuStripGradientBegin       => Bg;
        public override Color MenuStripGradientEnd         => Bg;
        public override Color ToolStripBorder             => Bg;
        public override Color ToolStripContentPanelGradientBegin=> Bg;
        public override Color ToolStripContentPanelGradientEnd  => Bg;
     public override Color MenuItemPressedGradientBegin      => ColorHelper.AmberBg;
 public override Color MenuItemPressedGradientEnd        => ColorHelper.AmberBg;
        public override Color MenuItemSelectedGradientBegin     => ColorHelper.AmberBg;
        public override Color MenuItemSelectedGradientEnd       => ColorHelper.AmberBg;
        public override Color MenuItemSelected          => ColorHelper.AmberBg;
      public override Color MenuItemBorder  => ColorHelper.AmberLo;
   public override Color ImageMarginGradientBegin          => Bg;
 public override Color ImageMarginGradientMiddle         => Bg;
   public override Color ImageMarginGradientEnd => Bg;
   public override Color SeparatorDark        => ColorHelper.Divider;
        public override Color SeparatorLight     => ColorHelper.Divider;
    }

    internal sealed class FlatMenuRenderer : FlatToolStripRenderer
    {
        public FlatMenuRenderer(Color bg) : base(bg) { }
    }
    #endregion
}