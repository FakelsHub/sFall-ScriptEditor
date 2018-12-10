using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

using ScriptEditor.CodeTranslation;
using ScriptEditor.TextEditorUI.ToolTips;

namespace ScriptEditor.TextEditorUI.CompleteList
{
    public class AutoComplete
    {
        private const int countItems = 12;
        private const int height = 17;
        
        private int x, y, itemWidth, itemHeight;
        private bool hidden;

        private LinearGradientBrush SelectItemBrush = new LinearGradientBrush(
                                        new PointF(0, 0), new PointF(0, height),
                                        Color.White, Color.FromArgb(240, 190, 100));

        public bool ShiftCaret { get; set; } // используется для возврата курсора каретки на ключевое слово.
        
        bool colored;
        public bool Colored {
            private get { return colored; }
            set { colored = value;
                  AutoComleteList.Font = colored ? font.BoldFont : font.RegularFont; }
        }

        public KeyValuePair<int, string> WordPosition { get; set; }
        private TextAreaControl TAC { get; set; }

        private ListBox AutoComleteList;
        private ToolTip tipAC;
        private Panel panel;
        private ImageList imageList;
        private FontContainer font;

        public AutoComplete(Panel panel, bool colored)
        {
            this.panel = panel;
            this.colored = colored;

            FontFamily family = Settings.Fonts.Families.FirstOrDefault(f => f.Name == "InputMono");
            this.font = new FontContainer(new Font((family ?? FontFamily.GenericMonospace), 10, FontStyle.Regular, GraphicsUnit.Point));

            AutoComleteList = new ListBox();
            AutoComleteList.Cursor = Cursors.Help;
            AutoComleteList.ItemHeight = height;
            AutoComleteList.MaximumSize = new Size(350, (countItems * height) + 4);
            AutoComleteList.MinimumSize = new Size(120, height + 3);
            AutoComleteList.Font = colored ? font.BoldFont : font.RegularFont;
            AutoComleteList.Visible = false;
            AutoComleteList.DrawMode = DrawMode.OwnerDrawFixed;
            AutoComleteList.IntegralHeight = false;

            AutoComleteList.DrawItem += ACL_Draw;
            AutoComleteList.VisibleChanged += ACL_VisibleChanged;
            AutoComleteList.SelectedIndexChanged += ACL_SelectedIndexChanged;
            AutoComleteList.MouseMove += ACL_MouseMove;
            AutoComleteList.MouseEnter += ACL_MouseEnter;
            AutoComleteList.PreviewKeyDown += ACL_PreviewKeyDown;
            AutoComleteList.MouseClick += ACL_MouseClick;
            AutoComleteList.KeyDown += ACL_KeyDown;

            tipAC = new ToolTip();
            tipAC.OwnerDraw = true;
            tipAC.Draw += tipDraw;

            imageList = new ImageList();
            imageList.TransparentColor = Color.FromArgb(255, 0, 255);
            imageList.Images.Add(NameType.Macro.ToString(), Properties.Resources.macros);
            imageList.Images.Add(NameType.Proc.ToString(), Properties.Resources.procedure);
            imageList.Images.Add(NameType.GVar.ToString(), Properties.Resources.variable);
            imageList.Images.Add(NameType.None.ToString(), Properties.Resources.opcode);

            panel.Controls.Add(AutoComleteList);
            AutoComleteList.BringToFront();

            UpdateColor();
            Program.SetDoubleBuffered(AutoComleteList);
        }

        public void UpdateColor()
        {
            if (ColorTheme.IsDarkTheme) {
                AutoComleteList.BackColor = ColorTheme.TipGradient.BackgroundColor;
                AutoComleteList.BorderStyle = BorderStyle.FixedSingle;
            } else {
                AutoComleteList.BackColor = Color.GhostWhite;
                AutoComleteList.BorderStyle = BorderStyle.Fixed3D;
            }
        }

        public void Hide()
        {
            if (AutoComleteList.Visible) {
                TAC.TextEditorProperties.MouseWheelTextZoom = false;
                hidden = true;
                AutoComleteList.Hide();
            }
        }

        public void UnHide()
        {
            if (hidden) {
                hidden = false;
                AutoComleteList.Show();
            }
            if (TAC != null)
                TAC.TextEditorProperties.MouseWheelTextZoom = true;
        }

        public void Close()
        {
            hidden = false;
            AutoComleteList.Hide();
            AutoComleteList.Items.Clear();
        }

        public void Show()
        {
            hidden = false;
            AutoComleteList.Show();
        }

        public bool IsVisible
        {
            get { return (AutoComleteList.Visible | hidden); }
        }

