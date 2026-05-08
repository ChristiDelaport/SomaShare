using Microsoft.AspNetCore.Components;
using SomaShare.Components.Model;
using SomaShare.Services;

public partial class Dashboard : ComponentBase
{
    [Inject]
    private IWantedAdService WantedAdService { get; set; } = default!;
    [Inject]
    private IListingService ListingService { get; set; } = default!;
    [Inject]
    private UserSession UserSession { get; set; } = default!;

    // Models bound to the forms
    private ListingAd newListing = new ListingAd
    {
        Textbook = new Textbook(),
        User_ID = "" // will be set from session
    };

    private WantedAd newWantedAd = new WantedAd
    {
        Textbook = new Textbook(),
        User_ID = "" // will be set from session
    };

    // Called when the listing form is submitted
    private async Task CreateListing()
    {
        newListing.User_ID = this.UserSession.UserId;
        await this.ListingService.CreateListingAsync(newListing.Textbook, newListing);
        // Reset form
        newListing = new ListingAd { Textbook = new Textbook(), User_ID = this.UserSession.UserId };
    }

    // Called when the wanted ad form is submitted
    private async Task CreateWantedAd()
    {
        newWantedAd.User_ID = this.UserSession.UserId;
        await this.WantedAdService.CreateWantedAdAsync(newWantedAd.Textbook, newWantedAd);
        // Reset form
        newWantedAd = new WantedAd { Textbook = new Textbook(), User_ID = this.UserSession.UserId };
    }
}
