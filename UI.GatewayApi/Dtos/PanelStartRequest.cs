namespace UI.GatewayApi.Dtos
{
    public sealed class PanelStartRequest
    {
        public string PanelId { get; set; } = default!;
        public string LotId { get; set; } = default!;
        public int FieldCount { get; set; }
        public string RecipeId { get; set; } = default!;
    }
}
