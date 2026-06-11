using Microsoft.AspNetCore.Components;
using SomaShare.Components.Model;
using SomaShare.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

public partial class Dashboard : ComponentBase
{
    [Inject]
    private IWantedAdService WantedAdService { get; set; } = default!;
    [Inject]
    private IListingService ListingService { get; set; } = default!;
    [Inject]
    private UserSession UserSession { get; set; } = default!;
    [Inject]
    private SomaContext Context { get; set; } = default!;
    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    // Models bound to the forms
    private ListingAd newListing = new ListingAd
    {
        Textbook = new Textbook(),
        User_Id = "" // will be set from session
    };

    private WantedAd newWantedAd = new WantedAd
    {
        Textbook = new Textbook(),
        User_Id = "" // will be set from session
    };

    // Called when the listing form is submitted
    private async Task CreateListing()
    {
        newListing.User_Id = this.UserSession.UserId;
        await this.ListingService.CreateListingAsync(newListing.Textbook, newListing);
        // Reset form
        newListing = new ListingAd { Textbook = new Textbook(), User_Id = this.UserSession.UserId };
    }

    protected override async Task OnInitializedAsync()
    {
        // Try to restore identity id from browser sessionStorage if UserSession doesn't yet have it
        try
        {
            if (string.IsNullOrEmpty(UserSession.UserId))
            {
                try
                {
                    var stored = await JS.InvokeAsync<string>("sessionStorage.getItem", "User_Id");
                    if (!string.IsNullOrEmpty(stored)) UserSession.User_Id = stored;
                }
                catch
                {
                    // ignore JS errors
                }
            }

            // If CurrentUser is null but we have a stored identity id, try to load the User from the DB
            if (UserSession.CurrentUser == null && !string.IsNullOrEmpty(UserSession.User_Id))
            {
                try
                {
                    var user = await Context.Users.FirstOrDefaultAsync(u => u.Id == UserSession.User_Id);
                    if (user != null)
                    {
                        UserSession.CurrentUser = user;
                        UserSession.CurrentUserId = user.User_Id;
                        UserSession.CurrentUserRoleId = user.Role_Id;
                    }
                }
                catch
                {
                    // ignore DB errors
                }
            }
        }
        catch
        {
            // swallow any unexpected errors during restore
        }

        // Ensure default form models have the current user's id where possible
        newListing.User_Id = UserSession.UserId ?? string.Empty;
        newWantedAd.User_Id = UserSession.UserId ?? string.Empty;
    }

    // Called when the wanted ad form is submitted
    private async Task CreateWantedAd()
    {
        newWantedAd.User_Id = this.UserSession.UserId;
        await this.WantedAdService.CreateWantedAdAsync(newWantedAd.Textbook, newWantedAd);
        // Reset form
        newWantedAd = new WantedAd { Textbook = new Textbook(), User_Id = this.UserSession.UserId };
    }
}
