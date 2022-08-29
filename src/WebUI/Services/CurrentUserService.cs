using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Enums;
using Location = CleanArchitecture.Domain.Enums.LocationCode;
using System.Security.Claims;

namespace CleanArchitecture.WebUI.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public const string LOCATION_HEADER = "X-API-LOCATION";

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public int? LocationHeaderId
    {
        get
        {
            var locationHeader = _httpContextAccessor.HttpContext?.Request?.Headers?[LOCATION_HEADER];
            if (string.IsNullOrEmpty(locationHeader))
                return null;
            else
                return int.Parse(locationHeader);
        }
    }

    public int? LocationId
    {
        get
        {
            //Convert Iowa to Evansville since Iowa is a virutal location
            if (LocationHeaderId == LocationID.Iowa)
                return LocationID.Evansville;
            return LocationHeaderId;
        }
    }

    public string Schema
    {
        get
        {
            switch (LocationHeaderId)
            {
                case LocationID.Evansville:
                case LocationID.Iowa:
                    return Schemas.Evansville;
                case LocationID.Mexico:
                    return Schemas.Mexico;
                case LocationID.Olney:
                    return Schemas.Olney;
                case LocationID.Gainesville:
                    return Schemas.Gainesville;
                case LocationID.StPaul:
                    return Schemas.StPaul;
                case LocationID.Bristol:
                    return Schemas.Bristol;
                case LocationID.Orlando:
                    return Schemas.Orlando;
                default: return Schemas.Evansville;
            }
        }
    }

    public string LocationCode
    {
        get
        {
            switch (LocationHeaderId)
            {
                case LocationID.Evansville:
                case LocationID.Iowa:
                    return Location.Evansville;
                case LocationID.Mexico:
                    return Location.Mexico;
                case LocationID.Olney:
                    return Location.Olney;
                case LocationID.Gainesville:
                    return Location.Gainesville;
                case LocationID.StPaul:
                    return Location.StPaul;
                case LocationID.Bristol:
                    return Location.Bristol;
                case LocationID.Orlando:
                    return Location.Orlando;
                default: return Location.Evansville;
            }
        }
    }

    public string ShipFromWarehouse
    {
        get
        {
            switch (LocationHeaderId)
            {
                case LocationID.Iowa:
                    return "IWA";
                default: return null;
            }
        }
    }
    public string OriginHeader
    {
        get
        {
            var originHeader = _httpContextAccessor.HttpContext?.Request?.Headers?["ORIGIN"];
            if (string.IsNullOrEmpty(originHeader))
                return "";
            else
                return originHeader.Value.ToString().ToLower();
        }
    }
}
