using EventManager1.DBCon;
using EventManager1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManager1.Areas.Organizer.Models
{
    public class HandleBanner
    {
        EventmanagerEntities db = new EventmanagerEntities();
        public Banner getBanner(int id)
        {
            Banner res = new Banner();
            try
            {
                res = (from r in db.BannerTimers
                       where r.Id == id && r.Bannerforwebsite == (int)Bannerforwebsite.Stream233 
                       select new Banner
                       {
                           ImagePath = r.ImagePath,
                           EndDate = r.EndDate,
                           EndTime = r.EndTime,
                           ID = r.Id,
                           IsImageEnble = r.IsImageEnble,
                           IsTimeEnable = r.IsTimeEnable,
                           StartDate = r.StartDate,
                           StartTime = r.StartTime
                       }).FirstOrDefault();
            }
            catch (Exception ex)
            { CusomlogWriter.Loghacksawgaming("Error Log:" + ex, "Error Log"); }
            return res;
        }
        public List<Banner> getBannerList()
        {
            List<Banner> res = new List<Banner>();
            try
            {
                res = (from r in db.BannerTimers
                       where r.Bannerforwebsite == (int)Bannerforwebsite.Stream233 
                       select new Banner
                       {
                           ImagePath = r.ImagePath,
                           EndDate = r.EndDate,
                           EndTime = r.EndTime,
                           ID = r.Id,
                           IsImageEnble = r.IsImageEnble,
                           IsTimeEnable = r.IsTimeEnable,
                           StartDate = r.StartDate,
                           StartTime = r.StartTime
                       }).ToList();
            }
            catch (Exception ex)
            { }
            return res;
        }
        public int SaveBanner(Banner bt)
        {
            if (bt != null)
            {
                try
                {
                    if (bt.ID > 0)
                    {
                        BannerTimer tt = db.BannerTimers.Where(id => id.Id == bt.ID).FirstOrDefault();
                        tt.ImagePath = bt.ImagePath;
                        tt.StartDate = bt.StartDate;
                        tt.EndDate = bt.EndDate;
                        tt.StartTime = bt.StartTime.Replace(" ", string.Empty);
                        tt.EndTime = bt.EndTime.Replace(" ", string.Empty);
                        tt.IsTimeEnable = bt.IsTimeEnable;
                        tt.Bannerforwebsite = (int)Bannerforwebsite.Stream233;
                        if (db.SaveChanges() > 0)
                        {
                            return 1;
                        }
                    }
                    else
                    {
                        BannerTimer tt = new BannerTimer();
                        var Id = db.BannerTimers.Count();
                        if (Id > 0)
                        {
                            Id = db.BannerTimers.Max(x => x.Id);
                        }
                        else
                        {
                            Id = 0;
                        }
                        tt.Id = Id + 1;
                        tt.ImagePath = bt.ImagePath;
                        tt.StartDate = bt.StartDate;
                        tt.EndDate = bt.EndDate;
                        tt.StartTime = bt.StartTime.Replace(" ", string.Empty);
                        tt.EndTime = bt.EndTime.Replace(" ", string.Empty);
                        tt.IsTimeEnable = bt.IsTimeEnable;
                        tt.Bannerforwebsite =(int)Bannerforwebsite.Stream233;
                        db.BannerTimers.Add(tt);
                        if (db.SaveChanges() > 0)
                        {
                            return 1;
                        }
                    }
                }
                catch (Exception ex)
                { }
            }
            return 0;
        }
    }
    public class Banner
    {
        public int ID { get; set; }
        public string ImagePath { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int? IsTimeEnable { get; set; }
        public int? IsImageEnble { get; set; }
        //public HttpPostedFileBase Image { get; set; }

    }
}