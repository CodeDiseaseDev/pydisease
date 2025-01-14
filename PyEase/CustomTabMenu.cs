﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PyEase
{
    class CustomTabMenu : Control
    {
        private List<BTabPage> TabPages = new List<BTabPage>();
        private TabControl _tabs;
        public TabControl tabs
        {
            get => _tabs;
            set
            {
                if (value == null)
                    return;
                _tabs = value;
                _tabs.SelectedIndexChanged += _tabs_SelectedIndexChanged;
            }
        }

        private void _tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        public void AddTab(BTabPage page)
        {
            DoubleBuffered = true;
            TabPages.Add(page);
            page.TextChanged += Page_TextChanged;
            Invalidate();
        }

        private void Page_TextChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        public void RemoveTab(BTabPage page)
        {
            page.TextChanged -= Page_TextChanged;
            TabPages.Remove(page);
            Invalidate();
        }

        public void RemoveTabAt(int at)
        {
            TabPages[at].TextChanged -= Page_TextChanged;
            TabPages.RemoveAt(at);
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            var match = TabPages.Where(a => a.TabRectangle.IntersectsWith(new Rectangle(e.X, e.Y, 1, 1)));

            if (match.Any())
                tabs.SelectedIndex = match.ToArray()[0].Index;

            base.OnMouseDown(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            Invalidate();
        }

        int DrawTab(int x, BTabPage page, Graphics graphics)
        {
            var size = graphics.MeasureString(page.Text, Font);
            int width = (int)size.Width + 40;
            page.TabRectangle = new Rectangle(x, 0, width, Height);
            Color a = Color.FromArgb(30, 34, 42);
            if (tabs.SelectedIndex == page.Index)
                a = Color.FromArgb(40, 44, 52);
            graphics.FillPath(new SolidBrush(a), Drawing.RoundedRect(page.TabRectangle, 6, true, false));
            graphics.DrawPath(new Pen(a), Drawing.RoundedRect(page.TabRectangle, 6, true, false));
            graphics.DrawString(page.Text, Font, Brushes.White, x + (width / 2) - (size.Width / 2), (Height / 2) - (size.Height / 2));
            return width;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // sorry about this crazy messy code... lol
            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            int x = 4;
            foreach (BTabPage page in TabPages)
                x += DrawTab(x, page, graphics) + 2;
        }
    }
}
