﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TempLoggerService
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class temperaturelogEntities : DbContext
    {
        public temperaturelogEntities()
            : base("name=temperaturelogEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<device> devices { get; set; }
        public virtual DbSet<temperature> temperatures { get; set; }
        public virtual DbSet<@event> events { get; set; }
    
        public virtual ObjectResult<GetTempRange_Result> GetTempRange(Nullable<System.Guid> device, Nullable<System.DateTime> start, Nullable<System.DateTime> end)
        {
            var deviceParameter = device.HasValue ?
                new ObjectParameter("device", device) :
                new ObjectParameter("device", typeof(System.Guid));
    
            var startParameter = start.HasValue ?
                new ObjectParameter("start", start) :
                new ObjectParameter("start", typeof(System.DateTime));
    
            var endParameter = end.HasValue ?
                new ObjectParameter("end", end) :
                new ObjectParameter("end", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetTempRange_Result>("GetTempRange", deviceParameter, startParameter, endParameter);
        }
    
        public virtual ObjectResult<GetTempRange_Result> GetTempsLast24Hours(Nullable<System.Guid> device)
        {
            var deviceParameter = device.HasValue ?
                new ObjectParameter("device", device) :
                new ObjectParameter("device", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetTempRange_Result>("GetTempsLast24Hours", deviceParameter);
        }
    
        public virtual ObjectResult<GetLatestTemp_Result> GetLatestTemp(Nullable<System.Guid> deviceID)
        {
            var deviceIDParameter = deviceID.HasValue ?
                new ObjectParameter("deviceID", deviceID) :
                new ObjectParameter("deviceID", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetLatestTemp_Result>("GetLatestTemp", deviceIDParameter);
        }
    }
}
