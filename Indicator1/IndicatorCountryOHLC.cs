// Copyright QUANTOWER LLC. © 2017-2024. All rights reserved.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TradingPlatform.BusinessLayer;
using TradingPlatform.BusinessLayer.Chart;
using TradingPlatform.BusinessLayer.Utils;

namespace CustomIndicators;

public class IndicatorCountryOHLC : Indicator
{
    #region Parameters   

    private HashSet<string> sentAlerts;
    public DateTime LondonRangeStartTime
    {
        get
        {
            if (this.londonRangeStartTime == default)
            {
                var session = this.CreateLondonSession();
                this.londonRangeStartTime = Core.Instance.TimeUtils.DateTimeUtcNow.Date.AddTicks(session.OpenTime.Ticks);
            }
            return this.londonRangeStartTime;
        }
        set => this.londonRangeStartTime = value;
    }
    private DateTime londonRangeStartTime;
    public DateTime LondonRangeEndTime
    {
        get
        {
            if (this.londonRangeEndTime == default)
            {
                var session = this.CreateLondonSession();
                this.londonRangeEndTime = Core.Instance.TimeUtils.DateTimeUtcNow.Date.AddTicks(session.CloseTime.Ticks);

            }
            return this.londonRangeEndTime;
        }
        set => this.londonRangeEndTime = value;
    }
    private DateTime londonRangeEndTime;
    public DateTime NewYorkRangeStartTime
    {
        get
        {
            if (this.newyorkRangeStartTime == default)
            {
                var session = this.CreateNewYorkSession();
                this.newyorkRangeStartTime = Core.Instance.TimeUtils.DateTimeUtcNow.Date.AddTicks(session.OpenTime.Ticks);
            }
            return this.newyorkRangeStartTime;
        }
        set => this.newyorkRangeStartTime = value;
    }
    private DateTime newyorkRangeStartTime;
    public DateTime NewYorkRangeEndTime
    {
        get
        {
            if (this.newyorkRangeEndTime == default)
            {
                var session = this.CreateNewYorkSession();
                this.newyorkRangeEndTime = Core.Instance.TimeUtils.DateTimeUtcNow.Date.AddTicks(session.CloseTime.Ticks);

            }
            return this.newyorkRangeEndTime;
        }
        set => this.newyorkRangeEndTime = value;
    }
    private DateTime newyorkRangeEndTime;
    public DateTime AsiaRangeStartTime
    {
        get
        {
            if (this.asiaRangeStartTime == default)
            {
                var session = this.CreateAsiaSession();
                this.asiaRangeStartTime = Core.Instance.TimeUtils.DateTimeUtcNow.Date.AddTicks(session.OpenTime.Ticks);
            }
            return this.asiaRangeStartTime;
        }
        set => this.asiaRangeStartTime = value;
    }
    private DateTime asiaRangeStartTime;
    public DateTime AsiaRangeEndTime
    {
        get
        {
            if (this.asiaRangeEndTime == default)
            {
                var session = this.CreateAsiaSession();
                this.asiaRangeEndTime = Core.Instance.TimeUtils.DateTimeUtcNow.Date.AddTicks(session.CloseTime.Ticks);

            }
            return this.asiaRangeEndTime;
        }
        set => this.asiaRangeEndTime = value;
    }
    private DateTime asiaRangeEndTime;

    public int DaysCount = 10;
    public int PreviousDataOffset = 0;

    private string telegramBotToken = "";
    private string telegramChatId = "";
    public NativeAlignment LabelAlignment { get; set; }
    public int labelFormat { get; set; }
    public int labelPosition { get; set; }
    public bool ShowLabel { get; private set; }

    public LineOptions LondonHighLineOptions
    {
        get => this.londonhighLineOptions;
        private set
        {
            this.londonhighLineOptions = value;
            this.londonhighLinePen = ProcessPen(this.londonhighLinePen, value);
        }
    }
    private LineOptions londonhighLineOptions;
    private Pen londonhighLinePen;
    public bool ShowLondonHighLineLabel { get; private set; }

    public LineOptions LondonLowLineOptions
    {
        get => this.londonlowLineOptions;
        private set
        {
            this.londonlowLineOptions = value;
            this.londonlowLinePen = ProcessPen(this.londonlowLinePen, value);
        }
    }
    private LineOptions londonlowLineOptions;
    private Pen londonlowLinePen;
    public bool ShowLondonLowLineLabel { get; private set; }

    public LineOptions LondonMiddleLineOptions
    {
        get => this.londonmiddleLineOptions;
        private set
        {
            this.londonmiddleLineOptions = value;
            this.londonmiddleLinePen = ProcessPen(this.londonmiddleLinePen, value);
        }
    }
    private LineOptions londonmiddleLineOptions;
    private Pen londonmiddleLinePen;
    public bool ShowLondonMiddleLineLabel { get; private set; }


    public LineOptions NewYorkHighLineOptions
    {
        get => this.newyorkhighLineOptions;
        private set
        {
            this.newyorkhighLineOptions = value;
            this.newyorkhighLinePen = ProcessPen(this.newyorkhighLinePen, value);
        }
    }
    private LineOptions newyorkhighLineOptions;
    private Pen newyorkhighLinePen;
    public bool ShowNewYorkHighLineLabel { get; private set; }

    public LineOptions NewYorkLowLineOptions
    {
        get => this.newyorklowLineOptions;
        private set
        {
            this.newyorklowLineOptions = value;
            this.newyorklowLinePen = ProcessPen(this.newyorklowLinePen, value);
        }
    }
    private LineOptions newyorklowLineOptions;
    private Pen newyorklowLinePen;
    public bool ShowNewYorkLowLineLabel { get; private set; }

    public LineOptions NewYorkMiddleLineOptions
    {
        get => this.newyorkmiddleLineOptions;
        private set
        {
            this.newyorkmiddleLineOptions = value;
            this.newyorkmiddleLinePen = ProcessPen(this.newyorkmiddleLinePen, value);
        }
    }
    private LineOptions newyorkmiddleLineOptions;
    private Pen newyorkmiddleLinePen;
    public bool ShowNewYorkMiddleLineLabel { get; private set; }


    public LineOptions AsiaHighLineOptions
    {
        get => this.asiahighLineOptions;
        private set
        {
            this.asiahighLineOptions = value;
            this.asiahighLinePen = ProcessPen(this.asiahighLinePen, value);
        }
    }
    private LineOptions asiahighLineOptions;
    private Pen asiahighLinePen;
    public bool ShowAsiaHighLineLabel { get; private set; }

    public LineOptions AsiaLowLineOptions
    {
        get => this.asialowLineOptions;
        private set
        {
            this.asialowLineOptions = value;
            this.asialowLinePen = ProcessPen(this.asialowLinePen, value);
        }
    }
    private LineOptions asialowLineOptions;
    private Pen asialowLinePen;
    public bool ShowAsiaLowLineLabel { get; private set; }

    public LineOptions AsiaMiddleLineOptions
    {
        get => this.asiamiddleLineOptions;
        private set
        {
            this.asiamiddleLineOptions = value;
            this.asiamiddleLinePen = ProcessPen(this.asiamiddleLinePen, value);
        }
    }
    private LineOptions asiamiddleLineOptions;
    private Pen asiamiddleLinePen;
    public bool ShowAsiaMiddleLineLabel { get; private set; }


    public LineOptions OpenLineOptions
    {
        get => this.openLineOptions;
        private set
        {
            this.openLineOptions = value;
            this.openLinePen = ProcessPen(this.openLinePen, value);
        }
    }
    private LineOptions openLineOptions;
    private Pen openLinePen;
    public bool ShowOpenLineLabel { get; private set; }

    public LineOptions CloseLineOptions
    {
        get => this.closeLineOptions;
        private set
        {
            this.closeLineOptions = value;
            this.closeLinePen = ProcessPen(this.closeLinePen, value);
        }
    }
    private LineOptions closeLineOptions;
    private Pen closeLinePen;
    public bool ShowCloseLineLabel { get; private set; }


    public Font CurrentFont { get; private set; }
    //public string OpenCustomText = "O: ", HighCustomText = "H: ", LowCustomText = "L: ", CloseCustomText = "C: ", MiddleCustomText = "M: ";
    public string AsiaHighCustomText = "Asia H: ", AsiaLowCustomText = "Asia L: ", AsiaMiddleCustomText = "Asia M: ";
    public string LondonHighCustomText = "London H: ", LondonLowCustomText = "London L: ", LondonMiddleCustomText = "London M: ";
    public string NewYorkHighCustomText = "NewYork H: ", NewYorkLowCustomText = "NewYork L: ", NewYorkMiddleCustomText = "NewYork M: ";

    private readonly IList<CountryRangeItem> londonRangeCache, newyorkRangeCache, asiaRangeCache;

