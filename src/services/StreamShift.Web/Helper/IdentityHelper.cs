namespace StreamShift.Web
{
    public static class IdentityHelper
    {
        public static string GetIdClaim(this System.Security.Claims.ClaimsPrincipal _user)
        {
            var id = _user.Claims.FirstOrDefault(a => a.Type == "ID")?.Value;
            try
            {
                if (id == null)
                {
                    id = _user.Claims.FirstOrDefault(a => a.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                }
                if (id == null)
                {
                    var _id = _user.Claims.ToList()[0].Value;
                    if (_id != null) id = _id;
                }
            }
            catch
            {
            }
            return id;
        }
    }
}