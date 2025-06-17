using BarsDataIndicators;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Reflection;
using TradingPlatform.BusinessLayer;
using TradingPlatform.BusinessLayer.Utils;
using System.Drawing.Drawing2D;

namespace Custom.Indicators
{
    public class WeeklyOHLC : Indicator
    {
        public enum LabelPosition
        {
            Above,
            Below
        }

        public enum Format
        {
            Price,
            PriceAndText,
            Text
        }


        private int weekNum = 1;
        private int loopbackWeeks = 0;
        private string telegramBotToken = "";
        private string telegramChatId = "";
        private bool showLabel = true;
        private Font labelFont;
        private NativeAlignment alignment = NativeAlignment.Left;
        private LabelPosition labelPosition = LabelPosition.Above;
        private Format format = Format.PriceAndText;
        public LineOptions highLineOptions;
        public bool highShowLabel = true;
        public string highPrefix = "High Week Value: ";
        public LineOptions lowLineOptions;
        public bool lowShowLabel = true;
        public string lowPrefix = "Low Week Value: ";



        public override IList<SettingItem> Settings
        {
            get
            {
                var settings = base.Settings;

                SettingItemSeparatorGroup inputGroup = new SettingItemSeparatorGroup("Input Parameter", 1);

                settings.Add(new SettingItemInteger("weekNumber", this.weekNum)
                {
                    Text = "Number of weeks to calculate",
                    SeparatorGroup = inputGroup,
                    SortIndex = 1,
                    Minimum = 0
                });
                settings.Add(new SettingItemInteger("loopback", this.loopbackWeeks)
                {
                    Text = "Use previous data (offset in days)",
                    SeparatorGroup = inputGroup,
                    SortIndex = 2,
                    Minimum = 0
                });
                settings.Add(new SettingItemString("teleBotToken", this.telegramBotToken)
                {
                    Text = "Telegram Bot Token",
                    SeparatorGroup = inputGroup,
                    SortIndex = 3
                });
                settings.Add(new SettingItemString("teleChatId", this.telegramChatId)
                {
                    Text = "Telegram Chat ID",
                    SeparatorGroup = inputGroup,
                    SortIndex = 4
                });
                settings.Add(new SettingItemBoolean("showLabel", this.showLabel)
                {
                    Text = "Show Label",
                    SeparatorGroup = inputGroup,
                    SortIndex = 5
                });

                if(this.showLabel){
                    settings.Add(new SettingItemFont("font", this.labelFont)
                    {
                        Text = "Label Font",
                        SeparatorGroup = inputGroup,
                        SortIndex = 6
                    });
                    settings.Add(new SettingItemAlignment("alignment", this.alignment)
                    {
                        Text = "Label Alignment",
                        SeparatorGroup = inputGroup,
                        SortIndex = 7
                    });
                    settings.Add(new SettingItemSelectorLocalized("labelPosition", this.labelPosition, new List<SelectItem> { new SelectItem("Above the line", LabelPosition.Above), new SelectItem("Below the line", LabelPosition.Below) })
                    {
                        Text = "Label Position",
                        SeparatorGroup = inputGroup,
                        SortIndex = 8
                    });
                    settings.Add(new SettingItemSelectorLocalized("format", this.format, new List<SelectItem> { new SelectItem("Price", Format.Price), new SelectItem("Price and Text", Format.PriceAndText), new SelectItem("Text", Format.Text) })
                    {
                        Text = "Format",
                        SeparatorGroup = inputGroup,
                        SortIndex = 9
                    });
                }

                SettingItemSeparatorGroup highLineGroup = new SettingItemSeparatorGroup("High Line Style", 10);
                settings.Add(new SettingItemLineOptions("highLineOptions", this.highLineOptions)
                {
                    Text = "Line Style",
                    SeparatorGroup = highLineGroup,
                    SortIndex = 11
                });
                settings.Add(new SettingItemBoolean("highShowLabel", this.highShowLabel)
                {
                    Text = "Show Label",
                    SeparatorGroup = highLineGroup,
                    SortIndex = 12
                });
                if (this.showLabel)
                {
                    settings.Add(new SettingItemString("highPrefix", this.highPrefix)
                    {
                        Text = "Custom Text",
                        SeparatorGroup = highLineGroup,
                        SortIndex = 13
                    });
                }

                SettingItemSeparatorGroup lowLineGroup = new SettingItemSeparatorGroup("Low Line Style", 14);
                settings.Add(new SettingItemLineOptions("lowLineOptions", this.lowLineOptions)
                {
                    Text = "Line Style",
                    SeparatorGroup = lowLineGroup,
                    SortIndex = 15
                });
                settings.Add(new SettingItemBoolean("lowShowLabel", this.lowShowLabel)
                {
                    Text = "Show Label",
                    SeparatorGroup = lowLineGroup,
                    SortIndex = 16
                });
                if (this.showLabel)
                {
                    settings.Add(new SettingItemString("lowPrefix", this.lowPrefix)
                    {
                        Text = "Custom Text",
                        SeparatorGroup = lowLineGroup,
                        SortIndex = 17
                    });
                }

                return settings;
            }
            set
            {
                base.Settings = value;

                if (value.TryGetValue("weekNumber", out int weekNum))
                    this.weekNum = weekNum;

                if (value.TryGetValue("loopback", out int loopback))
                    this.loopbackWeeks = loopback;

                if (value.TryGetValue("teleBotToken", out string telegramBotToken))
                    this.telegramBotToken = telegramBotToken;

                if (value.TryGetValue("teleChatId", out string telegramChatId))
                    this.telegramChatId = telegramChatId;

                if (value.TryGetValue("showLabel", out bool showLabel))
                    this.showLabel = showLabel;

                if (value.TryGetValue("font", out Font labelFont))
                    this.labelFont = labelFont;

                if (value.TryGetValue("alignment", out NativeAlignment alignment))
                    this.alignment = alignment;

                if (value.TryGetValue("labelPosition", out LabelPosition labelPosition))
                    this.labelPosition = labelPosition;

                if (value.TryGetValue("format", out Format format))
                    this.format = format;

                if (value.TryGetValue("highLineOptions", out LineOptions highLineOptions))
                    this.highLineOptions = highLineOptions;

                if (value.TryGetValue("highShowLabel", out bool highShowLabel))
                    this.highShowLabel = highShowLabel;

                if (value.TryGetValue("highPrefix", out string highPrefix))
                    this.highPrefix = highPrefix;

                if (value.TryGetValue("lowLineOptions", out LineOptions lowLineOptions))
                    this.lowLineOptions = lowLineOptions;

                if (value.TryGetValue("lowShowLabel", out bool lowShowLabel))
                    this.lowShowLabel = lowShowLabel;

                if (value.TryGetValue("lowPrefix", out string lowPrefix))
                    this.lowPrefix = lowPrefix;

                this.OnSettingsUpdated();
            }
        }



