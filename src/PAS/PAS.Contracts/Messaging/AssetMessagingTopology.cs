namespace PAS.Contracts.Messaging;

public static class AssetMessagingTopology {
    public const string Exchange = "pas.asset.events";
    public const string FundNavAddedRoutingKey = "fund.nav.added.v1";
    public const string FundSoftDeleteRoutingKey = "fund.soft.delete.v1";
    public const string FundNavSoftDeleteRoutingKey = "fund.nav.soft.delete.v1";
}