        private void ShowItemTip()
        {
            AutoCompleteItem acItem = (AutoCompleteItem)AutoComleteList.SelectedItem;
            if (acItem != null /*&& AutoComleteList.Focused*/) {
                tipAC.Show(acItem.Hint, panel, AutoComleteList.Left
                           + AutoComleteList.Width + 5, AutoComleteList.Top, 50000);
            }
        }

        private void PasteSelectedItem()
        {
            ShiftCaret = false;

            AutoCompleteItem item = (AutoCompleteItem)AutoComleteList.SelectedItem;

            int startOffs = TextUtilities.FindWordStart(TAC.Document, WordPosition.Key); //WordPosition.Key - WordPosition.Value.Length;
            TAC.Document.Replace(startOffs, WordPosition.Value.Length, item.Name);
            TAC.Caret.Position = TAC.Document.OffsetToPosition(startOffs + item.NameLength);

            AutoComleteList.Hide();
            TAC.TextArea.Focus();
            TAC.TextArea.Select();
        }

        public void GenerateList(string keyChar, TabInfo cTab, int caretOffset, object showTip, bool back = false)
        {
            if (!cTab.shouldParse)
                return;
            
            TAC = cTab.textEditor.ActiveTextAreaControl;
            string word = TextUtilities.GetWordAt(TAC.Document, caretOffset) + keyChar;
            if (word == String.Empty && keyChar == String.Empty)
                word = TextUtilities.GetWordAt(TAC.Document, --caretOffset);

            if (back && word != null) {
                if (word.Length > 2)
                    word = word.Remove(word.Length - 1);
                else
                    word = null;
            }
            
            if (word != null && word.Length > 1) {
                var matches = (cTab.parseInfo != null)
                    ? cTab.parseInfo.LookupAutosuggest(word)
                    : ProgramInfo.LookupOpcode(word);
                
                int shift = (back) ? -1 : 1;

                if (matches.Count > 0) {
                    AutoComleteList.BeginUpdate();
                    AutoComleteList.Items.Clear();
                    int maxLen = 0;
                    foreach (string item in matches)
                    {
                        AutoCompleteItem acItem = new AutoCompleteItem(item);
                        AutoComleteList.Items.Add(acItem);
                        if (acItem.NameLength > maxLen)
                            maxLen = acItem.NameLength;
                    }
                    AutoComleteList.EndUpdate();
                    
                    // size
                    AutoComleteList.Height = AutoComleteList.PreferredHeight - 3;
                    AutoComleteList.Width = (maxLen * 10) + shift_x
                                             + ((AutoComleteList.Items.Count > countItems) ? 15 : 0);

                    if (!AutoComleteList.Visible || back) {
                        var caretPos = TAC.Caret.ScreenPosition;
                        var tePos = TAC.FindForm().PointToClient(TAC.Parent.PointToScreen(TAC.Location));
                        tePos.Offset(caretPos);
                        if (back)
                            tePos.Offset(-6, 18);
                        else
                            tePos.Offset(10, 18);

                        if (showTip != null && (bool)showTip)
                            tePos.Offset(0, 22);

                        if (!back || AutoComleteList.Location.X > tePos.X) {
                            AutoComleteList.Location = tePos;
                        }
                        AutoComleteList.Show();
                    }
                    WordPosition = new KeyValuePair<int, string>(caretOffset + shift, word);
                } else if (AutoComleteList.Visible)
                    WordPosition = new KeyValuePair<int, string>(caretOffset + shift, word);
            } else if (AutoComleteList.Visible) {
                        AutoComleteList.Hide();
            }
        }

        public void TA_MouseScroll(TextAreaControl ATAC, MouseEventArgs e)
        {
            if (IsVisible /*&& e.Delta != 0*/) {
                int shiftY = 50;
                int bottom = shiftY + ATAC.Height + ATAC.Location.Y;
                int top = shiftY + ATAC.Parent.Height - ATAC.Height;

                var tePos = ATAC.FindForm().PointToClient(ATAC.Parent.PointToScreen(ATAC.Location));
                var caretPos = ATAC.Caret.ScreenPosition;
                
                tePos.Offset(caretPos);
                if (e.Button == MouseButtons.None) {
                    if (e.Delta < 0)
                        tePos.Offset(0, -32);
                    else
                        tePos.Offset(0, 70);
                } else
                    tePos.Offset(0, e.Delta + 18);

                if (tePos.Y > bottom || tePos.Y < ATAC.Location.Y + shiftY) {
                    Close();
                    return;
                } else
                    tipAC.Hide(panel);

                tePos.X = AutoComleteList.Location.X;
                AutoComleteList.Location = tePos;
            }
        }