    private CountryRangeItem londonRange, newyorkRange, asiaRange;
    private ISession londonSession;
    private ISession newyorkSession;
    private ISession asiaSession;
    private ISessionsContainer chartSessionContainer;
    private DateTime londonSessionOpenDateTime;
    private DateTime londonSessionCloseDateTime;
    private DateTime newyorkSessionOpenDateTime;
    private DateTime newyorkSessionCloseDateTime;
    private DateTime asiaSessionOpenDateTime;
    private DateTime asiaSessionCloseDateTime;
    private readonly StringFormat centerNearSF;

    #endregion Parameters

    public IndicatorCountryOHLC()
    {
        this.Name = "Country OHLC";

        this.AllowFitAuto = true;
        this.SeparateWindow = false;
        this.LabelAlignment = NativeAlignment.Right;
        this.ShowLabel = true;
        this.labelFormat = 1;
        this.labelPosition = 1;
        this.londonRangeCache = new List<CountryRangeItem>();
        this.newyorkRangeCache = new List<CountryRangeItem>();
        this.asiaRangeCache = new List<CountryRangeItem>();

        this.OpenLineOptions = new LineOptions()
        {
            Enabled = true,
            WithCheckBox = true,
            Color = Color.Orange,
            LineStyle = LineStyle.Solid,
            Width = 1
        };
        this.ShowOpenLineLabel = true;

        this.AsiaHighLineOptions = new LineOptions()
        {
            Enabled = true,
            WithCheckBox = true,
            Color = Color.Red,
            LineStyle = LineStyle.Solid,
            Width = 1
        };
        this.ShowAsiaHighLineLabel = true;

        this.AsiaLowLineOptions = new LineOptions()
        {
            Enabled = true,
            WithCheckBox = true,
            Color = Color.Orange,
            LineStyle = LineStyle.Solid,
            Width = 1
        };
        this.ShowAsiaLowLineLabel = true;

        this.AsiaMiddleLineOptions = new LineOptions()
        {
            Enabled = true,
            WithCheckBox = true,
            Color = Color.Gray,
            LineStyle = LineStyle.DashDot,
            Width = 1
        };
        this.ShowAsiaMiddleLineLabel = true;

        this.NewYorkHighLineOptions = new LineOptions()
        {
            Enabled = true,
            WithCheckBox = true,
            Color = Color.Green,
            LineStyle = LineStyle.Solid,
            Width = 1
        };
        this.ShowNewYorkHighLineLabel = true;

        this.NewYorkLowLineOptions = new LineOptions()
        {
            Enabled = true,
            WithCheckBox = true,
            Color = Color.LightGreen,
            LineStyle = LineStyle.Solid,
            Width = 1
        };
        this.ShowNewYorkLowLineLabel = true;

        this.NewYorkMiddleLineOptions = new LineOptions()
        {
            Enabled = true,
            WithCheckBox = true,
            Color = Color.Gray,
            LineStyle = LineStyle.DashDot,
            Width = 1
        };
        this.ShowNewYorkMiddleLineLabel = true;

        this.LondonHighLineOptions = new LineOptions()
        {
            Enabled = true,
            WithCheckBox = true,
            Color = Color.Blue,
            LineStyle = LineStyle.Solid,
            Width = 1
        };
        this.ShowLondonHighLineLabel = true;

        this.LondonLowLineOptions = new LineOptions()
        {
            Enabled = true,
            WithCheckBox = true,
            Color = Color.LightBlue,
            LineStyle = LineStyle.Solid,
            Width = 1
        };
        this.ShowLondonLowLineLabel = true;

        this.LondonMiddleLineOptions = new LineOptions()
        {
            Enabled = true,
            WithCheckBox = true,
            Color = Color.Gray,
            LineStyle = LineStyle.DashDot,
            Width = 1
        };
        this.ShowLondonMiddleLineLabel = true;

        this.CloseLineOptions = new LineOptions()
        {
            Enabled = true,
            WithCheckBox = true,
            Color = Color.FromArgb(33, 150, 243),
            LineStyle = LineStyle.Solid,
            Width = 1
        };
        this.ShowCloseLineLabel = true;

        this.CurrentFont = new Font("Verdana", 10, GraphicsUnit.Pixel);
        this.centerNearSF = new StringFormat()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Near
        };
    }

    #region Base overrides

    protected override void OnInit()
    {
        this.sentAlerts = new HashSet<string>();
        this.chartSessionContainer = this.CurrentChart?.CurrentSessionContainer;

        this.londonSession = new Session("London session", this.LondonRangeStartTime.TimeOfDay, this.LondonRangeEndTime.TimeOfDay);
        this.newyorkSession = new Session("NewYork session", this.NewYorkRangeStartTime.TimeOfDay, this.NewYorkRangeEndTime.TimeOfDay);
        this.asiaSession = new Session("Asia session", this.AsiaRangeStartTime.TimeOfDay, this.AsiaRangeEndTime.TimeOfDay);
    }
    protected override void OnUpdate(UpdateArgs args)
    {
        var currentBarTime = this.Time();
        double currentClose = this.HistoricalData.Close(0);


        var inLondonSession = this.londonSession.ContainsDate(currentBarTime);
        // Main range
        if (inLondonSession)
        {
            // create new 'main' range
            if (this.londonRange == null || currentBarTime >= this.londonSessionCloseDateTime)
            {
                this.londonSessionOpenDateTime = currentBarTime.Date.AddTicks(this.londonSession.OpenTime.Ticks);
                this.londonSessionCloseDateTime = currentBarTime.Date.AddTicks(this.londonSession.CloseTime.Ticks);

                if (currentBarTime < this.londonSessionOpenDateTime)
                    this.londonSessionOpenDateTime = this.londonSessionOpenDateTime.AddDays(-1);
                if (currentBarTime > this.londonSessionCloseDateTime)
                    this.londonSessionCloseDateTime = this.londonSessionCloseDateTime.AddDays(1);

                this.londonRangeCache.Insert(0, this.londonRange = new CountryRangeItem(currentBarTime, this.Open()));
            }

            // update High/Low/Close in history
            if (args.Reason == UpdateReason.HistoricalBar)
                this.londonRange.TryUpdate(this.High(), this.Low(), this.Close());
            else
                this.londonRange.TryUpdate(this.Close());

            this.londonRange.EndDateTime = currentBarTime;
        }

        var inNewYorkSession = this.newyorkSession.ContainsDate(currentBarTime);
        // Main range
        if (inNewYorkSession)
        {
            // create new 'main' range
            if (this.newyorkRange == null || currentBarTime >= this.newyorkSessionCloseDateTime)
            {
                this.newyorkSessionOpenDateTime = currentBarTime.Date.AddTicks(this.newyorkSession.OpenTime.Ticks);
                this.newyorkSessionCloseDateTime = currentBarTime.Date.AddTicks(this.newyorkSession.CloseTime.Ticks);

                if (currentBarTime < this.newyorkSessionOpenDateTime)
                    this.newyorkSessionOpenDateTime = this.newyorkSessionOpenDateTime.AddDays(-1);
                if (currentBarTime > this.newyorkSessionCloseDateTime)
                    this.newyorkSessionCloseDateTime = this.newyorkSessionCloseDateTime.AddDays(1);

                this.newyorkRangeCache.Insert(0, this.newyorkRange = new CountryRangeItem(currentBarTime, this.Open()));
            }

            // update High/Low/Close in history
            if (args.Reason == UpdateReason.HistoricalBar)
                this.newyorkRange.TryUpdate(this.High(), this.Low(), this.Close());
            else
                this.newyorkRange.TryUpdate(this.Close());

            this.newyorkRange.EndDateTime = currentBarTime;
        }

        var inAsiaSession = this.asiaSession.ContainsDate(currentBarTime);
        // Main range
        if (inAsiaSession)
        {
            // create new 'main' range
            if (this.asiaRange == null || currentBarTime >= this.asiaSessionCloseDateTime)
            {
                this.asiaSessionOpenDateTime = currentBarTime.Date.AddTicks(this.asiaSession.OpenTime.Ticks);
                this.asiaSessionCloseDateTime = currentBarTime.Date.AddTicks(this.asiaSession.CloseTime.Ticks);

                if (currentBarTime < this.asiaSessionOpenDateTime)
                    this.asiaSessionOpenDateTime = this.asiaSessionOpenDateTime.AddDays(-1);
                if (currentBarTime > this.asiaSessionCloseDateTime)
                    this.asiaSessionCloseDateTime = this.asiaSessionCloseDateTime.AddDays(1);

                this.asiaRangeCache.Insert(0, this.asiaRange = new CountryRangeItem(currentBarTime, this.Open()));
            }

            // update High/Low/Close in history
            if (args.Reason == UpdateReason.HistoricalBar)
                this.asiaRange.TryUpdate(this.High(), this.Low(), this.Close());
            else
                this.asiaRange.TryUpdate(this.Close());

            this.asiaRange.EndDateTime = currentBarTime;
        }

        // ====================== telegram alert ========================
        if (this.PreviousDataOffset >= 0)
        {
            bool london = false, asia = false, newyork = false;
            CountryRangeItem londonRange = default, asiaRange = default, newyorkRange = default;
            if (this.londonRangeCache.Count > this.PreviousDataOffset + 1)
            {
                london = true;
                londonRange = this.londonRangeCache[this.PreviousDataOffset + 1];
            }
            if (this.asiaRangeCache.Count > this.PreviousDataOffset + 1)
            {
                asia = true;
                asiaRange = this.asiaRangeCache[this.PreviousDataOffset + 1];
            }
            if (this.newyorkRangeCache.Count > this.PreviousDataOffset + 1)
            {
                newyork = true;
                newyorkRange = this.newyorkRangeCache[this.PreviousDataOffset + 1];
            }

            DateTime currentCloseTime = this.HistoricalData.Time(0);
            List<string> currentCloseSession = new List<string>();

            if (asia && asiaRange.StartDateTime <= currentCloseTime.AddDays(-(this.PreviousDataOffset + 1)) && asiaRange.EndDateTime >= currentCloseTime.AddDays(-(this.PreviousDataOffset + 1)))
            {
                currentCloseSession.Add("Asia");
            }

            if (london && londonRange.StartDateTime <= currentCloseTime.AddDays(-(this.PreviousDataOffset + 1)) && londonRange.EndDateTime >= currentCloseTime.AddDays(-(this.PreviousDataOffset + 1)))
            {
                currentCloseSession.Add("London");
            }

            if (newyork && newyorkRange.StartDateTime <= currentCloseTime.AddDays(-(this.PreviousDataOffset + 1)) && newyorkRange.EndDateTime >= currentCloseTime.AddDays(-(this.PreviousDataOffset + 1)))
            {
                currentCloseSession.Add("New York");
            }

            string curSessionString = String.Join('&', currentCloseSession);
            string highLondonAlertId = "", lowLondonAlertId = "", highAsiaAlertId = "", lowAsiaAlertId = "", highNewYorkAlertId = "", lowNewYorkAlertId = "";
            if (asia)
            {
                highAsiaAlertId = $"{currentCloseTime}_{curSessionString}_{asiaRange.StartDateTime}_asia_high";
                lowAsiaAlertId = $"{currentCloseTime}_{curSessionString}_{asiaRange.StartDateTime}_asia_low";
            }
            if (london)
            {
                highLondonAlertId = $"{currentCloseTime}_{curSessionString}_{londonRange.StartDateTime}_london_high";
                lowLondonAlertId = $"{currentCloseTime}_{curSessionString}_{londonRange.StartDateTime}_london_low";
            }
            if (newyork)
            {
                highNewYorkAlertId = $"{currentCloseTime}_{curSessionString}_{newyorkRange.StartDateTime}_newyork_high";
                lowNewYorkAlertId = $"{currentCloseTime}_{curSessionString}_{newyorkRange.StartDateTime}_newyork_low";
            }

            //SendTelegramAlert($"{curSessionString} alert london HIGH ({londonRange.High}) Low ({londonRange.Low}) value ({currentClose}) on {TimeZoneInfo.ConvertTimeFromUtc(currentCloseTime, TimeZoneInfo.Local)} for {TimeZoneInfo.ConvertTimeFromUtc(londonRange.StartDateTime.Date, TimeZoneInfo.Local)}", highLondonAlertId);
            //SendTelegramAlert($"{curSessionString} alert asia HIGH ({asiaRange.High}) Low ({asiaRange.Low}) value ({currentClose}) on {TimeZoneInfo.ConvertTimeFromUtc(currentCloseTime, TimeZoneInfo.Local)} for {TimeZoneInfo.ConvertTimeFromUtc(asiaRange.StartDateTime.Date, TimeZoneInfo.Local)}", highLondonAlertId);
            //SendTelegramAlert($"{curSessionString} alert newyork HIGH ({newyorkRange.High}) Low ({newyorkRange.Low}) value ({currentClose}) on {TimeZoneInfo.ConvertTimeFromUtc(currentCloseTime, TimeZoneInfo.Local)} for {TimeZoneInfo.ConvertTimeFromUtc(newyorkRange.StartDateTime.Date, TimeZoneInfo.Local)}", highLondonAlertId);
            if (london && !sentAlerts.Contains(highLondonAlertId) && currentClose > londonRange.High)
            {
                SendTelegramAlert($"{curSessionString} Price closed ABOVE previous London session HIGH ({londonRange.High}) on {TimeZoneInfo.ConvertTimeFromUtc(currentCloseTime, TimeZoneInfo.Local)}", highLondonAlertId);
                sentAlerts.Add(highLondonAlertId);
            }
            if (london && !sentAlerts.Contains(lowLondonAlertId) && currentClose < londonRange.Low)
            {
                SendTelegramAlert($"{curSessionString} Price closed BELOW previous London session LOW ({londonRange.Low}) on {TimeZoneInfo.ConvertTimeFromUtc(currentCloseTime, TimeZoneInfo.Local)}", lowLondonAlertId);
                sentAlerts.Add(lowLondonAlertId);
            }
            if (asia && !sentAlerts.Contains(highAsiaAlertId) && currentClose > asiaRange.High)
            {
                SendTelegramAlert($"{curSessionString} Price closed ABOVE previous Asia session HIGH ({asiaRange.High}) on {TimeZoneInfo.ConvertTimeFromUtc(currentCloseTime, TimeZoneInfo.Local)}", highAsiaAlertId);
                sentAlerts.Add(highAsiaAlertId);
            }
            if (asia && !sentAlerts.Contains(lowAsiaAlertId) && currentClose < asiaRange.Low)
            {
                SendTelegramAlert($"{curSessionString} Price closed BELOW previous Asia session LOW ({asiaRange.Low}) on {TimeZoneInfo.ConvertTimeFromUtc(currentCloseTime, TimeZoneInfo.Local)}", lowAsiaAlertId);
                sentAlerts.Add(lowAsiaAlertId);
            }
            if (newyork && !sentAlerts.Contains(highNewYorkAlertId) && currentClose > newyorkRange.High)
            {
                SendTelegramAlert($"{curSessionString} Price closed ABOVE previous New York session HIGH ({newyorkRange.High}) on {TimeZoneInfo.ConvertTimeFromUtc(currentCloseTime, TimeZoneInfo.Local)}", highNewYorkAlertId);
                sentAlerts.Add(highNewYorkAlertId);
            }
            if (newyork && !sentAlerts.Contains(lowNewYorkAlertId) && currentClose < newyorkRange.Low)
            {
                SendTelegramAlert($"{curSessionString} Price closed BELOW previous New York session LOW ({newyorkRange.Low}) on {TimeZoneInfo.ConvertTimeFromUtc(currentCloseTime, TimeZoneInfo.Local)}", lowNewYorkAlertId);
                sentAlerts.Add(lowNewYorkAlertId);
            }
        }
        // ====================== telegram alert ========================
    }
    protected override void OnClear()
    {
        this.londonRange = null;
        this.londonSession = null;
        this.londonSessionOpenDateTime = default;
        this.londonSessionCloseDateTime = default;

        this.londonRangeCache?.Clear();

        this.newyorkRange = null;
        this.newyorkSession = null;
        this.newyorkSessionOpenDateTime = default;
        this.newyorkSessionCloseDateTime = default;

        this.newyorkRangeCache?.Clear();

        this.asiaRange = null;
        this.asiaSession = null;
        this.asiaSessionOpenDateTime = default;
        this.asiaSessionCloseDateTime = default;

        this.asiaRangeCache?.Clear();
    }

    public override IList<SettingItem> Settings
    {
        get
        {
            var settings = base.Settings;

            var belowTL = new SelectItem("Below the line", 0);
            var aboveTL = new SelectItem("Above the line", 1);

            var formatPrice = new SelectItem("Price", 0);
            var formatTextPrice = new SelectItem("Text and Price", 1);
            var formatText = new SelectItem("Text", 2);

            //
            var defaultSeparator = settings.FirstOrDefault()?.SeparatorGroup;

            settings.Add(new SettingItemInteger("Numbers of days to calculate", this.DaysCount, 20)
            {
                SeparatorGroup = defaultSeparator,
                Text = loc._("Numbers of days to calculate"),
            });
            settings.Add(new SettingItemInteger("Use previous data (offset in days)", this.PreviousDataOffset, 20)
            {
                SeparatorGroup = defaultSeparator,
                Text = loc._("Use previous data (offset in days)"),
            });
            settings.Add(new SettingItemString("teleBotToken", this.telegramBotToken)
            {
                Text = loc._("Telegram Bot Token"),
                SeparatorGroup = defaultSeparator
            });
            settings.Add(new SettingItemString("teleChatId", this.telegramChatId)
            {
                Text = loc._("Telegram Chat ID"),
                SeparatorGroup = defaultSeparator
            });
            //
            settings.Add(new SettingItemBoolean("ShowLabel", this.ShowLabel, 60)
            {
                SeparatorGroup = defaultSeparator,
                Text = loc._("Show label")
            });
            settings.Add(new SettingItemFont("Font", this.CurrentFont, 60)
            {
                SeparatorGroup = defaultSeparator,
                Text = loc._("Font"),
                Relation = new SettingItemRelationVisibility("ShowLabel", true)
            });
            settings.Add(new SettingItemAlignment("Label alignment", this.LabelAlignment, 60)
            {
                Text = loc._("Label alignment"),
                SeparatorGroup = defaultSeparator,
                Relation = new SettingItemRelationVisibility("ShowLabel", true)
            });
            settings.Add(new SettingItemSelectorLocalized("Label position", new SelectItem("Label position", this.labelPosition), new List<SelectItem>
                             {
                                 belowTL,
                                 aboveTL
                             })
            {
                SeparatorGroup = defaultSeparator,
                Text = "Label position",
                SortIndex = 60,
                Relation = new SettingItemRelationVisibility("ShowLabel", true)
            });
            settings.Add(new SettingItemSelectorLocalized("Format", new SelectItem("Format", this.labelFormat), new List<SelectItem>
                             {
                                 formatPrice,
                                 formatTextPrice,
                                 formatText
                             })
            {
                SeparatorGroup = defaultSeparator,
                Text = "Format",
                SortIndex = 60,
                Relation = new SettingItemRelationVisibility("ShowLabel", true)
            });

            //
            //var openLineStyleSeparator = new SettingItemSeparatorGroup("Open line style", -999);
            //settings.Add(new SettingItemLineOptions("OpenLineOptions", this.OpenLineOptions, 60)
            //{
            //    SeparatorGroup = openLineStyleSeparator,
            //    Text = loc._("Main line style"),
            //    ExcludedStyles = new LineStyle[] { LineStyle.Points }
            //});
            //settings.Add(new SettingItemBoolean("ShowOpenLineLabel", this.ShowOpenLineLabel, 60)
            //{
            //    SeparatorGroup = openLineStyleSeparator,
            //    Text = loc._("Show line label")
            //});
            //settings.Add(new SettingItemString("OpenCustomText", this.OpenCustomText, 60)
            //{
            //    SeparatorGroup = openLineStyleSeparator,
            //    Text = loc._("Custom text"),
            //    Relation = new SettingItemRelationVisibility("Format", formatText, formatTextPrice)
            //});
            //
            var asiaSessionSeparator = new SettingItemSeparatorGroup("Asia Session Setting", -999);
            settings.Add(new SettingItemDateTime("Asia Start", this.asiaRangeStartTime, 20)
            {
                SeparatorGroup = asiaSessionSeparator,
                Text = loc._("Asia Start"),
                Format = DatePickerFormat.LongTime,
                ValueChangingBehavior = SettingItemValueChangingBehavior.WithConfirmation,
            });
            settings.Add(new SettingItemDateTime("Asia End", this.asiaRangeEndTime, 20)
            {
                SeparatorGroup = asiaSessionSeparator,
                Text = loc._("Asia End"),
                Format = DatePickerFormat.LongTime,
                ValueChangingBehavior = SettingItemValueChangingBehavior.WithConfirmation,
            });
            settings.Add(new SettingItemLineOptions("AsiaHighLineOptions", this.AsiaHighLineOptions, 60)
            {
                SeparatorGroup = asiaSessionSeparator,
                Text = loc._("High line style"),
                ExcludedStyles = new LineStyle[] { LineStyle.Points }
            });
            settings.Add(new SettingItemBoolean("ShowAsiaHighLineLabel", this.ShowAsiaHighLineLabel, 60)
            {
                SeparatorGroup = asiaSessionSeparator,
                Text = loc._("Show high line label")
            });
            settings.Add(new SettingItemString("AsiaHighCustomText", this.AsiaHighCustomText, 60)
            {
                SeparatorGroup = asiaSessionSeparator,
                Text = loc._("High line custom text"),
                Relation = new SettingItemRelationVisibility("Format", formatText, formatTextPrice)
            });
            settings.Add(new SettingItemLineOptions("AsiaLowLineOptions", this.AsiaLowLineOptions, 60)
            {
                SeparatorGroup = asiaSessionSeparator,
                Text = loc._("Low line style"),
                ExcludedStyles = new LineStyle[] { LineStyle.Points }
            });
            settings.Add(new SettingItemBoolean("ShowAsiaLowLineLabel", this.ShowAsiaLowLineLabel, 60)
            {
                SeparatorGroup = asiaSessionSeparator,
                Text = loc._("Show low line label")
            });
            settings.Add(new SettingItemString("AsiaLowCustomText", this.AsiaLowCustomText, 60)
            {
                SeparatorGroup = asiaSessionSeparator,
                Text = loc._("Low line custom text"),
                Relation = new SettingItemRelationVisibility("Format", formatText, formatTextPrice)
            });
            settings.Add(new SettingItemLineOptions("AsiaMiddleLineOptions", this.AsiaMiddleLineOptions, 60)
            {
                SeparatorGroup = asiaSessionSeparator,
                Text = loc._("Middle line style"),
                ExcludedStyles = new LineStyle[] { LineStyle.Points }
            });
            settings.Add(new SettingItemBoolean("ShowAsiaMiddleLineLabel", this.ShowAsiaMiddleLineLabel, 60)
            {
                SeparatorGroup = asiaSessionSeparator,
                Text = loc._("Show middle line label")
            });
            settings.Add(new SettingItemString("AsiaMiddleCustomText", this.AsiaMiddleCustomText, 60)
            {
                SeparatorGroup = asiaSessionSeparator,
                Text = loc._("Middle line custom text"),
                Relation = new SettingItemRelationVisibility("Format", formatText, formatTextPrice)
            });


            var londonSessionSeparator = new SettingItemSeparatorGroup("London Session Setting", -999);
            settings.Add(new SettingItemDateTime("London Start", this.londonRangeStartTime, 20)
            {
                SeparatorGroup = londonSessionSeparator,
                Text = loc._("London Start"),
                Format = DatePickerFormat.LongTime,
                ValueChangingBehavior = SettingItemValueChangingBehavior.WithConfirmation,
            });
            settings.Add(new SettingItemDateTime("London End", this.londonRangeEndTime, 20)
            {
                SeparatorGroup = londonSessionSeparator,
                Text = loc._("London End"),
                Format = DatePickerFormat.LongTime,
                ValueChangingBehavior = SettingItemValueChangingBehavior.WithConfirmation,
            });
            settings.Add(new SettingItemLineOptions("LondonHighLineOptions", this.LondonHighLineOptions, 60)
            {
                SeparatorGroup = londonSessionSeparator,
                Text = loc._("High line style"),
                ExcludedStyles = new LineStyle[] { LineStyle.Points }
            });
            settings.Add(new SettingItemBoolean("ShowLondonHighLineLabel", this.ShowLondonHighLineLabel, 60)
            {
                SeparatorGroup = londonSessionSeparator,
                Text = loc._("Show high line label")
            });
            settings.Add(new SettingItemString("LondonHighCustomText", this.LondonHighCustomText, 60)
            {
                SeparatorGroup = londonSessionSeparator,
                Text = loc._("High line custom text"),
                Relation = new SettingItemRelationVisibility("Format", formatText, formatTextPrice)
            });
            settings.Add(new SettingItemLineOptions("LondonLowLineOptions", this.LondonLowLineOptions, 60)
            {
                SeparatorGroup = londonSessionSeparator,
                Text = loc._("Low line style"),
                ExcludedStyles = new LineStyle[] { LineStyle.Points }
            });
            settings.Add(new SettingItemBoolean("ShowLondonLowLineLabel", this.ShowLondonLowLineLabel, 60)
            {
                SeparatorGroup = londonSessionSeparator,
                Text = loc._("Show low line label")
            });
            settings.Add(new SettingItemString("LondonLowCustomText", this.LondonLowCustomText, 60)
            {
                SeparatorGroup = londonSessionSeparator,
                Text = loc._("Low line custom text"),
                Relation = new SettingItemRelationVisibility("Format", formatText, formatTextPrice)
            });
            settings.Add(new SettingItemLineOptions("LondonMiddleLineOptions", this.LondonMiddleLineOptions, 60)
            {
                SeparatorGroup = londonSessionSeparator,
                Text = loc._("Middle line style"),
                ExcludedStyles = new LineStyle[] { LineStyle.Points }
            });
            settings.Add(new SettingItemBoolean("ShowLondonMiddleLineLabel", this.ShowLondonMiddleLineLabel, 60)
            {
                SeparatorGroup = londonSessionSeparator,
                Text = loc._("Show middle line label")
            });
            settings.Add(new SettingItemString("LondonMiddleCustomText", this.LondonMiddleCustomText, 60)
            {
                SeparatorGroup = londonSessionSeparator,
                Text = loc._("Middle line custom text"),
                Relation = new SettingItemRelationVisibility("Format", formatText, formatTextPrice)
            });


            var newyorkSessionSeparator = new SettingItemSeparatorGroup("NewYork Session Setting", -999);
            settings.Add(new SettingItemDateTime("NewYork Start", this.newyorkRangeStartTime, 20)
            {
                SeparatorGroup = newyorkSessionSeparator,
                Text = loc._("New York Start"),
                Format = DatePickerFormat.LongTime,
                ValueChangingBehavior = SettingItemValueChangingBehavior.WithConfirmation,
            });
            settings.Add(new SettingItemDateTime("New York End", this.newyorkRangeEndTime, 20)
            {
                SeparatorGroup = newyorkSessionSeparator,
                Text = loc._("New York End"),
                Format = DatePickerFormat.LongTime,
                ValueChangingBehavior = SettingItemValueChangingBehavior.WithConfirmation,
            });
            settings.Add(new SettingItemLineOptions("NewYorkHighLineOptions", this.NewYorkHighLineOptions, 60)
            {
                SeparatorGroup = newyorkSessionSeparator,
                Text = loc._("High line style"),
                ExcludedStyles = new LineStyle[] { LineStyle.Points }
            });
            settings.Add(new SettingItemBoolean("ShowNewYorkHighLineLabel", this.ShowNewYorkHighLineLabel, 60)
            {
                SeparatorGroup = newyorkSessionSeparator,
                Text = loc._("Show high line label")
            });
            settings.Add(new SettingItemString("NewYorkHighCustomText", this.NewYorkHighCustomText, 60)
            {
                SeparatorGroup = newyorkSessionSeparator,
                Text = loc._("High line custom text"),
                Relation = new SettingItemRelationVisibility("Format", formatText, formatTextPrice)
            });
            settings.Add(new SettingItemLineOptions("NewYorkLowLineOptions", this.NewYorkLowLineOptions, 60)
            {
                SeparatorGroup = newyorkSessionSeparator,
                Text = loc._("Low line style"),
                ExcludedStyles = new LineStyle[] { LineStyle.Points }
            });
            settings.Add(new SettingItemBoolean("ShowNewYorkLowLineLabel", this.ShowNewYorkLowLineLabel, 60)
            {
                SeparatorGroup = newyorkSessionSeparator,
                Text = loc._("Show low line label")
            });
            settings.Add(new SettingItemString("NewYorkLowCustomText", this.NewYorkLowCustomText, 60)
            {
                SeparatorGroup = newyorkSessionSeparator,
                Text = loc._("Low line custom text"),
                Relation = new SettingItemRelationVisibility("Format", formatText, formatTextPrice)
            });
            settings.Add(new SettingItemLineOptions("NewYorkMiddleLineOptions", this.NewYorkMiddleLineOptions, 60)
            {
                SeparatorGroup = newyorkSessionSeparator,
                Text = loc._("Middle line style"),
                ExcludedStyles = new LineStyle[] { LineStyle.Points }
            });
            settings.Add(new SettingItemBoolean("ShowNewYorkMiddleLineLabel", this.ShowNewYorkMiddleLineLabel, 60)
            {
                SeparatorGroup = newyorkSessionSeparator,
                Text = loc._("Show middle line label")
            });
            settings.Add(new SettingItemString("NewYorkMiddleCustomText", this.NewYorkMiddleCustomText, 60)
            {
                SeparatorGroup = newyorkSessionSeparator,
                Text = loc._("Middle line custom text"),
                Relation = new SettingItemRelationVisibility("Format", formatText, formatTextPrice)
            });
            //
            //var closeLineStyleSeparator = new SettingItemSeparatorGroup("Close line style", -999);
            //settings.Add(new SettingItemLineOptions("CloseLineOptions", this.CloseLineOptions, 60)
            //{
            //    SeparatorGroup = closeLineStyleSeparator,
            //    Text = loc._("Main line style"),
            //    ExcludedStyles = new LineStyle[] { LineStyle.Points }
            //});
            //settings.Add(new SettingItemBoolean("ShowCloseLineLabel", this.ShowCloseLineLabel, 60)
            //{
            //    SeparatorGroup = closeLineStyleSeparator,
            //    Text = loc._("Show line label")
            //});
            //settings.Add(new SettingItemString("CloseCustomText", this.CloseCustomText, 60)
            //{
            //    SeparatorGroup = closeLineStyleSeparator,
            //    Text = loc._("Custom text"),
            //    Relation = new SettingItemRelationVisibility("Format", formatText, formatTextPrice)
            //});
            //

            return settings;
        }
        set
        {
            var holder = new SettingsHolder(value);

            if (value.TryGetValue("teleBotToken", out string telegramBotToken))
                this.telegramBotToken = telegramBotToken;

            if (value.TryGetValue("teleChatId", out string telegramChatId))
                this.telegramChatId = telegramChatId;

            //if (holder.TryGetValue("OpenLineOptions", out SettingItem item) && item.Value is LineOptions openOptions)
            //    this.OpenLineOptions = openOptions;
            //if (holder.TryGetValue("ShowOpenLineLabel", out item) && item.Value is bool showOpenLabel)
            //    this.ShowOpenLineLabel = showOpenLabel;
            //if (holder.TryGetValue("OpenCustomText", out item) && item.Value is string openCustomText)
            //    this.OpenCustomText = openCustomText;

            //if (holder.TryGetValue("CloseLineOptions", out item) && item.Value is LineOptions closeOptions)
            //    this.CloseLineOptions = closeOptions;
            //if (holder.TryGetValue("ShowCloseLineLabel", out item) && item.Value is bool showCloseLabel)
            //    this.ShowCloseLineLabel = showCloseLabel;
            //if (holder.TryGetValue("CloseCustomText", out item) && item.Value is string closeCustomText)
            //    this.CloseCustomText = closeCustomText;

            if (holder.TryGetValue("AsiaHighLineOptions", out SettingItem item) && item.Value is LineOptions highAsiaOptions)
                this.AsiaHighLineOptions = highAsiaOptions;
            if (holder.TryGetValue("ShowAsiaHighLineLabel", out item) && item.Value is bool showAsiaHighLabel)
                this.ShowAsiaHighLineLabel = showAsiaHighLabel;
            if (holder.TryGetValue("AsiaHighCustomText", out item) && item.Value is string highAsiaCustomText)
                this.AsiaHighCustomText = highAsiaCustomText;

            if (holder.TryGetValue("AsiaLowLineOptions", out item) && item.Value is LineOptions lowAsiaOptions)
                this.AsiaLowLineOptions = lowAsiaOptions;
            if (holder.TryGetValue("ShowAsiaLowLineLabel", out item) && item.Value is bool showAsiaLowLabel)
                this.ShowAsiaLowLineLabel = showAsiaLowLabel;
            if (holder.TryGetValue("AsiaLowCustomText", out item) && item.Value is string lowAsiaCustomText)
                this.AsiaLowCustomText = lowAsiaCustomText;

            if (holder.TryGetValue("AsiaMiddleLineOptions", out item) && item.Value is LineOptions middleAsiaOptions)
                this.AsiaMiddleLineOptions = middleAsiaOptions;
            if (holder.TryGetValue("ShowAsiaMiddleLineLabel", out item) && item.Value is bool showAsiaMiddleLabel)
                this.ShowAsiaMiddleLineLabel = showAsiaMiddleLabel;
            if (holder.TryGetValue("AsiaMiddleCustomText", out item) && item.Value is string middleAsiaCustomText)
                this.AsiaMiddleCustomText = middleAsiaCustomText;

            if (holder.TryGetValue("LondonHighLineOptions", out item) && item.Value is LineOptions highLondonOptions)
                this.LondonHighLineOptions = highLondonOptions;
            if (holder.TryGetValue("ShowLondonHighLineLabel", out item) && item.Value is bool showLondonHighLabel)
                this.ShowLondonHighLineLabel = showLondonHighLabel;
            if (holder.TryGetValue("LondonHighCustomText", out item) && item.Value is string highLondonCustomText)
                this.LondonHighCustomText = highLondonCustomText;

            if (holder.TryGetValue("LondonLowLineOptions", out item) && item.Value is LineOptions lowLondonOptions)
                this.LondonLowLineOptions = lowLondonOptions;
            if (holder.TryGetValue("ShowLondonLowLineLabel", out item) && item.Value is bool showLondonLowLabel)
                this.ShowLondonLowLineLabel = showLondonLowLabel;
            if (holder.TryGetValue("LondonLowCustomText", out item) && item.Value is string lowLondonCustomText)
                this.LondonLowCustomText = lowLondonCustomText;

            if (holder.TryGetValue("LondonMiddleLineOptions", out item) && item.Value is LineOptions middleLondonOptions)
                this.LondonMiddleLineOptions = middleLondonOptions;
            if (holder.TryGetValue("ShowLondonMiddleLineLabel", out item) && item.Value is bool showLondonMiddleLabel)
                this.ShowLondonMiddleLineLabel = showLondonMiddleLabel;
            if (holder.TryGetValue("LondonMiddleCustomText", out item) && item.Value is string middleLondonCustomText)
                this.LondonMiddleCustomText = middleLondonCustomText;

            if (holder.TryGetValue("NewYorkHighLineOptions", out item) && item.Value is LineOptions highNewYorkOptions)
                this.NewYorkHighLineOptions = highNewYorkOptions;
            if (holder.TryGetValue("ShowNewYorkHighLineLabel", out item) && item.Value is bool showNewYorkHighLabel)
                this.ShowNewYorkHighLineLabel = showNewYorkHighLabel;
            if (holder.TryGetValue("NewYorkHighCustomText", out item) && item.Value is string highNewYorkCustomText)
                this.NewYorkHighCustomText = highNewYorkCustomText;

            if (holder.TryGetValue("NewYorkLowLineOptions", out item) && item.Value is LineOptions lowNewYorkOptions)
                this.NewYorkLowLineOptions = lowNewYorkOptions;
            if (holder.TryGetValue("ShowNewYorkLowLineLabel", out item) && item.Value is bool showNewYorkLowLabel)
                this.ShowNewYorkLowLineLabel = showNewYorkLowLabel;
            if (holder.TryGetValue("NewYorkLowCustomText", out item) && item.Value is string lowNewYorkCustomText)
                this.NewYorkLowCustomText = lowNewYorkCustomText;

            if (holder.TryGetValue("NewYorkMiddleLineOptions", out item) && item.Value is LineOptions middleNewYorkOptions)
                this.NewYorkMiddleLineOptions = middleNewYorkOptions;
            if (holder.TryGetValue("ShowNewYorkMiddleLineLabel", out item) && item.Value is bool showNewYorkMiddleLabel)
                this.ShowNewYorkMiddleLineLabel = showNewYorkMiddleLabel;
            if (holder.TryGetValue("NewYorkMiddleCustomText", out item) && item.Value is string middleNewYorkCustomText)
                this.NewYorkMiddleCustomText = middleNewYorkCustomText;

            var needRefresh = false;
            if (holder.TryGetValue("Asia Start", out item) && item.Value is DateTime dtAsiaStartTime)
            {
                this.asiaRangeStartTime = dtAsiaStartTime;
                needRefresh |= item.ValueChangingReason == SettingItemValueChangingReason.Manually;
            }
            if (holder.TryGetValue("Asia End", out item) && item.Value is DateTime dtAsiaEndTime)
            {
                this.asiaRangeEndTime = dtAsiaEndTime;
                needRefresh |= item.ValueChangingReason == SettingItemValueChangingReason.Manually;
            }
            if (holder.TryGetValue("London Start", out item) && item.Value is DateTime dtLondonStartTime)
            {
                this.londonRangeStartTime = dtLondonStartTime;
                needRefresh |= item.ValueChangingReason == SettingItemValueChangingReason.Manually;
            }
            if (holder.TryGetValue("London End", out item) && item.Value is DateTime dtLondonEndTime)
            {
                this.londonRangeEndTime = dtLondonEndTime;
                needRefresh |= item.ValueChangingReason == SettingItemValueChangingReason.Manually;
            }
            if (holder.TryGetValue("New York Start", out item) && item.Value is DateTime dtNewYorkStartTime)
            {
                this.newyorkRangeStartTime = dtNewYorkStartTime;
                needRefresh |= item.ValueChangingReason == SettingItemValueChangingReason.Manually;
            }
            if (holder.TryGetValue("New York End", out item) && item.Value is DateTime dtNewYorkEndTime)
            {
                this.newyorkRangeEndTime = dtNewYorkEndTime;
                needRefresh |= item.ValueChangingReason == SettingItemValueChangingReason.Manually;
            }
            if (holder.TryGetValue("Numbers of days to calculate", out item) && item.Value is int daysCount)
                this.DaysCount = daysCount;
            if (holder.TryGetValue("Use previous data (offset in days)", out item) && item.Value is int previousDataOffset)
                this.PreviousDataOffset = previousDataOffset;
            if (holder.TryGetValue("Font", out item) && item.Value is Font font)
                this.CurrentFont = font;
            if (holder.TryGetValue("Label alignment", out item) && item.Value is NativeAlignment labelAlignment)
                this.LabelAlignment = labelAlignment;
            if (holder.TryGetValue("ShowLabel", out item) && item.Value is bool showLabel)
                this.ShowLabel = showLabel;
            if (holder.TryGetValue("Label position", out var lpitem)&& lpitem.GetValue<int>() != this.labelPosition)
                this.labelPosition = lpitem.GetValue<int>();
            if (holder.TryGetValue("Format", out var lfitem)&& lfitem.GetValue<int>() != this.labelFormat)
                this.labelFormat = lfitem.GetValue<int>();
            if (needRefresh)
                this.Refresh();
            base.Settings = value;
        }
    }

    public override void OnPaintChart(PaintChartEventArgs args)
    {
        var gr = args.Graphics;
        var restoredClip = gr.ClipBounds;

        gr.SetClip(args.Rectangle);

        try
        {
            var currentWindow = this.CurrentChart.Windows[args.WindowIndex];

            var leftTime = currentWindow.CoordinatesConverter.GetTime(args.Rectangle.Left);
            var rightTime = currentWindow.CoordinatesConverter.GetTime(args.Rectangle.Right);

            int halfBarWidth = this.CurrentChart.BarsWidth / 2;
            for (int i = 0; i < this.DaysCount; i++)
            {
                if (i >= this.londonRangeCache.Count)
                    break;

                var range = this.londonRangeCache[i];

                bool isMainRangeOuside = range.EndDateTime < leftTime || range.StartDateTime > rightTime;

                if (isMainRangeOuside)
                {
                    if (!isMainRangeOuside || range.ExtendEndDateTime < leftTime || range.ExtendStartDateTime > rightTime)
                        continue;
                }

                int prevDailyRangeOffset = i + this.PreviousDataOffset;
                bool needUsePreviosRange = prevDailyRangeOffset != i;

                if (needUsePreviosRange && prevDailyRangeOffset >= this.londonRangeCache.Count)
                    break;

                float leftX = (float)currentWindow.CoordinatesConverter.GetChartX(range.StartDateTime) + halfBarWidth;
                if (leftX < args.Rectangle.Left)
                    leftX = args.Rectangle.Left;

                float rightX = (float)currentWindow.CoordinatesConverter.GetChartX(range.EndDateTime) + halfBarWidth;
                if (rightX > args.Rectangle.Right)
                    rightX = args.Rectangle.Right;

                float leftExtendX = default;
                float rightExtendX = default;

                int top = args.Rectangle.Top;
                int bottom = args.Rectangle.Bottom;

                // get previous date
                if (needUsePreviosRange)
                    range = this.londonRangeCache[prevDailyRangeOffset];

                if (this.LondonHighLineOptions.Enabled)
                {
                    float highY = (float)currentWindow.CoordinatesConverter.GetChartY(range.High);
                    if (highY > top && highY < bottom)
                    {
                        if (!isMainRangeOuside && this.LondonHighLineOptions.Enabled)
                        {
                            gr.DrawLine(this.londonhighLinePen, leftX, highY, rightX, highY);

                            if (this.ShowLondonHighLineLabel)
                                this.DrawBillet(gr, range.High, ref leftX, ref rightX, ref highY, this.CurrentFont, this.londonhighLineOptions, this.londonhighLinePen, this.centerNearSF, this.LabelAlignment, LondonHighCustomText);
                        }
                    }
                }

                if (this.LondonLowLineOptions.Enabled)
                {
                    float lowY = (float)currentWindow.CoordinatesConverter.GetChartY(range.Low);
                    if (lowY > top && lowY < bottom)
                    {
                        if (!isMainRangeOuside && this.LondonLowLineOptions.Enabled)
                        {
                            gr.DrawLine(this.londonlowLinePen, leftX, lowY, rightX, lowY);

                            if (this.ShowLondonLowLineLabel)
                                this.DrawBillet(gr, range.Low, ref leftX, ref rightX, ref lowY, this.CurrentFont, this.londonlowLineOptions, this.londonlowLinePen, this.centerNearSF, this.LabelAlignment, LondonLowCustomText);
                        }
                    }
                }

                //if (this.OpenLineOptions.Enabled)
                //{
                //    float openY = (float)currentWindow.CoordinatesConverter.GetChartY(range.Open);
                //    if (openY > top && openY < bottom)
                //    {
                //        if (!isMainRangeOuside && this.OpenLineOptions.Enabled)
                //        {
                //            gr.DrawLine(this.openLinePen, leftX, openY, rightX, openY);

                //            if (this.ShowOpenLineLabel)
                //                this.DrawBillet(gr, range.Open, ref leftX, ref rightX, ref openY, this.CurrentFont, this.openLineOptions, this.openLinePen, this.centerNearSF, this.LabelAlignment, OpenCustomText);
                //        }
                //    }
                //}

                //if (this.CloseLineOptions.Enabled)
                //{
                //    float closeY = (float)currentWindow.CoordinatesConverter.GetChartY(range.Close);
                //    if (closeY > top && closeY < bottom)
                //    {
                //        if (!isMainRangeOuside && this.CloseLineOptions.Enabled)
                //        {
                //            gr.DrawLine(this.closeLinePen, leftX, closeY, rightX, closeY);

                //            if (this.ShowCloseLineLabel)
                //                this.DrawBillet(gr, range.Close, ref leftX, ref rightX, ref closeY, this.CurrentFont, this.closeLineOptions, this.closeLinePen, this.centerNearSF, this.LabelAlignment, CloseCustomText);
                //        }
                //    }
                //}

                if (this.LondonMiddleLineOptions.Enabled)
                {
                    float middleY = (float)currentWindow.CoordinatesConverter.GetChartY(range.MiddlePrice);
                    if (middleY > top && middleY < bottom)
                    {
                        if (!isMainRangeOuside && this.LondonMiddleLineOptions.Enabled)
                        {
                            gr.DrawLine(this.londonmiddleLinePen, leftX, middleY, rightX, middleY);

                            if (this.ShowLondonMiddleLineLabel)
                                this.DrawBillet(gr, range.MiddlePrice, ref leftX, ref rightX, ref middleY, this.CurrentFont, this.londonmiddleLineOptions, this.londonmiddleLinePen, this.centerNearSF, this.LabelAlignment, LondonMiddleCustomText);
                        }
                    }
                }
            }

            for (int i = 0; i < this.DaysCount; i++)
            {
                if (i >= this.newyorkRangeCache.Count)
                    break;

                var range = this.newyorkRangeCache[i];

                bool isMainRangeOuside = range.EndDateTime < leftTime || range.StartDateTime > rightTime;

                if (isMainRangeOuside)
                {
                    if (!isMainRangeOuside || range.ExtendEndDateTime < leftTime || range.ExtendStartDateTime > rightTime)
                        continue;
                }

                int prevDailyRangeOffset = i + this.PreviousDataOffset;
                bool needUsePreviosRange = prevDailyRangeOffset != i;

                if (needUsePreviosRange && prevDailyRangeOffset >= this.newyorkRangeCache.Count)
                    break;

                float leftX = (float)currentWindow.CoordinatesConverter.GetChartX(range.StartDateTime) + halfBarWidth;
                if (leftX < args.Rectangle.Left)
                    leftX = args.Rectangle.Left;

                float rightX = (float)currentWindow.CoordinatesConverter.GetChartX(range.EndDateTime) + halfBarWidth;
                if (rightX > args.Rectangle.Right)
                    rightX = args.Rectangle.Right;

                float leftExtendX = default;
                float rightExtendX = default;

                int top = args.Rectangle.Top;
                int bottom = args.Rectangle.Bottom;

                // get previous date
                if (needUsePreviosRange)
                    range = this.newyorkRangeCache[prevDailyRangeOffset];

                if (this.NewYorkHighLineOptions.Enabled)
                {
                    float highY = (float)currentWindow.CoordinatesConverter.GetChartY(range.High);
                    if (highY > top && highY < bottom)
                    {
                        if (!isMainRangeOuside && this.NewYorkHighLineOptions.Enabled)
                        {
                            gr.DrawLine(this.newyorkhighLinePen, leftX, highY, rightX, highY);

                            if (this.ShowNewYorkHighLineLabel)
                                this.DrawBillet(gr, range.High, ref leftX, ref rightX, ref highY, this.CurrentFont, this.newyorkhighLineOptions, this.newyorkhighLinePen, this.centerNearSF, this.LabelAlignment, NewYorkHighCustomText);
                        }
                    }
                }

                if (this.NewYorkLowLineOptions.Enabled)
                {
                    float lowY = (float)currentWindow.CoordinatesConverter.GetChartY(range.Low);
                    if (lowY > top && lowY < bottom)
                    {
                        if (!isMainRangeOuside && this.NewYorkLowLineOptions.Enabled)
                        {
                            gr.DrawLine(this.newyorklowLinePen, leftX, lowY, rightX, lowY);

                            if (this.ShowNewYorkLowLineLabel)
                                this.DrawBillet(gr, range.Low, ref leftX, ref rightX, ref lowY, this.CurrentFont, this.newyorklowLineOptions, this.newyorklowLinePen, this.centerNearSF, this.LabelAlignment, NewYorkLowCustomText);
                        }
                    }
                }

                //if (this.OpenLineOptions.Enabled)
                //{
                //    float openY = (float)currentWindow.CoordinatesConverter.GetChartY(range.Open);
                //    if (openY > top && openY < bottom)
                //    {
                //        if (!isMainRangeOuside && this.OpenLineOptions.Enabled)
                //        {
                //            gr.DrawLine(this.openLinePen, leftX, openY, rightX, openY);

                //            if (this.ShowOpenLineLabel)
                //                this.DrawBillet(gr, range.Open, ref leftX, ref rightX, ref openY, this.CurrentFont, this.openLineOptions, this.openLinePen, this.centerNearSF, this.LabelAlignment, OpenCustomText);
                //        }
                //    }
                //}

                //if (this.CloseLineOptions.Enabled)
                //{
                //    float closeY = (float)currentWindow.CoordinatesConverter.GetChartY(range.Close);
                //    if (closeY > top && closeY < bottom)
                //    {
                //        if (!isMainRangeOuside && this.CloseLineOptions.Enabled)
                //        {
                //            gr.DrawLine(this.closeLinePen, leftX, closeY, rightX, closeY);

                //            if (this.ShowCloseLineLabel)
                //                this.DrawBillet(gr, range.Close, ref leftX, ref rightX, ref closeY, this.CurrentFont, this.closeLineOptions, this.closeLinePen, this.centerNearSF, this.LabelAlignment, CloseCustomText);
                //        }
                //    }
                //}

                if (this.NewYorkMiddleLineOptions.Enabled)
                {
                    float middleY = (float)currentWindow.CoordinatesConverter.GetChartY(range.MiddlePrice);
                    if (middleY > top && middleY < bottom)
                    {
                        if (!isMainRangeOuside && this.NewYorkMiddleLineOptions.Enabled)
                        {
                            gr.DrawLine(this.newyorkmiddleLinePen, leftX, middleY, rightX, middleY);

                            if (this.ShowNewYorkMiddleLineLabel)
                                this.DrawBillet(gr, range.MiddlePrice, ref leftX, ref rightX, ref middleY, this.CurrentFont, this.newyorkmiddleLineOptions, this.newyorkmiddleLinePen, this.centerNearSF, this.LabelAlignment, NewYorkMiddleCustomText);
                        }
                    }
                }
            }

            for (int i = 0; i < this.DaysCount; i++)
            {
                if (i >= this.asiaRangeCache.Count)
                    break;

                var range = this.asiaRangeCache[i];

                bool isMainRangeOuside = range.EndDateTime < leftTime || range.StartDateTime > rightTime;

                if (isMainRangeOuside)
                {
                    if (!isMainRangeOuside || range.ExtendEndDateTime < leftTime || range.ExtendStartDateTime > rightTime)
                        continue;
                }

                int prevDailyRangeOffset = i + this.PreviousDataOffset;
                bool needUsePreviosRange = prevDailyRangeOffset != i;

                if (needUsePreviosRange && prevDailyRangeOffset >= this.asiaRangeCache.Count)
                    break;

                float leftX = (float)currentWindow.CoordinatesConverter.GetChartX(range.StartDateTime) + halfBarWidth;
                if (leftX < args.Rectangle.Left)
                    leftX = args.Rectangle.Left;

                float rightX = (float)currentWindow.CoordinatesConverter.GetChartX(range.EndDateTime) + halfBarWidth;
                if (rightX > args.Rectangle.Right)
                    rightX = args.Rectangle.Right;

                float leftExtendX = default;
                float rightExtendX = default;

                int top = args.Rectangle.Top;
                int bottom = args.Rectangle.Bottom;

                // get previous date
                if (needUsePreviosRange)
                    range = this.asiaRangeCache[prevDailyRangeOffset];

                if (this.AsiaHighLineOptions.Enabled)
                {
                    float highY = (float)currentWindow.CoordinatesConverter.GetChartY(range.High);
                    if (highY > top && highY < bottom)
                    {
                        if (!isMainRangeOuside && this.AsiaHighLineOptions.Enabled)
                        {
                            gr.DrawLine(this.asiahighLinePen, leftX, highY, rightX, highY);

                            if (this.ShowAsiaHighLineLabel)
                                this.DrawBillet(gr, range.High, ref leftX, ref rightX, ref highY, this.CurrentFont, this.asiahighLineOptions, this.asiahighLinePen, this.centerNearSF, this.LabelAlignment, AsiaHighCustomText);
                        }
                    }
                }

                if (this.AsiaLowLineOptions.Enabled)
                {
                    float lowY = (float)currentWindow.CoordinatesConverter.GetChartY(range.Low);
                    if (lowY > top && lowY < bottom)
                    {
                        if (!isMainRangeOuside && this.AsiaLowLineOptions.Enabled)
                        {
                            gr.DrawLine(this.asialowLinePen, leftX, lowY, rightX, lowY);

                            if (this.ShowAsiaLowLineLabel)
                                this.DrawBillet(gr, range.Low, ref leftX, ref rightX, ref lowY, this.CurrentFont, this.asialowLineOptions, this.asialowLinePen, this.centerNearSF, this.LabelAlignment, AsiaLowCustomText);
                        }
                    }
                }

                //if (this.OpenLineOptions.Enabled)
                //{
                //    float openY = (float)currentWindow.CoordinatesConverter.GetChartY(range.Open);
                //    if (openY > top && openY < bottom)
                //    {
                //        if (!isMainRangeOuside && this.OpenLineOptions.Enabled)
                //        {
                //            gr.DrawLine(this.openLinePen, leftX, openY, rightX, openY);

                //            if (this.ShowOpenLineLabel)
                //                this.DrawBillet(gr, range.Open, ref leftX, ref rightX, ref openY, this.CurrentFont, this.openLineOptions, this.openLinePen, this.centerNearSF, this.LabelAlignment, OpenCustomText);
                //        }
                //    }
                //}

                //if (this.CloseLineOptions.Enabled)
                //{
                //    float closeY = (float)currentWindow.CoordinatesConverter.GetChartY(range.Close);
                //    if (closeY > top && closeY < bottom)
                //    {
                //        if (!isMainRangeOuside && this.CloseLineOptions.Enabled)
                //        {
                //            gr.DrawLine(this.closeLinePen, leftX, closeY, rightX, closeY);

                //            if (this.ShowCloseLineLabel)
                //                this.DrawBillet(gr, range.Close, ref leftX, ref rightX, ref closeY, this.CurrentFont, this.closeLineOptions, this.closeLinePen, this.centerNearSF, this.LabelAlignment, CloseCustomText);
                //        }
                //    }
                //}

                if (this.AsiaMiddleLineOptions.Enabled)
                {
                    float middleY = (float)currentWindow.CoordinatesConverter.GetChartY(range.MiddlePrice);
                    if (middleY > top && middleY < bottom)
                    {
                        if (!isMainRangeOuside && this.AsiaMiddleLineOptions.Enabled)
                        {
                            gr.DrawLine(this.asiamiddleLinePen, leftX, middleY, rightX, middleY);

                            if (this.ShowAsiaMiddleLineLabel)
                                this.DrawBillet(gr, range.MiddlePrice, ref leftX, ref rightX, ref middleY, this.CurrentFont, this.asiamiddleLineOptions, this.asiamiddleLinePen, this.centerNearSF, this.LabelAlignment, AsiaMiddleCustomText);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Core.Loggers.Log(ex);
        }
        finally
        {
            gr.SetClip(restoredClip);
        }
    }

    #endregion Base overrides


    private void DrawBillet(Graphics gr, double price, ref float leftX, ref float rightX, ref float priceY, Font font, LineOptions lineOptions, Pen pen, StringFormat stringFormat, NativeAlignment nativeAlignment, string prefix)
    {
        string label = "";
        if (ShowLabel==true)
            label = labelFormat==1 ? prefix + this.Symbol.FormatPrice(price) : labelFormat == 0 ? this.Symbol.FormatPrice(price) : prefix;
        var labelSize = gr.MeasureString(label, font);

        var rect = new RectangleF()
        {
            Height = labelSize.Height,
            Width = labelSize.Width + 5,
            Y = labelPosition == 1 ? priceY - labelSize.Height - lineOptions.Width : priceY - lineOptions.Width + 1
        };

        switch (nativeAlignment)
        {
            case NativeAlignment.Center:
                {
                    rect.X = (rightX - leftX) / 2f + leftX - rect.Width / 2f;
                    break;
                }
            case NativeAlignment.Right:
                {
                    rect.X = rightX - rect.Width;
                    break;
                }
            case NativeAlignment.Left:
            default:
                {
                    rect.X = leftX;
                    break;
                }
        }

        gr.FillRectangle(pen.Brush, rect);
        gr.DrawString(label, font, Brushes.White, rect, stringFormat);
    }
    private Session CreateLondonSession()
    {
        // 07:00
        var startTime = new DateTime(Core.Instance.TimeUtils.DateTimeUtcNow.Date.AddHours(7).Ticks, DateTimeKind.Unspecified);
        // 15:00
        var endTime = new DateTime(Core.Instance.TimeUtils.DateTimeUtcNow.Date.AddHours(15).Ticks, DateTimeKind.Unspecified);

        var timeZone = this.CurrentChart?.CurrentTimeZone ?? Core.Instance.TimeUtils.SelectedTimeZone;
        return new Session("Default",
            Core.Instance.TimeUtils.ConvertFromTimeZoneToUTC(startTime, timeZone).TimeOfDay,
            Core.Instance.TimeUtils.ConvertFromTimeZoneToUTC(endTime, timeZone).TimeOfDay);
    }

    private Session CreateNewYorkSession()
    {
        // 07:00
        var startTime = new DateTime(Core.Instance.TimeUtils.DateTimeUtcNow.Date.AddHours(17).Ticks, DateTimeKind.Unspecified);
        // 15:00
        var endTime = new DateTime(Core.Instance.TimeUtils.DateTimeUtcNow.Date.AddHours(23).Ticks, DateTimeKind.Unspecified);

        var timeZone = this.CurrentChart?.CurrentTimeZone ?? Core.Instance.TimeUtils.SelectedTimeZone;
        return new Session("Default",
            Core.Instance.TimeUtils.ConvertFromTimeZoneToUTC(startTime, timeZone).TimeOfDay,
            Core.Instance.TimeUtils.ConvertFromTimeZoneToUTC(endTime, timeZone).TimeOfDay);
    }

    private Session CreateAsiaSession()
    {
        // 07:00
        var startTime = new DateTime(Core.Instance.TimeUtils.DateTimeUtcNow.Date.AddHours(22).Ticks, DateTimeKind.Unspecified);
        // 15:00
        var endTime = new DateTime(Core.Instance.TimeUtils.DateTimeUtcNow.Date.AddHours(9+24).AddMinutes(45).Ticks, DateTimeKind.Unspecified);

        var timeZone = this.CurrentChart?.CurrentTimeZone ?? Core.Instance.TimeUtils.SelectedTimeZone;
        return new Session("Default",
            Core.Instance.TimeUtils.ConvertFromTimeZoneToUTC(startTime, timeZone).TimeOfDay,
            Core.Instance.TimeUtils.ConvertFromTimeZoneToUTC(endTime, timeZone).TimeOfDay);
    }

    private static Pen ProcessPen(Pen pen, LineOptions lineOptions)
    {
        if (pen == null)
            pen = new Pen(Color.Empty);

        pen.Color = lineOptions.Color;
        pen.Width = lineOptions.Width;

        try
        {
            switch (lineOptions.LineStyle)
            {
                case LineStyle.Solid:
                    {
                        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                        break;
                    }
                case LineStyle.Dot:
                    {
                        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                        break;
                    }
                case LineStyle.Dash:
                    {
                        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        break;
                    }
                case LineStyle.DashDot:
                    {
                        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
                        float[] dp = new float[] { 2, 4, 7, 4 };
                        pen.DashPattern = dp;
                        break;
                    }
                case LineStyle.Histogramm:
                    {
                        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
                        float[] dp = new float[] { 0.25F, 1 };
                        pen.DashPattern = dp;
                        pen.Width = 4;
                        break;
                    }
            }
        }
        catch { }
        return pen;
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

#region Nested

internal class CountryRangeItem
{
    public double High { get; private set; }
    public double Low { get; private set; }
    public double Open { get; private set; }
    public double Close { get; private set; }
    public double MiddlePrice => (this.High + this.Low) / 2;

    public DateTime StartDateTime { get; internal set; }
    public DateTime EndDateTime { get; internal set; }
    public DateTime ExtendStartDateTime { get; internal set; }
    public DateTime ExtendEndDateTime { get; internal set; }

    public CountryRangeItem(DateTime startDateTime, double openPrice)
    {
        this.StartDateTime = startDateTime;
        this.Open = openPrice;

        this.High = double.MinValue;
        this.Low = double.MaxValue;
    }

    public bool TryUpdate(double high, double low, double close)
    {
        bool updated = false;

        if (this.High < high)
        {
            this.High = high;
            updated = true;
        }

        if (this.Low > low)
        {
            this.Low = low;
            updated = true;
        }

        if (this.Close != close)
        {
            this.Close = close;
            updated = true;
        }

        return updated;
    }
    public bool TryUpdate(double close)
    {
        bool updated = false;

        if (this.High < close)
        {
            this.High = close;
            updated = true;
        }

        if (this.Low > close)
        {
            this.Low = close;
            updated = true;
        }

        if (this.Close != close)
        {
            this.Close = close;
            updated = true;
        }
        return updated;
    }
}

#endregion Nested