        private Dictionary<DateTime, (double high, double low)> weeklyLevels;

        private HashSet<string> sentAlerts;



        public WeeklyOHLC()
        {
            this.Name = "Weekly OHLC (Custom)";
            this.SeparateWindow = false;
        }

        protected override void OnInit()
        {
            this.weeklyLevels = new Dictionary<DateTime, (double high, double low)>();
            this.sentAlerts = new HashSet<string>();

            this.labelFont = new Font("Verdana", 12);
            this.highLineOptions = new LineOptions();
            this.highLineOptions.Color = Color.Yellow;
            this.highLineOptions.Width = 1;
            this.highLineOptions.Enabled = true;
            this.highPrefix = "High Week Value: ";
            //this.highLineOptions.WithCheckBox = false;
            this.lowLineOptions = new LineOptions();
            this.lowLineOptions.Color = Color.Purple;
            this.lowLineOptions.Width = 1;
            this.lowLineOptions.Enabled = true;
            this.lowPrefix = "Low Week Value: ";
            //this.lowLineOptions.WithCheckBox = false;
        }

        protected override void OnUpdate(UpdateArgs args)
        {
            //int barsCount = this.HistoricalData.Count;
            //this.SetValue(barsCount);

            //if (barsCount < 1)
            //    return;

            //CalculateWeeklyLevels();

            //int index = barsCount - 1;
            //double currentClose = this.HistoricalData.Close(index);
            //DateTime time = this.HistoricalData.Time(index);
            //DateTime weekStart = GetWeekStart(time.AddDays(-7)); // Previous week

            //// If the current bar belongs to a current week, draw past weeks' highs/lows
            //foreach (var kvp in weeklyLevels)
            //{
            //    DateTime wk = kvp.Key;
            //    var levels = kvp.Value;
            //    if (wk >= weekStart.AddDays(-7 * loopbackWeeks))
            //    {
            //        string alertId = $"{wk}_{index}";

            //        if (!sentAlerts.Contains(alertId))
            //        {
            //            if (currentClose > levels.high)
            //            {
            //                SendTelegramAlert($"📈 Price closed ABOVE previous weekly HIGH ({levels.high}) on {time}", alertId);
            //                sentAlerts.Add(alertId);
            //            }
            //            else if (currentClose < levels.low)
            //            {
            //                SendTelegramAlert($"📉 Price closed BELOW previous weekly LOW ({levels.low}) on {time}", alertId);
            //                sentAlerts.Add(alertId);
            //            }
            //        }
            //    }
            //}
        }

