﻿using System;

using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

using R5T.Corcyra;

using AppType = R5T.Corcyra.Catchment;
using AppTypeGeoJson = R5T.Corcyra.CatchmentGeoJson;
using EntityType = R5T.Borgue.Database.Entities.Catchment;


namespace R5T.Borgue.Database
{
    public static class CatchmentExtensions
    {
        public static AppType ToAppType(this EntityType entityType)
        {
            var appType = new AppType()
            {
                Identity =  CatchmentIdentity.From(entityType.Identity),
                Name = entityType.Name,
            };

            var lngLats = entityType.Boundary.ToLngLats();

            appType.Boundary.AddRange(lngLats);

            return appType;
        }

        public static AppTypeGeoJson ToAppTypeGeoJson(this EntityType entityType)
        {
            var appType = new AppTypeGeoJson()
            {
                Identity = CatchmentIdentity.From(entityType.Identity),
                Name = entityType.Name,
                MultiPolygonGeoJsonText = (entityType.Boundary as MultiPolygon).ToGeoJsonMultiPolygonJsonString(),
            };
            return appType;
        }

        public static EntityType ToEntityType(this AppType appType, IGeometryFactory geometryFactory)
        {
            var polygon = appType.Boundary.ToPolygon(geometryFactory);

            var entity = new EntityType()
            {
                Identity = appType.Identity.Value,
                Name = appType.Name,
                Boundary = polygon,
            };

            return entity;
        }
    }
}