        public void TA_PreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Tab || e.KeyCode == Keys.Down
                || (e.KeyCode == Keys.Up && AutoComleteList.SelectedIndex != -1)) {
                ShiftCaret = true;
                if (AutoComleteList.SelectedIndex < 0)
                    AutoComleteList.SelectedIndex = 0;
                AutoComleteList.Focus();
                ShowItemTip();
                if (e.KeyCode == Keys.Down)
                    TAC.Caret.Line -= 1;
                else if (e.KeyCode == Keys.Up)
                    TAC.Caret.Line += 1;
            } else if (e.KeyCode == Keys.Enter && AutoComleteList.SelectedIndex != -1) {
                PasteSelectedItem();
                e.IsInputKey = true;
            } else if (e.KeyCode == Keys.Enter || (e.KeyCode == Keys.Up && AutoComleteList.SelectedIndex == -1)
                      || e.KeyCode == Keys.Escape || e.KeyCode == Keys.Space) {
                    ACListClose();
            } else if (!e.Shift && !e.Control && !e.Alt) {
                    int caret = TAC.Caret.Offset;
                    if (e.KeyCode == Keys.Left)
                        caret--;
                    //else if (e.KeyCode == Keys.Right)
                    //    caret++;
                    if (!TextUtilities.IsLetterDigitOrUnderscore(TAC.Document.GetCharAt(caret)))
                        Close();
            }
        }

        private void ACListClose()
        {
            Close();
            ShiftCaret = false;
            TAC.Caret.Position = TAC.Document.OffsetToPosition(WordPosition.Key);
            TAC.TextArea.Focus();
        }

        private void ACL_MouseClick(object sender, MouseEventArgs e)
        {
            PasteSelectedItem();
        }

        private void ACL_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if ((e.KeyCode == Keys.Tab || e.KeyCode == Keys.Enter) && AutoComleteList.SelectedIndex != -1)
                PasteSelectedItem();
            else if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Space) {
                ACListClose();
            } else if (e.KeyCode == Keys.Left ) {
                TAC.TextArea.Focus();
                TAC.Caret.Column -= 1;
            } else if (e.KeyCode == Keys.Right) {
                TAC.TextArea.Focus();
                TAC.Caret.Column += 1;
            }
        }

        private void ACL_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right) {
                e.Handled = true;
                tipAC.Hide(panel);
            }
        }

        private void ACL_MouseEnter(object sender, EventArgs e)
        {
            if (AutoComleteList.SelectedIndex < 0)
                return;
            AutoComleteList.Focus();
        }

        private void ACL_MouseMove(object sender, MouseEventArgs e)
        {
            int item = 0;
            if (e.Y != 0)
                item = e.Y / AutoComleteList.ItemHeight;
            int selIndex = AutoComleteList.TopIndex + item;
            if (selIndex < AutoComleteList.Items.Count)
                AutoComleteList.SelectedIndex = selIndex;
        }

        private void ACL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowItemTip();
        }

        private void ACL_VisibleChanged(object sender, EventArgs e)
        {
            tipAC.Hide(panel);
        }

        void tipDraw(object sender, DrawToolTipEventArgs e)
        {
            TipPainter.DrawInfo(e);
        }

        // Specify custom text formatting flags
        static StringFormat sf = new StringFormat() { Trimming = StringTrimming.EllipsisCharacter };
        
        const int shift_x = 20;

        private void ACL_Draw(object s, DrawItemEventArgs e)
        {
            AutoCompleteItem acItem = (AutoCompleteItem)AutoComleteList.Items[e.Index];

            x = e.Bounds.X;
            y = e.Bounds.Y;
            itemWidth = e.Bounds.Width;
            itemHeight = e.Bounds.Height;

            Image image = imageList.Images[acItem.GetType.ToString()];

            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            if (!colored) e.Graphics.TextContrast = 0;

            e.DrawBackground();
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected) {
                e.Graphics.FillRectangle(SelectItemBrush, e.Bounds);
                e.Graphics.DrawImage(image, x + 2, y, 16, 16);
                e.Graphics.DrawRectangle(new Pen(Color.Peru, 1), x, y, itemWidth - 1, itemHeight - 1);
                e.Graphics.DrawString(acItem.Name, e.Font, Brushes.Black, new RectangleF(x + shift_x, y, itemWidth - shift_x + 5, itemHeight), sf);
            } else {
                e.Graphics.DrawImage(image, x + 2, y, 16, 16);
                e.Graphics.DrawString(acItem.Name, e.Font, acItem.GetBrush(colored), new RectangleF(x + shift_x, y, itemWidth - shift_x + 5, itemHeight), sf);
            }
        }
    }
}