        public override void OnPaintChart(PaintChartEventArgs args)
        {
            base.OnPaintChart(args);

            if (this.CurrentChart == null)
                return;

            Graphics graphics = args.Graphics;
            RectangleF prevClipRectangle = graphics.ClipBounds;
            graphics.SetClip(args.Rectangle);
            try
            {
                if (this.HistoricalData.Aggregation is HistoryAggregationTick)
                {
                    graphics.DrawString("Indicator does not work on tick aggregation", new Font("Arial", 20), Brushes.Red, 20, 50);
                    return;
                }

                var mainWindow = this.CurrentChart.MainWindow;

                // screen available time range
                DateTime leftBorderTime = mainWindow.CoordinatesConverter.GetTime(0);
                DateTime rightBorderTime = mainWindow.CoordinatesConverter.GetTime(mainWindow.ClientRectangle.Width);

                // corresponding data index
                int leftIndex = (int)mainWindow.CoordinatesConverter.GetBarIndex(leftBorderTime);
                int rightIndex = (int)mainWindow.CoordinatesConverter.GetBarIndex(rightBorderTime);

                // validation
                if (leftIndex < 0)
                    leftIndex = 0;
                if (rightIndex >= this.HistoricalData.Count)
                    rightIndex = this.HistoricalData.Count - 1;

                // reverse from history order to screen order
                leftIndex = this.HistoricalData.Count - leftIndex - 1;
                rightIndex = this.HistoricalData.Count - rightIndex - 1;

                DateTime endTime = leftBorderTime;
                foreach (var kvp in weeklyLevels)
                {
                    DateTime wk = kvp.Key;
                    if(wk <= endTime && (endTime - wk).Days / 7 < this.weekNum && weeklyLevels.ContainsKey(wk.AddDays(-this.loopbackWeeks)))
                    {
                        var levels = kvp.Value;

                        PointF highLeftPoint = new PointF();
                        PointF highRightPoint = new PointF();
                        PointF lowLeftPoint = new PointF();
                        PointF lowRightPoint = new PointF();

                        highLeftPoint.X = (float)mainWindow.CoordinatesConverter.GetChartX(wk);
                        lowLeftPoint.X = (float)mainWindow.CoordinatesConverter.GetChartX(wk);
                        highRightPoint.X = (float)mainWindow.CoordinatesConverter.GetChartX(endTime);
                        lowRightPoint.X = (float)mainWindow.CoordinatesConverter.GetChartX(endTime);
                        highLeftPoint.Y = (float)mainWindow.CoordinatesConverter.GetChartY(levels.high);
                        highRightPoint.Y = (float)mainWindow.CoordinatesConverter.GetChartY(levels.high);
                        lowLeftPoint.Y = (float)mainWindow.CoordinatesConverter.GetChartY(levels.low);
                        lowRightPoint.Y = (float)mainWindow.CoordinatesConverter.GetChartY(levels.low);

                        if (this.showLabel)
                        {
                            PointF labelPoint = new PointF();
                            string label = "";

                            if (this.highShowLabel)
                            {
                                switch (this.format)
                                {
                                    case Format.Text:
                                        label = this.highPrefix;
                                        break;
                                    case Format.PriceAndText:
                                        label = this.highPrefix + levels.high.ToString();
                                        break;
                                    case Format.Price:
                                        label = levels.high.ToString();
                                    break;
                                }
                                SizeF labelSize = graphics.MeasureString(label, this.labelFont);

                                switch (this.alignment)
                                {
                                    case NativeAlignment.Left:
                                        labelPoint.X = highLeftPoint.X;
                                        break;
                                    case NativeAlignment.Right:
                                        labelPoint.X = highRightPoint.X - labelSize.Width;
                                        break;
                                    case NativeAlignment.Center:
                                        labelPoint.X = (highLeftPoint.X + highRightPoint.X - labelSize.Width) / 2;
                                        break;
                                }
                                labelPoint.Y = highLeftPoint.Y - (this.labelPosition == LabelPosition.Above ? labelSize.Height : 0);

                                graphics.DrawString(label, this.labelFont, new SolidBrush(this.highLineOptions.Color), labelPoint);
                            }

                            if (this.lowShowLabel)
                            {
                                switch(this.format)
                                {
                                    case Format.Text:
                                        label = this.lowPrefix;
                                        break;
                                    case Format.PriceAndText:
                                        label = this.lowPrefix + levels.low.ToString();
                                        break;
                                    case Format.Price:
                                        label = levels.low.ToString();
                                        break;
                                }
                                SizeF labelSize = graphics.MeasureString(label, this.labelFont);

                                switch (this.alignment)
                                {
                                    case NativeAlignment.Left:
                                        labelPoint.X = lowLeftPoint.X;
                                        break;
                                    case NativeAlignment.Right:
                                        labelPoint.X = lowRightPoint.X - labelSize.Width;
                                        break;
                                    case NativeAlignment.Center:
                                        labelPoint.X = (lowLeftPoint.X + lowRightPoint.X - labelSize.Width) / 2;
                                        break;
                                }
                                labelPoint.Y = lowLeftPoint.Y - (this.labelPosition == LabelPosition.Above ? labelSize.Height : 0);

                                graphics.DrawString(label, this.labelFont, new SolidBrush(this.lowLineOptions.Color), labelPoint);
                            }
                        }

                        if (this.highLineOptions.Enabled)
                        {
                            Pen pen = new Pen(this.highLineOptions.Color, this.highLineOptions.Width);
                            pen.DashStyle = ConvertLineStyle(this.highLineOptions.LineStyle);
                            graphics.DrawLine(pen, highLeftPoint, highRightPoint);
                        }

                        if (this.lowLineOptions.Enabled)
                        {
                            Pen pen = new Pen(this.lowLineOptions.Color, this.lowLineOptions.Width);
                            pen.DashStyle = ConvertLineStyle(this.lowLineOptions.LineStyle);
                            graphics.DrawLine(pen, lowLeftPoint, lowRightPoint);
                        }

                        endTime = endTime.AddDays(-7);
                    }
                    else
                    {
                        break;
                    }
                }

                // draw
                //for (int i = leftIndex; i >= rightIndex; i--)
                //{
                //    var currBar = (HistoryItemBar)this.HistoricalData[i];
                //    SizeF labelSize = graphics.MeasureString("abc".ToString(), this.labelFont);
                //    PointF labelPoint = new PointF();
                //    labelPoint.Y = (float)mainWindow.CoordinatesConverter.GetChartY(currBar.High) - labelSize.Height;
                //    switch (labelPosition)
                //    {
                //        case LabelPosition.Left:
                //            labelPoint.X = (float)mainWindow.CoordinatesConverter.GetChartX(this.HistoricalData[i].TimeLeft);
                //            break;
                //        case LabelPosition.Right:
                //            labelPoint.X = (float)mainWindow.CoordinatesConverter.GetChartX(this.HistoricalData[i].TimeLeft) + this.CurrentChart.BarsWidth - labelSize.Width;
                //            break;
                //        case LabelPosition.Center:
                //            labelPoint.X = (float)mainWindow.CoordinatesConverter.GetChartX(this.HistoricalData[i].TimeLeft) + (this.CurrentChart.BarsWidth - labelSize.Width)/2;
                //            break;
                //        default:
                //            break;
                //    }

                //    graphics.DrawString("abc", this.labelFont, new SolidBrush(this.highLineOptions.Color), labelPoint);

                //}

            }
            finally
            {
                graphics.SetClip(prevClipRectangle);
            }

        }

