# Copilot Instructions

## Project Guidelines
- For the Blazor dashboard landing page, prioritize lightweight implementations and avoid changes that make the initial app load feel heavy.
- On the landing page, default to a Home-style quick access view and only show the full dashboard after the user explicitly clicks Dashboard.
- Use a variable/source of truth for the company name and reuse it in the layout/menu instead of hardcoding it; move login/user display into the menu area.
- For grid search UX, include a quick clear 'x' button to reset the search text.
- Do not add a DistCode field to the customer and supplier database-backed models, as it is not applicable.
- Preserve functional preview actions during UI cleanup; for this page, 'Display Rekap' should remain available inside the export section rather than being removed.
- When comparing projects, always verify the actual files from the requested path and do not rely on results from the navigator/workspace that may point to different repositories; note that D:\Project\BMA-PT uses .NET 10 and its Program.cs is different.

## Service Refactoring Guidelines
- When refactoring `OrderPurchaseServices`, preserve original intent: `AddTransH` passes header currency (`trans.Currency`) to item price updates; `EditTransH` originally also passed `trans.Currency`, so helpers should allow an explicit currency override.
- Validate that refactored service logic is identical to the original before accepting changes. Continue to perform side-by-side equivalence checks when refactoring and explicitly flag any behavioral differences.

## Export Formatting Guidelines
- When user requests Excel formatting changes, apply them to the export used by LaporanCurrentStock if explicitly specified, not to other report exports.