        public DashStyle ConvertLineStyle(LineStyle lineStyle)
        {
            switch (lineStyle)
            {
                case LineStyle.Solid:
                    return DashStyle.Solid;

                case LineStyle.Dash:
                    return DashStyle.Dash;

                case LineStyle.Dot:
                    return DashStyle.Dot;

                case LineStyle.DashDot:
                    return DashStyle.DashDot;

                default:
                    return DashStyle.Solid;
            }
        }

        private void CalculateWeeklyLevels()
        {
            this.weeklyLevels.Clear();

            DateTime startTime = this.HistoricalData.Time(0);
            DateTime startWeek = GetWeekStart(startTime);
            DateTime endWeek = startTime.Date.AddDays(7);


            int count = this.HistoricalData.Count;
            for (int i = 0; i < count; i++)
            {
                DateTime time = this.HistoricalData.Time(i);

                double high = this.HistoricalData.High(i);
                double low = this.HistoricalData.Low(i);

                if (time >= endWeek)
                {
                    weeklyLevels[startWeek] = (high, low);
                    startWeek = endWeek;
                    endWeek = startTime.AddDays(7);
                }
                else
                {
                    var old = weeklyLevels[startWeek];
                    weeklyLevels[startWeek] = (
                        Math.Max(old.high, high),
                        Math.Min(old.low, low)
                    );
                }
            }
        }

        private DateTime GetWeekStart(DateTime dt)
        {
            // Return start of ISO week (Monday)
            int diff = (7 + (dt.DayOfWeek - DayOfWeek.Monday)) % 7;
            return dt.Date.AddDays(-diff);
        }

        private void SendTelegramAlert(string message, string alertId)
        {
            if (string.IsNullOrWhiteSpace(telegramBotToken) || string.IsNullOrWhiteSpace(telegramChatId))
                return;

            string url = $"https://api.telegram.org/bot{telegramBotToken}/sendMessage";
            string postData = $"chat_id={telegramChatId}&text={Uri.EscapeDataString(message)}";

            try
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    var content = new System.Net.Http.FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("chat_id", telegramChatId),
                        new KeyValuePair<string, string>("text", message)
                    });

                    var response = client.PostAsync(url, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        sentAlerts.Add(alertId); // Mark the alert as sent
                    }
                }
            }
            catch (Exception ex)
            {
                //this.("TGError", "Telegram error: " + ex.Message, index: this.HistoricalData.Count - 1, y: 0, color: Color.Red);
            }
        }
    }
}
